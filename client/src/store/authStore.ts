import type { AuthSession } from '../types'

const KEY = 'tradeflow-session'

export function getSession(): AuthSession | null {
  const raw = localStorage.getItem(KEY)
  if (!raw) return null
  try { return JSON.parse(raw) as AuthSession } catch { localStorage.removeItem(KEY); return null }
}

export function saveSession(session: AuthSession): void { localStorage.setItem(KEY, JSON.stringify(session)) }
export function clearSession(): void { localStorage.removeItem(KEY) }
