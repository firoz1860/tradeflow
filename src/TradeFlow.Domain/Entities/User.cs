using TradeFlow.Domain.Enums;

namespace TradeFlow.Domain.Entities;

public sealed class User
{
    private User() { }

    public User(string email, string passwordHash, UserRole role = UserRole.Trader)
    {
        Id = Guid.NewGuid();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
}
