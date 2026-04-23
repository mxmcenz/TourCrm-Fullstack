import { createRouter, createWebHistory } from 'vue-router'
import { useSessionStore } from '@/app/store/sessionStore'

import HomePage from '@/features/home/pages/HomePage.vue'
import LoginPage from '@/features/auth/pages/LoginPage.vue'
import RegisterStep1Page from '@/features/register/pages/RegisterStep1Page.vue'
import RegisterVerify from '@/features/register/pages/RegisterVerify.vue'
import SetPasswordPage from '@/features/auth/pages/SetPasswordPage.vue'

import ProfilePage from '@/features/profile/pages/ProfilePage.vue'

import RestoreStep1Page from '@/features/restore/pages/RestoreStep1Page.vue'
import RestoreVerifyPage from '@/features/restore/pages/RestoreVerifyPage.vue'
import RestoreSetPasswordPage from '@/features/restore/pages/RestoreSetPasswordPage.vue'

import EmployeeListPage from '@/features/employees/pages/EmployeeListPage.vue'
import EmployeeCreatePage from '@/features/employees/pages/EmployeeCreatePage.vue'
import EmployeeEditPage from '@/features/employees/pages/EmployeeEditPage.vue'

import RolesPage from '@/features/roles/pages/RolesPage.vue'
import CreateRolePage from '@/features/roles/pages/CreateRolePage.vue'
import EditRolePage from '@/features/roles/pages/EditRolePage.vue'

import UsersPage from '@/features/users/pages/UsersPage.vue'
import UserEdit from '@/features/users/pages/UserEdit.vue'

import CompanyPage from '@/features/company/pages/CompanyPage.vue'
import LegalEntityUpsertPage from '@/features/touragent/pages/LegalEntityUpsertPage.vue'

import OfficesPage from '@/features/offices/pages/OfficesPage.vue'
import OfficeUpsertPage from '@/features/offices/pages/OfficeUpsertPage.vue'

import LeadsListPage from '@/features/leads/pages/LeadsListPage.vue'
import LeadUpsertPage from '@/features/leads/pages/LeadUpsertPage.vue'
import SelectionManualPage from '@/features/leads/pages/SelectionManualPage.vue'

import DictionaryPage from '@/features/dictionaries/pages/DictionaryListPage.vue'
import DictionaryUpsertPage from '@/features/dictionaries/pages/DictionaryUpsertPage.vue'

import ClientUpsertPage from '@/features/clients/pages/ClientUpsertPage.vue'
import ClientListPage from '@/features/clients/pages/ClientListPage.vue'
import ClientDetailsPage from '@/features/clients/pages/ClientDetailsPage.vue'
import DealsListPage from "@/features/deals/pages/DealsListPage.vue";
import DealUpsertPage from "@/features/deals/pages/DealUpsertPage.vue";

import TariffsListPage from '@/features/tariffs/pages/TariffsListPage.vue'
import TariffsUpsertPage from '@/features/tariffs/pages/TariffsUpsertPage.vue'

