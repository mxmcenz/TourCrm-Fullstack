<template>
  <v-container class="page pt-3">
    <div class="toolbar">
      <h1 class="text-h6 text-ink">Лиды</h1>

      <PermissionTooltip :can="canCreate">
        <AppAddButton
          @click="openCreate"
          v-can.disable="'CreateLeads'"
          :disabled="!canCreate"
        />
      </PermissionTooltip>

      <div class="grow"></div>

      <v-text-field
        v-model="q"
        class="search-input"
        placeholder="Поиск: № заявки, ФИО, телефон, email"
        hide-details
        density="comfortable"
        variant="outlined"
        clearable
        :append-inner-icon="'mdi-magnify'"
        @keyup.enter="applySearch"
        @click:append-inner="applySearch"
        style="max-width: 260px"
      />
    </div>

    <div v-if="!canView" class="content-wrap bg-paper ta-center py-8">
      Нет доступа
    </div>

    <div v-else class="table-wrap bg-paper">
      <v-table density="comfortable">
        <thead>
        <tr>
          <th class="sortable" @click="toggleSort('createdAt')">#</th>
          <th class="sortable" @click="toggleSort('status')">
            Статус
            <v-icon size="18" class="sort-ico" :icon="sortIcon('status')" />
          </th>
          <th class="sortable" @click="toggleSort('createdAt')">
            Дата создания
            <v-icon size="18" class="sort-ico" :icon="sortIcon('createdAt')" />
          </th>
          <th class="sortable" @click="toggleSort('travelDate')">
            Дата поездки
            <v-icon size="18" class="sort-ico" :icon="sortIcon('travelDate')" />
          </th>
          <th class="sortable" @click="toggleSort('nights')">
            Кол-во ночей
            <v-icon size="18" class="sort-ico" :icon="sortIcon('nights')" />
          </th>
          <th class="sortable" @click="toggleSort('country')">
            Страна
            <v-icon size="18" class="sort-ico" :icon="sortIcon('country')" />
          </th>
          <th class="sortable ta-right" @click="toggleSort('budget')">
            Бюджет
            <v-icon size="18" class="sort-ico" :icon="sortIcon('budget')" />
          </th>
          <th class="sortable" @click="toggleSort('manager')">
            Менеджер
            <v-icon size="18" class="sort-ico" :icon="sortIcon('manager')" />
          </th>
          <th class="ta-right">Действия</th>
        </tr>
        </thead>

        <tbody>
        <tr v-for="(r, i) in rows" :key="r.id">
          <td class="cell-strong">{{ offset + i + 1 }}</td>
          <td>{{ r.status || '—' }}</td>
          <td>{{ formatDate(r.createdAt) }}</td>
          <td>{{ formatDate(r.travelDate) || '—' }}</td>
          <td>{{ computeNights(r) }}</td>
          <td>{{ r.country || '—' }}</td>
          <td class="ta-right">{{ formatMoney(r.budget) }}</td>
          <td>{{ r.manager ?? r.managerFullName ?? '—' }}</td>

          <td class="ta-right">
            <PermissionTooltip :can="canEdit">
              <AppIconBtn
                icon="mdi-pencil"
                aria-label="Редактировать"
                @click="edit(r)"
                v-can.disable="'EditLeads'"
                :disabled="!canEdit"
              />
            </PermissionTooltip>

            <PermissionTooltip :can="canDelete">
              <AppIconBtn
                icon="mdi-trash-can"
                aria-label="Удалить"
                @click="removeRow(r)"
                v-can.disable="'DeleteLeads'"
                :disabled="!canDelete"
              />
            </PermissionTooltip>
          </td>
        </tr>

        <tr v-if="!loading && !rows.length">
          <td colspan="9" class="ta-center text-muted py-8">Данные не найдены</td>
        </tr>
        </tbody>
      </v-table>
    </div>

    <AppPager v-if="canView" v-model:page="page" v-model:pageSize="pageSize" :total="total" />
  </v-container>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useSessionStore } from '@/app/store/sessionStore'
import { fetchLeadsPage, deleteLead } from '../services/leadsService'

import AppIconBtn from '@/shared/components/AppIconBtn.vue'
import AppPager from '@/shared/components/AppPager.vue'
import AppAddButton from '@/shared/components/AppAddButton.vue'
import PermissionTooltip from '@/shared/components/PermissionTooltip.vue'

const router = useRouter()
const auth = useSessionStore()

const canView   = computed(() => auth.has('ViewLeads'))
const canCreate = computed(() => auth.has('CreateLeads'))
const canEdit   = computed(() => auth.has('EditLeads'))
const canDelete = computed(() => auth.has('DeleteLeads'))

function sortIcon(by) {
  if (sortBy.value !== by) return 'mdi-arrow-down'
  return sortDir.value === 'desc' ? 'mdi-arrow-down' : 'mdi-arrow-up'
}

