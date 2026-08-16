namespace Smartwyre.DeveloperTest.Incentives;

public sealed class RebateApplicationResult
{
    public bool Success { get; }
    public decimal Amount { get; }

    private RebateApplicationResult(bool success, decimal amount)
    {
        Success = success;
        Amount = amount;
    }

    public static RebateApplicationResult Succeeded(decimal amount) => new(true, amount);

    public static RebateApplicationResult Failed() => new(false, 0m);
}
