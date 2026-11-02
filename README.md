# Standard Interiors

Internal business web application for Standard Interiors.

## Structure
- `frontend/` Angular + Tailwind UI
- `backend/` ASP.NET Core Web API + EF Core
- `database/` SQL scripts and notes
- `docs/` setup, schema, API and progress docs

## Backend
```bash
dotnet restore backend/src/StandardInteriors.Api/StandardInteriors.Api.csproj
dotnet build backend/src/StandardInteriors.Api/StandardInteriors.Api.csproj
dotnet ef database update --project backend/src/StandardInteriors.Api/StandardInteriors.Api.csproj --startup-project backend/src/StandardInteriors.Api/StandardInteriors.Api.csproj
dotnet run --project backend/src/StandardInteriors.Api/StandardInteriors.Api.csproj
```

Backend URL:
- `http://localhost:5087`

## Frontend
```bash
cd frontend
npm install
npm run build
npm start
```

Frontend URL:
- `http://localhost:4200`

## Database
- Database name: `StandardInteriorsDb`
- Connection string: `backend/src/StandardInteriors.Api/appsettings.json`

## Authentication
The internal application uses JWT bearer authentication, secure ASP.NET Core password hashing, and Admin/Staff/Viewer authorization. Set `STANDARD_INTERIORS_JWT_KEY` and, for first-user seeding, `STANDARD_INTERIORS_ADMIN_PASSWORD` in the environment before starting the API. See `docs/SETUP.md`.

Manual script fallback:
- `database/scripts/001_CreateDatabase.sql`
- `database/scripts/002_CreateSchema.sql`
- `database/scripts/003_SeedMasterData.sql`

## Exports
The API generates real XLSX workbooks for customers, customer summaries/statements, estimates, payments, outstanding, fully-paid, and monthly collections. See `docs/API_DOCUMENTATION.md` for routes and filters.

## Verification
- Backend build: passed
- Backend tests: 7 passed
- Angular production build: passed
- SQL Server database and migration: verified
- Live dashboard/customers/services/estimates/payments/reports: verified
- Live acceptance result: Estimate `₹270,000`, Paid `₹80,000`, Balance `₹190,000`
- XLSX workbooks: generated and ZIP/workbook structure validated
- Sample PDFs: generated and visually inspected under `backend/tests/Acceptance/test-output/`

Authentication verification:
- Anonymous business API: `401`
- Admin login/current user: passed
- Viewer read-only enforcement: passed
- Staff settings/void restrictions: passed
- Staff explicit payment-void grant: passed
- Audit attribution: verified directly in SQL Server

The acceptance scripts are under `backend/tests/Acceptance/`.
