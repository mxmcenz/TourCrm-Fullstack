<template>
  <v-container class="page pt-3">
    <div class="toolbar">
      <h1 class="text-h6 text-ink">
        {{ isEdit ? 'Редактирование сделки' : 'Создание сделки' }}
      </h1>

      <v-btn class="btn-secondary" :disabled="saving" @click="submit">
        {{ isEdit ? 'Сохранить' : 'Создать' }}
      </v-btn>
      <v-btn class="ml-2" variant="text" @click="goBack">Отменить</v-btn>

      <div class="grow"/>
    </div>

    <v-form ref="formRef" v-model="formValid" class="mt-4" validate-on="submit lazy">
      <div class="card bg-paper">

        <div class="tabs-in-card">
          <button class="tab-btn active" @click.prevent="onTabClick('main')">Основные данные</button>
        </div>

        <div class="card-body pa-6">
          <div class="grid-12">
            <div class="col-6">
              <v-autocomplete
                  v-model="m.companyId"
                  :items="companyOptions"
                  item-title="name"
                  item-value="id"
                  label="Оформляющая компания"
                  density="comfortable"
                  variant="outlined"
                  hide-details="auto"
                  clearable
              />
            </div>
            <div class="col-6">
              <v-select
                  v-model="m.tourOperatorId"
                  :items="dicts.tourOperators"
                  item-title="name" item-value="id"
                  label="Туроператор"
                  density="comfortable" variant="outlined" hide-details="auto"
              />
            </div>
          </div>

          <div class="grid-12">
            <div class="col-6">
              <v-select
                  v-model="m.requestTypeId"
                  :items="dicts.requestTypes"
                  item-title="name" item-value="id"
                  label="Тип заявки"
                  density="comfortable" variant="outlined" hide-details="auto"
              />
            </div>
            <div class="col-6">
              <v-text-field
                  v-model="m.bookingNumbers"
                  label="Номер брони у ТО (несколько через запятую)"
                  density="comfortable" variant="outlined" hide-details="auto"
              />
            </div>
          </div>

          <div class="grid-12">
            <div class="col-6">
              <v-select
                  v-model="m.managerId"
                  :items="dicts.managers"
                  item-title="fullName" item-value="id"
                  :rules="[rules.required]"
                  label="Менеджер"
                  density="comfortable" variant="outlined" hide-details="auto"
              />
            </div>
            <div class="col-6">
              <v-select
                  v-model="m.visaTypeId"
                  :items="dicts.visaTypes"
                  item-title="name" item-value="id"
                  label="Виза"
                  density="comfortable" variant="outlined" hide-details="auto"
              />
            </div>
          </div>

          <div class="grid-12">
            <div class="col-6">
              <v-select
                  v-model="m.sourceId"
                  :items="dicts.sources"
                  item-title="name" item-value="id"
                  label="Источник заявки"
                  density="comfortable" variant="outlined" hide-details="auto"
              />
            </div>
            <div class="col-6">
              <v-text-field
                  v-model="m.note"
                  label="Примечание"
                  density="comfortable" variant="outlined" hide-details="auto"
              />
            </div>
          </div>

          <div class="grid-12">
            <div class="col-6">
              <v-text-field
                  v-model="m.clientPaymentDeadline"
                  type="date"
                  :min="todayStr"
                  :rules="[rules.notPastDate]"
                  label="Dead-line по оплате с клиентом"
                  density="comfortable" variant="outlined" hide-details="auto"
              />
            </div>
            <div class="col-6">
              <v-text-field
                  v-model="m.partnerPaymentDeadline"
                  type="date"
                  :min="todayStr"
                  :rules="[rules.notPastDate]"
                  label="Dead-line по оплате с партнёром"
                  density="comfortable" variant="outlined" hide-details="auto"
              />
            </div>
            <div class="col-12">
              <v-select
                  v-model="m.statusId"
                  :items="dicts.statuses"
                  item-title="name"
                  item-value="id"
                  label="Статус *"
                  :rules="[rules.required]"
                  density="comfortable"
                  variant="outlined"
                  hide-details="auto"
              />
            </div>
          </div>

          <div class="sections mt-2">
            <div class="section">
              <div class="section-head">
                <span>Клиент</span>
                <v-btn icon variant="text" size="small" @click="openPicker('customer')">
                  <v-icon>mdi-plus</v-icon>
                </v-btn>
              </div>

              <div v-if="customers.length" class="section-table">
                <div class="row head">
                  <div>ФИО</div>
                  <div>Контакты</div>
                  <div>Личный кабинет</div>
                  <div class="actions-col"></div>
                </div>
                <div v-for="(c,i) in customers" :key="'c'+i" class="row">
                  <div>{{ c.fullName }}</div>
                  <div>{{ c.phone || '—' }}</div>
                  <div>{{ c.email || '—' }}</div>
                  <div class="actions">
                    <v-btn icon variant="text" size="small" @click="replaceFrom('customer', i)">
                      <v-icon>mdi-pencil</v-icon>
                    </v-btn>
                    <v-btn icon variant="text" size="small" @click="removeFrom('customer', i)">
                      <v-icon>mdi-delete-outline</v-icon>
                    </v-btn>
                  </div>
                </div>
              </div>
            </div>

            <div class="section">
              <div class="section-head">
                <span>Турист</span>
                <v-btn icon variant="text" size="small" @click="openPicker('tourist')">
                  <v-icon>mdi-plus</v-icon>
                </v-btn>
              </div>

              <div v-if="tourists.length" class="section-table">
                <div class="row head">
                  <div>ФИО</div>
                  <div>Контакты</div>
                  <div>Личный кабинет</div>
                  <div class="actions-col"></div>
                </div>
                <div v-for="(t,i) in tourists" :key="'t'+i" class="row">
                  <div>{{ t.fullName }}</div>
                  <div>{{ t.phone || '—' }}</div>
                  <div>{{ t.email || '—' }}</div>
                  <div class="actions">
                    <v-btn icon variant="text" size="small" @click="replaceFrom('tourist', i)">
                      <v-icon>mdi-pencil</v-icon>
                    </v-btn>
                    <v-btn icon variant="text" size="small" @click="removeFrom('tourist', i)">
                      <v-icon>mdi-delete-outline</v-icon>
                    </v-btn>
                  </div>
                </div>
              </div>
            </div>

            <div class="section">
              <div class="section-head">
                <span>Услуги</span>
                <v-btn icon variant="text" size="small" @click="openPicker('service')">
                  <v-icon>mdi-plus</v-icon>
                </v-btn>
              </div>
              <div v-if="services.length" class="section-table">
                <div class="row head">
                  <div>Наименование</div>
                  <div>Примечание</div>
                  <div></div>
                  <div class="actions-col"></div>
                </div>
                <div v-for="(s,i) in services" :key="'s'+i" class="row">
                  <div>{{ s.name }}</div>
                  <div>{{ s.note || '—' }}</div>
                  <div></div>
                  <div class="actions">
                    <v-btn icon variant="text" size="small" @click="openPicker('service', i)">
                      <v-icon>mdi-pencil</v-icon>
                    </v-btn>
                    <v-btn icon variant="text" size="small" @click="removeFrom('service', i)">
                      <v-icon>mdi-delete-outline</v-icon>
                    </v-btn>
                  </div>
                </div>
              </div>
            </div>

            <div class="section">
              <div class="section-head">
                <span>Расчёты с клиентом</span>
                <v-btn icon variant="text" size="small" @click="openPicker('clientPay')">
                  <v-icon>mdi-plus</v-icon>
                </v-btn>
              </div>
              <div v-if="clientPays.length" class="section-table">
                <div class="row head">
                  <div>Описание</div>
                  <div>Сумма</div>
                  <div></div>
                  <div class="actions-col"></div>
                </div>
                <div v-for="(p,i) in clientPays" :key="'cp'+i" class="row">
                  <div>{{ p.title }}</div>
                  <div>{{ p.amount ?? '—' }}</div>
                  <div></div>
                  <div class="actions">
                    <v-btn icon variant="text" size="small" @click="openPicker('clientPay', i)">
                      <v-icon>mdi-pencil</v-icon>
                    </v-btn>
                    <v-btn icon variant="text" size="small" @click="removeFrom('clientPay', i)">
                      <v-icon>mdi-delete-outline</v-icon>
                    </v-btn>
                  </div>
                </div>
              </div>
            </div>

            <div class="section">
              <div class="section-head">
                <span>Расчёты с партнёром</span>
                <v-btn icon variant="text" size="small" @click="openPicker('partnerPay')">
                  <v-icon>mdi-plus</v-icon>
                </v-btn>
              </div>
              <div v-if="partnerPays.length" class="section-table">
                <div class="row head">
                  <div>Описание</div>
                  <div>Сумма</div>
                  <div></div>
                  <div class="actions-col"></div>
                </div>
                <div v-for="(p,i) in partnerPays" :key="'pp'+i" class="row">
                  <div>{{ p.title }}</div>
                  <div>{{ p.amount ?? '—' }}</div>
                  <div></div>
                  <div class="actions">
                    <v-btn icon variant="text" size="small" @click="openPicker('partnerPay', i)">
                      <v-icon>mdi-pencil</v-icon>
                    </v-btn>
                    <v-btn icon variant="text" size="small" @click="removeFrom('partnerPay', i)">
                      <v-icon>mdi-delete-outline</v-icon>
                    </v-btn>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <h3 class="section-title mt-6">Пакет документов</h3>
          <div class="grid-12">
            <div class="col-4">
              <v-text-field
                  v-model="m.docsPackageDate"
                  type="date"
                  :min="todayStr"
                  :rules="[rules.notPastDate]"
                  label="Дата для генерации документов"
                  density="comfortable" variant="outlined" hide-details="auto"
              />
            </div>
            <div class="col-8"/>
          </div>
          <div class="grid-12">
            <div class="col-12">
              <v-checkbox v-model="m.addStampAndSign" label="Добавлять печать и подпись" hide-details/>
              <v-checkbox v-model="m.includeCostCalc" label="Расчёт тура (себестоимость)" hide-details/>
              <v-checkbox v-model="m.includeTourCalc" label="Расчёт тура" hide-details/>
            </div>
          </div>

          <h3 class="section-title mt-6">Файлы</h3>
          <div class="grid-12">
            <div class="col-12">
              <input ref="fileInputRef" type="file" class="hidden-input" multiple @change="onFilesChosen">
              <v-btn class="attach-btn" variant="outlined" @click="triggerFilePick">Прикрепить файл</v-btn>
              <div v-if="files.length" class="files-list mt-3">
                <div v-for="(f,i) in files" :key="i" class="file-item">{{ f.name }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </v-form>

    <v-dialog v-model="search.open" max-width="760">
      <v-card>
        <v-card-title class="px-6 py-4">Поиск существующего</v-card-title>
        <v-divider/>
        <v-card-text class="px-6 py-4">
          <v-text-field
              v-model="search.query"
              :placeholder="searchPlaceholder"
              density="comfortable"
              variant="outlined"
              hide-details
              clearable
              @keydown.enter.stop
          />
          <v-skeleton-loader v-if="search.loading" type="list-item-two-line" class="mt-2"/>
          <v-alert
              v-else-if="search.error"
              type="warning"
              variant="tonal"
              density="compact"
              class="mt-2"
          >
            {{ search.error }}
          </v-alert>

          <div v-if="!search.loading && search.results.length" class="results mt-2">
            <div
                v-for="r in search.results"
                :key="r._key"
                class="res-row"
                :class="{active: search.selected && search.selected._key===r._key}"
            >
              <div class="res-main" @click="search.selected = r">
                <div class="res-title">{{ r.display }}</div>
                <div v-if="r.sub" class="res-sub">{{ r.sub }}</div>
              </div>
              <v-btn size="small" variant="tonal" class="res-choose" @click.stop="choose(r)">
                <v-icon start>mdi-plus</v-icon>
                Выбрать
              </v-btn>
            </div>
          </div>
          <div v-if="!search.loading && !search.results.length && search.query" class="muted mt-2">
            Ничего не найдено
          </div>
        </v-card-text>
        <v-divider/>
        <v-card-actions class="px-6 py-3">
          <v-btn variant="outlined" @click="openCreateFromSearch">Создать нового</v-btn>
          <v-spacer/>
          <v-btn variant="text" @click="search.open=false">Отменить</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog
        v-model="create.open"
        max-width="640"
        v-if="create.type!=='customer' && create.type!=='tourist'"
    >
      <v-card>
        <v-card-title class="px-6 py-4">{{ createTitle }}</v-card-title>
        <v-divider/>
        <v-card-text class="px-6 py-4">
          <v-form ref="createFormRef" v-model="create.valid" validate-on="submit lazy">
            <div class="field-gap">
              <template v-if="create.type==='service'">
                <v-text-field
                    v-model="create.model.name"
                    :rules="[rules.required]"
                    label="Наименование *"
                    density="comfortable" variant="outlined" hide-details="auto"
                />
                <v-text-field
                    v-model="create.model.note"
                    label="Примечание"
                    density="comfortable" variant="outlined" hide-details="auto"
                />
              </template>

              <template v-else>
                <v-text-field
                    v-model="create.model.title"
                    :rules="[rules.required]"
                    label="Описание *"
                    density="comfortable" variant="outlined" hide-details="auto"
                />
                <v-text-field
                    v-model.number="create.model.amount"
                    :rules="[rules.money]"
                    type="number" min="0" step="0.01"
                    label="Сумма"
                    density="comfortable" variant="outlined" hide-details="auto"
                />
              </template>
            </div>
          </v-form>
        </v-card-text>
        <v-divider/>
        <v-card-actions class="px-6 py-3">
          <v-spacer/>
          <v-btn class="btn-secondary" @click="saveCreate">Добавить</v-btn>
          <v-btn variant="text" @click="create.open=false">Отменить</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup>
import {reactive, ref, computed, onMounted, watch} from 'vue'
import {useRoute, useRouter} from 'vue-router'
import {fetchDealDicts, getDeal, createDeal, updateDeal} from '../services/dealsService'
import {useMyCompany} from '@/features/company/store/useMyCompany'
import {getClient} from '@/features/clients/services/clientsService'
import { getLead } from '@/features/leads/services/leadsService'

const {myCompany, ensureLoaded} = useMyCompany()
const route = useRoute()
const router = useRouter()
const isEdit = computed(() => !!route.params.id)

const formRef = ref(null)
const formValid = ref(false)
const saving = ref(false)
const fileInputRef = ref(null)
const files = ref([])

const DRAFT_KEY = 'dealDraft'

function saveDraft() {
  const draft = {
    m: {...m},
    customers: customers.value,
    tourists: tourists.value,
    services: services.value,
    clientPays: clientPays.value,
    partnerPays: partnerPays.value,
  }
  sessionStorage.setItem(DRAFT_KEY, JSON.stringify(draft))
}

function loadDraft() {
  const raw = sessionStorage.getItem(DRAFT_KEY)
  if (!raw) return false
  try {
    const d = JSON.parse(raw)
    if (d?.m) Object.assign(m, d.m)
    customers.value = Array.isArray(d?.customers) ? d.customers : []
    tourists.value = Array.isArray(d?.tourists) ? d.tourists : []
    services.value = Array.isArray(d?.services) ? d.services : []
    clientPays.value = Array.isArray(d?.clientPays) ? d.clientPays : []
    partnerPays.value = Array.isArray(d?.partnerPays) ? d.partnerPays : []
    return true
  } catch (e) {
    return false
  }
}

function clearDraft() {
  sessionStorage.removeItem(DRAFT_KEY)
}

function triggerFilePick() {
  fileInputRef.value?.click()
}

function onFilesChosen(e) {
  files.value = Array.from(e?.target?.files ?? [])
}

async function prefillFromLead(leadId) {
  const lead = await getLead(Number(leadId))
  if (!lead) return
  m.leadId = lead.id
  if (!m.managerId)     m.managerId     = lead.managerId ?? null
  if (!m.sourceId)      m.sourceId      = lead.sourceId ?? null
  if (!m.requestTypeId) m.requestTypeId = lead.requestTypeId ?? null
  if (!m.tourName) m.tourName = lead.country ? `Тур в ${lead.country}` : (m.tourName || '')
  if (m.price == null && lead.budget != null) m.price = lead.budget
  if (!m.startDate && lead.desiredArrival)   m.startDate = String(lead.desiredArrival).slice(0,10)
  if (!m.endDate   && lead.desiredDeparture) m.endDate   = String(lead.desiredDeparture).slice(0,10)
}

const dicts = reactive({
  statuses: [], offices: [], companies: [],
  requestTypes: [], sources: [], tourOperators: [],
  visaTypes: [], managers: []
})

const m = reactive({
  statusId: null, leadId: null, touristId: null, managerId: null,
  officeId: null, companyId: null,
  issuerLegalEntityId: null, requestTypeId: null, sourceId: null,
  tourOperatorId: null, visaTypeId: null,
  bookingNumbers: '', note: '',
  clientPaymentDeadline: null, partnerPaymentDeadline: null,
  docsPackageDate: null, addStampAndSign: false, includeCostCalc: false, includeTourCalc: false,
  tourName: '', price: null, startDate: null, endDate: null
})

const companyOptions = computed(() => {
  const dict = Array.isArray(dicts.companies) ? dicts.companies : []
  const mine = myCompany.value ? [{id: myCompany.value.id, name: myCompany.value.name}] : []
  const map = new Map()
  ;[...dict, ...mine].forEach(c => c && c.id && map.set(c.id, {id: c.id, name: c.name}))
  return Array.from(map.values())
})
watch(
    () => myCompany.value && myCompany.value.id,
    (id) => {
      if (!isEdit.value && !m.companyId && id) m.companyId = id
    },
    {immediate: true}
)

const todayStr = computed(() => {
  const d = new Date()
  const y = d.getFullYear()
  const m = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  return `${y}-${m}-${day}`
})

function parseDateOnly(v) {
  if (!v) return null
  const [y, m, d] = String(v).split('-').map(Number)
  return new Date(y, (m ?? 1) - 1, d ?? 1)
}

const rules = {
  required: v => (v !== null && v !== undefined && v !== '') || 'Обязательное поле',
  email: v => (!v || /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(String(v))) || 'Неверный email',
  phone: v => (!v || String(v).replace(/\D/g, '').length >= 8) || 'Телефон слишком короткий',
  money: v => (v === null || v === '' || (!Number.isNaN(Number(v)) && Number(v) >= 0)) || 'Укажите число ≥ 0',
  notPastDate: v => {
    if (!v) return true
    const picked = parseDateOnly(v)
    const now = new Date(); const today = new Date(now.getFullYear(), now.getMonth(), now.getDate())
    return (picked && picked >= today) || 'Дата не может быть в прошлом'
  },
}

const customers = ref([])
const tourists = ref([])
const services = ref([])
const clientPays = ref([])
const partnerPays = ref([])

const search = reactive({
  open: false,
  type: /** @type {'customer'|'tourist'|'service'|'clientPay'|'partnerPay'} */ ('customer'),
  idx: null,
  query: '',
  loading: false,
  results: [],
  selected: null,
  error: ''
})

const createFormRef = ref(null)
const create = reactive({
  open: false,
  type: /** @type {'service'|'clientPay'|'partnerPay'|'customer'|'tourist'} */ ('service'),
  valid: false,
  model: {}
})

const createTitle = computed(() => {
  switch (create.type) {
    case 'service':
      return 'Создание услуги'
    case 'clientPay':
      return 'Создание расчёта с клиентом'
    case 'partnerPay':
      return 'Создание расчёта с партнёром'
    default:
      return 'Создание'
  }
})
const searchPlaceholder = computed(() => {
  switch (search.type) {
    case 'customer':
    case 'tourist':
      return 'Поиск клиента по ФИО / телефону / email'
    case 'service':
      return 'Поиск услуги по названию'
    case 'clientPay':
    case 'partnerPay':
      return 'Поиск расчёта по названию'
    default:
      return 'Поиск'
  }
})

function openPicker(type, idx = null) {
  search.type = type
  search.idx = idx
  search.query = ''
  search.results = []
  search.selected = null
  search.error = ''
  search.open = true
}

function replaceFrom(type, idx) {
  search.type = type
  search.idx = idx
  saveDraft()
  router.push({
    name: 'ClientCreate',
    query: {
      back: isEdit.value ? 'DealEdit' : 'DealCreate',
      backId: isEdit.value ? String(route.params.id) : undefined,
      role: type,
      isTourist: type === 'tourist' ? '1' : '0',
      replaceIndex: String(idx)
    }
  })
}

function openCreateFromSearch() {
  if (search.type === 'customer' || search.type === 'tourist') {
    saveDraft()
    router.push({
      name: 'ClientCreate',
      query: {
        back: isEdit.value ? 'DealEdit' : 'DealCreate',
        backId: isEdit.value ? String(route.params.id) : undefined,
        role: search.type,
        isTourist: search.type === 'tourist' ? '1' : '0'
      }
    })
    return
  }
  create.type = search.type
  create.model = create.type === 'service'
      ? {name: search.query || '', note: ''}
      : {title: search.query || '', amount: null}
  create.valid = false
  create.open = true
}

function choose(r) {
  const addOrUpdate = (arr, obj) => {
    if (search.idx === null || search.idx === undefined) arr.push(obj)
    else arr.splice(search.idx, 1, {...arr[search.idx], ...obj})
  }

  if (search.type === 'customer' || search.type === 'tourist') {
    const obj = {id: r.id, fullName: r.fullName, phone: r.phone || null, email: r.email || null}
    const target = search.type === 'tourist' ? tourists : customers

    if (obj.id && target.value.some(x => x.id === obj.id)) {
      search.error = search.type === 'tourist'
          ? 'Этот турист уже добавлен'
          : 'Этот клиент уже добавлен'
      return
    }

    addOrUpdate(target.value, obj)
    search.open = false
    return
  }

  if (search.type === 'service') {
    addOrUpdate(services.value, {name: r.name, note: r.note || null})
  } else if (search.type === 'clientPay') {
    addOrUpdate(clientPays.value, {title: r.title, amount: r.amount ?? null})
  } else if (search.type === 'partnerPay') {
    addOrUpdate(partnerPays.value, {title: r.title, amount: r.amount ?? null})
  }
  search.open = false
}

async function saveCreate() {
  const ok = await createFormRef.value?.validate()
  if (!ok?.valid) return
  const addOrUpdate = (arr, obj) => {
    if (search.idx === null || search.idx === undefined) arr.push(obj)
    else arr.splice(search.idx, 1, {...arr[search.idx], ...obj})
  }
  switch (create.type) {
    case 'service':
      addOrUpdate(services.value, {
        name: String(create.model.name || '').trim(),
        note: create.model.note?.trim() || null
      })
      break
    case 'clientPay':
      addOrUpdate(clientPays.value, {
        title: String(create.model.title || '').trim(),
        amount: (create.model.amount === '' || create.model.amount == null) ? null : Number(create.model.amount)
      })
      break
    case 'partnerPay':
      addOrUpdate(partnerPays.value, {
        title: String(create.model.title || '').trim(),
        amount: (create.model.amount === '' || create.model.amount == null) ? null : Number(create.model.amount)
      })
      break
  }
  create.open = false
  search.open = false
}

let searchTimer = null
watch(() => search.query, (q) => {
  clearTimeout(searchTimer)
  if (!q || !q.trim()) {
    search.results = []
    search.selected = null
    search.error = ''
    return
  }
  searchTimer = setTimeout(async () => {
    search.loading = true
    try {
      if (search.type === 'customer' || search.type === 'tourist') {
        const arr = await apiSearchClients(q.trim())
        const filtered = (search.type === 'tourist') ? arr.filter(c => c.isTourist) : arr
        search.results = filtered.map(mapClientResult)
      } else if (search.type === 'service') {
        const arr = await apiSearchServiceNames(q.trim())
        search.results = (arr || []).map(mapServiceResult)
      } else {
        const arr = await apiSearchPaymentTitles(q.trim(), {kind: search.type})
        search.results = (arr || []).map(mapPaymentResult)
      }
    } catch {
      search.results = []
    } finally {
      search.loading = false
    }
  }, 350)
})

function toKey() {
  return Math.random().toString(36).slice(2)
}

function mapClientResult(c) {
  const full = c.fullName || [c.lastName, c.firstName, c.middleName].filter(Boolean).join(' ')
  return {
    _key: toKey(),
    id: c.id,
    display: full,
    sub: [c.phoneE164, c.email].filter(Boolean).join(' · '),
    fullName: full,
    phone: c.phoneE164 || '',
    email: c.email || ''
  }
}

function mapServiceResult(s) {
  return {_key: toKey(), display: s.name, sub: s.note || '', name: s.name, note: s.note || ''}
}

function mapPaymentResult(p) {
  return {
    _key: toKey(),
    display: p.title,
    sub: p.amount != null ? String(p.amount) : '',
    title: p.title,
    amount: p.amount ?? null
  }
}

async function apiSearchClients(q) {
  const u = new URL('/api/clients', window.location.origin)
  u.searchParams.set('q', q)
  u.searchParams.set('page', '1')
  u.searchParams.set('pageSize', '10')
  const r = await fetch(u.toString(), {credentials: 'include'})
  if (!r.ok) return []
  const data = await r.json()
  return Array.isArray(data) ? data : []
}

async function apiSearchServiceNames(q) {
  const u = new URL('/api/deals/services/suggest', window.location.origin)
  if (m.companyId || myCompany.value?.id) u.searchParams.set('companyId', String(m.companyId || myCompany.value.id))
  u.searchParams.set('query', q)
  const r = await fetch(u.toString(), {credentials: 'include'})
  if (!r.ok) return []
  const data = await r.json()
  return Array.isArray(data) ? data : (data.items ?? [])
}

async function apiSearchPaymentTitles(q, {kind} = {}) {
  const u = new URL('/api/deals/payments/suggest', window.location.origin)
  if (m.companyId || myCompany.value?.id) u.searchParams.set('companyId', String(m.companyId || myCompany.value.id))
  u.searchParams.set('query', q)
  if (kind) u.searchParams.set('kind', kind)
  const r = await fetch(u.toString(), {credentials: 'include'})
  if (!r.ok) return []
  const data = await r.json()
  return Array.isArray(data) ? data : (data.items ?? [])
}

async function loadDicts() {
  Object.assign(dicts, await fetchDealDicts().catch(() => ({})))
}

function toDateInput(v) {
  return v ? String(v).slice(0, 10) : null
}

function fullNameOf(x) {
  return x?.fullName || [x?.lastName, x?.firstName, x?.middleName].filter(Boolean).join(' ').trim()
}

async function loadIfEdit() {
  if (!isEdit.value) return
  const dto = await getDeal(Number(route.params.id))

  Object.assign(m, {
    statusId: dto.statusId ?? null,
    leadId: dto.leadId ?? null,
    touristId: dto.touristId ?? null,
    managerId: dto.managerId ?? null,
    officeId: dto.officeId ?? null,
    companyId: dto.companyId ?? null,
    issuerLegalEntityId: dto.issuerLegalEntityId ?? null,
    requestTypeId: dto.requestTypeId ?? null,
    sourceId: dto.sourceId ?? null,
    tourOperatorId: dto.tourOperatorId ?? null,
    visaTypeId: dto.visaTypeId ?? null,
    bookingNumbers: dto.bookingNumbers || '',
    note: dto.note || '',
    clientPaymentDeadline: toDateInput(dto.clientPaymentDeadline),
    partnerPaymentDeadline: toDateInput(dto.partnerPaymentDeadline),
    docsPackageDate: toDateInput(dto.docsPackageDate),
    addStampAndSign: !!dto.addStampAndSign,
    includeCostCalc: !!dto.includeCostCalc,
    includeTourCalc: !!dto.includeTourCalc,
    tourName: dto.tourName || '',
    price: dto.price ?? null,
  })

  customers.value = (dto.customers ?? []).map(x => {
    const c = x.client ?? x
    return {
      id: c.id ?? x.clientId ?? null,
      fullName: fullNameOf(c) || 'Без имени',
      phone: c.phoneE164 ?? c.phone ?? null,
      email: c.email ?? null,
    }
  })

  tourists.value = (dto.tourists ?? []).map(x => {
    const c = x.client ?? x
    return {
      id: c.id ?? x.clientId ?? null,
      fullName: fullNameOf(c) || 'Без имени',
      phone: c.phoneE164 ?? c.phone ?? null,
      email: c.email ?? null,
    }
  })

  services.value = (dto.services ?? []).map(s => ({
    name: s.name ?? s.title ?? '',
    note: s.note ?? null,
  }))

  clientPays.value = (dto.clientPays ?? []).map(p => ({
    title: p.title ?? '',
    amount: p.amount ?? null,
  }))

  partnerPays.value = (dto.partnerPays ?? []).map(p => ({
    title: p.title ?? '',
    amount: p.amount ?? null,
  }))
}

async function addClientById(id, role, replaceIndex) {
  try {
    const dto = await getClient(Number(id), {includeDeleted: true})
    const full = dto.fullName || [dto.lastName, dto.firstName, dto.middleName].filter(Boolean).join(' ').trim()
    const obj = {id: dto.id, fullName: full || 'Без имени', phone: dto.phoneE164 || null, email: dto.email || null}
    const target = role === 'tourist' ? tourists : customers
    if (replaceIndex != null && Number.isFinite(Number(replaceIndex))) {
      const idx = Number(replaceIndex)
      if (idx >= 0 && idx < target.value.length) target.value.splice(idx, 1, obj)
      else target.value.push(obj)
    } else {
      const existsAt = target.value.findIndex(x => x.id === obj.id && obj.id != null)
      if (existsAt >= 0) target.value.splice(existsAt, 1, obj)
      else target.value.push(obj)
    }
  } catch (e) {
    console.error('Failed to fetch added client', e)
  }
}

function clean(obj) {
  return Object.fromEntries(
      Object.entries(obj).filter(([, v]) =>
          v !== null && v !== undefined && !(typeof v === 'string' && v.trim() === '')
      )
  )
}

async function submit() {
  const ok = await formRef.value?.validate()
  if (!ok?.valid) return
  saving.value = true
  try {
    const raw = {
      statusId: m.statusId ?? undefined,
      leadId: m.leadId ?? undefined,
      touristId: m.touristId ?? undefined,
      managerId: m.managerId,
      officeId: m.officeId ?? undefined,
      companyId: m.companyId ?? undefined,
      issuerLegalEntityId: m.issuerLegalEntityId ?? undefined,
      requestTypeId: m.requestTypeId ?? undefined,
      sourceId: m.sourceId ?? undefined,
      tourOperatorId: m.tourOperatorId ?? undefined,
      visaTypeId: m.visaTypeId ?? undefined,
      bookingNumbers: m.bookingNumbers?.trim(),
      note: m.note?.trim(),
      clientPaymentDeadline: m.clientPaymentDeadline || undefined,
      partnerPaymentDeadline: m.partnerPaymentDeadline || undefined,
      docsPackageDate: m.docsPackageDate || undefined,
      addStampAndSign: !!m.addStampAndSign,
      includeCostCalc: !!m.includeCostCalc,
      includeTourCalc: !!m.includeTourCalc,
      tourName: m.tourName?.trim(),
      price: m.price ?? undefined,

      customers: customers.value.map(c =>
          c.id ? {clientId: c.id}
              : {fullName: String(c.fullName || '').trim(), phone: c.phone || null, email: c.email || null}
      ),
      tourists: tourists.value.map(t =>
          t.id ? {clientId: t.id}
              : {fullName: String(t.fullName || '').trim(), phone: t.phone || null, email: t.email || null}
      ),
      services: services.value.map(s => ({name: String(s.name || '').trim(), note: s.note?.trim() || null})),
      clientPays: clientPays.value.map(p => ({
        title: String(p.title || '').trim(),
        amount: (p.amount === '' || p.amount == null) ? null : Number(p.amount)
      })),
      partnerPays: partnerPays.value.map(p => ({
        title: String(p.title || '').trim(),
        amount: (p.amount === '' || p.amount == null) ? null : Number(p.amount)
      })),
      startDate: m.startDate || undefined,
      endDate:   m.endDate   || undefined,
    }

    const payload = clean(raw)

    if (isEdit.value) await updateDeal(Number(route.params.id), payload)
    else await createDeal(payload)

    clearDraft()
    goBack()
  } finally {
    saving.value = false
  }
}

function removeFrom(kind, idx) {
  const map = {
    customer: customers,
    tourist: tourists,
    service: services,
    clientPay: clientPays,
    partnerPay: partnerPays
  }
  map[kind]?.value.splice(idx, 1)
}

function goBack() {
  router.push({name: 'DealsList'})
}

onMounted(async () => {
  await ensureLoaded()
  await loadDicts()
  await loadIfEdit()

  const leadId = route.query.leadId
  if (!isEdit.value && leadId) {
    await prefillFromLead(leadId)
    const q = { ...route.query }; delete q.leadId
    router.replace({ query: q })
  }

  const addedId = route.query.addedClientId
  const role = route.query.role
  const replaceIndex = route.query.replaceIndex
  if (addedId && (role === 'customer' || role === 'tourist')) {
    loadDraft()
    await addClientById(addedId, role, replaceIndex)
    const q = {...route.query}
    delete q.addedClientId; delete q.role; delete q.replaceIndex
    router.replace({query: q})
  }

  if (!isEdit.value && !m.companyId && myCompany.value?.id) m.companyId = myCompany.value.id
})

function onTabClick(name) {
  console.info('tab click:', name)
}
</script>

<style scoped>
.page {
  width: 100%;
  padding-inline: 16px;
  box-sizing: border-box;
  margin-top: 60px
}

.toolbar {
  display: flex;
  align-items: center;
  gap: 12px;
  background: var(--color-baby-powder);
  padding: 0 0 16px
}

.grow {
  flex: 1
}

.btn-secondary {
  background: var(--color-pear) !important;
  color: #1a1a1a !important;
  font-weight: 600;
}

.card {
  border: 1px solid rgba(0, 0, 0, .12);
  border-radius: 12px;
  overflow: hidden
}

.field-gap > * + * { margin-top: 12px; }

.tabs-in-card {
  display: flex;
  gap: 6px;
  padding: 10px 10px 0 10px;
  background: #f5f6ef;
  border-bottom: 1px solid rgba(0, 0, 0, .08);
}

.tab-btn {
  padding: 6px 10px;
  border: 1px solid rgba(0, 0, 0, .2);
  border-bottom-color: rgba(0, 0, 0, .25);
  border-radius: 6px 6px 0 0;
  background: #f3f3f3;
  font-size: .92rem;
  cursor: pointer;
  user-select: none;
}

.tab-btn.active {
  background: #e9efcf;
  border-color: #c2cc8b;
  font-weight: 600
}

.card-body {
  background: #fff
}

.grid-12 {
  display: grid;
  grid-template-columns:repeat(12, minmax(0, 1fr));
  column-gap: 24px;
  row-gap: 16px;
  margin-bottom: 18px
}

.grid-12:last-child {
  margin-bottom: 0
}

.col-2 {
  grid-column: span 2
}

.col-3 {
  grid-column: span 3
}

.col-4 {
  grid-column: span 4
}

.col-6 {
  grid-column: span 6
}

.col-8 {
  grid-column: span 8
}

.col-12 {
  grid-column: span 12
}

.section-title {
  font-weight: 600;
  margin: 26px 0 12px
}

.sections {
  display: flex;
  flex-direction: column;
  gap: 14px
}

.section {
  background: #fff;
  border: 1px solid rgba(0, 0, 0, .12);
  border-radius: 12px;
  padding: 10px 12px
}

.section-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-weight: 600
}

