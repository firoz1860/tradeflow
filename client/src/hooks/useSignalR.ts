import { useEffect } from 'react'
import * as signalR from '@microsoft/signalr'
import { API_URL } from '../api/client'
import type { OrderBookSnapshot, Trade, Wallet } from '../types'

interface SignalREvents {
  onBook: (book: OrderBookSnapshot) => void
  onTrades: (trades: Trade[]) => void
  onPortfolio: (wallets: Wallet[]) => void
  onRisk: (message: string) => void
}

export function useSignalR(token: string | undefined, symbol: string, events: SignalREvents): void {
  useEffect(() => {
    if (!token) return

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_URL}/hubs/trading`, {
        accessTokenFactory: () => token,
        transport: signalR.HttpTransportType.WebSockets,
        skipNegotiation: true,
      })
      .withAutomaticReconnect()
      .build()

    connection.on('orderBookUpdated', (payload: { snapshot: OrderBookSnapshot }) => events.onBook(payload.snapshot))
    connection.on('tradesExecuted', (payload: { trades: Trade[] }) => events.onTrades(payload.trades))
    connection.on('portfolioUpdated', (payload: { wallets: Wallet[] }) => events.onPortfolio(payload.wallets))
    connection.on('riskAlert', (payload: { symbol: string; reason: string }) => events.onRisk(`${payload.symbol}: ${payload.reason}`))

    let stopped = false

    void (async () => {
      try {
        await connection.start()
        if (!stopped) await connection.invoke('JoinSymbol', symbol)
      } catch (error: unknown) {
        if (!stopped) events.onRisk(`Live connection unavailable: ${String(error)}`)
      }
    })()

    return () => {
      stopped = true
      void connection.stop()
    }
  }, [token, symbol, events])
}