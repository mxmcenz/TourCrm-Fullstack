<template>
  <v-main>
    <div class="full-page-bg">
      <v-container class="d-flex flex-column align-center pt-10 max-w-1200">
        <h1 class="text-h4 font-weight-medium mb-6">Регистрация</h1>

        <v-card class="pa-6 w-100" max-width="420" elevation="8" rounded="lg">
          <v-form ref="form" v-model="valid" autocomplete="off">
            <v-text-field
                v-model="email"
                :rules="emailRules"
                label="Email"
                placeholder="user@example.com"
                variant="outlined"
                bg-color="white"
                color="primary"
                density="comfortable"
                hide-details="auto"
                class="mb-3"
                :class="{ 'is-warning': emailTaken }"
                @input="emailTaken = false"
                required
            />
            <v-alert
                v-if="emailTaken"
                type="warning"
                variant="tonal"
                class="mt-n2 mb-3"
                density="comfortable"
                border="start"
            >
              Этот Email уже зарегистрирован.
              <RouterLink to="/login" class="link-brand">Войти</RouterLink>
              или
              <RouterLink to="/restore-pass" class="link-brand">восстановить пароль</RouterLink>.
            </v-alert>

            <v-text-field
                v-model="phone"
                :rules="phoneRules"
                label="Номер телефона"
                placeholder="+7 777 123 45 67"
                variant="outlined"
                bg-color="white"
                color="primary"
                density="comfortable"
                hide-details="auto"
                class="mb-1"
                required
            />
            <v-alert
                v-if="phoneInvalid"
                type="warning"
                variant="tonal"
                class="mt-1 mb-3"
                density="comfortable"
                border="start"
            >
              Номер выглядит неверно. Проверьте формат: +7 777 123 45 67
            </v-alert>

            <v-text-field
                v-model="firstName"
                :rules="nameRules"
                label="Имя"
                placeholder="Введите имя"
                variant="outlined"
                bg-color="white"
                color="primary"
                density="comfortable"
                hide-details="auto"
                class="mb-3"
                required
            />

            <v-alert
                v-if="error && !emailTaken"
                type="error"
                class="mt-2"
                dense
                border="start"
                border-color="red"
            >
              {{ error }}
            </v-alert>

            <v-btn
                class="pill btn-pear mt-4"
                variant="flat"
                :ripple="false"
                :disabled="!valid || loading || phoneInvalid"
                @click="onSubmit"
                height="52"
                block
            >
              <span v-if="!loading">Отправить код на почту</span>
              <v-progress-circular v-else indeterminate size="20" color="white" />
            </v-btn>
          </v-form>
        </v-card>
      </v-container>
    </div>
  </v-main>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/shared/services/api'

const router = useRouter()

const email = ref('')
const phone = ref('')
const firstName = ref('')
const valid = ref(false)
const form = ref(null)
const error = ref('')
const loading = ref(false)
const emailTaken = ref(false)

const emailRules = [
  v => !!v || 'Поле не может быть пустым',
  v => /.+@.+\..+/.test(v) || 'Неверный email',
]

const phoneRules = [
  v => !!v || 'Поле не может быть пустым',
  v =>
      !v ||
      /^\+?7[\s-]?\(?\d{3}\)?[\s-]?\d{3}[\s-]?\d{2}[\s-]?\d{2}$/.test(String(v).trim()) ||
      'Неверный номер',
]

const nameRules = [
  v => !!v || 'Поле не может быть пустым',
  v => (v?.length ?? 0) >= 2 || 'Минимум 2 символа',
]

const phoneInvalid = computed(() => {
  const v = (phone.value || '').trim()
  if (!v) return false
  return !/^\+?7[\s-]?\(?\d{3}\)?[\s-]?\d{3}[\s-]?\d{2}[\s-]?\d{2}$/.test(v)
})

async function onSubmit () {
  error.value = ''
  emailTaken.value = false

  const result = await form.value?.validate()
  if (!result?.valid || phoneInvalid.value) return

  loading.value = true
  try {
    const payload = {
      email: email.value.trim(),
      phoneNumber: phone.value.trim(),
      firstName: firstName.value.trim(),
    }

    const res = await api.post('/auth/start-registration', payload)
    localStorage.setItem('registerEmail', email.value)
    localStorage.setItem('registerUserId', String(res?.data?.data?.userId))
    await router.push('/register/verify')
  } catch (err) {
    const msg =
        err?.response?.data?.message ||
        err?.response?.data?.Message ||
        err?.message ||
        'Ошибка регистрации'

    if (/уже зарегистрирован/i.test(msg)) {
      emailTaken.value = true
      error.value = ''
    } else {
      error.value = msg
    }
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
.full-page-bg::before{
  content:''; position:absolute; inset:0;
  background:
      radial-gradient(circle at 20% 80%, rgba(206,219,149,.1) 0%, transparent 50%),
      radial-gradient(circle at 80% 20%, rgba(139,146,109,.08) 0%, transparent 50%),
      radial-gradient(circle at 40% 40%, rgba(242,243,237,.15) 0%, transparent 50%);
  pointer-events:none;
}
.max-w-1200{ max-width:1200px; width:100%; margin:0 auto; position:relative; z-index:1; }

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

.btn-pear{ background: var(--color-pear) !important; color:#000 !important; border-radius: var(--radius-pill) !important; }

:deep(.v-alert){
  border-radius: 12px;
  border: 1px solid #f5c2c7 !important;
  background: #fdecea !important;
  color: #7f1d1d !important;
  backdrop-filter: none !important;
  font-size: 15px;
}

@media (max-width: 960px){
  :deep(.v-card){ padding:24px !important; border-radius:20px !important; max-width:400px !important; }
}
@media (max-width: 600px){
  :deep(.v-card){ padding:20px !important; border-radius:16px !important; max-width:100% !important; margin:0 12px; }
  :deep(.v-field){ border-radius:12px !important; margin-bottom:16px; }
  .pill{ height:48px !important; font-size:15px; border-radius:14px !important; }
}

:deep(.v-text-field){ animation: fadeIn .5s ease-out; }
:deep(.v-text-field:nth-child(1)){ animation-delay:.1s; }
:deep(.v-text-field:nth-child(2)){ animation-delay:.2s; }
@keyframes fadeIn{
  from{ opacity:0; transform:translateX(-20px); }
  to{ opacity:1; transform:translateX(0); }
}

.full-page-bg::-webkit-scrollbar{ width:8px; }
.full-page-bg::-webkit-scrollbar-track{ background: rgba(139,146,109,.1); border-radius:4px; }
.full-page-bg::-webkit-scrollbar-thumb{ background: rgba(139,146,109,.3); border-radius:4px; }
.full-page-bg::-webkit-scrollbar-thumb:hover{ background: rgba(139,146,109,.5); }
</style>