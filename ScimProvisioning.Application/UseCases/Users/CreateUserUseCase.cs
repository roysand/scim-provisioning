using AutoMapper;
using ScimProvisioning.Application.DTOs;
using ScimProvisioning.Application.Services;
using ScimProvisioning.Core.Common;
using ScimProvisioning.Core.Entities;
using ScimProvisioning.Core.Interfaces;

namespace ScimProvisioning.Application.UseCases.Users;

/// <summary>
/// Use case for creating a new SCIM user
/// </summary>
public class CreateUserUseCase
{
    private readonly IScimUserRepository _userRepository;
    private readonly IOutboxService _outboxService;
    private readonly IAuditLogService _auditLogService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateUserUseCase(
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
        CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByExternalIdAsync(request.ExternalId, cancellationToken);
        if (existingUser != null)
            return Result.Failure<UserResponse>("User with this external ID already exists");

        // Create user entity
        var userResult = ScimUser.Create(
            request.ExternalId,
            request.UserName,
            request.DisplayName,
            request.PrimaryEmail);

        if (userResult.IsFailure)
            return Result.Failure<UserResponse>(userResult.Error);

        var user = userResult.Value;

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Add user to repository
            await _userRepository.AddAsync(user, cancellationToken);

            // Publish domain events to outbox
            await _outboxService.PublishDomainEventsAsync(user.DomainEvents, cancellationToken: cancellationToken);

            // Create audit log
            var auditLog = await _auditLogService.CreateAuditLogAsync(
                "Created",
                "User",
                user.Id,
                changeDetails: System.Text.Json.JsonSerializer.Serialize(request),
                cancellationToken: cancellationToken);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Mark audit as success
            await _auditLogService.MarkAsSuccessAsync(auditLog.Id, cancellationToken);

            user.ClearDomainEvents();

            var response = _mapper.Map<UserResponse>(user);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure<UserResponse>($"Failed to create user: {ex.Message}");
        }
    }
}
