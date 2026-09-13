# TradeFlow — Real-Time Paper Trading, Order Matching & Risk Management Platform

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18-61DAFB?logo=react\&logoColor=black)](https://react.dev/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Neon-4169E1?logo=postgresql\&logoColor=white)](https://neon.tech/)
[![Redis](https://img.shields.io/badge/Redis-Upstash-DC382D?logo=redis\&logoColor=white)](https://upstash.com/)
[![Render](https://img.shields.io/badge/API-Render-46E3B7)](https://render.com/)
[![Vercel](https://img.shields.io/badge/Dashboard-Vercel-black?logo=vercel)](https://vercel.com/)

TradeFlow is a full-stack **paper-trading platform** that simulates an exchange. Users receive virtual USD, BTC, and ETH; place market or limit orders; receive risk validation; and see order-book and trade updates in real time.

> This project uses only virtual balances. It never handles real money, real crypto, or brokerage accounts.

## Live Demo

| Service                   | URL                                                                                 |
| ------------------------- | ----------------------------------------------------------------------------------- |
| React Dashboard           | [https://client-brown-psi-57.vercel.app](https://client-brown-psi-57.vercel.app/)   |
| ASP.NET Core API          | [https://tradeflow-api-ppx2.onrender.com](https://tradeflow-api-ppx2.onrender.com/) |
| API Health Check          | https://tradeflow-api-ppx2.onrender.com/health                                      |
| Swagger API Documentation | https://tradeflow-api-ppx2.onrender.com/swagger                                     |

> Render free services can sleep when inactive. The first API request may take around one minute to wake up.

---

## Problem It Solves

A trading platform must safely handle many users placing orders at the same time. It needs to:

* Check whether a user has enough virtual USD or crypto before accepting an order.
* Reserve funds so the same balance cannot be spent twice.
* Match the best buy and sell orders fairly.
* Support partial order fills and cancellation.
* Update wallets after a trade.
* Store accounts, orders, trades, and balances permanently.
* Send instant updates to connected users.

TradeFlow demonstrates this complete flow in a safe paper-trading environment.

---

## Key Features

* JWT-based registration and login.
* Virtual wallet created for every new user:

  * `100,000 USD`
  * `5 BTC`
  * `100 ETH`
* BTC-USD and ETH-USD trading pairs.
* Limit and market orders.
* Price-time priority matching engine.
* FIFO matching for orders at the same price.
* Partial fills.
* Order cancellation.
* Funds reservation before matching.
* Wallet settlement after successful trades.
* Risk validation:

  * invalid quantity validation;
  * invalid price validation;
  * insufficient USD balance validation;
  * insufficient BTC/ETH balance validation;
  * maximum quantity/notional checks;
  * trading-pair halt support.
* Live order-book updates using SignalR.
* Live recent-trade updates using SignalR.
* PostgreSQL persistence using Entity Framework Core.
* Redis integration through StackExchange Redis.
* Structured request logging using Serilog.
* Swagger API documentation.
* Unit and integration tests.
* Docker support for local container-based development.
* Cloud deployment with Vercel, Render, Neon, and Upstash.

---

## Architecture

```mermaid
flowchart TD
    A[React + TypeScript Dashboard] -->|REST API + JWT| B[ASP.NET Core API]
    A -->|SignalR WebSocket| B
    B --> C[Risk Engine]
    C --> D[Per-Symbol Order Queue]
    D --> E[Matching Engine]
    E --> F[Settlement Service]
    F --> G[(Neon PostgreSQL)]
    B --> H[(Upstash Redis)]
    B --> I[SignalR Trading Hub]
    I --> A
```

### Order Processing Flow

```mermaid
sequenceDiagram
    participant User
    participant API as TradeFlow API
    participant Risk as Risk Engine
    participant Match as Matching Engine
    participant DB as PostgreSQL
    participant Hub as SignalR Hub

    User->>API: Submit order
    API->>Risk: Validate balance and order limits
    Risk-->>API: Accept or reject
    API->>Match: Queue accepted order
    Match->>Match: Match by price-time priority
    Match->>DB: Save order, trade, and wallets
    API->>Hub: Publish live updates
    Hub-->>User: Order book, trade, portfolio update
```

---

## Technology Stack

| Layer             | Technologies                                          |
| ----------------- | ----------------------------------------------------- |
| Frontend          | React 18, TypeScript, Vite, SignalR JavaScript Client |
| Backend           | C#, .NET 8, ASP.NET Core Web API                      |
| Authentication    | JWT Bearer Authentication                             |
| Database          | PostgreSQL, Entity Framework Core, Neon               |
| Cache             | Redis, StackExchange Redis, Upstash                   |
| Real Time         | ASP.NET Core SignalR                                  |
| Concurrency       | `async`/`await`, `Channel<T>`, per-symbol workers     |
| Logging           | Serilog                                               |
| API Documentation | Swagger / OpenAPI                                     |
| Testing           | xUnit                                                 |
| Containers        | Docker, Docker Compose                                |
| Hosting           | Vercel, Render, Neon, Upstash                         |
| Version Control   | Git and GitHub                                        |

---

## Project Structure

```text
TradeFlow/
├── client/                              # React + Vite dashboard
│   └── src/
│       ├── api/                         # REST API client
│       ├── components/                  # Reusable UI components
│       ├── hooks/                       # SignalR hook
│       ├── pages/                       # Trading and portfolio pages
│       ├── store/                       # Client-side state
│       └── types/                       # TypeScript models
│
├── src/
│   ├── TradeFlow.Api/                   # Controllers, SignalR hub, middleware
│   ├── TradeFlow.Application/           # Use cases, services, abstractions
│   ├── TradeFlow.Contracts/             # API request/response contracts
│   ├── TradeFlow.Domain/                # Entities, enums, value objects
│   ├── TradeFlow.Infrastructure/        # EF Core, PostgreSQL, JWT, Redis
│   ├── TradeFlow.MatchingEngine/        # Order book and matching algorithm
│   └── TradeFlow.Risk/                  # Risk rules and risk service
│
├── tests/
│   ├── TradeFlow.Application.Tests/
│   ├── TradeFlow.IntegrationTests/
│   ├── TradeFlow.MatchingEngine.Tests/
│   └── TradeFlow.Risk.Tests/
│
├── deploy/                              # Local database initialization files
├── docker-compose.yml                   # Local Docker setup
├── render.yaml                          # Render Blueprint configuration
├── TradeFlow.sln
└── README.md
```

---

## Core Design Principles

### OOP and SOLID

The solution separates responsibilities into focused projects:

* **Domain** contains business entities and rules.
* **Application** contains business use cases and service interfaces.
* **Infrastructure** contains database, authentication, caching, and external implementations.
* **API** exposes REST and SignalR endpoints.
* **MatchingEngine** handles price-time order matching.
* **Risk** validates balances and trading limits.

This structure follows separation of concerns, dependency inversion, and testable design.

### Matching Algorithm

TradeFlow uses price-time priority:

* Best buy order = highest price.
* Best sell order = lowest price.
* When prices are equal, the earlier order is executed first.
* Orders can be partially filled.
* Remaining quantity stays on the order book for limit orders.
* Market-order remainder is cancelled when no matching liquidity remains.

### Safe Concurrency

Each trading symbol uses its own processing queue and worker. This avoids unsafe concurrent changes to the same order book while allowing different pairs, such as BTC-USD and ETH-USD, to be processed independently.

---

## Local Setup Without Docker — WSL Ubuntu

This is the recommended setup for Windows + WSL users.

### 1. Open Ubuntu WSL

In PowerShell:

```powershell
wsl -d Ubuntu
```

### 2. Copy the project into the Linux file system

Do not run the frontend from `/mnt/c/...` because native packages such as `esbuild` can fail there.

```bash
cp -r "/mnt/c/Users/firoz ahmad/Downloads/TradeFlow/TradeFlow" /home/firoz_ahmad/tradeflow-linux
cd /home/firoz_ahmad/tradeflow-linux
```

### 3. Install and start PostgreSQL and Redis

```bash
sudo apt update
sudo apt install -y postgresql redis-server
sudo service postgresql start
sudo service redis-server start
redis-cli ping
```

Expected Redis result:

```text
PONG
```

### 4. Create the local PostgreSQL user and database

```bash
sudo -u postgres psql -tc "SELECT 1 FROM pg_roles WHERE rolname='tradeflow'" | grep -q 1 || sudo -u postgres psql -c "CREATE ROLE tradeflow LOGIN PASSWORD 'tradeflow_local_password';"

sudo -u postgres psql -tc "SELECT 1 FROM pg_database WHERE datname='tradeflow'" | grep -q 1 || sudo -u postgres psql -c "CREATE DATABASE tradeflow OWNER tradeflow;"
```

Verify PostgreSQL:

```bash
PGPASSWORD=tradeflow_local_password psql -h localhost -U tradeflow -d tradeflow -c "\conninfo"
```

### 5. Install prerequisites

Required:

* .NET SDK 8
* Node.js 18 or newer
* npm
* PostgreSQL 16
* Redis 7

Verify:

```bash
dotnet --version
node --version
/usr/bin/npm --version
```

### 6. Restore, build, and test backend

```bash
cd /home/firoz_ahmad/tradeflow-linux

dotnet restore TradeFlow.sln
dotnet build TradeFlow.sln --no-restore
dotnet test TradeFlow.sln --no-build
```

Expected result:

```text
Passed! - Failed: 0
```

### 7. Start the ASP.NET Core API

Open Terminal 1:

```bash
cd /home/firoz_ahmad/tradeflow-linux

dotnet run --project src/TradeFlow.Api --urls http://0.0.0.0:8080
```

Open:

* API: `http://localhost:8080`
* Swagger: `http://localhost:8080/swagger`
* Health: `http://localhost:8080/health`

### 8. Start the React dashboard

Open Terminal 2:

```bash
cd /home/firoz_ahmad/tradeflow-linux/client

PATH=/usr/bin:/bin /usr/bin/npm ci

VITE_API_URL=http://localhost:8080 PATH=/usr/bin:/bin /usr/bin/npm run dev
```

Open:

```text
http://localhost:5173
```

---

## Local Setup With Docker

Docker is optional.

```bash
docker compose up --build
```

Open:

| Service      | URL                             |
| ------------ | ------------------------------- |
| Dashboard    | `http://localhost:5173`         |
| Swagger      | `http://localhost:8080/swagger` |
| Health check | `http://localhost:8080/health`  |

Stop containers:

```bash
docker compose down
```

Reset Docker database data:

```bash
docker compose down -v
```

---

## Environment Variables

Never commit real passwords, JWT secrets, or database URLs to GitHub.

### Local Backend Configuration

`src/TradeFlow.Api/appsettings.json` is for local development.

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5432;Database=tradeflow;Username=tradeflow;Password=tradeflow_local_password",
    "Redis": "localhost:6379"
  }
}
```

### Production Environment Variables

Set these in Render.

| Key                           | Description                                         |
| ----------------------------- | --------------------------------------------------- |
| `ConnectionStrings__Postgres` | Neon PostgreSQL connection string in Npgsql format  |
| `ConnectionStrings__Redis`    | Upstash Redis StackExchange Redis connection string |
| `Jwt__SigningKey`             | Private random secret with at least 32 characters   |
| `Cors__AllowedOrigins`        | Public Vercel dashboard URL                         |

Example PostgreSQL format:

```text
Host=YOUR_NEON_HOST;Port=5432;Database=neondb;Username=YOUR_NEON_USER;Password=YOUR_NEON_PASSWORD;SSL Mode=Require;Trust Server Certificate=true
```

Example Redis format:

```text
YOUR_UPSTASH_ENDPOINT:YOUR_PORT,password=YOUR_UPSTASH_PASSWORD,ssl=True,abortConnect=False
```

Generate a secure JWT signing key:

```bash
openssl rand -base64 48
```

---

## Production Deployment

### 1. Neon PostgreSQL

1. Create a Neon project.
2. Keep the default database or create a `tradeflow` database.
3. Copy the pooled connection details.
4. Put the PostgreSQL connection string into Render as `ConnectionStrings__Postgres`.

TradeFlow automatically creates and seeds its tables on application startup.

### 2. Upstash Redis

1. Create a free Upstash Redis database.
2. Open **Connect**.
3. Copy endpoint, port, and password.
4. Create the StackExchange Redis format:

```text
ENDPOINT:PORT,password=PASSWORD,ssl=True,abortConnect=False
```

5. Put this into Render as `ConnectionStrings__Redis`.

### 3. Render API Deployment

1. Push the repository to GitHub.
2. In Render, choose **New + → Blueprint**.
3. Select the GitHub repository.
4. Use branch `main`.
5. Ensure `render.yaml` contains:

```yaml
plan: free
```

6. Add the production environment variables.
7. Deploy the Blueprint.
8. Verify:

```bash
curl -i https://YOUR_RENDER_API_URL/health
```

### 4. Vercel Dashboard Deployment

1. In Vercel, click **Add New → Project**.
2. Import the GitHub repository.
3. Set **Root Directory** to:

```text
client
```

4. Add environment variable:

```text
VITE_API_URL=https://YOUR_RENDER_API_URL
```

5. Deploy.
6. Copy the Vercel URL.
7. In Render, set:

```text
Cors__AllowedOrigins=https://YOUR_VERCEL_URL
```

8. Save changes and allow Render to redeploy.

---

## API Endpoints

| Method   | Endpoint                          | Purpose                                             |
| -------- | --------------------------------- | --------------------------------------------------- |
| `GET`    | `/health`                         | API health check                                    |
| `POST`   | `/api/auth/register`              | Create a paper-trading account                      |
| `POST`   | `/api/auth/login`                 | Log in and receive JWT                              |
| `GET`    | `/api/portfolio`                  | Get current user wallet balances                    |
| `POST`   | `/api/orders`                     | Place a market or limit order                       |
| `DELETE` | `/api/orders/{orderId}`           | Cancel an active order                              |
| `GET`    | `/api/market/pairs`               | Get available trading pairs                         |
| `GET`    | `/api/market/order-book/{symbol}` | Get current order-book snapshot                     |
| `GET`    | `/api/market/trades/{symbol}`     | Get recent executed trades                          |
| SignalR  | `/hubs/trading`                   | Real-time order book, trades, and portfolio updates |

Swagger UI is available at:

```text
https://tradeflow-api-ppx2.onrender.com/swagger
```

---

## Demo Trade Scenario

Use two browser sessions:

* Normal Chrome: Seller account
* Incognito Chrome: Buyer account

### Scenario: BTC Trade

**Seller**

```text
Pair: BTC-USD
Side: Sell
Order Type: Limit
Price: 60000
Quantity: 0.10
```

**Buyer**

```text
Pair: BTC-USD
Side: Buy
Order Type: Limit
Price: 60000
Quantity: 0.10
```

Expected result:

| Account |  BTC Change |   USD Change |
| ------- | ----------: | -----------: |
| Seller  | `-0.10 BTC` | `+6,000 USD` |
| Buyer   | `+0.10 BTC` | `-6,000 USD` |

TradeFlow validates balances, reserves funds, matches the orders, creates a trade, updates wallets, saves the records in PostgreSQL, and publishes live SignalR updates.

### Scenario: ETH Price Improvement

**Seller**

```text
Pair: ETH-USD
Side: Sell
Order Type: Limit
Price: 2500
Quantity: 2
```

**Buyer**

```text
Pair: ETH-USD
Side: Buy
Order Type: Limit
Price: 2600
Quantity: 2
```

Expected result:

* The orders match because the buyer accepts a price up to `$2,600`.
* The trade executes at the seller’s earlier price: `$2,500`.
* Seller receives `$5,000`.
* Buyer receives `2 ETH`.
* Buyer’s extra reserved `$200` is released.

This demonstrates price-time matching and price improvement.

---

## Verify Database Persistence

Open Neon SQL Editor and run:

```sql
SELECT "Email", "CreatedAt"
FROM users
ORDER BY "CreatedAt" DESC;
```

Check orders:

```sql
SELECT "Symbol", "Side", "Price", "RemainingQuantity", "Status", "CreatedAt"
FROM orders
ORDER BY "CreatedAt" DESC;
```

Check executed trades:

```sql
SELECT "Symbol", "Price", "Quantity", "ExecutedAt"
FROM trades
ORDER BY "ExecutedAt" DESC;
```

---

## Verify Redis

Copy the Redis CLI command from Upstash → **Connect** and run it in WSL.

Example:

```bash
redis-cli --tls -u "rediss://default:YOUR_PASSWORD@YOUR_ENDPOINT:6379" ping
```

Expected output:

```text
PONG
```

Redis is configured through `AddStackExchangeRedisCache` and is ready for distributed cache operations. The current business flow focuses on database-backed trading records and SignalR updates.

---

## Testing

Run all backend tests:

```bash
cd /home/firoz_ahmad/tradeflow-linux
dotnet test TradeFlow.sln
```

Build the production frontend:

```bash
cd /home/firoz_ahmad/tradeflow-linux/client
PATH=/usr/bin:/bin /usr/bin/npm run build
```

Test coverage includes:

* FIFO order matching.
* Partial fills.
* Order cancellation.
* Market-order remainder handling.
* Risk rejection logic.
* Application service behavior.
* Integration configuration behavior.

---

## Push README Changes to GitHub

After replacing `README.md` in VS Code:

```bash
cd /home/firoz_ahmad/tradeflow-linux

git add README.md
git commit -m "Add complete TradeFlow documentation"
git push
```

---

## Important Limitations

TradeFlow is a portfolio-grade learning project, not a real financial exchange.

A real trading platform would additionally require:

* KYC and AML compliance.
* Regulatory approvals.
* Real custody and wallet security.
* Broker or exchange integrations.
* Market-data licensing.
* Audit trails and immutable event storage.
* Rate limiting and DDoS protection.
* Monitoring, alerting, backups, and disaster recovery.
* Security review and penetration testing.
* High availability and multi-region infrastructure.

---

## Author

**Firoz Ahmad**

* GitHub: [firoz1860](https://github.com/firoz1860)
* LinkedIn: [Firoz Ahmad](https://www.linkedin.com/in/firoz-ahmad-020166251/)
* Portfolio: [job-portfolio-nine.vercel.app](https://job-portfolio-nine.vercel.app/)

---

If you found this project useful, please consider giving the repository a star.
