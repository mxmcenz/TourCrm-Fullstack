<template>
  <v-container class="page pt-6">
    <div class="toolbar">
      <h1 class="text-h6 text-ink">
        {{ isEdit ? 'Редактирование юридического лица' : 'Добавление юридического лица' }}
      </h1>
      <div class="d-flex" style="gap:12px">
        <PermissionTooltip :can="canSave">
          <v-btn
            class="pill"
            color="secondary"
            :loading="saving"
            :ripple="false"
            :disabled="!canSave || saving"
            @click="save"
            v-can.disable="isEdit ? 'EditLegalEntities' : 'CreateLegalEntities'"
          >
            {{ isEdit ? 'Сохранить' : 'Создать' }}
          </v-btn>
        </PermissionTooltip>
        <v-btn variant="text" :ripple="false" @click="goBack">Отменить</v-btn>
      </div>
    </div>

    <div v-if="!canView && !canSave" class="content-wrap bg-paper ta-center py-8">
      Нет доступа
    </div>

    <div v-else class="content-wrap bg-paper">
      <v-tabs v-model="tab" density="comfortable" class="mb-4">
        <v-tab value="base">Основные данные</v-tab>
        <v-tab value="bank">Банковские реквизиты</v-tab>
      </v-tabs>

      <v-window v-model="tab">
        <v-window-item value="base">
          <v-form ref="formRef" validate-on="input" @submit.prevent="save">
            <div class="form-grid">
              <div class="field-row">
                <label>Название (рус.)</label>
                <v-text-field v-model="form.nameRu" variant="outlined" density="comfortable" hide-details="auto"
                              :rules="[rules.required, rules.max(150)]" />
              </div>

              <div class="field-row">
                <label>Название (англ.)</label>
                <v-text-field v-model="form.nameEn" variant="outlined" density="comfortable" hide-details="auto"
                              :rules="[rules.max(150)]" />
              </div>

              <div class="field-row">
                <label>Полное название</label>
                <v-text-field v-model="form.nameFull" variant="outlined" density="comfortable" hide-details="auto"
                              :rules="[rules.max(200)]" />
              </div>

              <div class="field-row">
                <label>Страна</label>
                <v-select
                  v-model="form.countryId"
                  :items="countries"
                  item-title="name"
                  item-value="id"
                  :disabled="!canUseDicts"
                  variant="outlined"
                  density="comfortable"
                  hide-details="auto"
                  clearable
                  :loading="loadingCountries"
                />
              </div>

              <div class="field-row">
                <label>Город</label>
                <v-select
                  v-model="form.cityId"
                  :items="cities"
                  item-title="name"
                  item-value="id"
                  :disabled="!canUseDicts || !form.countryId"
                  variant="outlined"
                  density="comfortable"
                  hide-details="auto"
                  clearable
                  :loading="loadingCities"
                />
              </div>

              <div class="field-row field-textarea">
                <label>Адрес юридический</label>
                <v-textarea v-model="form.legalAddress" rows="3" variant="outlined" density="comfortable"
                            hide-details="auto" :rules="[rules.max(300)]" />
              </div>

              <div class="field-row field-textarea">
                <label>Адрес фактический</label>
                <v-textarea v-model="form.actualAddress" rows="3" variant="outlined" density="comfortable"
                            hide-details="auto" :rules="[rules.max(300)]" />
              </div>

              <div class="field-row">
                <label>ФИО директора</label>
                <v-text-field v-model="form.directorFio" variant="outlined" density="comfortable" hide-details="auto"
                              :rules="[rules.max(150)]" />
              </div>

              <div class="field-row">
                <label>ФИО директора (родит. падеж)</label>
                <v-text-field v-model="form.directorFioGen" variant="outlined" density="comfortable" hide-details="auto"
                              :rules="[rules.max(150)]" />
              </div>

              <div class="field-row">
                <label>Должность директора</label>
                <v-text-field v-model="form.directorPost" variant="outlined" density="comfortable" hide-details="auto"
                              :rules="[rules.max(100)]" />
              </div>

              <div class="field-row">
                <label>Должность директора (родит. падеж)</label>
                <v-text-field v-model="form.directorPostGen" variant="outlined" density="comfortable" hide-details="auto"
                              :rules="[rules.max(100)]" />
              </div>

              <div class="field-row">
                <label>Директор действует на основании</label>
                <v-text-field v-model="form.directorBasis" variant="outlined" density="comfortable" hide-details="auto"
                              :rules="[rules.max(100)]" />
              </div>

              <div class="field-row">
                <label>ФИО главного бухгалтера</label>
                <v-text-field v-model="form.cfoFio" variant="outlined" density="comfortable" hide-details="auto"
                              :rules="[rules.max(150)]" />
              </div>

              <div class="field-row">
                <label>Телефоны</label>
                <v-text-field v-model="form.phones" placeholder="несколько — через запятую" variant="outlined"
                              density="comfortable" hide-details="auto" :rules="[rules.phonesList]" />
              </div>

              <div class="field-row">
                <label>Основной сайт</label>
                <v-text-field v-model="form.website" variant="outlined" density="comfortable" hide-details="auto"
                              :rules="[rules.url, rules.max(200)]" />
              </div>

              <div class="field-row">
                <label>Email</label>
                <v-text-field v-model="form.email" type="email" variant="outlined" density="comfortable"
                              hide-details="auto" :rules="[rules.email, rules.max(150)]" />
              </div>

              <div class="field-row">
                <label>БИН/ИИН</label>
                <v-text-field v-model="form.binIin" variant="outlined" density="comfortable" hide-details="auto"
                              :rules="[rules.binIin]" />
              </div>
            </div>
          </v-form>
        </v-window-item>

        <v-window-item value="bank">
          <v-form ref="formRefBank" @submit.prevent="save">
            <v-alert type="info" variant="tonal" class="mb-4">
              Временная версия: можно вставить JSON или обычный текст с реквизитами.
            </v-alert>
            <v-textarea v-model="form.bankDetailsJson" rows="10" variant="outlined" density="comfortable"
                        placeholder='{"bank":"Kaspi","iban":"KZxx ..."} или свободный текст'
                        :messages="[bankJsonWarning].filter(Boolean)" hide-details="auto" />
          </v-form>
        </v-window-item>
      </v-window>
    </div>
  </v-container>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useSessionStore } from '@/app/store/sessionStore'
