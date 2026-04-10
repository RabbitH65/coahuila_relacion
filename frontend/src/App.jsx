import { useState } from 'react'
import CapturePage from './pages/CapturePage'
import ListPage from './pages/ListPage'
import EditPage from './pages/EditPage'
import './styles/app.css'

function App() {
  const [view, setView] = useState('capture')
  const [selectedId, setSelectedId] = useState('')

  const handleEdit = (id) => {
    setSelectedId(id)
    setView('edit')
  }

  return (
    <div className="app-shell">
      <header className="app-header">
        <h1 className="app-title">Coahuila Relacion</h1>
        <div className="app-nav">
          <button className="btn btn--ghost" type="button" onClick={() => setView('capture')}>
            Capturar
          </button>
          <button className="btn btn--ghost" type="button" onClick={() => setView('list')}>
            Listado
          </button>
        </div>
      </header>

      {view === 'capture' && <CapturePage />}
      {view === 'list' && <ListPage onEdit={handleEdit} />}
      {view === 'edit' && (
        <EditPage registroId={selectedId} onBack={() => setView('list')} />
      )}
    </div>
  )
}

export default App
