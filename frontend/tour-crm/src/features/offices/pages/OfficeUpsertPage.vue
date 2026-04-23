<template>
  <v-container class="page pt-6">
    <div class="toolbar">
      <h1 class="text-h6 text-ink">{{ isEdit ? 'Редактирование офиса' : 'Добавление офиса' }}</h1>
      <div class="d-flex" style="gap:12px">
        <PermissionTooltip :can="canSubmit">
          <v-btn
            class="pill"
            color="secondary"
            :loading="saving"
            :ripple="false"
            @click="save"
            v-can.disable="submitPerm"
            :disabled="!canSubmit"
          >
            {{ isEdit ? 'Сохранить' : 'Создать' }}
          </v-btn>
        </PermissionTooltip>
        <v-btn variant="text" :ripple="false" @click="goBack">Отменить</v-btn>
      </div>
    </div>

    <div v-if="!canViewPage" class="content-wrap bg-paper ta-center py-8">Нет доступа</div>

    <div v-else class="content-wrap bg-paper">
      <v-form ref="formRef" validate-on="input" @submit.prevent="save">
        <v-row>
          <v-col cols="12" md="6">
            <v-select
              v-model="form.legalEntityId"
              :items="legalOptions"
              item-title="name"
              item-value="id"
              label="Юридическое лицо *"
              :rules="[rules.required]"
              variant="outlined"
              density="comfortable"
              hide-details="auto"
            />
          </v-col>

          <v-col cols="12" md="6">
            <v-text-field
              v-model="form.name"
              label="Название офиса *"
              placeholder="Например: Офис"
              hint="Короткое уникальное имя офиса в рамках выбранного юрлица"
              :rules="[rules.required, rules.officeName]"
              variant="outlined"
              density="comfortable"
              hide-details="auto"
              persistent-hint
              counter="100"
            />
          </v-col>

          <v-col cols="12" md="6">
            <v-text-field
              v-model="form.address"
              label="Адрес / Город"
              variant="outlined"
              density="comfortable"
              hide-details="auto"
              counter="500"
              :rules="[rules.max(500)]"
            />
          </v-col>

          <v-col cols="12" md="6">
            <v-text-field
              v-model="form.phone"
              label="Телефон"
              placeholder="+7 777 123-45-67, +7 (727) 123-45-67"
              variant="outlined"
              density="comfortable"
              hide-details="auto"
              counter="64"
              :rules="[rules.phone]"
            />
          </v-col>

          <v-col cols="12" md="6">
            <v-text-field
              v-model="form.email"
              label="Email"
              type="email"
              placeholder="office@example.com"
              variant="outlined"
              density="comfortable"
              hide-details="auto"
              counter="150"
              :rules="[rules.email, rules.max(150)]"
            />
          </v-col>

          <v-col cols="12" md="6">
            <v-text-field
              v-model.number="form.leadLimit"
              type="number"
              min="0"
              label="Лимит лидов"
              variant="outlined"
              density="comfortable"
              hide-details="auto"
              :rules="[rules.leadLimit]"
            />
          </v-col>
        </v-row>
      </v-form>
    </div>
  </v-container>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useSessionStore } from '@/app/store/sessionStore'
import { getOffice, createOffice, updateOffice } from '../services/officesService'
import { fetchLegalEntities } from '@/features/touragent/services/companyService'
import PermissionTooltip from '@/shared/components/PermissionTooltip.vue'

const auth = useSessionStore()
const canCreate = computed(() => auth.has('CreateOffices'))
const canEdit = computed(() => auth.has('EditOffices'))

const route = useRoute()
const router = useRouter()
const isEdit = computed(() => !!route.params.id)
const canSubmit = computed(() => (isEdit.value ? canEdit.value : canCreate.value))
const canViewPage = computed(() => (isEdit.value ? canEdit.value : canCreate.value))
const submitPerm = computed(() => (isEdit.value ? 'EditOffices' : 'CreateOffices'))

const formRef = ref(null)
const saving = ref(false)
const legalOptions = ref([])

const form = ref({
  legalEntityId: Number(route.query.legal || 0),
  name: '',
  address: '',
  phone: '',
  email: '',
  leadLimit: null
})

const rules = {
  required: v => String(v ?? '').trim().length > 0 || 'Обязательное поле',
  max: n => v => !v || String(v).length <= n || `Не более ${n} символов`,
  officeName: v => { const s = String(v ?? '').trim(); return (s.length >= 2 && s.length <= 100) || 'От 2 до 100 символов' },
  phone: v => { if (!v) return true; const s = String(v).trim(); if (s.length > 64) return 'Максимум 64 символа'; return (/^[0-9+()\-\s]+$/).test(s) || 'Допустимы только цифры, пробел, + ( ) -' },
  email: v => { if (!v) return true; return (/^[^\s@]+@[^\s@]+\.[^\s@]+$/).test(String(v).trim()) || 'Некорректный email' },
  leadLimit: v => { if (v === null || v === undefined || v === '') return true; const n = Number(v); if (!Number.isFinite(n) || !Number.isInteger(n)) return 'Должно быть целое число'; return n >= 0 || 'Не может быть отрицательным' }
}

function goBack() {
  const legal = form.value.legalEntityId || route.query.legal
  router.push({ name: 'Offices', query: { legal: legal || undefined } })
}

async function load() {
  const legals = await fetchLegalEntities()
  legalOptions.value = (Array.isArray(legals) ? legals : []).map(x => ({ id: x.id, name: x.name }))
  if (!isEdit.value) return
  const dto = await getOffice(route.params.id)
  form.value = {
    legalEntityId: dto.legalEntityId,
    name: dto.name ?? '',
    address: dto.address ?? dto.city ?? '',
    phone: dto.phone ?? '',
    email: dto.email ?? '',
    leadLimit: dto.leadLimit ?? null
  }
}

async function save() {
  if (!canSubmit.value) return
  const ok = await formRef.value?.validate()
  if (!ok?.valid) return
  const payload = {
    legalEntityId: Number(form.value.legalEntityId),
    name: (form.value.name || '').trim(),
    address: (form.value.address || '').trim() || null,
    phone: (form.value.phone || '').trim() || null,
    email: (form.value.email || '').trim() || null,
    leadLimit: form.value.leadLimit ?? null
  }
  saving.value = true
  try {
    if (isEdit.value) await updateOffice(route.params.id, payload)
    else await createOffice(payload)
    goBack()
  } finally {
    saving.value = false
  }
}

onMounted(load)
</script>

<style scoped>
.toolbar{ display:flex; align-items:center; gap:16px; background: var(--color-baby-powder); padding:14px 16px; border-bottom:none; flex-wrap:nowrap; }
.content-wrap{ border:1px solid rgba(0,0,0,.12); border-radius:12px; padding:16px; margin-top:0; }
</style>