const rows = ref([])
const total = ref(0)
const loading = ref(false)

const page = ref(1)
const pageSize = ref(10)
const offset = computed(() => (page.value - 1) * pageSize.value)

const q = ref('')
const sortBy = ref('createdAt')
const sortDir = ref('desc')

function toggleSort(by) {
  if (sortBy.value === by) {
    sortDir.value = sortDir.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortBy.value = by
    sortDir.value = 'asc'
  }
  page.value = 1
  load()
}

function formatDate(d) {
  if (!d) return ''
  const date = new Date(d)
  return date.toLocaleDateString()
}

function formatMoney(v) {
  if (v == null) return '—'
  try { return new Intl.NumberFormat(undefined, { maximumFractionDigits: 0 }).format(v) }
  catch { return String(v) }
}

function openCreate() {
  if (!canCreate.value) return
  router.push({ name: 'LeadCreate' })
}

function edit(r) {
  if (!canEdit.value) return
  router.push({ name: 'LeadEdit', params: { id: r.id } })
}

async function removeRow(r) {
  if (!canDelete.value) return
  if (!confirm(`Удалить лид № ${r.leadNumber || r.id}?`)) return
  await deleteLead(r.id)
  await load()
}

function applySearch() { page.value = 1; load() }

function diffNights(a, b) {
  if (!a || !b) return null
  const d1 = new Date(a), d2 = new Date(b)
  const t1 = Date.UTC(d1.getFullYear(), d1.getMonth(), d1.getDate())
  const t2 = Date.UTC(d2.getFullYear(), d2.getMonth(), d2.getDate())
  return Math.max(0, Math.round((t2 - t1) / 86400000))
}

function computeNights(r) {
  if (Number.isFinite(r?.nights)) return r.nights
  const byDesired = diffNights(r?.desiredArrival, r?.desiredDeparture)
  if (byDesired != null) return byDesired
  const byDates = diffNights(r?.arrivalDate, r?.departureDate)
  if (byDates != null) return byDates
  const { nightsFrom: from, nightsTo: to } = r || {}
  if (Number.isFinite(from) && Number.isFinite(to)) return `${from}–${to}`
  if (Number.isFinite(from)) return from
  if (Number.isFinite(to)) return to
  return '—'
}

let currentAbort = null
async function load() {
  if (!canView.value) return
  if (currentAbort) currentAbort.abort()
  currentAbort = new AbortController()
  const { signal } = currentAbort

  loading.value = true
  try {
    const { items, total: t } = await fetchLeadsPage({
      page: page.value,
      pageSize: pageSize.value,
      query: q.value?.trim() || undefined,
      sortBy: sortBy.value,
      sortDir: sortDir.value
    }, { signal })

    if (signal.aborted) return
    rows.value = Array.isArray(items) ? items : []
    total.value = Number.isFinite(t) ? t : 0

    const maxPage = Math.max(1, Math.ceil(total.value / pageSize.value))
    if (page.value > maxPage) page.value = maxPage
  } catch (e) {
    if (e?.name !== 'CanceledError' && e?.code !== 'ERR_CANCELED' && e?.message !== 'canceled') console.error(e)
  } finally {
    if (!signal.aborted) loading.value = false
  }
}

let searchTimer
watch(q, () => {
  clearTimeout(searchTimer)
  searchTimer = setTimeout(() => { page.value = 1; load() }, 300)
})
watch(page, load)
watch(pageSize, () => { page.value = 1; load() })
watch(canView, (v) => { if (v) load() })

onMounted(load)
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

.search-input {
  max-width: 280px;
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

.sortable {
  cursor: pointer;
  user-select: none;
  transition: all 0.3s ease;
}

.sortable:hover {
  background: rgba(139, 146, 109, 0.05);
}

:deep(.sort-ico) {
  margin-left: 8px;
  vertical-align: middle;
  color: var(--brand-primary);
  opacity: 0.7;
  transition: all 0.3s ease;
}

.sortable:hover :deep(.sort-ico) {
  opacity: 1;
  transform: scale(1.1);
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
  color: var(--brand-ink);
}

.ta-right {
  text-align: right;
}

.ta-center {
  text-align: center;
}

/* Стили для статусов */
:deep(tbody td:nth-child(2)) {
  font-weight: 500;
  position: relative;
}

:deep(tbody td:nth-child(2))::before {
  content: '';
  position: absolute;
  left: 4px;
  top: 50%;
  transform: translateY(-50%);
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: var(--color-pear);
}

:deep(tbody td:nth-child(7)) {
  font-weight: 600;
  color: var(--brand-primary);
  font-family: 'Courier New', monospace;
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

  :deep(.sort-ico) {
    font-size: 16px;
    margin-left: 4px;
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