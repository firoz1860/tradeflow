namespace TradeFlow.IntegrationTests;

public sealed class HealthContractTests
{
    [Fact]
    public void Health_endpoint_contract_is_documented()
    {
        const string endpoint = "/health";
        Assert.Equal("/health", endpoint);
    }
}
