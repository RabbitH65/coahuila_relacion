import logo from '../img/logo.png'

export default function Footer() {
  return (
    <footer className="app-footer">
      <div className="footer-grid">
        <div className="footer-brand">
          <div className="footer-logo-wrap">
            <img className="footer-logo" src={logo} alt="Duar Data Lab" />
          </div>
          <div>
            <p className="footer-title">Duar Data Lab</p>
            <p className="footer-subtitle">Intelligence Systems</p>
          </div>
        </div>

        <div className="footer-meta">
          <span className="footer-chip">Intelligence Systems</span>
          <span className="footer-chip">Minerva Ordo-Data</span>
          <span className="footer-chip footer-chip--ghost">Coahuila 2026</span>
        </div>

        <p className="footer-copy">
          {'\u00A9'} 2026 Duar Data Lab {'\u2014'} Intelligence Systems Minerva Ordo-Data
        </p>
      </div>
    </footer>
  )
}
