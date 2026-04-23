<template>
  <v-container class="page pt-3">
    <div class="toolbar">
      <div class="grow"></div>
    </div>

    <div v-if="!isReady" class="bg-paper pa-6" style="border:1px solid rgba(0,0,0,.12); border-radius:12px;">
      <v-skeleton-loader type="heading, text, text"/>
    </div>

    <template v-else>
      <div class="company-header">
        <div class="name-row">
          <template v-if="!editingName">
            <h2 class="company-name">{{ company?.name }}</h2>
            <PermissionTooltip :can="canEdit">
              <v-btn
                class="edit-chip"
                icon
                variant="outlined"
                size="28"
                :ripple="false"
                v-can.disable="'EditLegalEntities'"
                :disabled="!canEdit"
                @click="startEdit"
                title="Переименовать"
              >
                <v-icon size="16">mdi-pencil</v-icon>
              </v-btn>
            </PermissionTooltip>
          </template>

          <template v-else>
            <v-text-field
              v-model="nameDraft"
              class="name-input"
              variant="outlined"
              density="comfortable"
              hide-details
              placeholder="Название компании"
              autofocus
              @keyup.enter="saveName"
            />
            <PermissionTooltip :can="canEdit">
              <v-btn
                icon
                :loading="savingName"
                :disabled="!canSaveName || !canEdit"
                v-can.disable="'EditLegalEntities'"
                @click="saveName"
                title="Сохранить"
              >
                <v-icon>mdi-check</v-icon>
              </v-btn>
            </PermissionTooltip>
            <v-btn icon variant="text" @click="cancelEdit" title="Отменить">
              <v-icon>mdi-close</v-icon>
            </v-btn>
          </template>
        </div>

        <div class="subrow">
          <p class="section-title">Юридические лица компании</p>

          <PermissionTooltip :can="canCreate">
            <AppAddButton
              label="+ Добавить"
              @click="openCreate"
              v-can.disable="'CreateLegalEntities'"
              :disabled="!canCreate"
            />
          </PermissionTooltip>

          <div class="grow"></div>

          <v-text-field
            v-model="query"
            class="search-input"
            placeholder="Поиск: название, страна/город, телефон, email, кол-во сотрудников"
            hide-details
            density="comfortable"
            variant="outlined"
            clearable
            :append-inner-icon="'mdi-magnify'"
            @keyup.enter="applySearch"
            @click:append-inner="applySearch"
            style="max-width: 380px"
          />
        </div>
      </div>

      <div v-if="!canView" class="content-wrap bg-paper ta-center py-8"
           style="border:1px solid rgba(0,0,0,.12); border-radius:12px;">
        Нет доступа
      </div>

      <div v-else class="table-wrap bg-paper">
        <v-table density="comfortable">
          <thead>
          <tr>
            <th>#</th>
            <th>Название</th>
            <th>Страна, город</th>
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
                <v-btn
                  variant="text"
                  class="name-btn"
                  :ripple="false"
                  @click="edit(r)"
                  :title="`Редактировать «${r.name}»`"
                  v-can.disable="'EditLegalEntities'"
                  :disabled="!canEdit"
                >
                  {{ r.name }}
                </v-btn>
              </PermissionTooltip>
            </td>
            <td>
              {{ r.countryName || '—' }}
              <template v-if="r.cityName && r.countryName">,</template>
              {{ r.cityName || '' }}
            </td>
            <td>
              <template v-if="Array.isArray(r.phones) && r.phones.length">
                {{ r.phones.join(', ') }}
              </template>
              <template v-else>
                {{ r.phone || '—' }}
              </template>
            </td>
            <td>{{ r.primaryEmail || r.email || '—' }}</td>
            <td>{{ r.employeesCount ?? r.employeeCount ?? '—' }}</td>
            <td class="ta-right" @click.stop>
              <PermissionTooltip :can="canEdit">
                <AppIconBtn
                  icon="mdi-pencil"
                  title="Редактировать"
                  @click.stop="edit(r)"
                  v-can.disable="'EditLegalEntities'"
                  :disabled="!canEdit"
                />
              </PermissionTooltip>
              <PermissionTooltip :can="canDelete">
                <AppIconBtn
                  icon="mdi-trash-can"
                  title="Удалить"
                  @click.stop="removeRow(r)"
                  v-can.disable="'DeleteLegalEntities'"
                  :disabled="!canDelete"
                />
              </PermissionTooltip>
            </td>
          </tr>

          <tr v-if="!loading && !rows.length">
            <td colspan="7" class="ta-center text-muted py-8">Данные не найдены</td>
          </tr>
          </tbody>
        </v-table>
      </div>

      <AppPager v-if="canView" v-model:page="page" v-model:pageSize="pageSize" :total="rows.length"/>
    </template>
  </v-container>
</template>

<script setup>
import {ref, computed, onMounted, watch} from 'vue'
import {useRouter} from 'vue-router'
import {useCompanyStore} from '@/features/company/store/companyStore'
import {useSessionStore} from '@/app/store/sessionStore'
import {fetchLegalEntities, deleteLegalEntity} from '@/features/touragent/services/companyService'
import AppAddButton from '@/shared/components/AppAddButton.vue'
import AppIconBtn from '@/shared/components/AppIconBtn.vue'
import AppPager from '@/shared/components/AppPager.vue'
import PermissionTooltip from '@/shared/components/PermissionTooltip.vue'

const router = useRouter()
const companyStore = useCompanyStore()
const auth = useSessionStore()

const canView = computed(() => auth.has('ViewLegalEntities'))
const canCreate = computed(() => auth.has('CreateLegalEntities'))
const canEdit = computed(() => auth.has('EditLegalEntities'))
const canDelete = computed(() => auth.has('DeleteLegalEntities'))

