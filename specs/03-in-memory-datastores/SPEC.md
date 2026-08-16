# In-Memory Data Stores Specification

## Problem Statement

`RebateService` hard-codes empty stub data stores, so lookups always return blank objects and unit tests that expect constructor injection do not compile. The Runner also has no sample rebates/products to exercise Calculate end-to-end. We need injectable in-memory stores, shared seed data, and service wiring so tests and the console app can use the same persistence seam.

## Goals

- [ ] `IRebateDataStore` / `IProductDataStore` exist and are implemented by in-memory stores
- [ ] `RebateService` is constructed with those stores; null rebate/product yield `Success = false`
- [ ] Shared `Data/Seed/` sample data can preload stores for Runner and Tests
- [ ] Existing `RebateServiceTests` (S01–S16) compile and pass; Runner can calculate against seeded samples

## Out of Scope

| Feature | Reason |
| --- | --- |
| EF Core / EF InMemory / SQL / external DB | Overkill for interview exercise; plain in-memory collections are enough |
| New shared `.csproj` for seed only | Main library already referenced by Runner and Tests |
| Redesigning incentive calculation strategies | Separate SOLID refactor; this feature only adds seams + storage |
| Changing the S01–S16 scenario matrix | Tests already authored; make them green |
| Authentication, concurrency locks, durability | In-process demo only |

---

## Assumptions & Open Questions

| Assumption / decision | Chosen default | Rationale | Confirmed? |
| --------------------- | -------------- | --------- | ---------- |
| Persistence style | Plain in-memory (`Dictionary`/`List`), not EF InMemory | Simple, fast, no extra packages | y |
| Seed location | `Smartwyre.DeveloperTest/Data/Seed/` | Lives next to stores; shared by Runner and Tests via main lib reference (not inside the test project) | y |
| Lookup miss | Return `null` | Enables failure paths and matches test expectations | y |
| `StoreCalculationResult` | `void`; append to in-memory calculation list | Current API only saves | y |
| Wire `RebateService` + null guards | Yes — part of this feature | Unblocks green `RebateServiceTests` | y |
| Concrete class names | Keep `RebateDataStore` / `ProductDataStore` implementing the interfaces (in-memory bodies) | Minimal rename churn; behavior becomes in-memory | y |
| Identifier matching | Case-sensitive exact match on `Identifier` | Predictable for seeds and tests | y |
| Seed content | At least one sample per incentive type (FixedCash, FixedRate, AmountPerUom) plus matching products | Runner can demo all three paths | y |

**Open questions:** none - all resolved or logged above.

---

## User Stories

### P1: Injectable in-memory stores ⭐ MVP

**User Story**: As a developer, I want rebate/product data behind interfaces backed by in-memory collections so that tests and the Runner can control and share sample data without a database.

**Why P1**: Without this, Calculate cannot be exercised with real lookups and unit tests cannot compile.

**Acceptance Criteria**:

1. The system SHALL provide `IRebateDataStore` with `GetRebate(string)` and `StoreCalculationResult(Rebate, decimal)`
2. The system SHALL provide `IProductDataStore` with `GetProduct(string)`
3. WHEN `GetRebate` / `GetProduct` is called with an unknown identifier THEN the store SHALL return `null`
4. WHEN `GetRebate` / `GetProduct` is called with a seeded/known identifier THEN the store SHALL return the matching entity
5. WHEN `StoreCalculationResult` is called THEN the rebate store SHALL retain a calculation record in memory (void return)
6. `RebateDataStore` and `ProductDataStore` SHALL implement the interfaces using in-memory collections (no DB I/O)

**Independent Test**: Unit-test stores directly or via Moq contracts; unknown id → null; known id → entity; store then observe retained calculation count/content.

---

### P1: Wire RebateService for testability ⭐ MVP

**User Story**: As a developer, I want `RebateService` to receive stores via constructor so that existing scenario tests compile and pass.

**Why P1**: Spec `02` already authored S01–S16 against this API.

**Acceptance Criteria**:

