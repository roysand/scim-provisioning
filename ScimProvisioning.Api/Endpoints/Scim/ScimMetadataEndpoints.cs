using FastEndpoints;

namespace ScimProvisioning.Api.Endpoints.Scim;

/// <summary>
/// Endpoint for SCIM Service Provider Configuration
/// </summary>
public class ServiceProviderConfigEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/scim/v2/ServiceProviderConfig");
        AllowAnonymous();
        Description(d => d
            .Produces<object>(200)
            .WithTags("SCIM Metadata"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var config = new
        {
            schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:ServiceProviderConfig" },
            documentationUri = "https://tools.ietf.org/html/rfc7644",
            patch = new { supported = true },
            bulk = new { supported = false, maxOperations = 0, maxPayloadSize = 0 },
            filter = new { supported = true, maxResults = 100 },
            changePassword = new { supported = false },
            sort = new { supported = true },
            etag = new { supported = false },
            authenticationSchemes = new[]
            {
                new
                {
                    type = "oauthbearertoken",
                    name = "OAuth Bearer Token",
                    description = "Authentication scheme using the OAuth Bearer Token Standard",
                    specUri = "https://tools.ietf.org/html/rfc6750",
                    documentationUri = "https://tools.ietf.org/html/rfc6750"
                }
            }
        };

        await SendOkAsync(config, ct);
    }
}

/// <summary>
/// Endpoint for SCIM Resource Types
/// </summary>
public class ResourceTypesEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/scim/v2/ResourceTypes");
        AllowAnonymous();
        Description(d => d
            .Produces<object>(200)
            .WithTags("SCIM Metadata"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var resourceTypes = new
        {
            schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:ResourceType" },
            Resources = new[]
            {
                new
                {
                    schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:ResourceType" },
                    id = "User",
                    name = "User",
                    endpoint = "/scim/v2/Users",
                    description = "SCIM 2.0 User Resource",
                    schema = "urn:ietf:params:scim:schemas:core:2.0:User"
                },
                new
                {
                    schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:ResourceType" },
                    id = "Group",
                    name = "Group",
                    endpoint = "/scim/v2/Groups",
                    description = "SCIM 2.0 Group Resource",
                    schema = "urn:ietf:params:scim:schemas:core:2.0:Group"
                }
            }
        };

        await SendOkAsync(resourceTypes, ct);
    }
}

/// <summary>
/// Endpoint for SCIM Schemas
/// </summary>
public class SchemasEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/scim/v2/Schemas");
        AllowAnonymous();
        Description(d => d
            .Produces<object>(200)
            .WithTags("SCIM Metadata"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var schemas = new
        {
            schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:Schema" },
            Resources = new[]
            {
                new
                {
                    id = "urn:ietf:params:scim:schemas:core:2.0:User",
                    name = "User",
                    description = "SCIM 2.0 User Resource Schema"
                },
                new
                {
                    id = "urn:ietf:params:scim:schemas:core:2.0:Group",
                    name = "Group",
                    description = "SCIM 2.0 Group Resource Schema"
                }
            }
        };

        await SendOkAsync(schemas, ct);
    }
}
