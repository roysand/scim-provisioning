using ScimProvisioning.Core.Entities;

namespace ScimProvisioning.Core.Interfaces;

/// <summary>
/// Repository interface for SCIM groups
/// </summary>
public interface IScimGroupRepository
{
    /// <summary>
    /// Gets a group by identifier
    /// </summary>
    Task<ScimGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a group by external identifier
    /// </summary>
    Task<ScimGroup?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all groups with optional filtering
    /// </summary>
    Task<(IEnumerable<ScimGroup> Groups, int TotalCount)> ListAsync(
        int startIndex = 0,
        int count = 100,
        string? filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a group
    /// </summary>
    Task AddAsync(ScimGroup group, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a group
    /// </summary>
    void Update(ScimGroup group);

    /// <summary>
    /// Deletes a group
    /// </summary>
    void Delete(ScimGroup group);
}
