<template>
  <v-container>
    <div class="toolbar">
      <h1 class="text-h6">
        {{ isEdit ? 'Редактирование клиента' : 'Добавление клиента' }}
      </h1>
      <div class="d-flex">
        <PermissionTooltip :can="canSave">
          <v-btn
            :loading="saving"
            :ripple="false"
            color="secondary"
            class="pill"
            @click="save"
            v-can.disable="savePerm"
            :disabled="!canSave"
          >
            {{ isEdit ? 'Сохранить' : 'Создать' }}
          </v-btn>
        </PermissionTooltip>
        <v-btn variant="text" :ripple="false" @click="goBack">Отменить</v-btn>
      </div>
    </div>

    <div>
      <v-form ref="formRef" validate-on="input" @submit.prevent="save">
        <v-row align="end">
          <v-col cols="12" md="4">
            <v-select
              v-model="form.clientType"
              :items="clientTypeItems"
              item-title="label"
              item-value="value"
              label="Тип клиента *"
              :rules="[rules.required]"
              density="comfortable"
              variant="outlined"
              hide-details="auto"
              class="field"
            />
          </v-col>

          <v-col cols="12" md="4">
            <v-text-field
              v-model="form.firstName"
              label="Имя *"
              :rules="[rules.required, rules.max(100)]"
              density="comfortable"
              variant="outlined"
              hide-details="auto"
              class="field"
            />
          </v-col>

          <v-col cols="12" md="4">
            <v-text-field
              v-model="form.lastName"
              label="Фамилия *"
              :rules="[rules.required, rules.max(100)]"
              density="comfortable"
              variant="outlined"
              hide-details="auto"
              class="field"
            />
          </v-col>

          <v-col cols="12" md="4">
            <v-text-field
              v-model="form.middleName"
              label="Отчество"
              :rules="[rules.max(100)]"
              density="comfortable"
              variant="outlined"
              hide-details="auto"
              class="field"
            />
          </v-col>

          <v-col cols="12" md="4">
            <v-text-field
              v-model="form.phoneE164"
              label="Телефон *"
              placeholder="+77011234567"
              inputmode="tel"
              maxlength="20"
              :rules="[rules.required, rules.phoneE164, rules.max(20)]"
              density="comfortable"
              variant="outlined"
              hide-details="auto"
              class="field"
            />
          </v-col>

          <v-col cols="12" md="4">
            <v-text-field
              v-model="form.email"
              label="Email"
              type="email"
              :rules="[rules.email, rules.max(254)]"
              density="comfortable"
              variant="outlined"
              hide-details="auto"
              class="field"
            />
          </v-col>

          <v-col cols="12" md="4">
            <v-select
              v-model="form.gender"
              :items="genderItems"
              item-title="label"
              item-value="value"
              label="Пол"
              density="comfortable"
              variant="outlined"
              hide-details="auto"
              class="field"
            />
          </v-col>

          <v-col cols="12" md="4">
            <VDateInput
              v-model="form.birthDay"
              label="Дата рождения"
              :prepend-icon="null"
              append-inner-icon="mdi-calendar"
              :max="today"
              hide-details="auto"
              variant="outlined"
              density="comfortable"
              clearable
              locale="ru"
              class="field"
            />
          </v-col>

          <v-col cols="12" md="4">
            <v-text-field
              v-model.number="form.discountPercent"
              type="number"
              min="0" max="99.99" step="0.01"
              label="Скидка %"
              density="comfortable"
              variant="outlined"
              hide-details="auto"
              class="field"
            />
          </v-col>

          <v-col cols="12" md="3">
            <v-switch v-model="form.isTourist" inset hide-details color="primary" :label="'Турист'"/>
          </v-col>

          <v-col cols="12" md="3">
            <v-switch v-model="form.isSubscribedToMailing" inset hide-details color="primary"
                      :label="'Подписан на рассылку'"/>
          </v-col>

          <v-col cols="12" md="3">
            <v-switch v-model="form.isEmailNotificationEnabled" inset hide-details color="primary"
                      :label="'Email-уведомления включены'"/>
          </v-col>

          <v-col cols="12" md="3">
            <v-switch v-model="showGenitive" inset hide-details color="primary"
                      :label="'Показать ФИО в родительном падеже'"/>
          </v-col>

          <template v-if="showGenitive">
            <v-col cols="12" md="4">
              <v-text-field v-model="form.lastNameGenitive" label="Фамилия (род.п.)" density="comfortable"
                            variant="outlined" hide-details="auto" class="field"/>
            </v-col>
            <v-col cols="12" md="4">
              <v-text-field v-model="form.firstNameGenitive" label="Имя (род.п.)" density="comfortable"
                            variant="outlined" hide-details="auto" class="field"/>
            </v-col>
            <v-col cols="12" md="4">
              <v-text-field v-model="form.middleNameGenitive" label="Отчество (род.п.)" density="comfortable"
                            variant="outlined" hide-details="auto" class="field"/>
            </v-col>
          </template>

          <v-col cols="12">
            <v-textarea
              v-model="form.note"
              label="Примечание"
              rows="3"
              density="comfortable"
              variant="outlined"
              hide-details="auto"
              class="field"
            />
          </v-col>
        </v-row>

        <v-expansion-panels variant="accordion">
          <v-expansion-panel>
            <v-expansion-panel-title>Паспорт</v-expansion-panel-title>
            <v-expansion-panel-text>
              <v-row align="end">
                <v-col cols="12" md="4">
                  <v-text-field v-model="form.passport.firstNameLatin" label="Имя (лат.)" variant="outlined"
                                density="comfortable" hide-details="auto" class="field"/>
                </v-col>
                <v-col cols="12" md="4">
                  <v-text-field v-model="form.passport.lastNameLatin" label="Фамилия (лат.)" variant="outlined"
                                density="comfortable" hide-details="auto" class="field"/>
                </v-col>
                <v-col cols="12" md="4">
                  <v-text-field v-model="form.passport.serialNumber" label="Серия/номер" variant="outlined"
                                density="comfortable" hide-details="auto" class="field"/>
                </v-col>
                <v-col cols="12" md="4">
                  <VDateInput
                    v-model="form.passport.issueDate"
                    label="Дата выдачи"
                    :prepend-icon="null"
                    append-inner-icon="mdi-calendar"
                    :max="today"
                    hide-details="auto"
                    variant="outlined"
                    density="comfortable"
                    clearable
                    locale="ru"
                    class="field"
                  />
                </v-col>
                <v-col cols="12" md="4">
                  <VDateInput
                    v-model="form.passport.expireDate"
                    label="Действителен до"
                    :prepend-icon="null"
                    append-inner-icon="mdi-calendar"
                    :min="form.passport.issueDate || undefined"
                    hide-details="auto"
                    variant="outlined"
                    density="comfortable"
                    clearable
                    locale="ru"
                    class="field"
                  />
                </v-col>
                <v-col cols="12" md="4">
                  <v-text-field v-model="form.passport.issuingAuthority" label="Кем выдан" variant="outlined"
                                density="comfortable" hide-details="auto" class="field"/>
                </v-col>
              </v-row>
            </v-expansion-panel-text>
          </v-expansion-panel>

          <v-expansion-panel>
            <v-expansion-panel-title>Удостоверение личности</v-expansion-panel-title>
            <v-expansion-panel-text>
              <v-row align="end">
                <v-col cols="12" md="4">
                  <v-text-field
                    v-model="form.identityDocument.citizenshipCountryId"
                    v-digit-only
                    inputmode="numeric"
                    pattern="\d*"
                    label="Гражданство (ID страны)"
                    :rules="[rules.required, rules.intId]"
                    density="comfortable"
                    variant="outlined"
                    hide-details="auto"
                    class="field"
                  />
                </v-col>
                <v-col cols="12" md="4">
                  <v-text-field
                    v-model="form.identityDocument.residenceCountryId"
                    v-digit-only
                    inputmode="numeric"
                    pattern="\d*"
                    label="Страна проживания (ID)"
                    :rules="[rules.required, rules.intId]"
                    density="comfortable"
                    variant="outlined"
                    hide-details="auto"
                    class="field"
                  />
                </v-col>
                <v-col cols="12" md="4">
                  <v-text-field
                    v-model="form.identityDocument.residenceCityId"
                    v-digit-only
                    inputmode="numeric"
                    pattern="\d*"
                    label="Город проживания (ID)"
                    :rules="[rules.required, rules.intId]"
                    density="comfortable"
                    variant="outlined"
                    hide-details="auto"
                    class="field"
                  />
                </v-col>
                <v-col cols="12" md="4">
                  <v-text-field v-model="form.identityDocument.birthPlace" label="Место рождения" variant="outlined"
                                density="comfortable" hide-details="auto" class="field"/>
                </v-col>
                <v-col cols="12" md="4">
                  <v-text-field v-model="form.identityDocument.serialNumber" label="Серия/номер" variant="outlined"
                                density="comfortable" hide-details="auto" class="field"/>
                </v-col>
                <v-col cols="12" md="4">
                  <v-text-field v-model="form.identityDocument.issuedBy" label="Кем выдан" variant="outlined"
                                density="comfortable" hide-details="auto" class="field"/>
                </v-col>
                <v-col cols="12" md="4">
                  <VDateInput
                    v-model="form.identityDocument.issueDate"
                    label="Дата выдачи"
                    :prepend-icon="null"
                    append-inner-icon="mdi-calendar"
                    :max="today"
                    hide-details="auto"
                    variant="outlined"
                    density="comfortable"
                    clearable
                    locale="ru"
                    class="field"
                  />
                </v-col>
                <v-col cols="12" md="4">
                  <v-text-field v-model="form.identityDocument.documentNumber" label="Номер документа"
                                variant="outlined" density="comfortable" hide-details="auto" class="field"/>
                </v-col>
                <v-col cols="12" md="4">
                  <v-text-field v-model="form.identityDocument.personalNumber" label="ИИН/Личный №" variant="outlined"
                                density="comfortable" hide-details="auto" class="field"/>
                </v-col>
                <v-col cols="12" md="6">
                  <v-text-field v-model="form.identityDocument.registrationAddress" label="Адрес регистрации"
                                variant="outlined" density="comfortable" hide-details="auto" class="field"/>
                </v-col>
                <v-col cols="12" md="6">
                  <v-text-field v-model="form.identityDocument.residentialAddress" label="Адрес проживания"
                                variant="outlined" density="comfortable" hide-details="auto" class="field"/>
                </v-col>
                <v-col cols="12" md="6">
                  <v-text-field v-model="form.identityDocument.motherFullName" label="Мать (ФИО)" variant="outlined"
                                density="comfortable" hide-details="auto" class="field"/>
                </v-col>
                <v-col cols="12" md="6">
                  <v-text-field v-model="form.identityDocument.fatherFullName" label="Отец (ФИО)" variant="outlined"
                                density="comfortable" hide-details="auto" class="field"/>
                </v-col>
                <v-col cols="12" md="6">
                  <v-text-field v-model="form.identityDocument.contactInfo" label="Контакты" variant="outlined"
                                density="comfortable" hide-details="auto" class="field"/>
                </v-col>
              </v-row>
            </v-expansion-panel-text>
          </v-expansion-panel>

          <v-expansion-panel>
            <v-expansion-panel-title>Свидетельство о рождении</v-expansion-panel-title>
            <v-expansion-panel-text>
              <v-row align="end">
                <v-col cols="12" md="4">
                  <v-text-field v-model="form.birthCertificate.serialNumber" label="Серия/номер" variant="outlined"
                                density="comfortable" hide-details="auto" class="field"/>
                </v-col>
                <v-col cols="12" md="4">
                  <v-text-field v-model="form.birthCertificate.issuedBy" label="Кем выдано" variant="outlined"
                                density="comfortable" hide-details="auto" class="field"/>
                </v-col>
                <v-col cols="12" md="4">
                  <VDateInput
                    v-model="form.birthCertificate.issueDate"
                    label="Дата выдачи"
                    :prepend-icon="null"
                    append-inner-icon="mdi-calendar"
                    hide-details="auto"
                    variant="outlined"
                    density="comfortable"
                    clearable
                    locale="ru"
                    class="field"
                  />
                </v-col>
              </v-row>
            </v-expansion-panel-text>
          </v-expansion-panel>

          <v-expansion-panel>
            <v-expansion-panel-title>Страховки</v-expansion-panel-title>
            <v-expansion-panel-text>
              <v-row class="section-actions" align="center" justify="end">
                <v-col cols="12" class="d-flex justify-end">
                  <v-btn size="small" variant="tonal" @click="addInsurance">+ Добавить страховку</v-btn>
                </v-col>
              </v-row>

              <v-row v-for="(ins, idx) in form.insurances" :key="'ins-'+idx" align="end">
                <v-col cols="12" md="2">
                  <VDateInput
                    v-model="ins.issueDate"
                    label="Выдана"
                    :prepend-icon="null"
                    append-inner-icon="mdi-calendar"
                    hide-details="auto"
                    variant="outlined"
                    density="comfortable"
                    clearable
                    locale="ru"
                    class="field"
                  />
                </v-col>
                <v-col cols="12" md="2">
                  <VDateInput
                    v-model="ins.expireDate"
                    label="Действует до"
                    :prepend-icon="null"
                    append-inner-icon="mdi-calendar"
                    :min="ins.issueDate || undefined"
                    hide-details="auto"
                    variant="outlined"
                    density="comfortable"
                    clearable
                    locale="ru"
                    class="field"
                  />
                </v-col>
                <v-col cols="12" md="2">
                  <v-text-field
                    v-model="ins.countryId"
                    v-digit-only
                    inputmode="numeric"
                    pattern="\d*"
                    label="Страна (ID)"
                    :rules="[rules.intId]"
                    density="comfortable"
                    variant="outlined"
                    hide-details="auto"
                    class="field"
                  />
                </v-col>
                <v-col cols="12" md="5">
                  <v-text-field
                    v-model="ins.note"
                    label="Примечание"
                    variant="outlined"
                    density="comfortable"
                    hide-details="auto"
                    class="field"
                  />
                </v-col>
                <v-col cols="12" md="1" class="d-flex align-center">
                  <v-btn icon variant="text" @click="form.insurances.splice(idx,1)">
                    <v-icon icon="mdi-close"/>
                  </v-btn>
                </v-col>
              </v-row>
            </v-expansion-panel-text>
          </v-expansion-panel>

          <v-expansion-panel>
            <v-expansion-panel-title>Визы</v-expansion-panel-title>
            <v-expansion-panel-text>
              <v-row class="section-actions" align="center" justify="end">
                <v-col cols="12" class="d-flex justify-end">
                  <v-btn size="small" variant="tonal" @click="addVisa">+ Добавить визу</v-btn>
                </v-col>
              </v-row>
              <v-row v-for="(v, idx) in form.visas" :key="'visa-'+idx" align="end">
                <v-col cols="12" md="2">
                  <VDateInput
                    v-model="v.issueDate"
                    label="Выдана"
                    :prepend-icon="null"
                    append-inner-icon="mdi-calendar"
                    hide-details="auto"
                    variant="outlined"
                    density="comfortable"
                    clearable
                    locale="ru"
                    class="field"
                  />
                </v-col>
                <v-col cols="12" md="2">
                  <VDateInput
                    v-model="v.expireDate"
                    label="Действует до"
                    :prepend-icon="null"
                    append-inner-icon="mdi-calendar"
                    :min="v.issueDate || undefined"
                    hide-details="auto"
                    variant="outlined"
                    density="comfortable"
                    clearable
                    locale="ru"
                    class="field"
                  />
                </v-col>
                <v-col cols="12" md="2">
                  <v-text-field
                    v-model="v.countryId"
                    v-digit-only
                    inputmode="numeric"
                    pattern="\d*"
                    label="Страна (ID)"
                    :rules="[rules.intId]"
                    density="comfortable"
                    variant="outlined"
                    hide-details="auto"
                    class="field"
                  />
                </v-col>

                <v-col cols="12" md="3">
                  <v-text-field
                    v-model="v.note"
                    label="Примечание"
                    variant="outlined"
                    density="comfortable"
                    hide-details="auto"
                    class="field"
                  />
                </v-col>

                <v-col cols="12" md="2">
                  <v-switch v-model="v.isSchengen" inset hide-details color="primary" :label="'Шенген'"/>
                </v-col>
                <v-col cols="12" md="1" class="d-flex align-center">
                  <v-btn icon variant="text" @click="form.visas.splice(idx,1)">
                    <v-icon icon="mdi-close"/>
                  </v-btn>
                </v-col>
              </v-row>
            </v-expansion-panel-text>
          </v-expansion-panel>
        </v-expansion-panels>
      </v-form>
    </div>
  </v-container>
