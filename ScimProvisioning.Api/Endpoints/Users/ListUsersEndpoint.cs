using FastEndpoints;
using ScimProvisioning.Application.DTOs;
using ScimProvisioning.Application.UseCases.Users;

namespace ScimProvisioning.Api.Endpoints.Users;

/// <summary>
/// Endpoint for listing SCIM users
/// </summary>
public class ListUsersEndpoint : Endpoint<ListUsersRequest, ListUsersResponse>
{
    private readonly ListUsersUseCase _useCase;

    public ListUsersEndpoint(ListUsersUseCase useCase)
    {
        _useCase = useCase;
    }

    public override void Configure()
    {
        Get("/scim/v2/Users");
        AllowAnonymous();
        Description(d => d
            .Produces<ListUsersResponse>(200)
            .WithTags("Users"));
    }

    public override async Task HandleAsync(ListUsersRequest req, CancellationToken ct)
    {
        var result = await _useCase.ExecuteAsync(
            req.StartIndex,
            req.Count,
            req.Filter,
            ct);

        await SendOkAsync(result.Value, ct);
    }
}

public class ListUsersRequest
{
    public int StartIndex { get; set; } = 0;
    public int Count { get; set; } = 100;
    public string? Filter { get; set; }
}
