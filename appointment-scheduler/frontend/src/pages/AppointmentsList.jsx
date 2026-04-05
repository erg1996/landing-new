import { useBusiness } from '../components/BusinessContext'
import { Link } from 'react-router-dom'

export default function AppointmentsList() {
  const { business, appointments, services, refreshAppointments } = useBusiness()

  if (!business) {
    return (
      <div className="flex flex-col items-center justify-center py-20">
        <div className="text-5xl mb-4">📋</div>
        <p className="text-gray-500 mb-4">Primero debes configurar un negocio</p>
        <Link
          to="/business"
          className="bg-indigo-600 text-white px-5 py-2.5 rounded-lg font-medium hover:bg-indigo-700 transition-colors"
        >
          Ir a Mi Negocio
        </Link>
      </div>
    )
  }

  const getServiceName = (serviceId) => {
    const service = services.find((s) => s.id === serviceId)
    return service?.name ?? 'Servicio desconocido'
  }

  const formatDate = (iso) => {
    const d = new Date(iso)
    return d.toLocaleDateString('es', {
      weekday: 'short',
      day: 'numeric',
      month: 'short',
      year: 'numeric',
    })
  }

  const formatTime = (iso) => {
    const d = new Date(iso)
    return d.toLocaleTimeString('es', { hour: '2-digit', minute: '2-digit' })
  }

  const isPast = (iso) => new Date(iso) < new Date()

  const sorted = [...appointments].sort(
    (a, b) => new Date(b.appointmentDate) - new Date(a.appointmentDate)
  )

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-gray-800">Citas</h1>
          <p className="text-gray-500 text-sm">{appointments.length} citas registradas</p>
        </div>
        <button
          onClick={refreshAppointments}
          className="text-sm text-indigo-600 hover:text-indigo-800 font-medium"
        >
          Actualizar
        </button>
      </div>

      {sorted.length === 0 ? (
        <div className="bg-white rounded-xl border border-gray-200 p-12 text-center">
          <div className="text-5xl mb-4">📭</div>
          <p className="text-gray-500 mb-4">No hay citas registradas</p>
          <Link
            to="/book"
            className="inline-block bg-indigo-600 text-white px-5 py-2.5 rounded-lg font-medium hover:bg-indigo-700 transition-colors"
          >
            Reservar Primera Cita
          </Link>
        </div>
      ) : (
        <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
          {/* Desktop table */}
          <table className="w-full hidden sm:table">
            <thead>
              <tr className="border-b border-gray-200 bg-gray-50">
                <th className="text-left px-5 py-3 text-sm font-medium text-gray-500">Cliente</th>
                <th className="text-left px-5 py-3 text-sm font-medium text-gray-500">Servicio</th>
                <th className="text-left px-5 py-3 text-sm font-medium text-gray-500">Fecha</th>
                <th className="text-left px-5 py-3 text-sm font-medium text-gray-500">Hora</th>
                <th className="text-left px-5 py-3 text-sm font-medium text-gray-500">Duración</th>
                <th className="text-left px-5 py-3 text-sm font-medium text-gray-500">Estado</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {sorted.map((a) => (
                <tr key={a.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-5 py-3 text-sm font-medium text-gray-800">
                    {a.customerName}
                  </td>
                  <td className="px-5 py-3 text-sm text-gray-600">
                    {getServiceName(a.serviceId)}
                  </td>
                  <td className="px-5 py-3 text-sm text-gray-600">
                    {formatDate(a.appointmentDate)}
                  </td>
                  <td className="px-5 py-3 text-sm text-gray-600">
                    {formatTime(a.appointmentDate)} — {formatTime(a.endTime)}
                  </td>
                  <td className="px-5 py-3 text-sm text-gray-600">{a.durationMinutes} min</td>
                  <td className="px-5 py-3">
                    {isPast(a.appointmentDate) ? (
                      <span className="inline-block bg-gray-100 text-gray-500 text-xs px-2.5 py-1 rounded-full">
                        Pasada
                      </span>
                    ) : (
                      <span className="inline-block bg-green-50 text-green-700 text-xs px-2.5 py-1 rounded-full">
                        Activa
                      </span>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {/* Mobile cards */}
          <div className="sm:hidden divide-y divide-gray-100">
            {sorted.map((a) => (
              <div key={a.id} className="p-4 space-y-2">
                <div className="flex justify-between items-center">
                  <span className="font-medium text-gray-800">{a.customerName}</span>
                  {isPast(a.appointmentDate) ? (
                    <span className="bg-gray-100 text-gray-500 text-xs px-2.5 py-1 rounded-full">
                      Pasada
                    </span>
                  ) : (
                    <span className="bg-green-50 text-green-700 text-xs px-2.5 py-1 rounded-full">
                      Activa
                    </span>
                  )}
                </div>
                <div className="text-sm text-gray-500">
                  {getServiceName(a.serviceId)} · {a.durationMinutes} min
                </div>
                <div className="text-sm text-gray-500">
                  {formatDate(a.appointmentDate)} · {formatTime(a.appointmentDate)}
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  )
}
