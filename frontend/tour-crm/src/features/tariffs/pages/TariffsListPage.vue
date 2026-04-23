<template>
  <v-container class="page pt-3">
    <div class="toolbar">
      <h1 class="text-h6 text-ink">Тарифы</h1>
      <div class="grow"></div>
      <v-btn v-if="isSuperAdmin" class="btn-pear" @click="createTariff">Добавить тариф</v-btn>
    </div>

    <v-row dense class="cards justify-center align-stretch">
      <v-col v-for="t in tariffs" :key="t.id" cols="12" sm="6" md="4" lg="3" class="d-flex">
        <TariffCard :tariff="t" class="flex-1">
          <template #actions>
            <v-btn
              v-if="!isSuperAdmin"
              class="btn-pear"
              block
              :loading="actionId===t.id && actionType==='buy'"
              @click="onBuy(t)"
            >
              Купить
            </v-btn>

            <div v-else class="admin-row">
              <v-btn size="small" variant="text" class="text-ink" @click="editTariff(t)">Редактировать</v-btn>
              <v-spacer/>
              <v-btn
                size="small"
                variant="text"
                color="error"
                :loading="actionId===t.id && actionType==='delete'"
                @click="removeTariff(t)"
              >
                Удалить
              </v-btn>
            </div>
          </template>
        </TariffCard>
      </v-col>
    </v-row>

    <v-alert v-if="error" type="error" variant="tonal" class="mt-4" closable @click:close="error=null">
      {{ error }}
    </v-alert>
  </v-container>
</template>

<script setup>
import {ref, computed, onMounted} from 'vue'
import {useRouter} from 'vue-router'
import {useSessionStore} from '@/app/store/sessionStore'
import {fetchTariffs, deleteTariff} from '@/features/tariffs/services/tariffsService'
import TariffCard from '@/features/tariffs/components/TariffCard.vue'

const router = useRouter()
const auth = useSessionStore()

const tariffs = ref([])
const error = ref(null)
const actionId = ref(null)
const actionType = ref(null)

const isLoggedIn = computed(() => !!auth.user)
const isSuperAdmin = computed(() => auth.isSuperAdmin)

onMounted(load)

async function load() {
  error.value = null
  try {
    tariffs.value = await fetchTariffs()
  } catch (e) {
    error.value = e?.message || 'Не удалось загрузить тарифы'
  }
}

function onBuy(t) {
  if (!isLoggedIn.value) return router.push('/register/step1')
  actionId.value = t.id
  actionType.value = 'buy'
  alert('Оплата будет доступна скоро. Сейчас тариф не назначается автоматически.')
  actionId.value = null
  actionType.value = null
}

function createTariff() {
  router.push({name: 'TariffCreate'})
}

function editTariff(t) {
  router.push({name: 'TariffEdit', params: {id: t.id}})
}

async function removeTariff(t) {
  if (!confirm(`Удалить тариф «${t.name}»?`)) return
  actionId.value = t.id
  actionType.value = 'delete'
  error.value = null
  try {
    await deleteTariff(t.id)
    await load()
  } catch (e) {
    error.value = e?.response?.data?.message || e?.message || 'Ошибка удаления'
  } finally {
    actionId.value = null
    actionType.value = null
  }
}
</script>

<style scoped>
.page {
  width: 100%;
  padding-inline: 24px;
  box-sizing: border-box;
  background: linear-gradient(135deg, var(--color-baby-powder) 0%, rgba(242, 243, 237, 0.3) 100%);
  min-height: calc(100vh - 64px);
}

.toolbar {
  display: flex;
  align-items: center;
  gap: 20px;
  padding: 20px 24px;
  margin: 32px 0 24px 0;
  position: relative;
  overflow: hidden;
}

.toolbar h1 {
  font-size: 24px;
  font-weight: 800;
  background: black;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin: 0;
  letter-spacing: -0.2px;
}

.grow {
  flex: 1;
}

.btn-pear {
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

.btn-pear:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(206, 219, 149, 0.5) !important;
}

.btn-pear:disabled {
  opacity: 0.5;
  transform: none !important;
  box-shadow: 0 2px 8px rgba(206, 219, 149, 0.2) !important;
}

.cards {
  margin-top: 24px;
  gap: 24px;
}