</template>

<script setup>
// eslint-disable-next-line no-undef
/* global defineProps */
import {ref, computed, onMounted} from 'vue'
import {useRouter,useRoute} from 'vue-router'
import {useSessionStore} from '@/app/store/sessionStore'
import {getClient, createClient, updateClient} from '@/features/clients/services/clientsService'
import {VDateInput} from 'vuetify/labs/VDateInput'
import PermissionTooltip from '@/shared/components/PermissionTooltip.vue'

const router = useRouter()
const route = useRoute()
const auth = useSessionStore()

const props = defineProps({ id: {type: Number, default: null}, isTourist: {type: Boolean, default: false} })
const isEdit = computed(() => !!props.id)
const savePerm = computed(() => (isEdit.value ? 'EditClients' : 'CreateClients'))
const canSave = computed(() => auth.has(savePerm.value))

const formRef = ref(null)
const saving = ref(false)
const showGenitive = ref(false)
const today = new Date().toISOString().slice(0, 10)

const clientTypeItems = [
  {value: 0, label: 'Физ.лицо'},
  {value: 1, label: 'Юр.лицо'},
  {value: 2, label: 'Агентство'}
]
const genderItems = [
  {value: 0, label: 'Не указан'},
  {value: 1, label: 'Мужской'},
  {value: 2, label: 'Женский'}
]

