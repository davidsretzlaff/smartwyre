using Smartwyre.DeveloperTest.Models;

namespace Smartwyre.DeveloperTest.Contracts;

public class CalculateRebateResult
{
    public bool Success { get; set; }

    public string RebateIdentifier { get; set; }

    public string ProductIdentifier { get; set; }

    /// <summary>Incentive / discount type applied.</summary>
    public IncentiveType? IncentiveType { get; set; }

    /// <summary>Product unit price.</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Request volume.</summary>
    public decimal Volume { get; set; }

    /// <summary>Subtotal before rebate: unit price × volume.</summary>
    public decimal AmountBeforeDiscount { get; set; }

    /// <summary>Rebate / discount amount.</summary>
    public decimal RebateAmount { get; set; }

    /// <summary>Subtotal after rebate (not below zero).</summary>
    public decimal AmountAfterDiscount { get; set; }
}
