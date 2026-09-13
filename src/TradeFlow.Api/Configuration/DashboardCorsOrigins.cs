namespace TradeFlow.Api.Configuration;

public static class DashboardCorsOrigins
{
    private static readonly string[] LocalOrigins = ["http://localhost:5173", "http://localhost:3000"];

    public static string[] Resolve(string? configuredOrigins)
    {
        var productionOrigins = (configuredOrigins ?? string.Empty)
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        return LocalOrigins
            .Concat(productionOrigins)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
