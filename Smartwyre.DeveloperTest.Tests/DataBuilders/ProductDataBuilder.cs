using Bogus;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Tests.DataBuilders;

public class ProductDataBuilder
{
    private int _id;
    private string _identifier;
    private decimal _price;
    private string _uom;
    private SupportedIncentiveType _supportedIncentives;

    public ProductDataBuilder()
    {
        var faker = new Faker("pt_BR");
        _id = faker.Random.Int(1, 10_000);
        _identifier = faker.Commerce.ProductName();
        _price = faker.Random.Decimal(1, 1000);
        _uom = faker.PickRandom("each", "kg", "liter", "box");
        _supportedIncentives = SupportedIncentiveType.FixedRateRebate;
    }

    public static ProductDataBuilder AProduct() => new();

    public ProductDataBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public ProductDataBuilder WithIdentifier(string identifier)
    {
        _identifier = identifier;
        return this;
    }

    public ProductDataBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public ProductDataBuilder WithUom(string uom)
    {
        _uom = uom;
        return this;
    }

    public ProductDataBuilder WithSupportedIncentives(SupportedIncentiveType supportedIncentives)
    {
        _supportedIncentives = supportedIncentives;
        return this;
    }

    public ProductDataBuilder SupportingFixedRateRebate()
    {
        _supportedIncentives = SupportedIncentiveType.FixedRateRebate;
        return this;
    }

    public ProductDataBuilder SupportingAmountPerUom()
    {
        _supportedIncentives = SupportedIncentiveType.AmountPerUom;
        return this;
    }

    public ProductDataBuilder SupportingFixedCashAmount()
    {
        _supportedIncentives = SupportedIncentiveType.FixedCashAmount;
        return this;
    }

    public ProductDataBuilder SupportingAllIncentives()
    {
        _supportedIncentives =
            SupportedIncentiveType.FixedRateRebate
            | SupportedIncentiveType.AmountPerUom
            | SupportedIncentiveType.FixedCashAmount;
        return this;
    }

    public Product Build()
    {
        return new Product
        {
            Id = _id,
            Identifier = _identifier,
            Price = _price,
            Uom = _uom,
            SupportedIncentives = _supportedIncentives
        };
    }
}