const form = ref({
  clientType: 0,
  managerId: null,
  firstName: '',
  lastName: '',
  middleName: '',
  firstNameGenitive: '',
  lastNameGenitive: '',
  middleNameGenitive: '',
  birthDay: null,
  gender: 0,
  phoneE164: '',
  email: '',
  isSubscribedToMailing: false,
  isEmailNotificationEnabled: false,
  referredBy: '',
  note: '',
  discountPercent: 0,
  isTourist: props.isTourist === true,
  passport: { firstNameLatin:'', lastNameLatin:'', serialNumber:'', issueDate:null, expireDate:null, issuingAuthority:'' },
  identityDocument: {
    citizenshipCountryId: null, residenceCountryId: null, residenceCityId: null, birthPlace: '',
    serialNumber: '', issuedBy: '', issueDate: null, documentNumber: '', personalNumber: '',
    registrationAddress: '', residentialAddress: '', motherFullName: '', fatherFullName: '', contactInfo: ''
  },
  birthCertificate: { serialNumber: '', issuedBy: '', issueDate: null },
  insurances: [],
  visas: []
})

const rules = {
  required: v => String(v ?? '').trim().length > 0 || 'Обязательное поле',
  max: n => v => !v || String(v).length <= n || `Не более ${n} символов`,
  email: v => { if (!v) return true; return (/^[^\s@]+@[^\s@]+\.[^\s@]+$/).test(String(v).trim()) || 'Некорректный email' },
  digits: v => v === null || v === '' || /^\d+$/.test(String(v)) || 'Только цифры',
  intId: v => v === null || v === '' || (/^\d+$/.test(String(v)) && Number(v) > 0) || 'Положительное число',
  phoneE164: v => { if (!v) return true; const s=String(v).trim().replace(/[\s()-]/g,''); if (/^\+7\d{10}$/.test(s)) return true; if (/^\+[1-9]\d{9,14}$/.test(s)) return true; return 'Телефон в формате E.164, напр. +77011234567' },
}

