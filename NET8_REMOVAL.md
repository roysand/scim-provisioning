# .NET 8 Support Removal

## Summary
Successfully removed .NET 8 support from the SCIM Provisioning project. All projects now target .NET 10.0 exclusively.

## Changes Made

### 1. Directory.Build.props
**Before:**
```xml
<!-- Default: Single target .NET 10.0 for executable projects -->
<TargetFramework Condition="'$(TargetFramework)' == '' AND ('$(MSBuildProjectName)' == 'ScimProvisioning.Api' OR '$(MSBuildProjectName)' == 'ScimProvisioning.AzureFunction' OR '$(MSBuildProjectName)' == 'ScimProvisioning.Sample' OR '$(MSBuildProjectName)' == 'ScimProvisioning.Tests')">net10.0</TargetFramework>

<!-- Multi-target for shared libraries to support both .NET 8 and .NET 10 -->
<TargetFrameworks Condition="'$(TargetFrameworks)' == '' AND ('$(MSBuildProjectName)' == 'ScimProvisioning.Core' OR '$(MSBuildProjectName)' == 'ScimProvisioning.Application' OR '$(MSBuildProjectName)' == 'ScimProvisioning.Infrastructure' OR '$(MSBuildProjectName)' == 'ScimProvisioning.Library')">net8.0;net10.0</TargetFrameworks>
```

**After:**
```xml
<!-- Default: Single target .NET 10.0 for all projects -->
<TargetFramework Condition="'$(TargetFramework)' == ''">net10.0</TargetFramework>
```

### 2. Directory.Packages.props
**Before:**
```xml
<!-- Entity Framework & Database -->
<ItemGroup>
  <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="8.0.11" Condition="'$(TargetFramework)' == 'net8.0'" />
  <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="10.0.0" Condition="'$(TargetFramework)' == 'net10.0'" />
  <PackageVersion Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.11" Condition="'$(TargetFramework)' == 'net8.0'" />
  <PackageVersion Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.0" Condition="'$(TargetFramework)' == 'net10.0'" />
  <PackageVersion Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.11" Condition="'$(TargetFramework)' == 'net8.0'" />
  <PackageVersion Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" Condition="'$(TargetFramework)' == 'net10.0'" />
  <PackageVersion Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.11" Condition="'$(TargetFramework)' == 'net8.0'" />
  <PackageVersion Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0" Condition="'$(TargetFramework)' == 'net10.0'" />
</ItemGroup>
```

**After:**
```xml
<!-- Entity Framework & Database -->
<ItemGroup>
  <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
  <PackageVersion Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.0" />
  <PackageVersion Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
  <PackageVersion Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0" />
</ItemGroup>
```

### 3. Build Output Cleanup
- Removed all `net8.0` build output folders from bin and obj directories
- Clean build now produces only `net10.0` artifacts

## Impact

### Projects Affected
All projects now target .NET 10.0 exclusively:
- ✅ ScimProvisioning.Api → net10.0
- ✅ ScimProvisioning.AzureFunction → net10.0
- ✅ ScimProvisioning.Core → net10.0 (was multi-targeted)
- ✅ ScimProvisioning.Application → net10.0 (was multi-targeted)
- ✅ ScimProvisioning.Infrastructure → net10.0 (was multi-targeted)
- ✅ ScimProvisioning.Library → net10.0 (was multi-targeted)
- ✅ ScimProvisioning.Sample → net10.0
- ✅ ScimProvisioning.Tests → net10.0

### Package Versions
All packages now use .NET 10 compatible versions:
- Entity Framework Core: 10.0.0
- Microsoft.AspNetCore.OpenApi: 10.0.0
- Microsoft.Extensions.Hosting: 10.0.0

## Verification

Build completed successfully:
```
Build succeeded with 12 warning(s) in 9.2s
```

All warnings are related to nullable reference types (CS8618) and are not related to the .NET 8 removal.

## Benefits

1. **Simplified Configuration**: No more multi-targeting complexity
2. **Reduced Build Time**: Single target framework per project
3. **Latest Features**: Full access to .NET 10 features and improvements
4. **Smaller Artifacts**: Only one set of binaries per project
5. **Easier Maintenance**: Single code path to maintain

## Important Note: WorkerExtensions and .NET 8.0

During the build, you may see a message like:
```
WorkerExtensions net8.0 succeeded (1.4s) → ScimProvisioning.AzureFunction\obj\Debug\net10.0\WorkerExtensions\bin\Release\net8.0\Microsoft.Azure.Functions.Worker.Extensions.dll
```

**This is expected and correct behavior.** Here's why:

- The `WorkerExtensions` project is **auto-generated** by the Azure Functions Worker SDK during build
- It **must target .NET 8.0** because it compiles binding extensions that run in the Azure Functions host context
- Azure Functions uses the **isolated worker process model**: your function app runs in .NET 10.0 as a separate process, while the binding extensions interface with the host (which uses .NET 8.0)
- Your **actual application code runs on .NET 10.0** - check `ScimProvisioning.AzureFunction\bin\Debug\net10.0\`

This is an architectural requirement of Azure Functions v4 with the isolated worker model and does not mean your application is using .NET 8.0.

## Date
February 15, 2026

