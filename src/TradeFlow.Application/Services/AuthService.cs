using TradeFlow.Application.Abstractions;
using TradeFlow.Domain.Entities;

namespace TradeFlow.Application.Services;

public sealed class AuthService(IUserRepository users, IPasswordService passwords, ITokenService tokens)
{
    public async Task<(bool Success, string? Error, User? User, string? Token, DateTimeOffset? ExpiresAt)> RegisterAsync(string email, string password, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@')) return (false, "Enter a valid email address.", null, null, null);
        if (password.Length < 8) return (false, "Password must contain at least 8 characters.", null, null, null);
        if (await users.FindByEmailAsync(email, cancellationToken) is not null) return (false, "An account with this email already exists.", null, null, null);

        var user = new User(email, passwords.Hash(password));
        await users.AddAsync(user, cancellationToken);
        await users.SaveChangesAsync(cancellationToken);
        var token = tokens.CreateAccessToken(user);
        return (true, null, user, token.Token, token.ExpiresAt);
    }

    public async Task<(bool Success, string? Error, User? User, string? Token, DateTimeOffset? ExpiresAt)> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await users.FindByEmailAsync(email, cancellationToken);
        if (user is null || !passwords.Verify(password, user.PasswordHash)) return (false, "Invalid email or password.", null, null, null);
        var token = tokens.CreateAccessToken(user);
        return (true, null, user, token.Token, token.ExpiresAt);
    }
}
