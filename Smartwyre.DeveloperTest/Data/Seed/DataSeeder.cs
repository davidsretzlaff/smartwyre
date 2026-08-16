namespace Smartwyre.DeveloperTest.Data.Seed;

public static class DataSeeder
{
    public static (RebateDataStore Rebates, ProductDataStore Products) CreateStores()
    {
        var rebates = new RebateDataStore(SampleData.Rebates);
        var products = new ProductDataStore(SampleData.Products);
        return (rebates, products);
    }
}
