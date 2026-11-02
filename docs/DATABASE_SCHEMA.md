# Database Schema

## Connection
- SQL Server instance: `.\SQLEXPRESS`
- Database: `StandardInteriorsDb`
- Migration: `InitialStandardInteriorsSchema`

## Tables
- `Customers`: customer identity, contact information, activity state
- `Services`: extensible service/work master with active state and display order
- `PaymentMethods`: configurable payment methods
- `Estimates`: customer estimate header, pricing mode, status, revision, cached totals
- `EstimateSections`: ordered estimate sections and optional section pricing override
- `EstimateItems`: linked master services or custom work items, quantity/rate/manual amount
- `Payments`: posted/voided financial transactions and audit fields
- `CompanySettings`: branding, prefixes, currency, default estimate note
- `EstimateStatusHistory`: estimate status change history
- `Users`: password hash, role, active state, and payment-void permission
- `Estimates`: execution status, progress, planned/actual work dates, stop/resume details
- `WorkStatusHistories`: immutable estimate work-status transitions with notes, dates, and user attribution

## Money and Totals
- SQL Server monetary columns use `decimal(18,2)`.
- Posted payments only contribute to received totals.
- `Sent`, `Approved`, and `Completed` estimates contribute to customer estimated value.
- Draft, rejected, and cancelled estimates do not contribute to customer financial totals.
- Outstanding is calculated as applicable estimate value minus posted payments, floored at zero for display.
- Estimate cached totals are recalculated by the backend after estimate/payment changes.
- Passwords are stored only as ASP.NET Core Identity-compatible password hashes.
- Customer, estimate, and payment records store nullable creator/updater user IDs; payments also store the user who voided them.
- Work status is separate from financial `EstimateStatus`. Existing estimates default to `NotStarted` and progress `0`.
- Migration `20260829123611_AddWorkStatusAndHistory` is additive and preserves existing estimate, payment, and customer rows.

## Query Strategy
- Customers, estimates, and payments use server-side paging for list APIs.
- Report date/customer/status/method filters are applied to EF queries before materialization.
- Outstanding report predicates are evaluated in SQL using aggregate subqueries.
- The dashboard top-five outstanding widget materializes only the small dashboard candidate set before local top-five ordering.
