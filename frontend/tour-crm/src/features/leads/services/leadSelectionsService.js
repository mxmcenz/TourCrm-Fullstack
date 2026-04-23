import api from '@/shared/services/api'

const BASE = (leadId) => `/leads/${leadId}/selections`;

export async function getLeadSelection(leadId, id, opts = {}) {
    const { signal } = opts;
    const { data } = await api.get(`${BASE(leadId)}/${id}`, { signal });
    return data;
}

export async function createLeadSelection(leadId, payload, opts = {}) {
    const { signal } = opts;
    const { data } = await api.post(BASE(leadId), payload, { signal });
    return data;
}

export async function getLastLeadSelection(leadId, opts = {}) {
    const { signal } = opts
    const { data } = await api.get(`/leads/${leadId}/selections/single`, { signal })
    return data
}

export async function updateLeadSelection(leadId, id, payload, opts = {}) {
    const { signal } = opts
    const { data } = await api.put(`/leads/${leadId}/selections/${id}`, payload, { signal })
    return data
}

export async function fetchSelectionDicts(options = {}) {
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
        cities,
        countries,
        hotels,
        roomTypes,
        accomTypes,
        mealPlans,
        partners,
        currenciesRaw,
    ] = await Promise.all([
        getArr('/City'),
        getArr('/Country'),
        getArr('/Hotels'),
        getArr('/NumberType'),
        getArr('/AccommodationType'),
        getArr('/MealType'),
        getArr('/Partner'),
        getArr('/Currencies'),
    ])

    const currencies = (currenciesRaw ?? [])
        .map(c => c.code ?? c.Code ?? c.name ?? c.Name)
        .filter(Boolean)

    return {
        cities,
        countries,
        hotels,
        roomTypes,
        accomTypes,
        mealPlans,
        partners,
        currencies,
    }
}