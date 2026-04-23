<template>
  <div class="full-page-bg">
    <div class="main-layout">
      <main class="content">
        <v-toolbar flat class="form-toolbar mb-4">
          <v-toolbar-title class="text-h5">Добавление сотрудника</v-toolbar-title>
          <v-spacer/>
          <v-btn icon @click="goBack" class="close-btn">
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </v-toolbar>

        <v-tabs v-model="tab" class="form-tabs mb-4">
          <v-tab value="main" class="tab-item">Основные данные</v-tab>
          <v-tab value="additional" class="tab-item">Дополнительно</v-tab>
        </v-tabs>

        <div class="form-scroll">
          <v-form ref="formRef" v-model="valid" autocomplete="off">
            <v-window v-model="tab">
              <v-window-item value="main">
                <div class="form-fields">
                  <v-row class="row-3 mb-2">
                    <v-col cols="4">
                      <v-text-field v-model="form.lastName" :rules="[rules.required]" variant="outlined"
                                    density="comfortable">
                        <template #label>Фамилия <span class="text-red">*</span></template>
                      </v-text-field>
                    </v-col>
                    <v-col cols="4">
                      <v-text-field v-model="form.firstName" :rules="[rules.required]" variant="outlined"
                                    density="comfortable">
                        <template #label>Имя <span class="text-red">*</span></template>
                      </v-text-field>
                    </v-col>
                    <v-col cols="4">
                      <v-text-field v-model="form.middleName" label="Отчество" variant="outlined"
                                    density="comfortable"/>
                    </v-col>
                  </v-row>

                  <v-row class="row-3 mb-2 same-height">
                    <v-col cols="4">
                      <div class="with-tip">
                        <v-select
                            v-model="form.legalEntityId"
                            :items="legals"
                            item-title="name"
                            item-value="id"
                            label="Юр.лицо"
                            :rules="[rules.required]"
                            variant="outlined"
                            density="comfortable"
                            clearable
                            :return-object="false"
                            :disabled="!canViewLegals"
                            :menu-icon="canViewLegals ? 'mdi-menu-down' : 'mdi-lock-outline'"
                        />
                        <v-tooltip v-if="!canViewLegals" activator="parent" text="Нет доступа"/>
                      </div>
                    </v-col>

                    <v-col cols="4">
                      <div class="with-tip">
                        <v-select
                            v-model="form.officeId"
                            :items="offices"
                            item-title="name"
                            item-value="id"
                            label="Офис"
                            :rules="[rules.required]"
                            variant="outlined"
                            density="comfortable"
                            clearable
                            :return-object="false"
                            :disabled="!canViewOffices"
                            :menu-icon="canViewOffices ? 'mdi-menu-down' : 'mdi-lock-outline'"
                        />
                        <v-tooltip v-if="!canViewOffices" activator="parent" text="Нет доступа"/>
                      </div>
                    </v-col>

                    <v-col cols="4">
                      <div class="with-tip">
                        <v-select
                            v-model="form.roleIds"
                            :items="selectableRoles"
                            item-title="name"
                            item-value="id"
                            label="Роли"
                            multiple
                            chips
                            :rules="[rules.required]"
                            variant="outlined"
                            density="comfortable"
                            :disabled="!canViewRoles"
                            :menu-icon="canViewRoles ? 'mdi-menu-down' : 'mdi-lock-outline'"
                        >
                          <template #label>Роли <span class="text-red">*</span></template>
                        </v-select>
                        <v-tooltip v-if="!canViewRoles" activator="parent" text="Нет доступа"/>
                      </div>
                    </v-col>
                  </v-row>

                  <v-row class="row-3 mb-1">
                    <v-col cols="4">
                      <v-text-field v-model="form.email" :rules="[rules.required, rules.email]" variant="outlined"
                                    density="comfortable">
                        <template #label>Email <span class="text-red">*</span></template>
                      </v-text-field>
                    </v-col>
                    <v-col cols="4">
                      <v-text-field
                          v-model="form.password"
                          type="password"
                          :rules="[rules.required, rules.length]"
                          variant="outlined"
                          density="comfortable"
                      >
                        <template #label>Пароль <span class="text-red">*</span></template>
                      </v-text-field>
                    </v-col>
                    <v-col cols="4">
                      <v-text-field
                          v-model="passwordConfirm"
                          type="password"
                          :rules="[rules.required, passwordMatch]"
                          variant="outlined"
                          density="comfortable"
                      >
                        <template #label>Повторите пароль <span class="text-red">*</span></template>
                      </v-text-field>
                    </v-col>
                  </v-row>

                  <v-row class="mb-2 justify-end">
                    <v-col cols="12" class="d-flex justify-end">
                      <PermissionTooltip :can="canCreate">
                        <div class="gen-wrap" v-can.disable="'CreateEmployees'">
                          <v-btn class="gen-btn" icon variant="text" :disabled="!canCreate" @click="onGeneratePassword">
                            <v-icon>mdi-refresh</v-icon>
                          </v-btn>
                          <span class="gen-text">Сгенерировать пароль</span>
                        </div>
                      </PermissionTooltip>
                    </v-col>
                  </v-row>
                </div>
              </v-window-item>

              <v-window-item value="additional">
                <div class="form-fields">
                  <v-row class="row-2 mb-2">
                    <v-col cols="6">
                      <v-text-field v-model="form.position" label="Должность" variant="outlined" density="comfortable"/>
                    </v-col>
                    <v-col cols="6">
                      <v-text-field v-model="form.positionGenitive" label="Должность (род. падеж)" variant="outlined"
                                    density="comfortable"/>
                    </v-col>
                  </v-row>

                  <v-row class="mb-2">
                    <v-col cols="6">
                      <v-text-field v-model="form.powerOfAttorneyNumber" label="Номер доверенности" variant="outlined"
                                    density="comfortable"/>
                    </v-col>
                  </v-row>

                  <v-row class="mb-2">
                    <v-col cols="6">
                      <v-text-field v-model="form.lastNameGenitive" label="Фамилия (род. падеж)" variant="outlined"
                                    density="comfortable"/>
                    </v-col>
                    <v-col cols="6">
                      <v-text-field v-model="form.firstNameGenitive" label="Имя (род. падеж)" variant="outlined"
                                    density="comfortable"/>
                    </v-col>
                    <v-col cols="6">
                      <v-text-field v-model="form.middleNameGenitive" label="Отчество (род. падеж)" variant="outlined"
                                    density="comfortable"/>
                    </v-col>
                  </v-row>

                  <v-row class="mb-2">
                    <v-col cols="6">
                      <v-text-field v-model="form.phone" label="Телефон (основной)" :rules="[rules.phone]"
                                    variant="outlined" density="comfortable"/>
                    </v-col>
                    <v-col cols="6">
                      <v-text-field v-model="form.additionalPhone" label="Телефон (дополнительный)" variant="outlined"
                                    density="comfortable"/>
                    </v-col>
                  </v-row>

                  <v-row class="mb-2">
                    <v-col cols="6">
                      <v-text-field v-model="form.birthDate" type="date" label="Дата рождения" variant="outlined"
                                    density="comfortable"/>
                    </v-col>
                    <v-col cols="6">
                      <v-text-field v-model="form.timeZone" label="Часовой пояс" variant="outlined"
                                    density="comfortable"/>
                    </v-col>
                  </v-row>

                  <v-row class="mb-2">
                    <v-col cols="6">
                      <v-textarea v-model="form.contactInfo" rows="2" label="Контактная информация" variant="outlined"
                                  density="comfortable"/>
                    </v-col>
                  </v-row>

                  <v-row class="mb-2">
                    <v-col cols="6">
                      <v-text-field v-model="form.hireDate" type="date" label="Дата принятия на работу"
                                    variant="outlined" density="comfortable"/>
                    </v-col>
                    <v-col cols="6">
                      <v-text-field v-model="form.salaryAmount" label="Размер з/п" variant="outlined"
                                    density="comfortable"/>
                    </v-col>
                  </v-row>

                  <v-row class="mb-2">
                    <v-col cols="6">
                      <v-textarea v-model="form.workConditions" rows="2" label="Условия работы" variant="outlined"
                                  density="comfortable"/>
                    </v-col>
                    <v-col cols="6">
                      <v-textarea v-model="form.note" rows="2" label="Примечания" variant="outlined"
                                  density="comfortable"/>
                    </v-col>
                  </v-row>
                </div>
              </v-window-item>
            </v-window>
          </v-form>
        </div>

        <div class="form-actions mt-4">
          <PermissionTooltip :can="canCreate">
            <v-btn color="#283106" :loading="saving" @click="onSave" class="save-btn" v-can.disable="'CreateEmployees'"
                   :disabled="!canCreate">Сохранить
            </v-btn>
          </PermissionTooltip>
          <v-btn variant="text" @click="goBack" class="cancel-btn">Отмена</v-btn>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import {ref, computed, onMounted, onBeforeUnmount, watch} from 'vue'
