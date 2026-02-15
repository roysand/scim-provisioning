using FastEndpoints;
using FastEndpoints.Swagger;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ScimProvisioning.Application.Services;
using ScimProvisioning.Application.UseCases.Users;
using ScimProvisioning.Application.UseCases.Groups;
using ScimProvisioning.Core.Interfaces;
using ScimProvisioning.Infrastructure.Persistence;
using ScimProvisioning.Infrastructure.Repositories;
using ScimProvisioning.Infrastructure.Services;
using ScimProvisioning.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ScimProvisioningDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("ScimProvisioning.Infrastructure")));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(ScimProvisioning.Application.Mappings.UserMappingProfile).Assembly);

// Register repositories
builder.Services.AddScoped<IScimUserRepository, ScimUserRepository>();
builder.Services.AddScoped<IScimGroupRepository, ScimGroupRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register services
builder.Services.AddScoped<IOutboxService, OutboxService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// Register use cases
builder.Services.AddScoped<CreateUserUseCase>();
builder.Services.AddScoped<UpdateUserUseCase>();
builder.Services.AddScoped<DeleteUserUseCase>();
builder.Services.AddScoped<GetUserByIdUseCase>();
builder.Services.AddScoped<ListUsersUseCase>();
builder.Services.AddScoped<CreateGroupUseCase>();
builder.Services.AddScoped<GetGroupByIdUseCase>();
builder.Services.AddScoped<ListGroupsUseCase>();

// Add Service Bus publisher (optional - configure if using Service Bus)
builder.Services.Configure<ServiceBusOptions>(builder.Configuration.GetSection("ServiceBus"));

// Add FastEndpoints
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "SCIM Provisioning API";
        s.Version = "v1";
        s.Description = "SCIM 2.0 compliant provisioning API";
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.UseHttpsRedirection();

// Use FastEndpoints
app.UseFastEndpoints();

app.Run();
