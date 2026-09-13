# TradeFlow — Real-Time Paper Trading, Order Matching & Risk Management Platform

TradeFlow is a full-stack **paper-trading** platform. Users receive virtual USD, BTC, and ETH, submit simulated orders, and see matched trades and order-book changes live. It never connects to a brokerage or real money.

## What it demonstrates

- C# OOP, SOLID boundaries, dependency injection, and clean modular architecture
- Price-time-priority (FIFO) order matching with partial fills and O(1) cancellation lookup
- `async`/`await`, `Channel<T>`, and per-symbol workers for safe concurrency
- ASP.NET Core REST APIs, JWT authentication, roles, EF Core, PostgreSQL, Redis, SignalR, Serilog, xUnit, Docker Compose, and React/TypeScript
- Practical risk checks: pair halt, max order quantity, max notional, virtual cash checks, and virtual asset checks

## Architecture

```text
React dashboard → REST API / SignalR → Risk engine → Per-symbol queue → Matching engine
                                         ↓                 ↓
                                   PostgreSQL wallets ← Settlement
                                         ↓
                                      Redis cache
```

## Quick start with Docker

Prerequisite: Docker Desktop.

```bash
docker compose up --build
```

Open:

- Dashboard: http://localhost:5173
- Swagger API: http://localhost:8080/swagger
- Health check: http://localhost:8080/health

Stop containers:

```bash
docker compose down
```

Reset local database data:

```bash
docker compose down -v
```

## Manual setup

Prerequisites: .NET 8 SDK, Node.js 20+, PostgreSQL 16, and Redis 7.

```bash
# Terminal 1: PostgreSQL and Redis can be started with Docker
docker compose up postgres redis -d

# Terminal 2: API
dotnet restore TradeFlow.sln
dotnet run --project src/TradeFlow.Api

# Terminal 3: dashboard
cd client
npm install
npm run dev
```

Use `http://localhost:5173`, create a new account, then place limit orders. Each account begins with **100,000 virtual USD, 5 virtual BTC, and 100 virtual ETH**.

## Example trade scenario

1. Trader A creates a paper account and places `SELL 1 BTC at 60,000 USD`.
2. Trader B creates a paper account and places `BUY 1 BTC at 60,000 USD`.
3. TradeFlow reserves virtual funds, matches the orders FIFO, records a trade, settles virtual wallets, and pushes the update through SignalR.

## Tests

```bash
dotnet test TradeFlow.sln
```

Key unit tests cover FIFO execution, partial fills, cancellation, market-order remainder handling, and risk rejections.

## Important limitation

This is a portfolio-grade learning project, not a real exchange. A production trading system additionally needs regulatory approval, KYC/AML, real custody, market-data licensing, comprehensive audit controls, resilient event storage, monitoring, security review, and broker/exchange integrations.