function goBack() { router.push({name: 'ClientList'}) }

async function load() {
  if (!isEdit.value) return
  const dto = await getClient(props.id, {includeDeleted: true})
  form.value = {
    clientType: dto.clientType,
    managerId: dto.managerId,
    firstName: dto.firstName ?? '',
    lastName: dto.lastName ?? '',
    middleName: dto.middleName ?? '',
    firstNameGenitive: dto.firstNameGenitive ?? '',
    lastNameGenitive: dto.lastNameGenitive ?? '',
    middleNameGenitive: dto.middleNameGenitive ?? '',
    birthDay: dto.birthDay ?? null,
    gender: dto.gender ?? 0,
    phoneE164: dto.phoneE164 ?? '',
    email: dto.email ?? '',
    isSubscribedToMailing: dto.isSubscribedToMailing ?? false,
    isEmailNotificationEnabled: dto.isEmailNotificationEnabled ?? false,
    referredBy: dto.referredBy ?? '',
    note: dto.note ?? '',
    discountPercent: dto.discountPercent ?? 0,
    isTourist: dto.isTourist ?? false,
    passport: dto.passport ?? { firstNameLatin:'', lastNameLatin:'', serialNumber:'', issueDate:null, expireDate:null, issuingAuthority:'' },
    identityDocument: dto.identityDocument ?? {
      citizenshipCountryId:null, residenceCountryId:null, residenceCityId:null, birthPlace:'', serialNumber:'', issuedBy:'', issueDate:null,
      documentNumber:'', personalNumber:'', registrationAddress:'', residentialAddress:'', motherFullName:'', fatherFullName:'', contactInfo:''
    },
    birthCertificate: dto.birthCertificate ?? {serialNumber:'', issuedBy:'', issueDate:null},
    insurances: Array.isArray(dto.insurances) ? dto.insurances : [],
    visas: Array.isArray(dto.visas) ? dto.visas : []
  }
}

