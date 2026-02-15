using FastEndpoints;
using ScimProvisioning.Application.DTOs;
using ScimProvisioning.Application.UseCases.Users;

namespace ScimProvisioning.Api.Endpoints.Users;

/// <summary>
/// Endpoint for updating a SCIM user
/// </summary>
public class UpdateUserEndpoint : Endpoint<UpdateUserEndpointRequest, UserResponse>
{
    private readonly UpdateUserUseCase _useCase;

    public UpdateUserEndpoint(UpdateUserUseCase useCase)
    {
        _useCase = useCase;
    }

    public override void Configure()
    {
        Patch("/scim/v2/Users/{id}");
        AllowAnonymous();
        Description(d => d
            .Produces<UserResponse>(200)
            .Produces(400)
            .Produces(404)
            .WithTags("Users"));
    }

    public override async Task HandleAsync(UpdateUserEndpointRequest req, CancellationToken ct)
    {
        var updateRequest = new UpdateUserRequest
        {
            DisplayName = req.DisplayName,
            PrimaryEmail = req.PrimaryEmail,
            Active = req.Active
        };

        var result = await _useCase.ExecuteAsync(req.Id, updateRequest, ct);

        if (result.IsFailure)
        {
            if (result.Error.Contains("not found"))
                await SendNotFoundAsync(ct);
            else
            {
                await SendErrorsAsync(400, ct);
                AddError(result.Error);
            }
            return;
        }

        await SendOkAsync(result.Value, ct);
    }
}

public class UpdateUserEndpointRequest
{
    public Guid Id { get; set; }
    public string? DisplayName { get; set; }
    public string? PrimaryEmail { get; set; }
    public bool? Active { get; set; }
}
