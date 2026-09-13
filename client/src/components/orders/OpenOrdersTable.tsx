export function OpenOrdersTable({ orderIds }: { orderIds: string[] }) {
  return <section className="panel"><div className="panel-title"><h2>This-session orders</h2><span className="muted">Full history is persisted in PostgreSQL</span></div>{orderIds.length ? <ul className="order-list">{orderIds.map(id => <li key={id}>{id}</li>)}</ul> : <p className="empty">No orders submitted in this session.</p>}</section>
}
