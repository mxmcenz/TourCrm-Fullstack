<template>
  <v-container class="page pt-3">
    <v-snackbar v-model="errorBar.visible" color="error" timeout="6000">
      {{ errorBar.text }}
      <template #actions>
        <v-btn variant="text" @click="errorBar.visible = false">OK</v-btn>
      </template>
    </v-snackbar>

    <div class="toolbar">
      <h1 class="text-h6 text-ink">
        {{ isEdit ? 'Редактирование лида' : 'Создание лида' }}
      </h1>

      <v-btn class="btn-secondary" :disabled="saving" @click="submit">
        {{ isEdit ? 'Сохранить' : 'Создать' }}
      </v-btn>
      <v-btn class="ml-2" variant="text" @click="goBack">Отменить</v-btn>

      <div class="grow"></div>
    </div>

    <v-form ref="formRef" v-model="formValid" class="mt-4" validate-on="submit lazy">
      <div class="card bg-paper pa-6">

        <div class="grid-12">
          <div class="col-6">
            <v-select
                v-model="model.requestTypeId"
                :items="dicts.requestTypes"
                item-title="name"
                item-value="id"
                label="Тип заявки"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
            />
          </div>
          <div class="col-6">
            <v-text-field
                v-model="fio"
                :rules="[rules.required, rules.fio]"
                label="ФИО"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
                @blur="splitFio"
            />
          </div>
        </div>

        <div class="grid-12">
          <div class="col-6">
            <v-select
                v-model="model.managerId"
                :items="managers"
                item-title="fullName"
                item-value="id"
                label="Менеджер"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
            />
          </div>
          <div class="col-3">
            <v-select
                v-model="model.customerType"
                :items="CUSTOMER_TYPES"
                label="Тип"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
            />
          </div>
          <div class="col-3">
            <v-text-field
                v-model="model.email"
                :rules="[rules.email]"
                label="Email"
                type="email"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
            />
          </div>
        </div>

        <div class="grid-12">
          <div class="col-6">
            <v-select
                v-model="model.sourceId"
                :items="dicts.sources"
                item-title="name"
                item-value="id"
                label="Источник заявки"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
            />
          </div>
          <div class="col-3">
            <v-select
                v-model="model.labelIds"
                :items="dicts.labels"
                item-title="name"
                item-value="id"
                label="Метки"
                multiple
                chips
                closable-chips
                density="comfortable"
                variant="outlined"
                hide-details="auto"
            />
          </div>
          <div class="col-3">
            <v-text-field
                v-model="model.phone"
                :rules="[rules.required, rules.phone]"
                label="Телефон"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
            />
          </div>
        </div>

        <h3 class="section-title mt-6">Пожелания клиента</h3>

        <div class="grid-12">
          <div class="col-6">
            <v-select
                v-model="model.country"
                :items="countryItems"
                item-title="name"
                item-value="code"
                label="Страны"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
            />
          </div>
          <div class="col-2">
            <v-text-field
                v-model.number="model.adults"
                type="number"
                min="1"
                :rules="[rules.intNonNeg]"
                label="Взрослые"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
            />
          </div>
          <div class="col-2">
            <v-text-field
                v-model.number="model.children"
                type="number"
                min="0"
                :rules="[rules.intNonNeg]"
                label="Дети"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
            />
          </div>
          <div class="col-2">
            <v-text-field
                v-model.number="model.infants"
                type="number"
                min="0"
                :rules="[rules.intNonNeg]"
                label="Младенцы"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
            />
          </div>
        </div>

        <div class="grid-12">
          <div class="col-6">
            <div class="grid-12 inner-2col">
              <div class="col-6">
                <v-text-field
                    v-model="model.desiredArrival"
                    type="date"
                    :rules="arrivalRules"
                    label="Желаемые даты — Приезд"
                    density="comfortable"
                    variant="outlined"
                    hide-details="auto"
                />
              </div>
              <div class="col-6">
                <v-text-field
                    v-model="model.desiredDeparture"
                    type="date"
                    :rules="departureRules"
                    label="Отлет"
                    density="comfortable"
                    variant="outlined"
                    hide-details="auto"
                />
              </div>
            </div>
          </div>

          <div class="col-3">
            <div class="grid-12 inner-2col">
              <div class="col-6">
                <v-text-field
                    v-model.number="model.nightsFrom"
                    type="number"
                    min="0"
                    :rules="nightsFromRules"
                    label="Кол-во ночей (от)"
                    density="comfortable"
                    variant="outlined"
                    hide-details="auto"
                />
              </div>
              <div class="col-6">
                <v-text-field
                    v-model.number="model.nightsTo"
                    type="number"
                    min="0"
                    :rules="nightsToRules"
                    label="до"
                    density="comfortable"
                    variant="outlined"
                    hide-details="auto"
                />
              </div>
            </div>
          </div>

          <div class="col-3">
            <v-text-field
                v-model.number="model.budget"
                type="number"
                min="0"
                :rules="[rules.money]"
                label="Цена / Бюджет"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
            />
          </div>
        </div>

        <div class="grid-12">
          <div class="col-3">
            <v-select
                v-model="model.accommodation"
                :items="accommodationItems"
                item-title="name"
                item-value="code"
                label="Размещение"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
            />
          </div>
          <div class="col-3">
            <v-select
                v-model="model.mealPlan"
                :items="mealPlanItems"
                item-title="name"
                item-value="code"
                label="Питание"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
            />
          </div>
          <div class="col-6">
            <v-textarea
                v-model="model.note"
                :rules="[rules.textMax1000]"
                label="Примечание"
                auto-grow
                rows="2"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
            />
          </div>
        </div>

        <div class="col-3">
          <v-select
              v-model="model.leadStatusId"
              :items="dicts.statuses"
              item-title="name"
              item-value="id"
              label="Статус *"
              density="comfortable"
              variant="outlined"
              hide-details="auto"
          />
        </div>

        <h3 class="section-title mt-6">Работа с клиентом</h3>
        <div class="grid-12">
          <div class="col-4 box clickable" @click="openManualSelection">
            <div class="box-title">Добавить вручную</div>
            <div class="box-sub">Создание подборки самостоятельно без интеграций</div>
          </div>
          <div class="col-4 box">
            <div class="box-title">Подборка через Oui-Quo</div>
            <div class="box-sub">Как настроить интеграцию?</div>
          </div>
          <div class="col-4 box">
            <div class="box-title">Найти в поиске</div>
            <div class="box-sub">Быстрый поиск тура и добавление в CRM</div>
          </div>
          <div class="col-4 box success clickable" @click="commitDeal">
            <div class="box-title">Совершить сделку</div>
            <div class="box-sub">Сохранить лид и перейти на создание сделки</div>
          </div>
        </div>

        <h3 class="section-title mt-6">Пакет документов</h3>
        <div class="grid-12">
          <div class="col-4">
            <v-text-field
                v-model="model.docsPackageDate"
                type="date"
                :rules="[rules.date]"
                label="Дата для генерации документов"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
            />
          </div>
        </div>

        <v-checkbox
            v-model="model.precontractRequired"
            label="Предварительный договор на бронирование и подбор туристского продукта"
            hide-details
        />
        <v-checkbox
            v-model="model.invoiceRequired"
            label="Счёт на оплату за консультацию"
            hide-details
        />
      </div>
    </v-form>
  </v-container>
