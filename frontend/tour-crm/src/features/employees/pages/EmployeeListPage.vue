<template>
  <v-container class="page pt-3">
    <div class="toolbar">
      <h1 class="text-h6 text-ink">Сотрудники</h1>

      <PermissionTooltip :can="canCreate">
        <v-btn
          class="pill"
          color="secondary"
          :ripple="false"
          @click="openCreate"
          v-can.disable="'CreateEmployees'"
          :disabled="!canCreate"
        >
          + Добавить
        </v-btn>
      </PermissionTooltip>

      <div class="grow"></div>

      <v-text-field
        v-model="searchQuery"
        class="search-input"
        placeholder="Поиск: ФИО, email, офис"
        hide-details
        density="comfortable"
        variant="outlined"
        clearable
        :append-inner-icon="'mdi-magnify'"
        @keyup.enter="onSearchImmediate"
        @click:append-inner="onSearchImmediate"
        style="max-width: 360px"
      />

      <div class="tabs ml-4">
        <v-btn
          variant="text"
          :class="['tab-button', { active: activeTab === 'active' }]"
          @click="setActiveTab('active')"
        >
          Активные
        </v-btn>
        <v-btn
          variant="text"
          :class="['tab-button', { active: activeTab === 'deleted' }]"
          @click="setActiveTab('deleted')"
        >
          Удаленные
        </v-btn>
      </div>
    </div>

    <div class="table-wrap bg-paper">
      <v-table density="comfortable">
        <thead>
        <tr>
          <th>#</th>
          <th>Роли</th>
          <th>
            <div class="th-flex">
              <span>ФИО</span>
              <button
                  class="sort-button"
                  :class="{ active: sortField==='fio', asc: sortField==='fio' && sortOrder==='asc' }"
                  @click="toggleSort('fio')"
              >
                <v-icon size="16">mdi-arrow-up</v-icon>
              </button>
            </div>
          </th>
          <th>
            <div class="th-flex">
              <span>Офис</span>
              <button
                  class="sort-button"
                  :class="{ active: sortField==='office', asc: sortField==='office' && sortOrder==='asc' }"
                  @click="toggleSort('office')"
              >
                <v-icon size="16">mdi-arrow-up</v-icon>
              </button>
            </div>
          </th>
          <th>Email</th>
          <th>
            <div class="th-flex">
              <span>З/п</span>
              <button
                  class="sort-button"
                  :class="{ active: sortField==='salary', asc: sortField==='salary' && sortOrder==='asc' }"
                  @click="toggleSort('salary')"
              >
                <v-icon size="16">mdi-arrow-up</v-icon>
              </button>
            </div>
          </th>
          <th>Удалён</th>
          <th class="ta-right">Действия</th>
        </tr>
        </thead>

        <tbody>
        <tr v-if="store.loading">
          <td colspan="8" class="ta-center text-muted py-8">
            <v-progress-circular indeterminate/>
          </td>
        </tr>

        <tr
          v-for="(r, i) in sortedEmployees"
          :key="r.id"
          :class="{ deleted: r.isDeleted }"
        >
          <td>{{ (store.pagination.currentPage - 1) * store.pagination.pageSize + i + 1 }}</td>
          <td class="cell-strong">{{ formatRoles(r.roles) }}</td>
          <td>{{ formatFullName(r) }}</td>
          <td>{{ r.officeName || '—' }}</td>
          <td>{{ r.email || '—' }}</td>
          <td>{{ formatSalary(r.salaryAmount) }}</td>
          <td>{{ r.isDeleted ? 'Да' : 'Нет' }}</td>

          <td class="ta-right">
            <div class="row-actions">
              <PermissionTooltip :can="canUpdate" v-if="!r.isDeleted">
                <AppIconBtn
                  icon="mdi-pencil"
                  title="Редактировать"
                  @click="openEdit(r)"
                  v-can.disable="'EditEmployees'"
                  :disabled="!canUpdate"
                />
              </PermissionTooltip>

              <PermissionTooltip :can="canDelete" v-if="!r.isDeleted">
                <AppIconBtn
                  icon="mdi-trash-can"
                  title="Удалить"
                  :loading="store.deletingId === r.id"
                  @click="onDelete(r)"
                  v-can.disable="'DeleteEmployees'"
                  :disabled="!canDelete"
                />
              </PermissionTooltip>

              <PermissionTooltip :can="canDelete" v-else>
                <AppIconBtn
                  icon="mdi-restore"
                  title="Восстановить"
                  :loading="store.restoringId === r.id"
                  @click="onRestore(r)"
                  v-can.disable="'DeleteEmployees'"
                  :disabled="!canDelete"
                />
              </PermissionTooltip>
            </div>
          </td>
        </tr>

        <tr v-if="!store.loading && sortedEmployees.length === 0">
          <td colspan="8" class="ta-center text-muted py-8">Данные не найдены</td>
        </tr>
        </tbody>
      </v-table>
    </div>

    <AppPager
      v-model:page="page"
      v-model:pageSize="pageSize"
      :total="total"
      :size-options="[10, 20, 25, 50]"
    />
  </v-container>
