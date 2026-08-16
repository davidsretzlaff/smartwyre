using Moq;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Incentives;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Tests.DataBuilders;
using Smartwyre.DeveloperTest.Contracts;
using Smartwyre.DeveloperTest.Models;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Services;

public class RebateServiceTests
{
    private readonly Mock<IRebateDataStore> _rebateDataStore = new();
    private readonly Mock<IProductDataStore> _productDataStore = new();

    private RebateService CreateSut() =>
        new(_rebateDataStore.Object, _productDataStore.Object, new RebateIncentiveFactory());

    private void SetupStores(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        _rebateDataStore
            .Setup(store => store.GetRebate(request.RebateIdentifier))
            .Returns(rebate);

        _productDataStore
            .Setup(store => store.GetProduct(request.ProductIdentifier))
            .Returns(product);
    }

    private void VerifyStoreNotCalled()
    {
        _rebateDataStore.Verify(
            store => store.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()),
            Times.Never);
    }

    [Fact]
    public void Calculate_WhenRebateIsNull_ReturnsFailureAndDoesNotStore()
    {
        var request = CalculateRebateRequestDataBuilder.ARebateRequest().Build();
        var product = ProductDataBuilder.AProduct().SupportingFixedCashAmount().Build();

        SetupStores(rebate: null, product, request);

        var result = CreateSut().Calculate(request);

        Assert.False(result.Success);
        VerifyStoreNotCalled();
    }

    [Fact]
    public void Calculate_FixedCashAmount_WhenProductIsNull_ReturnsFailureAndDoesNotStore()
    {
        var rebate = RebateDataBuilder.ARebate().AsFixedCashAmount(25m).Build();
        var request = CalculateRebateRequestDataBuilder.ARebateRequest()
            .ForRebate(rebate)
            .Build();

        SetupStores(rebate, product: null, request);

        var result = CreateSut().Calculate(request);

        Assert.False(result.Success);
        VerifyStoreNotCalled();
    }

    [Fact]
    public void Calculate_FixedCashAmount_WhenIncentiveNotSupported_ReturnsFailureAndDoesNotStore()
    {
        var rebate = RebateDataBuilder.ARebate().AsFixedCashAmount(25m).Build();
        var product = ProductDataBuilder.AProduct().SupportingFixedRateRebate().Build();
        var request = CalculateRebateRequestDataBuilder.ARebateRequest()
            .ForRebate(rebate)
            .ForProduct(product)
            .Build();

        SetupStores(rebate, product, request);

        var result = CreateSut().Calculate(request);

        Assert.False(result.Success);
        VerifyStoreNotCalled();
    }

    [Fact]
    public void Calculate_FixedCashAmount_WhenAmountIsZero_ReturnsFailureAndDoesNotStore()
    {
        var rebate = RebateDataBuilder.ARebate().AsFixedCashAmount(0m).Build();
        var product = ProductDataBuilder.AProduct().SupportingFixedCashAmount().Build();
        var request = CalculateRebateRequestDataBuilder.ARebateRequest()
            .ForRebate(rebate)
            .ForProduct(product)
            .Build();

        SetupStores(rebate, product, request);

        var result = CreateSut().Calculate(request);

        Assert.False(result.Success);
        VerifyStoreNotCalled();
    }

    [Fact]
    public void Calculate_FixedCashAmount_WhenValid_ReturnsSuccessAndStoresAmount()
    {
        var rebate = RebateDataBuilder.ARebate().AsFixedCashAmount(40m).Build();
        var product = ProductDataBuilder.AProduct().SupportingFixedCashAmount().Build();
        var request = CalculateRebateRequestDataBuilder.ARebateRequest()
            .ForRebate(rebate)
            .ForProduct(product)
            .WithVolume(5m)
            .Build();

        SetupStores(rebate, product, request);

        var result = CreateSut().Calculate(request);

        Assert.True(result.Success);
        _rebateDataStore.Verify(
            store => store.StoreCalculationResult(rebate, 40m),
            Times.Once);
    }

    [Fact]
    public void Calculate_FixedRateRebate_WhenProductIsNull_ReturnsFailureAndDoesNotStore()
    {
        var rebate = RebateDataBuilder.ARebate().AsFixedRateRebate(0.1m).Build();
        var request = CalculateRebateRequestDataBuilder.ARebateRequest()
            .ForRebate(rebate)
            .Build();

        SetupStores(rebate, product: null, request);

        var result = CreateSut().Calculate(request);

        Assert.False(result.Success);
        VerifyStoreNotCalled();
    }

    [Fact]
    public void Calculate_FixedRateRebate_WhenIncentiveNotSupported_ReturnsFailureAndDoesNotStore()
    {
        var rebate = RebateDataBuilder.ARebate().AsFixedRateRebate(0.1m).Build();
        var product = ProductDataBuilder.AProduct()
            .SupportingFixedCashAmount()
            .WithPrice(100m)
            .Build();
        var request = CalculateRebateRequestDataBuilder.ARebateRequest()
            .ForRebate(rebate)
            .ForProduct(product)
            .WithVolume(2m)
            .Build();

        SetupStores(rebate, product, request);

        var result = CreateSut().Calculate(request);

        Assert.False(result.Success);
        VerifyStoreNotCalled();
    }

    [Fact]
    public void Calculate_FixedRateRebate_WhenPercentageIsZero_ReturnsFailureAndDoesNotStore()
    {
        var rebate = RebateDataBuilder.ARebate().AsFixedRateRebate(0m).Build();
        var product = ProductDataBuilder.AProduct()
            .SupportingFixedRateRebate()
            .WithPrice(100m)
            .Build();
        var request = CalculateRebateRequestDataBuilder.ARebateRequest()
            .ForRebate(rebate)
            .ForProduct(product)
            .WithVolume(2m)
            .Build();

        SetupStores(rebate, product, request);

        var result = CreateSut().Calculate(request);

        Assert.False(result.Success);
        VerifyStoreNotCalled();
    }

    [Fact]
    public void Calculate_FixedRateRebate_WhenPriceIsZero_ReturnsFailureAndDoesNotStore()
    {
        var rebate = RebateDataBuilder.ARebate().AsFixedRateRebate(0.1m).Build();
        var product = ProductDataBuilder.AProduct()
            .SupportingFixedRateRebate()
            .WithPrice(0m)
            .Build();
        var request = CalculateRebateRequestDataBuilder.ARebateRequest()
            .ForRebate(rebate)
            .ForProduct(product)
            .WithVolume(2m)
            .Build();

        SetupStores(rebate, product, request);

        var result = CreateSut().Calculate(request);

        Assert.False(result.Success);
        VerifyStoreNotCalled();
    }

    [Fact]
    public void Calculate_FixedRateRebate_WhenVolumeIsZero_ReturnsFailureAndDoesNotStore()
    {
        var rebate = RebateDataBuilder.ARebate().AsFixedRateRebate(0.1m).Build();
        var product = ProductDataBuilder.AProduct()
            .SupportingFixedRateRebate()
            .WithPrice(100m)
            .Build();
        var request = CalculateRebateRequestDataBuilder.ARebateRequest()
            .ForRebate(rebate)
            .ForProduct(product)
            .WithVolume(0m)
            .Build();

        SetupStores(rebate, product, request);

        var result = CreateSut().Calculate(request);

        Assert.False(result.Success);
        VerifyStoreNotCalled();
    }

    [Fact]
    public void Calculate_FixedRateRebate_WhenValid_ReturnsSuccessAndStoresCalculatedAmount()
    {
        var rebate = RebateDataBuilder.ARebate().AsFixedRateRebate(0.1m).Build();
        var product = ProductDataBuilder.AProduct()
            .SupportingFixedRateRebate()
            .WithPrice(50m)
            .Build();
        var request = CalculateRebateRequestDataBuilder.ARebateRequest()
            .ForRebate(rebate)
            .ForProduct(product)
            .WithVolume(4m)
            .Build();

        SetupStores(rebate, product, request);

        var result = CreateSut().Calculate(request);

        Assert.True(result.Success);
        _rebateDataStore.Verify(
            store => store.StoreCalculationResult(rebate, 20m),
            Times.Once);
    }

    [Fact]
    public void Calculate_AmountPerUom_WhenProductIsNull_ReturnsFailureAndDoesNotStore()
    {
        var rebate = RebateDataBuilder.ARebate().AsAmountPerUom(3m).Build();
        var request = CalculateRebateRequestDataBuilder.ARebateRequest()
            .ForRebate(rebate)
            .Build();

        SetupStores(rebate, product: null, request);

        var result = CreateSut().Calculate(request);

        Assert.False(result.Success);
        VerifyStoreNotCalled();
    }

    [Fact]
    public void Calculate_AmountPerUom_WhenIncentiveNotSupported_ReturnsFailureAndDoesNotStore()
    {
        var rebate = RebateDataBuilder.ARebate().AsAmountPerUom(3m).Build();
        var product = ProductDataBuilder.AProduct().SupportingFixedCashAmount().Build();
        var request = CalculateRebateRequestDataBuilder.ARebateRequest()
            .ForRebate(rebate)
            .ForProduct(product)
            .WithVolume(5m)
            .Build();

        SetupStores(rebate, product, request);

        var result = CreateSut().Calculate(request);

        Assert.False(result.Success);
        VerifyStoreNotCalled();
    }

    [Fact]
    public void Calculate_AmountPerUom_WhenAmountIsZero_ReturnsFailureAndDoesNotStore()
    {
        var rebate = RebateDataBuilder.ARebate().AsAmountPerUom(0m).Build();
        var product = ProductDataBuilder.AProduct().SupportingAmountPerUom().Build();
        var request = CalculateRebateRequestDataBuilder.ARebateRequest()
            .ForRebate(rebate)
            .ForProduct(product)
            .WithVolume(5m)
            .Build();

        SetupStores(rebate, product, request);

        var result = CreateSut().Calculate(request);

        Assert.False(result.Success);
        VerifyStoreNotCalled();
    }

    [Fact]
    public void Calculate_AmountPerUom_WhenVolumeIsZero_ReturnsFailureAndDoesNotStore()
    {
        var rebate = RebateDataBuilder.ARebate().AsAmountPerUom(3m).Build();
        var product = ProductDataBuilder.AProduct().SupportingAmountPerUom().Build();
        var request = CalculateRebateRequestDataBuilder.ARebateRequest()
            .ForRebate(rebate)
            .ForProduct(product)
            .WithVolume(0m)
            .Build();

        SetupStores(rebate, product, request);

        var result = CreateSut().Calculate(request);

        Assert.False(result.Success);
        VerifyStoreNotCalled();
    }

    [Fact]
    public void Calculate_AmountPerUom_WhenValid_ReturnsSuccessAndStoresCalculatedAmount()
    {
        var rebate = RebateDataBuilder.ARebate().AsAmountPerUom(3m).Build();
        var product = ProductDataBuilder.AProduct().SupportingAmountPerUom().Build();
        var request = CalculateRebateRequestDataBuilder.ARebateRequest()
            .ForRebate(rebate)
            .ForProduct(product)
            .WithVolume(5m)
            .Build();

        SetupStores(rebate, product, request);

        var result = CreateSut().Calculate(request);

        Assert.True(result.Success);
        _rebateDataStore.Verify(
            store => store.StoreCalculationResult(rebate, 15m),
            Times.Once);
    }
}
