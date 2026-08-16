# Rebate Calculate Unit Tests Specification

## Problem Statement

`RebateService.Calculate` encodes three incentive paths with multiple validation branches, but the test project has no scenario coverage and still ships a placeholder test. Without exhaustive unit tests built from Data Builders, regressions and incorrect incentive rules will go unnoticed during the interview refactor.

## Goals

- [ ] Every distinct success and failure branch of `Calculate` is locked by a dedicated unit test
- [ ] Tests arrange `Product`, `Rebate`, and `CalculateRebateRequest` exclusively via existing Data Builders
- [ ] Each test asserts `CalculateRebateResult.Success` and whether `StoreCalculationResult` is invoked (and with which amount on success)

## Out of Scope

Explicitly excluded. Documented to prevent scope creep.

| Feature | Reason |
| --- | --- |
| Changing `RebateService.Calculate` business rules | Spec locks current intended scenarios as tests only; production logic changes are a later feature |
| Real `RebateDataStore` / `ProductDataStore` persistence | User deferred datastore implementation |
| `Smartwyre.DeveloperTest.Runner` CLI | Separate exercise item |
| New incentive types beyond the three existing enums | Out of current domain |
| Builders for `CalculateRebateResult` / `RebateCalculation` | Not needed for Arrange of Calculate inputs |
| Keeping `PaymentService.Tests` placeholder | Replaced by `RebateService` tests |

---

## Assumptions & Open Questions

Every ambiguity is resolved or recorded here - nothing is left silently unclear.

| Assumption / decision | Chosen default | Rationale | Confirmed? |
| --------------------- | -------------- | --------- | ---------- |
| Testability seams (store interfaces + ctor injection) | Deferred — not part of this feature’s deliverable | User ordered tests first and forbade changing implementation in the previous turn | y |
| How tests compile/run without seams | Tests are authored against the intended injectable `RebateService` API and remain red until a follow-up feature adds seams | TDD red phase; matches “create all tests, then implement datastore/seams” | y |
| Mock library | Moq | Prior Discuss choice | y |
| Null rebate before `switch` | Expected outcome: `Success = false`, store not called | Matches the dead `if (rebate == null)` branches already present in each case | y |
| Null product on `FixedCashAmount` | Expected outcome: `Success = false`, store not called | Aligns with FixedRate/AmountPerUom branches; current code would NRE — tests encode intended behavior | y |
| Replace placeholder test file | Yes — `RebateServiceTests` replaces `PaymentService.Tests.cs` | Placeholder is unrelated and throws | y |

**Open questions:** none - all resolved or logged above (required before the spec is confirmed).

---

## User Stories

### P1: Exhaustive Calculate scenario suite ⭐ MVP

**User Story**: As a developer, I want a unit test for every `Calculate` success and failure path so that incentive validation behavior is enforced before any datastore or service refactor.

**Why P1**: Without this suite there is no safety net for the interview exercise’s core method.

**Acceptance Criteria** (each line is one EARS pattern):

1. WHEN arranging Calculate inputs THEN the test suite SHALL construct `Product`, `Rebate`, and `CalculateRebateRequest` via Data Builders (`ProductDataBuilder`, `RebateDataBuilder`, `CalculateRebateRequestDataBuilder`)
2. WHEN rebate lookup returns null THEN `Calculate` SHALL return `Success = false` and SHALL NOT call `StoreCalculationResult`
3. WHEN incentive is `FixedCashAmount` and product is null THEN `Calculate` SHALL return `Success = false` and SHALL NOT call `StoreCalculationResult`
4. WHEN incentive is `FixedCashAmount` and product does not support `FixedCashAmount` THEN `Calculate` SHALL return `Success = false` and SHALL NOT call `StoreCalculationResult`
5. WHEN incentive is `FixedCashAmount` and `rebate.Amount` is 0 THEN `Calculate` SHALL return `Success = false` and SHALL NOT call `StoreCalculationResult`
6. WHEN incentive is `FixedCashAmount` and rebate/product/amount are valid THEN `Calculate` SHALL return `Success = true` and SHALL call `StoreCalculationResult` with amount equal to `rebate.Amount`
7. WHEN incentive is `FixedRateRebate` and product is null THEN `Calculate` SHALL return `Success = false` and SHALL NOT call `StoreCalculationResult`
8. WHEN incentive is `FixedRateRebate` and product does not support `FixedRateRebate` THEN `Calculate` SHALL return `Success = false` and SHALL NOT call `StoreCalculationResult`
9. WHEN incentive is `FixedRateRebate` and `rebate.Percentage` is 0 THEN `Calculate` SHALL return `Success = false` and SHALL NOT call `StoreCalculationResult`
10. WHEN incentive is `FixedRateRebate` and `product.Price` is 0 THEN `Calculate` SHALL return `Success = false` and SHALL NOT call `StoreCalculationResult`
11. WHEN incentive is `FixedRateRebate` and `request.Volume` is 0 THEN `Calculate` SHALL return `Success = false` and SHALL NOT call `StoreCalculationResult`
12. WHEN incentive is `FixedRateRebate` and inputs are valid THEN `Calculate` SHALL return `Success = true` and SHALL call `StoreCalculationResult` with amount equal to `product.Price * rebate.Percentage * request.Volume`
13. WHEN incentive is `AmountPerUom` and product is null THEN `Calculate` SHALL return `Success = false` and SHALL NOT call `StoreCalculationResult`
14. WHEN incentive is `AmountPerUom` and product does not support `AmountPerUom` THEN `Calculate` SHALL return `Success = false` and SHALL NOT call `StoreCalculationResult`
15. WHEN incentive is `AmountPerUom` and `rebate.Amount` is 0 THEN `Calculate` SHALL return `Success = false` and SHALL NOT call `StoreCalculationResult`
16. WHEN incentive is `AmountPerUom` and `request.Volume` is 0 THEN `Calculate` SHALL return `Success = false` and SHALL NOT call `StoreCalculationResult`
17. WHEN incentive is `AmountPerUom` and inputs are valid THEN `Calculate` SHALL return `Success = true` and SHALL call `StoreCalculationResult` with amount equal to `rebate.Amount * request.Volume`
18. The test project SHALL reference Moq for store doubles
19. IF a scenario is not in the matrix below THEN the suite SHALL NOT invent additional incentive types

