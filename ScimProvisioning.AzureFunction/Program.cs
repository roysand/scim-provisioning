using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScimProvisioning.Library;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        // Add SCIM Provisioning services
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
            ?? "Server=(localdb)\\mssqllocaldb;Database=ScimProvisioning;Trusted_Connection=True;";
        var serviceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnection");

        services.AddScimProvisioning(connectionString, serviceBusConnectionString);
    })
    .Build();

await host.RunAsync();