</template>

<script setup>
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getLead, createLead, updateLead, fetchLeadDicts, fetchManagers } from '../services/leadsService'
import { fetchSelectionDicts } from '@/features/leads/services/leadSelectionsService'

const route = useRoute()
const router = useRouter()
const isEdit = computed(() => !!route.params.id)

const formRef = ref(null)
const formValid = ref(false)
const saving = ref(false)
const savingDeal = ref(false)
const errorBar = reactive({ visible: false, text: '' })

function showError(msg) {
  errorBar.text = msg || 'Произошла ошибка.'
  errorBar.visible = true
}

function buildPayload () {
  return {
    ...model,
    country:          toCodeMax8(model.country) || null,
    accommodation:    toCodeMax8(model.accommodation) || null,
    mealPlan:         toCodeMax8(model.mealPlan) || null,
    desiredArrival:   model.desiredArrival || null,
    desiredDeparture: model.desiredDeparture || null,
    nightsFrom:       model.nightsFrom ?? null,
    nightsTo:         model.nightsTo ?? null,
    adults:           model.adults ?? null,
    children:         model.children ?? null,
    infants:          model.infants ?? null,
    budget:           model.budget ?? null,
    note:             model.note || null,
    docsPackageDate:  model.docsPackageDate || null
  }
}

