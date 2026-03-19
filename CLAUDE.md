# ShiftSchedular API - Claude AI Assistant Guide

This document provides context and guidelines for AI assistants working on the ShiftSchedular API project.

## Project Overview

ShiftSchedular is a .NET 8.0 ASP.NET Core Web API for managing shift scheduling, worker assignments, and entity management. The system supports:
- Multi-tenant entity management
- Worker and bot scheduling
- Shift rotations and absences
- Skills-based assignment
- JWT authentication with refresh tokens

## Architecture

### Project Structure (N-Tier Architecture)

```
ShiftSchedularAPI/           # Web API Layer (Controllers, Middleware)
ShiftSchedularBLL/           # Business Logic Layer (Services, Interfaces)
ShiftSchedularDAL/           # Data Access Layer (Repositories, EF Core)
ShiftSchedularEntity/        # Entity Models, DTOs, ViewModels
ShiftSchedularIL/            # Infrastructure Layer
ShiftSchedularRL/            # Repository Layer
```

### Key Design Patterns

1. **Repository Pattern**: All data access through repositories (e.g., `EntityWorkerRepository`)
2. **Unit of Work**: Transaction management via `IUnitOfWork`
3. **Dependency Injection**: Services registered in `Program.cs`
4. **DTO Pattern**: Separate DTOs for incoming/outgoing data

## Database

### Technology
- **Database**: SQL Server
- **ORM**: Entity Framework Core 10.0.2
- **Migrations**: Code-first approach

### Key Entities

- `Entity`: Organizations/teams
- `ApplicationUser`: Workers/users (ASP.NET Identity)
- `EntityWorker`: Many-to-many relationship between entities and workers
- `Shift`: Shift definitions
- `ScheduleEntry`: Scheduled shift instances
- `EntityWorkerAbsence`: Worker absences with approval workflow
- `UserBot`: Automated workers for scheduling

### Critical Relationships

```
Entity 1---* EntityWorker *---1 ApplicationUser
Entity 1---* Shift 1---* ScheduleEntry
ScheduleEntry 1---* ScheduleEntryWorkers *---1 ApplicationUser
EntityWorker 1---* EntityWorkerSkill *---1 Skill
```

### Performance Indexes

The following indexes exist for query optimization (added 2026-01-15):

**EntityWorkers**:
- `IX_EntityWorkers_EntityId`
- `IX_EntityWorkers_ApplicationUserId`
- `IX_EntityWorkers_EntityId_ApplicationUserId` (composite)

**EntityWorkerSkills**:
- `IX_EntityWorkerSkills_EntityId`
- `IX_EntityWorkerSkills_ApplicationUserId`

**ScheduleEntry**:
- `IX_ScheduleEntries_ShiftId`
- `IX_ScheduleEntries_DateRange` (ScheduleStartDate + ScheduleEndDate)
- `IX_ScheduleEntries_ShiftId_DateRange` (composite)

**EntityWorkerAbsences**:
- `IX_EntityWorkerAbsences_EntityId`
- `IX_EntityWorkerAbsences_ApplicationUserId`
- `IX_EntityWorkerAbsences_DateRange` (AbsenceStartDate + AbsenceEndDate)

**EntityWorkerInvitations**:
- `IX_EntityWorkerInvitations_EntityId`
- `IX_EntityWorkerInvitations_Email`

**EntityUserBots/EntityUserBotSkills**:
- `IX_EntityUserBots_EntityId`
- `IX_EntityUserBotSkills_EntityId`

**EntityShiftRotations**:
- `IX_EntityShiftRotations_EntityId`
- `IX_EntityShiftRotations_ShiftId`

**ScheduleEntryWorkers**:
- `IX_ScheduleEntryWorkers_ScheduleEntryId`

**Entity** (hierarchy):
- `IX_Entities_ParentEntityId`

**EntityPermissions**:
- `IX_EntityPermissions_ApplicationUserId`
- `IX_EntityPermissions_EntityId_RoleId` (composite)

## Security

### Authentication & Authorization

- **JWT Tokens**: Access tokens with configurable expiration
- **Refresh Tokens**: Secure token refresh mechanism
- **Token Validation**: HmacSha256 algorithm validation (CRITICAL)

### Security Best Practices

1. **SQL Injection Prevention**:
   - ALWAYS use Entity Framework LINQ queries with parameterization
   - NEVER concatenate strings for SQL queries
   - Use `.Contains()` for list filtering instead of string joining

2. **Token Validation**:
   - Verify algorithm is HmacSha256 using NOT operator: `!jwtSecurityToken.Header.Alg.Equals()`
   - Never disable algorithm validation

3. **Null Safety**:
   - Always check for null users before processing authentication
   - Use early returns for null cases

4. **DateTime Handling**:
   - ALWAYS use `DateTime.UtcNow` instead of `DateTime.Now`
   - Store UTC times in database, convert to user timezone in presentation layer

### Known Security Fixes (2026-01-15)

