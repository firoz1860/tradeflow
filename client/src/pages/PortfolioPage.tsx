import { HoldingsTable } from '../components/portfolio/HoldingsTable'
import { BalanceCard } from '../components/portfolio/BalanceCard'
import type { Wallet } from '../types'

export function PortfolioPage({ wallets }: { wallets: Wallet[] }) {
  return <main className="app-content"><div className="page-heading"><div><p className="eyebrow">VIRTUAL FUNDS</p><h1>Portfolio</h1></div></div><section className="balance-grid">{wallets.map(wallet => <BalanceCard key={wallet.asset} wallet={wallet} />)}</section><HoldingsTable wallets={wallets} /><section className="panel info-panel"><h2>How balances work</h2><p>When you submit a limit order, TradeFlow reserves the needed virtual funds. On a match, balances settle automatically; cancellation releases the unfilled reservation.</p></section></main>
}