async function save() {
  if (!canSave.value || saving.value) return
  const res = await formRef.value?.validate()
  if (!res?.valid) return

  const payload = normalizePayload(form.value)

  saving.value = true
  try {
    let clientId = null
    if (isEdit.value) {
      await updateClient(props.id, payload)
      clientId = props.id
    } else {
      const created = await createClient(payload)
      clientId = created?.id ?? created?.data?.id ?? null
    }

    if (route.query.back) {
      const backName = String(route.query.back)
      const backParams = route.query.backId ? { id: Number(route.query.backId) } : undefined
      const backQuery = {
        role: route.query.role ? String(route.query.role) : undefined,
        replaceIndex: route.query.replaceIndex ?? undefined,
        addedClientId: clientId != null ? String(clientId) : undefined
      }
      await router.push({ name: backName, params: backParams, query: backQuery })
    } else {
      await router.push({ name: 'ClientList' })
    }
  } catch (e) {
    const status = e?.response?.status
    if (status === 409) { alert('Клиент с таким телефоном уже существует в вашей компании.'); return }
    console.error('save client error', e?.response?.data || e)
    alert(`Ошибка сохранения${status ? `: ${status}` : ''}`)
  } finally {
    saving.value = false
  }
}


function addInsurance() { form.value.insurances.push({issueDate: null, expireDate: null, countryId: null, note: ''}) }
function addVisa() { form.value.visas.push({issueDate: null, expireDate: null, countryId: null, isSchengen: false, note: ''}) }

