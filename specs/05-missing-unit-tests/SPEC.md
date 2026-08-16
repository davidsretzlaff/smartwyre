# Missing Unit Tests Specification

## Problem Statement

`RebateServiceTests` covers Calculate end-to-end (S01–S16), but the new Strategy and in-memory store layers have no direct unit tests. Failures in `Apply`, factory mapping, or store lookup/persist can hide behind mocks or only surface indirectly. We need focused tests for incentives, factory, and data stores—using existing Data Builders where arranging entities.

## Goals

- [x] Unit-test each `IRebateIncentive` implementation’s `Apply` (success + main failure paths)
- [x] Unit-test `RebateIncentiveFactory.Create` mapping and null/unknown handling
- [x] Unit-test in-memory `RebateDataStore` / `ProductDataStore` get/store behavior
- [x] Keep using Data Builders for `Product` / `Rebate` arrangement
- [x] `dotnet test` remains fully green; remove any leftover placeholder test if present

## Out of Scope

| Feature | Reason |
| --- | --- |
| Rewriting existing `RebateServiceTests` S01–S16 | Already sufficient for service orchestration |
| Runner / console integration tests | Manual demo is enough for interview scope |
| Changing production incentive or store logic | Tests-only feature |
| Snapshot/UI tests | N/A |

---

## Assumptions & Open Questions

| Assumption / decision | Chosen default | Rationale | Confirmed? |
| --------------------- | -------------- | --------- | ---------- |
| Test layout | `Tests/Incentives/`, `Tests/Data/` mirroring production | Clear ownership | y |
| Arrange style | Prefer Data Builders for Product/Rebate | Consistency with feature 01/02 | y |
| Strategy coverage depth | Per type: 1 success + key failures already encoded in rules (null product, unsupported, zero inputs) | Avoid duplicating every service scenario 1:1 | y |
| Assert amounts | Exact decimals on success (e.g. FixedRate `50 * 0.1 * 4 = 20`) | Spec-defined outcomes | y |
| Factory unknown enum | No easy invalid enum without reflection; cover null rebate + three known mappings | Practical | y |
| Seed/DataSeeder tests | Optional P2 smoke: seeded ids resolve via stores from `DataSeeder.CreateStores()` | Nice demo of shared seed | y |
| Placeholder `PaymentService.Tests` | Delete if still present | Hygiene | y |

**Open questions:** none — defaults above are recommended; confirm to Execute.

---

## Coverage gap (current → target)

| Area | Today | This feature |
| --- | --- | --- |
| `RebateService.Calculate` | 16 tests | Keep as-is |
| `FixedCashAmountRebate.Apply` | Indirect only | Direct unit tests |
| `FixedRateRebate.Apply` | Indirect only | Direct unit tests |
| `AmountPerUomRebate.Apply` | Indirect only | Direct unit tests |
| `RebateIncentiveFactory` | Indirect only | Direct unit tests |
| `RebateDataStore` / `ProductDataStore` | None | Direct unit tests |
| `DataSeeder` / sample ids | None | Optional smoke |

---

## User Stories

### P1: Incentive strategy unit tests ⭐ MVP

**User Story**: As a developer, I want isolated tests for each strategy’s `Apply` so that formula and validation bugs are caught without going through the full service.

**Why P1**: Core of the OCP design; most important missing coverage.

**Acceptance Criteria**:

1. WHEN `FixedCashAmountRebate` is valid (supported product, amount ≠ 0) THEN `Apply` SHALL return `Success = true` and `Amount` equal to the constructed cash amount
2. IF product is null OR incentive unsupported OR amount is 0 THEN `FixedCashAmountRebate.Apply` SHALL return `Success = false`
3. WHEN `FixedRateRebate` is valid THEN `Apply` SHALL return `Success = true` and `Amount = price × percentage × volume`
4. IF product is null OR unsupported OR percentage/price/volume is 0 THEN `FixedRateRebate.Apply` SHALL return `Success = false`
5. WHEN `AmountPerUomRebate` is valid THEN `Apply` SHALL return `Success = true` and `Amount = amount × volume`
6. IF product is null OR unsupported OR amount/volume is 0 THEN `AmountPerUomRebate.Apply` SHALL return `Success = false`
7. Strategy tests SHALL arrange `Product` via `ProductDataBuilder` where a product instance is required

**Independent Test**: Filter tests under `Incentives/`; each AC has a dedicated `[Fact]`.

---

### P1: Factory unit tests ⭐ MVP

