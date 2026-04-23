<template>
  <v-app-bar app flat color="white" height="72" class="app-header">
    <v-container class="appbar-inner d-flex align-center" style="max-width: 1430px">
      <div class="left d-flex align-center">
        <RouterLink to="/" class="brand">
          <div class="brand-wrapper">
            <span class="brand-text">SapARide</span>
          </div>
        </RouterLink>

        <nav class="nav-links">
          <RouterLink to="/tariffs" class="nav-link">
            <v-icon icon="mdi-crown" size="18" class="nav-icon"/>
            <span>Тарифы</span>
          </RouterLink>
        </nav>
      </div>

      <div class="actions d-flex align-center ml-auto">
        <template v-if="!auth.user">
          <v-btn
              class="login-btn"
              variant="text"
              to="/login"
              size="large"
          >
            <template #prepend>
              <v-icon icon="mdi-login" size="15"/>
            </template>
            Вход
          </v-btn>
          <v-btn
              class="register-btn"
              variant="flat"
              to="/register/step1"
              size="large"
          >
            <template #prepend>
              <v-icon icon="mdi-account-plus" size="18"/>
            </template>
            Начать бесплатно
          </v-btn>
        </template>
        <template v-else>
          <v-btn
              variant="text"
              class="profile-btn mr-2"
              to="/profile"
              size="large"
          >
            <template #prepend>
              <v-icon icon="mdi-account" size="18"/>
            </template>
            Личный кабинет
          </v-btn>
          <v-btn
              class="logout-btn"
              variant="outlined"
              @click="logout"
              size="large"
          >
            <template #prepend>
              <v-icon icon="mdi-logout" size="18"/>
            </template>
            Выход
          </v-btn>
        </template>
      </div>
    </v-container>

    <div class="header-decoration"></div>
  </v-app-bar>
</template>

<script setup>
import {useRouter} from 'vue-router'
import {useSessionStore} from '@/app/store/sessionStore'

const auth = useSessionStore()
const router = useRouter()
const logout = () => {
  auth.logout();
  router.push('/')
}
</script>

<style scoped>
.app-header {
  position: sticky;
  top: 0;
  z-index: 1000;
  border-bottom: 1px solid rgba(139, 146, 109, 0.1) !important;
  background: linear-gradient(135deg, var(--brand-paper) 0%, rgba(255, 255, 255, 0.98) 100%) !important;
  backdrop-filter: blur(10px);
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.04) !important;
  transition: all 0.3s ease;
}

.app-header:hover {
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.08) !important;
}

.appbar-inner {
  gap: 0;
}

.brand {
  text-decoration: none;
  margin-right: 48px;
}

.brand-wrapper {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 8px 0;
}

.brand:hover .brand-icon {
  transform: scale(1.05) rotate(-5deg);
  box-shadow: 0 8px 20px rgba(139, 146, 109, 0.3);
}

.brand-text {
  font-family: var(--ff-sans);
  font-weight: 800;
  font-size: 28px;
  background: black;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  letter-spacing: 0.5px;
}

.nav-links {
  display: flex;
  align-items: center;
  gap: 8px;
}

.nav-link {
  position: relative;
  display: flex;
  align-items: center;
  gap: 8px;
  text-decoration: none;
  color: var(--brand-ink);
  font-size: 15px;
  font-weight: 500;
  letter-spacing: 0.2px;
  opacity: 0.8;
  padding: 12px 16px;
  border-radius: 12px;
  transition: all 0.3s ease;
  margin: 0 4px;
}

.nav-icon {
  opacity: 0.7;
  transition: all 0.3s ease;
}

.nav-link:hover {
  color: var(--brand-primary);
  opacity: 1;
  background: rgba(139, 146, 109, 0.08);
  transform: translateY(-1px);
}

.nav-link:hover .nav-icon {
  opacity: 1;
  transform: scale(1.1);
}

.nav-link.router-link-exact-active {
  color: var(--brand-primary);
  opacity: 1;
  background: rgba(139, 146, 109, 0.12);
  font-weight: 600;
}

.nav-link.router-link-exact-active .nav-icon {
  opacity: 1;
}

.nav-link::after {
  content: "";
  position: absolute;
  left: 50%;
  bottom: 6px;
  width: 0;
  height: 2px;
  background: linear-gradient(90deg, var(--color-pear), var(--brand-primary));
  border-radius: 2px;
  transform: translateX(-50%);
  transition: width 0.3s ease;
}

.nav-link.router-link-exact-active::after {
  width: 20px;
}

.actions :deep(.v-btn) {
  border-radius: 14px !important;
  font-weight: 600;
  text-transform: none;
  letter-spacing: 0.3px;
  transition: all 0.3s ease !important;
  margin-left: 8px;
}

.actions :deep(.v-btn:first-child) {
  margin-left: 0;
}

.login-btn {
  color: var(--brand-ink) !important;
  opacity: 0.8;
  padding: 0 20px !important;
}

.login-btn:hover {
  color: var(--brand-primary) !important;
  opacity: 1;
  background: rgba(139, 146, 109, 0.08) !important;
  transform: translateY(-1px);
}

.register-btn {
  background: linear-gradient(135deg, var(--color-pear) 0%, #d4e2a8 100%) !important;
  color: var(--brand-ink) !important;
  box-shadow: 0 4px 15px rgba(206, 219, 149, 0.4) !important;
  padding: 0 24px !important;
}

.register-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(206, 219, 149, 0.6) !important;
}

.profile-btn {
  color: var(--brand-ink) !important;
  opacity: 0.9;
  padding: 0 20px !important;
}

.profile-btn:hover {
  color: var(--brand-primary) !important;
  background: rgba(139, 146, 109, 0.08) !important;
  transform: translateY(-1px);
}

.logout-btn {
  border: 1.5px solid rgba(139, 146, 109, 0.3) !important;
  color: var(--brand-ink) !important;
  padding: 0 20px !important;
}

.logout-btn:hover {
  border-color: var(--brand-primary) !important;
  background: rgba(139, 146, 109, 0.08) !important;
  transform: translateY(-1px);
}

.header-decoration {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 1px;
  background: linear-gradient(90deg,
  transparent 0%,
  var(--color-pear) 20%,
  var(--brand-primary) 50%,
  var(--color-pear) 80%,
  transparent 100%);
  opacity: 0.6;
}

@media (max-width: 960px) {
  .nav-links {
    display: none;
  }

  .brand {
    margin-right: 24px;
  }

  .brand-text {
    font-size: 24px;
  }
}

@media (max-width: 600px) {
  .app-header {
    height: 64px;
  }

  .brand-text {
    font-size: 20px;
  }

  .actions :deep(.v-btn) {
    font-size: 0.875rem;
    padding: 0 16px !important;
  }

  .actions :deep(.v-btn .v-icon) {
    margin-right: 4px;
  }
}

@media (max-width: 420px) {
  .app-header {
    height: 64px;
  }

  .brand{
    width: 80px;
  }

  .login-btn{
    width: 50px;
  }

  .brand-text {
    font-size: 20px;
  }

  .actions :deep(.v-btn) {
    font-size: 0.875rem;
    padding: 0 16px !important;
  }

  .actions :deep(.v-btn .v-icon) {
    margin-right: 4px;
  }
}

.nav-link:focus-visible,
.actions :deep(.v-btn:focus-visible) {
  outline: 2px solid var(--brand-primary);
  outline-offset: 2px;
  border-radius: 12px;
}

.brand,
.nav-link,
.actions :deep(.v-btn) {
  transition: all 0.3s ease;
}
</style>