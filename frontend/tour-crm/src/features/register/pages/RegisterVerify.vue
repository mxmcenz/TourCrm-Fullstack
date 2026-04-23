<template>
  <v-main>
    <div class="full-page-bg">
      <v-container class="d-flex flex-column align-center pt-10" fluid>
        <v-row justify="center" class="w-100">
          <v-col cols="12" md="5" lg="4">
            <h1 class="text-h4 font-weight-medium mb-6">Подтвердить email</h1>
            <p class="text-center text-body-2 mb-6 text-ink">
              Вам направлено письмо с кодом на почту <b>{{ email }}</b>
            </p>

            <v-card class="pa-6" elevation="8" rounded="lg">
              <v-form ref="form" v-model="valid" autocomplete="off">
                <v-text-field
                    v-model="code"
                    label="Код из письма"
                    placeholder="Введите код из письма"
                    variant="outlined"
                    density="comfortable"
                    inputmode="numeric"
                    pattern="\\d*"
                    maxlength="6"
                    :rules="[rules.required, rules.code]"
                    bg-color="white"
                    color="primary"
                    class="mb-4"
                />

                <v-btn
                    class="pill btn-pear"
                    variant="flat"
                    :ripple="false"
                    :disabled="loading || !valid"
                    @click="submitForm"
                    height="52"
                    block
                >
                  <span v-if="!loading">Подтвердить</span>
                  <v-progress-circular v-else indeterminate size="20" color="white"/>
                </v-btn>

                <v-btn
                    variant="text"
                    class="mt-3 text-none"
                    :disabled="resendLoading || resendTimer > 0"
                    @click="resendCode"
                    block
                >
                  <template v-if="resendTimer > 0">
                    Отправить код повторно ({{ resendTimer }} сек)
                  </template>
                  <template v-else-if="!resendLoading">
                    Отправить код повторно
                  </template>
                  <v-progress-circular v-else indeterminate size="20" color="primary"/>
                </v-btn>

                <v-alert
                    v-if="infoMessage"
                    type="info"
                    class="mt-4"
                    density="compact"
                    border="start"
                >
                  {{ infoMessage }}
                </v-alert>

                <v-alert
                    v-if="error"
                    type="error"
                    class="mt-3"
                    density="compact"
                    border="start"
                >
                  {{ error }}
                </v-alert>
              </v-form>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </div>
  </v-main>
</template>

<script setup>
import {ref, onMounted, onBeforeUnmount} from 'vue'
import {useRouter} from 'vue-router'
import {rules} from '@/shared/utils/rules'
import api from '@/shared/services/api'

const router = useRouter()
const form = ref(null)
const valid = ref(false)
const code = ref('')
const email = ref(localStorage.getItem('registerEmail') || '')
const infoMessage = ref(email.value ? `Код подтверждения отправлен на ${email.value}` : '')
const error = ref('')
const loading = ref(false)
const resendLoading = ref(false)

const resendTimer = ref(0)
let timerInterval = null

function startTimer(seconds = 30) {
  resendTimer.value = seconds
  clearInterval(timerInterval)
  timerInterval = setInterval(() => {
    resendTimer.value--
    if (resendTimer.value <= 0) clearInterval(timerInterval)
  }, 1000)
}

onBeforeUnmount(() => clearInterval(timerInterval))

async function submitForm() {
  error.value = ''
  const result = await form.value?.validate()
  if (!result?.valid) return
  loading.value = true
  try {
    const userId = parseInt(localStorage.getItem('registerUserId') || '0', 10)
    if (!userId) throw new Error('Идентификатор пользователя не найден')
    await api.post('/auth/verify-email', {userId, code: code.value})
    router.push('/set-password')
  } catch (err) {
    error.value = err.response?.data?.message || err.message || 'Неверный код'
  } finally {
    loading.value = false
  }
}

async function resendCode() {
  resendLoading.value = true
  error.value = ''
  infoMessage.value = ''
  try {
    await api.post('/auth/resend-code', {email: email.value})
    infoMessage.value = `Новый код отправлен на ${email.value}`
    startTimer(30)
  } catch (err) {
    error.value = err.response?.data?.message || 'Ошибка при повторной отправке'
  } finally {
    resendLoading.value = false
  }
}

onMounted(() => startTimer(30))
</script>

<style scoped>
.full-page-bg{
  min-height:100vh; width:100%;
  display:flex; flex-direction:column;
  background: linear-gradient(135deg, var(--color-baby-powder) 0%, rgba(242,243,237,0.3) 100%);
  position:relative; overflow:hidden;
}
.full-page-bg::before{
  content:''; position:absolute; inset:0;
  background:
      radial-gradient(circle at 20% 80%, rgba(206,219,149,.1) 0%, transparent 50%),
      radial-gradient(circle at 80% 20%, rgba(139,146,109,.08) 0%, transparent 50%),
      radial-gradient(circle at 40% 40%, rgba(242,243,237,.15) 0%, transparent 50%);
  pointer-events:none;
}
.max-w-1200{ width:100%; margin:0 auto; position:relative; z-index:1; }

