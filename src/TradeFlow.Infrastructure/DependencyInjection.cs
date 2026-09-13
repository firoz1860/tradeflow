using Microsoft.Extensions.DependencyInjection;
using TradeFlow.Application.Abstractions;
using TradeFlow.Infrastructure.Authentication;
using TradeFlow.Infrastructure.Persistence.Repositories;

namespace TradeFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTradeFlowInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITradingRepository, EfTradingRepository>();
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        return services;
    }
}
