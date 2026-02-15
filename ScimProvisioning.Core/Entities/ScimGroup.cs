using ScimProvisioning.Core.Common;
using ScimProvisioning.Core.Events;

namespace ScimProvisioning.Core.Entities;

/// <summary>
/// Represents a SCIM Group aggregate root
/// </summary>
public class ScimGroup : BaseEntity
{
    private readonly List<GroupMember> _members = new();

    /// <summary>
    /// Gets the external identifier for the group (from source system)
    /// </summary>
    public string ExternalId { get; private set; }

    /// <summary>
    /// Gets the display name
    /// </summary>
    public string DisplayName { get; private set; }

    /// <summary>
    /// Gets the group members
    /// </summary>
    public IReadOnlyCollection<GroupMember> Members => _members.AsReadOnly();

    // EF Core constructor
    private ScimGroup() { }

    private ScimGroup(string externalId, string displayName)
    {
        Id = Guid.NewGuid();
        ExternalId = externalId;
        DisplayName = displayName;
        CreatedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new SCIM group
    /// </summary>
    public static Result<ScimGroup> Create(string externalId, string displayName)
    {
        if (string.IsNullOrWhiteSpace(externalId))
            return Result.Failure<ScimGroup>("External ID is required");

        if (string.IsNullOrWhiteSpace(displayName))
            return Result.Failure<ScimGroup>("Display name is required");

        var group = new ScimGroup(externalId, displayName);
        group.AddDomainEvent(new GroupProvisionedDomainEvent(group.Id, externalId, displayName));

        return Result.Success(group);
    }

    /// <summary>
    /// Updates the group's information
    /// </summary>
    public Result Update(string? displayName = null)
    {
        if (!string.IsNullOrWhiteSpace(displayName))
            DisplayName = displayName;

        ModifiedAt = DateTime.UtcNow;
        AddDomainEvent(new GroupUpdatedDomainEvent(Id, ExternalId, DisplayName));

        return Result.Success();
    }

    /// <summary>
    /// Marks the group as deleted
    /// </summary>
    public void Delete()
    {
        ModifiedAt = DateTime.UtcNow;
        AddDomainEvent(new GroupDeletedDomainEvent(Id, ExternalId, DisplayName));
    }

    /// <summary>
    /// Adds a member to the group
    /// </summary>
    public Result AddMember(Guid userId, string displayName)
    {
        if (_members.Any(m => m.UserId == userId))
            return Result.Failure("Member already exists in the group");

        _members.Add(new GroupMember(Id, userId, displayName));
        ModifiedAt = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Removes a member from the group
    /// </summary>
    public Result RemoveMember(Guid userId)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member == null)
            return Result.Failure("Member not found in the group");

        _members.Remove(member);
        ModifiedAt = DateTime.UtcNow;

        return Result.Success();
    }
}

/// <summary>
/// Represents a group member
/// </summary>
public class GroupMember
{
    /// <summary>
    /// Gets the group identifier
    /// </summary>
    public Guid GroupId { get; private set; }

    /// <summary>
    /// Gets the user identifier
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the member's display name
    /// </summary>
    public string DisplayName { get; private set; }

    /// <summary>
    /// Gets the date when the member was added
    /// </summary>
    public DateTime AddedAt { get; private set; }

    // EF Core constructor
    private GroupMember() { }

    internal GroupMember(Guid groupId, Guid userId, string displayName)
    {
        GroupId = groupId;
        UserId = userId;
        DisplayName = displayName;
        AddedAt = DateTime.UtcNow;
    }
}