h1{
  font-size:32px; font-weight:800;
  background: black;
  -webkit-background-clip:text; -webkit-text-fill-color:transparent; background-clip:text;
  margin-bottom:8px; letter-spacing:-.5px; text-align:center;
}

:deep(.v-card){
  background: linear-gradient(135deg, rgba(255,255,255,.95) 0%, rgba(242,243,237,.8) 100%) !important;
  backdrop-filter: blur(20px);
  border: 1px solid rgba(139,146,109,.15) !important;
  border-radius: 24px !important;
  box-shadow: 0 20px 40px rgba(0,0,0,.1), 0 8px 30px rgba(139,146,109,.15) !important;
  position: relative; overflow: hidden; padding: 32px !important;
  width: 500px !important;
  margin: 0 auto;
}
:deep(.v-card::before){
  content:''; position:absolute; top:0; left:0; right:0; height:4px;
  background: linear-gradient(90deg, var(--color-pear), var(--brand-primary));
  border-radius: 24px 24px 0 0;
}

:deep(.v-form){ animation: slideUp .6s ease-out; }
@keyframes slideUp{
  from{ opacity:0; transform:translateY(30px); }
  to{ opacity:1; transform:translateY(0); }
}

:deep(.v-field){
  border-radius:14px !important;
  background: rgba(255,255,255,.95) !important;
  border:1.5px solid var(--color-pear) !important;
  transition: border-color .25s ease, box-shadow .25s ease, background-color .25s ease;
  margin-bottom:20px;
}
:deep(.v-field:focus-within){
  border-color: var(--brand-primary) !important;
  box-shadow: 0 4px 20px rgba(139,146,109,.2), 0 0 0 4px rgba(139,146,109,.1) !important;
}

:deep(.v-field-label), :deep(.v-label){
  transform: translateY(0) scale(1);
  transition: transform .18s cubic-bezier(.2,0,0,1), color .18s ease, opacity .18s ease;
  transform-origin: left top; will-change: transform;
}
:deep(.v-field--focused .v-field-label),
:deep(.v-field--active  .v-field-label),
:deep(.v-field--focused .v-label),
:deep(.v-field--active  .v-label){
  transform: translateY(-10px) scale(.9);
  color: var(--brand-primary) !important;
  opacity:1;
}
:deep(.v-field__outline){ display:none; }
:deep(.v-messages__message){ color:#d32f2f; font-size:12px; font-weight:500; margin-top:4px; }

.pill{
  border-radius:16px !important;
  background: linear-gradient(135deg, var(--color-pear) 0%, #d4e2a8 100%) !important;
  color: var(--brand-ink) !important;
  font-weight:600; text-transform:none; letter-spacing:.3px;
  height:52px !important; font-size:16px;
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

:deep(.v-alert[type="error"]),
:deep(.v-alert.v-alert--variant-tonal.v-theme--light.error){
  border-radius:12px;
  border:1px solid #f5c2c7 !important;
  background:#fdecea !important;
  color:#7f1d1d !important;
  backdrop-filter:none !important;
}
:deep(.v-alert[type="info"]){
  border-radius:12px;
  border:1px solid #b6e0fe !important;
  background:#e6f2ff !important;
  color:#093a66 !important;
}

:deep(.v-text-field){ animation: fadeIn .5s ease-out; }
@keyframes fadeIn{
  from{ opacity:0; transform:translateX(-20px); }
  to{ opacity:1; transform:translateX(0); }
}

.full-page-bg::-webkit-scrollbar{ width:8px; }
.full-page-bg::-webkit-scrollbar-track{ background: rgba(139,146,109,.1); border-radius:4px; }
.full-page-bg::-webkit-scrollbar-thumb{ background: rgba(139,146,109,.3); border-radius:4px; }
.full-page-bg::-webkit-scrollbar-thumb:hover{ background: rgba(139,146,109,.5); }

@media (max-width: 960px){
  :deep(.v-card){ padding:24px !important; border-radius:20px !important; max-width: 480px !important; }
}
@media (max-width: 600px){
  :deep(.v-card){ padding:20px !important; border-radius:16px !important; max-width: 100% !important; margin: 0 12px; }
  :deep(.v-field){ border-radius:12px !important; margin-bottom:16px; }
  .pill{ height:48px !important; font-size:15px; border-radius:14px !important; }
}
</style>