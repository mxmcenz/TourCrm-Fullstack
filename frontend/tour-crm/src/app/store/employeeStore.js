import { defineStore } from 'pinia'
import api from '@/shared/services/api'

function isSuperAdminName(name = '') {
    if (!name) return false
    const n = String(name).trim().toLowerCase().replace(/\s+/g, '')
    return n === 'superadmin' || n === 'суперадмин'
}

function makeFullName(firstName, middleName, lastName) {
    return [firstName, middleName, lastName].filter(Boolean).join(' ').trim()
}

function sanitizePayload(dto) {
    const toNumericUniqueArray = (arr) => {
        if (!Array.isArray(arr)) return []
        const nums = arr
            .map(x => {
                if (x == null) return NaN
                if (typeof x === 'object') return Number(x.id ?? x.value ?? x)
                return Number(x)
            })
            .filter(n => !Number.isNaN(n))
        return Array.from(new Set(nums))
    }

    return {
        officeId: dto.officeId != null ? Number(dto.officeId) : 0,
        legalEntityId: dto.legalEntityId != null ? Number(dto.legalEntityId) : 0,
        email: dto.email ?? '',
        firstName: dto.firstName ?? '',
        lastName: dto.lastName ?? '',
        middleName: dto.middleName ?? null,
        phone: dto.phone ?? '',
        leadLimit: Number(dto.leadLimit ?? 0),
        password: dto.password || null,
        isDeleted: !!dto.isDeleted,
        roleIds: toNumericUniqueArray(dto.roleIds),

        position: dto.position ?? null,
        positionGenitive: dto.positionGenitive ?? null,
        powerOfAttorneyNumber: dto.powerOfAttorneyNumber ?? null,
        lastNameGenitive: dto.lastNameGenitive ?? null,
        firstNameGenitive: dto.firstNameGenitive ?? null,
        middleNameGenitive: dto.middleNameGenitive ?? null,

        mobilePhone: dto.mobilePhone ?? null,
        additionalPhone: dto.additionalPhone ?? null,

        birthDate: dto.birthDate ?? null,
        timeZone: dto.timeZone ?? null,
        contactInfo: dto.contactInfo ?? null,
        hireDate: dto.hireDate ?? null,
        salaryAmount: dto.salaryAmount != null && dto.salaryAmount !== '' ? Number(dto.salaryAmount) : null,
        workConditions: dto.workConditions ?? null,
        note: dto.note ?? null
    }
}

export const useEmployeeStore = defineStore('employee', {
    state: () => ({
        employees: [],
        roles: [],
        legals: [],
        offices: [],
        loading: false,
        error: '',
        deletingId: null,
        restoringId: null,
        pagination: {
            currentPage: 1,
            pageSize: 5,
            totalCount: 0,
            totalPages: 0
        }
    }),

    actions: {
        _normalizeEmployee(raw) {
            const roleNames = Array.isArray(raw?.roles) ? raw.roles.filter(Boolean) : []
            const roleIds = Array.isArray(raw?.roleIds) ? raw.roleIds.map(Number).filter(n => !isNaN(n)) : []

            return {
                ...raw,
                fullName: makeFullName(raw?.firstName, raw?.middleName, raw?.lastName),
                roles: roleNames,
                roleIds: Array.from(new Set(roleIds)),
                isDeleted: raw?.isDeleted || false
            }
        },

        async fetchEmployeesPaged(page = 1, pageSize = 5, isDeleted = false) {
            this.loading = true
            this.error = ''
            try {
                const params = new URLSearchParams({
                    page: String(page),
                    pageSize: String(pageSize),
                    isDeleted: String(isDeleted)
                })

                const res = await api.get(`/employees/paged?${params}`)
                const data = res?.data ?? {}

                const items =
                    Array.isArray(data.items) ? data.items
                        : Array.isArray(data.Items) ? data.Items
                            : []

                this.employees = items.map(r => this._normalizeEmployee(r))

                const pageFromApi = data.page ?? data.Page ?? page ?? 1
                const pageSizeFromApi = data.pageSize ?? data.PageSize ?? pageSize ?? 5
                const totalCountFromApi = data.totalCount ?? data.TotalCount ?? 0
                const totalPagesFromApi =
                    data.totalPages ?? data.TotalPages ?? Math.ceil(totalCountFromApi / (pageSizeFromApi || 1))

                this.pagination = {
                    currentPage: Number(pageFromApi),
                    pageSize: Number(pageSizeFromApi),
                    totalCount: Number(totalCountFromApi),
                    totalPages: Number(totalPagesFromApi)
                }
            } catch (err) {
                this.error = err?.response?.data?.message || 'Ошибка загрузки сотрудников'
            } finally {
                this.loading = false
            }
        },

        async fetchRoles() {
            try {
                const res = await api.get('/roles')
                const list = Array.isArray(res.data) ? res.data : (res.data?.data || [])
                this.roles = (Array.isArray(list) ? list : []).filter(r => !isSuperAdminName(r?.name))
            } catch (_err) {
                this.roles = []
            }
        },

        async fetchLegalEntities() {
            try {
                const res = await api.get('/legalentities')
                const list = Array.isArray(res.data) ? res.data : (res.data?.data || [])
                this.legals = Array.isArray(list) ? list : []
            } catch (err) {
                this.legals = []
            }
        },

        async fetchOfficesByLegal(legalEntityId) {
            if (!legalEntityId) {
                this.offices = []
                return []
            }
            try {
                const res = await api.get(`/offices/by-legal/${legalEntityId}`)
                const list = Array.isArray(res.data) ? res.data : (res.data?.data || [])
                this.offices = Array.isArray(list) ? list : []
                return this.offices
            } catch (err) {
                this.offices = []
                return []
            }
        },

        async fetchEmployeeById(id) {
            try {
                const response = await api.get(`/employees/${id}`)
                return this._normalizeEmployee(response.data)
            } catch (error) {
                this.error = error.response?.data?.message || 'Ошибка загрузки сотрудника'
                return null
            }
        },

        async createEmployee(dto) {
            try {
                this.loading = true
                const payload = sanitizePayload(dto)
                console.log('create payload', payload)
                const response = await api.post('/employees', payload)
                return response.data
            } finally {
                this.loading = false
            }
        },

        async updateEmployee(id, dto) {
            try {
                this.loading = true
                const payload = sanitizePayload(dto)
                console.log('update payload', payload)
                await api.put(`/employees/${id}`, payload)
            } finally {
                this.loading = false
            }
        },

        async deleteEmployee(id) {
            this.deletingId = id
            try {
                await api.delete(`/employees/${id}`)
            } finally {
                this.deletingId = null
            }
        },

        async restoreEmployee(id) {
            this.restoringId = id
            try {
                await api.patch(`/employees/${id}/restore`)
            } finally {
                this.restoringId = null
            }
        },
        async generatePassword(length = 12) {
            try {
                const res = await api.get('/employees/generate-password', {
                    params: { length }
                })
                return res.data?.password || ''
            } catch (err) {
                this.error = err?.response?.data?.message || 'Ошибка генерации пароля'
                return ''
            }
        }
    }
})
