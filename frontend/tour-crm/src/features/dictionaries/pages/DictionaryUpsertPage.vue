<template>
  <v-container class="page pt-3">
    <h1 class="text-h6 text-ink">
      {{ isEdit ? 'Редактирование' : 'Создание' }} — {{ config.title }}
    </h1>

    <v-form ref="formRef" @submit.prevent="save">
      <template v-for="f in config.fields" :key="f.key">
        <v-text-field
          v-if="!f.type || f.type === 'text'"
          v-model="form[f.key]"
          :label="f.label"
          variant="outlined"
          density="comfortable"
          :rules="f.required ? [v => !!v || 'Обязательное поле'] : []"
        />
        <v-select
          v-else-if="f.type === 'select'"
          v-model="form[f.key]"
          :label="f.label"
          :items="options[f.source] ?? []"
          item-value="id"
          item-title="name"
          :return-object="false"
          variant="outlined"
          density="comfortable"
          :rules="f.required ? [v => !!v || 'Обязательное поле'] : []"
          clearable
        />
        <v-checkbox
            v-else-if="f.type === 'checkbox'"
            v-model="form[f.key]"
            :label="f.label"
            :true-value="true"
            :false-value="false"
            density="comfortable"
            hide-details
        />
      </template>

      <div class="mt-4 flex gap-2">
        <PermissionTooltip :can="canSave">
          <v-btn
            color="primary"
            type="submit"
            v-can.disable="isEdit ? 'EditDictionaries' : 'CreateDictionaries'"
            :disabled="!canSave"
          >
            Сохранить
          </v-btn>
        </PermissionTooltip>
        <v-btn variant="text" @click="cancel">Отмена</v-btn>
      </div>
    </v-form>
  </v-container>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useSessionStore } from '@/app/store/sessionStore'
import { dictionaries } from '/src/features/dictionaries/dictionaryConfig'
import dictionaryService from '@/features/dictionaries/services/dictionaryService'
import PermissionTooltip from '@/shared/components/PermissionTooltip.vue'

const auth = useSessionStore()
const route = useRoute()
const router = useRouter()

const dictKey = String(route.params.dict || '')
const id = route.params.id
const isEdit = computed(() => !!id)
const canSave = computed(() => (isEdit.value ? auth.has('EditDictionaries') : auth.has('CreateDictionaries')))

const config = dictionaries[dictKey]
if (!config) throw new Error(`Справочник "${dictKey}" не найден в dictionaryConfig`)

const formRef = ref(null)
const form = ref({})
const options = ref({})

async function load() {
  form.value = id
      ? (await dictionaryService.getById(config.endpoint, id)) ?? {}
      : Object.fromEntries(
          config.fields.map(f => [f.key, f.type === 'checkbox' ? false : null])
      )

  for (const f of config.fields) {
    if (f.type === 'select' && f.source) {
      const list = await dictionaryService.fetchAll(f.source)
      options.value[f.source] = (Array.isArray(list) ? list : []).map(x => ({
        id: x?.id,
        name: x?.name ?? x?.title ?? String(x?.id),
      }))
    }
  }
}

function normalizePayload(src) {
  const payload = { ...src }

  for (const k of Object.keys(payload)) {
    if (payload[k] === 'true')  payload[k] = true
    if (payload[k] === 'false') payload[k] = false
  }

  for (const k of Object.keys(payload)) {
    if (k.endsWith('Id')) {
      const v = payload[k]
      payload[k] = v === '' || v == null ? null : Number(v)
    }
  }
  if ('stars' in payload) {
    const s = payload.stars
    payload.stars = s === '' || s == null ? null : Number(s)
  }
  for (const k of Object.keys(payload)) {
    if (typeof payload[k] === 'string') {
      const t = payload[k].trim()
      payload[k] = t === '' ? null : t
    }
  }
  return payload
}

async function save() {
  if (!canSave.value) return
  if (formRef.value?.validate) {
    const { valid } = await formRef.value.validate()
    if (!valid) return
  }
  const payload = normalizePayload(form.value)
  if (id) {
    await dictionaryService.update(config.endpoint, id, payload)
  } else {
    await dictionaryService.create(config.endpoint, payload)
  }
  await router.push({ name: 'Dictionary', params: { dict: dictKey } })
}

function cancel() {
  router.push({ name: 'Dictionary', params: { dict: dictKey } })
}

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

h1 {
  font-size: 24px;
  font-weight: 800;
  background: linear-gradient(135deg, var(--brand-ink) 0%, var(--brand-primary) 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin: 32px 0 24px 0;
  letter-spacing: -0.2px;
  padding: 0 8px;
}

:deep(.v-form) {
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.95) 0%, rgba(242, 243, 237, 0.8) 100%);
  padding: 32px;
  border-radius: 20px;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(139, 146, 109, 0.15);
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.06);
  position: relative;
  overflow: hidden;
}

