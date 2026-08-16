using Smartwyre.DeveloperTest.Incentives;
using Smartwyre.DeveloperTest.Tests.DataBuilders;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Incentives;

public class RebateIncentiveFactoryTests
{
    private readonly RebateIncentiveFactory _factory = new();

    [Fact]
    public void Create_WhenFixedCash_ReturnsStrategyThatAppliesAmount()
    {
        var rebate = RebateDataBuilder.ARebate().AsFixedCashAmount(40m).Build();
        var product = ProductDataBuilder.AProduct().SupportingFixedCashAmount().Build();

        var incentive = _factory.Create(rebate);

        Assert.IsType<FixedCashAmountRebate>(incentive);
        var result = incentive.Apply(product, volume: 1m);
        Assert.True(result.Success);
        Assert.Equal(40m, result.Amount);
    }

    [Fact]
    public void Create_WhenFixedRate_ReturnsStrategyThatAppliesPercentage()
    {
        var rebate = RebateDataBuilder.ARebate().AsFixedRateRebate(0.1m).Build();
        var product = ProductDataBuilder.AProduct()
            .SupportingFixedRateRebate()
            .WithPrice(50m)
            .Build();

        var incentive = _factory.Create(rebate);

        Assert.IsType<FixedRateRebate>(incentive);
        var result = incentive.Apply(product, volume: 4m);
        Assert.True(result.Success);
        Assert.Equal(20m, result.Amount);
    }

    [Fact]
    public void Create_WhenAmountPerUom_ReturnsStrategyThatAppliesAmount()
    {
        var rebate = RebateDataBuilder.ARebate().AsAmountPerUom(3m).Build();
        var product = ProductDataBuilder.AProduct().SupportingAmountPerUom().Build();

        var incentive = _factory.Create(rebate);

        Assert.IsType<AmountPerUomRebate>(incentive);
        var result = incentive.Apply(product, volume: 5m);
        Assert.True(result.Success);
        Assert.Equal(15m, result.Amount);
    }

    [Fact]
    public void Create_WhenRebateIsNull_ReturnsNull()
    {
        var incentive = _factory.Create(rebate: null);

        Assert.Null(incentive);
    }
}
