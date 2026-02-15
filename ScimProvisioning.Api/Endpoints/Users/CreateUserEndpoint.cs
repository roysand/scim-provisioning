using FastEndpoints;
using ScimProvisioning.Application.DTOs;
using ScimProvisioning.Application.UseCases.Users;

namespace ScimProvisioning.Api.Endpoints.Users;

/// <summary>
/// Endpoint for creating a SCIM user
/// </summary>
public class CreateUserEndpoint : Endpoint<CreateUserRequest, UserResponse>
{
    private readonly CreateUserUseCase _useCase;

    public CreateUserEndpoint(CreateUserUseCase useCase)
    {
        _useCase = useCase;
    }

    public override void Configure()
    {
        Post("/scim/v2/Users");
        AllowAnonymous();
        Description(d => d
            .Produces<UserResponse>(201)
            .Produces(400)
            .Produces(409)
            .WithTags("Users"));
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var result = await _useCase.ExecuteAsync(req, ct);

        if (result.IsFailure)
        {
            await SendErrorsAsync(400, ct);
            AddError(result.Error);
            return;
        }

        await SendCreatedAtAsync<GetUserByIdEndpoint>(
            new { id = result.Value.Id },
            result.Value,
            cancellation: ct);
    }
}