</template>

<script setup>
import {ref, computed, watch, onMounted} from 'vue'
import {useRoute, useRouter} from 'vue-router'
import {useEmployeeStore} from '@/app/store/employeeStore'
import {useSessionStore} from '@/app/store/sessionStore'
import AppIconBtn from '@/shared/components/AppIconBtn.vue'
import AppPager from '@/shared/components/AppPager.vue'
import PermissionTooltip from '@/shared/components/PermissionTooltip.vue'

const router = useRouter()
const route = useRoute()
const store = useEmployeeStore()
const auth = useSessionStore()

const canCreate = computed(() => auth.has('CreateEmployees'))
const canUpdate = computed(() => auth.has('EditEmployees'))
const canDelete = computed(() => auth.has('DeleteEmployees'))

const activeTab = ref(route.query.isDeleted === 'true' ? 'deleted' : 'active')
const searchQuery = ref(route.query.search || '')
const page = ref(Number(route.query.page) || 1)
const pageSize = ref(Number(route.query.pageSize) || 10)
const sortField = ref(route.query.sortField || null)
const sortOrder = ref(route.query.sortOrder || null)

let searchTimeout = null

onMounted(async () => {
  await store.fetchRoles()
  await loadEmployees()
})

function toggleSort(field) {
  if (sortField.value === field) {
    sortOrder.value = sortOrder.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortField.value = field
    sortOrder.value = 'asc'
  }
  page.value = 1
  loadEmployees()
}

async function loadEmployees() {
  await router.replace({
    query: {
      page: page.value,
      pageSize: pageSize.value,
      isDeleted: activeTab.value === 'deleted',
      search: searchQuery.value?.trim() || undefined,
      sortField: sortField.value || undefined,
      sortOrder: sortOrder.value || undefined,
    },
  })
  await store.fetchRoles()
  await store.fetchEmployeesPaged(
    page.value,
    pageSize.value,
    activeTab.value === 'deleted',
    searchQuery.value.trim(),
    sortField.value,
    sortOrder.value
  )
}

watch([page, pageSize, activeTab, sortField, sortOrder], loadEmployees)

function setActiveTab(tab) { activeTab.value = tab; page.value = 1; loadEmployees() }
function onSearchImmediate() { if (searchTimeout) clearTimeout(searchTimeout); page.value = 1; loadEmployees() }
function onSearch() { clearTimeout(searchTimeout); searchTimeout = setTimeout(() => { page.value = 1; loadEmployees() }, 400) }
watch(searchQuery, onSearch)

const total = computed(() => store.pagination.totalCount)

function openCreate() { if (!canCreate.value) return; router.push('/employees/create') }
function openEdit(row) { if (!canUpdate.value) return; router.push(`/employees/edit/${row.id}`) }

async function onDelete(row) {
  if (!canDelete.value) return
  if (!row?.id) return
  if (!confirm('Удалить сотрудника?')) return
  await store.deleteEmployee(row.id)
  await loadEmployees()
}

