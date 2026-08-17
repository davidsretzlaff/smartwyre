using System;
using Smartwyre.DeveloperTest.Contracts;
using Smartwyre.DeveloperTest.Models;

namespace Smartwyre.DeveloperTest.Mappings;

public static class CalculateRebateResultMapper
{
    public static CalculateRebateResult ToFailure(CalculateRebateRequest request, Rebate rebate = null)
    {
        return new CalculateRebateResult
        {
            Success = false,
            RebateIdentifier = request?.RebateIdentifier,
            ProductIdentifier = request?.ProductIdentifier,
            IncentiveType = rebate?.Incentive,
            Volume = request?.Volume ?? 0m
        };
    }

    public static CalculateRebateResult ToSuccess(
        CalculateRebateRequest request,
        Rebate rebate,
        Product product,
        decimal rebateAmount)
    {
        var unitPrice = product.Price;
        var volume = request.Volume;
        var amountBeforeDiscount = unitPrice * volume;

        return new CalculateRebateResult
        {
            Success = true,
            RebateIdentifier = request.RebateIdentifier,
            ProductIdentifier = request.ProductIdentifier,
            IncentiveType = rebate.Incentive,
            UnitPrice = unitPrice,
            Volume = volume,
            AmountBeforeDiscount = amountBeforeDiscount,
            RebateAmount = rebateAmount,
            AmountAfterDiscount = Math.Max(0m, amountBeforeDiscount - rebateAmount)
        };
    }
}
