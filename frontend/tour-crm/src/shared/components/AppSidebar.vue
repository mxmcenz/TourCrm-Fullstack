<template>
  <v-navigation-drawer
    app
    :permanent="true"
    :rail="collapsed"
    width="240"
    :rail-width="90"
    class="app-sidebar"
  >
    <div class="side-inner" :class="{ 'no-scrollbar': collapsed }">
      <v-menu v-model="createMenu" offset-y :close-on-content-click="false">
        <template #activator="{ props }">
          <v-btn
            v-bind="props"
            class="create-btn"
            density="comfortable"
            :rounded="collapsed ? 'lg' : 'lg'"
            aria-label="Создать"
            v-can="{ any: ['CreateClients','CreateLeads','CreateDeals','CreateTasks'] }"
          >
            <v-icon v-if="collapsed" small>mdi-plus</v-icon>
            <span v-if="!collapsed" class="create-text">Создать</span>
          </v-btn>
        </template>

        <v-list dense>
          <v-list-item @click="goCreate({ isTourist: 1 })" v-can="'CreateClients'">
            <v-list-item-title>Клиента</v-list-item-title>
          </v-list-item>
          <v-list-item @click="goTo({ name: 'LeadCreate' })" v-can="'CreateLeads'">
            <v-list-item-title>Лид</v-list-item-title>
          </v-list-item>
          <v-list-item @click="goTo({ name: 'DealCreate' })" v-can="'CreateDeals'">
            <v-list-item-title>Сделку</v-list-item-title>
          </v-list-item>
          <v-list-item @click="goTo({ name: 'TaskCreate' })" v-can="'CreateTasks'">
            <v-list-item-title>Задачу</v-list-item-title>
          </v-list-item>
        </v-list>
      </v-menu>

      <v-list density="comfortable" class="main-list" nav>
        <v-list-item
          v-for="item in menuItems"
          :key="item.key"
          :to="item.to"
          nav
          :rounded="collapsed ? 'lg' : 'xl'"
          class="nav-item"
          :class="{ active: isActive(item) }"
          :ripple="false"
          v-can="item.perm"
        >
          <v-icon v-if="collapsed" small class="menu-icon">{{ item.icon }}</v-icon>
          <v-list-item-title v-if="!collapsed">{{ item.label }}</v-list-item-title>
        </v-list-item>

        <v-menu
          v-if="canViewDictionaries"
          v-model="dictMenu"
          :close-on-content-click="false"
          location="end"
          offset="8"
          :max-height="360"
          :max-width="collapsed ? 360 : 320"
        >
          <template #activator="{ props: act }">
            <v-list-item
              v-bind="act"
              nav
              class="nav-item dict-activator"
              :class="{ active: dictMenu }"
              :rounded="collapsed ? 'lg' : 'xl'"
              :append-icon="!collapsed ? (dictMenu ? 'mdi-chevron-up' : 'mdi-chevron-right') : undefined"
              :ripple="false"
            >
              <v-icon v-if="collapsed" small class="menu-icon">mdi-book-open-page-variant</v-icon>
              <v-list-item-title v-if="!collapsed">Справочники</v-list-item-title>
            </v-list-item>
          </template>

          <div class="dict-popover">
            <v-text-field
              ref="dictSearchRef"
              v-model="dictSearch"
              prepend-inner-icon="mdi-magnify"
              placeholder="Поиск по справочникам"
              variant="outlined"
              density="comfortable"
              hide-details
              class="dict-search"
            />
            <v-divider/>
            <v-list density="comfortable" class="dict-scroll">
              <v-list-item
                v-for="d in filteredDictionaries"
                :key="d.key"
                @click="openDict(d)"
                :title="d.label"
              >
                <template #prepend>
                  <v-icon class="mr-2">{{ d.icon }}</v-icon>
                </template>
              </v-list-item>
              <v-list-item v-if="!filteredDictionaries.length" disabled>
                <v-list-item-title>Ничего не найдено</v-list-item-title>
              </v-list-item>
            </v-list>
          </div>
        </v-menu>
      </v-list>

      <div class="collapse-footer">
        <v-btn
          class="collapse-toggle"
          :variant="collapsed ? 'text' : 'tonal'"
          :icon="collapsed"
          :ripple="false"
          @click="toggleCollapse"
          :title="collapsed ? 'Развернуть' : 'Свернуть'"
        >
          <template v-if="collapsed">
            <v-icon>mdi-chevron-right</v-icon>
          </template>
          <template v-else>
            <v-icon start>mdi-chevron-left</v-icon>
            <span>Свернуть</span>
          </template>
        </v-btn>
      </div>
    </div>
  </v-navigation-drawer>