1. `RebateService` SHALL accept `IRebateDataStore` and `IProductDataStore` via constructor and SHALL NOT `new` store instances inside `Calculate`
2. WHEN rebate lookup returns `null` THEN `Calculate` SHALL return `Success = false` and SHALL NOT call `StoreCalculationResult`
3. WHEN incentive is `FixedCashAmount` and product is `null` THEN `Calculate` SHALL return `Success = false` and SHALL NOT call `StoreCalculationResult`
4. WHEN `Calculate` succeeds THEN it SHALL call `StoreCalculationResult` on the injected rebate store with the computed amount
5. WHEN `dotnet test Smartwyre.DeveloperTest.sln` runs THEN all `RebateServiceTests` SHALL pass

**Independent Test**: `dotnet test` — 16/16 green.

---

### P1: Shared seed data ⭐ MVP

**User Story**: As a developer running the console app or writing integration-style setups, I want sample rebates and products under `Data/Seed/` so that both Runner and Tests can preload the same in-memory stores.

**Why P1**: Keeps seed next to the data layer; avoids duplicating fixtures; Runner does not depend on the test project.

**Acceptance Criteria**:

1. The main library SHALL contain `Data/Seed/` (e.g. `SampleData` / seeder helper) that exposes sample `Rebate` and `Product` collections covering all three incentive types
2. WHEN the Runner starts THEN it SHALL load seed data into the in-memory stores before accepting Calculate input
3. Seed helpers SHALL be usable from the test project by referencing the main library (seed SHALL NOT live only under Tests)
4. The Runner SHALL construct `RebateService` with the seeded in-memory stores (not parameterless)

**Independent Test**: Run Runner with a seeded rebate/product id and volume; observe `Success: True` for at least one happy path. Tests may optionally call the same seeder in a fixture (not required for every unit test that already uses Moq).

---

### P2: Visible sample listing on startup

**User Story**: As a Runner user, I want sample identifiers printed at startup so that I know which rebate/product ids I can try.

**Why P2**: Improves demo UX; not required for Calculate correctness.

**Acceptance Criteria**:

1. WHEN the Runner starts AFTER seeding THEN it SHALL print a short list of sample rebate identifiers and product identifiers to the console

**Independent Test**: Launch Runner; see sample ids before prompts/args handling.

---

## Edge Cases

- IF identifier is null/whitespace on get THEN store SHALL return `null` (treat as not found)
- IF duplicate identifiers are seeded THEN last write wins OR seeder uses unique ids only (seeder SHALL use unique identifiers)
- IF `StoreCalculationResult` is called with null rebate THEN store SHALL no-op or ignore without throwing (defensive; Calculate should not call in that state)
- WHEN the same store instance is used for get and store THEN calculations remain visible on that instance for the process lifetime

---

## Requirement Traceability

| Requirement ID | Story | Phase | Status |
| --- | --- | --- | --- |
| STORE-01 | P1: IRebateDataStore | Execute | Verified |
| STORE-02 | P1: IProductDataStore | Execute | Verified |
| STORE-03 | P1: get unknown → null | Execute | Verified |
| STORE-04 | P1: get known → entity | Execute | Verified |
| STORE-05 | P1: StoreCalculationResult retains in memory | Execute | Verified |
| STORE-06 | P1: concrete in-memory implementations | Execute | Verified |
| STORE-07 | P1: RebateService ctor injection | Execute | Verified |
| STORE-08 | P1: null rebate → failure | Execute | Verified |
| STORE-09 | P1: FixedCash null product → failure | Execute | Verified |
| STORE-10 | P1: success stores via injected store | Execute | Verified |
| STORE-11 | P1: RebateServiceTests green | Execute | Verified |
| STORE-12 | P1: Data/Seed/ module in main lib | Execute | Verified |
| STORE-13 | P1: Runner loads seed | Execute | Verified |
| STORE-14 | P1: Seed usable from Tests | Execute | Verified |
| STORE-15 | P1: Runner wires RebateService with stores | Execute | Verified |
| STORE-16 | P2: print sample ids on startup | Execute | Verified |

**Coverage:** 16 total, 16 verified, 0 unmapped

---

## Success Criteria

- [x] Interfaces + in-memory `RebateDataStore` / `ProductDataStore` in place
- [x] `RebateService` injected; null guards for rebate and FixedCash product null
- [x] `Data/Seed/` shared samples covering three incentive types
- [x] Runner seeds, lists samples (P2), and calculates via injected service
- [x] `dotnet test Smartwyre.DeveloperTest.sln` — all tests pass
- [x] No EF / external database packages added
