import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from '@/app/router'
import vuetify from '@/app/plugins/vuetify'
import permissionPlugin from '@/shared/services/permissionPlugin'
import { useSessionStore } from '@/app/store/sessionStore'

import './styles/tokens.css'
import '@mdi/font/css/materialdesignicons.css'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(vuetify)
app.use(router)
app.use(permissionPlugin)

// Не блокируем рендер: профиль подгружается в фоне
const session = useSessionStore(pinia)
session.fetchUser().catch(() => undefined)

router.isReady().then(() => {
    app.mount('#app')
})
