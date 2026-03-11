# BookRepositoryApi

Secure ASP.NET Core Web API for managing books with CRUD, JWT authentication, and role-based access control.

## Folder structure

- Controllers
- Services
- Routes
- Models
- Security

## Roles

- Admin: full CRUD on books
- User: read-only on books

## Demo credentials

- admin / Admin@123
- reader / Reader@123

## Run

1. Install .NET 8 SDK.
2. Update `appsettings.json` -> `Jwt:Key` to a strong secret (32+ chars).
3. Run:

```bash
dotnet restore
dotnet run
```

## Endpoints

- POST `/api/auth/login`
- GET `/api/books`
- GET `/api/books/{id}`
- POST `/api/books`
- PUT `/api/books/{id}`
- DELETE `/api/books/{id}`
