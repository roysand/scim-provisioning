using ScimProvisioning.Core.Entities;

namespace ScimProvisioning.Core.Interfaces;

/// <summary>
/// Repository interface for SCIM users
/// </summary>
public interface IScimUserRepository
{
    /// <summary>
    /// Gets a user by identifier
    /// </summary>
    Task<ScimUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by external identifier
    /// </summary>
    Task<ScimUser?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by username
    /// </summary>
    Task<ScimUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all users with optional filtering
    /// </summary>
    Task<(IEnumerable<ScimUser> Users, int TotalCount)> ListAsync(
        int startIndex = 0,
        int count = 100,
        string? filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a user
    /// </summary>
    Task AddAsync(ScimUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user
    /// </summary>
    void Update(ScimUser user);

    /// <summary>
    /// Deletes a user
    /// </summary>
    void Delete(ScimUser user);
}
