using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Tests.DataBuilders;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Data;

public class RebateDataStoreTests
{
    [Fact]
    public void GetRebate_WhenKnownIdentifier_ReturnsRebate()
    {
        var rebate = RebateDataBuilder.ARebate()
            .WithIdentifier("rebate-1")
            .AsFixedCashAmount(10m)
            .Build();
        var store = new RebateDataStore([rebate]);

        var found = store.GetRebate("rebate-1");

        Assert.Same(rebate, found);
    }

    [Fact]
    public void GetRebate_WhenUnknownIdentifier_ReturnsNull()
    {
        var store = new RebateDataStore();

        var found = store.GetRebate("missing");

        Assert.Null(found);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetRebate_WhenIdentifierMissing_ReturnsNull(string identifier)
    {
        var store = new RebateDataStore();

        var found = store.GetRebate(identifier);

        Assert.Null(found);
    }

    [Fact]
    public void StoreCalculationResult_WhenRebateProvided_RetainsCalculation()
    {
        var rebate = RebateDataBuilder.ARebate()
            .WithIdentifier("rebate-1")
            .AsFixedCashAmount(25m)
            .Build();
        var store = new RebateDataStore([rebate]);

        store.StoreCalculationResult(rebate, 25m);

        Assert.Single(store.Calculations);
        var calculation = store.Calculations[0];
        Assert.Equal("rebate-1", calculation.RebateIdentifier);
        Assert.Equal(IncentiveType.FixedCashAmount, calculation.IncentiveType);
        Assert.Equal(25m, calculation.Amount);
    }

    [Fact]
    public void StoreCalculationResult_WhenRebateIsNull_DoesNotAddCalculation()
    {
        var store = new RebateDataStore();

        store.StoreCalculationResult(rebate: null, rebateAmount: 10m);

        Assert.Empty(store.Calculations);
    }
}
