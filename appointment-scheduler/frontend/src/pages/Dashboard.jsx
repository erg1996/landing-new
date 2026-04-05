import { useBusiness } from '../components/BusinessContext'
import { Link } from 'react-router-dom'

export default function Dashboard() {
  const { business, services, appointments } = useBusiness()

  if (!business) {
    return (
      <div className="flex flex-col items-center justify-center py-20">
        <div className="text-6xl mb-6">🏪</div>
        <h1 className="text-2xl font-bold text-gray-800 mb-2">Bienvenido a SchedulePro</h1>
        <p className="text-gray-500 mb-6">
          Configura tu negocio para comenzar a recibir citas
        </p>
        <Link
          to="/business"
          className="bg-indigo-600 text-white px-6 py-3 rounded-lg font-medium hover:bg-indigo-700 transition-colors"
        >
          Crear Negocio
        </Link>
      </div>
    )
  }

  const today = new Date().toISOString().split('T')[0]
  const todayAppointments = appointments.filter(
    (a) => a.appointmentDate.split('T')[0] === today
  )
  const upcoming = appointments
    .filter((a) => new Date(a.appointmentDate) >= new Date())
    .sort((a, b) => new Date(a.appointmentDate) - new Date(b.appointmentDate))
    .slice(0, 5)

  const stats = [
    { label: 'Servicios Activos', value: services.length, icon: '⚙️', color: 'bg-blue-50 text-blue-700' },
    { label: 'Citas Totales', value: appointments.length, icon: '📅', color: 'bg-green-50 text-green-700' },
    { label: 'Citas Hoy', value: todayAppointments.length, icon: '📌', color: 'bg-amber-50 text-amber-700' },
    { label: 'Próximas', value: upcoming.length, icon: '⏳', color: 'bg-purple-50 text-purple-700' },
  ]

  const formatDateTime = (iso) => {
    const d = new Date(iso)
    return d.toLocaleDateString('es', {
      day: 'numeric',
      month: 'short',
      hour: '2-digit',
      minute: '2-digit',
    })
  }

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-800">{business.name}</h1>
        <p className="text-gray-500">Panel de control</p>
      </div>

      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        {stats.map((s) => (
          <div
            key={s.label}
            className={`${s.color} rounded-xl p-5 text-center`}
          >
            <div className="text-2xl mb-1">{s.icon}</div>
            <div className="text-3xl font-bold">{s.value}</div>
            <div className="text-sm mt-1 opacity-75">{s.label}</div>
          </div>
        ))}
      </div>

      <div className="bg-white rounded-xl border border-gray-200 p-6">
        <h2 className="text-lg font-semibold text-gray-800 mb-4">Próximas Citas</h2>
        {upcoming.length === 0 ? (
          <p className="text-gray-400 text-center py-6">No hay citas próximas</p>
        ) : (
          <div className="divide-y divide-gray-100">
            {upcoming.map((a) => (
              <div key={a.id} className="flex items-center justify-between py-3">
                <div>
                  <span className="font-medium text-gray-800">{a.customerName}</span>
                  <span className="text-gray-400 mx-2">·</span>
                  <span className="text-sm text-gray-500">{a.durationMinutes} min</span>
                </div>
                <span className="text-sm text-gray-500">{formatDateTime(a.appointmentDate)}</span>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}