</template>

<script setup>
import {computed, ref, onMounted, watch, nextTick} from 'vue'
import {useRoute, useRouter} from 'vue-router'
import {useCompanyStore} from '@/features/company/store/companyStore'
import {useSessionStore} from '@/app/store/sessionStore'
import {dictionaries as dictionaryConfig} from '@/features/dictionaries/dictionaryConfig'

const route = useRoute()
const router = useRouter()
const company = useCompanyStore()
const session = useSessionStore()

const collapsed = ref(false)
const createMenu = ref(false)

const dictMenu = ref(false)
const dictSearch = ref('')
const dictSearchRef = ref(null)

const canViewDictionaries = computed(() => session.has('ViewDictionaries'))

watch(dictMenu, async v => {
  if (v) {
    dictSearch.value = ''
    await nextTick()
    dictSearchRef.value?.focus?.()
  }
})

const menuItems = computed(() => [
  { key: 'company',   label: 'Моя компания', to: { name: 'Company' }, icon: 'mdi-domain',            perm: 'ViewLegalEntities'},
  { key: 'offices',   label: 'Все офисы',    to: { name: 'Offices' }, icon: 'mdi-office-building',   perm: 'ViewOffices' },
  { key: 'roles',     label: 'Роли',         to: { name: 'Roles' },   icon: 'mdi-shield-account',    perm: 'ViewRoles' },
  { key: 'leads',     label: 'Лиды',         to: { name: 'LeadsList' }, icon: 'mdi-account-search',  perm: 'ViewLeads' },
  { key: 'deals',     label: 'Сделки',       to: { name: 'DealsList' },   icon: 'mdi-briefcase',         perm: 'ViewDeals' },
  { key: 'employees', label: 'Сотрудники',   to: { name: 'Employees' }, icon: 'mdi-account-group',   perm: 'ViewEmployees' },
  { key: 'clients',   label: 'Клиенты',      to: { name: 'ClientList' }, icon: 'mdi-account',        perm: 'ViewClients' },
].filter(i => (i.to?.name && router.hasRoute(i.to.name))))

const dictionaries = computed(() =>
  Object.entries(dictionaryConfig).map(([key, cfg]) => ({
    key,
    label: cfg.title,
    to: { name: 'Dictionary', params: { dict: key } },
    icon: pickIcon(key),
  }))
)
const filteredDictionaries = computed(() => {
  const s = dictSearch.value.trim().toLowerCase()
  if (!s) return dictionaries.value
  return dictionaries.value.filter(d => d.label.toLowerCase().includes(s) || d.key.toLowerCase().includes(s))
})

function goCreate({ isTourist = 1 } = {}) {
  if (router.hasRoute('ClientCreate')) router.push({ name: 'ClientCreate', query: { isTourist } })
  createMenu.value = false
}
function isActive(item) {
  if (item.to?.name === 'Dictionary') return route.name === 'Dictionary' && route.params.dict === item.key
  return route.name === item.to?.name
}
function goTo(to) {
  if (typeof to === 'string') {
    if (router.hasRoute(to)) router.push({ name: to })
  } else if (to?.name && router.hasRoute(to.name)) {
    router.push(to)
  }
}
function openDict(d) { dictMenu.value = false; router.push(d.to) }
function toggleCollapse() { collapsed.value = !collapsed.value }

onMounted(() => { if (!company.isReady) company.loadMyCompany() })

function pickIcon(key) {
  switch (key) {
    case 'countries': return 'mdi-earth'
    case 'cities': return 'mdi-city'
    case 'partners': return 'mdi-handshake'
    case 'servicetypes': return 'mdi-cog'
    case 'mealtypes': return 'mdi-silverware-fork-knife'
    case 'accommodationtypes': return 'mdi-bed'
    case 'numbertypes': return 'mdi-bed'
    case 'touroperators': return 'mdi-airplane'
    case 'citizenships': return 'mdi-passport'
    case 'partnermarks': return 'mdi-tag'
    case 'partnertypes': return 'mdi-account-supervisor'
    case 'currencies': return 'mdi-currency-usd'
    case 'hotels': return 'mdi-bed-king-outline'
    case 'hotel': return 'mdi-bed-king-outline'
    case 'labels': return 'mdi-label'
    case 'leadrequesttypes': return 'mdi-format-list-bulleted'
    case 'leadsources': return 'mdi-link-variant'
    case 'leadstatuses': return 'mdi-progress-check'
    case 'dealstatuses':
      return 'mdi-briefcase-check'
    default: return 'mdi-folder'
  }
}
</script>

