# EnterpriseCRM

Enterprise Customer Relationship Management (CRM) system built with .NET 8.0 and Clean Architecture principles.

## Tech Stack

- **.NET 8.0** - Target framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core 8.0** - ORM with SQL Server
- **ASP.NET Core Identity** - Authentication and authorization
- **JWT Bearer Authentication** - Token-based authentication
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Request validation
- **AutoMapper** - Object-to-object mapping
- **Serilog** - Structured logging (Console and File)
- **Swagger/OpenAPI** - API documentation with API Versioning
- **Health Checks** - Application health monitoring

## Project Structure

The solution follows Clean Architecture principles with the following layers:

```
EnterpriseCRM/
├── EnterpriseCRM.Domain/          # Domain layer - Entities and domain models
│   ├── AppUser.cs
│   ├── AppRole.cs
│   ├── Contact.cs
│   ├── Lead.cs
│   ├── BaseEntity.cs
│   └── ...
├── EnterpriseCRM.Application/      # Application layer - Business logic and use cases
│   ├── Features/                   # CQRS handlers organized by feature
│   │   ├── Contacts/
│   │   └── Leads/
│   ├── Interfaces/                 # Repository interfaces
│   ├── Mapping/                    # AutoMapper profiles
│   └── Behaviors/                  # MediatR pipeline behaviors (e.g., ValidationBehavior)
├── EnterpriseCRM.Infrastructure/   # Infrastructure layer - Data access and external services
│   ├── Persistence/                # DbContext, Repositories, UnitOfWork
│   └── Migrations/                 # Entity Framework migrations
├── EnterpriseCRM.API/              # Presentation layer - Controllers and API endpoints
│   ├── Controllers/                # API controllers
│   │   ├── AuthController.cs
│   │   ├── ContactsController.cs
│   │   ├── LeadsController.cs
│   │   ├── UsersController.cs
│   │   ├── BlogPostsController.cs
│   │   ├── MediaController.cs
│   │   └── SystemController.cs
│   ├── DTOs/                       # Data Transfer Objects organized by feature
│   │   ├── Auth/
│   │   ├── Contacts/
│   │   ├── Leads/
│   │   ├── Users/
│   │   ├── BlogPosts/
│   │   ├── Media/
│   │   └── System/
│   ├── Swagger/                    # Swagger configuration
│   └── wwwroot/                    # Static files (media uploads)
└── EnterpriseCRM.Tests/            # Test project
    └── Controllers/                # Controller tests
```

## Installation

### Prerequisites

- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** (or SQL Server LocalDB) - For database
- **Visual Studio 2022** or **VS Code** with C# extension (recommended)

### Setup Steps

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd EnterpriseCRM
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update database connection string** (if needed)
   
   Edit `EnterpriseCRM.API/appsettings.json` and update the `ConnectionStrings:DefaultConnection` value to match your SQL Server instance.

4. **Apply database migrations**
   ```bash
   cd EnterpriseCRM.Infrastructure
   dotnet ef database update --startup-project ../EnterpriseCRM.API
   ```

   Or from the solution root:
   ```bash
   dotnet ef database update --project EnterpriseCRM.Infrastructure --startup-project EnterpriseCRM.API
   ```

## Running the Application

### Development Mode

**Option 1: Using dotnet CLI**
```bash
cd EnterpriseCRM.API
dotnet run
```

**Option 2: Using dotnet watch (auto-reload on changes)**
```bash
cd EnterpriseCRM.API
dotnet watch run
```

**Option 3: Using Visual Studio**
- Open `EnterpriseCRM.sln`
- Set `EnterpriseCRM.API` as the startup project
- Press `F5` or click "Run"

The application will start and be available at:
- **HTTP**: `http://localhost:5163`
- **HTTPS**: `https://localhost:7230`
- **Swagger UI**: `https://localhost:7230/swagger` (or `http://localhost:5163/swagger`)

### Production Mode

```bash
cd EnterpriseCRM.API
dotnet publish -c Release
cd bin/Release/net8.0/publish
dotnet EnterpriseCRM.API.dll
```

## Configuration

### Environment Variables

The application uses `appsettings.json` and `appsettings.Development.json` for configuration. Key settings include:

#### Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EnterpriseCRM;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

