using Microsoft.AspNetCore.Identity;
using TradeFlow.Application.Abstractions;
using TradeFlow.Domain.Entities;

namespace TradeFlow.Infrastructure.Authentication;

public sealed class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(new User("placeholder@example.com", "placeholder"), password);

    public bool Verify(string password, string hash) =>
        _hasher.VerifyHashedPassword(new User("placeholder@example.com", "placeholder"), hash, password) is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
}
