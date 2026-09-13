using TradeFlow.Application.Abstractions;
using TradeFlow.Application.DTOs;
using TradeFlow.Domain.Entities;
using TradeFlow.Domain.Enums;
using TradeFlow.MatchingEngine.Models;
using TradeFlow.MatchingEngine.Services;
using TradeFlow.Risk.Models;
using TradeFlow.Risk.Services;
using TradeFlow.Application.Validators;

namespace TradeFlow.Application.Services;

public sealed class TradingService(
    ITradingRepository repository,
    RiskEngineService riskEngine,
    OrderBookRegistry books,
    ITradingNotifier notifier)
{
    public async Task<PlaceOrderResult> PlaceOrderAsync(PlaceOrderCommand command, CancellationToken cancellationToken)
    {
        var inputError = OrderInputValidator.Validate(command);
        if (inputError is not null) return new PlaceOrderResult(false, inputError, Guid.Empty, []);
        var pair = await repository.FindPairAsync(command.Symbol, cancellationToken);
        if (pair is null) return new PlaceOrderResult(false, "Unknown trading pair.", Guid.Empty, []);

        var order = command.Type == OrderType.Limit
            ? Order.Limit(command.UserId, pair.Symbol, command.Side, command.Price ?? 0m, command.Quantity, command.TimeInForce)
            : Order.Market(command.UserId, pair.Symbol, command.Side, command.Quantity, command.ReferencePrice);

        var quoteWallet = await repository.GetOrCreateWalletAsync(command.UserId, pair.QuoteAsset, cancellationToken);
        var baseWallet = await repository.GetOrCreateWalletAsync(command.UserId, pair.BaseAsset, cancellationToken);
        var limit = await repository.GetOrCreateRiskLimitAsync(command.UserId, cancellationToken);
        var riskContext = new RiskContext(quoteWallet.Available, baseWallet.Available, limit.MaxOrderQuantity, limit.MaxOrderNotional, pair.IsHalted);
        var risk = riskEngine.Evaluate(order, riskContext);
        if (!risk.IsAllowed)
        {
            order.Reject(risk.Reason!);
            await repository.UpsertOrderAsync(order, cancellationToken);
            await repository.AddAuditAsync(new AuditLog(command.UserId, "ORDER_REJECTED", risk.Reason!), cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            await notifier.PublishRiskAlertAsync(command.UserId, pair.Symbol, risk.Reason!, cancellationToken);
            return new PlaceOrderResult(false, risk.Reason, order.Id, []);
        }

        ReserveFunds(order, quoteWallet, baseWallet);
        await repository.UpsertOrderAsync(order, cancellationToken);

        var queue = books.GetOrCreate(pair.Symbol);
        var report = await queue.SubmitAsync(order, cancellationToken);
        await SettleAsync(pair, report, cancellationToken);
        foreach (var changedOrder in report.AffectedOrders) await repository.UpsertOrderAsync(changedOrder, cancellationToken);
        await repository.AddTradesAsync(report.Trades, cancellationToken);
        await repository.AddAuditAsync(new AuditLog(command.UserId, "ORDER_PROCESSED", $"{order.Id} {order.Status}"), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await notifier.PublishOrderBookAsync(queue.Engine.GetSnapshot(), cancellationToken);
        await notifier.PublishTradesAsync(report.Trades, cancellationToken);
        await PublishAffectedPortfoliosAsync(report, cancellationToken);
        return new PlaceOrderResult(true, null, order.Id, report.Trades.Select(trade => trade.Id).ToList());
    }

    public async Task<(bool Success, string? Error)> CancelOrderAsync(string userId, Guid orderId, CancellationToken cancellationToken)
    {
        var order = await repository.FindOrderAsync(orderId, cancellationToken);
        if (order is null || order.UserId != userId) return (false, "Order was not found.");
        if (!books.TryGet(order.Symbol, out var queue) || queue is null || !queue.Engine.Cancel(orderId, userId)) return (false, "Order cannot be cancelled.");

        var pair = await repository.FindPairAsync(order.Symbol, cancellationToken);
        if (pair is null) return (false, "Trading pair was not found.");
        await ReleaseRemainingReservationAsync(pair, order, cancellationToken);
        await repository.UpsertOrderAsync(order, cancellationToken);
        await repository.AddAuditAsync(new AuditLog(userId, "ORDER_CANCELLED", order.Id.ToString()), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await notifier.PublishOrderBookAsync(queue.Engine.GetSnapshot(), cancellationToken);
        await notifier.PublishPortfolioAsync(userId, await repository.GetWalletsAsync(userId, cancellationToken), cancellationToken);
        return (true, null);
    }

    private static void ReserveFunds(Order order, Wallet quoteWallet, Wallet baseWallet)
    {
        if (order.Side == OrderSide.Sell)
        {
            baseWallet.Reserve(order.OriginalQuantity);
            return;
        }

        var price = order.Price ?? order.ReferencePrice ?? throw new InvalidOperationException("A buy order needs an estimated price.");
        quoteWallet.Reserve(price * order.OriginalQuantity);
    }

    private async Task SettleAsync(TradingPair pair, ExecutionReport report, CancellationToken cancellationToken)
    {
        foreach (var trade in report.Trades)
        {
            var buyerId = trade.MakerSide == OrderSide.Buy ? trade.MakerUserId : trade.TakerUserId;
            var sellerId = trade.MakerSide == OrderSide.Sell ? trade.MakerUserId : trade.TakerUserId;
            var buyerQuote = await repository.GetOrCreateWalletAsync(buyerId, pair.QuoteAsset, cancellationToken);
            var buyerBase = await repository.GetOrCreateWalletAsync(buyerId, pair.BaseAsset, cancellationToken);
            var sellerQuote = await repository.GetOrCreateWalletAsync(sellerId, pair.QuoteAsset, cancellationToken);
            var sellerBase = await repository.GetOrCreateWalletAsync(sellerId, pair.BaseAsset, cancellationToken);
            var quoteQuantity = trade.Price * trade.Quantity;
            var buyerOrderId = trade.MakerSide == OrderSide.Buy ? trade.MakerOrderId : trade.TakerOrderId;
            var buyerOrder = report.AffectedOrders.Single(order => order.Id == buyerOrderId);

            buyerQuote.ConsumeReserved(quoteQuantity);
            var reservationPrice = buyerOrder.Price ?? buyerOrder.ReferencePrice ?? trade.Price;
            var priceImprovement = Math.Max(0m, reservationPrice - trade.Price) * trade.Quantity;
            if (priceImprovement > 0) buyerQuote.Release(priceImprovement);
            buyerBase.Credit(trade.Quantity);
            sellerBase.ConsumeReserved(trade.Quantity);
            sellerQuote.Credit(quoteQuantity);
        }

        foreach (var order in report.AffectedOrders.Where(order => order.Status is OrderStatus.Filled or OrderStatus.Cancelled))
        {
            await ReleaseRemainingReservationAsync(pair, order, cancellationToken);
        }
    }

    private async Task ReleaseRemainingReservationAsync(TradingPair pair, Order order, CancellationToken cancellationToken)
    {
        if (order.Side == OrderSide.Sell)
        {
            if (order.RemainingQuantity > 0)
            {
                var wallet = await repository.GetOrCreateWalletAsync(order.UserId, pair.BaseAsset, cancellationToken);
                wallet.Release(order.RemainingQuantity);
            }
            return;
        }

        var quotedPrice = order.Price ?? order.ReferencePrice ?? 0m;
        var amountToRelease = order.RemainingQuantity * quotedPrice;
        if (amountToRelease > 0)
        {
            var wallet = await repository.GetOrCreateWalletAsync(order.UserId, pair.QuoteAsset, cancellationToken);
            wallet.Release(amountToRelease);
        }
    }

    private async Task PublishAffectedPortfoliosAsync(ExecutionReport report, CancellationToken cancellationToken)
    {
        var users = report.AffectedOrders.Select(order => order.UserId).Distinct(StringComparer.Ordinal);
        foreach (var userId in users)
        {
            await notifier.PublishPortfolioAsync(userId, await repository.GetWalletsAsync(userId, cancellationToken), cancellationToken);
        }
    }
}
