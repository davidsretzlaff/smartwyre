using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Tests.DataBuilders;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Data;

public class ProductDataStoreTests
{
    [Fact]
    public void GetProduct_WhenKnownIdentifier_ReturnsProduct()
    {
        var product = ProductDataBuilder.AProduct()
            .WithIdentifier("product-1")
            .SupportingFixedCashAmount()
            .Build();
        var store = new ProductDataStore([product]);

        var found = store.GetProduct("product-1");

        Assert.Same(product, found);
    }

    [Fact]
    public void GetProduct_WhenUnknownIdentifier_ReturnsNull()
    {
        var store = new ProductDataStore();

        var found = store.GetProduct("missing");

        Assert.Null(found);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetProduct_WhenIdentifierMissing_ReturnsNull(string identifier)
    {
        var store = new ProductDataStore();

        var found = store.GetProduct(identifier);

        Assert.Null(found);
    }
}
