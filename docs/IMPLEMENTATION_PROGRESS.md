# Standard Interiors Management System

## Technology
- Angular
- ASP.NET Core
- EF Core
- SQL Server

## Completed
- [x] Inspected current repository structure
- [x] Inspected supplied Standard Interiors logo assets
- [x] Confirmed local SQL Server availability
- [x] Scaffolded backend API and Angular frontend
- [x] Implemented core EF Core domain model and migration
- [x] Applied initial SQL Server migration to `StandardInteriorsDb`
- [x] Implemented calculation tests and core backend tests
- [x] Built Angular frontend shell and responsive navigation
- [x] Wired live API client and major dashboard/customer/estimate/payment pages
- [x] Added repository docs and manual SQL fallback scripts
- [x] Implemented real XLSX exports with ClosedXML
- [x] Added JWT authentication with secure password hashing
- [x] Added Admin, Staff, and Viewer roles
- [x] Added backend authorization protection and payment-void permission
- [x] Added Angular branded login, auth guard, interceptor, logout, and Admin Users page
- [x] Added CreatedBy/UpdatedBy and payment void audit attribution
- [x] Applied authentication/audit migration to SQL Server
- [x] Added authentication and authorization tests
- [x] Added estimate-backed customer project/work status tracking
- [x] Added WorkStatusHistories and additive EF migration
- [x] Added work status dashboard summary, customer projects, status editor, and timeline UI
- [x] Added first-render loading/error states across primary data pages
- [x] Added server-side report/export filters
- [x] Added Angular XLSX export actions
- [x] Fixed customer, estimate, and payment list EF translation issues
- [x] Ran live acceptance flow against SQL Server
- [x] Generated and inspected sample PDFs and XLSX workbooks

## In Progress
- [ ] Production deployment secret management and operational hardening

## Pending
- [ ] Optional refresh-token/session revocation workflow for multi-device deployments

## Database
Database:
StandardInteriorsDb

Migration:
InitialStandardInteriorsSchema
AddAuthenticationAndAudit

Tables:
- Customers
- Services
- PaymentMethods
- Estimates
- EstimateSections
- EstimateItems
- Payments
- CompanySettings
- EstimateStatusHistory
- Users

## APIs Implemented
- Dashboard
- Customers
- Services
- Payment methods
- Estimates
- Payments
- Reports
- Settings
- XLSX exports under `/api/exports`

## UI Pages Implemented
- Dashboard
- Customers
- Customer Details
- Estimates
- Estimate Builder
- Payments
- Services
- Reports
- Settings

## PDF/Reports
- Estimate PDF
- Payment sheet PDF
- Payment receipt PDF
- Customer summary PDF
- Customer statement PDF
- XLSX: customers, customer summary, customer statement, estimates, payments, outstanding, fully paid, monthly collections

## Authentication
- `POST /api/auth/login`
- `GET /api/auth/me`
- `GET/POST/PUT /api/users` (Admin only)
- JWT bearer authentication; signing key is read from `STANDARD_INTERIORS_JWT_KEY`
- Initial Admin password is read from `STANDARD_INTERIORS_ADMIN_PASSWORD` only when seeding an empty Users table

## Authorization Matrix
- Admin: full access, settings, services, users, payment void
- Staff: customers, estimates, payments, reports; payment void only with `CanVoidPayments`
- Viewer: authenticated read-only business APIs

## Testing
- Backend calculation tests
- Payment void/outstanding tests
- Build verification for backend and frontend
- Live SQL-backed acceptance flow
- Live pricing-mode checks
- Live persistence check after backend restart
- XLSX ZIP/workbook integrity checks
- PDF visual/text inspection
- Authentication tests: hashed password, login, wrong-password rejection, permission handler
- Live auth verification: anonymous 401, Viewer 403 write, Staff 403 settings/void, granted void 204

## Known Issues
- PDF text extraction displays some glyphs such as `₹` and certain ligatures imperfectly in the current inspection renderer, but the rendered PDF visuals show the correct symbols and values.
- QuestPDF package restore emits NU1603 because version `2024.9.1` is unavailable; `2024.10.0` is resolved.
- Refresh-token/session revocation is not implemented; use short environment-configured token lifetimes and operational key rotation.

## Decisions
- Use SQL Server trusted connection during development.
- Treat backend as the source of truth for all financial calculations.
- Use soft-delete style states for financial and master records where required.

