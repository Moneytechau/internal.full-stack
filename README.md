# Incident Reporting

A three-step incident reporting wizard: an Angular frontend talking to an ASP.NET Core Web API backed by SQLite. A reporter's details, incident details, and specifics are saved progressively as they move through the wizard, so a report already exists in the database from step 1 onward.

## Projects

| Project | Type | Purpose |
|---|---|---|
| `backend/` | ASP.NET Core Web API (.NET 8) | Exposes the incident report endpoints (`IncidentReportsController`), backed by EF Core + SQLite (`AppDbContext`). |
| `backend.UnitTests/` | xUnit | Unit tests for `IncidentReportService`, with `IIncidentReportRepository` mocked via Moq. No external dependencies. |
| `backend.IntegrationTests/` | xUnit | Exercises the real, already-running API over HTTP and reads the SQLite database file directly to verify persistence. |
| `frontend/` | Angular | The reporting wizard UI (`IncidentReportComponent`). |
| `functionalTests/` | xUnit + Playwright | Drives a real browser against the already-running frontend for an end-to-end reporting flow. |

`internal.full-stack.slnx` ties together the four .NET projects (`backend`, `backend.UnitTests`, `backend.IntegrationTests`, `functionalTests`) — open it in an IDE, or run `dotnet build internal.full-stack.slnx` / `dotnet test internal.full-stack.slnx` to build or test all of them at once. `frontend/` isn't part of it, since it's an Angular project, not .NET.

## Prerequisites

- .NET 8 SDK
- Node.js + npm
- Playwright's Chromium binaries for `functionalTests` (one-time setup, after the first build):
  ```
  pwsh functionalTests/bin/Debug/net8.0/playwright.ps1 install chromium
  ```

## Running the app

**Backend** (from `backend/`):
```
dotnet run
```
The frontend's dev config (`frontend/src/environments/environment.development.ts`) expects the API at `http://localhost:5227`.

**Frontend** (from `frontend/`):
```
npm install
ng serve
```
Open `http://localhost:4200`.

## Testing

### backend.UnitTests

No setup required — pure unit tests with the repository mocked.
```
dotnet test backend.UnitTests
```

### backend.IntegrationTests

Start the backend first (`dotnet run` in `backend/`), then:
```
dotnet test backend.IntegrationTests
```
Tests call the real API over HTTP and also open the SQLite file directly (via `Microsoft.Data.Sqlite`) to verify what was persisted. If the backend isn't running on the defaults, override:
- `INTEGRATION_TESTS_API_BASE_URL` (default `http://localhost:5227`)
- `INTEGRATION_TESTS_DB_PATH` (default `backend/incidents.db`)

### frontend

```
cd frontend
npx ng test --watch=false --browsers=ChromeHeadless
```
(Omit the flags to run interactively in a watched browser via `ng test`.)

### functionalTests

Start both the backend and the frontend first (see above), then:
```
dotnet test functionalTests
```
This runs a single end-to-end test that completes the incident report wizard in a real (headless by default) Chromium browser and asserts the confirmation screen appears. Override if needed:
- `FUNCTIONAL_TESTS_FRONTEND_BASE_URL` (default `http://localhost:4200`)
- `FUNCTIONAL_TESTS_HEADLESS=false` to watch it run
