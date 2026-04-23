<template>
  <v-container class="page pt-3">
    <div class="toolbar">
      <h1 class="text-h6 text-ink">Клиенты</h1>

      <PermissionTooltip :can="canCreate">
        <v-btn
          class="pill"
          color="secondary"
          :ripple="false"
          @click="openCreate"
          v-can.disable="'CreateClients'"
          :disabled="!canCreate"
        >
          + Добавить
        </v-btn>
      </PermissionTooltip>

      <div class="grow"></div>

      <v-text-field
        v-model="q"
        class="search-input"
        placeholder="Поиск: имя, фамилия, телефон, email"
        hide-details
        density="comfortable"
        variant="outlined"
        clearable
        :append-inner-icon="'mdi-magnify'"
        @keyup.enter="applySearch"
        @click:append-inner="applySearch"
        style="max-width: 360px"
      />

      <v-switch
        v-model="onlyTourists"
        inset
        hide-details
        color="primary"
        :label="'Только туристы'"
        class="ml-4"
      />

      <v-switch
        v-model="includeDeleted"
        inset
        hide-details
        color="primary"
        :label="'Показывать удалённых'"
        class="ml-2"
      />
    </div>

    <div class="table-wrap bg-paper">
      <v-table density="comfortable">
        <thead>
        <tr>
          <th>#</th>
          <th>Имя</th>
          <th>Фамилия</th>
          <th>Телефон</th>
          <th>Email</th>
          <th>Тип</th>
          <th>Турист</th>
          <th>Удалён</th>
          <th class="ta-right">Действия</th>
        </tr>
        </thead>

        <tbody>
        <tr
          v-for="(r, i) in viewRows"
          :key="r.id"
          class="row-click"
          @click="openDetails(r)"
          :class="{ deleted: r.isDeleted }"
        >
          <td>{{ offset + i + 1 }}</td>
          <td class="cell-strong">{{ r.firstName || '—' }}</td>
          <td>{{ r.lastName || '—' }}</td>
          <td>{{ r.phoneE164 || '—' }}</td>
          <td>{{ r.email || '—' }}</td>
          <td>{{ clientTypeName(r.clientType) }}</td>
          <td>{{ r.isTourist ? 'Да' : 'Нет' }}</td>
          <td>{{ r.isDeleted ? 'Да' : 'Нет' }}</td>
          <td class="ta-right" @click.stop>
            <div class="row-actions">
              <PermissionTooltip :can="canUpdate">
                <AppIconBtn
                  icon="mdi-pencil"
                  title="Редактировать"
                  @click.stop="edit(r)"
                  v-can.disable="'EditClients'"
                  :disabled="!canUpdate"
                />
              </PermissionTooltip>

              <PermissionTooltip :can="canDelete" v-if="!r.isDeleted">
                <AppIconBtn
                  icon="mdi-trash-can"
                  title="Удалить"
                  @click.stop="removeRow(r)"
                  v-can.disable="'DeleteClients'"
                  :disabled="!canDelete"
                />
              </PermissionTooltip>

              <PermissionTooltip :can="canDelete" v-else>
                <AppIconBtn
                  icon="mdi-restore"
                  title="Восстановить"
                  @click.stop="restoreRow(r)"
                  v-can.disable="'DeleteClients'"
                  :disabled="!canDelete"
                />
              </PermissionTooltip>
            </div>
          </td>
        </tr>

        <tr v-if="!loading && !rows.length">
          <td colspan="9" class="ta-center text-muted py-8">Данные не найдены</td>
        </tr>
        </tbody>
      </v-table>
    </div>

    <AppPager
      v-model:page="page"
      v-model:pageSize="pageSize"
      :total="total"
    />
  </v-container>
</template>

<script setup>
import {ref, computed, watch, onMounted} from 'vue'
import {useRoute, useRouter} from 'vue-router'
import {useSessionStore} from '@/app/store/sessionStore'
import {searchClients, deleteClientSoft, restoreClient} from '@/features/clients/services/clientsService'
import AppIconBtn from '@/shared/components/AppIconBtn.vue'
import AppPager from '@/shared/components/AppPager.vue'
import PermissionTooltip from '@/shared/components/PermissionTooltip.vue'