async function saveLeadAndGetId () {
  splitFio()
  const ok = await formRef.value?.validate()
  if (!ok?.valid) return null

  const payload = buildPayload()
  try {
    if (isEdit.value) {
      await updateLead(Number(route.params.id), payload)
      return Number(route.params.id)
    } else {
      const created = await createLead(payload)
      return created?.id ?? created?.leadId ?? null
    }
  } catch (e) {
    const msg = extractApiError(e)
    showError(msg)
    throw e
  }
}

async function commitDeal () {
  if (savingDeal.value) return
  savingDeal.value = true
  try {
    const leadId = await saveLeadAndGetId()
    if (!leadId) return
    await router.push({name: 'DealCreate', query: {leadId}})
  } catch (e) {
    const msg = extractApiError(e)
    showError(msg)
  } finally {
    savingDeal.value = false
  }
}

const dicts = reactive({
  statuses: [],
  sources: [],
  requestTypes: [],
  labels: [],
  countries: [],
  accomTypes: [],
  mealPlans: []
})
const managers = ref([])

const model = reactive({
  leadStatusId: null,
  requestTypeId: null,
  sourceId: null,
  managerId: null,
  managerFullName: '',
  customerType: 'person',
  customerFirstName: '',
  customerLastName: '',
  customerMiddleName: '',
  phone: '',
  email: '',
  labelIds: [],
  country: '',
  adults: null, children: null, infants: null,
  desiredArrival: null, desiredDeparture: null,
  nightsFrom: null, nightsTo: null, budget: null,
  accommodation: null,
  mealPlan: null,
  note: '',
  docsPackageDate: null, precontractRequired: false, invoiceRequired: false
})

const fio = ref('')

function splitFio () {
  const s = (fio.value || '').trim().replace(/\s+/g, ' ')
  if (!s) {
    model.customerFirstName = ''
    model.customerLastName = ''
    model.customerMiddleName = ''
    return
  }
  const parts = s.split(' ')
  model.customerFirstName  = parts[0] ?? ''
  model.customerLastName   = parts[1] ?? ''
  model.customerMiddleName = parts.slice(2).join(' ') || ''
}

const rules = {
  required: v => (v !== null && v !== undefined && v !== '') || 'Обязательное поле',
  fio: v => (String(v).trim().split(/\s+/).length >= 2) || 'Укажите минимум Имя и Фамилию',
  email: v => (!v || /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(String(v))) || 'Неверный email',
  phone: v => (String(v || '').replace(/\D/g, '').length >= 8) || 'Телефон слишком короткий',
  intNonNeg: v => (v === null || v === '' || (Number.isInteger(Number(v)) && Number(v) >= 0)) || 'Только целое ≥ 0',
  money: v => (v === null || v === '' || (!Number.isNaN(Number(v)) && Number(v) >= 0)) || 'Укажите число ≥ 0',
  textMax1000: v => (!v || String(v).length <= 1000) || 'Максимум 1000 символов',
  date: v => (!v || !Number.isNaN(new Date(v).getTime())) || 'Неверная дата'
}

function toCodeMax8 (s) {
  if (!s) return null
  return String(s).slice(0, 8)
}
function deriveMealCode (name) {
  if (!name) return null
  const m = String(name).toUpperCase().match(/^[A-Z+]+/)
  return toCodeMax8(m ? m[0] : name)
}
function deriveAccomCode (name) {
  if (!name) return null
  const code = String(name).toUpperCase()
      .replace(/\s+/g, '')
      .replace(/[^A-Z0-9*+]/g, '')
  return toCodeMax8(code || name)
}

