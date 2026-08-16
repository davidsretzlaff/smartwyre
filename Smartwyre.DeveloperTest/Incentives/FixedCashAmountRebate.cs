using Smartwyre.DeveloperTest.Contracts;
using Smartwyre.DeveloperTest.Models;

namespace Smartwyre.DeveloperTest.Incentives;

public sealed class FixedCashAmountRebate : IRebateIncentive
{
    private readonly decimal _amount;

    public FixedCashAmountRebate(decimal amount)
    {
        _amount = amount;
    }

    public RebateApplicationResult Apply(Product product, decimal volume)
    {
        if (product == null)
        {
            return RebateApplicationResult.Failed();
        }

        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount))
        {
            return RebateApplicationResult.Failed();
        }

        if (_amount == 0)
        {
            return RebateApplicationResult.Failed();
        }

        return RebateApplicationResult.Succeeded(_amount);
    }
}
