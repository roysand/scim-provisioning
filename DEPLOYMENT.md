# .NET 10 Deployment Guide

## Overview
This guide provides instructions for deploying the SCIM Provisioning API to a .NET 10 environment.

## Prerequisites
- .NET 10.0 SDK (for building)
- .NET 10.0 Runtime (for running)
- Docker (optional, for containerized deployment)
- Kubernetes (optional, for orchestrated deployment)

## Deployment Options

### Option 1: Direct .NET 10 Deployment

#### Requirements
- Linux/Windows/macOS system with .NET 10 runtime installed
- SQL Server instance (configured in appsettings.json)

#### Steps

1. **Build the application:**
   ```bash
   dotnet build -c Release
   ```

2. **Publish the API:**
   ```bash
   dotnet publish ScimProvisioning.Api/ScimProvisioning.Api.csproj -c Release -o ./publish
   ```

3. **Configure application:**
   - Update `appsettings.json` with production settings
   - Configure connection strings
   - Set environment variables

4. **Run the application:**
   ```bash
   cd ./publish
   dotnet ScimProvisioning.Api.dll
   ```

5. **Verify deployment:**
   ```bash
   curl http://localhost:8080/health  # or appropriate health check endpoint
   ```

### Option 2: Docker Deployment

#### Requirements
- Docker and Docker Compose
- Docker registry (Docker Hub, Azure Container Registry, etc.)

#### Build and Run Docker Image

1. **Build Docker image:**
   ```bash
   docker build -t scim-provisioning:latest .
   ```

2. **Run Docker container:**
   ```bash
   docker run -d \
     -p 8080:8080 \
     -e ConnectionStrings__DefaultConnection="Server=<server>;Database=<db>;..." \
     --name scim-api \
     scim-provisioning:latest
   ```

3. **Verify container:**
   ```bash
   docker logs scim-api
   curl http://localhost:8080/health
   ```

4. **Push to registry:**
   ```bash
   docker tag scim-provisioning:latest <registry>/scim-provisioning:latest
   docker push <registry>/scim-provisioning:latest
   ```

### Option 3: Kubernetes Deployment

#### Requirements
- Kubernetes cluster (v1.24+)
- kubectl configured
- Docker image in accessible registry

#### Sample Kubernetes Manifests

1. **Create Namespace:**
   ```bash
   kubectl create namespace scim-provisioning
   ```

2. **Create ConfigMap for configuration:**
   ```yaml
   apiVersion: v1
   kind: ConfigMap
   metadata:
     name: scim-api-config
     namespace: scim-provisioning
   data:
     ASPNETCORE_ENVIRONMENT: Production
     ASPNETCORE_URLS: "http://+:8080"
   ```

3. **Create Deployment:**
   ```yaml
   apiVersion: apps/v1
   kind: Deployment
   metadata:
     name: scim-api
     namespace: scim-provisioning
   spec:
     replicas: 3
     selector:
       matchLabels:
         app: scim-api
     template:
       metadata:
         labels:
           app: scim-api
       spec:
         containers:
         - name: scim-api
           image: <registry>/scim-provisioning:latest
           ports:
           - containerPort: 8080
           envFrom:
           - configMapRef:
               name: scim-api-config
           env:
           - name: ConnectionStrings__DefaultConnection
             valueFrom:
               secretKeyRef:
                 name: scim-db-connection
                 key: connection-string
           resources:
             requests:
               cpu: "500m"
               memory: "512Mi"
             limits:
               cpu: "1000m"
               memory: "1Gi"
           livenessProbe:
             httpGet:
               path: /health
               port: 8080
             initialDelaySeconds: 30
             periodSeconds: 10
           readinessProbe:
             httpGet:
               path: /health/ready
               port: 8080
             initialDelaySeconds: 5
             periodSeconds: 5
   ```

