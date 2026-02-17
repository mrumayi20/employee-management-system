# Employee Management System

## Tech Stack:

- Frontend: React + TypeScript (Vite)
- Backend: ASP.NET Core Web API (.NET 8)
- Database: MySQL (Docker)
- Reports: PDF (QuestPDF), Excel (ClosedXML)
- Auth: JWT

## Prerequisites

- Docker Desktop
- .NET SDK 8
- Node.js (18+ recommended)

## Run Database

From repo root:

```bash
docker compose up -d
```

## Run Backend

```bash
cd backend
dotnet restore
dotnet run --project EmployeeManagementSystem.Api
```

Swagger:

http://localhost:5148/swagger

## Run Frontend

```bash
cd frontend
npm install
npm run dev
```

Frontend:

http://localhost:5173

## Default Login (Seeded)

Email: admin@ems.com

Password: Admin@123

## Features

- Secure login (JWT)
- Employee CRUD
- Department CRUD
- Attendance tracking
- Salary management
- Reports: PDF/Excel export (employees, departments, attendance, salary)

## Layers

1. Domain: pure business entities + rules (no EF, no web, no MySQL)
2. Application: use-cases (commands/queries), validation, DTOs, interfaces
3. Infrastructure: MySQL/EF Core, repositories, file/PDF/Excel services
4. API: controllers/endpoints, auth wiring, DI, swagger
5. Frontend stays separate and talks to API over HTTP.

Why: keeps business rules independent of frameworks/UI/DB; easier testing; easier growth (bonus analytics won’t become a mess).

1. API depends on Application + Infrastructure
2. Infrastructure depends on Application
3. Application depends on Domain

POST /api/auth/login → returns JWT token
dotnet run --project EmployeeManagementSystem.Api
