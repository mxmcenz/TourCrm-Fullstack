import { computed } from 'vue'
import { useCompanyStore } from '@/features/company/store/companyStore'

export function useMyCompany() {
    const store = useCompanyStore()

    async function ensureLoaded() {
        if (!store.isReady && !store.loading) {
            await store.loadMyCompany()
        }
    }

    const myCompany = computed(() => store.company || null)

    return { store, myCompany, ensureLoaded }
}