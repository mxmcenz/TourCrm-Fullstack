<template>
  <v-container class="page pt-3">
    <div class="toolbar">
      <h1 class="text-h6 text-ink">Добавление / изменение подборки</h1>

      <v-btn class="btn-primary" :disabled="saving" @click="submit">Сохранить</v-btn>
      <v-btn class="ml-2" variant="text" @click="goBack">Отменить</v-btn>
      <div class="grow"></div>
    </div>

    <v-form ref="formRef" v-model="formValid" validate-on="submit lazy">
      <div class="card bg-paper pa-6">
        <v-tabs v-model="tab" class="mb-4">
          <v-tab value="main">Основное</v-tab>
          <v-tab value="partner">Партнер</v-tab>
          <v-tab value="price">Сумма</v-tab>
        </v-tabs>

        <v-window v-model="tab">
          <v-window-item value="main">
            <div class="stack">
              <div class="row-2col">
                <FieldWithBtns label="Город вылета">
                  <v-select
                      v-model="m.departureCity"
                      :items="dicts.cities"
                      item-title="name" item-value="name"
                      density="comfortable" variant="outlined"
                  />
                </FieldWithBtns>

                <FieldWithRightBtn label="Дата(начало)">
                  <v-text-field
                      v-model="m.startDate"
                      type="date"
                      :rules="[rules.date]"
                      density="comfortable" variant="outlined"
                  />
                </FieldWithRightBtn>
              </div>

              <div class="row-2col">
                <FieldWithBtns label="Страна">
                  <v-select
                      v-model="m.country"
                      :items="dicts.countries"
                      item-title="name" item-value="name"
                      density="comfortable" variant="outlined"
                  />
                </FieldWithBtns>

                <FieldWithRightBtn label="Кол-во ночей">
                  <v-text-field
                      v-model.number="m.nights"
                      type="number" min="0"
                      :rules="[rules.intNonNeg]"
                      density="comfortable" variant="outlined"
                  />
                </FieldWithRightBtn>
              </div>

              <div class="row-2col">
                <FieldWithBtns label="Город">
                  <v-select
                      v-model="m.city"
                      :items="dicts.cities"
                      item-title="name" item-value="name"
                      density="comfortable" variant="outlined"
                  />
                </FieldWithBtns>

                <FieldWithRightBtn label="Кол-во взрослых">
                  <v-select
                      v-model.number="m.adults"
                      :items="COUNTS"
                      :rules="[v => (v !== null && v !== undefined) || 'Обязательное поле', v => Number(v) >= 1 || 'Минимум 1']"
                      density="comfortable" variant="outlined"
                  />
                </FieldWithRightBtn>
              </div>

              <div class="row-2col">
                <FieldWithBtns label="Отель">
                  <v-select
                      v-model="m.hotel"
                      :items="dicts.hotels"
                      item-title="name" item-value="name"
                      density="comfortable" variant="outlined"
                  />
                </FieldWithBtns>

                <FieldWithRightBtn label="Кол-во детей">
                  <v-select
                      v-model.number="m.children"
                      :items="COUNTS"
                      :rules="[rules.intNonNeg]"
                      density="comfortable" variant="outlined"
                  />
                </FieldWithRightBtn>
              </div>

              <div class="row-2col">
                <FieldWithBtns label="Тип номера">
                  <v-select
                      v-model="m.roomType"
                      :items="dicts.roomTypes"
                      item-title="name" item-value="name"
                      density="comfortable" variant="outlined"
                  />
                </FieldWithBtns>

                <FieldWithRightBtn label="Кол-во младенцев">
                  <v-select
                      v-model.number="m.infants"
                      :items="COUNTS"
                      :rules="[rules.intNonNeg]"
                      density="comfortable" variant="outlined"
                  />
                </FieldWithRightBtn>
              </div>

              <div class="row-2col">
                <FieldWithBtns label="Тип размещения">
                  <v-select
                      v-model="m.accommodation"
                      :items="dicts.accomTypes"
                      item-title="name" item-value="name"
                      density="comfortable" variant="outlined"
                  />
                </FieldWithBtns>

                <FieldWithRightBtn label="Ссылка">
                  <v-text-field
                      v-model="m.link"
                      :rules="[rules.url]"
                      density="comfortable" variant="outlined"
                  />
                </FieldWithRightBtn>
              </div>

              <div class="row-2col">
                <FieldWithBtns label="Тип питания">
                  <v-select
                      v-model="m.mealPlan"
                      :items="dicts.mealPlans"
                      item-title="name" item-value="name"
                      density="comfortable" variant="outlined"
                  />
                </FieldWithBtns>

                <FieldWithRightBtn label="Примечание">
                  <v-textarea
                      v-model="m.note"
                      :rules="[rules.textMax1000]"
                      rows="3" density="comfortable" variant="outlined"
                  />
                </FieldWithRightBtn>
              </div>
            </div>
          </v-window-item>

          <v-window-item value="partner">
            <div class="grid-12">
              <div class="col-12">
                <FieldWithRightBtn label="Партнер *" :showBtn="true">
                  <v-select
                      v-model="m.partnerId"
                      :items="dicts.partners"
                      item-title="name" item-value="id"
                      density="comfortable" variant="outlined"
                  />
                </FieldWithRightBtn>
              </div>
            </div>
          </v-window-item>

          <v-window-item value="price">
            <div class="grid-12">
              <div class="col-8">
                <FieldWithRightBtn label="Стоимость *">
                  <v-text-field
                      v-model.number="m.price"
                      type="number" min="0"
                      :rules="[rules.money]"
                      density="comfortable" variant="outlined"
                  />
                </FieldWithRightBtn>
              </div>
              <div class="col-4">
                <FieldWithRightBtn label="Валюта *">
                  <v-select
                      v-model="m.currency"
                      :items="dicts.currencies"
                      density="comfortable" variant="outlined"
                  />
                </FieldWithRightBtn>
              </div>
            </div>
          </v-window-item>
        </v-window>
      </div>
    </v-form>
  </v-container>
