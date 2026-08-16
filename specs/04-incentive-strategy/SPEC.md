# Incentive Strategy (OCP) Specification

## Problem Statement

`RebateService.Calculate` uses a growing `switch` on `IncentiveType`. Every new incentive forces edits to the service, violating Open/Closed and making the interview’s “many more incentive types” goal hard. We need a Strategy design: one `Apply` abstraction, one class per incentive, and polymorphism from `Calculate`.

## Goals

- [x] Replace the incentive `switch` with strategy polymorphism (`Apply`)
- [x] Adding a new incentive means a new strategy class (+ factory registration), not rewriting Calculate logic
- [x] Preserve existing Calculate behavior (S01–S16 still pass)
- [x] Keep `RebateService` thin: load data → resolve strategy → `Apply` → store on success

## Out of Scope

| Feature | Reason |
| --- | --- |
| EF / external persistence changes | Already covered by feature 03 |
| Renaming or removing `Types.Rebate` DTO | Still the persistence/API shape from the store |
| Full DI container / assembly scanning | Interview scope — explicit factory is enough |
| Changing Runner UX / seed ids | Unrelated unless wiring breaks |
| FluentValidation or other frameworks | Keep dependencies minimal |

---

## Assumptions & Open Questions

| Assumption / decision | Chosen default | Rationale | Confirmed? |
| --------------------- | -------------- | --------- | ---------- |
| Strategy interface name | `IRebateIncentive` with `Apply` (not `IRebate` / `Rebate`) | Avoids clash with existing `Types.Rebate` DTO | y |
| Concrete names | `FixedCashAmountRebate`, `FixedRateRebate`, `AmountPerUomRebate` | Matches user’s example and incentive types | y |
| Construction | Strategy ctor receives the values it needs from the DTO (e.g. FixedCash gets `amount`) | Encapsulates rule inputs; Apply focuses on product/volume context | y |
| `Apply` signature | `RebateApplicationResult Apply(Product product, decimal volume)` | Product + volume cover all three current formulas; result carries Success + Amount | y |
| `RebateApplicationResult` | `{ bool Success; decimal Amount; }` | Mirrors current Calculate outcomes without overloading `CalculateRebateResult` | y |
| Validation ownership | Each strategy validates its own rules (null product, supported flag, zeros) | True OCP — service does not know per-type rules | y |
| Service null rebate | Service still returns failure before resolving strategy when DTO is null | No strategy without a rebate record | y |
| Strategy resolution | `IRebateIncentiveFactory.Create(Rebate rebate)` maps `IncentiveType` → strategy | Service depends on abstraction; factory is the single registration point | y |
| Unknown `IncentiveType` | Factory returns null / failure path → Calculate Success = false | Safe default | y |
| Folder | `Smartwyre.DeveloperTest/Incentives/` (interface, strategies, factory, result) | Clear feature boundary next to Services | y |
| Existing tests | Keep S01–S16; adapt only if constructors/wiring require (behavior unchanged) | Regression safety | y |

**Open questions:** none — naming and Apply signature confirmed by user.

---

## Proposed shape (design intent)

```text
RebateService.Calculate
  → load Rebate DTO + Product
  → if rebate null → fail
  → IRebateIncentive strategy = factory.Create(rebate)   // polymorphism entry
  → if strategy null → fail
  → RebateApplicationResult applied = strategy.Apply(product, request.Volume)
  → if applied.Success → store(rebate, applied.Amount)
  → return CalculateRebateResult { Success = applied.Success }

IRebateIncentive
  ← FixedCashAmountRebate(amount)           Apply → amount (if product supports FixedCash, amount ≠ 0)
  ← FixedRateRebate(percentage)             Apply → price * percentage * volume
  ← AmountPerUomRebate(amount)              Apply → amount * volume
```

**Open/Closed:** new incentive = new class implementing `IRebateIncentive` + one factory mapping. `RebateService` stays closed for modification.

---

## User Stories

### P1: Strategy abstraction and implementations ⭐ MVP

**User Story**: As a developer adding incentive types, I want each incentive as its own class with `Apply` so that calculation rules stay isolated and extensible.

**Why P1**: Core of the SOLID OCP/Strategy refactor requested by the exercise and the user.

**Acceptance Criteria**:

