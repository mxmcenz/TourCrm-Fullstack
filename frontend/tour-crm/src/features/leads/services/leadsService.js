import api from '@/shared/services/api'

const BASE = '/Lead'

export async function fetchLeadsPage(params = {}, options = {}) {
    const { signal } = options
    const { data } = await api.get(`${BASE}/search`, { params, signal })
    return data
}

export async function getLead(id, options = {}) {
    const { signal } = options
    const { data } = await api.get(`${BASE}/${id}`, { signal })
    return data
}

export async function createLead(payload) {
    const { data } = await api.post(BASE, payload)
    return data
}

export async function updateLead(id, payload) {
    const { data } = await api.put(`${BASE}/${id}`, payload)
    return data
}

export async function deleteLead(id) {
    await api.delete(`${BASE}/${id}`)
}

export async function assignLead(id, userId) {
    await api.put(`${BASE}/${id}/assign`, { userId })
}

export async function fetchLeadHistory(id, options = {}) {
    const { signal } = options
    const { data } = await api.get(`${BASE}/${id}/history`, { signal })
    return Array.isArray(data) ? data : []
}

export async function fetchLeadDicts(options = {}) {
    const { signal } = options
    const [
        statuses,
        sources,
        requestTypes,
        labels,
        countries,
    ] = await Promise.all([
        api.get('/LeadStatuses',     { signal }).then(r => r.data ?? []),
        api.get('/LeadSources',      { signal }).then(r => r.data ?? []),
        api.get('/LeadRequestTypes', { signal }).then(r => r.data ?? []),
        api.get('/Labels',           { signal }).then(r => r.data ?? []),
        api.get('/Country',        { signal }).then(r => r.data ?? []),
    ])
    return { statuses, sources, requestTypes, labels, countries }
}

export async function fetchManagers(params = {}, options = {}) {
    const { signal } = options
    const { data } = await api.get('/Employees', { params, signal })
    const raw = Array.isArray(data) ? data : (data?.items ?? [])

    return raw.map(m => {
        const parts = [m.lastName, m.firstName, m.middleName].filter(Boolean).join(' ').trim()
        const full = (m.fullName ?? parts) || m.email || `ID ${m.id}`
        return { id: m.id, fullName: full }
    })
}