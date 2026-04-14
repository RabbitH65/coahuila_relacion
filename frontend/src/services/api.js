export const API_BASE = import.meta.env.VITE_API_BASE || 'http://localhost:5000'

const handleJson = async (response) => {
  if (!response.ok) {
    const message = await response.text()
    throw new Error(message || 'Error en la solicitud')
  }
  return response.json()
}

export const fetchRegistros = async (query = '') => {
  const url = new URL(`${API_BASE}/api/registros`)
  if (query) {
    url.searchParams.set('q', query)
  }
  const response = await fetch(url.toString())
  return handleJson(response)
}

export const fetchRegistro = async (id) => {
  const response = await fetch(`${API_BASE}/api/registros/${id}?includeDeclaraciones=true`)
  return handleJson(response)
}

export const fetchRelaciones = async (query = '') => {
  const url = new URL(`${API_BASE}/api/relaciones`)
  if (query) {
    url.searchParams.set('q', query)
  }
  const response = await fetch(url.toString())
  return handleJson(response)
}

export const fetchMapaPuntos = async (query = '') => {
  const url = new URL(`${API_BASE}/api/mapa/puntos`)
  if (query) {
    url.searchParams.set('q', query)
  }
  const response = await fetch(url.toString())
  return handleJson(response)
}

export const createRegistro = async (payload) => {
  const response = await fetch(`${API_BASE}/api/registros`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(payload),
  })
  return handleJson(response)
}

export const updateRegistro = async (id, payload) => {
  const response = await fetch(`${API_BASE}/api/registros/${id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(payload),
  })
  return handleJson(response)
}

export const deleteRegistro = async (id) => {
  const response = await fetch(`${API_BASE}/api/registros/${id}`, {
    method: 'DELETE',
  })

  if (!response.ok) {
    const message = await response.text()
    throw new Error(message || 'No se pudo eliminar el registro')
  }

  return true
}

export const uploadDeclaracionImagen = async (declaracionId, formData) => {
  const response = await fetch(`${API_BASE}/api/declaraciones/${declaracionId}/imagenes`, {
    method: 'POST',
    body: formData,
  })
  return handleJson(response)
}
