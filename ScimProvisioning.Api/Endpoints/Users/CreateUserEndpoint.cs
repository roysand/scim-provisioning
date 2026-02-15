using FastEndpoints;
using ScimProvisioning.Application.DTOs;
using ScimProvisioning.Application.UseCases.Users;

namespace ScimProvisioning.Api.Endpoints.Users;

/// <summary>
/// Endpoint for creating a SCIM user
/// </summary>
public class CreateUserEndpoint : Endpoint<CreateUserRequest, ApiResponse<UserResponse>>
{
    private readonly CreateUserUseCase _useCase;

    public CreateUserEndpoint(CreateUserUseCase useCase)
    {
        _useCase = useCase;
    }

    public override void Configure()
    {
        Post("/scim/v2/Users");
        AllowAnonymous();
        Description(d => d
            .Produces<ApiResponse<UserResponse>>(201)
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(409)
            .WithTags("Users"));
    }

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