**Independent Test**: Open `RebateServiceTests`, map each method to S01–S16; every AC above has a matching test method asserting Success and store interaction.

---

### P2: Test project hygiene

**User Story**: As a developer, I want obsolete placeholder tests removed so that the suite only reflects rebate calculation scenarios.

**Why P2**: The placeholder `PaymentServiceTests.Test1` always fails and is unrelated.

**Acceptance Criteria**:

1. WHEN the rebate test suite is added THEN the project SHALL NOT retain `PaymentService.Tests.cs` / `PaymentServiceTests`
2. The suite SHALL live under a clear tests location (e.g. `Services/RebateServiceTests.cs`)

**Independent Test**: Solution search shows no `PaymentServiceTests`; `RebateServiceTests` exists.

---

## Edge Cases

- IF rebate is null THEN system SHALL return failure without throwing (intended behavior encoded by tests; current production may NRE until a later feature)
- IF product is null on `FixedCashAmount` THEN system SHALL return failure without throwing (same note)
- WHEN multiple zero-conditions apply on `FixedRateRebate` (e.g. Percentage and Volume both 0) THEN system SHALL return `Success = false` (covered by any single zero branch; no separate combo AC required)
- IF `StoreCalculationResult` would be called on failure THEN the test SHALL fail the scenario (Verify Never)

---

## Scenario Matrix

| ID | AC | Incentive | Condition | Success | Store |
| --- | --- | --- | --- | --- | --- |
| S01 | 2 | any | rebate null | false | not called |
| S02 | 3 | FixedCashAmount | product null | false | not called |
| S03 | 4 | FixedCashAmount | unsupported | false | not called |
| S04 | 5 | FixedCashAmount | Amount == 0 | false | not called |
| S05 | 6 | FixedCashAmount | valid | true | rebate.Amount |
| S06 | 7 | FixedRateRebate | product null | false | not called |
| S07 | 8 | FixedRateRebate | unsupported | false | not called |
| S08 | 9 | FixedRateRebate | Percentage == 0 | false | not called |
| S09 | 10 | FixedRateRebate | Price == 0 | false | not called |
| S10 | 11 | FixedRateRebate | Volume == 0 | false | not called |
| S11 | 12 | FixedRateRebate | valid | true | Price×Percentage×Volume |
| S12 | 13 | AmountPerUom | product null | false | not called |
| S13 | 14 | AmountPerUom | unsupported | false | not called |
| S14 | 15 | AmountPerUom | Amount == 0 | false | not called |
| S15 | 16 | AmountPerUom | Volume == 0 | false | not called |
| S16 | 17 | AmountPerUom | valid | true | Amount×Volume |

---

## Requirement Traceability

| Requirement ID | Story | Phase | Status |
| --- | --- | --- | --- |
| CALC-01 | P1: builders for arrange | Execute | Verified |
| CALC-02 | P1: S01 null rebate | Execute | Verified |
| CALC-03 | P1: S02 FixedCash product null | Execute | Verified |
| CALC-04 | P1: S03 FixedCash unsupported | Execute | Verified |
| CALC-05 | P1: S04 FixedCash amount 0 | Execute | Verified |
| CALC-06 | P1: S05 FixedCash success | Execute | Verified |
| CALC-07 | P1: S06 FixedRate product null | Execute | Verified |
| CALC-08 | P1: S07 FixedRate unsupported | Execute | Verified |
| CALC-09 | P1: S08 FixedRate percentage 0 | Execute | Verified |
| CALC-10 | P1: S09 FixedRate price 0 | Execute | Verified |
| CALC-11 | P1: S10 FixedRate volume 0 | Execute | Verified |
| CALC-12 | P1: S11 FixedRate success | Execute | Verified |
| CALC-13 | P1: S12 AmountPerUom product null | Execute | Verified |
| CALC-14 | P1: S13 AmountPerUom unsupported | Execute | Verified |
| CALC-15 | P1: S14 AmountPerUom amount 0 | Execute | Verified |
| CALC-16 | P1: S15 AmountPerUom volume 0 | Execute | Verified |
| CALC-17 | P1: S16 AmountPerUom success | Execute | Verified |
| CALC-18 | P1: Moq reference | Execute | Verified |
| CALC-19 | P1: no extra incentive types | Execute | Verified |
| CALC-20 | P2: remove placeholder | Execute | Verified |
| CALC-21 | P2: RebateServiceTests location | Execute | Verified |

**Coverage:** 21 total, 21 mapped to tasks T1–T4, 0 unmapped

---

## Success Criteria

- [x] Sixteen scenario tests (S01–S16) exist and map 1:1 to the matrix
- [x] Arrange steps use Data Builders (no hand-built graphs for Product/Rebate/Request)
- [x] Moq is referenced; store interactions are verified
- [x] Placeholder `PaymentServiceTests` is gone
- [x] Real datastore persistence deferred; production unchanged this stage (tests target future injectable API per AD-002/AD-003)