## How to Run
- Backend: `dotnet run --project backend/src/StandardInteriors.Api/StandardInteriors.Api.csproj`
- Frontend: `cd frontend && npm start`
- SQL Server: `.\SQLEXPRESS` with database `StandardInteriorsDb`
- Set `STANDARD_INTERIORS_JWT_KEY` to a secret of at least 32 bytes.
- On first startup of an empty Users table set `STANDARD_INTERIORS_ADMIN_PASSWORD` to a password of at least 12 characters.

## Acceptance Results
- Acceptance customer: `CUS-000001`
- Main estimate: `EST-2026-00001`
- Estimate total: `₹270,000.00`
- Posted received: `₹80,000.00`
- Outstanding: `₹190,000.00`
- Voided payment: `₹30,000.00`, retained with status `Voided` and excluded from totals
- Dynamic service: `Modular Kitchen` created, used, disabled, and retained in the estimate
- Custom work: `Decorative Wall Panel` persisted
- Live pricing modes: item-based `₹100,000.00`, section-based `₹100,000.00`, overall `₹270,000.00`
- Restart persistence: confirmed through API and direct SQL query
- Generated artifacts: `backend/tests/Acceptance/test-output/`

## Next Steps
- Add refresh-token/session revocation if required by deployment policy.

## Manual UI/UX Correction Pass
- Customer create root cause: the API accepted the request and returned `201`; the Angular form had no request error state, no saving/finalize state, no success feedback, and optional controls were sent as empty strings. The form now normalizes optional values to `null`, disables duplicate submits, resets saving in `finalize`, shows API validation messages, closes on success, reloads the list, and displays a success notice.
- Customer list root cause: the API returns `{ items, totalCount, page, pageSize }` and the mapping was correct, but the page had no loading/error/empty state and provided weak feedback when a request failed. The page now requests an explicit first page, supports all filters, shows retry/error states, and renders responsive customer cards.
- Added backend DataAnnotations for required customer name/phone and bounded optional fields; invalid requests now produce normal API validation responses.
- Reworked the application shell with a responsive mobile drawer, overlay, compact top bar, human-readable role display, shared page spacing, and consistent button/card styles.
- Sidebar now crops the supplied logo asset to the emblem only, enlarges it inside the dark round mark, and keeps the company name beside it. Shared page headers no longer repeat `Standard Interiors`.
- Added intentional light/dark theme support with localStorage persistence and contrast overrides for surfaces, tables, forms, dialogs, and navigation.
- Added authenticated blob downloads for global XLSX exports and customer-specific PDF/XLSX exports so bearer-protected files work from the UI.
- Verification: API customer POST returned `201`; customer list returned `200` with existing records; frontend build passed; backend build passed; all 10 backend tests passed; Angular dev server smoke check passed at `http://127.0.0.1:4200/`.

## Authentication Acceptance Results
- Anonymous `GET /api/dashboard`: `401`
- Admin login and `/api/auth/me`: `200`, role `Admin`
- Viewer dashboard read: `200`
- Viewer create customer: `403`
- Staff settings access: `403`
- Staff without void grant: `403`
- Staff with void grant: `204`
- SQL audit columns populated for live staff-created customer/estimate/payment and granted void operation

## Customer Work Status Acceptance Results
- API dashboard work-status summary: `200`
- Customer projects endpoint: `200`, returning 2 estimate-backed projects for the acceptance customer
- Start work: `204`, actual start date recorded
- Progress update: `204`, progress stored at `40%` and `80%`
- Temporary stop: `204`, stop date and reason stored
- Resume: `204`, resume transition stored and prior stop history retained
- Completion: `204`, progress forced to `100%` and actual end date stored
- Permanent stop without confirmation/reason: rejected with `400`; permanent stop cannot resume
- Invalid progress: rejected with `400`
- Work status history: 5 transitions persisted for the lifecycle acceptance estimate

## Status Badge and User Management Refinement
- Added reusable `StatusBadgeComponent` and shared semantic chip styling: neutral, gold, blue, orange, green, and red.
- Applied consistent light status chips to estimate status, payment status, service status, user status, customer payment/work status, dashboard work summaries, and customer project views.
- Added Admin-only user activation endpoint `POST /api/users/{id}/activate`; existing deactivation remains `POST /api/users/{id}/deactivate`.
- Reworked Users page with username/display-name/role search, row selection, edit/update mode, optional password reset, active/inactive control, confirmation before blocking, and immediate list refresh.
- Alignment cleanup uses shared content cards, field/button spacing, table padding, status alignment, and responsive user/customer table behavior.
- Live verification: existing Admin ID `1` update returned `204`; block returned `204` and login returned `401`; activation returned `204` and login returned `200`; user count remained `4`.
- Final verification: backend build passed, `13/13` tests passed, and Angular build passed.

