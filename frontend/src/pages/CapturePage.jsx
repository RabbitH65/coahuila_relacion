import RegistroForm from '../components/RegistroForm'
import { createRegistro } from '../services/api'

function CapturePage() {
  const handleSubmit = (payload) => createRegistro(payload)

  return (
    <RegistroForm
      onSubmit={handleSubmit}
      title="Captura de registros"
      subtitle="Pantalla base sin diseno final. Todas las clases son ganchos para diseno."
      submitLabel="Guardar registro"
      resetOnSuccess
    />
  )
}

export default CapturePage
