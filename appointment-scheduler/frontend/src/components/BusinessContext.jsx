import { createContext, useContext, useState, useEffect } from 'react'
import { getServices, getAppointments } from '../api/client'

const BusinessContext = createContext(null)

export function BusinessProvider({ children }) {
  const [business, setBusiness] = useState(() => {
    const saved = localStorage.getItem('activeBusiness')
    return saved ? JSON.parse(saved) : null
  })
  const [services, setServices] = useState([])
  const [appointments, setAppointments] = useState([])

  useEffect(() => {
    if (business) {
      localStorage.setItem('activeBusiness', JSON.stringify(business))
      refreshServices()
      refreshAppointments()
    } else {
      localStorage.removeItem('activeBusiness')
      setServices([])
      setAppointments([])
    }
  }, [business?.id])

  const refreshServices = async () => {
    if (!business) return
    try {
      const data = await getServices(business.id)
      setServices(data)
    } catch {
      setServices([])
    }
  }

  const refreshAppointments = async () => {
    if (!business) return
    try {
      const data = await getAppointments(business.id)
      setAppointments(data)
    } catch {
      setAppointments([])
    }
  }

  return (
    <BusinessContext.Provider
      value={{
        business,
        setBusiness,
        services,
        refreshServices,
        appointments,
        refreshAppointments,
      }}
    >
      {children}
    </BusinessContext.Provider>
  )
}

export const useBusiness = () => useContext(BusinessContext)
