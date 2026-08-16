using Smartwyre.DeveloperTest.Contracts;
using Smartwyre.DeveloperTest.Models;

namespace Smartwyre.DeveloperTest.Data;

public interface IRebateDataStore
{
    Rebate GetRebate(string rebateIdentifier);

    void StoreCalculationResult(Rebate rebate, decimal rebateAmount);
}