<style scoped>
.app-sidebar {
  background: linear-gradient(180deg, var(--color-baby-powder) 0%, rgba(242, 243, 237, 0.95) 100%) !important;
  border-right: 1px solid rgba(139, 146, 109, 0.15) !important;
  box-shadow: 4px 0 20px rgba(0, 0, 0, 0.04) !important;
  z-index: 1000 !important;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1) !important;
}

.side-inner {
  padding: 24px 16px 100px;
  display: flex;
  flex-direction: column;
  flex: 1;
  overflow-y: auto;
  overflow-x: hidden;
  gap: 8px;
  background: transparent;
}

.side-inner.no-scrollbar::-webkit-scrollbar {
  display: none;
}

.side-inner.no-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}

.create-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 48px;
  background: linear-gradient(135deg, var(--color-pear) 0%, #d4e2a8 100%) !important;
  color: var(--brand-ink) !important;
  border-radius: 14px !important;
  text-transform: none;
  font-weight: 600;
  letter-spacing: 0.2px;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1) !important;
  margin-bottom: 16px;
  box-shadow: 0 4px 12px rgba(206, 219, 149, 0.3) !important;
  border: 1px solid rgba(206, 219, 149, 0.4) !important;
  position: relative;
  overflow: hidden;
}

.create-btn::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.4), transparent);
  transition: left 0.6s;
}

.create-btn:hover::before {
  left: 100%;
}

.create-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(206, 219, 149, 0.5) !important;
}

:not(.v-navigation-drawer--rail) .create-btn {
  justify-content: flex-start;
  padding: 0 20px;
  gap: 12px;
}

.v-navigation-drawer--rail .create-btn {
  width: 48px;
  height: 48px;
  margin: 0 auto 16px;
  border-radius: 12px !important;
}

.v-navigation-drawer--rail .create-btn .v-icon {
  margin: 0 !important;
  font-size: 20px;
}

.nav-item,
.dict-activator {
  display: flex;
  align-items: center;
  height: 44px;
  padding: 0 16px;
  margin: 2px 0;
  border-radius: 12px;
  box-sizing: border-box;
  background: transparent !important;
  color: var(--brand-ink) !important;
  border: 1.5px solid transparent;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  cursor: pointer;
  position: relative;
}

:not(.v-navigation-drawer--rail) .nav-item,
:not(.v-navigation-drawer--rail) .dict-activator {
  justify-content: flex-start;
}

.nav-item :deep(.v-list-item__content),
.dict-activator :deep(.v-list-item__content) {
  display: flex;
  align-items: center;
  padding: 0;
  height: 100%;
}

.nav-item :deep(.v-list-item__append),
.dict-activator :deep(.v-list-item__append) {
  margin-left: auto;
  align-self: center;
}

.nav-item :deep(.v-list-item-title),
.dict-activator :deep(.v-list-item-title) {
  font-size: 14px;
  line-height: 20px;
  font-weight: 500;
  letter-spacing: 0.1px;
}

.nav-item :deep(.v-list-item__overlay),
.nav-item :deep(.v-list-item__underlay),
.dict-activator :deep(.v-list-item__overlay),
.dict-activator :deep(.v-list-item__underlay) {
  display: none !important;
}

.nav-item:hover,
.dict-activator:hover {
  background: rgba(139, 146, 109, 0.08) !important;
  border-color: rgba(139, 146, 109, 0.2);
  transform: translateX(4px);
}

.nav-item.v-list-item--active,
.nav-item.active,
.dict-activator.active {
  background: linear-gradient(135deg, rgba(206, 219, 149, 0.15) 0%, rgba(139, 146, 109, 0.1) 100%) !important;
  border-color: var(--color-pear);
  font-weight: 600;
  box-shadow: 0 2px 8px rgba(206, 219, 149, 0.2);
}

.nav-item.v-list-item--active::before,
.nav-item.active::before,
.dict-activator.active::before {
  content: '';
  position: absolute;
  left: 0;
  top: 50%;
  transform: translateY(-50%);
  width: 3px;
  height: 20px;
  background: linear-gradient(135deg, var(--color-pear), var(--brand-primary));
  border-radius: 0 2px 2px 0;
}

.menu-icon {
  font-size: 20px !important;
  transition: all 0.3s ease;
}

.nav-item:hover .menu-icon,
.dict-activator:hover .menu-icon {
  transform: scale(1.1);
}

.nav-item.v-list-item--active .menu-icon,
.nav-item.active .menu-icon,
.dict-activator.active .menu-icon {
  color: var(--brand-primary) !important;
}