:deep(.v-form)::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 3px;
  background: linear-gradient(90deg, var(--color-pear), var(--brand-primary));
  border-radius: 20px 20px 0 0;
}

:deep(.v-text-field),
:deep(.v-select),
:deep(.v-checkbox) {
  margin-bottom: 20px;
}

:deep(.v-field) {
  border-radius: 12px;
  background: rgba(255, 255, 255, 0.9);
  border: 1.5px solid rgba(139, 146, 109, 0.15);
  transition: all 0.3s ease;
}

:deep(.v-field:focus-within) {
  border-color: var(--brand-primary);
  box-shadow: 0 4px 15px rgba(139, 146, 109, 0.15);
  transform: translateY(-1px);
}

:deep(.v-field__outline) {
  display: none;
}

:deep(.v-label) {
  color: var(--brand-ink);
  font-weight: 500;
  opacity: 0.8;
}

:deep(.v-field--focused .v-label) {
  color: var(--brand-primary);
  opacity: 1;
}

:deep(.v-input__details) {
  padding-left: 8px;
}

:deep(.v-messages__message) {
  color: #d32f2f;
  font-size: 12px;
  font-weight: 500;
}

:deep(.v-select .v-field) {
  cursor: pointer;
}

:deep(.v-select .v-field__append-inner) {
  color: var(--brand-primary);
}

:deep(.v-checkbox .v-selection-control) {
  min-height: auto;
  align-items: center;
}

:deep(.v-checkbox .v-selection-control__input) {
  margin-right: 12px;
}

:deep(.v-checkbox .v-label) {
  opacity: 1;
  font-weight: 600;
  color: var(--brand-ink);
}

:deep(.v-checkbox .v-selection-control__wrapper) {
  border-radius: 6px;
}

:deep(.v-checkbox .v-icon) {
  color: var(--brand-primary);
}

:deep(.v-checkbox:hover .v-selection-control__wrapper) {
  background: rgba(139, 146, 109, 0.1);
}

.flex {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-top: 32px;
  padding-top: 24px;
  border-top: 1px solid rgba(139, 146, 109, 0.1);
}

:deep(.v-btn) {
  border-radius: 12px;
  text-transform: none;
  font-weight: 600;
  letter-spacing: 0.3px;
  height: 44px;
  padding: 0 24px;
  transition: all 0.3s ease;
}

:deep(.v-btn--variant-primary) {
  background: linear-gradient(135deg, var(--color-pear) 0%, #d4e2a8 100%) !important;
  color: var(--brand-ink) !important;
  box-shadow: 0 4px 15px rgba(206, 219, 149, 0.3) !important;
  border: 1px solid rgba(206, 219, 149, 0.3) !important;
}

:deep(.v-btn--variant-primary:hover) {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(206, 219, 149, 0.5) !important;
}

:deep(.v-btn--variant-text) {
  color: var(--brand-ink) !important;
  background: rgba(139, 146, 109, 0.1) !important;
  border: 1px solid transparent !important;
}

:deep(.v-btn--variant-text:hover) {
  background: rgba(139, 146, 109, 0.15) !important;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(139, 146, 109, 0.2);
}

:deep(.v-btn:disabled) {
  opacity: 0.5;
  transform: none !important;
  box-shadow: none !important;
}

:deep(.v-text-field),
:deep(.v-select),
:deep(.v-checkbox) {
  animation: slideUp 0.4s ease-out;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Адаптивность */
@media (max-width: 960px) {
  .page {
    padding-inline: 16px;
  }

  h1 {
    font-size: 20px;
    margin: 24px 0 20px 0;
  }

  :deep(.v-form) {
    padding: 24px;
    border-radius: 16px;
  }
}

@media (max-width: 600px) {
  .page {
    padding-inline: 12px;
  }

  h1 {
    font-size: 18px;
    margin: 20px 0 16px 0;
  }

  :deep(.v-form) {
    padding: 20px 16px;
    border-radius: 12px;
  }

  .flex {
    flex-direction: column;
    align-items: stretch;
    gap: 8px;
  }

  :deep(.v-btn) {
    width: 100%;
    margin: 0 !important;
  }

  :deep(.v-text-field),
  :deep(.v-select),
  :deep(.v-checkbox) {
    margin-bottom: 16px;
  }
}

:deep(.v-menu .v-overlay__content) {
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.12);
}

:deep(.v-menu .v-overlay__content .v-list) {
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.98) 0%, rgba(242, 243, 237, 0.95) 100%);
  backdrop-filter: blur(10px);
}

:deep(.v-list-item--active) {
  background: rgba(139, 146, 109, 0.1) !important;
  color: var(--brand-primary) !important;
}

:deep(.v-list-item:hover) {
  background: rgba(139, 146, 109, 0.05) !important;
}
</style>
