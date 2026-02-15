using FastEndpoints;
using ScimProvisioning.Application.DTOs;
using ScimProvisioning.Application.UseCases.Users;

namespace ScimProvisioning.Api.Endpoints.Users;

/// <summary>
/// Endpoint for getting a SCIM user by ID
/// </summary>
public class GetUserByIdEndpoint : Endpoint<GetUserByIdRequest, UserResponse>
{
    private readonly GetUserByIdUseCase _useCase;

    public GetUserByIdEndpoint(GetUserByIdUseCase useCase)
    {
        _useCase = useCase;
    }

    public override void Configure()
    {
        Get("/scim/v2/Users/{id}");
        AllowAnonymous();
        Description(d => d
            .Produces<UserResponse>(200)
            .Produces(404)
            .WithTags("Users"));
    }

    public override async Task HandleAsync(GetUserByIdRequest req, CancellationToken ct)
    {
        var result = await _useCase.ExecuteAsync(req.Id, ct);

        if (result.IsFailure)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(result.Value, ct);
    }
}

public class GetUserByIdRequest
{
    public Guid Id { get; set; }
}
