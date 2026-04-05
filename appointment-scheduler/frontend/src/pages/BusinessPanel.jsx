import { useState, useEffect } from 'react'
import { useBusiness } from '../components/BusinessContext'
import {
  createBusiness,
  createService,
  createWorkingHours,
  getAllBusinesses,
} from '../api/client'

const DAY_NAMES = ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado']

export default function BusinessPanel() {
  const { business, setBusiness, services, refreshServices } = useBusiness()
  const [tab, setTab] = useState('info')
  const [showSwitcher, setShowSwitcher] = useState(false)

  if (!business) {
    return <BusinessSelector onSelected={setBusiness} />
  }

  if (showSwitcher) {
    return <BusinessSelector onSelected={(b) => { setBusiness(b); setShowSwitcher(false) }} currentId={business.id} />
  }

  const tabs = [
    { key: 'info', label: 'Negocio' },
    { key: 'services', label: 'Servicios' },
    { key: 'hours', label: 'Horarios' },
  ]

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-800">{business.name}</h1>
        <button
          onClick={() => setShowSwitcher(true)}
          className="text-sm text-indigo-600 hover:text-indigo-800 font-medium"
        >
          Cambiar negocio
        </button>
      </div>

      <div className="flex gap-1 mb-6 bg-gray-100 p-1 rounded-lg w-fit">
        {tabs.map((t) => (
          <button
            key={t.key}
            onClick={() => setTab(t.key)}
            className={`px-4 py-2 rounded-md text-sm font-medium transition-colors ${
              tab === t.key
                ? 'bg-white text-indigo-700 shadow-sm'
                : 'text-gray-500 hover:text-gray-700'
            }`}
          >
            {t.label}
          </button>
        ))}
      </div>

      {tab === 'info' && <BusinessInfo business={business} />}
      {tab === 'services' && (
        <ServicesTab
          businessId={business.id}
          services={services}
          onRefresh={refreshServices}
        />
      )}
      {tab === 'hours' && <WorkingHoursTab businessId={business.id} />}
    </div>
  )
}

function BusinessSelector({ onSelected, currentId }) {
  const [businesses, setBusinesses] = useState([])
  const [loading, setLoading] = useState(true)
  const [showCreate, setShowCreate] = useState(false)

  useEffect(() => {
    loadBusinesses()
  }, [])

  const loadBusinesses = async () => {
    try {
      const data = await getAllBusinesses()
      setBusinesses(data)
      if (data.length === 0) setShowCreate(true)
    } catch {
      setShowCreate(true)
    } finally {
      setLoading(false)
    }
  }

  if (loading) {
    return <p className="text-center text-gray-400 py-12">Cargando negocios...</p>
  }

  if (showCreate) {
    return (
      <div>
        {businesses.length > 0 && (
          <button
            onClick={() => setShowCreate(false)}
            className="text-sm text-indigo-600 hover:underline mb-4"
          >
            Ver negocios existentes
          </button>
        )}
        <CreateBusinessForm onCreated={(b) => { onSelected(b); loadBusinesses() }} />
      </div>
    )
  }

  return (
    <div className="max-w-md mx-auto py-12">
      <h1 className="text-2xl font-bold text-gray-800 mb-6 text-center">Seleccionar Negocio</h1>
      <div className="space-y-2 mb-6">
        {businesses.map((b) => (
          <button
            key={b.id}
            onClick={() => onSelected(b)}
            className={`w-full bg-white border rounded-xl p-4 text-left transition-colors ${
              b.id === currentId
                ? 'border-indigo-400 bg-indigo-50'
                : 'border-gray-200 hover:border-indigo-300 hover:bg-indigo-50'
            }`}
          >
            <div className="flex justify-between items-center">
              <div>
                <span className="font-medium text-gray-800">{b.name}</span>
                <span className="text-gray-400 text-sm ml-2">/{b.slug}</span>
              </div>
              {b.id === currentId && (
                <span className="text-xs bg-indigo-100 text-indigo-700 px-2 py-1 rounded-full">Actual</span>
              )}
            </div>
          </button>
        ))}
      </div>
      <button
        onClick={() => setShowCreate(true)}
        className="w-full border-2 border-dashed border-gray-300 rounded-xl p-4 text-gray-500 hover:border-indigo-400 hover:text-indigo-600 transition-colors text-sm font-medium"
      >
        + Crear nuevo negocio
      </button>
    </div>
  )
}

