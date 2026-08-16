using Bogus;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Tests.DataBuilders;

public class CalculateRebateRequestDataBuilder
{
    private string _rebateIdentifier;
    private string _productIdentifier;
    private decimal _volume;

    public CalculateRebateRequestDataBuilder()
    {
        var faker = new Faker("pt_BR");
        _rebateIdentifier = faker.Random.Guid().ToString();
        _productIdentifier = faker.Commerce.ProductName();
        _volume = faker.Random.Decimal(1, 100);
    }

    public static CalculateRebateRequestDataBuilder ACalculateRebateRequest() => new();

    public static CalculateRebateRequestDataBuilder ARebateRequest() => new();

    public CalculateRebateRequestDataBuilder WithRebateIdentifier(string rebateIdentifier)
    {
        _rebateIdentifier = rebateIdentifier;
        return this;
    }

    public CalculateRebateRequestDataBuilder WithProductIdentifier(string productIdentifier)
    {
        _productIdentifier = productIdentifier;
        return this;
    }

    public CalculateRebateRequestDataBuilder WithVolume(decimal volume)
    {
        _volume = volume;
        return this;
    }

    public CalculateRebateRequestDataBuilder ForRebate(Rebate rebate)
    {
        _rebateIdentifier = rebate.Identifier;
        return this;
    }

    public CalculateRebateRequestDataBuilder ForProduct(Product product)
    {
        _productIdentifier = product.Identifier;
        return this;
    }

    public CalculateRebateRequest Build()
    {
        return new CalculateRebateRequest
        {
            RebateIdentifier = _rebateIdentifier,
            ProductIdentifier = _productIdentifier,
            Volume = _volume
        };
    }
}
