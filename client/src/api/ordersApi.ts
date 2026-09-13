import { api } from './client'
import type { PlaceOrderInput, Wallet } from '../types'

export const placeOrder = (token: string, input: PlaceOrderInput) => api<{ orderId: string; tradeIds: string[] }>('/api/orders', token, { method: 'POST', body: JSON.stringify(input) })
export const cancelOrder = (token: string, orderId: string) => api<void>(`/api/orders/${orderId}`, token, { method: 'DELETE' })
export const getPortfolio = (token: string) => api<Wallet[]>('/api/portfolio', token)
