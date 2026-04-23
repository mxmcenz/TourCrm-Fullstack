<template>
  <v-container class="page pt-3">
    <div class="toolbar">
      <h2 class="text-h6 text-ink">Офисы компании</h2>

      <PermissionTooltip :can="canCreate">
        <AppAddButton
          @click="openCreate"
          v-can.disable="'CreateOffices'"
          :disabled="!canCreate"
        />
      </PermissionTooltip>

      <div class="grow"></div>

      <v-text-field
        v-model="q"
        class="search-input"
        placeholder="Поиск: название, адрес/город, юрлицо, телефон, email, кол-во сотрудников"
        hide-details density="comfortable" variant="outlined" clearable
        :append-inner-icon="'mdi-magnify'"
        @keyup.enter="applySearch"
        @click:append-inner="applySearch"
        style="max-width: 360px"
      />
    </div>

    <div v-if="!canView" class="content-wrap bg-paper ta-center py-8">Нет доступа</div>

    <div v-else class="table-wrap bg-paper">
      <v-table density="comfortable">
        <thead>
        <tr>
          <th>#</th>
          <th>Название</th>
          <th>Город</th>
          <th>Юр.лицо</th>
          <th>Телефоны</th>
          <th>Email (Основной)</th>
          <th>Кол-во сотрудников</th>
          <th class="ta-right">Действия</th>
        </tr>
        </thead>
        <tbody>
        <tr v-for="(r, i) in paged" :key="r.id">
          <td>{{ offset + i + 1 }}</td>
          <td class="cell-strong">
            <PermissionTooltip :can="canEdit">
              <RouterLink
                v-if="canEdit"
                class="no-underline"
                :to="{ name: 'OfficeEdit', params: { id: r.id }, query: { legal: legalId } }"
                v-can.disable="'EditOffices'"
              >{{ r.name }}</RouterLink>
            </PermissionTooltip>
            <span v-if="!canEdit" class="no-underline">{{ r.name }}</span>
          </td>
          <td>{{ r.city || r.address || '—' }}</td>
          <td>{{ r.legalEntityName || '—' }}</td>
          <td>{{ r.phones || r.phone || '—' }}</td>
          <td>{{ r.email || '—' }}</td>
          <td>{{ r.employeesCount ?? '—' }}</td>
          <td class="ta-right">
            <PermissionTooltip :can="canEdit">
              <AppIconBtn
                icon="mdi-pencil"
                aria-label="Редактировать"
                @click="edit(r)"
                v-can.disable="'EditOffices'"
                :disabled="!canEdit"
              />
            </PermissionTooltip>
            <PermissionTooltip :can="canDelete">
              <AppIconBtn
                icon="mdi-trash-can"
                aria-label="Удалить"
                @click="removeRow(r)"
                v-can.disable="'DeleteOffices'"
                :disabled="!canDelete"
              />
            </PermissionTooltip>
          </td>
        </tr>

        <tr v-if="!loading && !rows.length">
          <td colspan="8" class="ta-center text-muted py-8">Данные не найдены</td>
        </tr>
        </tbody>
      </v-table>
    </div>

    <AppPager v-if="canView" v-model:page="page" v-model:pageSize="pageSize" :total="rows.length" />
  </v-container>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useSessionStore } from '@/app/store/sessionStore'
import { fetchOfficesByLegal, deleteOffice } from '../services/officesService'
import { fetchLegalEntities } from '@/features/touragent/services/companyService'
import AppIconBtn from '@/shared/components/AppIconBtn.vue'
import AppPager from '@/shared/components/AppPager.vue'
import AppAddButton from '@/shared/components/AppAddButton.vue'
import PermissionTooltip from '@/shared/components/PermissionTooltip.vue'

const auth = useSessionStore()
const canView = computed(() => auth.has('ViewOffices'))
const canCreate = computed(() => auth.has('CreateOffices'))
const canEdit = computed(() => auth.has('EditOffices'))
const canDelete = computed(() => auth.has('DeleteOffices'))

