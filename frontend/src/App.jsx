import { useState } from 'react'
import Encabezado from "./components/Encabezado"

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
    <div className="min-h-screen bg-[#070A12] text-white">

      {/* 🔵 HEADER DIRECTO */}
      <Encabezado />

      {/* 📦 CONTENIDO */}
      <div className="p-4">
        {view === 'capture' && <CapturePage />}
        {view === 'list' && <ListPage onEdit={handleEdit} />}
        {view === 'edit' && (
          <EditPage registroId={selectedId} onBack={() => setView('list')} />
        )}
      </div>

    </div>
  )
}

export default App