import api from '@/shared/services/api'

export async function searchClients(params = {}, config = {}) {
    const res = await api.get('/clients', {params, ...config})
    const total = Number(res.headers?.['x-total-count'] || 0)
    return {items: res.data ?? [], total}
}

export async function fetchClients(params = {}, config = {}) {
    const {q, page = 1, pageSize = 20, includeDeleted} = params
    return (await api.get('/clients', {
        params: {q, page, pageSize, includeDeleted},
        ...config
    })).data
}

export async function getClient(id, {includeDeleted = false} = {}, cfg = {}) {
    const params = includeDeleted ? {includeDeleted: true} : undefined;
    const {data} = await api.get(`/clients/${id}`, {...cfg, params});
    return data;
}

export async function createClient(payload, config = {}) {
    return (await api.post('/clients', payload, config)).data
}

export async function updateClient(id, payload, config = {}) {
    await api.put(`/clients/${id}`, payload, config)
}

export async function deleteClientSoft(id, config = {}) {
    await api.delete(`/clients/${id}`, config)
}

export async function restoreClient(id) {
    const { data } = await api.post(`/clients/${id}/restore`)
    return data
}

export async function getClientHistory(id, { page = 1, pageSize = 50 } = {}) {
    const res = await api.get(`/clients/${id}/history`, { params: { page, pageSize } })
    const total = Number(res.headers['x-total-count'] ?? 0)
    return { items: res.data ?? [], total }
}


