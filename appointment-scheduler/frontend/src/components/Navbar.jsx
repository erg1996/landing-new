import { Link, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from './AuthContext'

const links = [
  { to: '/', label: 'Dashboard', icon: '📊' },
  { to: '/business', label: 'Mi Negocio', icon: '🏪' },
  { to: '/calendar', label: 'Calendario', icon: '📅' },
  { to: '/appointments', label: 'Citas', icon: '📋' },
  { to: '/book', label: 'Reservar', icon: '✏️' },
]

export default function Navbar() {
  const { pathname } = useLocation()
  const { auth, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  return (
    <nav className="bg-white border-b border-gray-200 shadow-sm">
      <div className="max-w-6xl mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          <span className="text-xl font-bold text-indigo-600">SchedulePro</span>
          <div className="flex items-center gap-1">
            {links.map((link) => (
              <Link
                key={link.to}
                to={link.to}
                className={`px-3 py-2 rounded-lg text-sm font-medium transition-colors ${
                  pathname === link.to
                    ? 'bg-indigo-50 text-indigo-700'
                    : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
                }`}
              >
                <span className="mr-1.5">{link.icon}</span>
                {link.label}
              </Link>
            ))}
            {auth && (
              <div className="flex items-center gap-3 ml-4 pl-4 border-l border-gray-200">
                <span className="text-xs text-gray-500 hidden sm:block">{auth.fullName}</span>
                <button
                  onClick={handleLogout}
                  className="text-sm text-gray-500 hover:text-red-600 transition-colors"
                >
                  Salir
                </button>
              </div>
            )}
          </div>
        </div>
      </div>
    </nav>
  )
}
