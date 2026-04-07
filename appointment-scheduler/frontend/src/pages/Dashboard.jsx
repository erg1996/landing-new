import { useBusiness } from '../components/BusinessContext'
import { Link } from 'react-router-dom'
import { useEffect, useState } from 'react'
import { getDashboardAnalytics } from '../api/client'

export default function Dashboard() {
  const { business, services, appointments } = useBusiness()
  const [analytics, setAnalytics] = useState(null)

  useEffect(() => {
    if (business?.id) {
      getDashboardAnalytics(business.id).then(setAnalytics).catch(() => {})
    }
  }, [business?.id, appointments.length])

  if (!business) {
    return (
      <div className="flex flex-col items-center justify-center py-20">
        <div className="text-6xl mb-6">🏪</div>
        <h1 className="text-2xl font-bold text-gray-800 mb-2">Bienvenido a SchedulePro</h1>
        <p className="text-gray-500 mb-6">Configura tu negocio para comenzar a recibir citas</p>
        <Link to="/business" className="bg-indigo-600 text-white px-6 py-3 rounded-lg font-medium hover:bg-indigo-700 transition-colors">
          Crear Negocio
        </Link>
      </div>
    )
  }

  // Exclude cancelled from frontend counts
  const active = appointments.filter((a) => a.status !== 'Cancelled')
  const cancelled = appointments.filter((a) => a.status === 'Cancelled')

  const today = new Date().toISOString().split('T')[0]
  const todayActive = active.filter((a) => a.appointmentDate.split('T')[0] === today)
  const upcoming = active
    .filter((a) => new Date(a.appointmentDate) >= new Date())
    .sort((a, b) => new Date(a.appointmentDate) - new Date(b.appointmentDate))
    .slice(0, 5)

  const stats = [
    { label: 'Servicios Activos', value: services.length, icon: '⚙️', color: 'bg-blue-50 text-blue-700' },
    { label: 'Citas Activas', value: analytics?.totalAppointments ?? active.length, icon: '📅', color: 'bg-green-50 text-green-700' },
    { label: 'Citas Hoy', value: analytics?.todayAppointments ?? todayActive.length, icon: '📌', color: 'bg-amber-50 text-amber-700' },
    { label: 'Canceladas', value: analytics?.cancelledAppointments ?? cancelled.length, icon: '🚫', color: 'bg-red-50 text-red-600' },
  ]

  const formatDateTime = (iso) => {
    const d = new Date(iso)
    return d.toLocaleDateString('es', { day: 'numeric', month: 'short', hour: '2-digit', minute: '2-digit' })
  }

  const formatHour = (hour) => {
    const h = hour % 12 || 12
    const ampm = hour < 12 ? 'AM' : 'PM'
    return `${h}:00 ${ampm}`
  }

  const STATUS_BADGE = {
    Pending: 'bg-yellow-50 text-yellow-700',
    Confirmed: 'bg-blue-50 text-blue-700',
    Completed: 'bg-green-50 text-green-700',
  }

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-800">{business.name}</h1>
        <p className="text-gray-500">Panel de control</p>
      </div>

      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        {stats.map((s) => (
          <div key={s.label} className={`${s.color} rounded-xl p-5 text-center`}>
            <div className="text-2xl mb-1">{s.icon}</div>
            <div className="text-3xl font-bold">{s.value}</div>
            <div className="text-sm mt-1 opacity-75">{s.label}</div>
          </div>
        ))}
      </div>

      {analytics && (analytics.topService || analytics.busiestHour || analytics.quietestHour) && (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-8">
          {analytics.topService && (
            <div className="bg-white rounded-xl border border-gray-200 p-5">
              <div className="text-sm text-gray-500 mb-1">Servicio con mas citas</div>
              <div className="text-xl font-bold text-gray-800">{analytics.topService.name}</div>
              <div className="text-sm text-indigo-600 font-medium mt-1">
                {analytics.topService.count} cita{analytics.topService.count !== 1 ? 's' : ''}
              </div>
            </div>
          )}
          {analytics.busiestHour && (
            <div className="bg-white rounded-xl border border-gray-200 p-5">
              <div className="text-sm text-gray-500 mb-1">Hora mas agendada</div>
              <div className="text-xl font-bold text-gray-800">{formatHour(analytics.busiestHour.hour)}</div>
              <div className="text-sm text-green-600 font-medium mt-1">
                {analytics.busiestHour.count} cita{analytics.busiestHour.count !== 1 ? 's' : ''}
              </div>
            </div>
          )}
          {analytics.quietestHour && (
            <div className="bg-white rounded-xl border border-gray-200 p-5">
              <div className="text-sm text-gray-500 mb-1">Hora menos agendada</div>
              <div className="text-xl font-bold text-gray-800">{formatHour(analytics.quietestHour.hour)}</div>
              <div className="text-sm text-amber-600 font-medium mt-1">
                {analytics.quietestHour.count} cita{analytics.quietestHour.count !== 1 ? 's' : ''}
              </div>
            </div>
          )}
        </div>
      )}

      <div className="bg-white rounded-xl border border-gray-200 p-6">
        <h2 className="text-lg font-semibold text-gray-800 mb-4">Proximas Citas</h2>
        {upcoming.length === 0 ? (
          <p className="text-gray-400 text-center py-6">No hay citas proximas</p>
        ) : (
          <div className="divide-y divide-gray-100">
            {upcoming.map((a) => (
              <div key={a.id} className="flex items-center justify-between py-3">
                <div>
                  <span className="font-medium text-gray-800">{a.customerName}</span>
                  <span className="text-gray-400 mx-2">·</span>
                  <span className="text-sm text-gray-500">{a.durationMinutes} min</span>
                </div>
                <div className="flex items-center gap-2">
                  <span className={`text-xs px-2 py-0.5 rounded-full ${STATUS_BADGE[a.status] ?? 'bg-gray-100 text-gray-500'}`}>
                    {a.status === 'Pending' ? 'Pendiente' : a.status === 'Confirmed' ? 'Confirmada' : a.status}
                  </span>
                  <span className="text-sm text-gray-500">{formatDateTime(a.appointmentDate)}</span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}
