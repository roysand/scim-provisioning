using FastEndpoints;
using ScimProvisioning.Application.DTOs;
using ScimProvisioning.Application.UseCases.Groups;

namespace ScimProvisioning.Api.Endpoints.Groups;

/// <summary>
/// Endpoint for creating a SCIM group
/// </summary>
public class CreateGroupEndpoint : Endpoint<CreateGroupRequest, ApiResponse<GroupResponse>>
{
    private readonly CreateGroupUseCase _useCase;

    public CreateGroupEndpoint(CreateGroupUseCase useCase)
    {
        _useCase = useCase;
    }

    public override void Configure()
    {
        Post("/scim/v2/Groups");
        AllowAnonymous();
        Description(d => d
            .Produces<ApiResponse<GroupResponse>>(201)
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(409)
            .WithTags("Groups"));
    }

    public override async Task HandleAsync(CreateGroupRequest req, CancellationToken ct)
    {
        var result = await _useCase.ExecuteAsync(req, ct);

        if (result.IsFailure)
        {
            ThrowError(r =>
            {
                r.StatusCode = 400;
                r.Message = result.Error;
            });
        }

        var response = new ApiResponse<GroupResponse>(result.Value, "Group created successfully");
        await SendCreatedAtAsync<GetGroupByIdEndpoint>(
            new { id = result.Value.Id },
            response,
            cancellation: ct);
    }
}

/// <summary>
/// Endpoint for getting a SCIM group by ID
/// </summary>
public class GetGroupByIdEndpoint : Endpoint<GetGroupByIdRequest, ApiResponse<GroupResponse>>
{
    private readonly GetGroupByIdUseCase _useCase;

    public GetGroupByIdEndpoint(GetGroupByIdUseCase useCase)
    {
        _useCase = useCase;
    }

    public override void Configure()
    {
        Get("/scim/v2/Groups/{id}");
        AllowAnonymous();
        Description(d => d
            .Produces<ApiResponse<GroupResponse>>(200)
            .Produces<ApiErrorResponse>(404)
            .WithTags("Groups"));
    }

    public override async Task HandleAsync(GetGroupByIdRequest req, CancellationToken ct)
    {
        var result = await _useCase.ExecuteAsync(req.Id, ct);

        if (result.IsFailure)
        {
            ThrowError(r =>
            {
                r.StatusCode = 404;
                r.Message = result.Error;
            });
        }

        var response = new ApiResponse<GroupResponse>(result.Value, "Group retrieved successfully");
        await SendOkAsync(response, ct);
    }
}

/// <summary>
/// Endpoint for listing SCIM groups
/// </summary>
public class ListGroupsEndpoint : Endpoint<ListGroupsRequest, ApiPagedResponse<GroupResponse>>
{
    private readonly ListGroupsUseCase _useCase;

    public ListGroupsEndpoint(ListGroupsUseCase useCase)
    {
        _useCase = useCase;
    }

    public override void Configure()
    {
        Get("/scim/v2/Groups");
        AllowAnonymous();
        Description(d => d
            .Produces<ApiPagedResponse<GroupResponse>>(200)
            .WithTags("Groups"));
    }

    public override async Task HandleAsync(ListGroupsRequest req, CancellationToken ct)
    {
        var result = await _useCase.ExecuteAsync(
            req.StartIndex,
            req.Count,
            req.Filter,
            ct);

        var listResponse = result.Value;
        var response = new ApiPagedResponse<GroupResponse>(
            listResponse.Resources,
            listResponse.TotalResults,
            listResponse.StartIndex,
            listResponse.ItemsPerPage,
            "Groups retrieved successfully");

        await SendOkAsync(response, ct);
    }
}

public class GetGroupByIdRequest
{
    public Guid Id { get; set; }
}

public class ListGroupsRequest
{
    public int StartIndex { get; set; } = 0;
    public int Count { get; set; } = 100;
    public string? Filter { get; set; }
}
