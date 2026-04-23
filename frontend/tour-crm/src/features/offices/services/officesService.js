import api from '@/shared/services/api'

const BASE = '/Offices'

export async function fetchOfficesByLegal(legalEntityId, params = {}, options = {}) {
    const { signal } = options
    const { data } = await api.get(`${BASE}/by-legal/${legalEntityId}`, { params, signal })
    return Array.isArray(data) ? data : data?.data ?? []
}

export async function getOffice(id, options = {}) {
    const { signal } = options
    const { data } = await api.get(`${BASE}/${id}`, { signal })
    return data
}

export async function createOffice(payload) {
    const { data } = await api.post(BASE, payload)
    return data
}

export async function updateOffice(id, payload) {
    const { data } = await api.put(`${BASE}/${id}`, payload)
    return data
}

export async function deleteOffice(id) {
    await api.delete(`${BASE}/${id}`)
}