import { useEffect, useState } from 'react'
import RegistroForm from '../components/RegistroForm'
import { fetchRegistro, updateRegistro } from '../services/api'
import '../styles/list.css'

function EditPage({ registroId, onBack }) {
  const [registro, setRegistro] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    if (!registroId) return
    const load = async () => {
      setLoading(true)
      setError('')
      try {
        const data = await fetchRegistro(registroId)
        setRegistro(data)
      } catch (err) {
        setError(err.message || 'No se pudo cargar el registro.')
      } finally {
        setLoading(false)
      }
    }
    load()
  }, [registroId])

  const handleSubmit = (payload) => updateRegistro(registroId, payload)

  if (loading) {
    return <p className="list__status">Cargando registro...</p>
  }

  if (error) {
    return <p className="list__status list__status--error">{error}</p>
  }

  return (
    <RegistroForm
      initialData={registro}
      onSubmit={handleSubmit}
      onCancel={onBack}
      submitLabel="Guardar cambios"
      title="Edicion de registro"
      subtitle="Edita los datos y agrega nuevas coordenadas, fechas o imagenes."
      resetOnSuccess={false}
    />
  )
}

export default EditPage
