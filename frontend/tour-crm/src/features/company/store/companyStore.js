import { defineStore } from 'pinia'
import { getMyCompany, renameMyCompany, setMainLegal } from '@/features/company/services/companyApi'

export const useCompanyStore = defineStore('company', {
    state: () => ({
        company: undefined,
        loading: false,
        error: null
    }),

    getters: {
        hasCompany: (s) => !!s.company,
        isReady:    (s) => s.company !== undefined
    },

    actions: {
        async loadMyCompany(force = false) {
            if (this.isReady && !force) return
            this.loading = true; this.error = null
            try {
                const data = await getMyCompany()
                this.company = data ?? null
            } catch (e) {
                this.error = e; this.company = null
            } finally {
                this.loading = false
            }
        },

        async rename(name) {
            await renameMyCompany(String(name || '').trim())
            await this.loadMyCompany(true)
        },

        async setMainLegal(legalEntityId) {
            await setMainLegal(legalEntityId)
            await this.loadMyCompany(true)
        }
    }
})