using Microsoft.EntityFrameworkCore;
using ScimProvisioning.Core.Entities;
using ScimProvisioning.Core.Interfaces;
using ScimProvisioning.Infrastructure.Persistence;

namespace ScimProvisioning.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for SCIM users
/// </summary>
public class ScimUserRepository : IScimUserRepository
{
    private readonly ScimProvisioningDbContext _context;

    public ScimUserRepository(ScimProvisioningDbContext context)
    {
        _context = context;
    }

    public async Task<ScimUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<ScimUser?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.ExternalId == externalId, cancellationToken);
    }

    public async Task<ScimUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }

    public async Task<(IEnumerable<ScimUser> Users, int TotalCount)> ListAsync(
        int startIndex = 0,
        int count = 100,
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users.AsQueryable();

        // Apply basic filtering (simplified - full SCIM filter parsing would be more complex)
        if (!string.IsNullOrWhiteSpace(filter))
        {
            // Simple filter support (e.g., "userName eq 'john'")
            if (filter.Contains("userName", StringComparison.OrdinalIgnoreCase))
            {
                var value = ExtractFilterValue(filter);
                if (!string.IsNullOrEmpty(value))
                    query = query.Where(u => u.UserName.Contains(value));
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderBy(u => u.CreatedAt)
            .Skip(startIndex)
            .Take(count)
            .ToListAsync(cancellationToken);

        return (users, totalCount);
    }

    public async Task AddAsync(ScimUser user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public void Update(ScimUser user)
    {
        _context.Users.Update(user);
    }

    public void Delete(ScimUser user)
    {
        _context.Users.Remove(user);
    }

    private string? ExtractFilterValue(string filter)
    {
        // Simplified filter value extraction
        var parts = filter.Split(new[] { "eq", "==" }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
        {
            return parts[1].Trim().Trim('\'', '"');
        }
        return null;
    }
}
