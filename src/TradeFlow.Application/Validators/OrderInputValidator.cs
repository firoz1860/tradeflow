using TradeFlow.Application.DTOs;
using TradeFlow.Domain.Enums;

namespace TradeFlow.Application.Validators;

public static class OrderInputValidator
{
    public static string? Validate(PlaceOrderCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Symbol)) return "Symbol is required.";
        if (command.Quantity <= 0) return "Quantity must be greater than zero.";
        if (command.Type == OrderType.Limit && (!command.Price.HasValue || command.Price <= 0)) return "Limit orders require a positive price.";
        if (command.Type == OrderType.Market && command.Side == OrderSide.Buy && (!command.ReferencePrice.HasValue || command.ReferencePrice <= 0)) return "Market buy orders require a positive reference price.";
        return null;
    }
}
