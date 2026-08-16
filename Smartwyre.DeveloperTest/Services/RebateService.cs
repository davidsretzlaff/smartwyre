using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Incentives;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;
    private readonly IRebateIncentiveFactory _incentiveFactory;

    public RebateService(
        IRebateDataStore rebateDataStore,
        IProductDataStore productDataStore,
        IRebateIncentiveFactory incentiveFactory)
    {
        _rebateDataStore = rebateDataStore;
        _productDataStore = productDataStore;
        _incentiveFactory = incentiveFactory;
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        Rebate rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        Product product = _productDataStore.GetProduct(request.ProductIdentifier);

        var result = new CalculateRebateResult();

        if (rebate == null)
        {
            result.Success = false;
            return result;
        }

        IRebateIncentive incentive = _incentiveFactory.Create(rebate);
        if (incentive == null)
        {
            result.Success = false;
            return result;
        }

        RebateApplicationResult applied = incentive.Apply(product, request.Volume);
        result.Success = applied.Success;

        if (result.Success)
        {
            _rebateDataStore.StoreCalculationResult(rebate, applied.Amount);
        }

        return result;
    }
}
