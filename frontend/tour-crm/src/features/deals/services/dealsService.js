import api from '@/shared/services/api'

const BASE = '/Deals'

export async function getDeal(id, options = {}) {
    const { signal } = options
    const { data } = await api.get(`${BASE}/${id}`, { signal })
    return data
}

export async function createDeal(payload) {
    const { data } = await api.post(BASE, payload)
    return data
}

export async function updateDeal(id, payload) {
    await api.put(`${BASE}/${id}`, payload)
}

export async function changeDealStatus(id, statusId) {
    await api.post(`${BASE}/${id}/status/${statusId}`)
}

export async function archiveDeal(id, options = {}) {
    const { signal } = options
    await api.delete(`${BASE}/${id}`, { signal })
}

export async function restoreDeal(id, options = {}) {
    const { signal } = options
    await api.post(`${BASE}/${id}/restore`, null, { signal })
}

export async function createDealFromLead(leadId, { managerId, touristId }) {
    const { data } = await api.post(
        `${BASE}/from-lead/${leadId}`,
        null,
        { params: { managerId, touristId } }
    )
    return data
}

export async function fetchDealHistory(id, options = {}) {
    const { signal } = options
    const { data } = await api.get(`${BASE}/${id}/history`, { signal })
    return Array.isArray(data) ? data : []
}

export async function fetchDealsPage(params = {}, options = {}) {
    const { signal } = options
    const { data } = await api.get(`${BASE}/search`, { params, signal })
    return data
}

export async function fetchDealDicts(options = {}) {
    const { signal } = options

    const getArr = async (url) => {
        try {
            const r = await api.get(url, { signal })
            return Array.isArray(r.data) ? r.data : (r.data?.items ?? [])
        } catch {
            return []
        }
    }

    const [
        statuses,
        offices,
        companies,
        requestTypes,
        sources,
        tourOperators,
        visaTypes,
        managersRaw
    ] = await Promise.all([
        getArr('/DealStatuses'),
        getArr('/Offices'),
        getArr('/Companies'),
        getArr('/LeadRequestTypes'),
        getArr('/LeadSources'),
        getArr('/TourOperator'),
        getArr('/VisaTypes'),
        (async () => {
            const a = await getArr('/Employees')
            return a.length ? a : getArr('/Employees')
        })()
    ])

    const managers = (managersRaw ?? []).map(m => {
        const parts = [m.lastName, m.firstName, m.middleName].filter(Boolean).join(' ').trim()
        const full = (m.fullName ?? parts) || m.email || `ID ${m.id}`
        return { id: m.id, fullName: full }
    })

    return {
        statuses,
        offices,
        companies,
        requestTypes,
        sources,
        tourOperators,
        visaTypes,
        managers,
    }
}

export async function fetchDealStatuses(options = {}) {
    const { signal } = options
    const { data } = await api.get('/DealStatuses', { signal })
    return Array.isArray(data) ? data : (data?.items ?? [])
}