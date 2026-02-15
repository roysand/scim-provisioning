using FastEndpoints;
using ScimProvisioning.Application.UseCases.Users;

namespace ScimProvisioning.Api.Endpoints.Users;

/// <summary>
/// Endpoint for deleting a SCIM user
/// </summary>
public class DeleteUserEndpoint : Endpoint<DeleteUserRequest>
{
    private readonly DeleteUserUseCase _useCase;

    public DeleteUserEndpoint(DeleteUserUseCase useCase)
    {
        _useCase = useCase;
    }

    public override void Configure()
    {
        Delete("/scim/v2/Users/{id}");
        AllowAnonymous();
        Description(d => d
            .Produces(204)
            .Produces(404)
            .WithTags("Users"));
    }

    public override async Task HandleAsync(DeleteUserRequest req, CancellationToken ct)
    {
        var result = await _useCase.ExecuteAsync(req.Id, ct);

        if (result.IsFailure)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendNoContentAsync(ct);
    }
}

public class DeleteUserRequest
{
    public Guid Id { get; set; }
}
