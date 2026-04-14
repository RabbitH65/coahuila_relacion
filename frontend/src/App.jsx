import { useState } from 'react'
import Encabezado from "./components/Encabezado"
import Footer from "./components/Footer"

import CapturePage from './pages/CapturePage'
import ListPage from './pages/ListPage'
import EditPage from './pages/EditPage'
import RelationsPage from './pages/RelationsPage'
import MapPage from './pages/MapPage'

import './styles/app.css'

function App() {
  const [view, setView] = useState('capture')
  const [selectedId, setSelectedId] = useState('')
  const [selectedDeclarationId, setSelectedDeclarationId] = useState('')

  const handleEdit = (id, declarationId = '') => {
    setSelectedId(id)
    setSelectedDeclarationId(declarationId)
    setView('edit')
  }

  const handleNavigate = (nextView) => {
    setView(nextView)
    if (nextView !== 'edit') {
      setSelectedId('')
      setSelectedDeclarationId('')
    }
  }

  return (
    <div className="app-shell">
      <Encabezado view={view} onNavigate={handleNavigate} />

      <main className="app-main">
        {view === 'capture' && <CapturePage />}
        {view === 'list' && <ListPage onEdit={handleEdit} />}
        {view === 'edit' && (
          <EditPage
            registroId={selectedId}
            focusDeclaracionId={selectedDeclarationId}
            onBack={() => setView('list')}
          />
        )}
        {view === 'relations' && <RelationsPage onEdit={handleEdit} />}
        {view === 'map' && <MapPage onEdit={handleEdit} />}
      </main>

      <Footer />
    </div>
  )
}

export default App
