import { BrowserRouter, Routes, Route, useLocation } from 'react-router-dom'
import { BusinessProvider } from './components/BusinessContext'
import Navbar from './components/Navbar'
import Dashboard from './pages/Dashboard'
import BusinessPanel from './pages/BusinessPanel'
import BookAppointment from './pages/BookAppointment'
import AppointmentsList from './pages/AppointmentsList'
import PublicBooking from './pages/PublicBooking'
import BookingConfirmation from './pages/BookingConfirmation'

function Layout() {
  const { pathname } = useLocation()
  const isPublicRoute = pathname.startsWith('/book/')

  // Public booking pages render without admin shell
  if (isPublicRoute) {
    return (
      <Routes>
        <Route path="/book/:slug/confirmed" element={<BookingConfirmation />} />
        <Route path="/book/:slug" element={<PublicBooking />} />
      </Routes>
    )
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />
      <main className="max-w-6xl mx-auto px-4 py-8">
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/business" element={<BusinessPanel />} />
          <Route path="/book" element={<BookAppointment />} />
          <Route path="/appointments" element={<AppointmentsList />} />
        </Routes>
      </main>
    </div>
  )
}

export default function App() {
  return (
    <BrowserRouter>
      <BusinessProvider>
        <Layout />
      </BusinessProvider>
    </BrowserRouter>
  )
}
