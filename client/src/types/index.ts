export type OrderSide = 'Buy' | 'Sell'
export type OrderType = 'Limit' | 'Market'

export interface AuthSession {
  accessToken: string
  expiresAt: string
  email: string
  role: string
}

export interface TradingPair {
  symbol: string
  baseAsset: string
  quoteAsset: string
  isHalted: boolean
}

export interface OrderBookLevel {
  price: number
  quantity: number
  orderCount: number
}

export interface OrderBookSnapshot {
  symbol: string
  bids: OrderBookLevel[]
  asks: OrderBookLevel[]
  generatedAt: string
}

export interface Trade {
  id: string
  symbol: string
  price: number
  quantity: number
  executedAt: string
}

export interface Wallet {
  asset: string
  available: number
  reserved: number
}

export interface PlaceOrderInput {
  symbol: string
  side: OrderSide
  type: OrderType
  quantity: number
  price?: number
  referencePrice?: number
  timeInForce: 'GoodTillCancelled' | 'ImmediateOrCancel'
}
