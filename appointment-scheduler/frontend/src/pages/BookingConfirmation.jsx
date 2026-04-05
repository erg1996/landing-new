import { useLocation, useParams, Link } from 'react-router-dom'

export default function BookingConfirmation() {
  const { slug } = useParams()
  const { state } = useLocation()

  if (!state) {
    return (
      <div className="min-h-screen bg-gray-50 flex flex-col items-center justify-center px-4">
        <div className="text-5xl mb-4">📅</div>
        <p className="text-gray-500 mb-4">No hay datos de confirmación</p>
        <Link
          to={`/book/${slug}`}
          className="bg-indigo-600 text-white px-5 py-2.5 rounded-lg font-medium hover:bg-indigo-700 transition-colors"
        >
          Reservar una cita
        </Link>
      </div>
    )
  }

  const formatDate = (iso) => {
    const d = new Date(iso)
    return d.toLocaleDateString('es', {
      weekday: 'long',
      day: 'numeric',
      month: 'long',
      year: 'numeric',
    })
  }

  const formatTime = (iso) => {
    const d = new Date(iso)
    return d.toLocaleTimeString('es', { hour: '2-digit', minute: '2-digit' })
  }

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center px-4">
      <div className="max-w-md w-full text-center">
        <div className="bg-white rounded-2xl border border-gray-200 shadow-sm p-8">
          <div className="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg className="w-8 h-8 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
            </svg>
          </div>

          <h1 className="text-2xl font-bold text-gray-800 mb-2">Cita Confirmada</h1>
          <p className="text-gray-500 text-sm mb-6">
            Tu cita en <strong>{state.business}</strong> ha sido reservada
          </p>

          <div className="bg-gray-50 rounded-xl p-5 text-left space-y-3 mb-6">
            <div className="flex justify-between">
              <span className="text-sm text-gray-500">Servicio</span>
              <span className="text-sm font-medium text-gray-800">{state.service}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-sm text-gray-500">Fecha</span>
              <span className="text-sm font-medium text-gray-800">{formatDate(state.date)}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-sm text-gray-500">Hora</span>
              <span className="text-sm font-medium text-gray-800">
                {formatTime(state.date)} — {formatTime(state.endTime)}
              </span>
            </div>
            <div className="flex justify-between">
              <span className="text-sm text-gray-500">Duración</span>
              <span className="text-sm font-medium text-gray-800">{state.duration} min</span>
            </div>
            <div className="flex justify-between">
              <span className="text-sm text-gray-500">Cliente</span>
              <span className="text-sm font-medium text-gray-800">{state.customer}</span>
            </div>
          </div>

          <Link
            to={`/book/${slug}`}
            className="inline-block bg-indigo-600 text-white px-6 py-2.5 rounded-lg font-medium hover:bg-indigo-700 transition-colors text-sm"
          >
            Reservar otra cita
          </Link>
        </div>
      </div>
    </div>
  )
}
