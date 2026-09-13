# API endpoints

| Method | Endpoint | Auth | Purpose |
| --- | --- | --- | --- |
| POST | `/api/auth/register` | No | Create a paper-trading account with virtual funds |
| POST | `/api/auth/login` | No | Obtain JWT access token |
| GET | `/api/market/pairs` | No | List available simulated trading pairs |
| GET | `/api/market/order-book/{symbol}` | No | Get current book snapshot |
| GET | `/api/market/trades/{symbol}` | No | Get recent fills |
| POST | `/api/orders` | Trader | Validate and submit an order |
| DELETE | `/api/orders/{orderId}` | Trader | Cancel an open order |
| GET | `/api/portfolio` | Trader | View virtual wallet balances |
| PUT | `/api/admin/pairs/{symbol}/status` | RiskAdmin/SystemAdmin | Halt or resume a pair |
| GET | `/health` | No | Service health response |

SignalR hub: `/hubs/trading`. Clients call `JoinSymbol("BTC-USD")` and receive `orderBookUpdated`, `tradesExecuted`, `portfolioUpdated`, and `riskAlert` events.
