# SCIM Provisioning Solution

A comprehensive SCIM (System for Cross-domain Identity Management) 2.0 API provisioning solution built with .NET 8, implementing clean architecture, outbox pattern, and Azure Service Bus integration.

## 🏗️ Architecture

This solution implements **Clean Architecture** with the following layers:

- **Core**: Domain models, entities, value objects, domain events
- **Application**: Use cases, commands, queries, validators, DTOs
- **Infrastructure**: EF Core, repositories, Azure Service Bus, data access
- **API**: FastEndpoints-based REST API with SCIM 2.0 endpoints
- **Library**: Reusable NuGet library for easy integration
- **AzureFunction**: Service Bus triggered function for outbox message processing

## 📦 Projects

### ScimProvisioning.Core
Domain layer containing:
- **Entities**: `ScimUser`, `ScimGroup`, `OutboxMessage`, `AuditLog`
- **Value Objects**: `Email`, `PhoneNumber`
- **Domain Events**: User and Group provisioning events
- **Result Pattern**: For error handling without exceptions

### ScimProvisioning.Application
Application layer with:
- **Use Cases**: CreateUser, UpdateUser, DeleteUser, ListUsers, CreateGroup, etc.
- **DTOs**: Request/Response models for API
- **Validators**: FluentValidation validators
- **AutoMapper Profiles**: Entity to DTO mappings

### ScimProvisioning.Infrastructure
Infrastructure layer providing:
- **EF Core DbContext**: Database access with SQL Server
- **Repositories**: User and Group repositories
- **Outbox Service**: Reliable event publishing
- **Audit Log Service**: Operation tracking
- **Azure Service Bus**: Event publishing to Service Bus

### ScimProvisioning.Api
FastEndpoints-based API with:
- **User Endpoints**: `/scim/v2/Users` (POST, GET, PATCH, DELETE)
- **Group Endpoints**: `/scim/v2/Groups` (POST, GET, PATCH, DELETE)
- **SCIM Metadata**: ServiceProviderConfig, ResourceTypes, Schemas
- **Swagger Documentation**: Auto-generated API docs

### ScimProvisioning.Library
Reusable library for integration:
```csharp
services.AddScimProvisioning(
    connectionString: "your-connection-string",
    serviceBusConnectionString: "your-service-bus-connection",
    serviceBusQueueName: "scim-events"
);
```

### ScimProvisioning.AzureFunction
Azure Function for processing outbox messages:
- Service Bus triggered
- Processes outbox messages asynchronously
- Updates audit logs
- Implements idempotency and retry logic

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or SQL Server instance)
- Azure Service Bus (optional, for message processing)
- Visual Studio 2022 or VS Code

### Setup

1. **Clone the repository**
```bash
git clone https://github.com/roysand/scim-provisioning.git
cd scim-provisioning
```

2. **Update connection strings**

Edit `ScimProvisioning.Api/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ScimProvisioning;Trusted_Connection=True;"
  },
  "ServiceBus": {
    "ConnectionString": "your-service-bus-connection-string",
    "QueueName": "scim-events"
  }
}
```

3. **Create database and run migrations**
```bash
cd ScimProvisioning.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../ScimProvisioning.Api
dotnet ef database update --startup-project ../ScimProvisioning.Api
```

4. **Run the API**
```bash
cd ScimProvisioning.Api
dotnet run
```

5. **Access Swagger UI**
Navigate to: `https://localhost:5001/swagger`

## 📝 SCIM API Endpoints

### Users
- `POST /scim/v2/Users` - Create a new user
- `GET /scim/v2/Users` - List users with filtering and pagination
- `GET /scim/v2/Users/{id}` - Get user by ID
- `PATCH /scim/v2/Users/{id}` - Update user
- `DELETE /scim/v2/Users/{id}` - Delete user

### Groups
- `POST /scim/v2/Groups` - Create a new group
- `GET /scim/v2/Groups` - List groups
- `GET /scim/v2/Groups/{id}` - Get group by ID
- `PATCH /scim/v2/Groups/{id}` - Update group
- `DELETE /scim/v2/Groups/{id}` - Delete group

### SCIM Metadata
- `GET /scim/v2/ServiceProviderConfig` - Service provider configuration
- `GET /scim/v2/ResourceTypes` - Available resource types
- `GET /scim/v2/Schemas` - SCIM schemas

### Example: Create User
```bash
POST /scim/v2/Users
Content-Type: application/json

{
  "externalId": "user123",
  "userName": "john.doe",
  "displayName": "John Doe",
  "primaryEmail": "john.doe@example.com",
  "active": true
}
```

## 🔧 Configuration

### Database Configuration
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ScimProvisioning;User Id=sa;Password=YourPassword;"
  }
}
```

### Service Bus Configuration (Optional)
```json
{
  "ServiceBus": {
    "ConnectionString": "Endpoint=sb://namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=key",
    "QueueName": "scim-events"
  }
}
```

## 🏛️ Design Patterns

### Outbox Pattern
- Ensures reliable event publishing
- Transactional consistency between DB operations and events
- Messages stored in `OutboxMessages` table
- Azure Function processes messages asynchronously

### Repository Pattern
- Abstraction over data access
- `IScimUserRepository`, `IScimGroupRepository`
- Unit of Work for transaction management

### CQRS
- Separation of Commands (Create, Update, Delete)
- Queries (Get, List) for read operations

### Result Pattern
- Error handling without exceptions
- `Result<T>` type for success/failure
- Clean error propagation

## 📊 Database Schema

### Tables
- **Users**: SCIM user data
- **Groups**: SCIM group data
- **GroupMembers**: Group membership relationships
- **OutboxMessages**: Event outbox for reliable publishing
- **AuditLogs**: Audit trail for all operations

### Indexes
- Unique indexes on ExternalId, UserName
- Performance indexes on frequently queried fields

## 🧪 Testing

Run tests:
```bash
cd ScimProvisioning.Tests
dotnet test
```

## 📦 NuGet Packaging

Build as NuGet package:
```bash
cd ScimProvisioning.Library
dotnet pack --configuration Release
```

## 🐳 Docker

Build Docker image:
```bash
docker build -t scim-provisioning-api -f ScimProvisioning.Api/Dockerfile .
```

Run container:
```bash
docker run -p 8080:8080 -e ConnectionStrings__DefaultConnection="your-connection" scim-provisioning-api
```

## 📖 Documentation

- **SCIM RFC 7643**: Schema specification
- **SCIM RFC 7644**: Protocol specification
- **FastEndpoints**: https://fast-endpoints.com/
- **Clean Architecture**: https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html

## 🤝 Contributing

Contributions are welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## 📄 License

This project is licensed under the MIT License.

## 🔒 Security

- Always use HTTPS in production
- Implement authentication (OAuth 2.0, Bearer tokens)
- Validate all inputs
- Use parameterized queries (EF Core handles this)
- Regular security updates

## 📞 Support

For questions or issues, please open an issue on GitHub.

## 🎯 Roadmap

- [ ] Add full SCIM filter support (RFC 7644 section 3.4.2)
- [ ] Implement bulk operations
- [ ] Add ETags for optimistic concurrency
- [ ] Support for custom schema extensions
- [ ] Performance optimizations
- [ ] Enhanced monitoring and observability
