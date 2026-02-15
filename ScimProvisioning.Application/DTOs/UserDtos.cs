namespace ScimProvisioning.Application.DTOs;

/// <summary>
/// Request DTO for creating a SCIM user
/// </summary>
public class CreateUserRequest
{
    public string ExternalId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PrimaryEmail { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
}

/// <summary>
/// Request DTO for updating a SCIM user
/// </summary>
public class UpdateUserRequest
{
    public string? DisplayName { get; set; }
    public string? PrimaryEmail { get; set; }
    public bool? Active { get; set; }
}

/// <summary>
/// Response DTO for a SCIM user
/// </summary>
public class UserResponse
{
    public Guid Id { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PrimaryEmail { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

/// <summary>
/// List response with pagination
/// </summary>
public class ListUsersResponse
{
    public int TotalResults { get; set; }
    public int StartIndex { get; set; }
    public int ItemsPerPage { get; set; }
    public List<UserResponse> Resources { get; set; } = new();
}
