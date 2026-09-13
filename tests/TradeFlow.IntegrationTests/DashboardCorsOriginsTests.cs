using TradeFlow.Api.Configuration;

namespace TradeFlow.IntegrationTests;

public sealed class DashboardCorsOriginsTests
{
    [Fact]
    public void Resolve_includes_local_defaults_and_configured_production_origins()
    {
        var origins = DashboardCorsOrigins.Resolve("https://tradeflow.vercel.app, https://preview.vercel.app");

        Assert.Contains("http://localhost:5173", origins);
        Assert.Contains("http://localhost:3000", origins);
        Assert.Contains("https://tradeflow.vercel.app", origins);
        Assert.Contains("https://preview.vercel.app", origins);
    }
}
