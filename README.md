# ShiftScheduler — .NET API

ASP.NET Core 8 Web API backend for the ShiftScheduler workforce management system. Provides multi-tenant shift scheduling, worker management, absence tracking, holiday configuration, and rule-based schedule generation.

---

## Tech Stack

| Category | Technology |
|---|---|
| Framework | .NET 8.0 / ASP.NET Core 8 |
| ORM | Entity Framework Core 8.0.11 |
| Database | SQL Server |
| Authentication | ASP.NET Identity + JWT Bearer (HttpOnly cookies) |
| Mapping | AutoMapper 12 |
| Logging | Serilog (console + rolling file) |
| Email | MailKit 4.8 |
| Password Hashing | BCrypt.Net-Next |
| API Docs | Swagger / Swashbuckle 6.5 |
| Testing | xUnit + Fluent Assertions |

---

## Prerequisites

- .NET 8 SDK
- SQL Server (local or remote)
- Visual Studio 2022 or Rider

---

## Getting Started

1. Set the connection string in `ShiftSchedularAPI/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=...;Database=ShiftSchedular;..."
   }
   ```

2. Apply migrations:
   ```bash
   dotnet ef database update --project ShiftSchedularDAL --startup-project ShiftSchedularAPI
   ```

3. Run the API:
   ```bash
   dotnet run --project ShiftSchedularAPI
   ```

The API starts on `http://localhost:5265`. Swagger UI is available at `/swagger` in development.

On first startup, roles (`Admin`, `User`) and a default admin account are seeded automatically.

---

## Solution Structure

The solution follows a clean N-tier architecture split across 7 projects:

```
ShiftSchedularAPI/
├── ShiftSchedularAPI/          # Presentation — Controllers, Middleware, Program.cs
├── ShiftSchedularBLL/          # Business Logic — Service implementations
├── ShiftSchedularDAL/          # Data Access — Repositories, UnitOfWork, DbContext, Migrations
├── ShiftSchedularEntity/       # Domain — Entities, DTOs, ViewModels
├── ShiftSchedularIL/           # Infrastructure — AutoMapper profiles, utility services
├── ShiftSchedularRL/           # Resources — .resx localization files
└── ShiftSchedularUnitTesting/  # Tests — xUnit test project
```

### Dependency Chain

```
ShiftSchedularAPI
    ↓
ShiftSchedularBLL  ←→  ShiftSchedularIL
    ↓
ShiftSchedularDAL
    ↓
ShiftSchedularEntity  ←  ShiftSchedularRL
```

---

## Architecture

### Patterns Used

- **Repository Pattern** — `IGenericRepository<T>` for standard CRUD; specialized repositories for complex queries
- **Unit of Work** — `IUnitOfWork` manages transaction boundaries and aggregates all repositories
- **Service Layer** — All business logic lives in `BLL` services; controllers only delegate
- **DTO Pattern** — `Incoming/` DTOs for API responses, `Outgoing/` DTOs for API requests, `ViewModels` as composite payloads
- **BaseResponse\<T\>** — Consistent response wrapper (`Success`, `Message`, `Result`) across all service methods

### Request Flow

```
Controller
  → IXxxService (BLL)
    → IUnitOfWork / IXxxRepository (DAL)
      → DataContext (EF Core)
        → SQL Server
```

---

## Controllers & Endpoints

| Controller | Responsibility |
|---|---|
| `AuthController` | Login, logout, refresh token, forgot/reset password, resend confirmation |
| `UserController` | Register, confirm email, update profile |
| `HomeController` | Dashboard view model, send email |
| `EntityController` | Organization CRUD, member management, skills, public profiles |
| `EntityScheduleController` | Schedule generation, entry assignment, view models |
| `ShiftController` | Shift templates, breaks, rotations |
| `EntityRuleController` | Entity-specific scheduling rules |
| `EntityWorkerAbsenceController` | Absence logging, approval, pagination |
| `HolidayController` | Holiday types, behaviours, catalog, entity holidays |
| `SearchController` | Worker and entity search |
| `SkillController` | Skill CRUD |
| `AbsenceTypeController` | Absence type CRUD |
| `BaseEntityRuleController` | Base rule definitions |
| `LocalizationController` | Localized resource strings |
| *(+ 6 reference data controllers)* | Genders, entity types, shift break types, etc. |

