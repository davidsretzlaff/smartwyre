using Smartwyre.DeveloperTest.Contracts;
using Smartwyre.DeveloperTest.Models;

namespace Smartwyre.DeveloperTest.Incentives;

public interface IRebateIncentive
{
    RebateApplicationResult Apply(Product product, decimal volume);
}
