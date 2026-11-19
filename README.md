# 💚 DonationHub

**Donation Management & Crowdfunding Platform**

A backend REST API for managing charitable campaigns and donations.

![.NET 10.0](https://img.shields.io/badge/dotnet_10-10.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/Database-SQL_Server-CC2927?style=flat-square&logo=microsoftsqlserver&logoColor=white)
![Architecture](https://img.shields.io/badge/Architecture-Clean_Architecture-0B7285?style=flat-square)
![CQRS](https://img.shields.io/badge/Pattern-CQRS-198754?style=flat-square)

## Table of Contents

1. [📖 Overview](#overview)
2. [📋 Prerequisites](#prerequisites)
3. [📁 Project Structure](#project-structure)
4. [🚀 Getting Started](#getting-started)
5. [🔐 Configuration & User Secrets](#configuration--user-secrets)
6. [🗄️ Database Setup](#database-setup)
7. [🌱 Database Seeding](#database-seeding)
8. [▶️ Running the API](#running-the-api)
9. [📚 API Documentation & Versioning](#api-documentation--versioning)
10. [⚠️ Global Exception Handling & RFC 9457 ProblemDetails](#global-exception-handling--rfc-9457-problemdetails)
11. [✨ Key Implementation Highlights](#key-implementation-highlights)
12. [🏗️ Design Decisions](#design-decisions)
13. [🎯 Sprint 1 & Sprint 2 Scope](#sprint-1--sprint-2-scope)

<a id="overview"></a>
## 📖 Overview

DonationHub is a backend REST API platform designed to manage charitable fundraising campaigns and donations.

The platform connects donors with charitable campaigns, allowing them to browse campaigns, contribute financially, receive donation receipts, and track their donation history. It also provides campaign management capabilities and administrative tools for managing users, categories, campaigns, and donations.

### User Roles

| Role | Responsibilities |
|------|------------------|
| **Anonymous** | Browse active campaigns, view campaign details, categories, and success stories, and register or log in. |
| **Donor** | Browse campaigns, make donations, view personal donation history, download receipts, and manage their profile. |
| **Campaign Manager** | Perform donor operations and create, update, close, and soft-delete their own campaigns. |
| **Admin** | Manage users, staff, categories, campaigns, donations, and administrative operations. |

> [!NOTE]
> **Project scope:** DonationHub is a backend-only solution. The frontend is outside the project scope, and Stripe is intended to operate in test mode rather than process real payments.

<a id="prerequisites"></a>
## 📋 Prerequisites

Before running the project, make sure you have:

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server
- Git

<a id="project-structure"></a>
## 📁 Project Structure

The solution follows Clean Architecture and is organized into four projects:

| Project | Responsibility |
|---------|----------------|
| **DonationHub.Api** | HTTP entry point, controllers, request contracts, API versioning, and OpenAPI configuration. |
| **DonationHub.Application** | Application use cases organized by features, CQRS handlers, validation, and application abstractions. |
| **DonationHub.Domain** | Core business entities, value objects, enums, domain errors, and Result abstractions. |
| **DonationHub.Infrastructure** | Persistence, EF Core configurations, repositories, Identity, JWT, migrations, and database seeding. |

```text
DonationHub/
└── src/
    ├── DonationHub.Api/
    │   ├── Contracts/
    │   │   ├── Admin/
    │   │   ├── Auth/
    │   │   └── Category/
    │   ├── Controllers/
    │   ├── Infrastructure/
    │   ├── OpenApi/
    │   │   └── Transformers/
    │   ├── Properties/
    │   └── Services/
    │
    ├── DonationHub.Application/
    │   ├── Common/
    │   │   ├── ApplicationErrors/
    │   │   ├── Behaviours/
    │   │   ├── Interfaces/
    │   │   └── Options/
    │   └── Features/
    │       ├── Admin/
    │       │   └── StaffManagement/
    │       │       ├── Commands/
    │       │       │   └── CreateStaff/
    │       │       └── Dtos/
    │       ├── Categories/
    │       │   ├── Commands/
    │       │   │   ├── Create/
    │       │   │   ├── Delete/
    │       │   │   └── Update/
    │       │   ├── Dtos/
    │       │   └── Queries/
    │       │       ├── GetAll/
    │       │       └── GetById/
    │       └── SecurityManager/
    │           ├── Authentication/
    │           │   ├── Commands/
    │           │   │   ├── ChangePassword/
    │           │   │   ├── Login/
    │           │   │   ├── Logout/
    │           │   │   ├── Refresh/
    │           │   │   └── Register/
    │           │   ├── DTOs/
    │           │   └── Queries/
    │           │       └── Me/
    │           └── Authorization/
    │
    ├── DonationHub.Domain/
    │   ├── Common/
    │   │   ├── Interfaces/
    │   │   └── Results/
    │   │       └── Abstractions/
    │   ├── Entities/
    │   │   ├── Campaigns/
    │   │   ├── Categories/
    │   │   ├── Donations/
    │   │   ├── Identity/
    │   │   └── PaymentAttempts/
    │   ├── Enums/
    │   └── ValueObjects/
    │
    └── DonationHub.Infrastructure/
        ├── Identity/
        │   └── Jwt/
        ├── Migrations/
        └── Persistence/
            ├── Configurations/
            ├── Interceptors/
            ├── Repositories/
            └── Seed/
```

### DonationHub.Domain

Contains the core business model and domain rules.

- **Common:** Base entity, value object, soft-delete abstraction, and Result Pattern types.
- **Entities:** Domain entities such as `Category`, `Campaign`, `Donation`, `RefreshToken`, and `PaymentAttempt`.
- **Enums:** Domain status enumerations.
- **ValueObjects:** Value objects such as `Money`.

The Domain project does not depend on the other application layers or infrastructure packages.

### DonationHub.Application

Contains application use cases and business orchestration.

- **Common/Interfaces:** Abstractions such as `IAppDbContext`, `IGenericRepository`, `IUnitOfWork`, and identity-related interfaces.
- **Common/Options:** Configuration options such as `DonationOptions`.
- **Common/Behaviours:** MediatR pipeline behaviors, including automatic validation.
- **Features:** Feature-based folders containing commands, queries, handlers, validators, and DTOs.

### DonationHub.Infrastructure

Contains implementations of infrastructure-related concerns.

- **Identity:** ASP.NET Core Identity services and user management.
- **Identity/Jwt:** JWT token generation and related configuration.
- **Persistence:** EF Core `DbContext`, entity configurations, migrations, repositories, and Unit of Work.
- **Persistence/Interceptors:** EF Core interceptors, including soft-delete handling.
- **Persistence/Seed:** Initial and development database seeding.

### DonationHub.Api

The API entry point and HTTP presentation layer.

- **Controllers:** API controllers.
- **Contracts:** HTTP request contracts.
- **Infrastructure:** Global exception handling.
- **OpenApi/Transformers:** OpenAPI document transformers.
- **Services:** API-level services such as the current-user implementation.

<a id="getting-started"></a>
## 🚀 Getting Started

### Clone the Repository

```bash
git clone <REPOSITORY_URL>
cd DonationHub/Backend
```

Replace `<REPOSITORY_URL>` with the actual repository URL.

<a id="configuration--user-secrets"></a>
## 🔐 Configuration & User Secrets

### User Secrets

The project uses .NET User Secrets to keep sensitive configuration values outside the source code.

Initialize User Secrets for the API project:

```bash
dotnet user-secrets init --project src/DonationHub.Api
```

Configure the required secrets:

```bash
dotnet user-secrets set "SeedAdmin:Password" "Your_Admin_Password" --project src/DonationHub.Api

dotnet user-secrets set "Jwt:SecretKey" "Your_SecretKey_at_least_32_characters" --project src/DonationHub.Api

dotnet user-secrets set "DevelopmentSeed:Donor:Password" "Your_Donor_Password" --project src/DonationHub.Api

dotnet user-secrets set "DevelopmentSeed:CampaignManager:Password" "Your_CampaignManager_Password" --project src/DonationHub.Api

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Your_SQL_Server_Connection_String" --project src/DonationHub.Api
```

The initial Admin account settings and development test-user information are configured through application settings files:

| File | Configuration |
|------|---------------|
| `appsettings.json` | Initial Admin configuration. |
| `appsettings.Development.json` | Development test-user configuration. |

Passwords, the JWT signing key, and the database connection string should be supplied through User Secrets or another secure configuration provider.

> [!WARNING]
> **Security:** Never commit real passwords, JWT keys, or database credentials to the repository.

### Donation Options Defaults

Donation-related settings are defined in `DonationOptions` under the `Donation` configuration section.

The required default configuration is:

```json
{
  "Donation": {
    "MinAmount": 1.00,
    "MaxAmount": 10000.00,
    "PendingTimeoutMinutes": 30,
    "DefaultCurrency": "USD"
  }
}
```

| Setting | Default | Description |
|---------|---------|-------------|
| `MinAmount` | `1.00` | Minimum donation amount. |
| `MaxAmount` | `10000.00` | Maximum donation amount. |
| `PendingTimeoutMinutes` | `30` | Pending donation timeout in minutes. |
| `DefaultCurrency` | `USD` | Default three-letter uppercase currency code. |

#### Validation

The `DonationOptions` class uses Data Annotations and cross-property validation:

- Donation amounts must be within the configured range.
- `PendingTimeoutMinutes` must be greater than zero.
- `DefaultCurrency` is required and must contain exactly three uppercase letters.
- `MinAmount` must be less than or equal to `MaxAmount`.

#### Registration

The options are registered using the Options Pattern:

```csharp
services.AddOptions<DonationOptions>()
    .BindConfiguration(DonationOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

`ValidateOnStart()` ensures invalid configuration is detected during application startup.

<a id="database-setup"></a>
## 🗄️ Database Setup

DonationHub uses Entity Framework Core with SQL Server and a code-first migration approach.

Apply the existing migrations to create or update the database:

```bash
dotnet ef database update --project src/DonationHub.Infrastructure --startup-project src/DonationHub.Api
```

The initial migration is:

```text
20261110212708_InitialCreate
```

The database is automatically seeded when the application starts successfully.

<a id="database-seeding"></a>
## 🌱 Database Seeding

DonationHub implements database seeding in the Infrastructure layer. Seeding is divided into three classes with separate responsibilities:

| Class | Responsibility |
|-------|----------------|
| `DatabaseSeeder` | Coordinates the seeding process according to the current environment. |
| `InitialDataSeeder` | Creates the required roles and initial Admin account in all environments. |
| `DevelopmentSeeder` | Creates development test users and sample categories in the Development environment only. |

### Seeding Execution

Seeding runs during application startup, before the API begins accepting requests.

1. The application is built.
2. An asynchronous dependency injection scope is created.
3. `DatabaseSeeder` is resolved and executed.
4. `InitialDataSeeder` creates the required roles and initial Admin account.
5. If the environment is Development, `DevelopmentSeeder` creates development test users and sample categories.
6. The application starts accepting requests after successful seeding.

If a required database or seeding operation fails, the exception propagates and application startup fails.

### Initial Data

`InitialDataSeeder` runs in all environments.

It is responsible for:

- Creating the `Admin`, `CampaignManager`, and `Donor` roles if they do not already exist.
- Creating the initial Admin account using `SeedAdminOptions`.
- Assigning the `Admin` role to the initial account.

Existing accounts are not recreated, and their passwords or profile information are not reset. If the configured Admin account already exists but does not have the `Admin` role, the role is assigned without removing any existing roles.

### Development Data

`DevelopmentSeeder` runs only in the Development environment.

It creates:

- A development Campaign Manager account.
- A development Donor account.
- Five sample categories with Arabic and English names.

The seeder checks whether users and categories already exist before creating them. Existing users are not recreated or modified; missing roles are assigned when necessary.

Categories are created through the `Category.Create` domain method and persisted using `AppDbContext`. Existing categories are detected using their English names, while unique database indexes on `NameEn` and `NameAr` provide additional duplicate protection.

### Identity and Reliability

- Identity users and roles are managed through ASP.NET Core Identity APIs, including `UserManager` and `RoleManager`, rather than modifying Identity tables directly.
- Every `IdentityResult` is checked. Failed operations report the relevant Identity error codes and descriptions.
- Creating a new user and assigning the corresponding role are handled within a transaction, so a failed role assignment does not leave a newly created user without the required role.
- Seeding is designed to be idempotent: running it again skips existing data and creates only missing records.
- Startup failures are logged at the application startup boundary and rethrown. These failures are separate from HTTP exception handling because they occur before the application starts processing requests.

<a id="running-the-api"></a>
## ▶️ Running the API

Run the API project:

```bash
dotnet run --project src/DonationHub.Api
```

The application will display its listening URLs in the terminal.

For example, during local development, the API was verified at:

```text
https://localhost:7141
```

The actual port may differ depending on your local launch configuration.

<a id="api-documentation--versioning"></a>
## 📚 API Documentation & Versioning

DonationHub uses Scalar with OpenAPI to provide interactive API documentation and allow developers to explore and test API endpoints, including protected endpoints that require JWT Bearer authentication.

### API Versioning

The API uses URL-segment versioning through `Asp.Versioning`.

- Configured API version `v1` as the default version.
- Used `UrlSegmentApiVersionReader` to identify the API version from the request URL.
- Enabled API version reporting.
- Configured API Explorer to group endpoints by version and substitute the version in route templates.

API endpoints follow this format:

```text
/api/v1/{controller}/{action}
```

### OpenAPI Configuration

OpenAPI document generation is integrated with API Versioning.

- A separate OpenAPI document is configured for each API version.
- `VersionInfoTransformer` customizes the generated document title and version information.
- `BearerSecuritySchemeTransformer` defines JWT Bearer authentication in the OpenAPI document and applies security requirements to protected endpoints.
- `EnumSchemaTransformer` represents enum values as strings in the generated OpenAPI schemas.

### Scalar Integration

- Installed `Scalar.AspNetCore` and configured Scalar as the interactive API documentation interface.
- Mapped OpenAPI documents and Scalar API Reference inside the Development environment.
- Configured Scalar to display the available API versions.
- Enabled developers to inspect endpoint details, request bodies, responses, and authenticate using JWT Bearer tokens.

Open Scalar in your browser:

```text
https://localhost:{port}/scalar
```

Replace `{port}` with the port displayed when the API starts.

### Testing Protected Endpoints

1. Run the API in the Development environment.
2. Open Scalar at `https://localhost:{port}/scalar`.
3. Execute `POST /api/v1/auth/login` using a development test account.
4. Copy the returned `accessToken`.
5. Enter the token in Scalar's Bearer Authentication section.
6. Execute protected endpoints using the authenticated token.

For example:

```http
GET /api/v1/auth/me
```

A successful response returns the authenticated user's profile information, including their user ID, email, full name, preferred language, creation date, and roles.

> [!IMPORTANT]
> Protected endpoints require a valid JWT access token.

<a id="global-exception-handling--rfc-9457-problemdetails"></a>
## ⚠️ Global Exception Handling & RFC 9457 ProblemDetails

DonationHub implements centralized exception handling using ASP.NET Core's `IExceptionHandler`.

The exception handler catches unexpected, unhandled exceptions and returns a sanitized response using the RFC 9457 Problem Details format.

### Expected and Unexpected Errors

The application distinguishes between two types of failures:

| Failure type | Handling |
|--------------|----------|
| Expected failures | Represented through the Result Pattern and returned by application operations. |
| Unexpected exceptions | Handled centrally by `GlobalExceptionHandler`. |

Both expected Result failures and unexpected exceptions are returned using:

```text
application/problem+json
```

### ProblemDetails Response

Error responses include additional fields required by the API specification:

| Field | Description |
|-------|-------------|
| `type` | URI identifying the HTTP status. |
| `title` | Description of the error. |
| `status` | HTTP status code. |
| `code` | Application error catalogue code. |
| `traceId` | Request correlation identifier. |
| `errors` | Field-to-messages collection for validation failures only. |

Example response for a business error:

```json
{
  "type": "https://httpstatuses.io/422",
  "title": "Goal amount cannot be lower than the raised amount.",
  "status": 422,
  "code": "Campaign.GoalBelowRaised",
  "traceId": "00-9f2d..."
}
```

### Security and Logging

Unexpected exceptions are logged with their full details and stack traces.

The HTTP response does not expose internal implementation details. In particular, a `500` response must not include:

- Exception messages.
- Stack traces.
- SQL statements or database details.

Instead, the response contains a generic error message and the request `traceId`. The trace ID can be used to locate the corresponding log entry and investigate the failure.

Validation failures additionally include an `errors` field containing the relevant validation messages.

The HTTP status code for expected failures is determined by the application's `ErrorKind` mapping.

<a id="key-implementation-highlights"></a>
## ✨ Key Implementation Highlights

This section presents selected implementation details that demonstrate how the project's architectural patterns are applied.

### 1. Result Pattern

The project defines common result abstractions for operations with and without return values.

```csharp
public interface IResult
{
    bool IsSuccess { get; }
    IReadOnlyList<Error> Errors { get; }
}

public interface IResult<out TValue> : IResult
{
    TValue Value { get; }
}
```

- `Result` represents success or failure for operations without a return value.
- `Result<TValue>` represents success with a typed value or failure with errors.
- Both expose a consistent `IsSuccess` property and an error collection.

### 2. Error Catalogue

Errors are represented using a strongly typed `Error` record containing a code, description, and error kind.

```csharp
public readonly record struct Error
{
    public string Code { get; }
    public string Description { get; }
    public ErrorKind Type { get; }
}
```

The `ErrorKind` enum classifies errors such as:

```csharp
public enum ErrorKind
{
    Failure,
    Unexpected,
    Validation,
    PayloadTooLarge,
    UnprocessableEntity,
    TooManyRequests,
    Conflict,
    NotFound,
    Unauthorized,
    Forbidden
}
```

This provides a consistent error representation that can be used by application handlers and mapped by the API layer.

### 3. Centralized HTTP Error Mapping

The API's base `ApiController` maps application error kinds to HTTP status codes.

```csharp
var statusCode = error.Type switch
{
    ErrorKind.Conflict => StatusCodes.Status409Conflict,
    ErrorKind.Validation => StatusCodes.Status400BadRequest,
    ErrorKind.NotFound => StatusCodes.Status404NotFound,
    ErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
    ErrorKind.Forbidden => StatusCodes.Status403Forbidden,
    ErrorKind.UnprocessableEntity => StatusCodes.Status422UnprocessableEntity,
    ErrorKind.PayloadTooLarge => StatusCodes.Status413PayloadTooLarge,
    ErrorKind.TooManyRequests => StatusCodes.Status429TooManyRequests,
    _ => StatusCodes.Status500InternalServerError,
};
```

The API layer uses the mapped status code to create a `ProblemDetails` response and includes the application error code.

### 4. Global Exception Handling

Unexpected exceptions are handled through `IExceptionHandler`.

```csharp
public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken)
{
    logger.LogError(exception,
        "Unhandled exception. TraceId: {TraceId}",
        Activity.Current?.Id ?? httpContext.TraceIdentifier);

    httpContext.Response.StatusCode =
        StatusCodes.Status500InternalServerError;

    return await problemDetailsService.TryWriteAsync(
        new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = { Status = 500 }
        });
}
```

The exception is logged with its details, while the client receives a sanitized `500` response.

### 5. Standardized ProblemDetails

The `AddCustomProblemDetails` configuration standardizes error responses.

```csharp
options.CustomizeProblemDetails = context =>
{
    var problemDetails = context.ProblemDetails;
    var status = problemDetails.Status
        ?? context.HttpContext.Response.StatusCode;

    problemDetails.Status = status;
    problemDetails.Type = $"https://httpstatuses.io/{status}";

    problemDetails.Extensions["traceId"] =
        Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

    if (!problemDetails.Extensions.ContainsKey("code"))
        problemDetails.Extensions["code"] =
            DefaultCode(status, problemDetails);

    if (status >= 500)
    {
        problemDetails.Title = "An unexpected error occurred.";
        problemDetails.Detail = null;
    }
};
```

This provides:

- A consistent `ProblemDetails` structure.
- An HTTP status type URI.
- A request correlation `traceId`.
- An application error code.
- Sanitized server-error responses.

<a id="design-decisions"></a>
## 🏗️ Design Decisions

This section documents important architectural decisions and the reasoning behind them.

### 1. Clean Architecture

**Decision:** Organize the solution into Domain, Application, Infrastructure, and API projects.

**Why:**

- Keeps business rules independent of infrastructure and presentation details.
- Defines clear dependency boundaries between layers.
- Makes the application easier to maintain, test, and extend.
- Allows infrastructure implementations to be replaced without changing the core business model.

**Alternative considered:** Implementing the entire application in a single project or using a traditional layered structure without clear dependency boundaries.

**Why not:** A single project or loosely defined layers can lead to tightly coupled code and make business logic harder to isolate.

### 2. CQRS with MediatR

**Decision:** Use CQRS with MediatR to separate application commands and queries.

Each use case is represented by a command or query with its own handler. API controllers remain thin and delegate application operations through MediatR.

**Why:**

- Separates operations that change application state from operations that retrieve data.
- Gives each use case a clear responsibility.
- Keeps controllers focused on HTTP concerns.
- Provides a consistent place for validation and application behavior through MediatR pipeline behaviors.

**Alternative considered:** Placing application logic directly inside controllers or using a single service method for both read and write operations.

**Why not:** This can mix HTTP concerns with application logic and make use cases harder to maintain as the application grows.

### 3. Result Pattern for Expected Errors

**Decision:** Use `Result` and `Result<T>` to represent expected application and business failures instead of throwing exceptions for normal business outcomes.

**Why:**

- Makes success and failure outcomes explicit in method results.
- Avoids using exceptions as a mechanism for normal control flow.
- Provides a consistent way to represent errors using the application's error catalogue.
- Allows the API layer to map error types to appropriate HTTP status codes through centralized error mapping.
- Avoids the overhead of throwing exceptions for expected failure cases.

The application distinguishes between:

- **Expected errors:** Returned through `Result` or `Result<T>`.
- **Unexpected errors:** Thrown as exceptions and handled by the global exception handler.

**Alternative considered:** Throwing exceptions for all failures, including expected business-rule violations.

**Why not:** Business failures such as a missing category or a rejected operation are normal application outcomes, not unexpected runtime failures. Representing them as Results makes the execution flow clearer and error handling more consistent.

### 4. Global Exception Handling

**Decision:** Use centralized exception handling with `IExceptionHandler` and RFC 9457 ProblemDetails.

**Why:**

- Handles unexpected exceptions consistently in one place.
- Prevents internal exception details from leaking through HTTP responses.
- Provides a consistent error response format.
- Includes `traceId` to correlate an API response with its corresponding log entry.
- Keeps expected application failures separate from unexpected technical failures.

**Alternative considered:** Handling exceptions separately inside every controller or returning raw exception details to the client.

**Why not:** Repeating exception-handling logic across controllers creates duplication and inconsistent responses. Exposing exception details can also reveal sensitive implementation information.

### 5. Authentication & Identity

#### 1. Identity and Token Service Abstractions

**Decision:** Define `IIdentityService` and `ITokenService` in the Application layer, with their implementations in Infrastructure.

**Rationale:** `IIdentityService` abstracts ASP.NET Core Identity operations, while `ITokenService` abstracts access-token generation, refresh-token generation, and hashing. This keeps framework-specific and cryptographic implementation details inside Infrastructure.

#### 2. Authentication Use Case Orchestration

**Decision:** Keep authentication workflow orchestration inside the Application handlers.

**Rationale:** `LoginCommandHandler` coordinates credential validation, account activation checks, token generation, and refresh-token persistence. This separates the use-case workflow from the underlying identity and token implementations.

#### 3. Refresh Token Transport

**Decision:** Return the refresh token in the JSON response body alongside the access token.

**Rationale:** The requirements do not mandate a specific transport mechanism. Returning both tokens in the response simplifies API integration and testing. Clients are responsible for storing tokens securely.

#### 4. Refresh Token Security and Rotation

**Decision:** Store only the SHA-256 hash of refresh tokens in the database and rotate the token after every successful refresh.

**Rationale:** Hashing prevents database exposure from directly revealing usable refresh tokens. Rotation revokes the previous token and issues a replacement, while recording revocation and replacement information to help track token reuse.

#### 5. Admin-Managed Staff Creation

**Decision:** Generate temporary passwords on the server when an Admin creates a staff account through `POST /admin/staff`. Define `ITemporaryPasswordGenerator` in Application and implement it in Infrastructure.

**Rationale:** The interface separates password generation from the use-case handler, allowing `CreateStaffCommandHandler` to depend on an abstraction rather than the generation implementation. The Infrastructure implementation uses cryptographically secure randomness and ensures the generated password satisfies the configured ASP.NET Core Identity password policy. Only the `Admin` and `CampaignManager` roles are accepted. The temporary password is returned to the Admin in the creation response and is neither stored in plaintext nor logged.

### 6. Data Access Strategy: Generic Repository, Unit of Work, and Direct Queries

**Decision:** Use the Generic Repository and Unit of Work for write operations, while allowing read queries to access the database through `IAppDbContext`.

The responsibilities are separated as follows:

| Operation | Approach |
|-----------|----------|
| Add an entity | Generic Repository |
| Retrieve an entity for modification | Generic Repository |
| Check whether an entity exists | Generic Repository |
| Remove an entity | Generic Repository |
| Commit pending changes | Unit of Work |
| Retrieve lists and display data | `IAppDbContext` |
| Filter, sort, paginate, and project results | `IAppDbContext` |

Command handlers use repositories to add or modify entities and call `IUnitOfWork.SaveChangesAsync()` to persist pending changes.

The Unit of Work and repositories use the same scoped `AppDbContext` instance, allowing multiple tracked changes to be committed together through a single `SaveChangesAsync()` call.

Query handlers use `IAppDbContext` directly to:

- Apply `AsNoTracking()` for read-only queries.
- Filter, sort, and paginate data.
- Project results directly into DTOs using `Select()`.
- Retrieve only the data required by the response.

`IAppDbContext` is defined in the Application layer and implemented by the Infrastructure layer, so Application does not depend on the concrete EF Core `DbContext`.

**Why:**

- Keeps write operations organized around entity persistence and a shared Unit of Work.
- Avoids adding generic repository methods for every possible read scenario.
- Gives queries the flexibility needed for filtering, sorting, pagination, and DTO projection.
- Avoids unnecessary entity tracking for read-only operations.
- Preserves the dependency direction between Application and Infrastructure.

**Alternative considered:** Using the Generic Repository for all reads and writes, including listing, filtering, sorting, and pagination.

**Why not:** Complex read queries often require specific projections and query composition. Forcing every query through generic repository methods can introduce unnecessary abstractions and reduce query flexibility.

### 7. Soft Delete with EF Core Interceptor and Global Query Filters

**Decision:** Implement soft deletion through a shared soft-delete abstraction, an EF Core `SaveChanges` interceptor, and global query filters.

**Why:**

- Preserves records instead of permanently deleting them.
- Applies soft-delete behavior consistently across entities that support it.
- Hides soft-deleted records from normal queries through global query filters.
- Reduces the need to implement the same deletion logic in multiple handlers.

**Alternative considered:** Hard-deleting records directly from the database.

**Why not:** Permanent deletion can remove information that may still be required for auditing, historical records, or business operations.

### 8. Options Pattern and Startup Validation

**Decision:** Use the .NET Options Pattern to bind and validate application configuration.

Options classes are registered using configuration binding and validation, with `ValidateOnStart()` where startup validation is required.

For example, `DonationOptions` defines the default donation limits, pending timeout, and currency.

**Why:**

- Keeps configuration strongly typed.
- Centralizes configuration validation.
- Detects invalid settings during application startup.
- Avoids directly accessing `IConfiguration` throughout application services.

**Alternative considered:** Reading configuration values directly through `IConfiguration` wherever they are needed.

**Why not:** Direct configuration access spreads configuration dependencies across the application and makes validation and maintenance less consistent.

<a id="sprint-1--sprint-2-scope"></a>
## 🎯 Sprint 1 & Sprint 2 Scope

The following summarizes the implemented work covered by this documentation.

### Sprint 1

- Established the four-project Clean Architecture solution.
- Created the core Domain entities, enums, value objects, and domain rules.
- Configured EF Core persistence using SQL Server.
- Added entity configurations, relationships, indexes, and the initial migration.
- Implemented Generic Repository and Unit of Work abstractions and Infrastructure implementations.
- Implemented soft-delete handling using an EF Core interceptor and global query filters.
- Added strongly typed configuration options and validation.
- Integrated ASP.NET Core Identity and JWT authentication.
- Implemented authentication operations, including registration, login, refresh-token rotation, logout, and change password.
- Added initial role and Admin seeding.
- Implemented the Categories feature.
- Configured OpenAPI and Scalar API documentation with Bearer authentication.

### Sprint 2

- Applied CQRS with MediatR to the implemented application features.
- Added command and query handlers, validators, and DTOs.
- Implemented the Result Pattern and centralized error catalogue.
- Added centralized HTTP error mapping from application errors to ProblemDetails responses.
- Implemented automatic request validation through a MediatR pipeline behavior.
- Added cancellation-token propagation through application and persistence operations.
- Introduced `TimeProvider` for time-dependent application operations.
- Implemented centralized exception handling using `IExceptionHandler`.
- Added sanitized RFC 9457 ProblemDetails responses with error codes and trace IDs.
- Added API Versioning using URL segments and version-specific OpenAPI documentation.
- Added development test-user and sample-category seeding.
- Added staff creation functionality for administrative user management.

> [!NOTE]
> **Scope note:** This README documents the project scope covered by Sprint 1 and Sprint 2. Features planned for later sprints are not presented as implemented functionality.
