import { useEffect, useMemo, useState } from 'react'
import { fetchRelaciones } from '../services/api'
import '../styles/relations.css'

const sections = [
  { key: 'actores', label: 'Actores' },
  { key: 'lugares', label: 'Lugares' },
  { key: 'circunstancias', label: 'Circunstancias' },
]

const formatDate = (value) => {
  if (!value) return '-'
  return value.slice(0, 10)
}

function RelationsPage({ onEdit }) {
  const [active, setActive] = useState('actores')
  const [query, setQuery] = useState('')
  const [data, setData] = useState({
    actores: [],
    lugares: [],
    circunstancias: [],
  })
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const load = async (search = query) => {
    setLoading(true)
    setError('')
    try {
      const response = await fetchRelaciones(search)
      setData({
        actores: response.actores || [],
        lugares: response.lugares || [],
        circunstancias: response.circunstancias || [],
      })
    } catch (err) {
      setError(err.message || 'No se pudieron cargar las relaciones.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load('')
  }, [])

  const totals = useMemo(
    () => ({
      actores: data.actores.length,
      lugares: data.lugares.length,
      circunstancias: data.circunstancias.length,
    }),
    [data],
  )

  const currentItems = data[active] || []

  const handleSearch = (event) => {
    event.preventDefault()
    load(query)
  }

  return (
    <section className="relations-page">
      <div className="relations-hero">
        <div>
          <span className="relations-eyebrow">Analisis vincular</span>
          <h2 className="relations-title">Relaciones de datos</h2>
          <p className="relations-subtitle">
            Consulta actores, lugares y circunstancias sin abrir cada declaracion.
          </p>
        </div>

        <form className="relations-search" onSubmit={handleSearch}>
          <input
            className="relations-search__input"
            type="search"
            placeholder="Buscar actor, lugar, AP, declarante..."
            value={query}
            onChange={(event) => setQuery(event.target.value)}
          />
          <button className="btn btn--primary" type="submit">
            Buscar
          </button>
        </form>
      </div>

      <div className="relations-tabs">
        {sections.map((section) => (
          <button
            className={`relations-tab ${active === section.key ? 'is-active' : ''}`}
            type="button"
            key={section.key}
            onClick={() => setActive(section.key)}
          >
            <span>{section.label}</span>
            <strong>{totals[section.key]}</strong>
          </button>
        ))}
      </div>

      {error && <p className="relations-status relations-status--error">{error}</p>}
      {loading && <p className="relations-status">Cargando relaciones...</p>}

      {!loading && currentItems.length === 0 && (
        <div className="relations-empty">
          <h3>Aun no hay relaciones capturadas</h3>
          <p>
            Agrega actores, lugares o circunstancias desde una declaracion para que
            aparezcan aqui agrupados.
          </p>
        </div>
      )}

      <div className="relations-list">
        {currentItems.map((item) => (
          <article className="relation-card" key={item.id}>
            <div className="relation-card__summary">
              <div>
                <span className="relation-card__type">{item.tipo}</span>
                <h3 className="relation-card__name">{item.nombre}</h3>
              </div>
              <div className="relation-card__metrics">
                <span>{item.totalRegistros} registros</span>
                <span>{item.totalDeclaraciones} declaraciones</span>
              </div>
            </div>

            <div className="relation-table">
              <div className="relation-table__head">
                <span>No.</span>
                <span>AP</span>
                <span>Fecha</span>
                <span>Declarante</span>
                <span>Resumen</span>
                <span>Accion</span>
              </div>

              {(item.coincidencias || []).map((match) => (
                <div className="relation-table__row" key={match.declaracionId}>
                  <span>{match.numero || '-'}</span>
                  <span>{match.ap || '-'}</span>
                  <span>{formatDate(match.fecha)}</span>
                  <span>{match.declarante || '-'}</span>
                  <span>{match.textoResumen || '-'}</span>
                  <span>
                    <button
                      className="btn btn--ghost"
                      type="button"
                      onClick={() => onEdit?.(match.registroId, match.declaracionId)}
                    >
                      Abrir
                    </button>
                  </span>
                </div>
              ))}
            </div>
          </article>
        ))}
      </div>
    </section>
  )
}

export default RelationsPage
