using Smartwyre.DeveloperTest.Contracts;
using Smartwyre.DeveloperTest.Models;

namespace Smartwyre.DeveloperTest.Incentives;

public interface IRebateIncentiveFactory
{
    /// <summary>
    /// Creates the incentive strategy for the given rebate DTO.
    /// To add a new incentive type: implement <see cref="IRebateIncentive"/> and register it here.
    /// </summary>
    IRebateIncentive Create(Rebate rebate);
}
