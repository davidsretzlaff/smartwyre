using Smartwyre.DeveloperTest.Incentives;
using Smartwyre.DeveloperTest.Tests.DataBuilders;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Incentives;

public class FixedCashAmountRebateTests
{
    [Fact]
    public void Apply_WhenValid_ReturnsSuccessWithAmount()
    {
        var product = ProductDataBuilder.AProduct().SupportingFixedCashAmount().Build();
        var incentive = new FixedCashAmountRebate(40m);

        var result = incentive.Apply(product, volume: 0m);

        Assert.True(result.Success);
        Assert.Equal(40m, result.Amount);
    }

    [Fact]
    public void Apply_WhenProductIsNull_ReturnsFailure()
    {
        var incentive = new FixedCashAmountRebate(40m);

        var result = incentive.Apply(product: null, volume: 1m);

        Assert.False(result.Success);
    }

    [Fact]
    public void Apply_WhenIncentiveNotSupported_ReturnsFailure()
    {
        var product = ProductDataBuilder.AProduct().SupportingFixedRateRebate().Build();
        var incentive = new FixedCashAmountRebate(40m);

        var result = incentive.Apply(product, volume: 1m);

        Assert.False(result.Success);
    }

    [Fact]
    public void Apply_WhenAmountIsZero_ReturnsFailure()
    {
        var product = ProductDataBuilder.AProduct().SupportingFixedCashAmount().Build();
        var incentive = new FixedCashAmountRebate(0m);

        var result = incentive.Apply(product, volume: 1m);

        Assert.False(result.Success);
    }
}
