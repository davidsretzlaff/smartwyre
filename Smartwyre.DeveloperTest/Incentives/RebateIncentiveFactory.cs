using Smartwyre.DeveloperTest.Contracts;
using Smartwyre.DeveloperTest.Models;

namespace Smartwyre.DeveloperTest.Incentives;

public sealed class RebateIncentiveFactory : IRebateIncentiveFactory
{
    /// <summary>
    /// Maps <see cref="IncentiveType"/> to a strategy.
    /// Extension point: add a new <see cref="IRebateIncentive"/> implementation and a case below.
    /// </summary>
    public IRebateIncentive Create(Rebate rebate)
    {
        if (rebate == null)
        {
            return null;
        }

        return rebate.Incentive switch
        {
            IncentiveType.FixedCashAmount => new FixedCashAmountRebate(rebate.Amount),
            IncentiveType.FixedRateRebate => new FixedRateRebate(rebate.Percentage),
            IncentiveType.AmountPerUom => new AmountPerUomRebate(rebate.Amount),
            _ => null
        };
    }
}