const route = useRoute()
const router = useRouter()
const auth = useSessionStore()

const canCreate = computed(() => auth.has('CreateClients'))
const canUpdate = computed(() => auth.has('EditClients'))
const canDelete = computed(() => auth.has('DeleteClients'))

const rows = ref([])
const total = ref(0)
const loading = ref(false)

const clientTypeMap = {0: 'Физлицо', 1: 'Юрлицо', 2: 'Агентство'}
const clientTypeName = (t) => clientTypeMap?.[t] ?? '—'

const onlyTourists = ref(false)
const viewRows = computed(() =>
  onlyTourists.value ? (rows.value || []).filter(x => x.isTourist) : (rows.value || [])
)

const page = ref(Number(route.query.page || 1))
const pageSize = ref(Number(route.query.pageSize || 10))
const q = ref(String(route.query.q || ''))
const includeDeleted = ref(route.query.includeDeleted === '1' || route.query.includeDeleted === 'true')

const offset = computed(() => (page.value - 1) * pageSize.value)

function syncQuery() {
  router.replace({
    name: route.name,
    query: {
      ...route.query,
      q: q.value || undefined,
      page: page.value > 1 ? page.value : undefined,
      pageSize: pageSize.value !== 10 ? pageSize.value : undefined,
      includeDeleted: includeDeleted.value ? 1 : undefined,
      onlyTourists: onlyTourists.value ? 1 : undefined,
    }
  })
}

function openDetails(r) {
  router.push({
    name: 'ClientDetails',
    params: {id: r.id},
    query: includeDeleted.value ? {includeDeleted: 1} : {}
  })
}

function applySearch() { page.value = 1; load() }

let currentAbort
async function load() {
  loading.value = true
  if (currentAbort) currentAbort.abort()
  currentAbort = new AbortController()
  const {signal} = currentAbort

  try {
    const {items, total: t} = await searchClients({
      q: q.value?.trim() || undefined,
      page: page.value,
      pageSize: pageSize.value,
      includeDeleted: includeDeleted.value
    }, {signal})

    if (signal.aborted) return
    rows.value = items || []
    total.value = Number(t || 0)
    if (total.value === 0) page.value = 1
  } finally {
    if (!signal.aborted) loading.value = false
  }
}

function openCreate() { if (!canCreate.value) return; router.push({name: 'ClientCreate'}) }
function edit(r) { if (!canUpdate.value) return; router.push({name: 'ClientEdit', params: {id: r.id}}) }

async function removeRow(r) {
  if (!canDelete.value) return
  if (!confirm(`Удалить ${r.isTourist ? 'туриста' : 'клиента'} «${r.firstName} ${r.lastName}»?`)) return
  await deleteClientSoft(r.id)
  await load()
}

async function restoreRow(r) {
  if (!canDelete.value) return
  if (!confirm(`Восстановить клиента «${r.firstName} ${r.lastName}»?`)) return
  await restoreClient(r.id)
  await load()
}

watch([q, page, pageSize, includeDeleted, onlyTourists], () => { syncQuery(); load() })
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

.ml-4 {
  margin-left: 16px;
}

.ml-2 {
  margin-left: 8px;
}

:deep(.v-switch .v-label) {
  color: var(--brand-ink);
  font-weight: 500;
  font-size: 14px;
}

:deep(.v-switch .v-selection-control) {
  min-height: auto;
}

:deep(.v-switch .v-switch__track) {
  background: rgba(139, 146, 109, 0.2);
  opacity: 1;
}

:deep(.v-switch .v-switch__thumb) {
  background: white;
}

:deep(.v-switch--density-comfortable .v-switch__track) {
  height: 20px;
  width: 36px;
  border-radius: 10px;
}

:deep(.v-switch--density-comfortable .v-switch__thumb) {
  height: 16px;
  width: 16px;
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

.row-click {
  cursor: pointer;
  transition: all 0.3s ease;
}

.row-click:hover td {
  background: rgba(139, 146, 109, 0.05);
  transform: translateY(-1px);
}

/* Стили для удаленных клиентов */
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

/* Адаптивность */
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

  :deep(.v-switch) {
    order: 4;
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

  :deep(.v-switch .v-label) {
    font-size: 13px;
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
