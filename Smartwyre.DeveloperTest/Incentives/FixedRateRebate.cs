using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Incentives;

public sealed class FixedRateRebate : IRebateIncentive
{
    private readonly decimal _percentage;

    public FixedRateRebate(decimal percentage)
    {
        _percentage = percentage;
    }

    public RebateApplicationResult Apply(Product product, decimal volume)
    {
        if (product == null)
        {
            return RebateApplicationResult.Failed();
        }

        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate))
        {
            return RebateApplicationResult.Failed();
        }

        if (_percentage == 0 || product.Price == 0 || volume == 0)
        {
            return RebateApplicationResult.Failed();
        }

        return RebateApplicationResult.Succeeded(product.Price * _percentage * volume);
    }
}
