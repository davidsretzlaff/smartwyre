# Organize Contracts and Models Specification

## Problem Statement

All request/result DTOs, persistence shapes, and enums live under a catch-all `Types/` folder. That blurs API contracts (`CalculateRebateRequest`/`Result`) with data models (`Rebate`, `Product`, …). A light rename clarifies intent without inventing a fake rich domain or vertical slices.

## Goals

- [x] Move request/result types into `Contracts/`
- [x] Move entities/enums into `Models/`
- [x] Update namespaces and all references; solution builds; all tests pass
- [x] Remove empty `Types/` folder
- [x] Adjust Readme wording if it still says `Types` in a misleading way

## Out of Scope

| Feature | Reason |
| --- | --- |
| Vertical slices / Clean Architecture projects | Previously decided overkill |
| Adding mappers between contracts and models | Shapes already match use case |
| Moving `Incentives/` or inventing Domain entities with behavior | Behavior already in strategies |
| Renaming `RebateApplicationResult` location | Already under Incentives |

---

## Assumptions & Open Questions

| Assumption / decision | Chosen default | Rationale | Confirmed? |
| --------------------- | -------------- | --------- | ---------- |
| Contracts | `CalculateRebateRequest`, `CalculateRebateResult` | Clear API/use-case boundary | y |
| Models | `Rebate`, `Product`, `RebateCalculation`, `IncentiveType`, `SupportedIncentiveType` | Persistence/shared data shapes | y |
| Namespaces | `Smartwyre.DeveloperTest.Contracts` / `.Models` | Match folders | y |
| Behavior | No logic changes | Rename/move only | y |

**Open questions:** none.

---

## User Stories

### P1: Split Types into Contracts and Models ⭐ MVP

**User Story**: As a developer reading the solution, I want DTOs and models in clearly named folders so that contracts vs data shapes are obvious in review.

**Why P1**: Agreed light polish.

**Acceptance Criteria**:

1. The system SHALL place `CalculateRebateRequest` and `CalculateRebateResult` under `Contracts/` with namespace `Smartwyre.DeveloperTest.Contracts`
2. The system SHALL place `Rebate`, `Product`, `RebateCalculation`, `IncentiveType`, and `SupportedIncentiveType` under `Models/` with namespace `Smartwyre.DeveloperTest.Models`
3. WHEN the solution builds THEN all projects SHALL resolve the new namespaces (no remaining `Smartwyre.DeveloperTest.Types` references except historical docs if any)
4. WHEN `dotnet test` runs THEN all tests SHALL pass with unchanged behavior
5. The `Types/` folder SHALL be removed after the move

**Independent Test**: Search for `DeveloperTest.Types` → zero code references; tests green.

---

## Requirement Traceability

| Requirement ID | Story | Phase | Status |
| --- | --- | --- | --- |
| ORG-01 | P1: Contracts folder/namespace | Execute | Verified |
| ORG-02 | P1: Models folder/namespace | Execute | Verified |
| ORG-03 | P1: update all references | Execute | Verified |
| ORG-04 | P1: tests green | Execute | Verified |
| ORG-05 | P1: remove Types/ | Execute | Verified |

---

## Success Criteria

- [x] `Contracts/` + `Models/` exist; `Types/` gone
- [x] Build + all tests green
- [x] No production behavior change