async function onRestore(row) {
  if (!canDelete.value) return
  if (!row?.id) return
  await store.restoreEmployee(row.id)
  await loadEmployees()
}

function formatFullName(e) { const name = [e.firstName, e.middleName, e.lastName].filter(Boolean).join(' '); return name || e.fullName || '—' }
function formatRoles(roles) { return Array.isArray(roles) && roles.length ? roles.join(', ') : '—' }
function formatSalary(val) { if (val == null) return '—'; const num = Number(val); if (Number.isNaN(num)) return '—'; return num.toLocaleString('ru-RU') }

const normalize = (s) => String(s || '').toLowerCase().replace(/\s+/g, ' ').trim()
const filteredEmployees = computed(() => {
  const q = normalize(searchQuery.value)
  if (!q) return store.employees
  return store.employees.filter(e => {
    const fio1 = normalize([e.firstName, e.middleName, e.lastName].filter(Boolean).join(' '))
    const fio2 = normalize([e.lastName, e.firstName, e.middleName].filter(Boolean).join(' '))
    const email = normalize(e.email)
    const office = normalize(e.officeName)
    return fio1.includes(q) || fio2.includes(q) || email.includes(q) || office.includes(q)
  })
})

const sortedEmployees = computed(() => {
  const arr = [...filteredEmployees.value]
  if (!sortField.value || !sortOrder.value) return arr
  const dir = sortOrder.value === 'asc' ? 1 : -1
  const cmpStr = (a, b) => { const sa=String(a||'').toLocaleLowerCase(), sb=String(b||'').toLocaleLowerCase(); if (sa<sb) return -1*dir; if (sa>sb) return 1*dir; return 0 }
  const cmpNum = (a, b) => { const na=Number(a??0), nb=Number(b??0); if (na<nb) return -1*dir; if (na>nb) return 1*dir; return 0 }
  return arr.sort((a, b) => {
    if (sortField.value === 'fio') {
      const aF = [a.lastName, a.firstName, a.middleName].filter(Boolean).join(' ')
      const bF = [b.lastName, b.firstName, b.middleName].filter(Boolean).join(' ')
      return cmpStr(aF, bF)
    }
    if (sortField.value === 'office') return cmpStr(a.officeName, b.officeName)
    if (sortField.value === 'salary') return cmpNum(a.salaryAmount, b.salaryAmount)
    return 0
  })
})
</script>

<style scoped>
.page {
  width: 100%;
  padding-inline: 24px;
  box-sizing: border-box;
  background: linear-gradient(135deg, var(--color-baby-powder) 0%, rgba(242, 243, 237, 0.3) 100%);
  min-height: calc(100vh - 64px);
}

.toolbar {
  display: flex;
  align-items: center;
  gap: 20px;
  margin: 32px 0 24px 0;
  position: relative;
  overflow: hidden;
}

.toolbar h1 {
  font-size: 24px;
  font-weight: 800;
  background: black;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin: 0;
  letter-spacing: -0.2px;
}

.grow {
  flex: 1;
}

.pill {
  border-radius: 14px !important;
  background: linear-gradient(135deg, var(--color-pear) 0%, #d4e2a8 100%) !important;
  color: var(--brand-ink) !important;
  font-weight: 600;
  text-transform: none;
  letter-spacing: 0.3px;
  padding: 0 20px !important;
  height: 40px !important;
  transition: all 0.3s ease !important;
  box-shadow: 0 4px 15px rgba(206, 219, 149, 0.3) !important;
  border: 1px solid rgba(206, 219, 149, 0.3) !important;
}

.pill:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(206, 219, 149, 0.5) !important;
}

.pill:disabled {
  opacity: 0.5;
  transform: none !important;
  box-shadow: 0 2px 8px rgba(206, 219, 149, 0.2) !important;
}

.search-input {
  max-width: 360px;
}

.search-input :deep(.v-field) {
  border-radius: 12px;
  background: rgba(255, 255, 255, 0.9);
  border: 1.5px solid rgba(139, 146, 109, 0.15);
  transition: all 0.3s ease;
}