1. The system SHALL define a strategy interface exposing `Apply` that returns success/amount for a product and volume
2. WHEN incentive is Fixed Cash THEN `FixedCashAmountRebate` SHALL be constructible with the cash amount and SHALL compute via `Apply`
3. WHEN incentive is Fixed Rate THEN `FixedRateRebate` SHALL be constructible with the percentage and SHALL compute `Price * Percentage * Volume` via `Apply` when valid
4. WHEN incentive is Amount Per Uom THEN `AmountPerUomRebate` SHALL be constructible with the per-uom amount and SHALL compute `Amount * Volume` via `Apply` when valid
5. IF a strategy’s validation fails (null product, unsupported flag, zero inputs per existing rules) THEN `Apply` SHALL return `Success = false`
6. The system SHALL NOT keep per-incentive calculation branches inside `RebateService.Calculate`

**Independent Test**: Unit-test each strategy’s `Apply` in isolation (success + main failure paths); service tests S01–S16 remain green.

---

### P1: Factory + polymorphic Calculate ⭐ MVP

**User Story**: As `RebateService`, I want to resolve an `IRebateIncentive` from the loaded rebate DTO and call `Apply` so that Calculate uses polymorphism instead of a switch on formulas.

**Why P1**: Connects store DTO to strategy without the service knowing formulas.

**Acceptance Criteria**:

1. The system SHALL provide `IRebateIncentiveFactory` with `Create(Rebate rebate)` returning the matching strategy (or null if unsupported)
2. WHEN `Calculate` runs with a known rebate THEN the service SHALL obtain the strategy from the factory and call `Apply`
3. WHEN `Apply` succeeds THEN the service SHALL persist via `StoreCalculationResult` with the applied amount
4. WHEN rebate is null OR factory returns null OR `Apply` fails THEN the service SHALL return `Success = false` and SHALL NOT store
5. `RebateService` SHALL receive `IRebateIncentiveFactory` (in addition to existing stores) via constructor injection

**Independent Test**: Existing `RebateServiceTests` pass with real factory (or test double factory only if needed); no formula `switch` remains in the service.

---

### P2: Document extension point

**User Story**: As a future developer, I want a short comment or README note on how to add an incentive so that OCP is obvious in review.

**Why P2**: Interview walkthrough; not required for behavior.

**Acceptance Criteria**:

1. The factory (or Incentives folder README snippet) SHALL document: add class implementing the strategy interface, register in factory

**Independent Test**: Manual review.

---

## Edge Cases

- IF `product` is null THEN each strategy that requires product support SHALL fail (same as today’s FixedCash/FixedRate/AmountPerUom rules)
- IF `volume` is 0 THEN FixedRate and AmountPerUom SHALL fail; FixedCash ignores volume (unchanged behavior)
- IF factory receives null rebate DTO THEN it SHALL return null (service already guards)
- IF a new enum value is not registered THEN Calculate SHALL fail closed (Success = false)

---

## Requirement Traceability

| Requirement ID | Story | Phase | Status |
| --- | --- | --- | --- |
| STRAT-01 | P1: strategy interface with Apply | Execute | Verified |
| STRAT-02 | P1: FixedCashAmountRebate | Execute | Verified |
| STRAT-03 | P1: FixedRateRebate strategy | Execute | Verified |
| STRAT-04 | P1: AmountPerUomRebate strategy | Execute | Verified |
| STRAT-05 | P1: validation inside strategies | Execute | Verified |
| STRAT-06 | P1: no formula switch in service | Execute | Verified |
| STRAT-07 | P1: factory Create(Rebate) | Execute | Verified |
| STRAT-08 | P1: service calls Apply polymorphically | Execute | Verified |
| STRAT-09 | P1: store on success | Execute | Verified |
| STRAT-10 | P1: fail paths do not store | Execute | Verified |
| STRAT-11 | P1: inject factory into RebateService | Execute | Verified |
| STRAT-12 | P1: S01–S16 remain green | Execute | Verified |
| STRAT-13 | P2: extension documentation | Execute | Verified |

**Coverage:** 13 total, 13 verified, 0 unmapped

---

## Success Criteria

- [x] `Calculate` has no per-incentive amount formula `switch`
- [x] Three strategy classes + factory + interface under `Incentives/`
- [x] Service orchestrates: load → create strategy → `Apply` → store
- [x] `dotnet test` all green (including S01–S16)
- [x] Adding a fourth incentive requires a new class + factory registration only (service untouched for formulas)
