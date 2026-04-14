import logo from '../img/logo.png'

export default function Encabezado({ view = 'capture', onNavigate }) {
  const goTo = (next) => {
    if (onNavigate) onNavigate(next)
  }

  return (
    <header className="app-header">
      <div className="header-grid">
        <div className="brand">
          <div className="brand__logo-wrap">
            <img className="brand__logo" src={logo} alt="Duar Data Lab" />
          </div>
          <div className="brand__text">
            <span className="brand__eyebrow">Duar Data Lab</span>
            <h1 className="brand__title">Sistema de registro y consulta de casos</h1>
            <p className="brand__subtitle">Intelligence Systems / Minerva Ordo-Data</p>
          </div>
        </div>

        <div className="header-actions">
          <div className="header-chip">
            <span>Coahuila - 2026</span>
          </div>
          <nav className="nav-group" aria-label="Navegacion principal">
            <button
              className={`nav-btn ${view === 'capture' ? 'is-active' : ''}`}
              type="button"
              onClick={() => goTo('capture')}
            >
              Captura
            </button>
            <button
              className={`nav-btn ${view === 'list' ? 'is-active' : ''}`}
              type="button"
              onClick={() => goTo('list')}
            >
              Registros
            </button>
            <button
              className={`nav-btn ${view === 'relations' ? 'is-active' : ''}`}
              type="button"
              onClick={() => goTo('relations')}
            >
              Relaciones
            </button>
            <button
              className={`nav-btn ${view === 'map' ? 'is-active' : ''}`}
              type="button"
              onClick={() => goTo('map')}
            >
              Mapa
            </button>
          </nav>
        </div>
      </div>
    </header>
  )
}
