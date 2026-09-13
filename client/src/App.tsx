import { useEffect, useState } from 'react'
import { getPortfolio } from './api/ordersApi'
import { Navbar } from './components/common/Navbar'
import { LoginPage } from './pages/LoginPage'
import { PortfolioPage } from './pages/PortfolioPage'
import { TradingPage } from './pages/TradingPage'
import { clearSession, getSession, saveSession } from './store/authStore'
import type { AuthSession, Wallet } from './types'

export default function App() {
  const [session, setSession] = useState<AuthSession | null>(() => getSession())
  const [page, setPage] = useState<'trade' | 'portfolio'>('trade')
  const [wallets, setWallets] = useState<Wallet[]>([])

  useEffect(() => {
    if (!session) { setWallets([]); return }
    void getPortfolio(session.accessToken).then(setWallets).catch(() => setWallets([]))
  }, [session])

  if (!session) return <LoginPage onAuthenticated={next => { saveSession(next); setSession(next) }} />

  return <><Navbar email={session.email} page={page} onNavigate={setPage} onLogout={() => { clearSession(); setSession(null) }} />{page === 'trade' ? <TradingPage token={session.accessToken} wallets={wallets} onWallets={setWallets} /> : <PortfolioPage wallets={wallets} />}</>
}
