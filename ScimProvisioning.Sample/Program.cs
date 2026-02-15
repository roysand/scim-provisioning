using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScimProvisioning.Application.DTOs;
using ScimProvisioning.Application.UseCases.Users;
using ScimProvisioning.Library;

// Sample application demonstrating how to use the SCIM Provisioning Library

var builder = Host.CreateApplicationBuilder(args);

// Configure SCIM Provisioning services
var connectionString = "Server=(localdb)\\mssqllocaldb;Database=ScimProvisioningSample;Trusted_Connection=True;";
builder.Services.AddScimProvisioning(connectionString);

var host = builder.Build();

// Get the service provider
using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    Console.WriteLine("=== SCIM Provisioning Library Sample ===\n");

    // Example 1: Create a user
    var createUserUseCase = services.GetRequiredService<CreateUserUseCase>();
    
    var createUserRequest = new CreateUserRequest
    {
        ExternalId = "ext-" + Guid.NewGuid().ToString("N").Substring(0, 8),
        UserName = "john.doe",
        DisplayName = "John Doe",
        PrimaryEmail = "john.doe@example.com",
        Active = true
    };

    Console.WriteLine("Creating user...");
    var createResult = await createUserUseCase.ExecuteAsync(createUserRequest);

    if (createResult.IsSuccess)
    {
        Console.WriteLine($"✓ User created successfully!");
        Console.WriteLine($"  ID: {createResult.Value.Id}");
        Console.WriteLine($"  UserName: {createResult.Value.UserName}");
        Console.WriteLine($"  DisplayName: {createResult.Value.DisplayName}");
        Console.WriteLine($"  Email: {createResult.Value.PrimaryEmail}");
        Console.WriteLine($"  Active: {createResult.Value.Active}\n");

        // Example 2: Get user by ID
        var getUserUseCase = services.GetRequiredService<GetUserByIdUseCase>();
        Console.WriteLine("Retrieving user by ID...");
        var getUserResult = await getUserUseCase.ExecuteAsync(createResult.Value.Id);

        if (getUserResult.IsSuccess)
        {
            Console.WriteLine($"✓ User retrieved successfully!");
            Console.WriteLine($"  ID: {getUserResult.Value.Id}");
            Console.WriteLine($"  UserName: {getUserResult.Value.UserName}\n");
        }

        // Example 3: Update user
        var updateUserUseCase = services.GetRequiredService<UpdateUserUseCase>();
        Console.WriteLine("Updating user...");
        var updateResult = await updateUserUseCase.ExecuteAsync(
            createResult.Value.Id,
            new UpdateUserRequest
            {
                DisplayName = "John Updated Doe",
                Active = true
            });

        if (updateResult.IsSuccess)
        {
            Console.WriteLine($"✓ User updated successfully!");
            Console.WriteLine($"  New DisplayName: {updateResult.Value.DisplayName}\n");
        }

        // Example 4: List users
        var listUsersUseCase = services.GetRequiredService<ListUsersUseCase>();
        Console.WriteLine("Listing users...");
        var listResult = await listUsersUseCase.ExecuteAsync(0, 10);

        if (listResult.IsSuccess)
        {
            Console.WriteLine($"✓ Found {listResult.Value.TotalResults} user(s)");
            foreach (var user in listResult.Value.Resources)
            {
                Console.WriteLine($"  - {user.UserName} ({user.DisplayName})");
            }
            Console.WriteLine();
        }

        // Example 5: Delete user
        var deleteUserUseCase = services.GetRequiredService<DeleteUserUseCase>();
        Console.WriteLine("Deleting user...");
        var deleteResult = await deleteUserUseCase.ExecuteAsync(createResult.Value.Id);

        if (deleteResult.IsSuccess)
        {
            Console.WriteLine($"✓ User deleted successfully!\n");
        }
        else
        {
            Console.WriteLine($"✗ Failed to delete user: {deleteResult.Error}\n");
        }
    }
    else
    {
        Console.WriteLine($"✗ Failed to create user: {createResult.Error}\n");
    }

    Console.WriteLine("=== Sample completed ===");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
}
