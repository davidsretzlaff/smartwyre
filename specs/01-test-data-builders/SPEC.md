# SPEC: Test Data Builders

## Intent

Add a Data Builder pattern in `Smartwyre.DeveloperTest.Tests` so unit tests can assemble existing `Types` objects with fluent, readable setup — without introducing a domain layer.

## Context

- Source project has anemic DTOs under `Smartwyre.DeveloperTest.Types` (`Product`, `Rebate`, `CalculateRebateRequest`, etc.), not a rich domain model.
- Reference style: FC4 `DataBuilders` (`*DataBuilder`, fluent `With*`, `Build()`, Bogus defaults, static entry factories).
- Scope for this feature: builders + test project wiring only (no production service refactor, no example test usage).

## Decisions (Discuss)

| # | Decision |
| --- | --- |
| Types | Builders for `Product`, `Rebate`, `CalculateRebateRequest` only |
| Defaults | Bogus (`pt_BR`) for realistic randomized defaults |
| API shape | Generic `With*` plus incentive scenario helpers |
| Layout | `Smartwyre.DeveloperTest.Tests/DataBuilders/*DataBuilder.cs` |
| Deliverable | Builders + `ProjectReference` + Bogus package; no demo tests |
| Out of scope | `CalculateRebateResult`, `RebateCalculation`, Moq stores, RebateService changes |

## Requirements (EARS)

### Ubiquitous

- **REQ-01**: While building test data, the test project SHALL provide fluent Data Builders for `Product`, `Rebate`, and `CalculateRebateRequest`.
- **REQ-02**: While constructing objects, each builder SHALL expose a `Build()` method that returns a fully populated instance of the target type.
- **REQ-03**: While constructing objects, each builder SHALL initialize property defaults via Bogus using the `pt_BR` locale unless overridden.

### Event-driven

- **REQ-04**: When a caller chains a `With*` method, the builder SHALL override only that property and preserve other current values.
- **REQ-05**: When a caller invokes an incentive scenario helper on `RebateDataBuilder`, the builder SHALL set `Incentive` (and sensible related fields) for that incentive type.
- **REQ-06**: When a caller invokes a supported-incentive helper on `ProductDataBuilder`, the builder SHALL set `SupportedIncentives` accordingly (including multi-flag combinations where applicable).

### State-driven

- **REQ-07**: While the test project builds, it SHALL reference `Smartwyre.DeveloperTest` and include the Bogus NuGet package.
- **REQ-08**: While builders live in the test assembly, they SHALL reside under namespace `Smartwyre.DeveloperTest.Tests.DataBuilders`.

### Unwanted

- **REQ-09**: If a type is not `Product`, `Rebate`, or `CalculateRebateRequest`, the feature SHALL NOT add a Data Builder for it in this change.
- **REQ-10**: The feature SHALL NOT modify production code under `Smartwyre.DeveloperTest` (except what is already required by referencing it from tests).
- **REQ-11**: The feature SHALL NOT add or rewrite unit test cases that exercise `RebateService` (builders-only delivery).

## Acceptance criteria

1. `Smartwyre.DeveloperTest.Tests` compiles with a `ProjectReference` to `Smartwyre.DeveloperTest`.
2. Bogus is referenced from the test project.
3. `DataBuilders` folder contains `ProductDataBuilder`, `RebateDataBuilder`, and `CalculateRebateRequestDataBuilder`.
4. Each builder supports fluent overrides and `Build()`.
5. `dotnet test Smartwyre.DeveloperTest.sln` still runs (existing placeholder test behavior unchanged except project can resolve types if referenced).

## Traceability notes

- Builders target existing anemic types (public setters), not domain constructors.
- Scenario helpers recommended for the three known `IncentiveType` / `SupportedIncentiveType` values to keep future rebate tests readable.