function CreateBusinessForm({ onCreated }) {
  const [name, setName] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (e) => {
    e.preventDefault()
    if (!name.trim()) return
    setLoading(true)
    setError('')
    try {
      const result = await createBusiness({ name: name.trim() })
      onCreated(result)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="max-w-md mx-auto py-12">
      <h1 className="text-2xl font-bold text-gray-800 mb-6 text-center">Crear Negocio</h1>
      <form onSubmit={handleSubmit} className="bg-white rounded-xl border border-gray-200 p-6">
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Nombre del negocio
        </label>
        <input
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          placeholder="Ej: Mi Barbería"
          className="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none"
        />
        {error && <p className="text-red-500 text-sm mt-2">{error}</p>}
        <button
          type="submit"
          disabled={loading || !name.trim()}
          className="w-full mt-4 bg-indigo-600 text-white py-2.5 rounded-lg font-medium hover:bg-indigo-700 disabled:opacity-50 transition-colors"
        >
          {loading ? 'Creando...' : 'Crear Negocio'}
        </button>
      </form>
    </div>
  )
}

function BusinessInfo({ business }) {
  const publicUrl = `${window.location.origin}/book/${business.slug}`
  const [copied, setCopied] = useState(false)

  const copyLink = async () => {
    await navigator.clipboard.writeText(publicUrl)
    setCopied(true)
    setTimeout(() => setCopied(false), 2000)
  }

  return (
    <div className="space-y-6">
      {/* Public booking link */}
      <div className="bg-indigo-50 border border-indigo-200 rounded-xl p-5">
        <h2 className="font-semibold text-indigo-800 mb-2">Link de Reserva para Clientes</h2>
        <p className="text-indigo-600 text-sm mb-3">
          Comparte este link para que tus clientes agenden citas:
        </p>
        <div className="flex gap-2">
          <input
            type="text"
            value={publicUrl}
            readOnly
            className="flex-1 bg-white border border-indigo-200 rounded-lg px-3 py-2 text-sm text-gray-700 font-mono"
          />
          <button
            onClick={copyLink}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
              copied
                ? 'bg-green-600 text-white'
                : 'bg-indigo-600 text-white hover:bg-indigo-700'
            }`}
          >
            {copied ? 'Copiado!' : 'Copiar'}
          </button>
        </div>
      </div>

      <div className="bg-white rounded-xl border border-gray-200 p-6">
        <h2 className="font-semibold text-gray-800 mb-4">Información del Negocio</h2>
        <div className="space-y-3 text-sm">
          <div className="flex justify-between py-2 border-b border-gray-100">
            <span className="text-gray-500">ID</span>
            <span className="font-mono text-gray-700 text-xs">{business.id}</span>
          </div>
          <div className="flex justify-between py-2 border-b border-gray-100">
            <span className="text-gray-500">Nombre</span>
            <span className="text-gray-800 font-medium">{business.name}</span>
          </div>
          <div className="flex justify-between py-2 border-b border-gray-100">
            <span className="text-gray-500">Slug</span>
            <span className="font-mono text-gray-700">{business.slug}</span>
          </div>
          <div className="flex justify-between py-2">
            <span className="text-gray-500">Creado</span>
            <span className="text-gray-700">
              {new Date(business.createdAt).toLocaleDateString('es')}
            </span>
          </div>
        </div>
      </div>
    </div>
  )
}

function ServicesTab({ businessId, services, onRefresh }) {
  const [name, setName] = useState('')
  const [duration, setDuration] = useState(30)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (e) => {
    e.preventDefault()
    if (!name.trim()) return
    setLoading(true)
    setError('')
    try {
      await createService({
        businessId,
        name: name.trim(),
        durationMinutes: Number(duration),
      })
      setName('')
      setDuration(30)
      onRefresh()
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="space-y-6">
      <div className="bg-white rounded-xl border border-gray-200 p-6">
        <h2 className="font-semibold text-gray-800 mb-4">Agregar Servicio</h2>
        <form onSubmit={handleSubmit} className="flex gap-3 items-end flex-wrap">
          <div className="flex-1 min-w-[200px]">
            <label className="block text-sm font-medium text-gray-700 mb-1">Nombre</label>
            <input
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="Ej: Corte de cabello"
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none"
            />
          </div>
          <div className="w-32">
            <label className="block text-sm font-medium text-gray-700 mb-1">Duración (min)</label>
            <input
              type="number"
              value={duration}
              onChange={(e) => setDuration(e.target.value)}
              min="5"
              step="5"
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none"
            />
          </div>
          <button
            type="submit"
            disabled={loading || !name.trim()}
            className="bg-indigo-600 text-white px-5 py-2 rounded-lg text-sm font-medium hover:bg-indigo-700 disabled:opacity-50 transition-colors"
          >
            {loading ? '...' : 'Agregar'}
          </button>
        </form>
        {error && <p className="text-red-500 text-sm mt-2">{error}</p>}
      </div>

      <div className="bg-white rounded-xl border border-gray-200 p-6">
        <h2 className="font-semibold text-gray-800 mb-4">
          Servicios ({services.length})
        </h2>
        {services.length === 0 ? (
          <p className="text-gray-400 text-center py-4">No hay servicios registrados</p>
        ) : (
          <div className="divide-y divide-gray-100">
            {services.map((s) => (
              <div key={s.id} className="flex items-center justify-between py-3">
                <span className="font-medium text-gray-800">{s.name}</span>
                <span className="bg-indigo-50 text-indigo-700 px-3 py-1 rounded-full text-sm">
                  {s.durationMinutes} min
                </span>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}

function WorkingHoursTab({ businessId }) {
  const [day, setDay] = useState(1)
  const [start, setStart] = useState('09:00')
  const [end, setEnd] = useState('17:00')
  const [loading, setLoading] = useState(false)
  const [message, setMessage] = useState({ type: '', text: '' })
  const [saved, setSaved] = useState([])

  const handleSubmit = async (e) => {
    e.preventDefault()
    setLoading(true)
    setMessage({ type: '', text: '' })
    try {
      const [sh, sm] = start.split(':').map(Number)
      const [eh, em] = end.split(':').map(Number)
      await createWorkingHours({
        businessId,
        dayOfWeek: Number(day),
        startTime: `${String(sh).padStart(2, '0')}:${String(sm).padStart(2, '0')}:00`,
        endTime: `${String(eh).padStart(2, '0')}:${String(em).padStart(2, '0')}:00`,
      })
      setSaved((prev) => [...prev, { day: Number(day), start, end }])
      setMessage({ type: 'success', text: `Horario de ${DAY_NAMES[day]} guardado` })
    } catch (err) {
      setMessage({ type: 'error', text: err.message })
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="space-y-6">
      <div className="bg-white rounded-xl border border-gray-200 p-6">
        <h2 className="font-semibold text-gray-800 mb-4">Definir Horario Laboral</h2>
        <form onSubmit={handleSubmit} className="flex gap-3 items-end flex-wrap">
          <div className="w-44">
            <label className="block text-sm font-medium text-gray-700 mb-1">Día</label>
            <select
              value={day}
              onChange={(e) => setDay(e.target.value)}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none"
            >
              {DAY_NAMES.map((name, i) => (
                <option key={i} value={i}>
                  {name}
                </option>
              ))}
            </select>
          </div>
          <div className="w-32">
            <label className="block text-sm font-medium text-gray-700 mb-1">Apertura</label>
            <input
              type="time"
              value={start}
              onChange={(e) => setStart(e.target.value)}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none"
            />
          </div>
          <div className="w-32">
            <label className="block text-sm font-medium text-gray-700 mb-1">Cierre</label>
            <input
              type="time"
              value={end}
              onChange={(e) => setEnd(e.target.value)}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none"
            />
          </div>
          <button
            type="submit"
            disabled={loading}
            className="bg-indigo-600 text-white px-5 py-2 rounded-lg text-sm font-medium hover:bg-indigo-700 disabled:opacity-50 transition-colors"
          >
            {loading ? '...' : 'Guardar'}
          </button>
        </form>
        {message.text && (
          <p
            className={`text-sm mt-3 ${
              message.type === 'success' ? 'text-green-600' : 'text-red-500'
            }`}
          >
            {message.text}
          </p>
        )}
      </div>

      {saved.length > 0 && (
        <div className="bg-white rounded-xl border border-gray-200 p-6">
          <h2 className="font-semibold text-gray-800 mb-4">Horarios Guardados</h2>
          <div className="divide-y divide-gray-100">
            {saved.map((s, i) => (
              <div key={i} className="flex items-center justify-between py-3">
                <span className="font-medium text-gray-800">{DAY_NAMES[s.day]}</span>
                <span className="text-sm text-gray-500">
                  {s.start} — {s.end}
                </span>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  )
}