.dict-popover {
  width: 320px;
  max-width: 90vw;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.98) 0%, rgba(242, 243, 237, 0.95) 100%);
  border: 1px solid rgba(139, 146, 109, 0.15);
  border-radius: 16px;
  box-shadow: 0 12px 40px rgba(0, 0, 0, 0.12);
  overflow: hidden;
  backdrop-filter: blur(20px);
}

:deep(.v-navigation-drawer--rail) .dict-popover {
  width: 360px;
}

.dict-search {
  padding: 16px;
}

.dict-search :deep(.v-field) {
  background: rgba(255, 255, 255, 0.9);
  border-radius: 12px;
  border: 1.5px solid rgba(139, 146, 109, 0.15);
  transition: all 0.3s ease;
}

.dict-search :deep(.v-field:focus-within) {
  border-color: var(--brand-primary);
  box-shadow: 0 4px 12px rgba(139, 146, 109, 0.15);
}

.dict-search :deep(.v-field__prepend-inner) {
  color: var(--brand-primary);
}

.dict-scroll {
  max-height: 300px;
  overflow: auto;
}

.dict-scroll::-webkit-scrollbar {
  width: 4px;
}

.dict-scroll::-webkit-scrollbar-thumb {
  background: rgba(139, 146, 109, 0.3);
  border-radius: 2px;
}

.dict-scroll::-webkit-scrollbar-thumb:hover {
  background: rgba(139, 146, 109, 0.5);
}

.collapse-footer {
  position: sticky;
  bottom: 0;
  background: linear-gradient(180deg, transparent 0%, var(--color-baby-powder) 20%);
  padding: 20px 12px;
  margin-top: auto;
  border-top: 1px solid rgba(139, 146, 109, 0.1);
}

.collapse-footer::before {
  content: "";
  position: absolute;
  left: 0;
  right: 0;
  top: -20px;
  height: 20px;
  background: linear-gradient(180deg, rgba(255, 255, 255, 0), var(--color-baby-powder));
  pointer-events: none;
}

.collapse-toggle {
  width: 100%;
  height: 44px;
  border-radius: 12px;
  justify-content: center;
  text-transform: none;
  font-weight: 500;
  letter-spacing: 0.2px;
  background: rgba(139, 146, 109, 0.1) !important;
  color: var(--brand-ink) !important;
  border: 1.5px solid transparent;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1) !important;
}

.collapse-toggle:hover {
  background: rgba(139, 146, 109, 0.15) !important;
  border-color: rgba(139, 146, 109, 0.2);
  transform: translateX(-2px);
}

:deep(.v-navigation-drawer--rail) .v-list-item__title,
:deep(.v-navigation-drawer--rail) .create-text,
:deep(.v-navigation-drawer--rail) .dict-activator .v-icon[end] {
  display: none !important;
}

:deep(.v-navigation-drawer--rail) .nav-item,
:deep(.v-navigation-drawer--rail) .dict-activator {
  justify-content: center;
  padding: 0 12px;
  height: 44px;
  border-radius: 12px;
  margin: 2px 8px;
}

:deep(.v-navigation-drawer--rail) .collapse-footer {
  padding: 16px 8px;
}

:deep(.v-navigation-drawer--rail) .collapse-toggle {
  width: 44px;
  height: 44px;
  min-width: 44px;
  padding: 0;
  border-radius: 12px;
}

.side-inner:not(.no-scrollbar)::-webkit-scrollbar {
  width: 6px;
}

.side-inner:not(.no-scrollbar)::-webkit-scrollbar-track {
  background: rgba(139, 146, 109, 0.1);
  border-radius: 3px;
  margin: 8px 0;
}

.side-inner:not(.no-scrollbar)::-webkit-scrollbar-thumb {
  background: rgba(139, 146, 109, 0.3);
  border-radius: 3px;
}

.side-inner:not(.no-scrollbar)::-webkit-scrollbar-thumb:hover {
  background: rgba(139, 146, 109, 0.5);
}

.v-list-item {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.v-list-group__items {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

:deep(.v-icon) {
  transition: all 0.3s ease;
}

:deep(.v-list-item--active .v-icon) {
  color: var(--brand-primary) !important;
}

.app-sidebar:hover {
  box-shadow: 8px 0 30px rgba(0, 0, 0, 0.08) !important;
}

@media (max-width: 768px) {
  .app-sidebar {
    width: 260px !important;
    :rail-width: 70px !important;
  }

  .side-inner {
    padding: 20px 12px 90px;
  }

  .dict-popover {
    width: 280px;
  }
}

@media (prefers-color-scheme: dark) {
  .app-sidebar {
    border-right-color: rgba(255, 255, 255, 0.1) !important;
  }

  .nav-item:hover,
  .dict-activator:hover {
    background: rgba(255, 255, 255, 0.05) !important;
  }
}
</style>