function extractApiError(err) {
  const r = err?.response
  const data = r?.data
  if (!r) return err?.message || 'Сетевая ошибка. Проверьте соединение.'
  if (r.status === 409) {
    return data?.detail || 'Лимит офиса исчерпан'
  }
  if (!data) return `Ошибка ${r.status}`
  if (typeof data === 'string') return data
  if (data.errors && typeof data.errors === 'object') {
    const firstKey = Object.keys(data.errors)[0]
    const arr = data.errors[firstKey]
    if (Array.isArray(arr) && arr.length) return arr[0]
  }
  if (data.detail) return data.detail
  if (data.title)  return data.title
  return `Ошибка ${r.status}`
}

const countryItems = computed(() =>
    (dicts.countries ?? []).map(c => {
      if (typeof c === 'string') return { name: c, code: toCodeMax8(c) }
      return { name: c.name ?? String(c), code: toCodeMax8(c.code ?? c.name ?? '') }
    })
)
const accommodationItems = computed(() =>
    (dicts.accomTypes ?? []).map(a => {
      const name = typeof a === 'string' ? a : (a.name ?? '')
      const code = (typeof a === 'object' && a.code) ? a.code : deriveAccomCode(name)
      return { name, code: toCodeMax8(code) }
    })
)
const mealPlanItems = computed(() =>
    (dicts.mealPlans ?? []).map(m => {
      const name = typeof m === 'string' ? m : (m.name ?? '')
      const code = (typeof m === 'object' && m.code) ? m.code : deriveMealCode(name)
      return { name, code: toCodeMax8(code) }
    })
)

function calcNights (a, b) {
  try {
    const d1 = new Date(a), d2 = new Date(b)
    const ms = d2.setHours(12,0,0,0) - d1.setHours(12,0,0,0)
    return Math.max(0, Math.round(ms / 86400000))
  } catch { return null }
}

const arrivalRules = computed(() => [
  rules.date,
  v => (!v || !model.desiredDeparture || new Date(v) <= new Date(model.desiredDeparture)) || 'Приезд не позже отлёта'
])
const departureRules = computed(() => [
  rules.date,
  v => (!v || !model.desiredArrival || new Date(v) >= new Date(model.desiredArrival)) || 'Отлёт не раньше приезда'
])
const nightsFromRules = computed(() => [
  rules.intNonNeg,
  v => (model.nightsTo == null || v == null || Number(v) <= Number(model.nightsTo)) || '«От» не больше «до»',
  v => {
    const n = calcNights(model.desiredArrival, model.desiredDeparture)
    return (n == null || v == null || Number(v) <= n) || `Не больше ${n}`
  }
])
const nightsToRules = computed(() => [
  rules.intNonNeg,
  v => (model.nightsFrom == null || v == null || Number(v) >= Number(model.nightsFrom)) || '«До» не меньше «от»',
  v => {
    const n = calcNights(model.desiredArrival, model.desiredDeparture)
    return (n == null || v == null || Number(v) <= n) || `Не больше ${n}`
  }
])

watch(() => [model.desiredArrival, model.desiredDeparture], ([arr, dep]) => {
  if (arr && dep) {
    const n = calcNights(arr, dep)
    if (n != null) {
      if (model.nightsFrom == null) model.nightsFrom = n
      if (model.nightsTo   == null) model.nightsTo   = n
    }
  }
})

async function loadDicts () {
  const d = await fetchLeadDicts()

  dicts.statuses     = (d.statuses ?? []).map(x => ({ id: Number(x.id), name: x.name }))
  dicts.sources      = (d.sources ?? []).map(x => ({ id: Number(x.id), name: x.name }))
  dicts.requestTypes = (d.requestTypes ?? []).map(x => ({ id: Number(x.id), name: x.name }))
  dicts.labels       = (d.labels ?? []).map(x => ({ id: Number(x.id), name: x.name }))
  dicts.countries    = d.countries ?? []

  const sdicts = await fetchSelectionDicts().catch(() => ({}))
  dicts.accomTypes = sdicts?.accomTypes ?? []
  dicts.mealPlans  = sdicts?.mealPlans  ?? []

  managers.value = await fetchManagers()

  if (!isEdit.value && dicts.statuses.length) {
    const def = dicts.statuses.find(s => /новый/iu.test(s.name)) || dicts.statuses[0]
    model.leadStatusId = def.id
  }
}

