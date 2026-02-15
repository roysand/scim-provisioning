namespace ScimProvisioning.Application.DTOs;

/// <summary>
/// Request DTO for creating a SCIM group
/// </summary>
public class CreateGroupRequest
{
    public string ExternalId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for updating a SCIM group
/// </summary>
public class UpdateGroupRequest
{
    public string? DisplayName { get; set; }
}

/// <summary>
/// DTO for adding a member to a group
/// </summary>
public class AddGroupMemberRequest
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for a SCIM group
/// </summary>
public class GroupResponse
{
    public Guid Id { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<GroupMemberDto> Members { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

/// <summary>
/// DTO for a group member
/// </summary>
public class GroupMemberDto
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}

/// <summary>
/// List response with pagination
/// </summary>
public class ListGroupsResponse
{
    public int TotalResults { get; set; }
    public int StartIndex { get; set; }
    public int ItemsPerPage { get; set; }
    public List<GroupResponse> Resources { get; set; } = new();
}
