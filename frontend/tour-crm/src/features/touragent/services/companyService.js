import api from '@/shared/services/api'

const BASE = '/LegalEntities'

export async function fetchLegalEntities(search = '', opts = {}) {
    const params = {}
    const term = (search || '').trim()
    if (term) params.q = term
    const { data } = await api.get(BASE, { params, ...opts })
    return Array.isArray(data) ? data : data?.data ?? []
}

export async function getLegalEntity(id) {
    const { data } = await api.get(`${BASE}/${id}`)
    return data
}

export async function createLegalEntity(payload) {
    const { data } = await api.post(BASE, payload)
    return data
}

export async function updateLegalEntity(id, payload) {
    const { data } = await api.put(`${BASE}/${id}`, payload)
    return data
}

export async function deleteLegalEntity(id) {
    await api.delete(`${BASE}/${id}`)
}

function pickArray(data) {
    if (Array.isArray(data)) return data
    if (Array.isArray(data?.items)) return data.items
    if (Array.isArray(data?.data)) return data.data
    return []
}

const toId = (v) => {
    const n = Number(v)
    return Number.isFinite(n) && String(v).trim() !== '' ? n : (v != null ? String(v) : null)
}

export async function fetchCountries(opts = {}) {
    const res = await api.get('/Country', { ...opts })
    const raw = pickArray(res.data)
    return raw
        .map(x => ({
            id: toId(x.id ?? x.Id ?? x.code ?? x.Code),
            name: x.name ?? x.Name ?? x.nameRu ?? x.NameRu ?? x.title ?? x.Title ?? ''
        }))
        .filter(x => x.name)
}

export async function fetchCities(params = {}, opts = {}) {
    const query = {}
    if (params.countryId != null) query.countryId = params.countryId
    if (params.q && String(params.q).trim()) query.q = String(params.q).trim()

    const res = await api.get('/City', { params: query, ...opts })
    const raw = pickArray(res.data)

    return raw
        .map(x => ({
            id: toId(x.id ?? x.Id),
            name: x.name ?? x.Name ?? x.nameRu ?? x.NameRu ?? x.title ?? x.Title ?? '',
            countryId: toId(x.countryId ?? x.CountryId)
        }))
        .filter(x => x.name)
}