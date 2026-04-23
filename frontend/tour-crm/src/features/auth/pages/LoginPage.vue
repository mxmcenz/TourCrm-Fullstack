<template>
  <div class="full-page-bg">
    <v-container class="d-flex flex-column align-center pt-10 max-w-1200">
      <h1 class="text-h4 font-weight-medium mb-6">Авторизация</h1>
      <v-card class="pa-6 w-100" max-width="420" elevation="8" style="background-color: #e3f2fd;" rounded="lg">
        <v-form ref="form" v-model="valid" autocomplete="off">
          <v-text-field v-model="email" label="Email" placeholder="Введите ваш email" variant="outlined"
                        bg-color="white" color="primary" autocomplete="email" :rules="[rules.required, rules.email]"
                        class="mb-4"/>
          <v-text-field v-model="password" label="Пароль" placeholder="Введите ваш пароль" variant="outlined"
                        bg-color="white" color="primary" autocomplete="current-password" type="password"
                        :rules="[rules.required]"/>
          <v-btn class="pill btn-pear mt-5" variant="flat" :ripple="false" block :disabled="loading || !valid"
                 @click="login">
            <span v-if="!loading">Войти</span>
            <v-progress-circular v-else indeterminate size="20" color="white"/>
          </v-btn>
          <v-alert v-if="error" type="error" class="mt-4" dense border="start" border-color="red">{{ error }}</v-alert>
        </v-form>
        <div class="mt-4 text-center text-body-2">
          Нет аккаунта?
          <RouterLink to="/register/step1" class="link-brand">Зарегистрироваться</RouterLink>
          <br/>
          Забыли пароль?
          <RouterLink to="/restore-pass" class="link-brand">Восстановить</RouterLink>
        </div>
      </v-card>
    </v-container>
  </div>
</template>

<script setup>
import {ref} from 'vue'
import {useRouter} from 'vue-router'
import api from '@/shared/services/api'
import {rules} from '@/shared/utils/rules'
import {useSessionStore} from '@/app/store/sessionStore'
import {useCompanyStore} from '@/features/company/store/companyStore'

const email = ref('')
const password = ref('')
const valid = ref(false)
const error = ref('')
const loading = ref(false)
const form = ref(null)

const router = useRouter()
const session = useSessionStore()
const company = useCompanyStore()

async function login() {
  error.value = ''
  const result = await form.value?.validate()
  if (!result?.valid) return
  loading.value = true
  try {
    const res = await api.post('/auth/login', {email: email.value, password: password.value})
    const payload = res.data?.data ?? res.data
    session.login(payload)
    await session.fetchUser()
    await company.loadMyCompany()
    await router.replace({name: 'Company'})
  } catch (err) {
    error.value = err.response?.data?.message || 'Ошибка авторизации'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.full-page-bg {
  min-height: 100vh;
  width: 100%;
  display: flex;
  flex-direction: column;
  background: linear-gradient(135deg, var(--color-baby-powder) 0%, rgba(242, 243, 237, 0.3) 100%);
  position: relative;
  overflow: hidden;
}

.full-page-bg::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background:
      radial-gradient(circle at 20% 80%, rgba(206, 219, 149, 0.1) 0%, transparent 50%),
      radial-gradient(circle at 80% 20%, rgba(139, 146, 109, 0.08) 0%, transparent 50%),
      radial-gradient(circle at 40% 40%, rgba(242, 243, 237, 0.15) 0%, transparent 50%);
  pointer-events: none;
}

.max-w-1200 {
  max-width: 1200px;
  width: 100%;
  margin: 0 auto;
  position: relative;
  z-index: 1;
}

h1 {
  font-size: 32px;
  font-weight: 800;
  background: black;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin-bottom: 8px;
  letter-spacing: -0.5px;
  text-align: center;
}

:deep(.v-card) {
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.95) 0%, rgba(242, 243, 237, 0.8) 100%) !important;
  backdrop-filter: blur(20px);
  border: 1px solid rgba(139, 146, 109, 0.15) !important;
  border-radius: 24px !important;
  box-shadow:
      0 20px 40px rgba(0, 0, 0, 0.1),
      0 8px 30px rgba(139, 146, 109, 0.15) !important;
  position: relative;
  overflow: hidden;
  padding: 32px !important;
}

:deep(.v-card::before) {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, var(--color-pear), var(--brand-primary));
  border-radius: 24px 24px 0 0;
}

