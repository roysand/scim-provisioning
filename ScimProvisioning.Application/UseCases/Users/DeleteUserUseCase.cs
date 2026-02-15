using ScimProvisioning.Application.Services;
using ScimProvisioning.Core.Common;
using ScimProvisioning.Core.Interfaces;

namespace ScimProvisioning.Application.UseCases.Users;

/// <summary>
/// Use case for deleting a SCIM user
/// </summary>
public class DeleteUserUseCase
{
    private readonly IScimUserRepository _userRepository;
    private readonly IOutboxService _outboxService;
    private readonly IAuditLogService _auditLogService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserUseCase(
        IScimUserRepository userRepository,
        IOutboxService outboxService,
        IAuditLogService auditLogService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _outboxService = outboxService;
        _auditLogService = auditLogService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            return Result.Failure("User not found");

        user.Delete();

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            _userRepository.Update(user);

            await _outboxService.PublishDomainEventsAsync(user.DomainEvents, cancellationToken: cancellationToken);

            var auditLog = await _auditLogService.CreateAuditLogAsync(
                "Deleted",
                "User",
                user.Id,
                cancellationToken: cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            await _auditLogService.MarkAsSuccessAsync(auditLog.Id, cancellationToken);

            user.ClearDomainEvents();

            return Result.Success();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure($"Failed to delete user: {ex.Message}");
        }
    }
}
