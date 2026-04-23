import { defineStore } from 'pinia'
import api from '@/shared/services/api'

function toArr(x) {
    if (!x) return []
    if (Array.isArray(x)) return x.map(String)
    if (typeof x === 'string') return [x]
    return []
}
function uniq(arr) { return Array.from(new Set(arr)) }

export const useSessionStore = defineStore('session', {
    state: () => ({
        user: null,
        isLoading: false,
        fetched: false,
        _promise: null,
    }),

    getters: {
        isLoggedIn: (s) => !!s.user,
        isSuperAdmin: (s) => {
            const u = s.user || {}
            return !!(u.isSuperAdmin || toArr(u.roles).includes('SuperAdmin') || u.role === 'SuperAdmin' || toArr(u.claims?.roles).includes('SuperAdmin'))
        },
        roles: (s) => {
            const u = s.user || {}
            return uniq([...toArr(u.roles), ...toArr(u.claims?.roles)])
        },
        permissions: (s) => {
            const u = s.user || {}
            return uniq([
                ...toArr(u.permissions),
                ...toArr(u.perms),
                ...toArr(u.claims?.permissions),
                ...toArr(u.claims?.perms),
            ])
        },
        has() {
            return (key) => {
                if (!key) return false
                if (this.isSuperAdmin) return true
                return this.permissions.includes(String(key))
            }
        },
        hasAny() {
            return (keys) => {
                if (this.isSuperAdmin) return true
                const arr = toArr(keys)
                if (!arr.length) return false
                const set = new Set(this.permissions)
                return arr.some(k => set.has(k))
            }
        },
        hasAll() {
            return (keys) => {
                if (this.isSuperAdmin) return true
                const arr = toArr(keys)
                if (!arr.length) return false
                const set = new Set(this.permissions)
                return arr.every(k => set.has(k))
            }
        },
        can() { return this.has },
        canAny() { return this.hasAny },
        canAll() { return this.hasAll },
    },

    actions: {
        async fetchUser() {
            this.isLoading = true
            try {
                const res = await api.get('/auth/me')
                this.user = res?.data?.data ?? res?.data ?? null
            } catch {
                this.user = null
                throw new Error('me failed')
            } finally {
                this.isLoading = false
                this.fetched = true
            }
        },
        async ensureLoaded() {
            if (this.fetched) return
            if (!this._promise) {
                this._promise = this.fetchUser().catch(() => undefined).finally(() => { this._promise = null })
            }
            return this._promise
        },
        async logout() {
            await api.post('/auth/logout').catch(() => undefined)
            this.user = null
            this.fetched = false
        },
        async login(user) {
            this.user = user ?? null
            this.fetched = true
            this.isLoading = false
            await this.fetchUser().catch(() => undefined)
        },
    },
})