</template>

<script setup>
import {reactive, ref, onMounted, watch, h, resolveComponent} from 'vue'
import {useRoute, useRouter} from 'vue-router'
import {
  createLeadSelection, getLeadSelection, fetchSelectionDicts,
  updateLeadSelection, getLastLeadSelection
} from '@/features/leads/services/leadSelectionsService'
import {getLead} from '@/features/leads/services/leadsService'

const route = useRoute()
const router = useRouter()
const leadId = ref(Number(route.params.leadId))
const selId = ref(route.params.id ? Number(route.params.id) : null)
const tab = ref('main')
const formRef = ref(null)
const formValid = ref(false)
const saving = ref(false)

const dicts = reactive({
  cities: [], countries: [], hotels: [],
  roomTypes: [], accomTypes: [], mealPlans: [],
  partners: [], currencies: []
})
const m = reactive({
  departureCity: '', country: '', city: '',
  hotel: '', roomType: '', accommodation: '',
  mealPlan: '', startDate: null, nights: null,
  adults: null, children: null, infants: null,
  link: '', note: '', partnerId: null,
  price: null, currency: 'RUB'
})
const COUNTS = Array.from({length: 8}, (_, i) => i)

function toDateInput(v) {
  return v ? String(v).slice(0, 10) : null
}

const rules = {
  required: v => (v !== null && v !== undefined && v !== '') || 'Обязательное поле',
  intNonNeg: v => (v === null || v === '' || (Number.isInteger(Number(v)) && Number(v) >= 0)) || 'Только целое ≥ 0',
  date: v => (!v || !Number.isNaN(new Date(v).getTime())) || 'Неверная дата',
  money: v => (v === null || v === '' || (!Number.isNaN(Number(v)) && Number(v) >= 0)) || 'Укажите число ≥ 0',
  url: v => (!v || /^(https?:\/\/|www\.)[^\s]+$/i.test(String(v))) || 'Неверная ссылка',
  textMax1000: v => (!v || String(v).length <= 1000) || 'Максимум 1000 символов'
}

const dictsLoaded = ref(false)

async function loadDictsOnce() {
  if (dictsLoaded.value) return
  Object.assign(dicts, await fetchSelectionDicts())
  dictsLoaded.value = true
}

