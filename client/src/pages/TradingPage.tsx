import { useEffect, useMemo, useState } from 'react'
import { getOrderBook, getPairs, getTrades } from '../api/marketApi'
import { OrderBook } from '../components/trading/OrderBook'
import { OrderForm } from '../components/trading/OrderForm'
import { RecentTrades } from '../components/trading/RecentTrades'
import { OpenOrdersTable } from '../components/orders/OpenOrdersTable'
import { RiskAlerts } from '../components/risk/RiskAlerts'
import { Notice } from '../components/common/Notice'
import { useSignalR } from '../hooks/useSignalR'
import type { OrderBookSnapshot, Trade, TradingPair, Wallet } from '../types'

interface Props { token: string; wallets: Wallet[]; onWallets: (wallets: Wallet[]) => void }

const blankBook = (symbol: string): OrderBookSnapshot => ({ symbol, bids: [], asks: [], generatedAt: new Date().toISOString() })

export function TradingPage({ token, wallets, onWallets }: Props) {
  const [pairs, setPairs] = useState<TradingPair[]>([])
  const [symbol, setSymbol] = useState('BTC-USD')
  const [book, setBook] = useState<OrderBookSnapshot>(blankBook('BTC-USD'))
  const [trades, setTrades] = useState<Trade[]>([])
  const [alerts, setAlerts] = useState<string[]>([])
  const [notice, setNotice] = useState('')
  const [orderIds, setOrderIds] = useState<string[]>([])

  const selectedPair = pairs.find(pair => pair.symbol === symbol) ?? { symbol, baseAsset: symbol.split('-')[0], quoteAsset: 'USD', isHalted: false }
  const events = useMemo(() => ({
    onBook: setBook,
    onTrades: (next: Trade[]) => setTrades(current => [...next, ...current].slice(0, 30)),
    onPortfolio: onWallets,
    onRisk: (message: string) => setAlerts(current => [message, ...current].slice(0, 4)),
  }), [onWallets])
  useSignalR(token, symbol, events)

  useEffect(() => { void getPairs().then(setPairs).catch(error => setAlerts([error instanceof Error ? error.message : 'Cannot load market pairs.'])) }, [])
  useEffect(() => {
    void Promise.all([getOrderBook(symbol), getTrades(symbol)]).then(([nextBook, nextTrades]) => { setBook(nextBook); setTrades(nextTrades) }).catch(error => setAlerts([error instanceof Error ? error.message : 'Cannot load market data.']))
  }, [symbol])

  return <main className="app-content">
    <div className="page-heading"><div><p className="eyebrow">LIVE SIMULATION</p><h1>Trading terminal</h1></div><select value={symbol} onChange={event => setSymbol(event.target.value)}>{pairs.map(pair => <option key={pair.symbol}>{pair.symbol}</option>)}</select></div>
    <Notice message={notice} kind="success" />
    <RiskAlerts alerts={alerts} />
    <section className="wallet-strip">{wallets.map(wallet => <div key={wallet.asset}><span>{wallet.asset}</span><strong>{wallet.available.toLocaleString(undefined, { maximumFractionDigits: 4 })}</strong><small>reserved {wallet.reserved.toLocaleString(undefined, { maximumFractionDigits: 4 })}</small></div>)}</section>
    <div className="trading-grid"><OrderForm token={token} pair={selectedPair} onPlaced={id => { setOrderIds(current => [id, ...current]); setNotice(`Order ${id.slice(0, 8)} accepted by TradeFlow.`) }} onError={error => setAlerts(current => [error, ...current])} /><OrderBook book={book} /><RecentTrades trades={trades} /></div>
    <OpenOrdersTable orderIds={orderIds} />
  </main>
}