Fixed critical vulnerabilities:
- ✅ Token validation bypass (semicolon bug) in `TokenService.cs`
- ✅ Null reference in login flow in `AuthService.cs`
- ✅ SQL injection vulnerability in `EntityWorkerRepository.GetDistinctMembersByEntityId()`

## Code Conventions

### LINQ vs Raw SQL

**Prefer LINQ queries**:
```csharp
// GOOD - Parameterized, safe from SQL injection
var query = from ew in _context.EntityWorkers
           join w in _context.ApplicationUsers on ew.ApplicationUserId equals w.Id
           where ew.EntityId == entityId && workers.Contains(w.Id)
           select new EntityWorkerMemberModel { ... };
var result = await query.ToListAsync();
```

**Avoid raw SQL with string concatenation**:
```csharp
// BAD - SQL injection vulnerable
string listInString = string.Join(",", workers.Select(v => $"'{v}'"));
string query = string.Format(SQL_TEMPLATE, listInString);
var result = await _sqlRawRepository.ExecuteQuery<T>(query, parameters);
```

### DateTime Usage

```csharp
// GOOD
user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(minutes);

// BAD
user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(minutes);
```

### Null Checking

```csharp
// GOOD - Early return pattern
ApplicationUser user = await _userManager.FindByEmailAsync(loginDTO.Email);
if (user == null)
{
    loginResponseDTO.Message = WorkerRelatedMessages.WorkerLoginEmailNotFoundError;
    return loginResponseDTO;
}
// Continue with user operations...

// BAD - Missing return allows null reference
ApplicationUser user = await _userManager.FindByEmailAsync(loginDTO.Email);
if (user == null)
{
    loginResponseDTO.Message = WorkerRelatedMessages.WorkerLoginEmailNotFoundError;
}
bool validLogin = await _userManager.CheckPasswordAsync(user, loginDTO.Password); // NullReferenceException!
```

### Async/Await

- All repository methods should be async
- Use `ToListAsync()`, `FirstOrDefaultAsync()`, etc.
- Always await async calls

## Common Patterns

### Service Layer Pattern

```csharp
public async Task<BaseResponse<T>> MethodName(DTO inputDTO)
{
    BaseResponse<T> response = new BaseResponse<T>();
    try
    {
        // 1. Validate input
        if (inputDTO == null)
        {
            response.Message = "Validation error message";
            return response;
        }

        // 2. Call repository/data access
        var result = await _repository.MethodName(inputDTO.Id);

        // 3. Map to DTO if needed
        response.Result = _mapper.Map<T>(result);
        response.Success = true;
        return response;
    }
    catch (Exception ex)
    {
        response.Message = ex.Message;
        return response;
    }
}
```

### Repository Pattern

```csharp
public async Task<IEnumerable<T>> GetByEntityId(Guid entityId)
{
    if (entityId == Guid.Empty)
        return null;

    return await _dbSet
        .Where(i => i.EntityId.Equals(entityId))
        .ToListAsync();
}
```

## Entity Framework Migrations

### Creating Migrations

```bash
# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef

# Create migration
dotnet ef migrations add MigrationName --project ShiftSchedularDAL --startup-project ShiftSchedularAPI

# Apply migration
dotnet ef database update --project ShiftSchedularDAL --startup-project ShiftSchedularAPI

# Remove last migration (if not applied)
dotnet ef migrations remove --project ShiftSchedularDAL --startup-project ShiftSchedularAPI
```

### Index Configuration

Indexes are configured in `DataContext.OnModelCreating()`:

```csharp
modelBuilder.Entity<EntityWorker>()
    .HasIndex(ew => ew.EntityId)
    .HasDatabaseName("IX_EntityWorkers_EntityId");

modelBuilder.Entity<EntityWorker>()
    .HasIndex(ew => new { ew.EntityId, ew.ApplicationUserId })
    .HasDatabaseName("IX_EntityWorkers_EntityId_ApplicationUserId");
```

## API Structure

### Middleware Pipeline Order (CRITICAL)

The middleware order in `Program.cs` is critical for proper CORS and authentication:

```csharp
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);  // MUST come after Swagger
app.UseHttpsRedirection();
app.UseAuthentication();              // Before Authorization
app.UseAuthorization();
app.UseRateLimiter();                 // After Authorization
app.MapControllers();
```

### CORS Configuration

CORS origins are configured in `appsettings.{Environment}.json`:

```json
{
  "AllowedOrigins": [
    "https://yourfrontend.azurestaticapps.net"
  ]
}
```

For Azure deployment, set Application Settings:
- `ASPNETCORE_ENVIRONMENT` = `Staging` (or appropriate environment)
- `AllowedOrigins__0` = `https://yourfrontend.azurestaticapps.net`
- `AllowedOrigins__1` = `https://anotherdomain.com` (if needed)

## Azure Deployment

### Environment Configuration

Azure App Service uses Application Settings to override `appsettings.json`:
- Array values use double underscore: `AllowedOrigins__0`, `AllowedOrigins__1`
- Set `ASPNETCORE_ENVIRONMENT` to load correct appsettings file
- Sensitive values (connection strings, JWT secrets) should be in Azure Application Settings, NOT committed to git

