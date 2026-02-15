using AutoMapper;
using ScimProvisioning.Application.DTOs;
using ScimProvisioning.Core.Common;
using ScimProvisioning.Core.Interfaces;

namespace ScimProvisioning.Application.UseCases.Users;

/// <summary>
/// Use case for listing SCIM users with filtering and pagination
/// </summary>
public class ListUsersUseCase
{
    private readonly IScimUserRepository _userRepository;
    private readonly IMapper _mapper;

    public ListUsersUseCase(IScimUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<ListUsersResponse>> ExecuteAsync(
        int startIndex = 0,
        int count = 100,
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var (users, totalCount) = await _userRepository.ListAsync(startIndex, count, filter, cancellationToken);

        var response = new ListUsersResponse
        {
            TotalResults = totalCount,
            StartIndex = startIndex,
            ItemsPerPage = count,
            Resources = _mapper.Map<List<UserResponse>>(users)
        };

        return Result.Success(response);
    }
}