.cards :deep(.v-col) {
  display: flex;
  animation: fadeInUp 0.6s ease-out;
}

.cards :deep(.v-col:nth-child(1)) { animation-delay: 0.1s; }
.cards :deep(.v-col:nth-child(2)) { animation-delay: 0.2s; }
.cards :deep(.v-col:nth-child(3)) { animation-delay: 0.3s; }
.cards :deep(.v-col:nth-child(4)) { animation-delay: 0.4s; }

.cards :deep(.v-card) {
  width: 100%;
  border-radius: 20px;
  overflow: hidden;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.95) 0%, rgba(242, 243, 237, 0.8) 100%);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(139, 146, 109, 0.15);
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.08);
  position: relative;
}

.cards :deep(.v-card::before) {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, var(--color-pear), var(--brand-primary));
  border-radius: 20px 20px 0 0;
}

.cards :deep(.v-card:hover) {
  transform: translateY(-8px);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.12);
  border-color: rgba(139, 146, 109, 0.3);
}

.admin-row {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-top: auto;
}

.admin-row :deep(.v-btn) {
  border-radius: 10px !important;
  text-transform: none;
  font-weight: 500;
  font-size: 13px;
  height: 36px;
  padding: 0 16px !important;
  transition: all 0.3s ease !important;
}

.admin-row :deep(.v-btn--variant-text) {
  background: rgba(139, 146, 109, 0.08) !important;
  border: 1px solid transparent !important;
}

.admin-row :deep(.v-btn--variant-text:hover) {
  background: rgba(139, 146, 109, 0.15) !important;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(139, 146, 109, 0.2);
}

.admin-row :deep(.v-btn--variant-text.text-ink) {
  color: var(--brand-ink) !important;
}

.admin-row :deep(.v-btn--variant-text.color-error) {
  color: #d32f2f !important;
}

:deep(.v-alert) {
  border-radius: 16px;
  border: 1px solid rgba(139, 146, 109, 0.2);
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.95) 0%, rgba(242, 243, 237, 0.8) 100%);
  backdrop-filter: blur(10px);
  margin-top: 24px;
  padding: 16px 20px;
}

:deep(.v-alert--variant-tonal) {
  background: rgba(211, 47, 47, 0.05) !important;
  border-color: rgba(211, 47, 47, 0.2) !important;
}

:deep(.v-alert__close) {
  margin-top: -2px;
  margin-right: -4px;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@media (max-width: 960px) {
  .page {
    padding-inline: 16px;
  }

  .toolbar {
    padding: 16px 20px;
    flex-wrap: wrap;
    gap: 16px;
    margin: 24px 0 20px 0;
  }

  .toolbar h1 {
    font-size: 20px;
  }

  .cards {
    gap: 20px;
  }

  .cards :deep(.v-col) {
    max-width: 400px;
    margin: 0 auto;
  }
}

@media (max-width: 600px) {
  .page {
    padding-inline: 12px;
  }

  .toolbar {
    padding: 12px 16px;
    margin: 20px 0 16px 0;
    border-radius: 16px;
  }

  .toolbar h1 {
    font-size: 18px;
  }

  .btn-pear {
    height: 40px !important;
    padding: 0 20px !important;
    font-size: 14px;
  }

  .cards {
    gap: 16px;
    margin-top: 20px;
  }

  .cards :deep(.v-card) {
    border-radius: 16px;
  }

  .cards :deep(.v-card:hover) {
    transform: translateY(-4px);
  }

  .admin-row {
    padding: 12px 12px 0 12px;
    flex-direction: column;
    gap: 6px;
  }

  .admin-row :deep(.v-btn) {
    width: 100%;
    justify-content: center;
  }

  :deep(.v-alert) {
    border-radius: 12px;
    padding: 14px 16px;
  }
}

:deep(.v-progress-circular) {
  color: var(--brand-primary);
}

.page::-webkit-scrollbar {
  width: 6px;
}

.page::-webkit-scrollbar-track {
  background: rgba(139, 146, 109, 0.1);
  border-radius: 3px;
}

.page::-webkit-scrollbar-thumb {
  background: rgba(139, 146, 109, 0.3);
  border-radius: 3px;
}

.page::-webkit-scrollbar-thumb:hover {
  background: rgba(139, 146, 109, 0.5);
}
</style>