import {useRouter, onBeforeRouteLeave} from 'vue-router'
import {useEmployeeStore} from '@/app/store/employeeStore'
import {useSessionStore} from '@/app/store/sessionStore'
import PermissionTooltip from '@/shared/components/PermissionTooltip.vue'
import {rules} from '@/shared/utils/rules'

const router = useRouter()
const employeeStore = useEmployeeStore()
const auth = useSessionStore()

const canCreate = computed(() => auth.has('CreateEmployees'))
const canViewRoles = computed(() => auth.has('ViewRoles') || auth.has('ViewDictionaries') || auth.has('Admin'))
const canViewLegals = computed(() => auth.has('ViewLegalEntities') || auth.has('ViewDictionaries') || auth.has('Admin'))
const canViewOffices = computed(() => auth.has('ViewOffices') || auth.has('ViewDictionaries') || auth.has('Admin'))

const tab = ref('main')
const formRef = ref(null)
const valid = ref(false)
const saving = ref(false)
const skipLeaveGuard = ref(false)
const passwordConfirm = ref('')

const form = ref({
  firstName: '', lastName: '', middleName: '', email: '', phone: '',
  officeId: null, legalEntityId: null, roleIds: [], leadLimit: 0, password: '',
  position: '', positionGenitive: '', powerOfAttorneyNumber: '',
  lastNameGenitive: '', firstNameGenitive: '', middleNameGenitive: '',
  additionalPhone: '', birthDate: '', timeZone: '', contactInfo: '',
  hireDate: '', salaryAmount: null, workConditions: '', note: ''
})

