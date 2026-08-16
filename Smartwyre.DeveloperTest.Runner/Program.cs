using System;
using System.Collections.Generic;
using Smartwyre.DeveloperTest.Data.Seed;
using Smartwyre.DeveloperTest.Incentives;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Contracts;
using Smartwyre.DeveloperTest.Models;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        var (rebateStore, productStore) = DataSeeder.CreateStores();
        var rebateService = new RebateService(rebateStore, productStore, new RebateIncentiveFactory());

        WriteSampleData();

        var rebateId = args.Length > 0 ? args[0] : Read("Rebate identifier");
        var productId = args.Length > 1 ? args[1] : Read("Product identifier");
        var volumeText = args.Length > 2 ? args[2] : Read("Volume");

        if (!TryValidate(rebateId, productId, volumeText, out var volume, out var errors))
        {
            WriteValidationErrors(errors);
            return;
        }

        var result = rebateService.Calculate(new CalculateRebateRequest
        {
            RebateIdentifier = rebateId,
            ProductIdentifier = productId,
            Volume = volume
        });

        Console.WriteLine($"Success: {result.Success}");
    }

    static void WriteSampleData()
    {
        Console.WriteLine("Sample rebates:");
        foreach (var rebate in SampleData.Rebates)
        {
            Console.WriteLine($"- {rebate.Identifier} ({rebate.Incentive})");
        }

        Console.WriteLine("Sample products:");
        foreach (var product in SampleData.Products)
        {
            Console.WriteLine($"- {product.Identifier} (supports {product.SupportedIncentives})");
        }

        Console.WriteLine();
    }

    static bool TryValidate(
        string rebateId,
        string productId,
        string volumeText,
        out decimal volume,
        out List<string> errors)
    {
        errors = new List<string>();
        volume = 0m;

        if (string.IsNullOrWhiteSpace(rebateId))
        {
            errors.Add("Rebate identifier is required.");
        }

        if (string.IsNullOrWhiteSpace(productId))
        {
            errors.Add("Product identifier is required.");
        }

        if (!decimal.TryParse(volumeText, out volume))
        {
            errors.Add($"Volume must be a valid decimal. Received: '{volumeText}'.");
        }

        return errors.Count == 0;
    }

    static void WriteValidationErrors(IEnumerable<string> errors)
    {
        Console.WriteLine("Invalid input:");
        foreach (var error in errors)
        {
            Console.WriteLine($"- {error}");
        }
    }

    static string Read(string label)
    {
        Console.Write($"{label}: ");
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }
}
