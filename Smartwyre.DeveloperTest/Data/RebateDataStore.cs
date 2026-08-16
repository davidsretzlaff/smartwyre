using System;
using System.Collections.Generic;
using System.Linq;
using Smartwyre.DeveloperTest.Contracts;
using Smartwyre.DeveloperTest.Models;

namespace Smartwyre.DeveloperTest.Data;

public class RebateDataStore : IRebateDataStore
{
    private readonly Dictionary<string, Rebate> _rebates;
    private readonly List<RebateCalculation> _calculations = new();
    private int _nextCalculationId = 1;

    public RebateDataStore() : this(Enumerable.Empty<Rebate>())
    {
    }

    public RebateDataStore(IEnumerable<Rebate> rebates)
    {
        _rebates = new Dictionary<string, Rebate>(StringComparer.Ordinal);
        foreach (var rebate in rebates)
        {
            if (rebate == null || string.IsNullOrWhiteSpace(rebate.Identifier))
            {
                continue;
            }

            _rebates[rebate.Identifier] = rebate;
        }
    }

    public IReadOnlyList<RebateCalculation> Calculations => _calculations.AsReadOnly();

    public Rebate GetRebate(string rebateIdentifier)
    {
        if (string.IsNullOrWhiteSpace(rebateIdentifier))
        {
            return null;
        }

        return _rebates.TryGetValue(rebateIdentifier, out var rebate) ? rebate : null;
    }

    public void StoreCalculationResult(Rebate rebate, decimal rebateAmount)
    {
        if (rebate == null)
        {
            return;
        }

        _calculations.Add(new RebateCalculation
        {
            Id = _nextCalculationId++,
            Identifier = Guid.NewGuid().ToString("N"),
            RebateIdentifier = rebate.Identifier,
            IncentiveType = rebate.Incentive,
            Amount = rebateAmount
        });
    }
}
