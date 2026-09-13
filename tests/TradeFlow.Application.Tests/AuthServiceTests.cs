using TradeFlow.Application.Abstractions;
using TradeFlow.Application.Services;
using TradeFlow.Domain.Entities;

namespace TradeFlow.Application.Tests;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task Rejects_short_registration_password()
    {
        var service = new AuthService(new FakeUsers(), new FakePasswords(), new FakeTokens());

        var result = await service.RegisterAsync("trader@example.com", "short", CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Password must contain at least 8 characters.", result.Error);
    }

    private sealed class FakeUsers : IUserRepository
    {
        public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken) => Task.FromResult<User?>(null);
        public Task AddAsync(User user, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class FakePasswords : IPasswordService
    {
        public string Hash(string password) => password;
        public bool Verify(string password, string hash) => password == hash;
    }

    private sealed class FakeTokens : ITokenService
    {
        public (string Token, DateTimeOffset ExpiresAt) CreateAccessToken(User user) => ("token", DateTimeOffset.UtcNow.AddHours(1));
    }
}
