<template>
  <v-container class="page pt-3">
    <div class="toolbar">
      <h1 class="text-h6 text-ink">{{ config?.title }}</h1>

      <PermissionTooltip :can="canCreate">
        <AppAddButton
          @click="openCreate"
          v-can.disable="'CreateDictionaries'"
          :disabled="!canCreate"
        />
      </PermissionTooltip>

      <div class="grow"></div>

      <v-text-field
        v-model="q"
        class="search-input"
        placeholder="Поиск: название"
        hide-details
        density="comfortable"
        variant="outlined"
        clearable
        :append-inner-icon="'mdi-magnify'"
        @keyup.enter="applySearch"
        @click:append-inner="applySearch"
        style="max-width: 360px"
      />
    </div>

    <div class="table-wrap bg-paper">
      <v-table density="comfortable">
        <thead>
        <tr>
          <th>#</th>
          <th
            v-for="f in config.fields"
            :key="f.key"
            @click="toggleSort(f.key)"
            style="cursor: pointer; user-select: none;"
          >
            {{ f.label }}
            <span v-if="sortBy === f.key">
              {{ sortOrder === 'asc' ? '▲' : '▼' }}
            </span>
          </th>
          <th class="ta-right">Действия</th>
        </tr>
        </thead>

        <tbody>
        <tr v-for="(r, i) in paged" :key="r.id">
          <td>{{ offset + i + 1 }}</td>
          <td v-for="f in config.fields" :key="f.key">{{ displayValue(r, f) }}</td>
          <td class="ta-right" @click.stop>
            <div class="row-actions">
              <PermissionTooltip :can="canUpdate">
                <AppIconBtn
                  icon="mdi-pencil"
                  title="Редактировать"
                  @click.stop="edit(r)"
                  v-can.disable="'EditDictionaries'"
                  :disabled="!canUpdate"
                />
              </PermissionTooltip>
              <PermissionTooltip :can="canDelete">
                <AppIconBtn
                  icon="mdi-trash-can"
                  title="Удалить"
                  @click.stop="removeRow(r)"
                  v-can.disable="'DeleteDictionaries'"
                  :disabled="!canDelete"
                />
              </PermissionTooltip>
            </div>
          </td>
        </tr>

        <tr v-if="!loading && !totalCount">
          <td :colspan="config.fields.length + 2" class="ta-center text-muted py-8">
            Данные не найдены
          </td>
        </tr>
        </tbody>
      </v-table>
    </div>

    <AppPager v-model:page="page" v-model:pageSize="pageSize" :total="totalCount" />
  </v-container>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useSessionStore } from '@/app/store/sessionStore'
import { dictionaries } from '/src/features/dictionaries/dictionaryConfig'
import dictionaryService from '@/features/dictionaries/services/dictionaryService'
import AppPager from '@/shared/components/AppPager.vue'
import AppAddButton from '@/shared/components/AppAddButton.vue'
import AppIconBtn from '@/shared/components/AppIconBtn.vue'
import PermissionTooltip from '@/shared/components/PermissionTooltip.vue'

const auth = useSessionStore()
const canCreate = computed(() => auth.has('CreateDictionaries'))
const canUpdate = computed(() => auth.has('EditDictionaries'))
const canDelete = computed(() => auth.has('DeleteDictionaries'))

const route = useRoute()
const router = useRouter()

const dictKey = computed(() => String(route.params.dict ?? ''))
const config = computed(() => dictionaries[dictKey.value])
if (!config.value) throw new Error(`Справочник "${dictKey.value}" не найден в dictionaryConfig`)

const allRows = ref([])
const loading = ref(false)
const lookups = ref({})

const page = ref(1)
const pageSize = ref(10)
const q = ref('')
const sortBy = ref(null)
const sortOrder = ref('asc')

const offset = computed(() => (page.value - 1) * pageSize.value)

