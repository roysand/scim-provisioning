# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ScimProvisioning.slnx ./
COPY ScimProvisioning.Core/ScimProvisioning.Core.csproj ScimProvisioning.Core/
COPY ScimProvisioning.Application/ScimProvisioning.Application.csproj ScimProvisioning.Application/
COPY ScimProvisioning.Infrastructure/ScimProvisioning.Infrastructure.csproj ScimProvisioning.Infrastructure/
COPY ScimProvisioning.Api/ScimProvisioning.Api.csproj ScimProvisioning.Api/

# Restore dependencies
RUN dotnet restore ScimProvisioning.Api/ScimProvisioning.Api.csproj

# Copy all source code
COPY . .

# Build the application
WORKDIR /src/ScimProvisioning.Api
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ScimProvisioning.Api.dll"]
