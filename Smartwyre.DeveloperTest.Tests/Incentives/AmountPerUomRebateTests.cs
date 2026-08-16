using Smartwyre.DeveloperTest.Incentives;
using Smartwyre.DeveloperTest.Tests.DataBuilders;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Incentives;

public class AmountPerUomRebateTests
{
    [Fact]
    public void Apply_WhenValid_ReturnsSuccessWithCalculatedAmount()
    {
        var product = ProductDataBuilder.AProduct().SupportingAmountPerUom().Build();
        var incentive = new AmountPerUomRebate(3m);

        var result = incentive.Apply(product, volume: 5m);

        Assert.True(result.Success);
        Assert.Equal(15m, result.Amount);
    }

    [Fact]
    public void Apply_WhenProductIsNull_ReturnsFailure()
    {
        var incentive = new AmountPerUomRebate(3m);

        var result = incentive.Apply(product: null, volume: 5m);

        Assert.False(result.Success);
    }

    [Fact]
    public void Apply_WhenIncentiveNotSupported_ReturnsFailure()
    {
        var product = ProductDataBuilder.AProduct().SupportingFixedCashAmount().Build();
        var incentive = new AmountPerUomRebate(3m);

        var result = incentive.Apply(product, volume: 5m);

        Assert.False(result.Success);
    }

    [Fact]
    public void Apply_WhenAmountIsZero_ReturnsFailure()
    {
        var product = ProductDataBuilder.AProduct().SupportingAmountPerUom().Build();
        var incentive = new AmountPerUomRebate(0m);

        var result = incentive.Apply(product, volume: 5m);

        Assert.False(result.Success);
    }

    [Fact]
    public void Apply_WhenVolumeIsZero_ReturnsFailure()
    {
        var product = ProductDataBuilder.AProduct().SupportingAmountPerUom().Build();
        var incentive = new AmountPerUomRebate(3m);

        var result = incentive.Apply(product, volume: 0m);

        Assert.False(result.Success);
    }
}
