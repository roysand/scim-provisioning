using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScimProvisioning.Application.Services;
using ScimProvisioning.Application.UseCases.Groups;
using ScimProvisioning.Application.UseCases.Users;
using ScimProvisioning.Core.Interfaces;
using ScimProvisioning.Infrastructure.Messaging;
using ScimProvisioning.Infrastructure.Persistence;
using ScimProvisioning.Infrastructure.Repositories;
using ScimProvisioning.Infrastructure.Services;

namespace ScimProvisioning.Library;

/// <summary>
/// Extension methods for adding SCIM provisioning services to the DI container
/// </summary>
public static class ScimProvisioningExtensions
{
    /// <summary>
    /// Adds all SCIM provisioning services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="connectionString">Database connection string</param>
    /// <param name="serviceBusConnectionString">Optional Service Bus connection string</param>
    /// <param name="serviceBusQueueName">Optional Service Bus queue name</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddScimProvisioning(
        this IServiceCollection services,
        string connectionString,
        string? serviceBusConnectionString = null,
        string? serviceBusQueueName = "scim-events")
    {
        // Add DbContext
        services.AddDbContext<ScimProvisioningDbContext>(options =>
            options.UseSqlServer(connectionString, 
                b => b.MigrationsAssembly("ScimProvisioning.Infrastructure")));

        // Add AutoMapper
        services.AddAutoMapper(typeof(Application.Mappings.UserMappingProfile).Assembly);

        // Register repositories
        services.AddScoped<IScimUserRepository, ScimUserRepository>();
        services.AddScoped<IScimGroupRepository, ScimGroupRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register services
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddScoped<IAuditLogService, AuditLogService>();

        // Register use cases
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<UpdateUserUseCase>();
        services.AddScoped<DeleteUserUseCase>();
        services.AddScoped<GetUserByIdUseCase>();
        services.AddScoped<ListUsersUseCase>();
        services.AddScoped<CreateGroupUseCase>();
        services.AddScoped<GetGroupByIdUseCase>();
        services.AddScoped<ListGroupsUseCase>();

        // Register Service Bus publisher if connection string is provided
        if (!string.IsNullOrWhiteSpace(serviceBusConnectionString))
        {
            services.Configure<ServiceBusOptions>(options =>
            {
                options.ConnectionString = serviceBusConnectionString;
                options.QueueName = serviceBusQueueName ?? "scim-events";
            });
            services.AddSingleton<AzureServiceBusOutboxPublisher>();
        }

        return services;
    }
}

/// <summary>
/// Configuration options for SCIM provisioning
/// </summary>
public class ScimProvisioningOptions
{
    /// <summary>
    /// Database connection string
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Azure Service Bus connection string (optional)
    /// </summary>
    public string? ServiceBusConnectionString { get; set; }

    /// <summary>
    /// Azure Service Bus queue name
    /// </summary>
    public string ServiceBusQueueName { get; set; } = "scim-events";
}
