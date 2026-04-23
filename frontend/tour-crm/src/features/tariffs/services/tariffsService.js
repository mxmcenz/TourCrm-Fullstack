import api from '@/shared/services/api'

export const fetchTariffs = () => api.get('/tariffs').then(r => r.data?.data ?? [])
export const fetchTariff = (id) => api.get(`/tariffs/${id}`).then(r => r.data?.data ?? null)
export const createTariff = (payload) => api.post('/tariffs', payload).then(r => r.data?.data ?? null)
export const updateTariff = (id, payload) => api.put(`/tariffs/${id}`, payload).then(r => r.data?.data ?? null)
export const deleteTariff = (id) => api.delete(`/tariffs/${id}`)

export const fetchTariffPrice = (id, period /* 'Month' | 'HalfYear' | 'Year' */) =>
    api.get(`/tariffs/${id}/price`, { params: { period } }).then(r => r.data)

export const fetchTariffMonthly = (id, period) =>
    api.get(`/tariffs/${id}/price-per-month`, { params: { period } }).then(r => r.data)

export const fetchPermissionTree = () =>
    api.get('/permissions/tree').then(r => r.data)
