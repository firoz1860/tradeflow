import { useState } from 'react'
import { placeOrder } from '../../api/ordersApi'
import type { OrderSide, OrderType, PlaceOrderInput, TradingPair } from '../../types'

interface Props {
  token: string
  pair: TradingPair
  onPlaced: (id: string) => void
  onError: (error: string) => void
}

export function OrderForm({ token, pair, onPlaced, onError }: Props) {
  const [side, setSide] = useState<OrderSide>('Buy')
  const [type, setType] = useState<OrderType>('Limit')
  const [quantity, setQuantity] = useState('0.10')
  const [price, setPrice] = useState(pair.symbol === 'BTC-USD' ? '60000' : '3000')
  const [submitting, setSubmitting] = useState(false)

  async function submit(event: React.FormEvent) {
    event.preventDefault()
    setSubmitting(true)
    try {
      const input: PlaceOrderInput = {
        symbol: pair.symbol,
        side,
        type,
        quantity: Number(quantity),
        price: type === 'Limit' ? Number(price) : undefined,
        referencePrice: type === 'Market' ? Number(price) : undefined,
        timeInForce: type === 'Market' ? 'ImmediateOrCancel' : 'GoodTillCancelled',
      }
      const result = await placeOrder(token, input)
      onPlaced(result.orderId)
    } catch (error) {
      onError(error instanceof Error ? error.message : 'Unable to place order.')
    } finally {
      setSubmitting(false)
    }
  }

  return <section className="panel order-form">
    <div className="panel-title"><h2>Place order</h2><span className="pair-chip">{pair.symbol}</span></div>
    <form onSubmit={submit}>
      <div className="segment">
        <button className={side === 'Buy' ? 'buy selected' : 'buy'} type="button" onClick={() => setSide('Buy')}>Buy</button>
        <button className={side === 'Sell' ? 'sell selected' : 'sell'} type="button" onClick={() => setSide('Sell')}>Sell</button>
      </div>
      <label>Order type<select value={type} onChange={event => setType(event.target.value as OrderType)}><option>Limit</option><option>Market</option></select></label>
      <label>{type === 'Market' ? 'Risk reference price (USD)' : 'Limit price (USD)'}<input min="0" step="any" required value={price} onChange={event => setPrice(event.target.value)} /></label>
      <label>Quantity ({pair.baseAsset})<input min="0.0001" step="any" required value={quantity} onChange={event => setQuantity(event.target.value)} /></label>
      <div className="order-summary">Estimated value <strong>${(Number(quantity || 0) * Number(price || 0)).toLocaleString(undefined, { maximumFractionDigits: 2 })}</strong></div>
      <button className={`primary ${side === 'Sell' ? 'sell-action' : ''}`} disabled={submitting}>{submitting ? 'Sending…' : `${side} ${pair.baseAsset}`}</button>
    </form>
  </section>
}
