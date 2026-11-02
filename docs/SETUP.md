# Setup

## Backend
1. Restore packages.
2. Apply EF Core migrations.
3. Run `backend/src/StandardInteriors.Api`.

## Frontend
1. Run `npm install` in `frontend/`.
2. Start Angular with `npm start`.

## SQL Server
- Trusted connection is configured for `.\SQLEXPRESS`.
- Database name: `StandardInteriorsDb`.

## Authentication Secrets
Set secrets in the process/user environment, never in source control:
```powershell
$env:STANDARD_INTERIORS_JWT_KEY = 'use-a-random-secret-at-least-32-bytes-long'
$env:STANDARD_INTERIORS_ADMIN_PASSWORD = 'use-a-password-at-least-12-characters'
$env:STANDARD_INTERIORS_ADMIN_USERNAME = 'admin'
```

`STANDARD_INTERIORS_ADMIN_PASSWORD` is needed only when seeding the first Admin account into an empty `Users` table. Passwords are hashed before persistence.

## Migration
```bash
dotnet ef database update --project backend/src/StandardInteriors.Api/StandardInteriors.Api.csproj --startup-project backend/src/StandardInteriors.Api/StandardInteriors.Api.csproj
```

If database creation is not permitted, run the scripts under `database/scripts/` manually, then apply the EF migration.

## Acceptance Artifacts
Generated sample PDFs and XLSX files are stored under:
`backend/tests/Acceptance/test-output/`

## Authentication Verification
- Anonymous business calls return `401`.
- Admin login returns JWT and `/api/auth/me` returns the current user.
- Viewer writes and Staff administration access return `403`.
- Staff with `CanVoidPayments` can void; the void action returns `204` and records audit data.