.search-input :deep(.v-field:focus-within) {
  border-color: var(--brand-primary);
  box-shadow: 0 4px 15px rgba(139, 146, 109, 0.15);
}

.search-input :deep(.v-field__append-inner) {
  color: var(--brand-primary);
}

.tabs {
  display: flex;
  gap: 4px;
  background: rgba(255, 255, 255, 0.8);
  border-radius: 12px;
  padding: 4px;
  border: 1.5px solid rgba(139, 146, 109, 0.1);
  backdrop-filter: blur(8px);
}

.tab-button {
  padding: 10px 20px !important;
  border-radius: 10px !important;
  font-weight: 500;
  text-transform: none;
  letter-spacing: 0.2px;
  color: var(--brand-ink) !important;
  opacity: 0.7;
  transition: all 0.3s ease !important;
  background: transparent !important;
  min-width: auto !important;
}

.tab-button:hover {
  opacity: 0.9;
  background: rgba(139, 146, 109, 0.08) !important;
}

.tab-button.active {
  opacity: 1;
  background: linear-gradient(135deg, var(--color-pear) 0%, rgba(206, 219, 149, 0.8) 100%) !important;
  color: var(--brand-ink) !important;
  font-weight: 600;
  box-shadow: 0 2px 8px rgba(206, 219, 149, 0.3);
}

.table-wrap {
  border: 1px solid rgba(139, 146, 109, 0.1);
  border-radius: 20px 20px 0 0;
  overflow: hidden;
  margin-top: 24px;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.95) 0%, rgba(242, 243, 237, 0.3) 100%);
  backdrop-filter: blur(10px);
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.06);
  position: relative;
}

.table-wrap::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 1px;
  background: linear-gradient(90deg, transparent, var(--color-pear), transparent);
}

:deep(table) {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
}

:deep(thead) {
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.9) 0%, rgba(242, 243, 237, 0.8) 100%);
}

:deep(thead th) {
  font-weight: 700;
  color: var(--brand-ink);
  border-bottom: 2px solid rgba(139, 146, 109, 0.2) !important;
  background: transparent;
  padding: 20px 16px;
  font-size: 14px;
  letter-spacing: 0.3px;
  position: relative;
  transition: all 0.3s ease;
}

:deep(thead th::after) {
  content: '';
  position: absolute;
  bottom: -2px;
  left: 0;
  width: 0;
  height: 2px;
  background: linear-gradient(90deg, var(--color-pear), var(--brand-primary));
  transition: width 0.3s ease;
}

:deep(thead th:hover::after) {
  width: 100%;
}

.th-flex {
  display: flex;
  align-items: center;
  gap: 8px;
}

.sort-arrows {
  display: inline-flex;
  flex-direction: column;
  gap: 2px;
}

.sort-arrow {
  width: 24px;
  height: 20px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border: none;
  border-radius: 4px;
  background: transparent;
  cursor: pointer;
  transition: all 0.2s ease;
  color: var(--brand-primary);
  opacity: 0.6;
}

.sort-arrow:hover {
  background: rgba(139, 146, 109, 0.1);
  opacity: 0.8;
  transform: scale(1.1);
}

.sort-arrow.active,
.sort-arrow:disabled {
  background: rgba(139, 146, 109, 0.15);
  opacity: 1;
  color: var(--brand-primary);
}

.sort-arrow:disabled {
  cursor: not-allowed;
  transform: none;
}

:deep(tbody tr) {
  transition: all 0.3s ease;
  background: transparent;
}

.sort-button {
  width: 24px;
  height: 24px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border: none;
  border-radius: 6px;
  background: transparent;
  cursor: pointer;
  transition: all 0.3s ease;
  color: var(--color-gray);
  opacity: 0.6;
}

.sort-button:hover {
  background: rgba(139, 146, 109, 0.1);
  opacity: 0.8;
  transform: scale(1.1);
}

.sort-button.active {
  opacity: 1;
  background: rgba(139, 146, 109, 0.15);
  color: var(--brand-primary);
}