function resetForm() {
  Object.assign(m, {
    departureCity: '', country: '', city: '',
    hotel: '', roomType: '', accommodation: '',
    mealPlan: '', startDate: null, nights: null,
    adults: null, children: null, infants: null,
    link: '', note: '', partnerId: null,
    price: null, currency: 'RUB'
  })
}

function diffNights(a, b) {
  try {
    const d1 = new Date(a), d2 = new Date(b)
    const ms = d2.setHours(12, 0, 0, 0) - d1.setHours(12, 0, 0, 0)
    return Math.max(0, Math.round(ms / 86400000))
  } catch {
    return null
  }
}

async function loadSelection() {
  if (!selId.value) {
    resetForm();
    return
  }
  const s = await getLeadSelection(leadId.value, selId.value)
  m.departureCity = s.departureCity || ''
  m.country = s.country || ''
  m.city = s.city || ''
  m.hotel = s.hotel || ''
  m.roomType = s.roomType || ''
  m.accommodation = s.accommodation || ''
  m.mealPlan = s.mealPlan || ''
  m.startDate = toDateInput(s.startDate)
  m.nights = s.nights ?? null
  m.adults = s.adults ?? null
  m.children = s.children ?? null
  m.infants = s.infants ?? null
  m.link = s.link || ''
  m.note = s.note || ''
  m.partnerId = s.partnerId ?? null
  m.price = s.price ?? null
  m.currency = s.currency || 'RUB'
}

async function ensureEditIfExists() {
  if (route.name !== 'LeadSelectionCreate') return false
  const last = await getLastLeadSelection(leadId.value).catch(() => null)
  if (last?.id ?? last?.Id) {
    const id = Number(last.id ?? last.Id)
    await router.replace({name: 'LeadSelectionEdit', params: {leadId: leadId.value, id}})
    selId.value = id
    return true
  }
  return false
}

async function prefillFromLead() {
  const dto = await getLead(leadId.value).catch(() => null)
  if (!dto) return
  m.country = dto.country || ''
  m.adults = (dto.adults ?? 2)
  m.children = (dto.children ?? 0)
  m.infants = (dto.infants ?? 0)
  if (!m.accommodation && dto.accommodation) m.accommodation = dto.accommodation
  if (!m.mealPlan && dto.mealPlan) m.mealPlan = dto.mealPlan

  const arr = dto.desiredArrival
  const dep = dto.desiredDeparture
  if (!m.startDate && arr) m.startDate = String(arr).slice(0, 10)

  const byDates = (arr && dep) ? diffNights(arr, dep) : null
  m.nights = byDates ?? (dto.nightsFrom ?? null)
}

onMounted(async () => {
  await loadDictsOnce()
  const redirected = await ensureEditIfExists()
  if (redirected) {
    await loadSelection()
  } else {
    resetForm()
    await prefillFromLead()
    if (m.adults == null) m.adults = 2
    if (m.children == null) m.children = 0
    if (m.infants == null) m.infants = 0
  }
})

watch(() => route.params.leadId, v => {
  leadId.value = Number(v)
})
watch(() => route.params.id, async v => {
  selId.value = v ? Number(v) : null
  if (selId.value) {
    await loadSelection()
  } else if (route.name === 'LeadSelectionCreate') {
    resetForm()
    await prefillFromLead()
    if (m.adults == null) m.adults = 2
    if (m.children == null) m.children = 0
    if (m.infants == null) m.infants = 0
  }
})

async function submit() {
  const ok = await formRef.value?.validate()
  if (!ok?.valid) return
  if (saving.value) return
  saving.value = true
  try {
    const payload = {
      departureCity: m.departureCity || null,
      country: m.country || null,
      city: m.city || null,
      hotel: m.hotel || null,
      roomType: m.roomType || null,
      accommodation: m.accommodation || null,
      mealPlan: m.mealPlan || null,
      startDate: m.startDate || null,
      nights: m.nights != null ? Number(m.nights) : null,
      adults: Number(m.adults ?? 2),
      children: Number(m.children ?? 0),
      infants: Number(m.infants ?? 0),
      link: m.link || null,
      note: m.note || null,
      partnerId: m.partnerId != null ? Number(m.partnerId) : null,
      price: m.price != null ? Number(m.price) : null,
      currency: m.currency || 'RUB'
    }

    if (selId.value) {
      await updateLeadSelection(leadId.value, selId.value, payload)
    } else {
      await createLeadSelection(leadId.value, payload)
    }
    await router.push({name: 'LeadEdit', params: {id: String(leadId.value)}})
  } catch (e) {
    console.error('Failed to save selection', e)
  } finally {
    saving.value = false
  }
}

