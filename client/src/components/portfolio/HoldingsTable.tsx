import type { Wallet } from '../../types'

export function HoldingsTable({ wallets }: { wallets: Wallet[] }) {
  return <section className="panel"><div className="panel-title"><h2>Wallet balances</h2><span className="muted">Paper funds</span></div><table><thead><tr><th>Asset</th><th>Available</th><th>Reserved</th></tr></thead><tbody>{wallets.map(wallet => <tr key={wallet.asset}><td>{wallet.asset}</td><td>{wallet.available.toLocaleString()}</td><td>{wallet.reserved.toLocaleString()}</td></tr>)}</tbody></table></section>
}