onMounted(load)

function objEmpty(o){ return Object.values(o || {}).every(v => v === null || v === '' || v === false) }
function toIntOrNull(v){ if (v === null || v === '') return null; const n = Number(v); return Number.isFinite(n) ? n : null }
function toPhoneE164(v){ if (!v) return ''; let s=String(v).trim().replace(/[\s()-]/g,''); if (s[0] !== '+') s='+'+s.replace(/^\+/, ''); if (s.length===12 && s.startsWith('+8')) s = '+7'+s.slice(2); return s }
function toYMD(v){ if (!v) return null; if (typeof v === 'string') return v.slice(0,10); const d=new Date(v); if (Number.isNaN(d.getTime())) return null; const y=d.getFullYear(); const m=String(d.getMonth()+1).padStart(2,'0'); const da=String(d.getDate()).padStart(2,'0'); return `${y}-${m}-${da}` }

function normalizePayload(src){
  const p = structuredClone(src)
  p.birthDay = toYMD(p.birthDay)
  p.phoneE164 = toPhoneE164(p.phoneE164)

  if (p.passport){ p.passport.issueDate = toYMD(p.passport.issueDate); p.passport.expireDate = toYMD(p.passport.expireDate); if (objEmpty(p.passport)) p.passport = null }
  if (p.identityDocument){
    p.identityDocument.issueDate = toYMD(p.identityDocument.issueDate)
    p.identityDocument.citizenshipCountryId = toIntOrNull(p.identityDocument.citizenshipCountryId)
    p.identityDocument.residenceCountryId = toIntOrNull(p.identityDocument.residenceCountryId)
    p.identityDocument.residenceCityId = toIntOrNull(p.identityDocument.residenceCityId)
    if (objEmpty(p.identityDocument)) p.identityDocument = null
  }
  if (p.birthCertificate){ p.birthCertificate.issueDate = toYMD(p.birthCertificate.issueDate); if (objEmpty(p.birthCertificate)) p.birthCertificate = null }

  if (Array.isArray(p.insurances)){
    p.insurances = p.insurances
      .map(x => ({...x, issueDate: toYMD(x.issueDate), expireDate: toYMD(x.expireDate), countryId: toIntOrNull(x.countryId)}))
      .filter(x => !objEmpty(x))
  }
  if (Array.isArray(p.visas)){
    p.visas = p.visas
      .map(x => ({...x, issueDate: toYMD(x.issueDate), expireDate: toYMD(x.expireDate), countryId: toIntOrNull(x.countryId)}))
      .filter(x => !objEmpty(x))
  }
  return p
}
</script>

<script>
export default {
  directives: {
    digitOnly: {
      mounted(el) {
        el.addEventListener('input', () => {
          const i = el.querySelector('input');
          if (!i) return;
          const cur = i.value || '';
          const next = cur.replace(/\D+/g, '');
          if (cur !== next) i.value = next;
          i.dispatchEvent(new Event('change', {bubbles: true}));
          i.dispatchEvent(new Event('input', {bubbles: true}));
        });
      }
    }
  }
}
</script>

<style scoped>
.toolbar { display: flex; align-items: center; gap: 16px; background: var(--color-baby-powder); padding: 14px 16px; border-bottom: none; flex-wrap: nowrap; }
.pill { border-radius: 9999px; text-transform: none; }
.field :deep(.v-field) { min-height: 40px; }
.section-actions { margin-bottom: 8px; }
</style>
