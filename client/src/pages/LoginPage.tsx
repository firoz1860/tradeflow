import { useState } from 'react'
import { login, register } from '../api/authApi'
import type { AuthSession } from '../types'

export function LoginPage({ onAuthenticated }: { onAuthenticated: (session: AuthSession) => void }) {
  const [mode, setMode] = useState<'login' | 'register'>('login')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function submit(event: React.FormEvent) {
    event.preventDefault()
    setLoading(true)
    setError('')
    try {
      onAuthenticated(mode === 'login' ? await login(email, password) : await register(email, password))
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : 'Authentication failed.')
    } finally { setLoading(false) }
  }

  return <main className="auth-shell"><section className="auth-card">
    <p className="eyebrow">PAPER TRADING PLATFORM</p><h1>Trade<span>Flow</span></h1><p className="muted">Practice order matching and risk controls with virtual funds.</p>
    <div className="auth-tabs"><button className={mode === 'login' ? 'active' : ''} onClick={() => setMode('login')}>Log in</button><button className={mode === 'register' ? 'active' : ''} onClick={() => setMode('register')}>Create account</button></div>
    <form onSubmit={submit}><label>Email<input type="email" required value={email} onChange={event => setEmail(event.target.value)} placeholder="you@example.com" /></label><label>Password<input type="password" minLength={8} required value={password} onChange={event => setPassword(event.target.value)} placeholder="Minimum 8 characters" /></label>{error && <div className="notice error">{error}</div>}<button className="primary" disabled={loading}>{loading ? 'Please wait…' : mode === 'login' ? 'Log in' : 'Create paper account'}</button></form>
    <p className="fine-print">New accounts receive simulated USD, BTC, and ETH. No real money is used.</p>
  </section></main>
}
