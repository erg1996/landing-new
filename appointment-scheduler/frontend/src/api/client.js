// In dev, Vite proxy handles /api → backend. In production, set VITE_API_URL.
const BASE_URL = import.meta.env.VITE_API_URL ?? ''

async function request(path, options = {}) {
  const res = await fetch(`${BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json', ...options.headers },
    ...options,
  })
  if (!res.ok) {
    const body = await res.json().catch(() => ({}))
    throw new Error(body.error ?? `HTTP ${res.status}`)
  }
  if (res.status === 204) return null
  return res.json()
}

// Business
export const createBusiness = (data) =>
  request('/api/business', { method: 'POST', body: JSON.stringify(data) })

export const getBusiness = (id) => request(`/api/business/${id}`)

export const getBusinessBySlug = (slug) => request(`/api/business/slug/${slug}`)

export const getAllBusinesses = () => request('/api/business')

// Services
export const createService = (data) =>
  request('/api/services', { method: 'POST', body: JSON.stringify(data) })

export const getServices = (businessId) =>
  request(`/api/services?businessId=${businessId}`)

// Working Hours
export const getWorkingHours = (businessId) =>
  request(`/api/working-hours?businessId=${businessId}`)

export const createWorkingHours = (data) =>
  request('/api/working-hours', { method: 'POST', body: JSON.stringify(data) })

export const updateWorkingHours = (id, data) =>
  request(`/api/working-hours/${id}`, { method: 'PUT', body: JSON.stringify(data) })

export const deleteWorkingHours = (id) =>
  request(`/api/working-hours/${id}`, { method: 'DELETE' })

// Blocked Dates
export const getBlockedDates = (businessId) =>
  request(`/api/blocked-dates?businessId=${businessId}`)

export const createBlockedDate = (data) =>
  request('/api/blocked-dates', { method: 'POST', body: JSON.stringify(data) })

export const deleteBlockedDate = (id, businessId) =>
  request(`/api/blocked-dates/${id}?businessId=${businessId}`, { method: 'DELETE' })

// Availability
export const getAvailability = (businessId, date, serviceId) =>
  request(
    `/api/availability?businessId=${businessId}&date=${encodeURIComponent(date)}&serviceId=${serviceId}`
  )

// Auth
export const register = (data) =>
  request('/api/auth/register', { method: 'POST', body: JSON.stringify(data) })

export const login = (data) =>
  request('/api/auth/login', { method: 'POST', body: JSON.stringify(data) })

// Appointments
export const createAppointment = (data) =>
  request('/api/appointments', { method: 'POST', body: JSON.stringify(data) })

export const getAppointments = (businessId) =>
  request(`/api/appointments?businessId=${businessId}`)

// Analytics
export const getDashboardAnalytics = (businessId) =>
  request(`/api/analytics/dashboard?businessId=${businessId}`)
