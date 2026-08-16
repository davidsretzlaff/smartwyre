# TASKS: Test Data Builders

## Tasks

- [x] T1: Wire test project (Bogus + ProjectReference) — REQ-07
- [x] T2: Add `ProductDataBuilder` — REQ-01, REQ-02, REQ-03, REQ-04, REQ-06, REQ-08
- [x] T3: Add `RebateDataBuilder` — REQ-01, REQ-02, REQ-03, REQ-04, REQ-05, REQ-08
- [x] T4: Add `CalculateRebateRequestDataBuilder` — REQ-01, REQ-02, REQ-03, REQ-04, REQ-08
- [x] T5: Verify solution builds / tests run — Acceptance criteria

## Traceability

| Task | Requirements |
| --- | --- |
| T1 | REQ-07 |
| T2 | REQ-01, REQ-02, REQ-03, REQ-04, REQ-06, REQ-08, REQ-09 |
| T3 | REQ-01, REQ-02, REQ-03, REQ-04, REQ-05, REQ-08, REQ-09 |
| T4 | REQ-01, REQ-02, REQ-03, REQ-04, REQ-08, REQ-09 |
| T5 | Acceptance criteria 1–5 |
| (all) | REQ-10, REQ-11 (no prod changes / no demo tests) |

## Execution notes

- Build: succeeded (0 warnings, 0 errors)
- Tests: run executed; existing `PaymentServiceTests.Test1` still fails with `NotImplementedException` (unchanged, out of scope)