#### JWT Settings
```json
{
  "JwtSettings": {
    "SecurityKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong",
    "ValidIssuer": "EnterpriseCRM",
    "ValidAudience": "EnterpriseCRM",
    "ExpiryMinutes": 60,
    "RefreshTokenValidityInDays": 7
  }
}
```

**⚠️ Important**: Change the `SecurityKey` in production to a secure, randomly generated key (at least 32 characters).

#### Serilog Configuration
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": { "path": "logs/log-.txt", "rollingInterval": "Day" }
      }
    ]
  }
}
```

Logs are written to:
- **Console** - Standard output
- **File** - `EnterpriseCRM.API/logs/log-{date}.txt` (daily rolling)

### API Versioning

The API uses URL-based versioning. All endpoints are prefixed with `/api/v{version}/`:
- Example: `/api/v1.0/auth/login`
- Default version: `1.0`

## API Documentation

### Swagger UI

Once the application is running, access the Swagger UI at:
- `https://localhost:7230/swagger` (HTTPS)
- `http://localhost:5163/swagger` (HTTP)

The Swagger UI provides:
- Interactive API documentation
- Endpoint testing capabilities
- JWT authentication support (use the "Authorize" button)

### Health Checks

Health check endpoint:
- `GET /health` - Returns application and database health status

### Main API Endpoints

#### Authentication (`/api/v1.0/auth`)
- `POST /register` - Register a new user
- `POST /login` - Authenticate and receive JWT token
- `POST /refresh-token` - Refresh access token

#### Contacts (`/api/v1.0/contacts`)
- CRUD operations for contacts

#### Leads (`/api/v1.0/leads`)
- CRUD operations for leads

#### Users (`/api/v1.0/users`)
- User management operations

#### Blog Posts (`/api/v1.0/blogposts`)
- Blog post management

#### Media (`/api/v1.0/media`)
- Media file upload and management

#### System (`/api/v1.0/system`)
- System information and utilities

## Testing

### Run Tests

```bash
dotnet test
```

Or for a specific test project:
```bash
cd EnterpriseCRM.Tests
dotnet test
```

### Test Structure

Tests are organized in `EnterpriseCRM.Tests` with:
- Controller tests for each API controller
- Custom WebApplicationFactory for integration testing
- Test helpers for common test scenarios

## Development Guidelines

### Adding New Features

1. **Domain Layer**: Add entities in `EnterpriseCRM.Domain`
2. **Application Layer**: 
   - Create feature folder in `EnterpriseCRM.Application/Features/`
   - Add CQRS handlers (Commands/Queries)
   - Add validation rules using FluentValidation
   - Add mapping profiles in `Mapping/`
3. **Infrastructure Layer**: 
   - Add repository implementations if needed
   - Create migrations: `dotnet ef migrations add MigrationName --project EnterpriseCRM.Infrastructure --startup-project EnterpriseCRM.API`
4. **API Layer**: 
   - Add controller in `EnterpriseCRM.API/Controllers/`
   - Add DTOs in `EnterpriseCRM.API/DTOs/{FeatureName}/`
   - Use API versioning attributes

### Code Style

- Follow C# coding conventions
- Use nullable reference types (enabled)
- Use dependency injection for all services
- Implement repository pattern for data access
- Use MediatR for CQRS pattern
- Validate all inputs using FluentValidation

## Database Migrations

### Create a new migration
```bash
dotnet ef migrations add MigrationName --project EnterpriseCRM.Infrastructure --startup-project EnterpriseCRM.API
```

### Apply migrations
```bash
dotnet ef database update --project EnterpriseCRM.Infrastructure --startup-project EnterpriseCRM.API
```

### Remove last migration (if not applied)
```bash
dotnet ef migrations remove --project EnterpriseCRM.Infrastructure --startup-project EnterpriseCRM.API
```

## Troubleshooting

### Database Connection Issues

- Ensure SQL Server (or LocalDB) is running
- Verify the connection string in `appsettings.json`
- Check that the database exists or migrations have been applied

### JWT Authentication Issues

- Verify `JwtSettings:SecurityKey` is configured in `appsettings.json`
- Ensure the key is at least 32 characters long
- Check token expiration settings

### Port Already in Use

If port 5163 or 7230 is already in use:
- Change ports in `launchSettings.json`
- Or stop the process using the port