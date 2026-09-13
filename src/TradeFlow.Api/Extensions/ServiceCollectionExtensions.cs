using Microsoft.Extensions.DependencyInjection;

namespace TradeFlow.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTradeFlowApiDefaults(this IServiceCollection services) => services;
}
