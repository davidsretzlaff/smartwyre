using System.Linq;
using Smartwyre.DeveloperTest.Data.Seed;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Data;

public class DataSeederTests
{
    [Fact]
    public void CreateStores_ResolvesAllSampleIdentifiers()
    {
        var (rebates, products) = DataSeeder.CreateStores();

        foreach (var rebate in SampleData.Rebates)
        {
            Assert.NotNull(rebates.GetRebate(rebate.Identifier));
        }

        foreach (var product in SampleData.Products)
        {
            Assert.NotNull(products.GetProduct(product.Identifier));
        }

        Assert.Equal(SampleData.Rebates.Count, SampleData.Rebates.Select(r => r.Identifier).Distinct().Count());
        Assert.Equal(SampleData.Products.Count, SampleData.Products.Select(p => p.Identifier).Distinct().Count());
    }
}