const route = useRoute()
const router = useRouter()
const legalId = computed(() => Number(route.query.legal || route.query.legalEntityId || 0))

const rows = ref([])
const loading = ref(false)
const page = ref(1)
const pageSize = ref(10)
const q = ref('')

const offset = computed(() => (page.value - 1) * pageSize.value)
const paged = computed(() => rows.value.slice(offset.value, offset.value + pageSize.value))

async function ensureLegalId() {
  if (legalId.value) return true
  const legals = await fetchLegalEntities()
  const first = Array.isArray(legals) && legals.length ? legals[0].id : null
  if (first) {
    await router.replace({ name: route.name, query: { ...route.query, legal: first } })
    return true
  }
  rows.value = []
  return false
}

function applySearch() { page.value = 1; load() }

let currentAbort = null
async function load() {
  if (!canView.value) return
  if (!(await ensureLegalId())) return

  if (currentAbort) currentAbort.abort()
  currentAbort = new AbortController()
  const { signal } = currentAbort

  loading.value = true
  try {
    const list = await fetchOfficesByLegal(legalId.value, { q: q.value?.trim() || undefined }, { signal })
    if (signal.aborted) return
    rows.value = Array.isArray(list) ? list : []
    const newPages = Math.max(1, Math.ceil(rows.value.length / pageSize.value))
    if (page.value > newPages) page.value = newPages
  } catch (e) {
    if (e?.name === 'CanceledError' || e?.code === 'ERR_CANCELED' || e?.message === 'canceled') return
    console.error(e)
  } finally {
    if (!signal.aborted) loading.value = false
  }
}

let searchTimer
watch(q, () => {
  clearTimeout(searchTimer)
  searchTimer = setTimeout(() => { page.value = 1; load() }, 300)
})
watch(() => route.query, load, { immediate: true })
watch(pageSize, () => { page.value = 1; load() })

function openCreate() {
  if (!canCreate.value) return
  router.push({ name: 'OfficeCreate', query: { legal: legalId.value || undefined } })
}
function edit(r) {
  if (!canEdit.value) return
  router.push({ name: 'OfficeEdit', params: { id: r.id }, query: { legal: legalId.value || undefined } })
}
async function removeRow(r) {
  if (!canDelete.value) return
  if (!confirm(`Удалить офис «${r.name}»?`)) return
  await deleteOffice(r.id)
  await load()
}
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

.toolbar h2 {
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

.search-input {
  max-width: 420px;
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

.content-wrap {
  border: 1px solid rgba(139, 146, 109, 0.1);
  border-radius: 20px;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.95) 0%, rgba(242, 243, 237, 0.3) 100%);
  backdrop-filter: blur(10px);
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.06);
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

:deep(tbody tr) {
  transition: all 0.3s ease;
  background: transparent;
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
}

.ta-right {
  text-align: right;
}

.ta-center {
  text-align: center;
}

.no-underline {
  text-decoration: none;
  color: inherit;
  font-weight: 600;
  transition: all 0.3s ease;
  display: inline-block;
}

.no-underline:hover {
  color: var(--brand-primary);
  transform: translateY(-1px);
}

:deep(.app-icon-btn) {
  border-radius: 10px !important;
  transition: all 0.3s ease !important;
  margin: 0 2px;
}

:deep(.app-icon-btn:hover) {
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

:deep(.app-add-button) {
  border-radius: 12px !important;
  background: linear-gradient(135deg, var(--color-pear) 0%, #d4e2a8 100%) !important;
  color: var(--brand-ink) !important;
  font-weight: 600;
  transition: all 0.3s ease !important;
  box-shadow: 0 4px 15px rgba(206, 219, 149, 0.3) !important;
  border: 1px solid rgba(206, 219, 149, 0.3) !important;
}

:deep(.app-add-button:hover) {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(206, 219, 149, 0.5) !important;
}

:deep(.permission-tooltip) {
  display: inline-flex;
}

.text-muted {
  color: var(--color-gray);
  opacity: 0.8;
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