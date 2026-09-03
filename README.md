# Internal Full Stack

Shell application: a single ASP.NET Core 8 Web API + an Angular standalone-component frontend.

## Structure

```
backend/
  Controllers/       GreetingController (GET /api/greeting)
  Models/             Greeting response DTO
  Program.cs           app + middleware setup
frontend/
  src/app/
    pages/home/          single routed page
    services/            GreetingService (HttpClient wrapper)
    models/
  src/environments/
```

## Running locally

Backend (http://localhost:5227):
```
cd backend
dotnet run --urls http://localhost:5227
```

Frontend (http://localhost:4200):
```
cd frontend
npm start
```

The home page calls `GET /api/greeting` on the backend and displays the response. CORS is configured via `Cors:Origins` in `backend/appsettings.Development.json`.