.sort-button.asc {
  transform: rotate(0deg);
}

.sort-button:not(.asc) {
  transform: rotate(180deg);
}

.sort-button.active:not(.asc) {
  transform: rotate(180deg) scale(1.1);
}

:deep(tbody tr:hover) {
  background: rgba(139, 146, 109, 0.05) !important;
  transform: translateY(-1px);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
}

:deep(tbody td) {
  border-bottom: 1px solid rgba(139, 146, 109, 0.08);
  padding: 16px;
  transition: all 0.3s ease;
  position: relative;
}

:deep(tbody tr:hover td) {
  border-color: rgba(139, 146, 109, 0.15);
}

:deep(tbody tr:last-child td) {
  border-bottom: none;
}

.cell-strong {
  font-weight: 700;
  color: var(--brand-ink);
}

.ta-right {
  text-align: right;
}

.ta-center {
  text-align: center;
}

.deleted td {
  color: rgba(119, 119, 119, 0.7);
  background: rgba(0, 0, 0, 0.02);
}

.deleted:hover td {
  background: rgba(0, 0, 0, 0.04) !important;
  color: rgba(119, 119, 119, 0.9);
}

.deleted .cell-strong {
  color: rgba(119, 119, 119, 0.8);
}

.row-actions {
  display: flex;
  gap: 6px;
  justify-content: flex-end;
  align-items: center;
  white-space: nowrap;
}

:deep(.row-actions .v-btn) {
  min-width: 0;
  padding: 0;
  border-radius: 8px !important;
  transition: all 0.3s ease !important;
}

:deep(.row-actions .v-btn:hover) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(139, 146, 109, 0.3);
}

:deep(.app-pager) {
  margin-top: 24px;
  padding: 20px;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.9) 0%, rgba(242, 243, 237, 0.6) 100%);
  border-radius: 16px;
  border: 1px solid rgba(139, 146, 109, 0.1);
  backdrop-filter: blur(10px);
}

:deep(.permission-tooltip) {
  display: inline-flex;
}

.text-muted {
  color: var(--color-gray);
  opacity: 0.8;
}

:deep(.v-progress-circular) {
  color: var(--brand-primary);
}

@media (max-width: 1200px) {
  .table-wrap {
    overflow-x: auto;
  }

  :deep(table) {
    min-width: 1000px;
  }
}

@media (max-width: 960px) {
  .page {
    padding-inline: 16px;
  }

  .toolbar {
    padding: 16px 20px;
    flex-wrap: wrap;
    gap: 16px;
  }

  .toolbar h1 {
    font-size: 20px;
  }

  .search-input {
    max-width: 100%;
    order: 3;
    flex: 1 0 100%;
  }

  .tabs {
    order: 4;
    width: 100%;
    justify-content: center;
  }
}

@media (max-width: 600px) {
  .page {
    padding-inline: 12px;
  }

  .toolbar {
    padding: 12px 16px;
    margin: 24px 0 20px 0;
  }

  .toolbar h1 {
    font-size: 18px;
  }

  .table-wrap {
    border-radius: 16px;
    margin-top: 20px;
  }

  :deep(table) {
    font-size: 14px;
  }

  :deep(thead th),
  :deep(tbody td) {
    padding: 12px 8px;
  }

  .tab-button {
    padding: 8px 16px !important;
    font-size: 14px;
  }

  .sort-arrow {
    width: 20px;
    height: 18px;
  }

  .sort-arrow :deep(.v-icon) {
    font-size: 14px;
  }
}

:deep(.table-wrap)::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}

:deep(.table-wrap)::-webkit-scrollbar-track {
  background: rgba(139, 146, 109, 0.1);
  border-radius: 3px;
}

:deep(.table-wrap)::-webkit-scrollbar-thumb {
  background: rgba(139, 146, 109, 0.3);
  border-radius: 3px;
}

:deep(.table-wrap)::-webkit-scrollbar-thumb:hover {
  background: rgba(139, 146, 109, 0.5);
}
</style>
