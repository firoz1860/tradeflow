import type { Trade } from '../../types'

export function RecentTrades({ trades }: { trades: Trade[] }) {
  return <section className="panel recent-trades">
    <div className="panel-title"><h2>Recent trades</h2><span className="status-dot">Live</span></div>
    <div className="trade-head"><span>Time</span><span>Price</span><span>Size</span></div>
    {trades.length ? trades.slice(0, 12).map(trade => <div className="trade-row" key={trade.id}><span>{new Date(trade.executedAt).toLocaleTimeString()}</span><strong>${trade.price.toLocaleString()}</strong><span>{trade.quantity}</span></div>) : <p className="empty">Trades will appear here when orders match.</p>}
  </section>
}