### Deployment Branches

- `main`: Production environment
- `stage`: Staging environment (Azure staging slot)
- `dev`: Development environment (local/dev deployments)

## Testing

### Unit Tests Location

Unit tests are in the test projects. Note: Some test classes are currently empty stubs (identified as technical debt).

### Critical Areas to Test

1. **Authentication flows**: Login, token refresh, token validation
2. **Authorization**: Entity ownership, member permissions
3. **Schedule operations**: Conflict detection, worker availability
4. **Absence approval**: Workflow state management

## Common Issues & Solutions

### Issue: CORS Errors in Azure

**Symptoms**: "No 'Access-Control-Allow-Origin' header is present"

**Solution**:
1. Verify middleware order (CORS before Authentication)
2. Set `ASPNETCORE_ENVIRONMENT` in Azure Application Settings
3. Add allowed origins to Application Settings using `AllowedOrigins__0` notation

### Issue: Token Validation Failing

**Symptoms**: Tokens are accepted when they shouldn't be

**Check**:
1. Algorithm validation uses NOT operator: `|| !jwtSecurityToken.Header.Alg.Equals(...)`
2. No semicolon after the if condition
3. Exception is thrown for invalid tokens

### Issue: Null Reference in Authentication

**Symptoms**: NullReferenceException when logging in

**Check**:
1. Early return when user is null
2. All user operations happen after null check

## Performance Considerations

### Query Optimization

1. **Use Indexes**: All foreign keys and commonly filtered columns should be indexed
2. **Avoid N+1 Queries**: Use `.Include()` for related entities
3. **Project Only Needed Columns**: Use `.Select()` to limit data transfer
4. **Pagination**: Use `Skip()` and `Take()` for large result sets

### Identified N+1 Query Issues

Watch for these patterns in existing code:
- Loading EntityWorkers without including Skills
- Loading ScheduleEntries without including Shift
- Repeated queries in loops

## Technical Debt

### Known Issues (Prioritized)

1. ~~**CRITICAL**: Token validation bypass~~ ✅ Fixed 2026-01-15
2. ~~**CRITICAL**: Null reference in login~~ ✅ Fixed 2026-01-15
3. ~~**HIGH**: SQL injection vulnerability~~ ✅ Fixed 2026-01-15
4. **HIGH**: Exposed secrets in appsettings.json (move to Azure Key Vault)
5. **MEDIUM**: Empty unit test classes need implementation
6. **MEDIUM**: Inconsistent DateTime handling (some DateTime.Now still exist)

### Dead Code

Recently removed (2026-01-15):
- ✅ `EntityService.GetEntityMembersByList()` - Superseded by `GetEntityMembers()`

## Future Improvements

1. **Caching**: Implement Redis caching for frequently accessed entities
2. **Logging**: Add structured logging with Serilog
3. **Health Checks**: Expand health endpoints for monitoring
4. **API Versioning**: Implement versioning strategy for breaking changes
5. **Background Jobs**: Use Hangfire for scheduled tasks (e.g., rotation generation)

## Git Workflow

### Branch Strategy

- Create feature branches from `dev`
- Naming convention: `feature/description`, `fix/description`, `refactor/description`
- Merge to `dev` → `stage` → `main`
- Use `--no-ff` for merge commits to preserve branch history

### Commit Message Format

```
Brief summary (50 chars or less)

Detailed explanation of what changed and why.
Include any breaking changes or migration requirements.

- Bullet points for specific changes
- Reference issue numbers if applicable

Co-Authored-By: Claude Sonnet 4.5 <noreply@anthropic.com>
```

## Useful Commands

```bash
# Build solution
dotnet build

# Run API locally
dotnet run --project ShiftSchedularAPI

# Run tests
dotnet test

# Clean solution
dotnet clean

# Restore packages
dotnet restore

# Create migration
dotnet ef migrations add MigrationName --project ShiftSchedularDAL --startup-project ShiftSchedularAPI

# Update database
dotnet ef database update --project ShiftSchedularDAL --startup-project ShiftSchedularAPI

# Check migration status
dotnet ef migrations list --project ShiftSchedularDAL --startup-project ShiftSchedularAPI
```

## Contact & Resources

- **Repository**: https://github.com/manueljscruz/ShiftSchedularAPI
- **Frontend**: Angular (deployed to Azure Static Web Apps)
- **API**: ASP.NET Core 8.0 (deployed to Azure App Service)

## Change Log

### 2026-01-15
- Fixed critical authentication vulnerabilities (token validation, null reference)
- Added 24 database indexes for performance optimization
- Fixed SQL injection vulnerability in EntityWorkerRepository
- Removed dead code (GetEntityMembersByList)
- Created EF Core migration: AddPerformanceIndexes
- Standardized DateTime usage to UTC

---

*This document should be updated as the project evolves. When making significant architectural changes, update this guide.*
