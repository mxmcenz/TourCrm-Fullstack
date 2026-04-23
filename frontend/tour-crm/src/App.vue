<template>
  <v-app>
    <AppHeader v-if="showChrome" />
    <AppSidebar v-if="showSidebar" />

    <v-main class="bg-background">
      <router-view />
    </v-main>

    <AppFooter v-if="showChrome" />
  </v-app>
</template>

<script setup>
import {computed} from 'vue'
import {useRoute} from 'vue-router'
import {useSessionStore} from '@/app/store/sessionStore'
import AppHeader from '@/shared/components/AppHeader.vue'
import AppFooter from '@/shared/components/AppFooter.vue'
import AppSidebar from '@/shared/components/AppSidebar.vue'

const auth = useSessionStore()
const route = useRoute()


const showChrome = computed(() => route.meta?.hideChrome !== true)

const showSidebar = computed(() =>
    !auth.isLoading && auth.isLoggedIn && route.meta?.sidebar !== false
)
</script>
