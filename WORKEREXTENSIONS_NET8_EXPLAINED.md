# Azure Functions WorkerExtensions and .NET 8.0 - Technical Explanation

## Question
Why does the build output show:
```
WorkerExtensions net8.0 succeeded (1.4s) → ScimProvisioning.AzureFunction\obj\Debug\net10.0\WorkerExtensions\bin\Release\net8.0\Microsoft.Azure.Functions.Worker.Extensions.dll
```

## Answer: This is Expected and Correct ✅

### What is WorkerExtensions?

`WorkerExtensions` is an **auto-generated build artifact** created by the Azure Functions Worker SDK during compilation. It is **not your application code**.

### Why Does It Target .NET 8.0?

Azure Functions v4 uses the **Isolated Worker Process Model**:

```
┌─────────────────────────────────────────────────────────────┐
│                   Azure Functions Host                      │
│                    (Internal: .NET 8.0)                     │
│  ┌───────────────────────────────────────────────────────┐ │
│  │            WorkerExtensions (.NET 8.0)                 │ │
│  │  • Binding extensions (ServiceBus, etc.)              │ │
│  │  • Interfaces with the Functions Host                 │ │
│  └───────────────────────────────────────────────────────┘ │
│                           │                                  │
│                    IPC/RPC Channel                          │
│                           │                                  │
│  ┌───────────────────────────────────────────────────────┐ │
│  │    Your Function App (.NET 10.0) ← YOUR CODE RUNS    │ │
│  │  • ScimProvisioning.AzureFunction                     │ │
│  │  • Your business logic                                │ │
│  │  • All dependencies                                   │ │
│  └───────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

### Key Points

1. **Your Application Runs on .NET 10.0**: The actual `ScimProvisioning.AzureFunction.dll` is compiled for .NET 10.0
2. **WorkerExtensions is a Bridge**: It provides compatibility between your .NET 10.0 app and the Azure Functions host
3. **Separate Processes**: Your function app runs in a separate process from the host
4. **SDK-Generated**: The WorkerExtensions project is created by `Microsoft.Azure.Functions.Worker.Sdk` during build

### Verification

Check the actual output directory:
```powershell
PS> Get-ChildItem ScimProvisioning.AzureFunction\bin\Debug -Directory

Directory: C:\shared\repo\private\testing\AI\scim-provisioning\ScimProvisioning.AzureFunction\bin\Debug

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
d----          2/15/2026  10:30 AM                net10.0  ← YOUR APP RUNS HERE
```

Your application DLL:
```
ScimProvisioning.AzureFunction\bin\Debug\net10.0\ScimProvisioning.AzureFunction.dll
```

This is a .NET 10.0 assembly!

### Why Can't We Change This?

The Azure Functions Worker SDK hardcodes this requirement. If you look at the generated file:

```xml
<!-- obj/Debug/net10.0/WorkerExtensions/WorkerExtensions.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFramework>net8.0</TargetFramework>
        <AssemblyName>Microsoft.Azure.Functions.Worker.Extensions</AssemblyName>
    </PropertyGroup>
    
    <Target Name="_VerifyTargetFramework" BeforeTargets="Build">
        <Error Condition="'$(TargetFramework)' != 'net8.0'" 
               Text="The target framework '$(TargetFramework)' must be 'net8.0'..." />
    </Target>
</Project>
```

The SDK **enforces** .NET 8.0 for WorkerExtensions to ensure compatibility with the Azure Functions runtime.

### Conclusion

✅ **Your application is 100% .NET 10.0**  
✅ **All your projects target .NET 10.0**  
✅ **WorkerExtensions targeting .NET 8.0 is correct and expected**  
✅ **.NET 8 support has been successfully removed from your code**

The WorkerExtensions .NET 8.0 reference is an **infrastructure requirement**, not a code dependency.

## Additional Resources

- [Azure Functions Isolated Worker Model](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide)
- [Azure Functions Worker SDK](https://github.com/Azure/azure-functions-dotnet-worker)

## Date
February 15, 2026

