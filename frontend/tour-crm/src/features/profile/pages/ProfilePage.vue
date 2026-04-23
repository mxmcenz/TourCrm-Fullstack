<template>
  <div class="profile-page" v-if="auth.user">
    <v-container class="page pt-6 pb-16">
      <div class="profile-header">
        <h1 class="text-h4 font-weight-bold">Личный кабинет</h1>
        <p class="text-subtitle-1 text-medium-emphasis">Управление вашими персональными данными</p>
      </div>

      <v-card class="profile-card" elevation="0">
        <v-form ref="form" v-model="valid" lazy-validation>
          <div class="form-grid">
            <div class="field-group">
              <v-text-field
                  v-model="formData.fullName"
                  :readonly="!editMode.fullName"
                  label="ФИО"
                  :rules="[rules.required]"
                  bg-color="white"
                  variant="outlined"
                  density="comfortable"
                  hide-details="auto"
              >
                <template #append-inner>
                  <v-btn
                      icon="mdi-pencil"
                      variant="text"
                      size="small"
                      @click="toggleEdit('fullName')"
                      class="edit-btn"
                      :color="editMode.fullName ? 'primary' : 'default'"
                  />
                </template>
              </v-text-field>
            </div>

            <div class="field-group">
              <v-text-field
                  v-model="formData.phoneNumber"
                  :readonly="!editMode.phoneNumber"
                  label="Телефон"
                  :rules="[rules.required, rules.phone]"
                  bg-color="white"
                  variant="outlined"
                  density="comfortable"
                  hide-details="auto"
              >
                <template #append-inner>
                  <v-btn
                      icon="mdi-pencil"
                      variant="text"
                      size="small"
                      @click="toggleEdit('phoneNumber')"
                      class="edit-btn"
                      :color="editMode.phoneNumber ? 'primary' : 'default'"
                  />
                </template>
              </v-text-field>
            </div>

            <div class="field-group">
              <v-text-field
                  v-model="formData.passport"
                  :readonly="!editMode.passport"
                  label="Паспорт"
                  :rules="[rules.required]"
                  bg-color="white"
                  variant="outlined"
                  density="comfortable"
                  hide-details="auto"
              >
                <template #append-inner>
                  <v-btn
                      icon="mdi-pencil"
                      variant="text"
                      size="small"
                      @click="toggleEdit('passport')"
                      class="edit-btn"
                      :color="editMode.passport ? 'primary' : 'default'"
                  />
                </template>
              </v-text-field>
            </div>

            <div class="field-group">
              <v-text-field
                  v-model="formData.email"
                  label="Email"
                  readonly
                  bg-color="white"
                  variant="outlined"
                  density="comfortable"
                  hide-details="auto"
              />
            </div>

            <div class="field-group full-width">
              <v-textarea
                  v-model="formData.travelPreferences"
                  :readonly="!editMode.travelPreferences"
                  label="Предпочтения к путешествиям"
                  rows="3"
                  bg-color="white"
                  variant="outlined"
                  density="comfortable"
                  hide-details="auto"
                  placeholder="Расскажите о ваших предпочтениях в путешествиях..."
              >
                <template #append-inner>
                  <v-btn
                      icon="mdi-pencil"
                      variant="text"
                      size="small"
                      @click="toggleEdit('travelPreferences')"
                      class="edit-btn"
                      :color="editMode.travelPreferences ? 'primary' : 'default'"
                  />
                </template>
              </v-textarea>
            </div>
          </div>

          <v-btn
              class="save-btn"
              color="primary"
              :disabled="loading || !valid"
              rounded
              size="large"
              @click="saveProfile"
          >
            <v-icon v-if="!loading" start>mdi-content-save</v-icon>
            <span v-if="!loading">Сохранить изменения</span>
            <v-progress-circular v-else indeterminate size="20" color="white" />
          </v-btn>
        </v-form>
      </v-card>
    </v-container>

    <v-snackbar v-model="snackbar" color="success" timeout="3000" location="bottom right">
      <v-icon start>mdi-check-circle</v-icon>
      Профиль успешно обновлён
    </v-snackbar>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import {useSessionStore} from '@/app/store/userStore'
import { rules } from '@/shared/utils/rules'

const router = useRouter()
const auth = useSessionStore()
const form = ref(null)
const valid = ref(false)
const loading = ref(false)
const snackbar = ref(false)

const formData = ref({
  fullName: '',
  phoneNumber: '',
  passport: '',
  email: '',
  travelPreferences: ''
})

const editMode = ref({
  fullName: false,
  phoneNumber: false,
  passport: false,
  travelPreferences: false
})

const toggleEdit = (field) => {
  editMode.value[field] = !editMode.value[field]
}

onMounted(async () => {
  await auth.fetchUser()

  if (!auth.user) {
    await router.push('/login')
    return
  }

  Object.assign(formData.value, auth.user)
})

