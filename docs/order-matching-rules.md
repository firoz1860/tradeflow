# Order matching rules

- **Best price wins:** a buy takes the lowest available sell; a sell takes the highest available buy.
- **FIFO at equal price:** first resting order at a price is filled first.
- **Limit order:** fills only at its requested price or better. A Good-Till-Cancelled remainder rests on the book.
- **Market order:** fills immediately against available liquidity; an unfilled remainder is cancelled.
- **Partial fill:** an order can fill across multiple counter-orders or price levels.
- **Cancellation:** open order removal is O(1) through an ID-to-linked-list-node index.

TradeFlow is an educational paper-trading engine. It does not implement real-exchange regulatory controls, market-data licensing, clearing, or brokerage connectivity.
