import {defineStore} from 'pinia'
import api from '@/shared/services/api'

export const useSessionStore = defineStore('user', {
    state: () => ({
        user: null,
    }),
    actions: {
        async fetchUser() {
            try {
                const res = await api.get('/auth/me')
                this.user = res.data
            } catch (e) {
                this.user = null
            }
        },
        async updateUser(payload) {
            try {
                const res = await api.put('/auth/update-profile', payload)
                this.user = res.data.data
            } catch (e) {
                console.error('Ошибка при обновлении профиля: ', e)
            }
        },
        logout() {
            api.post('/auth/logout')
            this.user = null
        },
    },
})