## Accepted Layout Refinement
- Preserved the existing cream/off-white shell, dark sidebar, gold accents, header, typography, theme, and navigation.
- Corrected table geometry with shared table-cell padding, vertical alignment, explicit dashboard/report column widths, separate status/action columns, and controlled horizontal scrolling.
- Removed the broad table-cell `nowrap` rule that caused headings and values to visually collide; ordinary text can wrap while dates, amounts, statuses, and actions remain compact.
- Restored a practical Users two-column layout with a minimum 400px edit panel at desktop widths and full-width stacked behavior on smaller screens.
- Retained StatusBadgeComponent, semantic status colors, user search/select/edit, password reset, block, and activate functionality.
- Running verification: Angular served `200` at `http://127.0.0.1:4200/`; API login served `200` at `http://127.0.0.1:5087`; backend tests `13/13` passed; Angular build passed.

## First-Click Data Loading
- Root cause: the protected API and bearer interceptor were functioning on the first request; the blank appearance came from pages rendering their empty collections while asynchronous `ngOnInit` requests were pending, with no request-level loading/error state. Payments and Reports also started multiple independent requests without an aggregate state.
- Fix: primary pages now render explicit loading, success, empty, and retryable error states. Payments and Reports use `forkJoin` for their initial request sets. Customer Details loads the consolidated customer/projects view first and lazy-loads secondary tabs.
- Verification: authenticated live requests for Dashboard, Customers, Customer Details, Projects, Estimates, Payments, Services, Reports, Settings, and Users all returned `200` on the first request after one API start.

## Save Reliability and Baseline Profiling
- Estimate Builder now requires a customer before submission, prevents duplicate estimate requests, shows `Saving...`, and displays API/network errors while reliably clearing the saving state with `finalize`.
- Services now prevent duplicate submissions, show `Saving...`, display API/network errors, and reset all form defaults after a successful create.
- Existing Users save flow already had equivalent saving guards, error feedback, and `finalize` cleanup; no change was needed there.
- Authenticated read-only baseline on the local API: Customers page 68 ms, Estimates page 28 ms, Services 14 ms, Users 13 ms for the first warm requests using page size 20 where applicable.
- Verification: backend tests passed `13/13`; Angular production build passed using the direct CLI entry point. The normal `npm run build` wrapper is unreliable in this workspace because the root path contains `&`; the build emitted only the existing Angular compiler cache warning for missing optional `lmdb`.

## Live Optimization Acceptance
- Isolated live customer `CUS-000010` was created with HTTP `201` in `325 ms`.
- Item Based estimate `EST-2026-00007` / ID `7`: HTTP `201`, `905 ms`, reloaded with total `100.00`, exactly one matching estimate.
- Section Based estimate `EST-2026-00008` / ID `8`: HTTP `201`, `82 ms`, reloaded with total `200.00`, exactly one matching estimate.
- Overall estimate `EST-2026-00009` / ID `9`: HTTP `201`, `83 ms`, reloaded with total `300.00`, exactly one matching estimate.
- Test Staff user `live_staff_20260902125928` / ID `5` was created once, appeared in the user list, authenticated successfully, and was deactivated with HTTP `204`. Duplicate username POST returned HTTP `500` with a generic error, so duplicate-name error handling remains a backend issue.
- Test service `LIVE VERIFY SERVICE 20260902125928` / ID `13` was created with HTTP `200` in `40 ms`; one record was returned by the service list. API-level request count was one.
- Test payment `PAY-2026-00005` / ID `5` was created with HTTP `201` in `306 ms`, persisted through GET, and recalculated estimate `9` to received `50.00` and balance `250.00`.
- Work-status update on estimate `9` returned HTTP `204` in `228 ms`.
- Authenticated warm API timings: Dashboard `256 ms`; Customers `177 ms`; Customer Search `234 ms`; Customer Details `121 ms`; Estimates `101 ms`; Estimate Details `130 ms`; Payments `98 ms`; Services `66 ms`; Reports `241 ms`; Users `64 ms`.
- Before timings: not reproducible. No endpoint was rewritten based only on these measurements.
- Browser-level Angular network timing and runtime SQL command counts were not available in this environment because no browser automation/runtime SQL logging capture was configured. Therefore UI duplicate-click behavior and SQL N+1 status remain unverified rather than claimed as passes.
