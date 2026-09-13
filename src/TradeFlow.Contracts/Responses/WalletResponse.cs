using TradeFlow.Domain.Entities;

namespace TradeFlow.Contracts.Responses;

public sealed record WalletResponse(string Asset, decimal Available, decimal Reserved)
{
    public static WalletResponse From(Wallet wallet) => new(wallet.Asset, wallet.Available, wallet.Reserved);
}
