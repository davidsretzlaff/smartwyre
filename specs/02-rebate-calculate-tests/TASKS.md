# Tasks: rebate-calculate-tests

## Execution Plan

| ID | Task | Depends on | Tests | Gate |
| --- | --- | --- | --- | --- |
| T1 | Add Moq; delete `PaymentService.Tests.cs` | — | CALC-18, CALC-20 | test project restores; no PaymentServiceTests |
| T2 | Add `IRebateDataStore` / `IProductDataStore`; stubs implement them | T1 | — | solution builds Data project |
| T3 | Inject stores into `RebateService`; null rebate/product guards | T2 | CALC-02, CALC-03 | solution builds |
| T4 | Add `RebateServiceTests` for S01–S16 with builders | T3 | CALC-01…CALC-17, CALC-19, CALC-21 | `dotnet test` all pass |
| T5 | Update `specs/STATE.md` + requirement statuses | T4 | — | STATE handoff current |

## Test Coverage Matrix

| Spec AC / Scenario | Test method |
| --- | --- |
| S01 null rebate | `Calculate_WhenRebateIsNull_ReturnsFailureAndDoesNotStore` |
| S02 FixedCash product null | `Calculate_FixedCashAmount_WhenProductIsNull_ReturnsFailureAndDoesNotStore` |
| S03 FixedCash unsupported | `Calculate_FixedCashAmount_WhenIncentiveNotSupported_ReturnsFailureAndDoesNotStore` |
| S04 FixedCash amount 0 | `Calculate_FixedCashAmount_WhenAmountIsZero_ReturnsFailureAndDoesNotStore` |
| S05 FixedCash success | `Calculate_FixedCashAmount_WhenValid_ReturnsSuccessAndStoresAmount` |
| S06 FixedRate product null | `Calculate_FixedRateRebate_WhenProductIsNull_ReturnsFailureAndDoesNotStore` |
| S07 FixedRate unsupported | `Calculate_FixedRateRebate_WhenIncentiveNotSupported_ReturnsFailureAndDoesNotStore` |
| S08 FixedRate percentage 0 | `Calculate_FixedRateRebate_WhenPercentageIsZero_ReturnsFailureAndDoesNotStore` |
| S09 FixedRate price 0 | `Calculate_FixedRateRebate_WhenPriceIsZero_ReturnsFailureAndDoesNotStore` |
| S10 FixedRate volume 0 | `Calculate_FixedRateRebate_WhenVolumeIsZero_ReturnsFailureAndDoesNotStore` |
| S11 FixedRate success | `Calculate_FixedRateRebate_WhenValid_ReturnsSuccessAndStoresCalculatedAmount` |
| S12 AmountPerUom product null | `Calculate_AmountPerUom_WhenProductIsNull_ReturnsFailureAndDoesNotStore` |
| S13 AmountPerUom unsupported | `Calculate_AmountPerUom_WhenIncentiveNotSupported_ReturnsFailureAndDoesNotStore` |
| S14 AmountPerUom amount 0 | `Calculate_AmountPerUom_WhenAmountIsZero_ReturnsFailureAndDoesNotStore` |
| S15 AmountPerUom volume 0 | `Calculate_AmountPerUom_WhenVolumeIsZero_ReturnsFailureAndDoesNotStore` |
| S16 AmountPerUom success | `Calculate_AmountPerUom_WhenValid_ReturnsSuccessAndStoresCalculatedAmount` |

## Gate Check Commands

```bash
dotnet test Smartwyre.DeveloperTest.sln
```

## Note

This stage is tests-only. Production seams/datastore come in a follow-up so the suite can compile and pass.
