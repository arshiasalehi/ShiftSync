# 🍽️ ShiftSync Restaurant Workforce Management System

**ShiftSync** is a full-stack restaurant workforce platform for managing staffing, employee availability, schedule publishing, and payroll estimation in one clean workflow.

It includes three clients backed by one API:
- **ShiftSync.Api** (ASP.NET Core Web API)
- **ShiftSync.Web** (Blazor Web App)
- **ShiftSync.Mobile** (.NET MAUI)

Built with **.NET 8**, **ASP.NET Core**, **Blazor**, **.NET MAUI**, **EF Core**, **SQL Server**, and **Tailwind CSS**, ShiftSync is structured for real-world restaurant operations and easy extension.

---

## 🚀 Features

### 👥 Staff & Access Management
- Role-based auth with JWT (**Admin** and **Employee**)
- Admin registration + employee onboarding with company code
- Employee role assignment and pay settings management

### 📅 Availability & Schedule Operations
- Employees manage weekly recurring availability
- Admin views team availability by employee
- Weekly schedule builder with draft/published flow
- Shift creation with business-rule validation:
  - No overlapping availability slots
  - Shift must fit employee availability
  - Role must be assigned to employee
  - Weekly max-hour enforcement

### 💵 Payroll Insights
- Employee payroll estimate from published shifts
- Admin payroll overview by employee
- Total payroll roll-up for selected week
- Payroll calculations only include published schedules

### 🌐 Multi-Client Experience
- Responsive web admin/employee interface (Blazor)
- Mobile employee workflow (.NET MAUI + MVVM)
- Shared DTO contracts across API/Web/Mobile projects

---

# 💻 Tech Stack

## 🖥️ Backend
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-5C2D91?style=for-the-badge&logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF_Core-6DB33F?style=for-the-badge)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)

- **ASP.NET Core Web API**
- **Entity Framework Core** (SQL Server)
- **JWT auth**, password hashing, middleware-based error handling
- Layered services + repositories + DTO contracts

## 🎨 Frontend
![Blazor](https://img.shields.io/badge/Blazor-512BD4?style=for-the-badge&logo=blazor&logoColor=white)
![TailwindCSS](https://img.shields.io/badge/Tailwind_CSS-38B2AC?style=for-the-badge&logo=tailwindcss&logoColor=white)
![.NET MAUI](https://img.shields.io/badge/.NET_MAUI-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

- **Blazor Server** web app for admin + employee UX
- **Tailwind CSS (CDN)** for modern responsive UI
- **.NET MAUI** mobile app with MVVM view models

## 🧰 Dev Tools
![VSCode](https://img.shields.io/badge/VS_Code-007ACC?style=for-the-badge&logo=visualstudiocode&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)

- Local SQL Server via Docker
- Swagger UI for API testing
- Multi-project .NET solution workflow

---

# 🧠 Architecture Overview

## 🎨 Presentation Layer
- `ShiftSync.Web` (Blazor pages + shared layout shell)
- `ShiftSync.Mobile` (.NET MAUI views + MVVM)
- Role-specific flows for admin and employee

## ⚙️ Business Logic Layer
- API services enforce scheduling and payroll rules
- Auth service handles registration/login/token issuing
- Admin and employee service boundaries are explicit

## 🗄️ Data Access Layer
- EF Core `DbContext` + SQL Server
- Repository abstractions for users, availability, schedules, roles
- Seeded startup data for fast local onboarding

---

# 🛠️ Setup Instructions

1. **Prerequisites**
   - .NET 8 SDK
   - SQL Server (local) or Docker
   - Optional for mobile: `dotnet workload install maui`

2. **Start SQL Server (Docker option)**
   ```bash
   docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong!Passw0rd" \
     -p 1433:1433 --name shiftsync-sql -d mcr.microsoft.com/mssql/server:2022-latest
   ```

3. **Configure API settings**
   - Update connection string/JWT settings in:
   - `ShiftSync.Api/appsettings.json`

4. **Restore + build solution**
   ```bash
   dotnet restore ShiftSync.sln
   dotnet build ShiftSync.sln
   ```

5. **Run API**
   ```bash
   dotnet run --project ShiftSync.Api
   ```
   API: `http://localhost:5008`  
   Swagger: `http://localhost:5008/swagger`

6. **Run Web App**
   ```bash
   dotnet run --project ShiftSync.Web
   ```
   Web: `http://localhost:5010`

7. **Run Mobile App (optional)**
   ```bash
   dotnet build ShiftSync.Mobile/ShiftSync.Mobile.csproj -f net8.0-maccatalyst
   ```
   Mobile API base URL is configured in `ShiftSync.Mobile/MauiProgram.cs`.

---

# 🔐 Seeded Local Accounts

- **Admin**: `admin@sunset.local` / `Admin123!`
- **Employee**: `alice@sunset.local` / `Employee123!`
- **Employee**: `ben@sunset.local` / `Employee123!`
- **Company Code**: `SUN123`

---

# 📊 Project Stats

| Metric                 | Value                                  |
|------------------------|----------------------------------------|
| 🧑‍💻 Main Language      | C#                                    |
| 🧱 Backend Framework    | ASP.NET Core + EF Core                |
| 🗃️ Database            | SQL Server                            |
| 🌐 Web Client          | Blazor + Tailwind                     |
| 📱 Mobile Client       | .NET MAUI (MVVM)                      |
| 🏗️ Solution Structure  | API + Web + Mobile + Shared Contracts |

---

# 📚 Top Languages Used

![C#](https://img.shields.io/badge/C%23-75%25-239120?style=for-the-badge&logo=csharp&logoColor=white)
![Razor/XAML](https://img.shields.io/badge/Razor%2FXAML-15%25-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL](https://img.shields.io/badge/SQL-10%25-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)

---

# 👥 Team Members

- [**Arshia Salehi**](https://github.com/arshiasalehi)