**User Story**: As a developer, I want factory tests so that DTO → strategy wiring stays correct when adding types.

**Why P1**: Extension point called out in the Readme.

**Acceptance Criteria**:

1. WHEN `Create` receives a FixedCash DTO THEN it SHALL return a `FixedCashAmountRebate` that applies that amount
2. WHEN `Create` receives a FixedRate DTO THEN it SHALL return a `FixedRateRebate` that applies that percentage
3. WHEN `Create` receives an AmountPerUom DTO THEN it SHALL return an `AmountPerUomRebate` that applies that amount
4. WHEN `Create` receives `null` THEN it SHALL return `null`
5. Factory tests SHALL arrange rebate DTOs via `RebateDataBuilder`

**Independent Test**: `RebateIncentiveFactoryTests` green.

---

### P1: In-memory store unit tests ⭐ MVP

**User Story**: As a developer, I want store tests so that lookup miss/hit and calculation persistence behave as specified in feature 03.

**Why P1**: Stores are real production code used by the Runner; currently untested directly.

**Acceptance Criteria**:

1. WHEN `GetRebate` / `GetProduct` is called with a known seeded identifier THEN the store SHALL return the matching entity
2. WHEN called with an unknown identifier OR whitespace/null THEN the store SHALL return `null`
3. WHEN `StoreCalculationResult` is called with a non-null rebate THEN `RebateDataStore.Calculations` SHALL contain a record with the expected rebate id, incentive type, and amount
4. WHEN `StoreCalculationResult` is called with null rebate THEN the store SHALL not add a calculation
5. Store arrangement MAY use builders and/or small inline seed lists

**Independent Test**: `RebateDataStoreTests` / `ProductDataStoreTests` green.

---

### P2: Seed smoke + hygiene

**User Story**: As a reviewer, I want a quick proof that `DataSeeder` sample ids resolve, and no obsolete placeholder tests remain.

**Why P2**: Confidence for Runner samples; cleanup.

**Acceptance Criteria**:

1. WHEN stores are created via `DataSeeder.CreateStores()` THEN each `SampleData` rebate/product identifier SHALL resolve to non-null
2. IF `PaymentService.Tests.cs` / `PaymentServiceTests` exists THEN it SHALL be removed

**Independent Test**: One smoke test file; solution has no PaymentServiceTests.

---

## Edge Cases

- IF FixedCash `Apply` is called with volume = 0 THEN it SHALL still succeed when other rules pass (volume ignored — matches production)
- IF store constructor receives rebates/products with blank identifiers THEN those entries SHALL be skipped (optional assert if cheap)

---

## Requirement Traceability

| Requirement ID | Story | Phase | Status |
| --- | --- | --- | --- |
| MISS-01 | P1: FixedCash success amount | Execute | Verified |
| MISS-02 | P1: FixedCash failure paths | Execute | Verified |
| MISS-03 | P1: FixedRate success amount | Execute | Verified |
| MISS-04 | P1: FixedRate failure paths | Execute | Verified |
| MISS-05 | P1: AmountPerUom success amount | Execute | Verified |
| MISS-06 | P1: AmountPerUom failure paths | Execute | Verified |
| MISS-07 | P1: builders in strategy tests | Execute | Verified |
| MISS-08 | P1: factory FixedCash mapping | Execute | Verified |
| MISS-09 | P1: factory FixedRate mapping | Execute | Verified |
| MISS-10 | P1: factory AmountPerUom mapping | Execute | Verified |
| MISS-11 | P1: factory null → null | Execute | Verified |
| MISS-12 | P1: factory uses RebateDataBuilder | Execute | Verified |
| MISS-13 | P1: store get known | Execute | Verified |
| MISS-14 | P1: store get unknown/whitespace | Execute | Verified |
| MISS-15 | P1: store calculation retained | Execute | Verified |
| MISS-16 | P1: store null rebate no-op | Execute | Verified |
| MISS-17 | P2: DataSeeder smoke | Execute | Verified |
| MISS-18 | P2: remove PaymentServiceTests | Execute | Verified |

**Coverage:** 18 total, 18 verified, 0 unmapped

---

## Success Criteria

- [x] New tests under `Tests/Incentives/` and `Tests/Data/`
- [x] Strategy + factory + store ACs covered
- [x] Builders used for Product/Rebate arrangement in those tests
- [x] `dotnet test Smartwyre.DeveloperTest.sln` all green
- [x] No production behavior changes
