
import { useEffect, useMemo, useState } from 'react'
import { API_BASE, uploadDeclaracionImagen } from '../services/api'
import '../styles/capture.css'

const createId = () =>
  (typeof crypto !== 'undefined' && crypto.randomUUID
    ? crypto.randomUUID()
    : `${Date.now()}-${Math.random().toString(16).slice(2)}`)

const toInput = (value) => (value === null || value === undefined ? '' : String(value))

const toDateInput = (value) => {
  if (!value) return ''
  if (typeof value === 'string') return value.slice(0, 10)
  return new Date(value).toISOString().slice(0, 10)
}

const createRegistroState = () => ({
  numero: '',
  ap: '',
  tomo: '',
  foja: '',
  fecha: '',
  entrevistado: '',
  declarante: '',
  observaciones: '',
})

const createCoordenada = () => ({
  id: createId(),
  latitud: '',
  longitud: '',
  precision: '',
  fuente: '',
  observacion: '',
})

const createFecha = () => ({
  id: createId(),
  fecha: '',
  tipoFecha: '',
  observacion: '',
})

const createImagen = () => ({
  id: createId(),
  archivo: null,
  descripcion: '',
  elementoRelevante: '',
  tipoImagen: '',
  existingUrl: '',
})

const createDeclaracion = () => ({
  id: createId(),
  texto: '',
  tipo: '',
  fechaCaptura: '',
  observaciones: '',
  coordenadas: [createCoordenada()],
  fechas: [createFecha()],
  imagenes: [createImagen()],
})

const normalizeRegistro = (data) => {
  if (!data) return createRegistroState()
  return {
    numero: toInput(data.numero),
    ap: toInput(data.ap),
    tomo: toInput(data.tomo),
    foja: toInput(data.foja),
    fecha: toDateInput(data.fecha),
    entrevistado: toInput(data.entrevistado),
    declarante: toInput(data.declarante),
    observaciones: toInput(data.observaciones),
  }
}

const normalizeDeclaraciones = (data) => {
  if (!data?.declaraciones?.length) return [createDeclaracion()]

  return data.declaraciones.map((declaracion) => {
    const coords = declaracion.coordenadas?.length
      ? declaracion.coordenadas.map((coord) => ({
          id: coord.id || createId(),
          latitud: toInput(coord.latitud),
          longitud: toInput(coord.longitud),
          precision: toInput(coord.precisionM),
          fuente: toInput(coord.fuente),
          observacion: toInput(coord.observacion),
        }))
      : [createCoordenada()]

    const fechas = declaracion.fechas?.length
      ? declaracion.fechas.map((fecha) => ({
          id: fecha.id || createId(),
          fecha: toDateInput(fecha.fecha),
          tipoFecha: toInput(fecha.tipoFecha),
          observacion: toInput(fecha.observacion),
        }))
      : [createFecha()]

    const imagenes = declaracion.imagenes?.length
      ? declaracion.imagenes.map((imagen) => ({
          id: imagen.id || createId(),
          archivo: null,
          descripcion: toInput(imagen.descripcion),
          elementoRelevante: toInput(imagen.elementoRelevante),
          tipoImagen: toInput(imagen.tipoImagen),
          existingUrl: imagen.rutaArchivo ? `${API_BASE}/${imagen.rutaArchivo}` : '',
        }))
      : [createImagen()]

    return {
      id: declaracion.id || createId(),
      texto: toInput(declaracion.textoDeclaracion),
      tipo: toInput(declaracion.tipo),
      fechaCaptura: toDateInput(declaracion.fechaCaptura),
      observaciones: toInput(declaracion.observaciones),
      coordenadas: coords,
      fechas,
      imagenes,
    }
  })
}

