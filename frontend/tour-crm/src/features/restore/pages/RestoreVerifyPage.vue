<template>
  <div class="full-page-bg">
    <v-container class="pt-10" style="max-width: 1200px; margin: 0 auto; min-height: 80vh;">
      <h1 class="text-h4 font-weight-medium mb-2 text-center">Подтверждение кода</h1>
      <p class="text-subtitle-1 mb-6 text-center">Код отправлен на <strong>{{ email }}</strong></p>

      <v-row justify="center">
        <v-col cols="12" md="6">
          <v-card class="pa-6" elevation="8" rounded="lg">
            <v-form ref="form" v-model="valid" autocomplete="off">
              <v-text-field
                  v-model="code"
                  label="Код подтверждения"
                  placeholder="Введите код из письма"
                  variant="outlined"
                  bg-color="white"
                  color="primary"
                  :rules="[rules.required, rules.code]"
                  maxlength="6"
                  class="mb-4"
              />
              <v-btn
                  class="pill btn-pear mt-2"
                  variant="flat"
                  block
                  :disabled="loading || !valid"
                  @click="submitForm"
              >
                <span v-if="!loading">Подтвердить</span>
                <v-progress-circular v-else indeterminate size="20" color="white"/>
              </v-btn>

              <v-btn
                  variant="text"
                  class="mt-4"
                  block
                  :disabled="resendLoading || resendTimer > 0"
                  @click="resendCode"
              >
                <template v-if="resendTimer > 0">
                  Отправить код повторно ({{ resendTimer }} сек)
                </template>
                <template v-else-if="!resendLoading">
                  Отправить код повторно
                </template>
                <v-progress-circular
                    v-else
                    indeterminate
                    size="20"
                    color="primary"
                ></v-progress-circular>
              </v-btn>

              <v-alert
                  v-if="infoMessage"
                  type="info"
                  class="mt-4"
                  dense
                  border="start"
                  border-color="blue"
              >
                {{ infoMessage }}
              </v-alert>
              <v-alert v-if="error" type="error" class="mt-4" dense border="start" border-color="red">
                {{ error }}
              </v-alert>
            </v-form>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script setup>
import {ref, onMounted} from 'vue'
import {useRouter} from 'vue-router'
import api from '@/shared/services/api'

const router = useRouter()
const form = ref(null)
const valid = ref(false)
const code = ref('')
const email = ref(localStorage.getItem('restoreEmail') || '')
const error = ref('')
const loading = ref(false)

const resendLoading = ref(false)
const infoMessage = ref('')
const resendTimer = ref(0)
let timerInterval = null

const rules = {
  required: v => !!v || 'Поле обязательно',
  code: v => /^\d{6}$/.test(v) || 'Код должен состоять из 6 цифр'
}

async function submitForm() {
  error.value = ''
  loading.value = true
  const result = await form.value?.validate()
  if (!result.valid) {
    loading.value = false
    return
  }

  try {
    const userId = parseInt(localStorage.getItem('restoreUserId'))
    if (!userId) throw new Error('Идентификатор пользователя не найден')

    await api.post('/auth/verify-reset-code', {
      userId,
      code: code.value
    })

    router.push('/restore/set-password')
  } catch (err) {
    error.value = err.response?.data?.Message || 'Неверный код'
  } finally {
    loading.value = false
  }
}

async function resendCode() {
  resendLoading.value = true
  error.value = ''
  infoMessage.value = ''

  try {
    await api.post('/auth/resend-reset-code', {email: email.value})
    infoMessage.value = `Новый код отправлен на ${email.value}`
    startTimer()
  } catch (err) {
    error.value = err.response?.data?.Message || 'Ошибка при повторной отправке'
  } finally {
    resendLoading.value = false
  }
}

function startTimer() {
  resendTimer.value = 60
  clearInterval(timerInterval)
  timerInterval = setInterval(() => {
    resendTimer.value--
    if (resendTimer.value <= 0) {
      clearInterval(timerInterval)
    }
  }, 1000)
}

