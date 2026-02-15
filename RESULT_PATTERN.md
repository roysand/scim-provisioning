# Result Pattern Implementation Guide

This document explains how the Result Pattern has been implemented in the SCIM Provisioning API.

## Overview

The Result Pattern is an architectural pattern that provides a structured way to handle operation results, including both success and failure cases. This eliminates exceptions for control flow and provides a more consistent, predictable API response structure.

## Core Components

### 1. Result Classes (ScimProvisioning.Core.Common)

The foundation of the pattern is in `Result.cs`:

```csharp
// Base result class (no return value)
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
    
    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);
}

// Generic result class (with return value)
public class Result<T> : Result
{
    public T Value { get; }
    
    public static Result<T> Success<T>(T value) => new(value, true, string.Empty);
    public static Result<T> Failure<T>(string error) => new(default!, false, error);
}
```

**Key Features:**
- Enforces validity: Success results cannot have errors, and failure results must have errors
- Type-safe: Use `Result<T>` for operations that return values
- Immutable: Properties are read-only, making instances thread-safe

### 2. API Response DTOs (ScimProvisioning.Application.DTOs)

Three new response wrappers were added in `ApiResponseDtos.cs`:

#### ApiResponse<T> - Single Resource Response
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
}
```

Usage:
```csharp
var response = new ApiResponse<UserResponse>(user, "User created successfully");
await SendOkAsync(response, ct);
```

#### ApiErrorResponse - Error Response
```csharp
public class ApiErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; }
    public string Code { get; set; }
    public Dictionary<string, string[]> Errors { get; set; }
}
```

#### ApiPagedResponse<T> - Paginated Response
```csharp
public class ApiPagedResponse<T>
{
    public bool Success { get; set; }
    public List<T> Data { get; set; }
    public PaginationMetadata Pagination { get; set; }
    public string Message { get; set; }
}
```

Usage:
```csharp
var response = new ApiPagedResponse<UserResponse>(
    users,
    totalResults: 100,
    startIndex: 0,
    itemsPerPage: 50,
    "Users retrieved successfully");
await SendOkAsync(response, ct);
```

## Usage in Endpoints

### Pattern Flow

1. **Use Case returns Result<T>**
```csharp
// In CreateUserUseCase
public async Task<Result<UserResponse>> ExecuteAsync(
    CreateUserRequest request,
    CancellationToken cancellationToken = default)
{
    // ... validation logic ...
    if (userResult.IsFailure)
        return Result.Failure<UserResponse>(userResult.Error);
    
    // ... processing logic ...
    return Result.Success(response);
}
```

2. **Endpoint handles Result and wraps in ApiResponse**
```csharp
public class CreateUserEndpoint : Endpoint<CreateUserRequest, ApiResponse<UserResponse>>
{
    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var result = await _useCase.ExecuteAsync(req, ct);
        
        if (result.IsFailure)
        {
            ThrowError(r =>
            {
                r.StatusCode = 400;
                r.Message = result.Error;
            });
        }
        
        var response = new ApiResponse<UserResponse>(result.Value, "User created successfully");
        await SendCreatedAtAsync<GetUserByIdEndpoint>(
            new { id = result.Value.Id },
            response,
            cancellation: ct);
    }
}
```

### Updated Endpoints

All endpoints have been updated to return wrapped responses:

| Endpoint | Success Response | Error Response |
|----------|-----------------|----------------|
| POST /scim/v2/Users | ApiResponse<UserResponse> (201) | ApiErrorResponse (400) |
| GET /scim/v2/Users/{id} | ApiResponse<UserResponse> (200) | ApiErrorResponse (404) |
| PATCH /scim/v2/Users/{id} | ApiResponse<UserResponse> (200) | ApiErrorResponse (400/404) |
| DELETE /scim/v2/Users/{id} | ApiResponse<string> (200) | ApiErrorResponse (404) |
| GET /scim/v2/Users | ApiPagedResponse<UserResponse> (200) | - |
| POST /scim/v2/Groups | ApiResponse<GroupResponse> (201) | ApiErrorResponse (400) |
| GET /scim/v2/Groups/{id} | ApiResponse<GroupResponse> (200) | ApiErrorResponse (404) |
| GET /scim/v2/Groups | ApiPagedResponse<GroupResponse> (200) | - |

## Benefits

1. **Consistency**: All API responses follow the same structure
2. **Type Safety**: Compile-time checking prevents errors
3. **Predictability**: Clients know exactly what to expect
4. **Reduced Exceptions**: No exception-based control flow
5. **Clear Intent**: Code explicitly shows success/failure paths
6. **Better Error Handling**: Standardized error messages and codes

## Example API Responses

### Success Response (200)
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "externalId": "user123",
    "userName": "john.doe",
    "displayName": "John Doe",
    "primaryEmail": "john@example.com",
    "active": true,
    "createdAt": "2024-01-15T10:30:00Z",
    "modifiedAt": "2024-01-15T10:30:00Z"
  },
  "message": "User retrieved successfully"
}
```

### List Response (200)
```json
{
  "success": true,
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "externalId": "user123",
      "userName": "john.doe",
      ...
    }
  ],
  "pagination": {
    "totalResults": 150,
    "startIndex": 0,
    "itemsPerPage": 50
  },
  "message": "Users retrieved successfully"
}
```

### Error Response (400)
```json
{
  "success": false,
  "message": "User with this external ID already exists",
  "code": "CONFLICT",
  "errors": {}
}
```

## Best Practices

1. **Always check IsFailure before accessing Value**
```csharp
var result = await _useCase.ExecuteAsync(request, ct);
if (result.IsFailure)
    return Result.Failure<T>(result.Error);

var value = result.Value; // Safe to access
```

2. **Use meaningful error messages**
```csharp
return Result.Failure<T>("User with this external ID already exists");
```

3. **Chain Results in pipelines**
```csharp
var userResult = ScimUser.Create(/*params*/);
if (userResult.IsFailure)
    return Result.Failure<UserResponse>(userResult.Error);

var savedResult = await _repository.AddAsync(userResult.Value);
if (savedResult.IsFailure)
    return Result.Failure<UserResponse>(savedResult.Error);
```

4. **Provide context in error messages**
```csharp
// Good
return Result.Failure($"Failed to create user: {ex.Message}");

// Less helpful
return Result.Failure("An error occurred");
```

## Migration Checklist

When adding new endpoints or use cases, ensure:

- [ ] Use Case returns `Result<T>`
- [ ] Endpoint returns `ApiResponse<T>` or `ApiPagedResponse<T>`
- [ ] Handle failures with appropriate HTTP status codes
- [ ] Include meaningful error messages
- [ ] Update OpenAPI descriptions with response types
- [ ] Test both success and failure paths

## Resources

- **Result Pattern**: https://github.com/dotnet-architecture/eShopOnContainers
- **SCIM Protocol**: https://tools.ietf.org/html/rfc7643
- **Error Handling in .NET**: https://docs.microsoft.com/en-us/dotnet/standard/exceptions/

