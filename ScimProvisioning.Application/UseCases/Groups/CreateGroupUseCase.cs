using AutoMapper;
using ScimProvisioning.Application.DTOs;
using ScimProvisioning.Application.Services;
using ScimProvisioning.Core.Common;
using ScimProvisioning.Core.Entities;
using ScimProvisioning.Core.Interfaces;

namespace ScimProvisioning.Application.UseCases.Groups;

/// <summary>
/// Use case for creating a new SCIM group
/// </summary>
public class CreateGroupUseCase
{
    private readonly IScimGroupRepository _groupRepository;
    private readonly IOutboxService _outboxService;
    private readonly IAuditLogService _auditLogService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateGroupUseCase(
        IScimGroupRepository groupRepository,
        IOutboxService outboxService,
        IAuditLogService auditLogService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _groupRepository = groupRepository;
        _outboxService = outboxService;
        _auditLogService = auditLogService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<GroupResponse>> ExecuteAsync(
        CreateGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        var existingGroup = await _groupRepository.GetByExternalIdAsync(request.ExternalId, cancellationToken);
        if (existingGroup != null)
            return Result.Failure<GroupResponse>("Group with this external ID already exists");

        var groupResult = ScimGroup.Create(request.ExternalId, request.DisplayName);
        if (groupResult.IsFailure)
            return Result.Failure<GroupResponse>(groupResult.Error);

        var group = groupResult.Value;

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            await _groupRepository.AddAsync(group, cancellationToken);

            await _outboxService.PublishDomainEventsAsync(group.DomainEvents, cancellationToken: cancellationToken);

            var auditLog = await _auditLogService.CreateAuditLogAsync(
                "Created",
                "Group",
                group.Id,
                changeDetails: System.Text.Json.JsonSerializer.Serialize(request),
                cancellationToken: cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            await _auditLogService.MarkAsSuccessAsync(auditLog.Id, cancellationToken);

            group.ClearDomainEvents();

            var response = _mapper.Map<GroupResponse>(group);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure<GroupResponse>($"Failed to create group: {ex.Message}");
        }
    }
}
