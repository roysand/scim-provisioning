using Microsoft.EntityFrameworkCore;
using ScimProvisioning.Core.Entities;
using ScimProvisioning.Core.Interfaces;
using ScimProvisioning.Infrastructure.Persistence;

namespace ScimProvisioning.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for SCIM groups
/// </summary>
public class ScimGroupRepository : IScimGroupRepository
{
    private readonly ScimProvisioningDbContext _context;

    public ScimGroupRepository(ScimProvisioningDbContext context)
    {
        _context = context;
    }

    public async Task<ScimGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<ScimGroup?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.ExternalId == externalId, cancellationToken);
    }

    public async Task<(IEnumerable<ScimGroup> Groups, int TotalCount)> ListAsync(
        int startIndex = 0,
        int count = 100,
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Groups.Include(g => g.Members).AsQueryable();

        // Apply basic filtering
        if (!string.IsNullOrWhiteSpace(filter))
        {
            if (filter.Contains("displayName", StringComparison.OrdinalIgnoreCase))
            {
                var value = ExtractFilterValue(filter);
                if (!string.IsNullOrEmpty(value))
                    query = query.Where(g => g.DisplayName.Contains(value));
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var groups = await query
            .OrderBy(g => g.CreatedAt)
            .Skip(startIndex)
            .Take(count)
            .ToListAsync(cancellationToken);

        return (groups, totalCount);
    }

    public async Task AddAsync(ScimGroup group, CancellationToken cancellationToken = default)
    {
        await _context.Groups.AddAsync(group, cancellationToken);
    }

    public void Update(ScimGroup group)
    {
        _context.Groups.Update(group);
    }

    public void Delete(ScimGroup group)
    {
        _context.Groups.Remove(group);
    }

    private string? ExtractFilterValue(string filter)
    {
        var parts = filter.Split(new[] { "eq", "==" }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
        {
            return parts[1].Trim().Trim('\'', '"');
        }
        return null;
    }
}
