# TradeFlow architecture

TradeFlow is intentionally a modular monolith: it can run as one Docker API while its high-value boundaries remain clear and testable.

| Module | Owns | Must not depend on |
| --- | --- | --- |
| Domain | entities, order state, value rules | HTTP, database, Redis |
| MatchingEngine | FIFO price-time matching and symbol queues | EF Core, SignalR |
| Risk | pre-trade checks | controller code |
| Application | use cases and settlement orchestration | Npgsql, web framework |
| Infrastructure | EF Core, JWT, Redis adapters | API controllers |
| API | REST endpoints and SignalR hub | matching data-structure internals |

## Order flow

1. Authenticated trader posts an order.
2. `RiskEngineService` checks the pair state, limits, and available virtual funds.
3. Required funds are moved from `Available` to `Reserved`.
4. `SymbolOrderQueue` serializes the request for that symbol.
5. `MatchingEngineService` creates fills using price-time priority.
6. Settlement moves reserved funds/assets between virtual wallets.
7. PostgreSQL stores the resulting orders, trades, wallets, and audit record.
8. SignalR publishes book, trade, portfolio, and risk updates.

## Concurrency rule

One `Channel<T>` worker owns each symbol's order book. BTC-USD and ETH-USD can run simultaneously, but two BTC-USD commands are always processed in arrival order. This avoids unsafe locking around each individual price level.
