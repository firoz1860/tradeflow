namespace TradeFlow.Contracts.Responses;

public sealed record AuthResponse(string AccessToken, DateTimeOffset ExpiresAt, string Email, string Role);
