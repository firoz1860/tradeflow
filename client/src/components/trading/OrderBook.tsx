import type { CSSProperties } from 'react'
import type { OrderBookLevel, OrderBookSnapshot } from '../../types'

// Depth bars are scaled against the largest resting size on either side so a
// row's fill reads as its share of visible liquidity.
function depthStyle(level: OrderBookLevel, maxQuantity: number): CSSProperties {
  const pct = maxQuantity > 0 ? Math.min(100, (level.quantity / maxQuantity) * 100) : 0
  return { '--depth': `${pct}%` } as CSSProperties
}

export function OrderBook({ book }: { book: OrderBookSnapshot }) {
  const asks = [...book.asks].reverse()
  const maxQuantity = Math.max(1, ...book.bids.map(l => l.quantity), ...book.asks.map(l => l.quantity))

  return <section className="panel order-book">
    <div className="panel-title"><h2>Order book</h2><span className="muted">{book.symbol}</span></div>
    <div className="book-head"><span>Price (USD)</span><span>Size</span><span>Orders</span></div>
    <div className="book-list asks">{asks.length ? asks.map(level => <div className="book-row" style={depthStyle(level, maxQuantity)} key={`ask-${level.price}-${level.quantity}`}><span>${level.price.toLocaleString()}</span><span>{level.quantity}</span><span>{level.orderCount}</span></div>) : <p className="empty">No sell orders</p>}</div>
    <div className="spread">Live paper market</div>
    <div className="book-list bids">{book.bids.length ? book.bids.map(level => <div className="book-row" style={depthStyle(level, maxQuantity)} key={`bid-${level.price}-${level.quantity}`}><span>${level.price.toLocaleString()}</span><span>{level.quantity}</span><span>{level.orderCount}</span></div>) : <p className="empty">No buy orders</p>}</div>
  </section>
}
