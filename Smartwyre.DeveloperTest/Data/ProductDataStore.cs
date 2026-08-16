using System;
using System.Collections.Generic;
using System.Linq;
using Smartwyre.DeveloperTest.Contracts;
using Smartwyre.DeveloperTest.Models;

namespace Smartwyre.DeveloperTest.Data;

public class ProductDataStore : IProductDataStore
{
    private readonly Dictionary<string, Product> _products;

    public ProductDataStore() : this(Enumerable.Empty<Product>())
    {
    }

    public ProductDataStore(IEnumerable<Product> products)
    {
        _products = new Dictionary<string, Product>(StringComparer.Ordinal);
        foreach (var product in products)
        {
            if (product == null || string.IsNullOrWhiteSpace(product.Identifier))
            {
                continue;
            }

            _products[product.Identifier] = product;
        }
    }

    public Product GetProduct(string productIdentifier)
    {
        if (string.IsNullOrWhiteSpace(productIdentifier))
        {
            return null;
        }

        return _products.TryGetValue(productIdentifier, out var product) ? product : null;
    }
}