const routes = [
    { path: '/', name: 'Home', component: HomePage },

    { path: '/login', name: 'Login', component: LoginPage, meta: { sidebar: false } },
    { path: '/register/step1', name: 'RegisterStep1', component: RegisterStep1Page, meta: { sidebar: false } },
    { path: '/register/verify', name: 'Verify', component: RegisterVerify, meta: { sidebar: false } },
    { path: '/set-password', name: 'SetPassword', component: SetPasswordPage, meta: { sidebar: false } },

    { path: '/restore-pass', name: 'RestoreStep1', component: RestoreStep1Page },
    { path: '/restore/verify', name: 'RestoreVerify', component: RestoreVerifyPage },
    { path: '/restore/set-password', name: 'RestoreSetPassword', component: RestoreSetPasswordPage },

    { path: '/profile', name: 'Profile', component: ProfilePage, meta: { requiresAuth: true } },
    { path: '/company', name: 'Company', component: CompanyPage,
        meta: { requiresAuth: true, perms: 'ViewLegalEntities' } },

    {
        path: '/company/legal-entities/new',
        name: 'LegalEntityCreate',
        component: LegalEntityUpsertPage,
        meta: { requiresAuth: true, requiresCompany: true },
    },
    {
        path: '/company/legal-entities/:id/edit',
        name: 'LegalEntityEdit',
        component: LegalEntityUpsertPage,
        props: true,
        meta: { requiresAuth: true, requiresCompany: true },
    },

    { path: '/employees', name: 'Employees', component: EmployeeListPage, meta: { requiresAuth: true, requiresCompany: true } },
    { path: '/employees/create', name: 'EmployeesCreate', component: EmployeeCreatePage, meta: { requiresAuth: true, requiresCompany: true } },
    { path: '/employees/edit/:id', name: 'EmployeesEdit', component: EmployeeEditPage, meta: { requiresAuth: true, requiresCompany: true } },

    { path: '/roles', name: 'Roles', component: RolesPage, meta: { requiresAuth: true, requiresCompany: true } },
    { path: '/roles/create', name: 'RolesCreate', component: CreateRolePage, meta: { requiresAuth: true, requiresCompany: true } },
    { path: '/roles/:id/edit', name: 'RolesEdit', component: EditRolePage, meta: { requiresAuth: true, requiresCompany: true } },

    { path: '/users', name: 'Users', component: UsersPage, meta: { requiresAuth: true, requiresCompany: true } },
    { path: '/users/:id/edit', name: 'UserEdit', component: UserEdit, meta: { requiresAuth: true, requiresCompany: true } },

    { path: '/offices', name: 'Offices', component: OfficesPage, meta: { requiresAuth: true, requiresCompany: true } },
    { path: '/offices/new', name: 'OfficeCreate', component: OfficeUpsertPage, meta: { requiresAuth: true, requiresCompany: true } },
    { path: '/offices/:id/edit', name: 'OfficeEdit', component: OfficeUpsertPage, props: true, meta: { requiresAuth: true, requiresCompany: true } },

    { path: '/leads', name: 'LeadsList', component: LeadsListPage, meta: { requiresAuth: true, requiresCompany: true } },
    { path: '/leads/create', name: 'LeadCreate', component: LeadUpsertPage, meta: { requiresAuth: true, requiresCompany: true } },
    { path: '/leads/:id(\\d+)', name: 'LeadEdit', component: LeadUpsertPage, props: true, meta: { requiresAuth: true, requiresCompany: true } },
    { path: '/leads/:leadId(\\d+)/selections/new', name: 'LeadSelectionCreate', component: SelectionManualPage, props: true, meta: { requiresAuth: true, requiresCompany: true } },
    { path: '/leads/:leadId(\\d+)/selections/:id(\\d+)', name: 'LeadSelectionEdit', component: SelectionManualPage, props: true, meta: { requiresAuth: true, requiresCompany: true } },

    { path: '/dictionaries/:dict', name: 'Dictionary', component: DictionaryPage, meta: { requiresAuth: true, requiresCompany: true } },
    { path: '/dictionaries/:dict/create', name: 'DictionaryCreate', component: DictionaryUpsertPage, meta: { requiresAuth: true, requiresCompany: true } },
    { path: '/dictionaries/:dict/edit/:id', name: 'DictionaryEdit', component: DictionaryUpsertPage, props: true, meta: { requiresAuth: true, requiresCompany: true } },

    { path: '/clients', name: 'ClientList', component: ClientListPage, meta: { requiresAuth: true, requiresCompany: true } },
    {
        path: '/clients/new',
        name: 'ClientCreate',
        component: ClientUpsertPage,
        props: r => ({ isTourist: r.query.isTourist === '1' || r.query.isTourist === 'true' }),
        meta: { requiresAuth: true, requiresCompany: true },
    },
    { path: '/clients/:id/edit', name: 'ClientEdit', component: ClientUpsertPage, props: r => ({ id: Number(r.params.id) }), meta: { requiresAuth: true, requiresCompany: true } },
    { path: '/clients/:id', name: 'ClientDetails', component: ClientDetailsPage, props: true, meta: { requiresAuth: true, requiresCompany: true } },

    { path: '/tariffs', name: 'Tariffs', component: TariffsListPage },
    { path: '/tariffs/new', name: 'TariffCreate', component: TariffsUpsertPage, meta: { requiresAuth: true, requiresSuperAdmin: true } },
    { path: '/tariffs/:id/edit', name: 'TariffEdit', component: TariffsUpsertPage, props: true, meta: { requiresAuth: true, requiresSuperAdmin: true } },
    { path: '/deals', name: 'DealsList',  component: DealsListPage,  meta: { requiresAuth: true, requiresCompany: true } },
    { path: '/deals/create', name: 'DealCreate', component: DealUpsertPage, meta: { requiresAuth: true, requiresCompany: true } },
    { path: '/deals/:id(\\d+)', name: 'DealEdit', component: DealUpsertPage, props: true, meta: { requiresAuth: true, requiresCompany: true } },
]

const router = createRouter({
    history: createWebHistory(),
    routes,
    scrollBehavior: () => ({ top: 0 }),
})

function normalizePerm(v) {
    if (!v) return { type: 'none', keys: [] }
    if (typeof v === 'string') return { type: 'one', keys: [v] }
    if (Array.isArray(v)) return { type: 'all', keys: v }
    if (v.any && Array.isArray(v.any)) return { type: 'any', keys: v.any }
    if (v.all && Array.isArray(v.all)) return { type: 'all', keys: v.all }
    return { type: 'none', keys: [] }
}

function checkPerm(session, meta) {
    const { type, keys } = normalizePerm(meta)
    if (type === 'none') return true
    if (type === 'one') return session.has(keys[0])
    if (type === 'any') return session.hasAny(keys)
    if (type === 'all') return session.hasAll(keys)
    return true
}

router.beforeEach(async (to) => {
    const session = useSessionStore()

    const needAuth = !!(to.meta?.requiresAuth || to.meta?.requiresSuperAdmin || to.meta?.perms)
    if (!needAuth) {
        if (to.name === 'Login' && session.isLoggedIn) {
            const redirect = to.query?.redirect || '/'
            return typeof redirect === 'string' ? redirect : { path: '/' }
        }
        return true
    }

    await session.ensureLoaded()


    if (!session.isLoggedIn) {
        return { name: 'Login', query: { redirect: to.fullPath } }
    }

    if (to.meta?.requiresSuperAdmin && !session.isSuperAdmin) {
        return { name: 'Tariffs' }
    }

    if (to.meta?.perms && !checkPerm(session, to.meta.perms)) {
        return { name: 'Home' }
    }

    return true
})

export default router
