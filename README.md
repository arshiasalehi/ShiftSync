# ShiftSync

ShiftSync is a full-stack restaurant employee scheduling MVP with:

- `ShiftSync.Api` (ASP.NET Core Web API + EF Core + SQL Server + JWT)
- `ShiftSync.Web` (Blazor Web App)
- `ShiftSync.Mobile` (.NET MAUI)
- `ShiftSync.Shared` (shared DTO contracts)

## Architecture

### Backend

`ShiftSync.Api` uses a clean layered structure:

- `Models` - EF entities
- `Data` - `ShiftSyncDbContext` + `DbSeeder`
- `Repositories` - data access abstractions
- `Services` - validation + business rules
- `Controllers` - API endpoints
- `Infrastructure` - JWT, password hashing, error middleware, claim helpers

Business rules are enforced in services:

- no overlapping availability
- shift minimum 4 hours
- shift inside employee availability
- one shift per employee per day
- weekly hours cannot exceed employee max
- shift role must be assigned to employee
- payroll only counts published schedules

### Clients

- `ShiftSync.Web` calls API through `Services/ApiClient` and keeps auth state in `SessionState`
- `ShiftSync.Mobile` uses MVVM (`ViewModels`) + `Services/ApiClient`

## Seeded Data

When API starts, it seeds:

- Business: `Sunset Diner` (`CompanyCode: SUN123`)
- Admin: `admin@sunset.local` / `Admin123!`
- Employee: `alice@sunset.local` / `Employee123!`
- Employee: `ben@sunset.local` / `Employee123!`
- Roles: Kitchen, Cash, Drinks
- Sample availability + published schedule + shifts

## Run Prerequisites

- .NET 8 SDK (required by project target frameworks)
- SQL Server (local or Docker)
- MAUI workload for mobile (`dotnet workload install maui`)

### SQL Server via Docker

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong!Passw0rd" \
  -p 1433:1433 --name shiftsync-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

Connection string is configured in `ShiftSync.Api/appsettings.json`.

## Build

```bash
dotnet restore ShiftSync.sln
dotnet build ShiftSync.Api/ShiftSync.Api.csproj
dotnet build ShiftSync.Web/ShiftSync.Web.csproj
dotnet build ShiftSync.Mobile/ShiftSync.Mobile.csproj -f net8.0-maccatalyst
```

## Run API

```bash
dotnet run --project ShiftSync.Api
```

API URLs (from launch settings):

- `http://localhost:5008`
- `https://localhost:7008`

Swagger:

- `http://localhost:5008/swagger`

## Run Blazor Web

```bash
dotnet run --project ShiftSync.Web
```

Web URLs:

- `http://localhost:5010`
- `https://localhost:7010`

Blazor API base URL is configured in:

- `ShiftSync.Web/appsettings.json`

## Run MAUI

```bash
dotnet build ShiftSync.Mobile/ShiftSync.Mobile.csproj -f net8.0-android
# or
 dotnet build ShiftSync.Mobile/ShiftSync.Mobile.csproj -f net8.0-ios
# or
 dotnet build ShiftSync.Mobile/ShiftSync.Mobile.csproj -f net8.0-maccatalyst
```

Mobile API base URL is configured in `ShiftSync.Mobile/MauiProgram.cs`:

- Android emulator: `http://10.0.2.2:5008/`
- iOS/MacCatalyst: `http://localhost:5008/`

## Main API Endpoints

### Auth

- `POST /api/auth/register-admin`
- `POST /api/auth/register-employee`
- `POST /api/auth/login`

### Employee

- `GET /api/me/shifts`
- `GET /api/me/payroll`
- `GET /api/me/availability`
- `POST /api/me/availability`
- `PUT /api/me/availability/{id}`
- `DELETE /api/me/availability/{id}`

### Admin

- `GET /api/employees`
- `PUT /api/employees/{id}/payrate`
- `PUT /api/employees/{id}/roles`
- `GET /api/availability`
- `POST /api/schedules`
- `POST /api/shifts`
- `GET /api/schedules/{weekStart}`

