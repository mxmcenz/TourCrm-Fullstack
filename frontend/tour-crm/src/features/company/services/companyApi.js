import api from '@/shared/services/api'

export async function getMyCompany() {
    const { data } = await api.get('/Companies/mine')
    return data
}

export async function renameMyCompany(name) {
    const { data } = await api.put('/Companies/mine/name', { name })
    return data
}

export async function setMainLegal(legalEntityId) {
    await api.put(`/Companies/mine/main-legal/${legalEntityId}`)
}