.section-table {
  margin-top: 8px
}

.section-table .row {
  display: grid;
  grid-template-columns: 2fr 1.4fr 1.6fr 96px;
  gap: 12px;
  padding: 8px 4px;
  border-top: 1px solid #eee;
  align-items: center;
}

.section-table .actions {
  margin-left: auto !important;
}

.section-table .row.head {
  font-weight: 600;
  border-top: none;
  padding-top: 0
}

.actions {
  display: flex;
  gap: 4px;
  justify-content: flex-end
}

.actions-col {
  width: 96px
}

.hidden-input {
  display: none
}

.attach-btn {
  text-transform: none
}

.files-list {
  display: flex;
  gap: 10px;
  flex-wrap: wrap
}

.file-item {
  background: #fff;
  border: 1px solid rgba(0, 0, 0, .12);
  border-radius: 8px;
  padding: 6px 10px;
  font-size: .9rem
}

:deep(.v-field) {
  border-radius: 10px
}

.results .res-row {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 8px 10px;
  border: 1px solid #eee;
  border-radius: 8px;
  margin-bottom: 8px;
}

.results .res-row.active {
  border-color: #c2cc8b;
  background: #f8fbe8;
}

.results .res-main {
  flex: 1;
  cursor: pointer;
}

.results .res-title {
  font-weight: 600;
}

.results .res-sub {
  font-size: .9rem;
  color: #666;
}

:deep(.v-input--density-compact) {
  --v-input-control-height: 44px
}

@media (max-width: 1100px) {
  .col-2, .col-3, .col-4, .col-6, .col-8, .col-12 {
    grid-column: 1 / -1
  }

  .section-table .row {
    grid-template-columns: 1fr 1fr 1fr 80px
  }
}
</style>