const company = computed(() => companyStore.company)
const isReady = computed(() => companyStore.isReady)

const editingName = ref(false)
const nameDraft = ref('')
const savingName = ref(false)
const canSaveName = computed(() => (nameDraft.value || '').trim().length > 1)

function startEdit() {
  if (!canEdit.value) return
  nameDraft.value = company.value?.name || ''
  editingName.value = true
}

function cancelEdit() {
  editingName.value = false
}

async function saveName() {
  if (!canEdit.value || !canSaveName.value) return
  savingName.value = true
  try {
    await companyStore.rename(nameDraft.value.trim())
    editingName.value = false
  } finally {
    savingName.value = false
  }
}

const rows = ref([])
const loading = ref(false)
const page = ref(1)
const pageSize = ref(10)
const query = ref('')

const offset = computed(() => (page.value - 1) * pageSize.value)
const paged = computed(() => rows.value.slice(offset.value, offset.value + pageSize.value))

let currentAbort = null

async function loadLegals() {
  if (!canView.value || !company.value) {
    rows.value = [];
    return
  }

  if (currentAbort) currentAbort.abort()
  currentAbort = new AbortController()
  const {signal} = currentAbort

  loading.value = true
  try {
    const data = await fetchLegalEntities(query.value, {signal})
    if (signal.aborted) return

    rows.value = Array.isArray(data) ? data : []
    const pages = Math.max(1, Math.ceil(rows.value.length / pageSize.value))
    if (page.value > pages) page.value = pages
  } catch (e) {
    if (e?.name !== 'CanceledError' && e?.code !== 'ERR_CANCELED' && e?.message !== 'canceled') {
      console.error(e)
    }
  } finally {
    if (!signal.aborted) loading.value = false
  }
}


function applySearch() {
  page.value = 1;
  loadLegals()
}

function openCreate() {
  if (canCreate.value) router.push({name: 'LegalEntityCreate'})
}

function edit(r) {
  if (canEdit.value) router.push({name: 'LegalEntityEdit', params: {id: r.id}})
}

async function removeRow(r) {
  if (!canDelete.value) return
  if (!confirm(`Удалить «${r.name}»?`)) return
  await deleteLegalEntity(r.id)
  await loadLegals()
}

let searchTimer
watch(query, () => {
  clearTimeout(searchTimer)
  searchTimer = setTimeout(() => {
    page.value = 1;
    loadLegals()
  }, 300)
})

onMounted(async () => {
  await companyStore.loadMyCompany(true)
  await loadLegals()
})
watch([canView, () => companyStore.company], () => loadLegals())
watch(pageSize, () => {
  page.value = 1
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

.grow {
  flex: 1;
}

.company-header {
  margin-top: 32px;
  padding: 0;
  position: relative;
}

.name-row {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 24px;
}

.company-name {
  font-size: 28px;
  font-weight: 800;
  background: black;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin: 0;
  letter-spacing: -0.2px;
}

.edit-chip {
  border-radius: 10px !important;
  min-width: 32px !important;
  height: 32px !important;
  padding: 0 !important;
  border: 1.5px solid rgba(139, 146, 109, 0.2) !important;
  background: rgba(255, 255, 255, 0.8) !important;
  transition: all 0.3s ease !important;
}

.edit-chip:hover {
  border-color: var(--brand-primary) !important;
  background: rgba(139, 146, 109, 0.1) !important;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(139, 146, 109, 0.2);
}

.name-input {
  max-width: 420px;
}

.name-input :deep(.v-field) {
  border-radius: 12px;
  background: rgba(255, 255, 255, 0.9);
  border: 1.5px solid rgba(139, 146, 109, 0.2);
  transition: all 0.3s ease;
}

.name-input :deep(.v-field:focus-within) {
  border-color: var(--brand-primary);
  box-shadow: 0 4px 15px rgba(139, 146, 109, 0.2);
}

.subrow {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 20px;
}

.section-title {
  font-size: 20px;
  font-weight: 700;
  color: var(--brand-ink);
  margin: 0;
  display: flex;
  align-items: center;
  gap: 8px;
}

.section-title::before {
  content: '';
  width: 4px;
  height: 20px;
  background: linear-gradient(135deg, var(--color-pear), var(--brand-primary));
  border-radius: 2px;
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

.table-wrap {
  border: 1px solid rgba(139, 146, 109, 0.1);
  border-radius: 20px 20px 0 0;
  overflow: hidden;
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

.name-btn {
  padding: 8px 12px !important;
  min-width: auto !important;
  height: auto !important;
  line-height: 1.2;
  text-transform: none;
  font-weight: 700;
  color: var(--brand-ink) !important;
  border-radius: 10px !important;
  transition: all 0.3s ease !important;
  background: rgba(139, 146, 109, 0.08) !important;
  position: relative;
  overflow: hidden;
}

.name-btn::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.4), transparent);
  transition: left 0.5s ease;
}

.name-btn:hover::before {
  left: 100%;
}

.name-btn:hover {
  background: rgba(139, 146, 109, 0.15) !important;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(139, 146, 109, 0.2);
}

.content-wrap {
  border: 1px solid rgba(139, 146, 109, 0.1);
  border-radius: 20px;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.95) 0%, rgba(242, 243, 237, 0.3) 100%);
  backdrop-filter: blur(10px);
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.06);
}

:deep(.v-skeleton-loader__heading) {
  border-radius: 12px;
  margin-bottom: 16px;
}

:deep(.v-skeleton-loader__text) {
  border-radius: 8px;
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

  .subrow {
    flex-wrap: wrap;
    gap: 12px;
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

  .company-name {
    font-size: 24px;
  }

  .name-row {
    flex-wrap: wrap;
  }

  .table-wrap {
    border-radius: 16px;
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