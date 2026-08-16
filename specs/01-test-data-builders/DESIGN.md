# DESIGN: Test Data Builders

## Approach

Mirror FC4 `DataBuilders`: one fluent builder per target type, Bogus defaults in the constructor, `With*` overrides, static entry factories, `Build()` returning a mutable anemic object via object initializer / property assignment.

## Structure

```
Smartwyre.DeveloperTest.Tests/
  DataBuilders/
    ProductDataBuilder.cs
    RebateDataBuilder.cs
    CalculateRebateRequestDataBuilder.cs
  Smartwyre.DeveloperTest.Tests.csproj  (+ Bogus, ProjectReference)
```

## Builder API

### ProductDataBuilder
- Defaults: Id, Identifier, Price, Uom, SupportedIncentives (random flag subset or FixedRateRebate)
- `AProduct()`
- `WithId`, `WithIdentifier`, `WithPrice`, `WithUom`, `WithSupportedIncentives`
- Scenario helpers: `SupportingFixedRateRebate()`, `SupportingAmountPerUom()`, `SupportingFixedCashAmount()`, `SupportingAllIncentives()`

### RebateDataBuilder
- Defaults: Identifier, Incentive, Amount, Percentage
- `ARebate()`
- `WithIdentifier`, `WithIncentive`, `WithAmount`, `WithPercentage`
- Scenario helpers: `AsFixedCashAmount()`, `AsFixedRateRebate()`, `AsAmountPerUom()`

### CalculateRebateRequestDataBuilder
- Defaults: RebateIdentifier, ProductIdentifier, Volume
- `ACalculateRebateRequest()` / `ARebateRequest()`
- `WithRebateIdentifier`, `WithProductIdentifier`, `WithVolume`
- Optional compose: `ForRebate(Rebate)`, `ForProduct(Product)` using identifiers

## Dependencies

- Package: Bogus (aligned with FC4 ~35.x)
- ProjectReference: `..\Smartwyre.DeveloperTest\Smartwyre.DeveloperTest.csproj`

## Risks

- Random Bogus values can make flaky tests if assertions depend on defaults — callers must `With*` values under test. Documented by design; no seeded faker required for this scope.
