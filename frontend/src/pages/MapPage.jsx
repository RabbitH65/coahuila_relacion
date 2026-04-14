import { useEffect, useMemo, useRef, useState } from 'react'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import { API_BASE, fetchMapaPuntos } from '../services/api'
import '../styles/map.css'

const defaultCenter = [25.438, -100.973]

const formatDate = (value) => {
  if (!value) return '-'
  return value.slice(0, 10)
}

const getImageUrl = (ruta) => {
  if (!ruta) return ''
  return ruta.startsWith('http') ? ruta : `${API_BASE}/${ruta}`
}

function MapPage({ onEdit }) {
  const mapNodeRef = useRef(null)
  const mapRef = useRef(null)
  const markersRef = useRef(null)
  const [points, setPoints] = useState([])
  const [selected, setSelected] = useState(null)
  const [query, setQuery] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const totalImages = useMemo(
    () => points.reduce((sum, point) => sum + (point.imagenes?.length || 0), 0),
    [points],
  )

  const load = async (search = query) => {
    setLoading(true)
    setError('')
    try {
      const data = await fetchMapaPuntos(search)
      setPoints(data)
      setSelected(data[0] || null)
    } catch (err) {
      setError(err.message || 'No se pudieron cargar las coordenadas.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load('')
  }, [])

  useEffect(() => {
    if (mapRef.current || !mapNodeRef.current) return

    const map = L.map(mapNodeRef.current, {
      zoomControl: true,
      scrollWheelZoom: true,
    }).setView(defaultCenter, 6)

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '&copy; OpenStreetMap',
    }).addTo(map)

    markersRef.current = L.layerGroup().addTo(map)
    mapRef.current = map
  }, [])

  useEffect(() => {
    if (!mapRef.current || !markersRef.current) return

    markersRef.current.clearLayers()

    if (!points.length) {
      mapRef.current.setView(defaultCenter, 6)
      return
    }

    const bounds = []

    points.forEach((point) => {
      const latLng = [point.latitud, point.longitud]
      bounds.push(latLng)

      const marker = L.marker(latLng, {
        icon: L.divIcon({
          className: 'map-marker',
          html: '<span></span>',
          iconSize: [28, 28],
          iconAnchor: [14, 14],
        }),
      })

      marker.on('click', () => {
        setSelected(point)
      })

      marker.bindPopup(`
        <strong>${point.numero || `AP ${point.ap || '-'}`}</strong><br/>
        Declaracion ${point.declaracionOrden || '-'}<br/>
        ${point.declarante || 'Sin declarante'}
      `)

      marker.addTo(markersRef.current)
    })

    mapRef.current.fitBounds(bounds, { padding: [40, 40], maxZoom: 14 })
  }, [points])

  const handleSearch = (event) => {
    event.preventDefault()
    load(query)
  }

  const handleOpen = (registroId, declaracionId) => {
    if (registroId) {
      onEdit?.(registroId, declaracionId)
    }
  }

  return (
    <section className="map-page">
      <div className="map-hero">
        <div>
          <span className="map-eyebrow">Ubicaciones</span>
          <h2 className="map-title">Mapa de coordenadas</h2>
          <p className="map-subtitle">
            Consulta puntos capturados, imagenes asociadas y abre el registro desde el ojo.
          </p>
        </div>

        <form className="map-search" onSubmit={handleSearch}>
          <input
            className="map-search__input"
            type="search"
            placeholder="Buscar AP, declarante, texto..."
            value={query}
            onChange={(event) => setQuery(event.target.value)}
          />
          <button className="btn btn--primary" type="submit">
            Buscar
          </button>
        </form>
      </div>

      <div className="map-stats">
        <span>{points.length} coordenadas</span>
        <span>{totalImages} imagenes</span>
        <span>{selected ? `AP ${selected.ap || '-'}` : 'Sin seleccion'}</span>
      </div>

      {error && <p className="map-status map-status--error">{error}</p>}
      {loading && <p className="map-status">Cargando mapa...</p>}

      <div className="map-layout">
        <div className="map-canvas" ref={mapNodeRef} />

        <aside className="map-panel">
          {selected ? (
            <>
              <div className="map-panel__head">
                <div>
                  <span className="map-panel__eyebrow">
                    Declaracion {selected.declaracionOrden || '-'}
                  </span>
                  <h3>{selected.numero || `AP ${selected.ap || '-'}`}</h3>
                </div>
                <button
                  className="eye-button"
                  type="button"
                  title="Abrir registro"
                  onClick={() => handleOpen(selected.registroId, selected.declaracionId)}
                >
                  <span className="eye-button__icon" />
                  <span>Abrir</span>
                </button>
              </div>

              <dl className="map-detail">
                <div>
                  <dt>Fecha</dt>
                  <dd>{formatDate(selected.fecha)}</dd>
                </div>
                <div>
                  <dt>Declarante</dt>
                  <dd>{selected.declarante || '-'}</dd>
                </div>
                <div>
                  <dt>Coordenadas</dt>
                  <dd>
                    {selected.latitud}, {selected.longitud}
                  </dd>
                </div>
                <div>
                  <dt>Observacion</dt>
                  <dd>{selected.observacionCoordenada || '-'}</dd>
                </div>
              </dl>

              <p className="map-summary">{selected.textoResumen || 'Sin resumen.'}</p>

              <div className="map-images">
                <h4>Imagenes relacionadas</h4>
                {selected.imagenes?.length ? (
                  <div className="map-images__grid">
                    {selected.imagenes.map((image) => (
                      <figure className="map-image" key={image.id}>
                        <img src={getImageUrl(image.rutaArchivo)} alt={image.descripcion || 'Imagen relacionada'} />
                        <figcaption>{image.descripcion || image.tipoImagen || 'Sin descripcion'}</figcaption>
                      </figure>
                    ))}
                  </div>
                ) : (
                  <p className="map-empty">Sin imagenes cargadas para esta declaracion.</p>
                )}
              </div>
            </>
          ) : (
            <p className="map-empty">Selecciona un punto del mapa.</p>
          )}
        </aside>
      </div>

      <div className="map-point-list">
        {points.map((point) => (
          <button
            className={`map-point-row ${selected?.coordenadaId === point.coordenadaId ? 'is-active' : ''}`}
            type="button"
            key={point.coordenadaId}
            onClick={() => setSelected(point)}
          >
            <span>{point.numero || `AP ${point.ap || '-'}`}</span>
            <span>{point.declarante || '-'}</span>
            <span>{formatDate(point.fecha)}</span>
            <span className="eye-button eye-button--mini">
              <span className="eye-button__icon" />
            </span>
          </button>
        ))}
      </div>
    </section>
  )
}

export default MapPage
