using AutoMapper;
using ScimProvisioning.Application.DTOs;
using ScimProvisioning.Core.Common;
using ScimProvisioning.Core.Interfaces;

namespace ScimProvisioning.Application.UseCases.Users;

/// <summary>
/// Use case for getting a SCIM user by ID
/// </summary>
public class GetUserByIdUseCase
{
    private readonly IScimUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdUseCase(IScimUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserResponse>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            return Result.Failure<UserResponse>("User not found");

        var response = _mapper.Map<UserResponse>(user);
        return Result.Success(response);
    }
}