:deep(.v-form) {
  animation: slideUp 0.6s ease-out;
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

:deep(.v-field) {
  border-radius: 14px !important;
  background: rgba(255, 255, 255, 0.95) !important;
  border: 1.5px solid var(--color-pear) !important;
  transition: border-color .25s ease, box-shadow .25s ease, background-color .25s ease;
  margin-bottom: 20px;
}

:deep(.v-field:focus-within) {
  border-color: var(--brand-primary) !important;
  box-shadow:
      0 4px 20px rgba(139, 146, 109, 0.2),
      0 0 0 4px rgba(139, 146, 109, 0.1) !important;
}

:deep(.v-field-label),
:deep(.v-label) {
  transform: translateY(0) scale(1);
  transition: transform .18s cubic-bezier(.2,0,0,1), color .18s ease, opacity .18s ease;
  transform-origin: left top;
  will-change: transform;
}

:deep(.v-field--focused .v-field-label),
:deep(.v-field--active  .v-field-label),
:deep(.v-field--focused .v-label),
:deep(.v-field--active  .v-label) {
  transform: translateY(-10px) scale(.9);
  color: var(--brand-primary) !important;
  opacity: 1;
}

:deep(.v-field__outline) {
  display: none;
}

:deep(.v-messages__message) {
  color: #d32f2f;
  font-size: 12px;
  font-weight: 500;
  margin-top: 4px;
}

.pill {
  border-radius: 16px !important;
  background: linear-gradient(135deg, var(--color-pear) 0%, #d4e2a8 100%) !important;
  color: var(--brand-ink) !important;
  font-weight: 600;
  text-transform: none;
  letter-spacing: 0.3px;
  height: 52px !important;
  font-size: 16px;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1) !important;
  box-shadow:
      0 8px 25px rgba(206, 219, 149, 0.4),
      0 4px 15px rgba(206, 219, 149, 0.3) !important;
  position: relative;
  overflow: hidden;
}

.pill::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.4), transparent);
  transition: left 0.6s ease;
}

.pill:hover::before {
  left: 100%;
}

.pill:hover {
  transform: translateY(-3px) scale(1.02);
  box-shadow:
      0 15px 35px rgba(206, 219, 149, 0.6),
      0 8px 25px rgba(206, 219, 149, 0.4) !important;
}

.pill:active {
  transform: translateY(-1px) scale(1.01);
}

.pill:disabled {
  opacity: 0.6;
  transform: none !important;
  box-shadow: 0 2px 8px rgba(206, 219, 149, 0.2) !important;
}

:deep(.v-alert) {
  border-radius: 12px;
  border: 1px solid #f5c2c7 !important;
  background: #fdecea !important;
  color: #7f1d1d !important;
  backdrop-filter: none !important;
  margin-top: 20px;
  padding: 16px;
  animation: shake 0.5s ease-in-out;
}

@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-5px); }
  75% { transform: translateX(5px); }
}

:deep(.v-alert__border) {
  opacity: 0.6;
}

.link-brand {
  color: var(--brand-primary) !important;
  text-decoration: none;
  font-weight: 600;
  transition: all 0.3s ease;
  position: relative;
  padding: 2px 4px;
  border-radius: 4px;
}

.link-brand::after {
  content: '';
  position: absolute;
  bottom: -2px;
  left: 0;
  width: 0;
  height: 2px;
  background: linear-gradient(90deg, var(--color-pear), var(--brand-primary));
  transition: width 0.3s ease;
}

.link-brand:hover::after {
  width: 100%;
}

.link-brand:hover {
  color: var(--brand-primary) !important;
  background: rgba(139, 146, 109, 0.08);
}

.text-center {
  color: var(--brand-ink);
  opacity: 0.8;
  font-size: 14px;
  line-height: 1.6;
}

:deep(.v-progress-circular) {
  color: var(--brand-ink) !important;
}

@media (max-width: 960px) {
  .max-w-1200 {
    padding-inline: 16px;
  }

  h1 {
    font-size: 28px;
  }

  :deep(.v-card) {
    padding: 24px !important;
    border-radius: 20px !important;
    max-width: 400px !important;
  }
}

@media (max-width: 600px) {
  .max-w-1200 {
    padding-inline: 12px;
  }

  h1 {
    font-size: 24px;
    margin-bottom: 16px;
  }

  :deep(.v-card) {
    padding: 20px !important;
    border-radius: 16px !important;
    max-width: 100% !important;
    margin: 0 12px;
  }

  :deep(.v-field) {
    border-radius: 12px !important;
    margin-bottom: 16px;
  }

  .pill {
    height: 48px !important;
    font-size: 15px;
    border-radius: 14px !important;
  }

  .text-center {
    font-size: 13px;
  }
}

:deep(.v-text-field) {
  animation: fadeIn 0.5s ease-out;
}

:deep(.v-text-field:nth-child(1)) { animation-delay: 0.1s; }
:deep(.v-text-field:nth-child(2)) { animation-delay: 0.2s; }

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateX(-20px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

.full-page-bg::-webkit-scrollbar {
  width: 8px;
}

.full-page-bg::-webkit-scrollbar-track {
  background: rgba(139, 146, 109, 0.1);
  border-radius: 4px;
}

.full-page-bg::-webkit-scrollbar-thumb {
  background: rgba(139, 146, 109, 0.3);
  border-radius: 4px;
}

.full-page-bg::-webkit-scrollbar-thumb:hover {
  background: rgba(139, 146, 109, 0.5);
}
</style>