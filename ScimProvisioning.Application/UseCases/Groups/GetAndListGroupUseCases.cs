using AutoMapper;
using ScimProvisioning.Application.DTOs;
using ScimProvisioning.Core.Common;
using ScimProvisioning.Core.Interfaces;

namespace ScimProvisioning.Application.UseCases.Groups;

/// <summary>
/// Use case for getting a SCIM group by ID
/// </summary>
public class GetGroupByIdUseCase
{
    private readonly IScimGroupRepository _groupRepository;
    private readonly IMapper _mapper;

    public GetGroupByIdUseCase(IScimGroupRepository groupRepository, IMapper mapper)
    {
        _groupRepository = groupRepository;
        _mapper = mapper;
    }

    public async Task<Result<GroupResponse>> ExecuteAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var group = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        if (group == null)
            return Result.Failure<GroupResponse>("Group not found");

        var response = _mapper.Map<GroupResponse>(group);
        return Result.Success(response);
    }
}

/// <summary>
/// Use case for listing SCIM groups with filtering and pagination
/// </summary>
public class ListGroupsUseCase
{
    private readonly IScimGroupRepository _groupRepository;
    private readonly IMapper _mapper;

    public ListGroupsUseCase(IScimGroupRepository groupRepository, IMapper mapper)
    {
        _groupRepository = groupRepository;
        _mapper = mapper;
    }

    public async Task<Result<ListGroupsResponse>> ExecuteAsync(
        int startIndex = 0,
        int count = 100,
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var (groups, totalCount) = await _groupRepository.ListAsync(startIndex, count, filter, cancellationToken);

        var response = new ListGroupsResponse
        {
            TotalResults = totalCount,
            StartIndex = startIndex,
            ItemsPerPage = count,
            Resources = _mapper.Map<List<GroupResponse>>(groups)
        };

        return Result.Success(response);
    }
}
