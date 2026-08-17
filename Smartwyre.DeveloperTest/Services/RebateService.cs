using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Incentives;
using Smartwyre.DeveloperTest.Contracts;
using Smartwyre.DeveloperTest.Mappings;
using Smartwyre.DeveloperTest.Models;

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

        if (rebate == null)
        {
            return CalculateRebateResultMapper.ToFailure(request);
        }

        IRebateIncentive incentive = _incentiveFactory.Create(rebate);
        if (incentive == null)
        {
            return CalculateRebateResultMapper.ToFailure(request, rebate);
        }

        RebateApplicationResult applied = incentive.Apply(product, request.Volume);
        if (!applied.Success)
        {
            return CalculateRebateResultMapper.ToFailure(request, rebate);
        }

        _rebateDataStore.StoreCalculationResult(rebate, applied.Amount);

        return CalculateRebateResultMapper.ToSuccess(request, rebate, product, applied.Amount);
    }
}