import PermissionTooltip from '@/shared/components/PermissionTooltip.vue'
import {
  getLegalEntity,
  createLegalEntity,
  updateLegalEntity,
  fetchCountries,
  fetchCities
} from '@/features/touragent/services/companyService'

const route = useRoute()
const router = useRouter()
const auth = useSessionStore()

const isEdit = computed(() => !!route.params.id)
const canView = computed(() => auth.has('ViewLegalEntities'))
const canCreate = computed(() => auth.has('CreateLegalEntities'))
const canEdit = computed(() => auth.has('EditLegalEntities'))
const canUseDicts = computed(() => auth.has('ViewDictionaries'))
const canSave = computed(() => (isEdit.value ? canEdit.value : canCreate.value))

const tab = ref('base')
const formRef = ref(null)
const saving = ref(false)

const form = ref({
  nameRu: '', nameEn: '', nameFull: '',
  countryId: null, cityId: null,
  legalAddress: '', actualAddress: '',
  directorFio: '', directorFioGen: '',
  directorPost: 'Директор', directorPostGen: 'Директора', directorBasis: 'Устава',
  cfoFio: '', phones: '', website: '', email: '', binIin: '', bankDetailsJson: ''
})

const countries = ref([])
const cities = ref([])
const loadingCountries = ref(false)
const loadingCities = ref(false)

const rules = {
  required: v => (v && String(v).trim().length > 1) || 'Обязательное поле',
  max: n => v => (!v || String(v).length <= n) || `Не более ${n} символов`,
  email: v => (!v || /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v)) || 'Некорректный email',
  url: v => { if (!v) return true; try { new URL(v.startsWith('http') ? v : `https://${v}`); return true } catch { return 'Некорректный URL' } },
  binIin: v => (!v || String(v).length <= 20) || 'БИН/ИИН: максимум 20 символов',
  phonesList: v => { if (!v) return true; const bad = String(v).split(',').map(s=>s.trim()).filter(s=>s.length>32 || /[^0-9+()\-\s]/.test(s)); return bad.length===0 || 'Проверьте телефоны' }
}

