using Smartwyre.DeveloperTest.Incentives;
using Smartwyre.DeveloperTest.Tests.DataBuilders;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Incentives;

public class FixedRateRebateTests
{
    [Fact]
    public void Apply_WhenValid_ReturnsSuccessWithCalculatedAmount()
    {
        var product = ProductDataBuilder.AProduct()
            .SupportingFixedRateRebate()
            .WithPrice(50m)
            .Build();
        var incentive = new FixedRateRebate(0.1m);

        var result = incentive.Apply(product, volume: 4m);

        Assert.True(result.Success);
        Assert.Equal(20m, result.Amount);
    }

    [Fact]
    public void Apply_WhenProductIsNull_ReturnsFailure()
    {
        var incentive = new FixedRateRebate(0.1m);

        var result = incentive.Apply(product: null, volume: 1m);

        Assert.False(result.Success);
    }

    [Fact]
    public void Apply_WhenIncentiveNotSupported_ReturnsFailure()
    {
        var product = ProductDataBuilder.AProduct()
            .SupportingFixedCashAmount()
            .WithPrice(100m)
            .Build();
        var incentive = new FixedRateRebate(0.1m);

        var result = incentive.Apply(product, volume: 2m);

        Assert.False(result.Success);
    }

    [Fact]
    public void Apply_WhenPercentageIsZero_ReturnsFailure()
    {
        var product = ProductDataBuilder.AProduct()
            .SupportingFixedRateRebate()
            .WithPrice(100m)
            .Build();
        var incentive = new FixedRateRebate(0m);

        var result = incentive.Apply(product, volume: 2m);

        Assert.False(result.Success);
    }

    [Fact]
    public void Apply_WhenPriceIsZero_ReturnsFailure()
    {
        var product = ProductDataBuilder.AProduct()
            .SupportingFixedRateRebate()
            .WithPrice(0m)
            .Build();
        var incentive = new FixedRateRebate(0.1m);

        var result = incentive.Apply(product, volume: 2m);

        Assert.False(result.Success);
    }

    [Fact]
    public void Apply_WhenVolumeIsZero_ReturnsFailure()
    {
        var product = ProductDataBuilder.AProduct()
            .SupportingFixedRateRebate()
            .WithPrice(100m)
            .Build();
        var incentive = new FixedRateRebate(0.1m);

        var result = incentive.Apply(product, volume: 0m);

        Assert.False(result.Success);
    }
}
