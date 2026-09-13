# TradeFlow Design

## Goal

Build a dockerized paper-trading platform in C#/.NET with price-time-priority order matching, pre-trade risk checks, JWT authentication, PostgreSQL persistence, Redis caching, SignalR notifications, and a React dashboard.

## Scope

TradeFlow uses simulated USD and assets only. It does not connect to a real exchange, broker, payment provider, or use real money.

## Architecture

The backend is a modular monolith. `Domain` owns entities and enums; `MatchingEngine` owns in-memory books; `Risk` evaluates validation rules; `Application` orchestrates use cases; `Infrastructure` integrates EF Core, JWT, Redis, and SignalR; `Api` exposes HTTP and hub endpoints. One `Channel<T>` worker processes each symbol in order so a single order book is never modified by two requests concurrently.

## Key rules

- Limit buys match the lowest eligible ask; limit sells match the highest eligible bid.
- Within a price level, earliest accepted orders fill first (FIFO).
- Market orders consume available opposite-side liquidity and cancel any remaining quantity.
- A user cannot place an order without sufficient available simulated funds/assets.
- A risk rejection is stored and returned with a human-readable reason.
- A successful match creates fills, updates balances, persists records, and pushes SignalR updates.

## Data

PostgreSQL stores users, wallets, orders, trades, risk limits, and audit logs. Redis caches order-book snapshots and can be used as a SignalR scale-out backplane.

## Security

JWT Bearer tokens protect APIs; roles are `Trader`, `RiskAdmin`, and `SystemAdmin`. Development secrets are supplied through environment variables and are not suitable for production.

## Testing

xUnit tests cover price-time priority, partial fills, cancellation, market-order remainder handling, balance validation, position limits, and request validation.