---

## Authentication

- JWT tokens are issued on login and stored in **HttpOnly, Secure cookies**
- `access_token` cookie is extracted in the `OnMessageReceived` JWT event
- Access token expiry and refresh token expiry are configurable in `appsettings.json`
- Refresh tokens are rotated on each use
- `[Authorize]` is applied to all entity-scoped endpoints
- Admin-only endpoints use `[Authorize(Roles = "Admin")]`

---

## Database

EF Core migrations are in `ShiftSchedularDAL/Migrations/`.

### Adding a Migration

```bash
dotnet ef migrations add <MigrationName> --project ShiftSchedularDAL --startup-project ShiftSchedularAPI
```

### Key Entities

| Entity | Purpose |
|---|---|
| `ApplicationUser` | ASP.NET Identity user (worker account) |
| `Entity` | Organization / team |
| `EntityWorker` | Worker membership in an organization |
| `Shift` | Shift template with breaks |
| `ScheduleEntry` | A scheduled shift instance |
| `EntityWorkerAbsence` | Worker absence record |
| `EntityHoliday` | Holiday configured per organization |
| `HolidayCatalog` | Shared holiday definitions |
| `EntityRule` | Scheduling constraint rule |
| `UserBot` | Automated/bot worker |

24 database indexes are defined across key foreign keys and date range columns for query performance.

---

## Middleware Pipeline

Middleware is applied in this order:

1. `GlobalExceptionHandlerMiddleware` — Catches all unhandled exceptions; returns consistent JSON with a `correlationId`; hides stack traces in production
2. Swagger — Development only
3. CORS
4. `LanguageMiddleware` — Parses `Accept-Language` header; stores language code in `HttpContext.Items`
5. HTTPS Redirection
6. Role & admin seeding
7. Authentication (JWT)
8. Authorization
9. Rate Limiter
10. Controllers

---

## Rate Limiting

| Policy | Limit | Applied To |
|---|---|---|
| `auth` | 5 req/min per IP | Login, registration |
| `forgot-password` | 3 req/15 min per IP | Password reset request |
| `refresh` | 30 req/hour per user | Token refresh |
| `general` | 100 req/min (auth) / 20 (anon) | Standard endpoints |
| `write` | 30 req/min per user | Create/update mutations |
| `heavy` | 10 req/min per user | Schedule generation |
| `public` | 200 req/min per IP | Public profile endpoints |

All rate-limited endpoints return `429 Too Many Requests` with a `Retry-After` header on violation.

---

## Localization

The `ShiftSchedularRL` project contains `.resx` resource files for all user-facing messages, organized by domain:

| Resource File | Domain |
|---|---|
| `AbsenceRelatedMessages` | Absence management |
| `HolidayRelatedMessages` | Holiday management |
| `MemberManagementMessages` | Worker/member operations |
| `ScheduleManagementMessages` | Schedule generation |
| `ShiftManagementMessages` | Shift operations |
| `EntityRulesMessages` | Rule management |
| `DashboardMessages` | Dashboard data |
| `SearchMessages` | Search operations |
| `HomeMessages` | Home/landing |
| `LoggingMessages` | Internal logging |

The `LanguageMiddleware` extracts the `Accept-Language` header and makes it available via `ILanguageAccessor` throughout the request pipeline.

---

## Logging

Serilog is configured with two sinks:

- **Console** — `[HH:mm:ss Level] Message`
- **File** — Daily rolling logs in `Logs/`, retained for 30 days

Log level defaults to `Information`, with `Warning` for `Microsoft.*` and `System.*` namespaces.
