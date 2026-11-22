# JEX Assessment — Companies & Vacancies API

A small ASP.NET Core REST API for managing companies and vacancies, created for the JEX .NET developer assessment.

## Requirements implemented

1. **Backoffice:** Create, retrieve, update and delete companies and vacancies.
2. **Public listing:** Retrieve companies with at least one active vacancy, including only their active vacancies.
3. **Public vacancy submission:** Create a vacancy for an existing company with a required title and optional description.

## Technology

- C# / ASP.NET Core Web API — **.NET 11**
- Entity Framework Core **11** with SQLite
- EF Core migrations for database creation
- Controller-based HTTP endpoints, DTOs and DataAnnotations validation
- Built-in ASP.NET Core OpenAPI document generation in Development

## Getting started

### Prerequisites

- .NET **11 SDK**
- Visual Studio with ASP.NET Core support, or the .NET CLI
- For the CLI migration command: `dotnet-ef` tool installed and available

### Using Visual Studio

1. Open `JexAssessment.slnx` in Visual Studio.
2. Restore NuGet packages and build the solution.
3. In **Tools > NuGet Package Manager > Package Manager Console**, set `JexAssessment` as the default project and run:

   ```powershell
   Update-Database
   ```

4. Select the **https** launch profile and run the application.
5. The configured development addresses are `https://localhost:7221` and `http://localhost:5034`. If Visual Studio uses a different port, adjust `@host` in `JexAssessment.http`.
6. Open `JexAssessment.http` in Visual Studio and use **Send Request** to exercise the API.

### Using the .NET CLI

From the directory containing `JexAssessment.csproj`:

```bash
dotnet restore
dotnet build
dotnet ef database update
dotnet run --launch-profile https
```

If `dotnet ef` is not available, install the matching EF Core CLI tool (version 11) or use the Visual Studio Package Manager Console instructions above.

The connection string is configured in `appsettings.json` as `Data Source=jexassessment.db`. The SQLite database file is created when migrations are applied; it is not included in the source submission.

The OpenAPI JSON document is available in the Development environment at `/openapi/v1.json`. No Swagger UI is configured; use `JexAssessment.http` to send requests.

## API endpoints

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/api/companies` | List all companies |
| `GET` | `/api/companies/{id}` | Retrieve one company |
| `POST` | `/api/companies` | Create a company |
| `PUT` | `/api/companies/{id}` | Update a company |
| `DELETE` | `/api/companies/{id}` | Delete a company without vacancies |
| `GET` | `/api/vacancies` | List all vacancies |
| `GET` | `/api/vacancies/{id}` | Retrieve one vacancy |
| `POST` | `/api/vacancies` | Create a backoffice vacancy |
| `PUT` | `/api/vacancies/{id}` | Update a vacancy |
| `DELETE` | `/api/vacancies/{id}` | Delete a vacancy |
| `GET` | `/api/companies/with-active-vacancies` | List companies with active vacancies and include their active vacancies |
| `POST` | `/api/companies/{companyId}/vacancies` | Submit a public vacancy for a company |

### Example: public vacancy submission

```http
POST https://localhost:7221/api/companies/1/vacancies
Content-Type: application/json

{
  "title": "Senior .NET Developer"
}
```

The company must already exist. On success the endpoint returns `201 Created` and the created vacancy, initially with `isActive: false`.

## Design decisions and assumptions

- **Simple architecture:** Controllers call `AppDbContext` directly. For this limited scope, adding generic repositories or services would introduce more complexity than value. DTOs separate API contracts from EF Core entities.
- **Relationship:** A vacancy belongs to one company; a company can have multiple vacancies. The foreign key is enforced by EF Core and SQLite.
- **Deletion:** A company with vacancies cannot be deleted. The API returns `409 Conflict`, and the relationship uses `DeleteBehavior.Restrict`.
- **Public submission:** The company is identified by the URL. Only `title` is required; `description` is optional. Publicly submitted vacancies are **inactive by default**. This is an assumption because the assignment does not specify a publishing workflow.
- **Backoffice submission:** A company ID, title and description are required. The backoffice request can set `isActive`.
- **Active listings:** The public listing excludes companies without active vacancies and excludes inactive vacancies from each returned company.
- **Read queries:** `AsNoTracking()` and projections to DTOs avoid unnecessary entity tracking and exposure of persistence models.
- **Validation:** DataAnnotations provide required-field and length validation; the public creation endpoint also explicitly rejects whitespace-only titles.

## Manual verification

`JexAssessment.http` contains example requests for the three user stories, invalid inputs and deletion scenarios. The numbered walkthrough assumes a **fresh database** and should be executed **in order** because some requests use IDs generated by earlier requests. On an existing database, use the IDs returned by your own create requests instead.

The three user stories and the public creation edge cases were manually verified during development. Automated tests were not added because they were explicitly outside the assessment scope.

## Scope and limitations

- **Authentication and authorization are not implemented**, as specified in the assignment. In particular, the backoffice endpoints must not be exposed to untrusted clients in a real deployment without access controls.
- No automated test suite, pagination, concurrency conflict handling or centralized exception middleware was added.
- The application is intended as an assessment solution, not a production-ready vacancy platform.

## Development process and AI usage

The solution was built incrementally and manually tested after each user story. AI assistance was used for implementation guidance, explanations, HTTP request examples and code review. The scope and technical choices were kept deliberately simple to match the assessment.

**Actual time spent:** 5 hours, including research, coding, testing and documentation.

**Outstanding issues / deviations:** None known
