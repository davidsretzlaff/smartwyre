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

### AD-005: In-memory stores + shared Seed + wire RebateService
- Date: 2026-08-16
- Decision: Use plain in-memory collections (not EF InMemory); seed under `Smartwyre.DeveloperTest/Data/Seed/`; not-found returns null; `StoreCalculationResult` void append; include RebateService ctor injection and null guards in the same feature so tests go green
- Rationale: Discuss with user (Runner+Tests share seed via main lib, not Tests; yes to wiring service; seed beside Data)

### AD-006: Incentive Strategy (OCP)
- Date: 2026-08-16
- Decision: `IRebateIncentive.Apply(Product, volume)` → `RebateApplicationResult`; concrete strategies per incentive constructed with rule values; `IRebateIncentiveFactory.Create(Types.Rebate)`; `RebateService` calls Apply via polymorphism (no formula switch)
- Rationale: User confirmed recommendations (interface name + Apply signature)

## Handoff

- Specs root: `specs/`
- Features (order):
  - `specs/01-test-data-builders/` — complete
  - `specs/02-rebate-calculate-tests/` — complete
  - `specs/03-in-memory-datastores/` — complete
  - `specs/04-incentive-strategy/` — Execute complete (16/16 tests green)
- Branch: `feature/refactor`
- Next: commit when user asks
