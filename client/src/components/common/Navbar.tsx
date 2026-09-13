interface NavbarProps {
  email: string
  page: 'trade' | 'portfolio'
  onNavigate: (page: 'trade' | 'portfolio') => void
  onLogout: () => void
}

export function Navbar({ email, page, onNavigate, onLogout }: NavbarProps) {
  return <header className="navbar">
    <button className="brand" onClick={() => onNavigate('trade')}>Trade<span>Flow</span></button>
    <nav>
      <button className={page === 'trade' ? 'active' : ''} onClick={() => onNavigate('trade')}>Trading</button>
      <button className={page === 'portfolio' ? 'active' : ''} onClick={() => onNavigate('portfolio')}>Portfolio</button>
    </nav>
    <div className="nav-user"><span>{email}</span><button className="link-button" onClick={onLogout}>Log out</button></div>
  </header>
}