const filtered = computed(() => {
  if (!q.value || !q.value.trim()) return allRows.value
  const term = q.value.trim().toLowerCase()
  return allRows.value.filter(r =>
    config.value.fields.some(f => {
      const v = r[f.key]
      return v != null && String(v).toLowerCase().includes(term)
    })
  )
})

const sorted = computed(() => {
  const arr = filtered.value.slice()
  if (!sortBy.value) return arr
  const key = sortBy.value
  arr.sort((a, b) => {
    const va = a[key] ?? ''
    const vb = b[key] ?? ''
    const na = Number(va)
    const nb = Number(vb)
    if (!Number.isNaN(na) && !Number.isNaN(nb)) {
      return sortOrder.value === 'asc' ? na - nb : nb - na
    }
    return sortOrder.value === 'asc'
      ? String(va).localeCompare(String(vb))
      : String(vb).localeCompare(String(va))
  })
  return arr
})

const paged = computed(() => sorted.value.slice(offset.value, offset.value + pageSize.value))
const totalCount = computed(() => sorted.value.length)

function displayValue(row, f) {
  const val = row[f.key] ?? row[f.key.charAt(0).toUpperCase() + f.key.slice(1)]
  if (val === null || val === undefined || val === '') return '—'
  if (f.type === 'select' && lookups.value[f.key]) {
    const label = lookups.value[f.key][val]
    if (label !== undefined) return label
  }
  return val
}

async function load() {
  if (!config.value) {
    allRows.value = []
    return
  }
  loading.value = true
  try {
    await loadLookups()
    allRows.value = await dictionaryService.fetchAll(config.value.endpoint, {})
    const maxPages = Math.max(1, Math.ceil(totalCount.value / pageSize.value))
    if (page.value > maxPages) page.value = maxPages
  } finally {
    loading.value = false
  }
}

async function loadLookups() {
  const selects = (config.value?.fields || []).filter(f => f.type === 'select' && f.source)
  const entries = await Promise.all(
    selects.map(async f => {
      const list = await dictionaryService.fetchAll(f.source)
      const map = Object.fromEntries((list || []).map(o => [o.id, o.name ?? o.title ?? o.id]))
      return [f.key, map]
    })
  )
  lookups.value = Object.fromEntries(entries)
}

function applySearch() { page.value = 1 }

watch(
  dictKey,
  () => {
    page.value = 1
    q.value = ''
    sortBy.value = null
    sortOrder.value = 'asc'
    load()
  },
  { immediate: true }
)

watch(pageSize, () => { page.value = 1 })

function toggleSort(key) {
  if (sortBy.value === key) {
    sortOrder.value = sortOrder.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortBy.value = key
    sortOrder.value = 'asc'
  }
}

function openCreate() {
  if (!canCreate.value) return
  router.push({ name: 'DictionaryCreate', params: { dict: dictKey.value } })
}

function edit(r) {
  if (!canUpdate.value) return
  router.push({ name: 'DictionaryEdit', params: { dict: dictKey.value, id: r.id } })
}

async function removeRow(r) {
  if (!canDelete.value) return
  if (!confirm(`Удалить «${r.name || r.title || r.id}»?`)) return
  await dictionaryService.remove(config.value.endpoint, r.id)
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
  cursor: pointer;
  user-select: none;
}

:deep(thead th:hover) {
  background: rgba(139, 146, 109, 0.05);
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

:deep(thead th span) {
  margin-left: 8px;
  font-size: 12px;
  color: var(--brand-primary);
  transition: all 0.3s ease;
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

.ta-right {
  text-align: right;
}

.ta-center {
  text-align: center;
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

:deep(.app-add-button) {
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

:deep(.app-add-button:hover) {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(206, 219, 149, 0.5) !important;
}

:deep(.app-add-button:disabled) {
  opacity: 0.5;
  transform: none !important;
  box-shadow: 0 2px 8px rgba(206, 219, 149, 0.2) !important;
}

@media (max-width: 1200px) {
  .table-wrap {
    overflow-x: auto;
  }

  :deep(table) {
    min-width: 800px;
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

  .row-actions {
    gap: 4px;
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
