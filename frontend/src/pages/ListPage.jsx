import { useEffect, useState } from 'react'
import { deleteRegistro, fetchRegistros } from '../services/api'
import '../styles/list.css'

function ListPage({ onEdit }) {
  const [items, setItems] = useState([])
  const [query, setQuery] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const [deletingId, setDeletingId] = useState('')

  const load = async (currentQuery = '') => {
    setLoading(true)
    setError('')
    setSuccess('')
    try {
      const data = await fetchRegistros(currentQuery)
      setItems(data)
    } catch (err) {
      setError(err.message || 'No se pudieron cargar los registros.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load('')
  }, [])

  const handleSearch = () => {
    load(query)
  }

  const handleDelete = async (item) => {
    const label = item.numero || `AP ${item.ap || '-'} / Foja ${item.foja || '-'}`
    const confirmed = window.confirm(
      `Estas seguro de eliminar por completo el registro ${label}? Esta accion no se puede deshacer.`,
    )

    if (!confirmed) return

    setDeletingId(item.id)
    setError('')
    setSuccess('')

    try {
      await deleteRegistro(item.id)
      setItems((prev) => prev.filter((registro) => registro.id !== item.id))
      setSuccess('Registro eliminado correctamente.')
    } catch (err) {
      setError(err.message || 'No se pudo eliminar el registro.')
    } finally {
      setDeletingId('')
    }
  }

  return (
    <main className="list">
      <header className="list__header">
        <div>
          <h1 className="list__title">Listado de registros</h1>
          <p className="list__subtitle">Vista base para consultar y editar.</p>
        </div>
        <div className="list__actions">
          <input
            className="list__search"
            type="text"
            placeholder="Buscar por no., ap, tomo, foja..."
            value={query}
            onChange={(event) => setQuery(event.target.value)}
          />
          <button className="btn btn--primary" type="button" onClick={handleSearch}>
            Buscar
          </button>
        </div>
      </header>

      {loading && <p className="list__status">Cargando...</p>}
      {error && <p className="list__status list__status--error">{error}</p>}
      {success && <p className="list__status list__status--success">{success}</p>}

      {!loading && !items.length && (
        <p className="list__status">Aun no hay registros.</p>
      )}

      <div className="list__table">
        <div className="list__row list__row--head">
          <span>No.</span>
          <span>Ap</span>
          <span>Tomo</span>
          <span>Foja</span>
          <span>Fecha</span>
          <span>Declarante</span>
          <span>Declaraciones</span>
          <span>Accion</span>
        </div>
        {items.map((item) => (
          <div className="list__row" key={item.id}>
            <span>{item.numero || '-'}</span>
            <span>{item.ap || '-'}</span>
            <span>{item.tomo || '-'}</span>
            <span>{item.foja || '-'}</span>
            <span>{item.fecha ? item.fecha.slice(0, 10) : '-'}</span>
            <span>{item.declarante || '-'}</span>
            <span>{item.declaracionesCount ?? 0}</span>
            <span className="list__row-actions">
              <button
                className="btn btn--ghost"
                type="button"
                onClick={() => onEdit(item.id)}
              >
                Editar
              </button>
              <button
                className="btn btn--danger"
                type="button"
                disabled={deletingId === item.id}
                onClick={() => handleDelete(item)}
              >
                {deletingId === item.id ? 'Eliminando...' : 'Eliminar'}
              </button>
            </span>
          </div>
        ))}
      </div>
    </main>
  )
}

export default ListPage
