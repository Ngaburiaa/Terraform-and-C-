# BookRepositoryApi

Secure ASP.NET Core Web API for managing books with CRUD, JWT authentication, and role-based access control.

## Stack

- .NET 10 (`net10.0`)
- EF Core + Npgsql (PostgreSQL)
- JWT auth

## Folder structure

- Controllers
- Services
- Routes
- Models
- Security

## Roles

- Admin: full CRUD on books, can read users
- User: read-only on books

## Demo credentials

- admin / Admin@123
- reader / Reader@123

## Database

- Connection string in `appsettings.json` (`DefaultConnection`).
- IDs for both `Books` and `Users` are database-generated identity columns.
- Migrations are applied on startup via `db.Database.Migrate()`.
- Important: the initial migration drops `Books` and `Users` if they already exist, then recreates them.

## Run

1. Install .NET 10 SDK.
2. Update `appsettings.json` -> `Jwt:Key` to a strong secret (32+ chars).
3. Run:

```bash
dotnet restore
dotnet run
```

## Endpoints

Auth
- POST `/api/auth/login`
- POST `/api/auth/register` (always creates `User` role)

Books
- GET `/api/books` (Admin, User)
- GET `/api/books/{id}` (Admin, User)
- POST `/api/books` (Admin)
- PUT `/api/books/{id}` (Admin)
- DELETE `/api/books/{id}` (Admin)

Users
- GET `/api/users` (Admin)
- GET `/api/users/{id}` (Admin)
