import api from '@/shared/services/api'

export default {
    async fetchAll(endpoint, params = {}, options = {}) {
        try {
            const res = await api.get(`/${endpoint}`, { params, ...options })
            return res?.data ?? []
        } catch (e) {
            console.error(`Ошибка при загрузке списка из ${endpoint}:`, e)
            return []
        }
    },

    async getById(endpoint, id) {
        try {
            const res = await api.get(`/${endpoint}/${id}`)
            return res?.data ?? null
        } catch (e) {
            console.error(`Ошибка при загрузке элемента из ${endpoint}/${id}:`, e)
            return null
        }
    },

    async create(endpoint, dto) {
        try {
            const res = await api.post(`/${endpoint}`, dto)
            return res?.data ?? null
        } catch (e) {
            console.error(`Ошибка при создании в ${endpoint}:`, e)
            return null
        }
    },

    async update(endpoint, id, dto) {
        try {
            const res = await api.put(`/${endpoint}/${id}`, dto)
            return res?.data ?? null
        } catch (e) {
            console.error(`Ошибка при обновлении в ${endpoint}/${id}:`, e)
            return null
        }
    },

    async remove(endpoint, id) {
        try {
            return await api.delete(`/${endpoint}/${id}`)
        } catch (e) {
            console.error(`Ошибка при удалении из ${endpoint}/${id}:`, e)
            return null
        }
    }
}