4. **Create Service:**
   ```yaml
   apiVersion: v1
   kind: Service
   metadata:
     name: scim-api-service
     namespace: scim-provisioning
   spec:
     type: LoadBalancer
     selector:
       app: scim-api
     ports:
     - protocol: TCP
       port: 80
       targetPort: 8080
   ```

5. **Deploy to Kubernetes:**
   ```bash
   kubectl apply -f deployment.yaml
   kubectl apply -f service.yaml
   ```

## Environment Configuration

### Key Environment Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Execution environment | `Production` |
| `ASPNETCORE_URLS` | Server URLs | `http://+:8080` |
| `ConnectionStrings__DefaultConnection` | Database connection string | `Server=...;Database=...` |

### appsettings.Production.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://+:8080"
      }
    }
  }
}
```

## Health Checks

The API should expose health check endpoints:
- `GET /health` - Overall health status
- `GET /health/ready` - Readiness check

## Database Migrations

Run Entity Framework Core migrations:

```bash
# Using dotnet CLI
dotnet ef database update --project ScimProvisioning.Infrastructure

# Or from the published application directory
dotnet ScimProvisioning.Api.dll migrate
```

## Performance Tuning for Production

### Recommended Settings

```json
{
  "Kestrel": {
    "Limits": {
      "MaxRequestBodySize": 10485760,
      "RequestHeadersTimeout": "00:00:30"
    },
    "Endpoints": {
      "Http": {
        "Url": "http://+:8080"
      },
      "Https": {
        "Url": "https://+:8443"
      }
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Max Pool Size=20;Connection Lifetime=300;"
  }
}
```

### Docker Resource Limits

For Docker deployments:
- **CPU**: 1-2 cores recommended
- **Memory**: 512MB minimum, 1-2GB recommended
- **Storage**: 2-5GB for logs and temp files

### Kubernetes Resource Limits

```yaml
resources:
  requests:
    cpu: "500m"      # 0.5 CPU
    memory: "512Mi"   # 512MB
  limits:
    cpu: "2000m"     # 2 CPU cores
    memory: "2Gi"     # 2GB
```

## Monitoring and Logging

### Logs to Monitor

1. Application logs - Check for errors and warnings
2. Database connection logs - Verify database connectivity
3. Performance metrics - Monitor response times

### Recommended Logging Configuration

```csharp
// In Program.cs
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
// Add Application Insights or other monitoring
```

## Troubleshooting

### Common Issues

1. **Port already in use**
   ```bash
   # Change port in environment variable
   export ASPNETCORE_URLS=http://+:8081
   ```

2. **Database connection issues**
   - Verify connection string
   - Check network connectivity to database
   - Validate credentials

3. **Container startup fails**
   ```bash
   docker logs <container-id>
   ```

### Rollback Procedure

For Kubernetes:
```bash
kubectl rollout history deployment/scim-api -n scim-provisioning
kubectl rollout undo deployment/scim-api -n scim-provisioning --to-revision=<revision>
```

## CI/CD Integration

### GitHub Actions
See `.github/workflows/dotnet.yml` for automated build and test pipeline.

### Azure DevOps
See `azure-pipelines.yml` for Azure DevOps pipeline configuration.

## Security Considerations

1. **HTTPS in Production**: Ensure TLS/SSL certificates are configured
2. **Secrets Management**: Use environment-specific secrets, not in appsettings
3. **Database Encryption**: Enable encryption at rest and in transit
4. **API Authentication**: Configure JWT or OAuth as needed

## Verification Checklist

- [ ] .NET 10 runtime installed on target environment
- [ ] Database connectivity tested
- [ ] Environment variables configured
- [ ] Health check endpoints responding
- [ ] Logs being generated appropriately
- [ ] Database migrations completed
- [ ] Load balancer (if applicable) routing to API
- [ ] SSL/TLS certificates configured (if using HTTPS)
- [ ] Monitoring and alerting configured

## Support and Documentation

- [.NET 10 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [ASP.NET Core Hosting](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [Kubernetes Documentation](https://kubernetes.io/docs/)