const buildRegistroPayload = (registro, declaraciones) => ({
  numero: registro.numero || null,
  ap: registro.ap || null,
  tomo: registro.tomo || null,
  foja: registro.foja || null,
  fecha: registro.fecha || null,
  entrevistado: registro.entrevistado || null,
  declarante: registro.declarante || null,
  observaciones: registro.observaciones || null,
  declaraciones: declaraciones.map((declaracion, index) => ({
    id: declaracion.id || null,
    textoDeclaracion: declaracion.texto || null,
    orden: index + 1,
    tipo: declaracion.tipo || null,
    fechaCaptura: declaracion.fechaCaptura || null,
    observaciones: declaracion.observaciones || null,
    coordenadas: declaracion.coordenadas
      .filter((coord) => coord.latitud !== '' && coord.longitud !== '')
      .map((coord) => ({
        latitud: Number(coord.latitud),
        longitud: Number(coord.longitud),
        precisionM: coord.precision ? Number(coord.precision) : null,
        fuente: coord.fuente || null,
        observacion: coord.observacion || null,
      })),
    fechas: declaracion.fechas
      .filter((fecha) => fecha.fecha)
      .map((fecha) => ({
        fecha: fecha.fecha,
        tipoFecha: fecha.tipoFecha || null,
        observacion: fecha.observacion || null,
      })),
  })),
})