async function loadIfEdit () {
  if (!isEdit.value) return
  const dto = await getLead(Number(route.params.id))
  Object.assign(model, {
    leadStatusId: dto.leadStatusId, requestTypeId: dto.requestTypeId, sourceId: dto.sourceId,
    managerId: dto.managerId, managerFullName: dto.managerFullName,
    customerType: dto.customerType, customerFirstName: dto.customerFirstName, customerLastName: dto.customerLastName,
    phone: dto.phone, email: dto.email, labelIds: dto.labelIds || [],
    country: dto.country || '', // ожидаем КОД
    adults: dto.adults, children: dto.children, infants: dto.infants,
    desiredArrival: dto.desiredArrival?.substring(0, 10) || null,
    desiredDeparture: dto.desiredDeparture?.substring(0, 10) || null,
    nightsFrom: dto.nightsFrom, nightsTo: dto.nightsTo, budget: dto.budget,
    accommodation: dto.accommodation, mealPlan: dto.mealPlan, note: dto.note,
    docsPackageDate: dto.docsPackageDate?.substring(0, 10) || null,
    precontractRequired: dto.precontractRequired, invoiceRequired: dto.invoiceRequired
  })
  fio.value = [dto.customerFirstName, dto.customerLastName, dto.customerMiddleName].filter(Boolean).join(' ')
}

function openManualSelection () {
  if (!isEdit.value) return
  router.push({ name: 'LeadSelectionCreate', params: { leadId: Number(route.params.id) } })
}

async function submit () {
  splitFio()
  const ok = await formRef.value?.validate()
  if (!ok?.valid) return
  saving.value = true
  try {
    const payload = buildPayload()
    if (isEdit.value) {
      await updateLead(Number(route.params.id), payload)
    } else {
      await createLead(payload)
    }
    goBack()
  } catch (e) {
    const msg = extractApiError(e)
    showError(msg)
  } finally {
    saving.value = false
  }
}

function goBack () {
  router.push({ name: 'LeadsList' })
}

onMounted(async () => {
  await loadDicts()
  await loadIfEdit()
})

const CUSTOMER_TYPES = [
  { title: 'Частное лицо', value: 'person' },
  { title: 'Юр. лицо',     value: 'legal'  },
  { title: 'Турагентство', value: 'agency' }
]
</script>

<style scoped>
.page{ width:100%; padding-inline:16px; box-sizing:border-box; margin-top: 60px }
.toolbar{ display:flex; align-items:center; gap:12px; background: var(--color-baby-powder); padding:0 0 16px 0; }
.grow{ flex:1; }

.btn-secondary {
  background: var(--color-pear) !important;
  color: #1a1a1a !important;
  font-weight: 600;
}

.card { border:1px solid rgba(0,0,0,.12); border-radius:12px; }

.section-title { font-weight:600; margin:26px 0 12px; }

.grid-12 {
  display:grid;
  grid-template-columns:repeat(12, minmax(0, 1fr));
  column-gap:24px;
  row-gap:16px;
  margin-bottom:18px;
}
.grid-12:last-child { margin-bottom:0; }

.col-2 { grid-column: span 2; }
.col-3 { grid-column: span 3; }
.col-4 { grid-column: span 4; }
.col-6 { grid-column: span 6; }
.col-12 { grid-column: span 12; }

.inner-2col { column-gap:16px; }

.box { border:1px solid rgba(0,0,0,.15); border-radius:12px; padding:14px; background:#fff; }
.box-title { font-weight:600; margin-bottom:6px; }
.box-sub { color:#7a7a7a; font-size:.92rem; }

:deep(.v-field) { border-radius:10px; }
:deep(.v-input--density-compact) { --v-input-control-height:44px; }

.box.clickable { cursor:pointer; transition: background .15s ease, box-shadow .15s ease; }
.box.clickable:hover { background:#fafafa; box-shadow:0 1px 0 rgba(0,0,0,.06) inset; }
.box.success {
  border-color: var(--color-pear);
  background: #fbfff2;
}
@media (max-width: 1100px) {
  .col-2, .col-3, .col-4, .col-6, .col-12 { grid-column: 1 / -1; }
}
</style>