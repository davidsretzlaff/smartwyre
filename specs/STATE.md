# STATE

## Decisions

### AD-001: Test Data Builders for anemic Types
- Date: 2026-08-16
- Decision: Provide Bogus-backed Data Builders for `Product`, `Rebate`, and `CalculateRebateRequest` under `Smartwyre.DeveloperTest.Tests/DataBuilders`
- Rationale: Project has no rich domain yet; builders target existing Types for readable tests

### AD-002: Calculate tests before production changes
- Date: 2026-08-16
- Decision: This stage delivers only the test suite (builders + Moq). Do not change `RebateService` / datastore implementation yet.
- Rationale: Explicit user direction — tests first; seams and real datastore later

### AD-003: Seams reverted
- Date: 2026-08-16
- Decision: Removed temporary interfaces/DI/null-guard changes. Production matches original concrete `new` stores again.
- Rationale: User rejected changing implementation in this stage. Tests target the future injectable API (expected red / non-compiling until seams exist).

### AD-004: Single specs root
- Date: 2026-08-16
- Decision: Keep all feature docs under `specs/` (not `.specs/`)
- Rationale: User asked to consolidate everything in the same `specs` folder

## Handoff

- Specs root: `specs/`
- Features (order):
  - `specs/01-test-data-builders/` — complete
  - `specs/02-rebate-calculate-tests/` — tests authored; production unchanged; build red until seams
- Branch: `feature/refactor`
- Next: implement injectable stores / `RebateService` wiring so `RebateServiceTests` compile and pass
