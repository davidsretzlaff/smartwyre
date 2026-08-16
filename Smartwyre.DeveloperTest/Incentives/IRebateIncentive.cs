using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Incentives;

public interface IRebateIncentive
{
    RebateApplicationResult Apply(Product product, decimal volume);
}