const bankJsonWarning = computed(() => {
  const val = form.value.bankDetailsJson?.trim()
  if (!val) return ''
  const looksLikeJson = val.startsWith('{') || val.startsWith('[')
  if (!looksLikeJson) return ''
  try { JSON.parse(val); return '' } catch { return 'Похоже на JSON, но не парсится — будет сохранено как текст' }
})

function goBack(){ router.push({ name: 'Company' }) }

async function safeFetchCountries(){
  if (!canUseDicts.value) { countries.value = []; return }
  loadingCountries.value = true
  try { countries.value = await fetchCountries() }
  catch { countries.value = [] }
  finally { loadingCountries.value = false }
}

async function safeFetchCities(countryId){
  if (!canUseDicts.value || !countryId) { cities.value = []; return }
  loadingCities.value = true
  try {
    const list = await fetchCities({ countryId })
    cities.value = Array.isArray(list) ? list.filter(c => !c.countryId || String(c.countryId)===String(countryId)) : []
    if (!cities.value.some(c => String(c.id) === String(form.value.cityId))) form.value.cityId = null
  } catch { cities.value = [] }
  finally { loadingCities.value = false }
}

watch(() => form.value.countryId, cid => { form.value.cityId = null; safeFetchCities(cid) })

async function load(){
  if (isEdit.value && !canEdit.value) { goBack(); return }
  if (!isEdit.value && !canCreate.value) { goBack(); return }

  await safeFetchCountries()

  if (!isEdit.value) return
  const id = route.params.id
  const dto = await getLegalEntity(id).catch(() => null)
  if (!dto) return
  form.value = {
    nameRu: dto.nameRu ?? '', nameEn: dto.nameEn ?? '', nameFull: dto.nameFull ?? '',
    countryId: Number(dto.countryId ?? null), cityId: Number(dto.cityId ?? null),
    legalAddress: dto.legalAddress ?? '', actualAddress: dto.actualAddress ?? '',
    directorFio: dto.directorFio ?? '', directorFioGen: dto.directorFioGen ?? '',
    directorPost: dto.directorPost ?? 'Директор', directorPostGen: dto.directorPostGen ?? 'Директора', directorBasis: dto.directorBasis ?? 'Устава',
    cfoFio: dto.cfoFio ?? '', phones: dto.phones ?? dto.phone ?? '', website: dto.website ?? '',
    email: dto.email ?? '', binIin: dto.binIin ?? '', bankDetailsJson: dto.bankDetailsJson ?? ''
  }
  await safeFetchCities(form.value.countryId)
}

async function save(){
  if (!canSave.value) return
  const ok = await formRef.value?.validate()
  if (!ok?.valid) return
  const payload = {
    nameRu: form.value.nameRu?.trim() || null, nameEn: form.value.nameEn?.trim() || null, nameFull: form.value.nameFull?.trim() || null,
    countryId: form.value.countryId || null, cityId: form.value.cityId || null,
    legalAddress: form.value.legalAddress?.trim() || null, actualAddress: form.value.actualAddress?.trim() || null,
    directorFio: form.value.directorFio?.trim() || null, directorFioGen: form.value.directorFioGen?.trim() || null,
    directorPost: form.value.directorPost?.trim() || null, directorPostGen: form.value.directorPostGen?.trim() || null, directorBasis: form.value.directorBasis?.trim() || null,
    cfoFio: form.value.cfoFio?.trim() || null, phones: form.value.phones?.trim() || null, website: form.value.website?.trim() || null,
    email: form.value.email?.trim() || null, binIin: form.value.binIin?.trim() || null, bankDetailsJson: form.value.bankDetailsJson || null
  }
  saving.value = true
  try {
    if (isEdit.value) await updateLegalEntity(route.params.id, payload)
    else await createLegalEntity(payload)
    goBack()
  } finally { saving.value = false }
}

onMounted(load)
</script>

<style scoped>
.toolbar{ display:flex; align-items:center; gap:16px; background:var(--color-baby-powder); padding:14px 16px; border-bottom:none; }
.content-wrap{ border:1px solid rgba(0,0,0,.12); border-radius:12px; padding:16px; }
.form-grid{ display:grid; grid-template-columns: 1fr; gap:10px; }
.field-row{ display:grid; grid-template-columns:260px minmax(260px,1fr); align-items:center; gap:12px; }
.field-row>label{ color:var(--brand-ink); font-size:14px; }
.field-textarea{ align-items:start; }
.field-textarea>label{ padding-top:10px; }
</style>
