import { api } from './client'
import type { OrderBookSnapshot, Trade, TradingPair } from '../types'

export const getPairs = () => api<TradingPair[]>('/api/market/pairs')
export const getOrderBook = (symbol: string) => api<OrderBookSnapshot>(`/api/market/order-book/${symbol}`)
export const getTrades = (symbol: string) => api<Trade[]>(`/api/market/trades/${symbol}`)
