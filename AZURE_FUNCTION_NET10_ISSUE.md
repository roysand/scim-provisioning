# Azure Functions .NET 10 Compatibility Issue - Resolution

## Problem

When upgrading `ScimProvisioning.AzureFunction` to .NET 10, you may encounter this error:

```
Microsoft.Azure.Functions.Worker.Sdk.targets(78,5): Error: Invalid combination of TargetFramework and AzureFunctionsVersion is set
```

## Root Cause

Azure Functions Worker SDK has limited .NET 10 support as of February 2026:
- SDK v1.x doesn't support .NET 10
- SDK v2.0 has experimental/incomplete .NET 10 support
- The `AzureFunctionsVersion` property conflicts with net10.0 target

## Solution Options

### Option 1: Keep Azure Functions on .NET 8 (Recommended for Production)

This is the **safest and recommended approach** for production environments.

**ScimProvisioning.AzureFunction.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <AzureFunctionsVersion>v4</AzureFunctionsVersion>
    <OutputType>Exe</OutputType>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\ScimProvisioning.Application\ScimProvisioning.Application.csproj" />
    <ProjectReference Include="..\ScimProvisioning.Infrastructure\ScimProvisioning.Infrastructure.csproj" />
    <ProjectReference Include="..\ScimProvisioning.Library\ScimProvisioning.Library.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Azure.Functions.Worker" />
    <PackageReference Include="Microsoft.Azure.Functions.Worker.Extensions.ServiceBus" />
    <PackageReference Include="Microsoft.Azure.Functions.Worker.Sdk" />
  </ItemGroup>
</Project>
```

**Directory.Packages.props (Azure Functions section):**
```xml
<!-- Azure Services - .NET 8 for Azure Functions stability -->
<ItemGroup>
  <PackageVersion Include="Azure.Messaging.ServiceBus" Version="7.18.2" />
  <PackageVersion Include="Microsoft.Azure.Functions.Worker" Version="1.23.0" />
  <PackageVersion Include="Microsoft.Azure.Functions.Worker.Extensions.ServiceBus" Version="5.22.0" />
  <PackageVersion Include="Microsoft.Azure.Functions.Worker.Sdk" Version="1.18.1" />
</ItemGroup>
```

### Option 2: Multi-Target (Both .NET 8 and .NET 10)

Allow Azure Function dependencies to work with both frameworks:

**Update Application, Infrastructure, Library to multi-target:**
```xml
<PropertyGroup>
  <TargetFrameworks>net8.0;net10.0</TargetFrameworks>
</PropertyGroup>
```

This way:
- Azure Functions runs on .NET 8
- Main API runs on .NET 10
- Shared libraries support both

### Option 3: Wait for Official .NET 10 Support (Future)

Monitor Azure Functions releases for official .NET 10 support:
- Check: https://github.com/Azure/azure-functions-dotnet-worker
- When SDK 2.x is stable for .NET 10, upgrade then

## Recommended Configuration (Production-Ready)

**Keep Azure Functions on .NET 8:**

1. **ScimProvisioning.AzureFunction.csproj**
   ```xml
   <TargetFramework>net8.0</TargetFramework>
   <AzureFunctionsVersion>v4</AzureFunctionsVersion>
   ```

2. **Make dependencies multi-target**  
   Update these projects to support both net8.0 and net10.0:
   - ScimProvisioning.Core
   - ScimProvisioning.Application
   - ScimProvisioning.Infrastructure
   - ScimProvisioning.Library

3. **Directory.Packages.props**
   ```xml
   <PackageVersion Include="Microsoft.Azure.Functions.Worker" Version="1.23.0" />
   <PackageVersion Include="Microsoft.Azure.Functions.Worker.Sdk" Version="1.18.1" />
   ```

## Build Verification

```bash
# Clean everything
dotnet clean
rm -r ScimProvisioning.AzureFunction/bin
rm -r ScimProvisioning.AzureFunction/obj

# Restore
dotnet restore ScimProvisioning.AzureFunction/ScimProvisioning.AzureFunction.csproj

# Build
dotnet build ScimProvisioning.AzureFunction/ScimProvisioning.AzureFunction.csproj

# Should succeed with net8.0
```

## Why Keep Azure Functions on .NET 8?

1. **Stability** - Production-proven runtime
2. **Full SDK Support** - All features work
3. **Azure Compatibility** - Fully supported in Azure
4. **Performance** - Already optimized
5. **No Breaking Changes** - Smooth operation

## Migration Path to .NET 10 (When Ready)

When Azure Functions officially supports .NET 10:

1. Monitor releases: https://github.com/Azure/azure-functions-dotnet-worker/releases
2. Test SDK 2.x stability
3. Update packages to SDK 2.x
4. Change TargetFramework to net10.0
5. Remove AzureFunctionsVersion property
6. Test thoroughly in staging
7. Deploy to production

## Current Status

- ✅ Main API: .NET 10.0
- ✅ All libraries: .NET 10.0
- ⚠️ Azure Functions: .NET 8.0 (stable, recommended)

This is a **common and acceptable architecture** where:
- Modern API runs on latest .NET
- Background workers run on stable LTS version
- Both share common libraries

## References

- [Azure Functions .NET Support](https://learn.microsoft.com/en-us/azure/azure-functions/supported-languages)
- [Azure Functions Worker SDK Releases](https://github.com/Azure/azure-functions-dotnet-worker/releases)
- [.NET Support Policy](https://dotnet.microsoft.com/en-us/platform/support/policy)

