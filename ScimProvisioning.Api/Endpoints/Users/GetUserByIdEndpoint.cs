using FastEndpoints;
using ScimProvisioning.Application.DTOs;
using ScimProvisioning.Application.UseCases.Users;

namespace ScimProvisioning.Api.Endpoints.Users;

/// <summary>
/// Endpoint for getting a SCIM user by ID
/// </summary>
public class GetUserByIdEndpoint : Endpoint<GetUserByIdRequest, ApiResponse<UserResponse>>
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
            .Produces<ApiResponse<UserResponse>>(200)
            .Produces<ApiErrorResponse>(404)
            .WithTags("Users"));
    }

    public override async Task HandleAsync(GetUserByIdRequest req, CancellationToken ct)
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

        var response = new ApiResponse<UserResponse>(result.Value, "User retrieved successfully");
        await SendOkAsync(response, ct);
    }
}

public class GetUserByIdRequest
{
    public Guid Id { get; set; }
}