onMounted(() => {
  startTimer()
})
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

:deep(.v-card) {
  background: linear-gradient(135deg, rgba(255,255,255,0.95) 0%, rgba(242,243,237,0.8) 100%) !important;
  backdrop-filter: blur(20px);
  border: 1px solid rgba(139,146,109,0.15) !important;
  border-radius: 24px !important;
  box-shadow: 0 20px 40px rgba(0,0,0,0.1), 0 8px 30px rgba(139,146,109,0.15) !important;
  position: relative;
  overflow: hidden;
  padding: 32px !important;
}
:deep(.v-card::before) {
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

:deep(.v-field__outline) { display: none; }
:deep(.v-messages__message) { color: #d32f2f; font-size: 12px; font-weight: 500; margin-top: 4px; }

.pill {
  border-radius: 16px !important;
  background: linear-gradient(135deg, var(--color-pear) 0%, #d4e2a8 100%) !important;
  color: var(--brand-ink) !important;
  font-weight: 600;
  text-transform: none;
  letter-spacing: .3px;
  height: 52px !important;
  font-size: 16px;
  transition: all .4s cubic-bezier(.4,0,.2,1) !important;
  box-shadow: 0 8px 25px rgba(206,219,149,.4), 0 4px 15px rgba(206,219,149,.3) !important;
  position: relative; overflow: hidden;
}
.pill::before {
  content: ''; position: absolute; top: 0; left: -100%; width: 100%; height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,.4), transparent);
  transition: left .6s ease;
}
.pill:hover::before { left: 100%; }
.pill:hover { transform: translateY(-3px) scale(1.02); box-shadow: 0 15px 35px rgba(206,219,149,.6), 0 8px 25px rgba(206,219,149,.4) !important; }
.pill:active { transform: translateY(-1px) scale(1.01); }
.pill:disabled { opacity: .6; transform: none !important; box-shadow: 0 2px 8px rgba(206,219,149,.2) !important; }

:deep(.v-alert) {
  border-radius: 12px;
  border: 1px solid #f5c2c7 !important;
  background: #fdecea !important;
  color: #7f1d1d !important;
  backdrop-filter: none !important;
  margin-top: 20px;
  padding: 16px;
}

@media (max-width: 960px) {
  h1 { font-size: 28px; }
  :deep(.v-card) { padding: 24px !important; border-radius: 20px !important; max-width: 420px !important; margin: 0 auto; }
}
@media (max-width: 600px) {
  h1 { font-size: 24px; margin-bottom: 16px; }
  :deep(.v-card) { padding: 20px !important; border-radius: 16px !important; max-width: 100% !important; margin: 0 12px; }
  :deep(.v-field) { border-radius: 12px !important; margin-bottom: 16px; }
  .pill { height: 48px !important; font-size: 15px; border-radius: 14px !important; }
}

:deep(.v-text-field) { animation: fadeIn .5s ease-out; }
@keyframes fadeIn {
  from { opacity: 0; transform: translateX(-20px); }
  to   { opacity: 1; transform: translateX(0); }
}

.full-page-bg::-webkit-scrollbar { width: 8px; }
.full-page-bg::-webkit-scrollbar-track { background: rgba(139,146,109,0.1); border-radius: 4px; }
.full-page-bg::-webkit-scrollbar-thumb { background: rgba(139,146,109,0.3); border-radius: 4px; }
.full-page-bg::-webkit-scrollbar-thumb:hover { background: rgba(139,146,109,0.5); }

.btn-pear{
  background: var(--color-pear) !important;
  color: #000 !important;
  border-radius: var(--radius-pill) !important;
}
.btn-pear:hover{ filter: brightness(0.96); }
.btn-pear:active,
.btn-pear.v-btn--active{
  background: var(--brand-primary) !important;
  color: #fff !important;
}
</style>