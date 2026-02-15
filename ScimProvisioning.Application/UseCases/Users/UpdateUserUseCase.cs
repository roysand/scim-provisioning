using AutoMapper;
using ScimProvisioning.Application.DTOs;
using ScimProvisioning.Application.Services;
using ScimProvisioning.Core.Common;
using ScimProvisioning.Core.Interfaces;

namespace ScimProvisioning.Application.UseCases.Users;

/// <summary>
/// Use case for updating a SCIM user
/// </summary>
public class UpdateUserUseCase
{
    private readonly IScimUserRepository _userRepository;
    private readonly IOutboxService _outboxService;
    private readonly IAuditLogService _auditLogService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateUserUseCase(
        IScimUserRepository userRepository,
        IOutboxService outboxService,
        IAuditLogService auditLogService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _outboxService = outboxService;
        _auditLogService = auditLogService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserResponse>> ExecuteAsync(
        Guid userId,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            return Result.Failure<UserResponse>("User not found");

        var updateResult = user.Update(request.DisplayName, request.PrimaryEmail, request.Active);
        if (updateResult.IsFailure)
            return Result.Failure<UserResponse>(updateResult.Error);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            _userRepository.Update(user);

            await _outboxService.PublishDomainEventsAsync(user.DomainEvents, cancellationToken: cancellationToken);

            var auditLog = await _auditLogService.CreateAuditLogAsync(
                "Updated",
                "User",
                user.Id,
                changeDetails: System.Text.Json.JsonSerializer.Serialize(request),
                cancellationToken: cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            await _auditLogService.MarkAsSuccessAsync(auditLog.Id, cancellationToken);

            user.ClearDomainEvents();

            var response = _mapper.Map<UserResponse>(user);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure<UserResponse>($"Failed to update user: {ex.Message}");
        }
    }
}
