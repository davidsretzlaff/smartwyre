using Bogus;
using Smartwyre.DeveloperTest.Contracts;
using Smartwyre.DeveloperTest.Models;

namespace Smartwyre.DeveloperTest.Tests.DataBuilders;

public class RebateDataBuilder
{
    private string _identifier;
    private IncentiveType _incentive;
    private decimal _amount;
    private decimal _percentage;

    public RebateDataBuilder()
    {
        var faker = new Faker("pt_BR");
        _identifier = faker.Random.Guid().ToString();
        _incentive = faker.PickRandom<IncentiveType>();
        _amount = faker.Random.Decimal(1, 100);
        _percentage = faker.Random.Decimal(0.01m, 0.5m);
    }

    public static RebateDataBuilder ARebate() => new();

    public RebateDataBuilder WithIdentifier(string identifier)
    {
        _identifier = identifier;
        return this;
    }

    public RebateDataBuilder WithIncentive(IncentiveType incentive)
    {
        _incentive = incentive;
        return this;
    }

    public RebateDataBuilder WithAmount(decimal amount)
    {
        _amount = amount;
        return this;
    }

    public RebateDataBuilder WithPercentage(decimal percentage)
    {
        _percentage = percentage;
        return this;
    }

    public RebateDataBuilder AsFixedCashAmount(decimal? amount = null)
    {
        _incentive = IncentiveType.FixedCashAmount;
        if (amount.HasValue)
        {
            _amount = amount.Value;
        }
        return this;
    }

    public RebateDataBuilder AsFixedRateRebate(decimal? percentage = null)
    {
        _incentive = IncentiveType.FixedRateRebate;
        if (percentage.HasValue)
        {
            _percentage = percentage.Value;
        }
        return this;
    }

    public RebateDataBuilder AsAmountPerUom(decimal? amount = null)
    {
        _incentive = IncentiveType.AmountPerUom;
        if (amount.HasValue)
        {
            _amount = amount.Value;
        }
        return this;
    }

    public Rebate Build()
    {
        return new Rebate
        {
            Identifier = _identifier,
            Incentive = _incentive,
            Amount = _amount,
            Percentage = _percentage
        };
    }
}
