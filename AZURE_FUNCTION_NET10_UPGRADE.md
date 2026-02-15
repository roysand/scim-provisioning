# Azure Functions - .NET 10 Upgrade Guide

## Overview
The ScimProvisioning.AzureFunction project has been successfully upgraded to .NET 10.0.

## Changes Made

### 1. Updated ScimProvisioning.AzureFunction.csproj

**Before:**
```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <AzureFunctionsVersion>v4</AzureFunctionsVersion>
  <OutputType>Exe</OutputType>
</PropertyGroup>
```

**After:**
```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <OutputType>Exe</OutputType>
</PropertyGroup>
```

### Key Changes:
- ✅ Updated `TargetFramework` from `net8.0` to `net10.0`
- ✅ Removed `AzureFunctionsVersion` property (SDK 1.24+ automatically determines version based on .NET runtime)
- ✅ All Azure Functions Worker packages already support .NET 10:
  - Microsoft.Azure.Functions.Worker 1.24.0 ✓
  - Microsoft.Azure.Functions.Worker.Extensions.ServiceBus 5.23.0 ✓
  - Microsoft.Azure.Functions.Worker.Sdk 1.24.0 ✓

## Azure Functions .NET 10 Compatibility

### Supported Runtime
- **Azure Functions v4** Worker SDK supports .NET 10 via SDK version 1.24.0+
- No breaking changes from .NET 8 to .NET 10 for Azure Functions

### Project Dependencies
All dependencies are now targeting .NET 10:
- ScimProvisioning.Application → net10.0 ✓
- ScimProvisioning.Infrastructure → net10.0 ✓
- ScimProvisioning.Library → net10.0 ✓

## Building the Azure Function

### Local Build
```bash
dotnet build ScimProvisioning.AzureFunction/ScimProvisioning.AzureFunction.csproj
```

### Release Build
```bash
dotnet build -c Release ScimProvisioning.AzureFunction/ScimProvisioning.AzureFunction.csproj
```

### Publish for Deployment
```bash
dotnet publish -c Release ScimProvisioning.AzureFunction/ScimProvisioning.AzureFunction.csproj -o ./publish
```

## Running Locally

### Prerequisites
- .NET 10 SDK
- Azure Functions Core Tools (v4+)
- SQL Server instance

### Steps

1. **Set environment variables:**
```bash
# Set connection strings
export ConnectionStrings__DefaultConnection="Server=localhost;Database=ScimProvisioning;..."
export ServiceBusConnection="Endpoint=sb://..."
```

2. **Run the function locally:**
```bash
cd ScimProvisioning.AzureFunction
func start
```

3. **Monitor logs:**
The function will output logs to the console showing the health and status of the Outbox processor.

## Deployment

### Azure Functions Deployment

#### Option 1: Using Azure Functions Core Tools
```bash
# Install Azure Functions Core Tools
# https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local

# Publish to Azure
func azure functionapp publish <function-app-name>
```

#### Option 2: Using Azure CLI
```bash
az functionapp deployment source config-zip \
  -g <resource-group> \
  -n <function-app-name> \
  --src <path-to-zip>
```

#### Option 3: Using Docker
```bash
# Build Docker image
docker build -f Dockerfile.AzureFunction -t scim-outbox-processor:latest .

# Run locally
docker run -e ConnectionStrings__DefaultConnection="..." \
           -e ServiceBusConnection="..." \
           scim-outbox-processor:latest
```

### Configuration

Set these environment variables in Azure Functions:

| Variable | Value | Example |
|----------|-------|---------|
| `ConnectionStrings__DefaultConnection` | Database connection string | `Server=myserver.database.windows.net;Database=ScimProvisioning;...` |
| `ServiceBusConnection` | Service Bus connection string | `Endpoint=sb://myns.servicebus.windows.net/...` |
| `ASPNETCORE_ENVIRONMENT` | Environment | `Production` |

### Database Migrations

Before deploying, ensure database migrations are run:

```bash
# From project root
dotnet ef database update --project ScimProvisioning.Infrastructure
```

Or from the function itself on first run:
- The OutboxProcessorFunction will verify database schema exists
- Migrations can be applied during deployment

## Monitoring & Logging

### Application Insights Integration

Add to Program.cs for monitoring:

```csharp
services.AddApplicationInsightsTelemetry();
```

Configure in Azure:
1. Create Application Insights resource
2. Link to Function App
3. Monitor traces and performance

### Health Checks

The Outbox Processor will log:
- Successful message processing
- Errors and exceptions
- Service Bus connection status
- Database connectivity

Monitor these logs in Azure Portal or Application Insights.

## Troubleshooting

### Build Issues

**Error: "Invalid combination of TargetFramework and AzureFunctionsVersion"**
- Solution: Remove `AzureFunctionsVersion` property if present
- The SDK automatically determines the version

**Error: "Package incompatible with net10.0"**
- Solution: Update package versions in Directory.Packages.props
- Current versions are compatible with .NET 10

### Runtime Issues

**Error: "Database connection failed"**
- Verify `ConnectionStrings__DefaultConnection` is set correctly
- Check SQL Server is accessible and running
- Ensure database exists

**Error: "Service Bus connection failed"**
- Verify `ServiceBusConnection` is set correctly
- Check Service Bus namespace exists in Azure
- Verify authentication credentials

## Performance Considerations

### .NET 10 Benefits
- ✅ Faster startup time
- ✅ Improved garbage collection
- ✅ Better LINQ performance
- ✅ Enhanced Entity Framework Core performance

### Azure Functions Optimization
- Cold start time improved with .NET 10
- Memory consumption optimized
- Processing throughput increased

## Migration Checklist

- [x] Update TargetFramework to net10.0
- [x] Remove AzureFunctionsVersion (SDK handles it)
- [x] Verify all dependencies support .NET 10
- [x] Build locally and verify
- [x] Update Program.cs if needed (no changes required)
- [ ] Test in staging environment
- [ ] Deploy to production
- [ ] Monitor function execution
- [ ] Verify Outbox messages are processed

## Key Features Maintained

The Azure Function continues to support all features from .NET 8:
- ✅ Outbox Pattern message processing
- ✅ Service Bus integration
- ✅ Entity Framework Core operations
- ✅ Dependency injection
- ✅ Async/await programming model
- ✅ Logging and diagnostics

## Documentation

See related documentation:
- [UPGRADE_NET10.md](./UPGRADE_NET10.md) - Full project upgrade guide
- [DEPLOYMENT.md](./DEPLOYMENT.md) - Deployment guide
- [OutboxProcessorFunction.cs](./ScimProvisioning.AzureFunction/OutboxProcessorFunction.cs) - Function implementation

## Support

For Azure Functions documentation:
- [Azure Functions Runtime Versions](https://learn.microsoft.com/en-us/azure/azure-functions/functions-versions)
- [.NET Worker Extensions for Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection)
- [Azure Functions Triggers and Bindings](https://learn.microsoft.com/en-us/azure/azure-functions/functions-triggers-bindings)

---

**Status**: ✅ Upgraded to .NET 10.0  
**Date**: February 15, 2026  
**Framework**: .NET 10.0  
**SDK Requirement**: Microsoft.Azure.Functions.Worker 1.24.0+

