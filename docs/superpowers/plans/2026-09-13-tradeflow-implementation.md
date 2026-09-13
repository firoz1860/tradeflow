# TradeFlow Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Deliver a runnable, Docker-based paper-trading application and dashboard.

**Architecture:** A modular .NET 8 solution separates domain rules, matching, risk, application orchestration, infrastructure adapters, and Web API presentation. React consumes REST APIs and SignalR updates.

**Tech Stack:** C#/.NET 8, ASP.NET Core, EF Core, PostgreSQL, Redis, SignalR, React, TypeScript, Vite, Docker Compose, xUnit.

**Spec:** `docs/superpowers/specs/2026-09-13-tradeflow-design.md`

## Global Constraints

- Use virtual funds only.
- Process a symbol's order stream sequentially.
- Keep entities independent of database and web concerns.
- Keep secrets out of source control.

---

### Task 1: Domain and matching engine

**Files:**
- Create: `src/TradeFlow.Domain/Entities/Order.cs`
- Create: `src/TradeFlow.MatchingEngine/Services/MatchingEngine.cs`
- Test: `tests/TradeFlow.MatchingEngine.Tests/MatchingEngineTests.cs`

- [ ] Write tests for FIFO, partial fills, and cancellation.
- [ ] Implement order-book data structures and matching behavior.
- [ ] Run `dotnet test tests/TradeFlow.MatchingEngine.Tests`.

### Task 2: Risk rules and application services

**Files:**
- Create: `src/TradeFlow.Risk/Services/RiskEngine.cs`
- Create: `src/TradeFlow.Application/Services/TradingService.cs`
- Test: `tests/TradeFlow.Risk.Tests/RiskEngineTests.cs`

- [ ] Write tests for insufficient funds, quantity limits, and halted symbols.
- [ ] Implement pre-trade evaluation and order processing orchestration.
- [ ] Run `dotnet test tests/TradeFlow.Risk.Tests`.

### Task 3: Infrastructure and API

**Files:**
- Create: `src/TradeFlow.Infrastructure/Persistence/TradeFlowDbContext.cs`
- Create: `src/TradeFlow.Api/Controllers/OrdersController.cs`
- Create: `src/TradeFlow.Api/Hubs/TradingHub.cs`

- [ ] Add EF Core persistence, JWT configuration, Redis cache, logging, controllers, and SignalR hub.
- [ ] Run `dotnet test`.

### Task 4: Dashboard and containers

**Files:**
- Create: `client/src/pages/TradingPage.tsx`
- Create: `docker-compose.yml`
- Create: `README.md`

- [ ] Create responsive dashboard screens and a SignalR client.
- [ ] Configure PostgreSQL, Redis, API, and frontend services.
- [ ] Run `docker compose up --build` and document manual startup.
