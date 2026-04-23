<template>
  <v-app>
    <v-main class="set-password-page">
      <v-container class="d-flex align-center justify-center mt-16" fluid>
        <v-row justify="center">
          <v-col cols="12" md="4">
            <h1 class="text-h4 text-center mb-6 font-weight-medium">Установить пароль</h1>
            <div class="form-box">
              <v-form ref="form" v-model="valid">
                <v-text-field v-model="password" label="Новый пароль" type="password"
                              :rules="[rules.required, rules.length]" class="custom-text-field mb-4"/>
                <v-text-field v-model="confirmPassword" label="Повторите пароль" type="password"
                              :rules="[rules.required, matchPassword]" class="custom-text-field"/>
                <div class="text-center mt-4">
                  <v-btn class="pill btn-pear" @click="submit" :disabled="!valid">
                    <p>Сохранить пароль</p>
                  </v-btn>
                </div>
                <v-alert v-if="error" class="mt-4" color="error" variant="tonal" density="compact">{{ error }}</v-alert>
              </v-form>
            </div>
          </v-col>
        </v-row>
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup>
import {ref} from 'vue'
import {useRouter} from 'vue-router'
import api from '@/shared/services/api'
import {useSessionStore} from '@/app/store/sessionStore'
import {useCompanyStore} from '@/features/company/store/companyStore'

const router = useRouter()
const session = useSessionStore()
const company = useCompanyStore()

const valid = ref(false)
const form = ref(null)
const password = ref('')
const confirmPassword = ref('')
const error = ref('')

const rules = {
  required: v => !!v || 'Обязательное поле',
  length: v => (v?.length ?? 0) >= 6 || 'Минимум 6 символов'
}
const matchPassword = v => v === password.value || 'Пароли не совпадают'

async function submit() {
  error.value = ''
  const result = await form.value?.validate()
  if (!result?.valid) return
  try {
    const userId = Number(localStorage.getItem('restoreUserId'))
    if (!userId) throw new Error('Пользователь не найден')
    const res = await api.post('/auth/reset-password', {
      userId,
      password: password.value,
      confirmPassword: confirmPassword.value
    })
    const payload = res.data?.data ?? res.data
    session.login(payload)
    await session.fetchUser()
    await company.loadMyCompany()
    await router.replace({name: 'Company'})
  } catch (err) {
    error.value = err?.response?.data?.message || err?.response?.data?.Message || 'Ошибка установки пароля'
  }
}
</script>

<style scoped>
.set-password-page {
  min-height: 100vh;
  background: linear-gradient(135deg, var(--color-baby-powder) 0%, rgba(242, 243, 237, 0.3) 100%);
  position: relative;
  overflow: hidden;
}
.set-password-page::before {
  content: '';
  position: absolute;
  inset: 0;
  background:
      radial-gradient(circle at 20% 80%, rgba(206, 219, 149, 0.1) 0%, transparent 50%),
      radial-gradient(circle at 80% 20%, rgba(139, 146, 109, 0.08) 0%, transparent 50%),
      radial-gradient(circle at 40% 40%, rgba(242, 243, 237, 0.15) 0%, transparent 50%);
  pointer-events: none;
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

.form-box {
  background: linear-gradient(135deg, rgba(255,255,255,0.95) 0%, rgba(242,243,237,0.8) 100%);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(139,146,109,0.15);
  border-radius: 24px;
  box-shadow: 0 20px 40px rgba(0,0,0,0.1), 0 8px 30px rgba(139,146,109,0.15);
  position: relative;
  overflow: hidden;
  padding: 32px;
}
.form-box::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0; height: 4px;
  background: linear-gradient(90deg, var(--color-pear), var(--brand-primary));
  border-radius: 24px 24px 0 0;
}

:deep(.v-form) { animation: slideUp .6s ease-out; }
@keyframes slideUp {
  from { opacity: 0; transform: translateY(30px); }
  to   { opacity: 1; transform: translateY(0); }
}

:deep(.custom-text-field .v-field),
:deep(.v-field) {
  border-radius: 14px !important;
  background: rgba(255,255,255,0.95) !important;
  border: 1.5px solid var(--color-pear) !important;
  transition: border-color .25s ease, box-shadow .25s ease, background-color .25s ease;
  margin-bottom: 20px;
}
:deep(.v-field:focus-within) {
  border-color: var(--brand-primary) !important;
  box-shadow: 0 4px 20px rgba(139,146,109,0.2), 0 0 0 4px rgba(139,146,109,0.1) !important;
}

:deep(.v-field-label), :deep(.v-label) {
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
:deep(.v-field__outline){ display:none; }
:deep(.v-messages__message){ color:#d32f2f; font-size:12px; font-weight:500; margin-top:4px; }

.pill{
  border-radius:16px !important;
  background: linear-gradient(135deg, var(--color-pear) 0%, #d4e2a8 100%) !important;
  color: var(--brand-ink) !important;
  font-weight:600;
  text-transform:none;
  letter-spacing:.3px;
  height:52px !important;
  font-size:16px;
  transition: all .4s cubic-bezier(.4,0,.2,1) !important;
  box-shadow: 0 8px 25px rgba(206,219,149,.4), 0 4px 15px rgba(206,219,149,.3) !important;
  position:relative; overflow:hidden;
}
.pill::before{
  content:''; position:absolute; top:0; left:-100%; width:100%; height:100%;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,.4), transparent);
  transition:left .6s ease;
}
.pill:hover::before{ left:100%; }
.pill:hover{ transform: translateY(-3px) scale(1.02); box-shadow: 0 15px 35px rgba(206,219,149,.6), 0 8px 25px rgba(206,219,149,.4) !important; }
.pill:active{ transform: translateY(-1px) scale(1.01); }
.pill:disabled{ opacity:.6; transform:none !important; box-shadow:0 2px 8px rgba(206,219,149,.2) !important; }

:deep(.v-alert){
  border-radius:12px;
  border:1px solid #f5c2c7 !important;
  background:#fdecea !important;
  color:#7f1d1d !important;
  backdrop-filter:none !important;
  margin-top:20px;
  padding:16px;
}

@media (max-width: 960px){
  .form-box{ padding:24px; border-radius:20px; }
}
@media (max-width: 600px){
  .form-box{ padding:20px; border-radius:16px; }
  :deep(.v-field){ border-radius:12px !important; margin-bottom:16px; }
  .pill{ height:48px !important; font-size:15px; border-radius:14px !important; }
}

.btn-pear{
  background: var(--color-pear) !important;
  color:#000 !important;
  border-radius: var(--radius-pill) !important;
}
.btn-pear:hover{ filter: brightness(0.96); }
.btn-pear:active,
.btn-pear.v-btn--active{
  background: var(--brand-primary) !important;
  color:#fff !important;
}
</style>