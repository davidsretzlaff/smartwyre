using Smartwyre.DeveloperTest.Contracts;
using Smartwyre.DeveloperTest.Models;

namespace Smartwyre.DeveloperTest.Services;

public interface IRebateService
{
    CalculateRebateResult Calculate(CalculateRebateRequest request);
}
