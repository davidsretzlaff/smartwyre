using System.Collections.Generic;
using Smartwyre.DeveloperTest.Contracts;
using Smartwyre.DeveloperTest.Models;

namespace Smartwyre.DeveloperTest.Data.Seed;

public static class SampleData
{
    public const string FixedCashRebateId = "rebate-fixed-cash";
    public const string FixedRateRebateId = "rebate-fixed-rate";
    public const string AmountPerUomRebateId = "rebate-amount-per-uom";

    public const string FixedCashProductId = "product-fixed-cash";
    public const string FixedRateProductId = "product-fixed-rate";
    public const string AmountPerUomProductId = "product-amount-per-uom";

    public static IReadOnlyList<Rebate> Rebates { get; } =
    [
        new Rebate
        {
            Identifier = FixedCashRebateId,
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 50m,
            Percentage = 0m
        },
        new Rebate
        {
            Identifier = FixedRateRebateId,
            Incentive = IncentiveType.FixedRateRebate,
            Amount = 0m,
            Percentage = 0.1m
        },
        new Rebate
        {
            Identifier = AmountPerUomRebateId,
            Incentive = IncentiveType.AmountPerUom,
            Amount = 2.5m,
            Percentage = 0m
        }
    ];

    public static IReadOnlyList<Product> Products { get; } =
    [
        new Product
        {
            Id = 1,
            Identifier = FixedCashProductId,
            Price = 100m,
            Uom = "each",
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount
        },
        new Product
        {
            Id = 2,
            Identifier = FixedRateProductId,
            Price = 80m,
            Uom = "each",
            SupportedIncentives = SupportedIncentiveType.FixedRateRebate
        },
        new Product
        {
            Id = 3,
            Identifier = AmountPerUomProductId,
            Price = 25m,
            Uom = "kg",
            SupportedIncentives = SupportedIncentiveType.AmountPerUom
        }
    ];
}
