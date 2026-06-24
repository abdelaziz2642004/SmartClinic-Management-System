# SmartClinic Backend — ASP.NET Core API

SmartClinic is a premium, robust clinic management backend system built with **ASP.NET Core 9.0**, **Entity Framework Core**, and **Microsoft SQL Server**. It uses state-of-the-art Object-Oriented design patterns to model a real-world clinic's operations, state lifecycles, and audit logs.

---

## 🚀 Key Features & Design Patterns

The backend incorporates several advanced design patterns to respect the SOLID design principles:

### 1. State Pattern (Dev 3 - Appointment Lifecycle)
Rather than using fragile conditional logic, the booking status transitions are governed by an encapsulation of states (`PendingState`, `ConfirmedState`, `CompletedState`, `CancelledState`).
*   **Workflow**: `Pending` ➡️ `Confirmed` ➡️ `Completed` or `Cancelled`.
*   **Enforcement**: Attempts to transition illegally (e.g. `Pending` ➡️ `Completed` without confirmation) result in custom exceptions, which are intercepted and returned as structured errors.

```
                  ┌──────────────┐
                  │   Pending    │
                  └──────┬───────┘
             Confirm     │     Cancel
           ┌─────────────┴─────────────┐
           ▼                           ▼
     ┌───────────┐               ┌───────────┐
     │ Confirmed │               │ Cancelled │
     └─────┬─────┘               └─────┬─────┘
  Complete │                           │ Undo
           ▼                           │ (Restores State)
     ┌───────────┐                     │
     │ Completed │◄────────────────────┘
     └───────────┘
```

### 2. Decorator Pattern (Dev 6 - Dynamic Tagging)
Staff can dynamically assign modifiers or attributes (like `VIP`, `Urgent`, or `LateFee`) to appointments.
*   **Implementation**: Done via the `AppointmentTags` relational table and repositories, allowing dynamic runtime extensions of the `Appointment` entity without changing its core fields or schema.

### 3. Command Pattern (Dev 6 - Undo Cancellations)
To secure the business against accidental cancellations, every cancellation is encapsulated as a database-recorded `CancellationCommand`.
*   **Rollback**: The `POST /api/appointments/undo-last-cancel` endpoint pops the last non-undone cancellation for the authenticated user, verifies it using the State pattern, and reverts the appointment to its previous state.

---

## 🛠️ Technology Stack
*   **Core API**: ASP.NET Core 9.0 Web API
*   **Database ORM**: Entity Framework Core 9.0
*   **Database Engine**: Microsoft SQL Server (LocalDB / Express / Default Instances)
*   **Security & Identity**: ASP.NET Core Identity Core + JWT Bearer Tokens
*   **API Exploration**: Swagger / OpenAPI UI (`/swagger/index.html`)

---

## 📂 Project Structure

```
SmartClinic/
├── Controllers/         # API Controllers (AppointmentsController, AuthController)
├── Data/                # Database context & configurations (AppDbContext)
├── DTOS/                # Data Transfer Objects (Register, Login, Appointment, Dev6Dtos)
├── Exceptions/          # Custom exceptions (BadRequest, NotFound, Duplicate)
├── Middlewares/         # Request interceptors (GlobalException & RequestLogging)
├── Migrations/          # EF Core migration history files
├── Models/              # Database schema entities (Appointment, User, Doctor, Specialty, etc.)
├── Repositories/        # Data access layers (AppointmentRepository, AppointmentTagRepository, etc.)
├── Services/            # Business services & patterns helper classes (RoleFactory, MedicalRecordProxy)
├── Program.cs           # Main Application entry-point and DI registration
├── appsettings.json     # Configuration file (including JWT keys and database connections)
└── SmartClinicApiDoc.txt# Comprehensive plaintext API endpoint document
```

---

## ⚙️ Setup & Local Deployment

### 1. Database Configuration
Update the database connection string in your [appsettings.json](file:///c:/Users/abdel/Downloads/Clinic/appsettings.json):
```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=localhost;Initial Catalog=Clinic;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"
}
```
*Make sure your SQL Server service is running (e.g. `MSSQLSERVER` or `SQLEXPRESS`).*

### 2. Install Dependencies
Restore NuGet packages by running:
```bash
dotnet restore
```

### 3. Run EF Migrations
Apply the initial migrations to construct the SQL database tables (including `AppointmentTags` and `CancellationCommands` tables):
```bash
dotnet ef database update
```

### 4. Start the Application
Run the API locally on port `5050` or the configuration default:
```bash
dotnet run --urls "http://localhost:5050"
```
Once started, explore the Swagger UI at:
*   [http://localhost:5050/swagger/index.html](http://localhost:5050/swagger/index.html)

---

## 📊 Developer Vertical Slice Progress

| Slice | Focus Area | Status | Design Pattern | Notes |
|:---:|---|:---:|:---:|---|
| **DEV 1** | Architecture | **PARTIAL** | Repository, Singleton | Base skeleton, DbContext, and Global Exceptions Middleware are completed. Generic repository abstraction is missing. |
| **DEV 2** | Identity | **PARTIAL** | Factory, Proxy | JWT login/register is functional. RoleFactory doesn't do profile splitting. MedicalRecordProxy is incomplete/not wired in. |
| **DEV 3** | Appointments | **DONE** | State | Full booking engine CRUD + lifecycle state transitions are fully complete. |
| **DEV 4** | Billing | **NOT DONE** | Strategy | Payment.cs entity exists. No invoicing, pricing strategy, or endpoints exist. |
| **DEV 5** | Notifications | **NOT DONE** | Observer | SMS/Email triggers on state change are completely missing. |
| **DEV 6** | Modifiers | **DONE** | Decorator, Command | Dynamic tags and Undo-Last-Cancel command history are fully implemented and verified. |
| **DEV 7** | Reporting | **NOT DONE** | Adapter | Report.cs exists. Exporting, stored procedures, and Google Calendar sync are missing. |

---

## 📖 API Documentation
For detailed guidance on request/response models, input JSON templates, headers, query filters, and response status codes, check the [SmartClinicApiDoc.txt](file:///c:/Users/abdel/Downloads/Clinic/SmartClinicApiDoc.txt) file in the root directory.