const passwordMatch = computed(() => () => form.value.password === passwordConfirm.value || 'Пароли не совпадают')

onMounted(async () => {
  const loaders = []
  if (canViewRoles.value) loaders.push(employeeStore.fetchRoles())
  if (canViewLegals.value) loaders.push(employeeStore.fetchLegalEntities())
  await Promise.all(loaders)
  initialForm.value = JSON.parse(JSON.stringify(form.value))
  window.addEventListener('beforeunload', handleBeforeUnload)
})

const initialForm = ref(null)
const isDirty = computed(() => initialForm.value && JSON.stringify(form.value) !== JSON.stringify(initialForm.value))
onBeforeUnmount(() => window.removeEventListener('beforeunload', handleBeforeUnload))

function handleBeforeUnload(e) {
  if (isDirty.value && !skipLeaveGuard.value) {
    e.preventDefault();
    e.returnValue = ''
  }
}

onBeforeRouteLeave((to, from, next) => {
  if (skipLeaveGuard.value || !isDirty.value) next(); else if (confirm('У вас есть несохранённые данные. Выйти без сохранения?')) next(); else next(false)
})

const selectableRoles = computed(() => employeeStore.roles || [])
const legals = computed(() => employeeStore.legals || [])
const offices = computed(() => employeeStore.offices || [])

