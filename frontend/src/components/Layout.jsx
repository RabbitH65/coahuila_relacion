import Encabezado from "./Encabezado"; // ajusta la ruta si está en components

export default function Layout({ children, setView }) {
  return (
    <div className="min-h-screen bg-[#070A12] text-white">
      
      {/* 🔵 HEADER */}
      <Encabezado />

      {/* 🧭 NAV / MENU si tienes */}
      {/* aquí podrías tener botones que usan setView */}

      {/* 📦 CONTENIDO PRINCIPAL */}
      <main>
        {children}
      </main>

    </div>
  );
}export default function Layout({ children, setView }) {
  return (
    <div className="layout">
      <aside className="sidebar">
        <div className="sidebar__title">Fiscalía</div>

        <nav className="sidebar__menu">
            <div className="sidebar__item" onClick={() => setView('capture')}>
        📄 <span>Captura</span>
      </div>

      <div className="sidebar__item" onClick={() => setView('list')}>
        📂 <span>Registros</span>
      </div>
          </nav>
      </aside>

      <div className="main">
        <header className="header">
          Sistema de Declaraciones
        </header>

        <div className="content">
          {children}
        </div>
      </div>
    </div>
  )
}