function goBack() {
  router.push({name: 'LeadEdit', params: {id: leadId.value}})
}

const FieldWithBtns = (props, {slots}) => {
  const VBtn = resolveComponent('v-btn')
  return h('div', {class: 'field-wrap'}, [
    h('div', {class: 'label'}, props.label),
    h('div', {class: 'row'}, [
      h('div', {class: 'field'}, slots.default?.()),
      h('div', {class: 'icons'}, [
        h(VBtn, {class: 'icon-btn', icon: 'mdi-plus', size: 'small', variant: 'text'}),
        h(VBtn, {class: 'icon-btn', icon: 'mdi-pencil', size: 'small', variant: 'text'})
      ])
    ])
  ])
}
FieldWithBtns.props = {label: String}

const FieldWithRightBtn = (props, {slots}) => {
  const VBtn = resolveComponent('v-btn')
  return h('div', {class: 'field-wrap'}, [
    h('div', {class: 'label'}, props.label),
    h('div', {class: 'row'}, [
      h('div', {class: 'field'}, slots.default?.()),
      props.showBtn
          ? h('div', {class: 'icons'}, [h(VBtn, {class: 'icon-btn', icon: 'mdi-plus', size: 'small', variant: 'text'})])
          : h('div', {class: 'icons empty'})
    ])
  ])
}
FieldWithRightBtn.props = {label: String, showBtn: {type: Boolean, default: false}}
</script>

<style scoped>
.page {
  width: 100%;
  padding-inline: 16px;
  box-sizing: border-box;
  margin-top: 60px;
}

.toolbar {
  display: flex;
  align-items: center;
  gap: 12px;
  background: var(--color-baby-powder);
  padding: 0 0 16px;
}

.grow {
  flex: 1
}

.btn-primary {
  background: var(--color-pear) !important;
  color: #1a1a1a !important;
  font-weight: 600;
}

.card {
  border: 1px solid rgba(0, 0, 0, .12);
  border-radius: 12px
}

.stack {
  display: flex;
  flex-direction: column;
  gap: 14px
}

.row-2col {
  display: grid;
  grid-template-columns:3fr 2fr;
  gap: 12px 24px;
  align-items: center;
  grid-auto-rows: minmax(72px, auto);
}

:global(.field-wrap .label) {
  font-size: .9rem;
  margin-bottom: 6px;
}

:global(.v-input__details) {
  display: none !important;
  height: 0 !important;
  padding: 0 !important;
}

:global(.field-wrap .row) {
  display: grid;
  grid-template-columns:1fr auto;
  align-items: center;
}

:global(.field-wrap .field) {
  min-width: 0;
}

:global(.field-wrap .field .v-input) {
  width: 100%;
}

:global(.v-input) {
  --v-input-control-height: 44px;
}

:global(.field-wrap .field .v-field) {
  border-radius: 10px;
}

:global(.field-wrap .icons) {
  display: flex;
  align-items: center;
  justify-content: center;
  justify-self: end;
  height: var(--v-input-control-height, 56px);
  gap: 6px;
  white-space: nowrap;
  transform: translateY(1px);
}

:global(.field-wrap .icons.empty) {
  width: 0;
  height: var(--v-input-control-height, 56px);
  padding: 0;
  margin: 0;
}

:global(.field-wrap .icons .icon-btn) {
  width: 36px;
  height: 36px;
  min-width: 36px;
  padding: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  line-height: 1;
}

.grid-12 {
  display: grid;
  grid-template-columns:repeat(12, minmax(0, 1fr));
  gap: 16px 24px
}

.col-12 {
  grid-column: span 12
}

.col-8 {
  grid-column: span 8
}

.col-4 {
  grid-column: span 4
}

@media (max-width: 1100px) {
  .row-2col {
    grid-template-columns:1fr;
    gap: 12px
  }

  .col-12, .col-8, .col-4 {
    grid-column: 1/-1
  }
}
</style>