const saveProfile = async () => {
  if (!(await form.value.validate())) return
  loading.value = true

  try {
    await auth.updateUser({
      fullName: formData.value.fullName,
      phoneNumber: formData.value.phoneNumber,
      passport: formData.value.passport,
      travelPreferences: formData.value.travelPreferences
    })
    snackbar.value = true

    Object.keys(editMode.value).forEach(key => {
      editMode.value[key] = false
    })

  } catch (e) {
    alert('Ошибка при обновлении профиля')
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.profile-page {
  background: linear-gradient(135deg, var(--color-baby-powder) 0%, rgba(242, 243, 237, 0.3) 100%);
  min-height: 100vh;
}

.page {
  width: 100%;
  padding-inline: 24px;
  box-sizing: border-box;
  max-width: 800px;
  margin: 0 auto;
}

.profile-header {
  text-align: center;
  margin-bottom: 32px;
  padding: 0 20px;
}

.profile-header h1 {
  font-size: 32px;
  font-weight: 800;
  background: linear-gradient(135deg, var(--brand-ink) 0%, var(--brand-primary) 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin-bottom: 8px;
  letter-spacing: -0.5px;
}

.profile-header p {
  font-size: 16px;
  color: var(--color-gray);
  opacity: 0.8;
}

.profile-card {
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.95) 0%, rgba(242, 243, 237, 0.8) 100%) !important;
  padding: 32px;
  border-radius: 20px;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(139, 146, 109, 0.15);
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.06);
  position: relative;
  overflow: hidden;
}

.profile-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 3px;
  background: linear-gradient(90deg, var(--color-pear), var(--brand-primary));
  border-radius: 20px 20px 0 0;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
  margin-bottom: 32px;
}

.field-group {
  display: flex;
  flex-direction: column;
}

.field-group.full-width {
  grid-column: 1 / -1;
}

:deep(.v-field) {
  border-radius: 12px;
  background: rgba(255, 255, 255, 0.95) !important;
  border: 1.5px solid var(--color-pear) !important;
  transition: all 0.3s ease;
}

:deep(.v-field:focus-within) {
  border-color: var(--brand-primary) !important;
  box-shadow: 0 4px 15px rgba(139, 146, 109, 0.2) !important;
  transform: translateY(-1px);
}

:deep(.v-field--readonly .v-field) {
  border-color: rgba(139, 146, 109, 0.2) !important;
  background: rgba(255, 255, 255, 0.8) !important;
}

:deep(.v-label) {
  color: var(--brand-ink);
  font-weight: 500;
  opacity: 0.8;
  font-size: 14px;
}

:deep(.v-field--focused .v-label) {
  color: var(--brand-primary);
  opacity: 1;
}

.edit-btn {
  margin-top: -4px;
  transition: all 0.3s ease;
}

.edit-btn:hover {
  transform: scale(1.1);
}

:deep(.v-field--readonly .edit-btn) {
  opacity: 0.7;
}

:deep(.v-field--readonly .edit-btn:hover) {
  opacity: 1;
}

.save-btn {
  border-radius: 14px !important;
  background: linear-gradient(135deg, var(--color-pear) 0%, #d4e2a8 100%) !important;
  color: var(--brand-ink) !important;
  font-weight: 600;
  text-transform: none;
  letter-spacing: 0.3px;
  height: 48px !important;
  font-size: 16px;
  transition: all 0.3s ease !important;
  box-shadow: 0 4px 15px rgba(206, 219, 149, 0.3) !important;
  border: 1px solid rgba(206, 219, 149, 0.3) !important;
}

.save-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(206, 219, 149, 0.5) !important;
}

.save-btn:disabled {
  opacity: 0.5;
  transform: none !important;
  box-shadow: 0 2px 8px rgba(206, 219, 149, 0.2) !important;
}

:deep(.v-snackbar__wrapper) {
  border-radius: 12px;
}

/* Адаптивность */
@media (max-width: 960px) {
  .page {
    padding-inline: 16px;
  }

  .profile-card {
    padding: 24px;
    border-radius: 16px;
  }

  .form-grid {
    grid-template-columns: 1fr;
    gap: 16px;
  }

  .profile-header h1 {
    font-size: 28px;
  }
}

@media (max-width: 600px) {
  .page {
    padding-inline: 12px;
  }

  .profile-card {
    padding: 20px 16px;
    border-radius: 12px;
  }

  .profile-header {
    padding: 0;
  }

  .profile-header h1 {
    font-size: 24px;
  }

  .profile-header p {
    font-size: 14px;
  }

  .save-btn {
    height: 44px !important;
    font-size: 14px;
  }
}

/* Анимации */
.profile-card {
  animation: slideUp 0.5s ease-out;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.field-group {
  animation: fadeIn 0.4s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateX(-10px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

.field-group:nth-child(even) {
  animation-delay: 0.1s;
}

.field-group:nth-child(3),
.field-group:nth-child(4) {
  animation-delay: 0.2s;
}

.field-group.full-width {
  animation-delay: 0.3s;
}
</style>