function RegistroForm({
  initialData,
  onSubmit,
  onCancel,
  submitLabel = 'Guardar registro',
  title = 'Captura de registros',
  subtitle = 'Pantalla base sin diseno final. Todas las clases son ganchos para diseno.',
  resetOnSuccess = true,
}) {
  const [registro, setRegistro] = useState(createRegistroState)
  const [declaraciones, setDeclaraciones] = useState([createDeclaracion()])
  const [status, setStatus] = useState({ type: '', message: '' })
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    setRegistro(normalizeRegistro(initialData))
    setDeclaraciones(normalizeDeclaraciones(initialData))
  }, [initialData])

  const handleRegistroChange = (field) => (event) => {
    setRegistro((prev) => ({ ...prev, [field]: event.target.value }))
  }

  const handleReset = () => {
    setRegistro(createRegistroState())
    setDeclaraciones([createDeclaracion()])
    setStatus({ type: '', message: '' })
  }

  const addDeclaracion = () => {
    setDeclaraciones((prev) => [...prev, createDeclaracion()])
  }

  const removeDeclaracion = (id) => {
    setDeclaraciones((prev) => prev.filter((item) => item.id !== id))
  }

  const updateDeclaracion = (id, field, value) => {
    setDeclaraciones((prev) =>
      prev.map((item) => (item.id === id ? { ...item, [field]: value } : item)),
    )
  }

  const addCoordenada = (declaracionId) => {
    setDeclaraciones((prev) =>
      prev.map((item) =>
        item.id === declaracionId
          ? { ...item, coordenadas: [...item.coordenadas, createCoordenada()] }
          : item,
      ),
    )
  }

  const updateCoordenada = (declaracionId, coordId, field, value) => {
    setDeclaraciones((prev) =>
      prev.map((item) =>
        item.id === declaracionId
          ? {
              ...item,
              coordenadas: item.coordenadas.map((coord) =>
                coord.id === coordId ? { ...coord, [field]: value } : coord,
              ),
            }
          : item,
      ),
    )
  }

  const removeCoordenada = (declaracionId, coordId) => {
    setDeclaraciones((prev) =>
      prev.map((item) =>
        item.id === declaracionId
          ? {
              ...item,
              coordenadas: item.coordenadas.filter((coord) => coord.id !== coordId),
            }
          : item,
      ),
    )
  }

  const addFecha = (declaracionId) => {
    setDeclaraciones((prev) =>
      prev.map((item) =>
        item.id === declaracionId
          ? { ...item, fechas: [...item.fechas, createFecha()] }
          : item,
      ),
    )
  }

  const updateFecha = (declaracionId, fechaId, field, value) => {
    setDeclaraciones((prev) =>
      prev.map((item) =>
        item.id === declaracionId
          ? {
              ...item,
              fechas: item.fechas.map((fecha) =>
                fecha.id === fechaId ? { ...fecha, [field]: value } : fecha,
              ),
            }
          : item,
      ),
    )
  }

  const removeFecha = (declaracionId, fechaId) => {
    setDeclaraciones((prev) =>
      prev.map((item) =>
        item.id === declaracionId
          ? { ...item, fechas: item.fechas.filter((fecha) => fecha.id !== fechaId) }
          : item,
      ),
    )
  }

  const addImagen = (declaracionId) => {
    setDeclaraciones((prev) =>
      prev.map((item) =>
        item.id === declaracionId
          ? { ...item, imagenes: [...item.imagenes, createImagen()] }
          : item,
      ),
    )
  }

  const updateImagen = (declaracionId, imagenId, field, value) => {
    setDeclaraciones((prev) =>
      prev.map((item) =>
        item.id === declaracionId
          ? {
              ...item,
              imagenes: item.imagenes.map((imagen) =>
                imagen.id === imagenId ? { ...imagen, [field]: value } : imagen,
              ),
            }
          : item,
      ),
    )
  }

  const removeImagen = (declaracionId, imagenId) => {
    setDeclaraciones((prev) =>
      prev.map((item) =>
        item.id === declaracionId
          ? {
              ...item,
              imagenes: item.imagenes.filter((imagen) => imagen.id !== imagenId),
            }
          : item,
      ),
    )
  }

  const validateForm = () => {
    const errors = []

    if (!registro.numero.trim()) {
      errors.push('El campo No. es obligatorio.')
    }
    if (!registro.fecha) {
      errors.push('La fecha es obligatoria.')
    }
    if (!registro.declarante.trim()) {
      errors.push('El declarante es obligatorio.')
    }

    declaraciones.forEach((declaracion, index) => {
      if (!declaracion.texto.trim()) {
        errors.push(`Declaracion ${index + 1}: el texto es obligatorio.`)
      }

      declaracion.coordenadas.forEach((coord, coordIndex) => {
        const hasLat = coord.latitud !== ''
        const hasLng = coord.longitud !== ''
        if (hasLat || hasLng) {
          if (!hasLat || !hasLng) {
            errors.push(
              `Declaracion ${index + 1}: coordenada ${coordIndex + 1} requiere latitud y longitud.`,
            )
            return
          }
          const lat = Number(coord.latitud)
          const lng = Number(coord.longitud)
          if (Number.isNaN(lat) || lat < -90 || lat > 90) {
            errors.push(
              `Declaracion ${index + 1}: coordenada ${coordIndex + 1} tiene latitud invalida.`,
            )
          }
          if (Number.isNaN(lng) || lng < -180 || lng > 180) {
            errors.push(
              `Declaracion ${index + 1}: coordenada ${coordIndex + 1} tiene longitud invalida.`,
            )
          }
        }
      })

      declaracion.fechas.forEach((fecha, fechaIndex) => {
        const hasMeta = fecha.tipoFecha || fecha.observacion
        if (hasMeta && !fecha.fecha) {
          errors.push(
            `Declaracion ${index + 1}: fecha ${fechaIndex + 1} requiere una fecha.`,
          )
        }
      })

      declaracion.imagenes.forEach((imagen, imagenIndex) => {
        const hasMeta = imagen.descripcion || imagen.elementoRelevante || imagen.tipoImagen
        if (hasMeta && !imagen.archivo && !imagen.existingUrl) {
          errors.push(
            `Declaracion ${index + 1}: imagen ${imagenIndex + 1} requiere archivo.`,
          )
        }
      })
    })

    return errors
  }

  const uploadImages = async (createdDeclaraciones) => {
    const uploads = []

    declaraciones.forEach((declaracion, index) => {
      const created = createdDeclaraciones.find((item) => item.orden === index + 1)
      if (!created) return

      declaracion.imagenes
        .filter((imagen) => imagen.archivo)
        .forEach((imagen, imageIndex) => {
          const formData = new FormData()
          formData.append('Archivo', imagen.archivo)
          formData.append('Descripcion', imagen.descripcion || '')
          formData.append('ElementoRelevante', imagen.elementoRelevante || '')
          formData.append('TipoImagen', imagen.tipoImagen || '')
          formData.append('Orden', `${imageIndex + 1}`)
          uploads.push(uploadDeclaracionImagen(created.id, formData))
        })
    })

    if (uploads.length) {
      await Promise.all(uploads)
    }
  }

  const handleSubmit = async (event) => {
    event.preventDefault()
    if (saving) return

    const errors = validateForm()
    if (errors.length) {
      setStatus({ type: 'error', message: errors.join(' ') })
      return
    }

    setSaving(true)
    setStatus({ type: '', message: '' })

    try {
      const payload = buildRegistroPayload(registro, declaraciones)
      const created = await onSubmit(payload)
      await uploadImages(created.declaraciones || [])
      setStatus({ type: 'success', message: 'Registro guardado correctamente.' })
      if (resetOnSuccess) {
        handleReset()
      }
    } catch (err) {
      setStatus({
        type: 'error',
        message: err.message || 'No se pudo guardar el registro.',
      })
    } finally {
      setSaving(false)
    }
  }

  const declaracionesOrdenadas = useMemo(
    () => declaraciones.map((item, index) => ({ ...item, ordenVisual: index + 1 })),
    [declaraciones],
  )

  return (
    <main className="capture">
      <header className="capture__header">
        <h1 className="capture__title">{title}</h1>
        <p className="capture__subtitle">{subtitle}</p>
      </header>

      <form className="capture__form" onSubmit={handleSubmit} noValidate>
        <section className="capture__section">
          <div className="section-header">
            <h2 className="section-title">Datos del registro</h2>
            <div className="section-actions">
              <button className="btn btn--ghost" type="button" onClick={handleReset}>
                Limpiar
              </button>
            </div>
          </div>

          <div className="field-grid">
            <label className="field">
              <span className="field__label">No.</span>
              <input
                className="field__input"
                type="text"
                name="numero"
                value={registro.numero}
                onChange={handleRegistroChange('numero')}
              />
            </label>

            <label className="field">
              <span className="field__label">Ap</span>
              <input
                className="field__input"
                type="text"
                name="ap"
                value={registro.ap}
                onChange={handleRegistroChange('ap')}
              />
            </label>

            <label className="field">
              <span className="field__label">Tomo</span>
              <input
                className="field__input"
                type="text"
                name="tomo"
                value={registro.tomo}
                onChange={handleRegistroChange('tomo')}
              />
            </label>

            <label className="field">
              <span className="field__label">Foja</span>
              <input
                className="field__input"
                type="text"
                name="foja"
                value={registro.foja}
                onChange={handleRegistroChange('foja')}
              />
            </label>

            <label className="field">
              <span className="field__label">Fecha</span>
              <input
                className="field__input"
                type="date"
                name="fecha"
                value={registro.fecha}
                onChange={handleRegistroChange('fecha')}
              />
            </label>

            <label className="field">
              <span className="field__label">Entrevistado</span>
              <input
                className="field__input"
                type="text"
                name="entrevistado"
                value={registro.entrevistado}
                onChange={handleRegistroChange('entrevistado')}
              />
            </label>

            <label className="field">
              <span className="field__label">Declarante</span>
              <input
                className="field__input"
                type="text"
                name="declarante"
                value={registro.declarante}
                onChange={handleRegistroChange('declarante')}
              />
            </label>

            <label className="field field--full">
              <span className="field__label">Observaciones</span>
              <textarea
                className="field__textarea"
                name="observaciones"
                rows="4"
                value={registro.observaciones}
                onChange={handleRegistroChange('observaciones')}
              />
            </label>
          </div>
        </section>

        <section className="capture__section">
          <div className="section-header">
            <h2 className="section-title">Declaraciones</h2>
            <button className="btn btn--primary" type="button" onClick={addDeclaracion}>
              Agregar declaracion
            </button>
          </div>

          {declaracionesOrdenadas.map((declaracion) => (
            <article className="declaracion" key={declaracion.id}>
              <div className="declaracion__header">
                <h3 className="declaracion__title">Declaracion {declaracion.ordenVisual}</h3>
               <button
                className="btn btn--danger"
                type="button"
                onClick={() => removeDeclaracion(declaracion.id)}
              >
                🗑 Eliminar declaración
              </button>
              </div>

              <div className="field-grid">
                <label className="field field--full">
                  <span className="field__label">Texto de la declaracion</span>
                  <textarea
                    className="field__textarea"
                    rows="5"
                    value={declaracion.texto}
                    onChange={(event) =>
                      updateDeclaracion(declaracion.id, 'texto', event.target.value)
                    }
                  />
                </label>

                <label className="field">
                  <span className="field__label">Tipo</span>
                  <input
                    className="field__input"
                    type="text"
                    value={declaracion.tipo}
                    onChange={(event) =>
                      updateDeclaracion(declaracion.id, 'tipo', event.target.value)
                    }
                  />
                </label>

                <label className="field">
                  <span className="field__label">Fecha de captura</span>
                  <input
                    className="field__input"
                    type="date"
                    value={declaracion.fechaCaptura}
                    onChange={(event) =>
                      updateDeclaracion(
                        declaracion.id,
                        'fechaCaptura',
                        event.target.value,
                      )
                    }
                  />
                </label>

                <label className="field field--full">
                  <span className="field__label">Observaciones</span>
                  <input
                    className="field__input"
                    type="text"
                    value={declaracion.observaciones}
                    onChange={(event) =>
                      updateDeclaracion(
                        declaracion.id,
                        'observaciones',
                        event.target.value,
                      )
                    }
                  />
                </label>
              </div>

              <div className="subsection">
                <div className="subsection__header">
                  <h4 className="subsection__title">Coordenadas</h4>
                  <button
                    className="btn btn--secondary"
                    type="button"
                    onClick={() => addCoordenada(declaracion.id)}
                  >
                    Agregar coordenada
                  </button>
                </div>

                {declaracion.coordenadas.length === 0 ? (
                  <p className="empty">Sin coordenadas capturadas.</p>
                ) : (
                  declaracion.coordenadas.map((coord) => (
                    <div className="field-grid" key={coord.id}>
                      <label className="field">
                        <span className="field__label">Latitud</span>
                        <input
                          className="field__input"
                          type="number"
                          step="any"
                          value={coord.latitud}
                          onChange={(event) =>
                            updateCoordenada(
                              declaracion.id,
                              coord.id,
                              'latitud',
                              event.target.value,
                            )
                          }
                        />
                      </label>
                      <label className="field">
                        <span className="field__label">Longitud</span>
                        <input
                          className="field__input"
                          type="number"
                          step="any"
                          value={coord.longitud}
                          onChange={(event) =>
                            updateCoordenada(
                              declaracion.id,
                              coord.id,
                              'longitud',
                              event.target.value,
                            )
                          }
                        />
                      </label>
                      <label className="field">
                        <span className="field__label">Precision (m)</span>
                        <input
                          className="field__input"
                          type="number"
                          step="any"
                          value={coord.precision}
                          onChange={(event) =>
                            updateCoordenada(
                              declaracion.id,
                              coord.id,
                              'precision',
                              event.target.value,
                            )
                          }
                        />
                      </label>
                      <label className="field">
                        <span className="field__label">Fuente</span>
                        <input
                          className="field__input"
                          type="text"
                          value={coord.fuente}
                          onChange={(event) =>
                            updateCoordenada(
                              declaracion.id,
                              coord.id,
                              'fuente',
                              event.target.value,
                            )
                          }
                        />
                      </label>
                      <label className="field field--full">
                        <span className="field__label">Observacion</span>
                        <input
                          className="field__input"
                          type="text"
                          value={coord.observacion}
                          onChange={(event) =>
                            updateCoordenada(
                              declaracion.id,
                              coord.id,
                              'observacion',
                              event.target.value,
                            )
                          }
                        />
                      </label>
                      <div className="row-actions">
                        <button
                          className="btn btn--danger"
                          type="button"
                          onClick={() => removeCoordenada(declaracion.id, coord.id)}
                        >
                          Quitar coordenada
                        </button>
                      </div>
                    </div>
                  ))
                )}
              </div>

              <div className="subsection">
                <div className="subsection__header">
                  <h4 className="subsection__title">Fechas</h4>
                  <button
                    className="btn btn--secondary"
                    type="button"
                    onClick={() => addFecha(declaracion.id)}
                  >
                    Agregar fecha
                  </button>
                </div>

                {declaracion.fechas.length === 0 ? (
                  <p className="empty">Sin fechas capturadas.</p>
                ) : (
                  declaracion.fechas.map((fecha) => (
                    <div className="field-grid" key={fecha.id}>
                      <label className="field">
                        <span className="field__label">Fecha</span>
                        <input
                          className="field__input"
                          type="date"
                          value={fecha.fecha}
                          onChange={(event) =>
                            updateFecha(
                              declaracion.id,
                              fecha.id,
                              'fecha',
                              event.target.value,
                            )
                          }
                        />
                      </label>
                      <label className="field">
                        <span className="field__label">Tipo de fecha</span>
                        <input
                          className="field__input"
                          type="text"
                          value={fecha.tipoFecha}
                          onChange={(event) =>
                            updateFecha(
                              declaracion.id,
                              fecha.id,
                              'tipoFecha',
                              event.target.value,
                            )
                          }
                        />
                      </label>
                      <label className="field field--full">
                        <span className="field__label">Observacion</span>
                        <input
                          className="field__input"
                          type="text"
                          value={fecha.observacion}
                          onChange={(event) =>
                            updateFecha(
                              declaracion.id,
                              fecha.id,
                              'observacion',
                              event.target.value,
                            )
                          }
                        />
                      </label>
                      <div className="row-actions">
                        <button
                          className="btn btn--danger"
                          type="button"
                          onClick={() => removeFecha(declaracion.id, fecha.id)}
                        >
                          Quitar fecha
                        </button>
                      </div>
                    </div>
                  ))
                )}
              </div>

              <div className="subsection">
                <div className="subsection__header">
                  <h4 className="subsection__title">Imagenes</h4>
                  <button
                    className="btn btn--secondary"
                    type="button"
                    onClick={() => addImagen(declaracion.id)}
                  >
                    Agregar imagen
                  </button>
                </div>

                {declaracion.imagenes.length === 0 ? (
                  <p className="empty">Sin imagenes capturadas.</p>
                ) : (
                  declaracion.imagenes.map((imagen) => (
                    <div className="field-grid" key={imagen.id}>
                      <label className="field">
                        <span className="field__label">Archivo</span>
                        <input
                          className="field__input"
                          type="file"
                          onChange={(event) =>
                            updateImagen(
                              declaracion.id,
                              imagen.id,
                              'archivo',
                              event.target.files?.[0] ?? null,
                            )
                          }
                        />
                      </label>
                      <label className="field">
                        <span className="field__label">Descripcion</span>
                        <input
                          className="field__input"
                          type="text"
                          value={imagen.descripcion}
                          onChange={(event) =>
                            updateImagen(
                              declaracion.id,
                              imagen.id,
                              'descripcion',
                              event.target.value,
                            )
                          }
                        />
                      </label>
                      <label className="field">
                        <span className="field__label">Elemento relevante</span>
                        <input
                          className="field__input"
                          type="text"
                          value={imagen.elementoRelevante}
                          onChange={(event) =>
                            updateImagen(
                              declaracion.id,
                              imagen.id,
                              'elementoRelevante',
                              event.target.value,
                            )
                          }
                        />
                      </label>
                      <label className="field">
                        <span className="field__label">Tipo de imagen</span>
                        <input
                          className="field__input"
                          type="text"
                          value={imagen.tipoImagen}
                          onChange={(event) =>
                            updateImagen(
                              declaracion.id,
                              imagen.id,
                              'tipoImagen',
                              event.target.value,
                            )
                          }
                        />
                      </label>
                      {imagen.existingUrl && (
                        <p className="hint">Imagen actual: {imagen.existingUrl}</p>
                      )}
                      <div className="row-actions">
                        <button
                          className="btn btn--danger"
                          type="button"
                          onClick={() => removeImagen(declaracion.id, imagen.id)}
                        >
                          Quitar imagen
                        </button>
                      </div>
                    </div>
                  ))
                )}
              </div>
            </article>
          ))}
        </section>

        <section className="capture__section">
          <div className="section-header">
            <h2 className="section-title">Acciones</h2>
          </div>
          <div className="action-row">
            <button className="btn btn--primary" type="submit" disabled={saving}>
            {saving ? (
              <>
                <span className="spinner-btn"></span>
                Guardando...
              </>
            ) : (
              submitLabel
            )}
          </button>
            {onCancel && (
              <button className="btn btn--ghost" type="button" onClick={onCancel}>
                Cancelar
              </button>
            )}
          </div>
          {status.message && (
            <p className={`status status--${status.type}`}>{status.message}</p>
          )}
        </section>
      </form>
    </main>
  )
}

export default RegistroForm
