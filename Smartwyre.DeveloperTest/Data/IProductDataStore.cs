using Smartwyre.DeveloperTest.Contracts;
using Smartwyre.DeveloperTest.Models;

namespace Smartwyre.DeveloperTest.Data;

public interface IProductDataStore
{
    Product GetProduct(string productIdentifier);
}