watch(() => form.value.legalEntityId, async (newLegalId) => {
  if (!newLegalId) {
    employeeStore.offices = [];
    form.value.officeId = null;
    return
  }
  if (!canViewOffices.value) return
  await employeeStore.fetchOfficesByLegal(newLegalId)
  const allowed = (employeeStore.offices || []).map(o => Number(o.id))
  if (!allowed.includes(Number(form.value.officeId))) form.value.officeId = null
})
watch(() => form.value.officeId, (newOfficeId) => {
  if (!newOfficeId) return
  const office = (employeeStore.offices || []).find(o => Number(o.id) === Number(newOfficeId))
  if (office?.legalEntityId) form.value.legalEntityId = office.legalEntityId
})

function goBack() {
  router.push('/employees')
}

async function onGeneratePassword() {
  if (!canCreate.value) return;
  const p = await employeeStore.generatePassword(12);
  if (p) {
    form.value.password = p;
    passwordConfirm.value = p
  }
}

async function onSave() {
  if (!canCreate.value) return
  const {valid} = await formRef.value.validate()
  if (!valid) return
  saving.value = true
  try {
    await employeeStore.createEmployee({
      ...form.value,
      officeId: Number(form.value.officeId),
      legalEntityId: Number(form.value.legalEntityId),
      roleIds: form.value.roleIds.map(Number),
      salaryAmount: form.value.salaryAmount != null ? Number(form.value.salaryAmount) : null,
      birthDate: form.value.birthDate || null,
      hireDate: form.value.hireDate || null
    })
    skipLeaveGuard.value = true
    await router.push('/employees')
  } finally {
    saving.value = false
  }
}
</script>

<style scoped>
.form-toolbar {
  background: #fff;
  border-bottom: 1px solid #e0e0e0;
  padding: 0 16px
}

.form-tabs {
  background: #fff;
  padding: 0 16px
}

.tab-item {
  font-weight: 600;
  color: #283106;
  text-transform: none;
  letter-spacing: normal
}

.form-fields {
  background: #fff;
  padding: 16px;
  border-radius: 8px
}

.form-actions {
  display: flex;
  gap: 16px;
  padding: 16px 0
}

.save-btn {
  border-radius: 14px !important;
  background: linear-gradient(135deg, var(--color-pear) 0%, #d4e2a8 100%) !important;
  color: var(--brand-ink) !important;
  font-weight: 600;
  text-transform: none;
  letter-spacing: 0.3px;
  padding: 0 24px !important;
  height: 44px !important;
  transition: all 0.3s ease !important;
  box-shadow: 0 4px 15px rgba(206, 219, 149, 0.3) !important;
  border: 1px solid rgba(206, 219, 149, 0.3) !important;
}

.cancel-btn {
  color: #808080 !important;
  text-transform: none;
  letter-spacing: normal;
  height: 40px !important
}

.full-page-bg {
  background: #F2F3ED;
  min-height: 100vh;
  font-family: 'Manrope', system-ui, sans-serif
}

.main-layout {
  display: flex;
  max-width: 1440px;
  margin: 0 auto;
  padding: 24px;
  gap: 24px
}

.content {
  flex: 1;
  background: #fff;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, .05);
  display: flex;
  flex-direction: column
}

.text-red {
  color: #ff5252
}

.form-scroll {
  flex: 1;
  overflow-y: auto;
  max-height: calc(100vh - 300px)
}

.row-3 :deep(.v-field) {
  min-height: 44px
}

.with-tip {
  position: relative;
  display: block
}

.gen-wrap {
  display: flex;
  align-items: center;
  gap: 8px
}

.gen-btn {
  width: 36px;
  height: 36px;
  min-width: 36px
}

.gen-text {
  color: #808080;
  font-size: .9rem
}
</style>
