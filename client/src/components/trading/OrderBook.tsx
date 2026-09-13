import type { OrderBookSnapshot } from '../../types'

export function OrderBook({ book }: { book: OrderBookSnapshot }) {
  const asks = [...book.asks].reverse()
  return <section className="panel order-book">
    <div className="panel-title"><h2>Order book</h2><span className="muted">{book.symbol}</span></div>
    <div className="book-head"><span>Price (USD)</span><span>Size</span><span>Orders</span></div>
    <div className="book-list asks">{asks.length ? asks.map(level => <div className="book-row" key={`ask-${level.price}`}><span>${level.price.toLocaleString()}</span><span>{level.quantity}</span><span>{level.orderCount}</span></div>) : <p className="empty">No sell orders</p>}</div>
    <div className="spread">Live paper market</div>
    <div className="book-list bids">{book.bids.length ? book.bids.map(level => <div className="book-row" key={`bid-${level.price}`}><span>${level.price.toLocaleString()}</span><span>{level.quantity}</span><span>{level.orderCount}</span></div>) : <p className="empty">No buy orders</p>}</div>
  </section>
}
