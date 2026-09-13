using TradeFlow.Domain.Entities;

namespace TradeFlow.Application.Abstractions;

public interface ITokenService
{
    (string Token, DateTimeOffset ExpiresAt) CreateAccessToken(User user);
}
