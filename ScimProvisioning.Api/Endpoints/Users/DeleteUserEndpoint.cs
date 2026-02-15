using FastEndpoints;
using ScimProvisioning.Application.DTOs;
using ScimProvisioning.Application.UseCases.Users;

namespace ScimProvisioning.Api.Endpoints.Users;

/// <summary>
/// Endpoint for deleting a SCIM user
/// </summary>
public class DeleteUserEndpoint : Endpoint<DeleteUserRequest, ApiResponse<string>>
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
            .Produces<ApiResponse<string>>(200)
            .Produces<ApiErrorResponse>(404)
            .WithTags("Users"));
    }

    public override async Task HandleAsync(DeleteUserRequest req, CancellationToken ct)
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

        var response = new ApiResponse<string>("", "User deleted successfully");
        await SendOkAsync(response, ct);
    }
}

public class DeleteUserRequest
{
    public Guid Id { get; set; }
}
