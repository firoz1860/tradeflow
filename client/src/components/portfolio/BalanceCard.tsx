import type { Wallet } from '../../types'

export function BalanceCard({ wallet }: { wallet: Wallet }) {
  return <article className="balance-card"><p>{wallet.asset}</p><strong>{wallet.available.toLocaleString(undefined, { maximumFractionDigits: 4 })}</strong><small>Reserved: {wallet.reserved.toLocaleString(undefined, { maximumFractionDigits: 4 })}</small></article>
}
