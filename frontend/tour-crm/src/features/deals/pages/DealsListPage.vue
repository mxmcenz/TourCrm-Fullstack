<template>
  <v-container class="page pt-3">
    <div class="toolbar">
      <h1 class="text-h6 text-ink">Сделки</h1>

      <AppAddButton @click="openCreate"/>

      <v-btn-toggle
          v-model="scope"
          class="segmented"
          density="comfortable"
          mandatory
          divided
          rounded="xl"
      >
        <v-btn value="all" variant="outlined">Все</v-btn>
        <v-btn value="active" variant="outlined">Активные</v-btn>
        <v-btn value="archived" variant="outlined">Архивные</v-btn>
      </v-btn-toggle>

      <div class="grow"/>

      <v-btn
          icon
          variant="text"
          class="settings-btn"
          title="Настроить столбцы"
          @click="openColumnsDialog"
      >
        <v-icon>mdi-cog</v-icon>
      </v-btn>

      <!-- Поиск -->
      <v-text-field
          v-model="q"
          class="search-input"
          placeholder="Поиск: № сделки, ВН, турист, менеджер"
          hide-details
          density="comfortable"
          variant="outlined"
          clearable
          :append-inner-icon="'mdi-magnify'"
          @keyup.enter="applySearch"
          @click:append-inner="applySearch"
          style="max-width: 260px"
      />

      <v-btn
          class="filters-btn ml-2"
          variant="text"
          @click="filtersOpen = true"
      >
        Фильтры
        <v-icon end class="ml-1">mdi-chevron-right</v-icon>
      </v-btn>
    </div>

    <div class="table-wrap bg-paper">
      <v-table density="comfortable">
        <thead>
        <tr>
          <th>#</th>
          <th
              v-for="col in visibleColumns"
              :key="col.key"
              :class="[{ sortable: !!col.sortKey }, col.align === 'right' ? 'ta-right' : '']"
              @click="col.sortKey && toggleSort(col.sortKey)"
          >
            <span>{{ col.label }}</span>
            <v-icon
                v-if="col.sortKey"
                size="18"
                class="sort-ico"
                :icon="sortIcon(col.sortKey)"
            />
          </th>
          <th class="ta-right">Действия</th>
        </tr>
        </thead>

        <tbody>
        <tr v-for="(r, i) in rows" :key="r.id">
          <td class="cell-strong">{{ offset + i + 1 }}</td>

          <td
              v-for="col in visibleColumns"
              :key="col.key"
              :class="col.align === 'right' ? 'ta-right' : ''"
          >
            {{ renderCell(r, col) }}
          </td>

          <td class="ta-right">
            <div class="row-actions">
              <AppIconBtn icon="mdi-pencil" aria-label="Редактировать" @click="edit(r)"/>
              <AppIconBtn
                  v-if="scope !== 'archived'"
                  icon="mdi-trash-can"
                  aria-label="Удалить"
                  @click="askArchive(r)"
              />
              <AppIconBtn
                  v-else
                  icon="mdi-backup-restore"
                  aria-label="Восстановить"
                  @click="askRestore(r)"
              />
            </div>
          </td>
        </tr>

        <tr v-if="!loading && !rows.length">
          <td :colspan="visibleColumns.length + 2" class="ta-center text-muted py-8">
            Данные не найдены
          </td>
        </tr>
        </tbody>
      </v-table>
    </div>

    <AppPager v-model:page="page" v-model:pageSize="pageSize" :total="total"/>

    <v-dialog v-model="columnsDialog" max-width="720">
      <v-card>
        <v-card-title class="px-6 py-4">
          <div class="text-h6">Столбцы в списке сделок</div>
        </v-card-title>
        <v-divider/>
        <v-card-text class="px-6 py-4">
          <div class="cols-grid">
            <v-checkbox
                v-for="c in allColumns"
                :key="c.key"
                v-model="draftSelectedKeys"
                :value="c.key"
                :label="c.label"
                hide-details
                density="comfortable"
            />
          </div>
        </v-card-text>
        <v-divider/>
        <v-card-actions class="px-6 py-3">
          <v-spacer/>
          <v-btn class="btn-secondary" @click="saveColumns">Сохранить</v-btn>
          <v-btn variant="text" class="ml-2" @click="columnsDialog=false">Отменить</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-navigation-drawer
        v-model="filtersOpen"
        location="right"
        temporary
        width="420"
        class="filters-drawer"
        :scrim="false"
    >
      <div class="filters-header">
        <div class="title">Фильтры</div>
        <div class="actions">
          <v-btn class="btn-secondary mr-2" size="small" @click="resetFilters">Сбросить</v-btn>
          <v-btn class="mr-2" variant="tonal" size="small" @click="toggleAll">
            {{ allOpen ? 'Свернуть всё' : 'Раскрыть всё' }}
          </v-btn>
          <v-btn icon variant="text" size="small" @click="filtersOpen = false">
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </div>
      </div>

      <div class="filters-scroll">
        <v-text-field
            v-model="f.number"
            label="Номер заказа"
            density="comfortable"
            variant="outlined"
            hide-details="auto"
            :class="isFilled('number') ? 'is-filled mb-3' : 'mb-3'"
        />

        <v-text-field
            v-model="f.booking"
            label="Номер брони"
            density="comfortable"
            variant="outlined"
            hide-details="auto"
            :class="isFilled('booking') ? 'is-filled mb-4' : 'mb-4'"
        />

        <v-select
            v-model="f.statusId"
            :items="statuses"
            item-title="name"
            item-value="id"
            label="Статус"
            clearable
            density="comfortable"
            variant="outlined"
            hide-details="auto"
            :class="isFilled('statusId') ? 'is-filled mb-4' : 'mb-4'"
        />

        <v-expansion-panels
            multiple
            variant="accordion"
            class="mb-4"
            v-model="openedPanels"
        >
          <v-expansion-panel value="date">
            <v-expansion-panel-title>
              <div :class="['panel-title', isSectionActive('date') && 'panel-title--active']">Дата</div>
            </v-expansion-panel-title>
            <v-expansion-panel-text>
              <div class="grid-2">
                <v-text-field v-model="f.createdFrom" type="date" label="От даты"
                              density="comfortable" variant="outlined" hide-details="auto"
                              :class="isFilled('createdFrom') ? 'is-filled' : ''"/>
                <v-text-field v-model="f.createdTo" type="date" label="До даты"
                              density="comfortable" variant="outlined" hide-details="auto"
                              :class="isFilled('createdTo') ? 'is-filled' : ''"/>
              </div>
            </v-expansion-panel-text>
          </v-expansion-panel>

          <v-expansion-panel value="deadline">
            <v-expansion-panel-title>
              <div :class="['panel-title', isSectionActive('deadline') && 'panel-title--active']">Deadline</div>
            </v-expansion-panel-title>
            <v-expansion-panel-text>
              <v-checkbox v-model="f.clientDeadlineEnabled" label="По оплате клиентом" hide-details
                          :class="isFilled('clientDeadlineEnabled') ? 'is-filled mb-2' : 'mb-2'"/>
              <v-checkbox v-model="f.partnerDeadlineEnabled" label="По оплате партнёром" hide-details
                          :class="isFilled('partnerDeadlineEnabled') ? 'is-filled' : ''"/>
            </v-expansion-panel-text>
          </v-expansion-panel>

          <v-expansion-panel value="clientFinance">
            <v-expansion-panel-title>
              <div :class="['panel-title', isSectionActive('clientFinance') && 'panel-title--active']">
                Финансы по клиенту
              </div>
            </v-expansion-panel-title>
            <v-expansion-panel-text>
              <v-select v-model="f.clientCashType" :items="cashTypes" label="Тип кассы"
                        density="comfortable" variant="outlined" hide-details="auto"
                        :class="isFilled('clientCashType') ? 'is-filled mb-3' : 'mb-3'"/>
              <v-select v-model="f.clientCurrency" :items="currencies" label="Валюта"
                        density="comfortable" variant="outlined" hide-details="auto"
                        :class="isFilled('clientCurrency') ? 'is-filled mb-3' : 'mb-3'"/>
              <div class="grid-2 mb-3">
                <v-text-field v-model.number="f.priceFrom" type="number" min="0" label="Стоимость — От"
                              density="comfortable" variant="outlined" hide-details="auto"
                              :class="isFilled('priceFrom') ? 'is-filled' : ''"/>
                <v-text-field v-model.number="f.priceTo" type="number" min="0" label="До"
                              density="comfortable" variant="outlined" hide-details="auto"
                              :class="isFilled('priceTo') ? 'is-filled' : ''"/>
              </div>
              <div class="grid-2 mb-3">
                <v-text-field v-model.number="f.paidFrom" type="number" min="0" label="Оплачено — От"
                              density="comfortable" variant="outlined" hide-details="auto"
                              :class="isFilled('paidFrom') ? 'is-filled' : ''"/>
                <v-text-field v-model.number="f.paidTo" type="number" min="0" label="До"
                              density="comfortable" variant="outlined" hide-details="auto"
                              :class="isFilled('paidTo') ? 'is-filled' : ''"/>
              </div>
              <div class="grid-2">
                <v-text-field v-model.number="f.debtFrom" type="number" min="0" label="Долг — От"
                              density="comfortable" variant="outlined" hide-details="auto"
                              :class="isFilled('debtFrom') ? 'is-filled' : ''"/>
                <v-text-field v-model.number="f.debtTo" type="number" min="0" label="До"
                              density="comfortable" variant="outlined" hide-details="auto"
                              :class="isFilled('debtTo') ? 'is-filled' : ''"/>
              </div>
            </v-expansion-panel-text>
          </v-expansion-panel>

          <v-expansion-panel value="partnerFinance">
            <v-expansion-panel-title>
              <div :class="['panel-title', isSectionActive('partnerFinance') && 'panel-title--active']">
                Финансы по партнёру
              </div>
            </v-expansion-panel-title>
            <v-expansion-panel-text>
              <v-select v-model="f.partnerCashType" :items="cashTypes" label="Тип кассы"
                        density="comfortable" variant="outlined" hide-details="auto"
                        :class="isFilled('partnerCashType') ? 'is-filled mb-3' : 'mb-3'"/>
              <v-select v-model="f.partnerCurrency" :items="currencies" label="Валюта"
                        density="comfortable" variant="outlined" hide-details="auto"
                        :class="isFilled('partnerCurrency') ? 'is-filled mb-3' : 'mb-3'"/>

              <div class="grid-2 mb-3">
                <v-text-field v-model.number="f.partnerCostFrom" type="number" min="0" label="Себестоимость — От"
                              density="comfortable" variant="outlined" hide-details="auto"
                              :class="isFilled('partnerCostFrom') ? 'is-filled' : ''"/>
                <v-text-field v-model.number="f.partnerCostTo" type="number" min="0" label="До"
                              density="comfortable" variant="outlined" hide-details="auto"
                              :class="isFilled('partnerCostTo') ? 'is-filled' : ''"/>
              </div>

              <div class="grid-2 mb-3">
                <v-text-field v-model.number="f.partnerPaidFrom" type="number" min="0" label="Оплачено — От"
                              density="comfortable" variant="outlined" hide-details="auto"
                              :class="isFilled('partnerPaidFrom') ? 'is-filled' : ''"/>
                <v-text-field v-model.number="f.partnerPaidTo" type="number" min="0" label="До"
                              density="comfortable" variant="outlined" hide-details="auto"
                              :class="isFilled('partnerPaidTo') ? 'is-filled' : ''"/>
              </div>

              <div class="grid-2">
                <v-text-field v-model.number="f.partnerDebtFrom" type="number" min="0" label="Долг — От"
                              density="comfortable" variant="outlined" hide-details="auto"
                              :class="isFilled('partnerDebtFrom') ? 'is-filled' : ''"/>
                <v-text-field v-model.number="f.partnerDebtTo" type="number" min="0" label="До"
                              density="comfortable" variant="outlined" hide-details="auto"
                              :class="isFilled('partnerDebtTo') ? 'is-filled' : ''"/>
              </div>
            </v-expansion-panel-text>
          </v-expansion-panel>

          <v-expansion-panel value="placement">
            <v-expansion-panel-title>
              <div :class="['panel-title', isSectionActive('placement') && 'panel-title--active']">Размещение</div>
            </v-expansion-panel-title>
            <v-expansion-panel-text>
              <v-text-field v-model="f.country" label="Страна" density="comfortable" variant="outlined"
                            hide-details="auto"
                            :class="isFilled('country') ? 'is-filled mb-3' : 'mb-3'"/>
              <v-text-field v-model="f.city" label="Город" density="comfortable" variant="outlined" hide-details="auto"
                            :class="isFilled('city') ? 'is-filled mb-3' : 'mb-3'"/>
              <v-text-field v-model="f.hotel" label="Отель" density="comfortable" variant="outlined" hide-details="auto"
                            :class="isFilled('hotel') ? 'is-filled mb-3' : 'mb-3'"/>
              <v-select v-model="f.roomType" :items="roomTypes" label="Тип номера" density="comfortable"
                        variant="outlined" hide-details="auto"
                        :class="isFilled('roomType') ? 'is-filled mb-3' : 'mb-3'"/>
              <v-select v-model="f.accommodationType" :items="accommodationTypes" label="Тип размещения"
                        density="comfortable" variant="outlined" hide-details="auto"
                        :class="isFilled('accommodationType') ? 'is-filled mb-3' : 'mb-3'"/>
              <v-select v-model="f.mealPlan" :items="mealPlans" label="Тип питания" density="comfortable"
                        variant="outlined" hide-details="auto"
                        :class="isFilled('mealPlan') ? 'is-filled' : ''"/>
            </v-expansion-panel-text>
          </v-expansion-panel>

          <v-expansion-panel value="customer">
            <v-expansion-panel-title>
              <div :class="['panel-title', isSectionActive('customer') && 'panel-title--active']">Заказчик</div>
            </v-expansion-panel-title>
            <v-expansion-panel-text>
              <v-select v-model="f.customerType" :items="customerTypes" item-title="label" item-value="value"
                        label="Тип заказчика" density="comfortable" variant="outlined" hide-details="auto"
                        :class="isFilled('customerType') ? 'is-filled mb-3' : 'mb-3'"/>
              <v-text-field v-model="f.tourist" label="Заказчик / Турист" density="comfortable" variant="outlined"
                            hide-details="auto"
                            :class="isFilled('tourist') ? 'is-filled mb-3' : 'mb-3'"/>
              <v-select v-model="f.customerLabel" :items="customerLabels" label="Метка заказчика" density="comfortable"
                        variant="outlined" hide-details="auto"
                        :class="isFilled('customerLabel') ? 'is-filled mb-3' : 'mb-3'"/>
              <v-select v-model="f.visaType" :items="visaTypes" label="Виза" clearable density="comfortable"
                        variant="outlined" hide-details="auto"
                        :class="isFilled('visaType') ? 'is-filled' : ''"/>
            </v-expansion-panel-text>
          </v-expansion-panel>

          <v-expansion-panel value="manager">
            <v-expansion-panel-title>
              <div :class="['panel-title', isSectionActive('manager') && 'panel-title--active']">Менеджер</div>
            </v-expansion-panel-title>
            <v-expansion-panel-text>
              <v-text-field v-model="f.createdBy" label="Кто создал" density="comfortable" variant="outlined"
                            hide-details="auto"
                            :class="isFilled('createdBy') ? 'is-filled mb-3' : 'mb-3'"/>
              <v-select v-model="f.managerId" :items="managers" item-title="fullName" item-value="id" clearable
                        label="Ответственный" density="comfortable" variant="outlined" hide-details="auto"
                        :class="isFilled('managerId') ? 'is-filled' : ''"/>
            </v-expansion-panel-text>
          </v-expansion-panel>

          <v-expansion-panel value="other">
            <v-expansion-panel-title>
              <div :class="['panel-title', isSectionActive('other') && 'panel-title--active']">Прочее</div>
            </v-expansion-panel-title>
            <v-expansion-panel-text>
              <v-select v-model="f.sourceId" :items="sources" item-title="name" item-value="id" clearable
                        label="Источник заявки"
                        density="comfortable" variant="outlined" hide-details="auto"
                        :class="isFilled('sourceId') ? 'is-filled mb-3' : 'mb-3'"/>
              <v-select v-model="f.operatorId" :items="operators" item-title="name" item-value="id" clearable
                        label="Туроператор"
                        density="comfortable" variant="outlined" hide-details="auto"
                        :class="isFilled('operatorId') ? 'is-filled mb-3' : 'mb-3'"/>
              <v-select v-model="f.partnerId" :items="partners" item-title="name" item-value="id" clearable
                        label="Партнёр"
                        density="comfortable" variant="outlined" hide-details="auto"
                        :class="isFilled('partnerId') ? 'is-filled mb-3' : 'mb-3'"/>
              <v-select v-model="f.companyId" :items="companies" item-title="name" item-value="id" clearable
                        label="Оформляющая компания"
                        density="comfortable" variant="outlined" hide-details="auto"
                        :class="isFilled('companyId') ? 'is-filled mb-3' : 'mb-3'"/>
              <v-select v-model="f.requestTypeId" :items="requestTypes" item-title="name" item-value="id" clearable
                        label="Тип заявки"
                        density="comfortable" variant="outlined" hide-details="auto"
                        :class="isFilled('requestTypeId') ? 'is-filled mb-3' : 'mb-3'"/>
              <v-select v-model="f.serviceType" :items="serviceTypes" clearable label="Тип услуги"
                        density="comfortable" variant="outlined" hide-details="auto"
                        :class="isFilled('serviceType') ? 'is-filled mb-3' : 'mb-3'"/>

              <div class="toggle-row">
                <div class="toggle-label">Отзывы</div>
                <v-radio-group v-model="f.hasReviews" inline hide-details>
                  <v-radio label="Есть" value="yes"/>
                  <v-radio label="Нет" value="no"/>
                </v-radio-group>
              </div>
              <div class="toggle-row">
                <div class="toggle-label">Задачи</div>
                <v-radio-group v-model="f.hasTasks" inline hide-details>
                  <v-radio label="Есть" value="yes"/>
                  <v-radio label="Нет" value="no"/>
                </v-radio-group>
              </div>
            </v-expansion-panel-text>
          </v-expansion-panel>
        </v-expansion-panels>

        <div class="toggle-row">
          <div class="toggle-label">Документ подписан?</div>
          <v-radio-group v-model="f.docSigned" inline hide-details>
            <v-radio label="Да" value="yes"/>
            <v-radio label="Нет" value="no"/>
          </v-radio-group>
        </div>

        <div class="toggle-row">
          <div class="toggle-label">Есть туристы?</div>
          <v-radio-group v-model="f.hasTourists" inline hide-details>
            <v-radio label="Да" value="yes"/>
            <v-radio label="Нет" value="no"/>
          </v-radio-group>
        </div>

        <v-btn block class="btn-secondary mt-3" @click="applyFilters">Применить</v-btn>
      </div>
    </v-navigation-drawer>
  </v-container>
  <v-dialog v-model="confirmArchive.open" max-width="520">
    <v-card>
      <v-card-title class="px-6 py-4">Переместить в архив</v-card-title>
      <v-divider/>
      <v-card-text class="px-6 py-4">
        Архивировать сделку <b>{{ confirmArchive.row?.dealNumber || ('#' + confirmArchive.row?.id) }}</b>?
      </v-card-text>
      <v-divider/>
      <v-card-actions class="px-6 py-3">
        <v-spacer/>
        <v-btn variant="text" @click="confirmArchive.open=false">Отменить</v-btn>
        <v-btn color="error" :loading="confirmArchive.loading" @click="doArchive">Архивировать</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>

  <v-dialog v-model="confirmRestore.open" max-width="520">
    <v-card>
      <v-card-title class="px-6 py-4">Восстановить сделку</v-card-title>
      <v-divider/>
      <v-card-text class="px-6 py-4">
        Восстановить сделку <b>{{ confirmRestore.row?.dealNumber || ('#' + confirmRestore.row?.id) }}</b> из архива?
      </v-card-text>
      <v-divider/>
      <v-card-actions class="px-6 py-3">
        <v-spacer/>
        <v-btn variant="text" @click="confirmRestore.open=false">Отменить</v-btn>
        <v-btn class="btn-secondary" :loading="confirmRestore.loading" @click="doRestore">Восстановить</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup>
import {ref, reactive, computed, watch, onMounted} from 'vue'
import {useRouter} from 'vue-router'
import {fetchDealsPage, fetchDealStatuses, archiveDeal, restoreDeal} from '../services/dealsService'
import {fetchManagers} from '@/features/leads/services/leadsService'

import AppIconBtn from '@/shared/components/AppIconBtn.vue'
import AppPager from '@/shared/components/AppPager.vue'
import AppAddButton from '@/shared/components/AppAddButton.vue'

const router = useRouter()

const rows = ref([])
const total = ref(0)
const loading = ref(false)

const page = ref(1)
const pageSize = ref(10)
const offset = computed(() => (page.value - 1) * pageSize.value)

const q = ref('')
const scope = ref('all')

const sortBy = ref('createdAt')
const sortDir = ref('desc')

const sections = ['date', 'deadline', 'clientFinance', 'partnerFinance', 'placement', 'customer', 'manager', 'other']
const openedPanels = ref([])
const allOpen = computed(() => sections.every(s => openedPanels.value.includes(s)))

function toggleAll() {
  openedPanels.value = allOpen.value ? [] : [...sections]
}

function sortIcon(by) {
  if (sortBy.value !== by) return 'mdi-arrow-down'
  return sortDir.value === 'desc' ? 'mdi-arrow-down' : 'mdi-arrow-up'
}

function toggleSort(by) {
  if (sortBy.value === by) sortDir.value = sortDir.value === 'asc' ? 'desc' : 'asc'
  else {
    sortBy.value = by
    sortDir.value = 'asc'
  }
  page.value = 1
  load()
}

function formatDate(d) {
  if (!d) return '—'
  const date = new Date(d)
  return Number.isNaN(date.getTime()) ? '—' : date.toLocaleDateString()
}

function formatMoney(v) {
  if (v == null) return '—'
  try {
    return new Intl.NumberFormat(undefined, {maximumFractionDigits: 0}).format(v)
  } catch {
    return String(v)
  }
}

function diffNights(a, b) {
  if (!a || !b) return null
  try {
    const d1 = new Date(a), d2 = new Date(b)
    const t1 = Date.UTC(d1.getFullYear(), d1.getMonth(), d1.getDate())
    const t2 = Date.UTC(d2.getFullYear(), d2.getMonth(), d2.getDate())
    return Math.max(0, Math.round((t2 - t1) / 86400000))
  } catch {
    return null
  }
}

function openCreate() {
  router.push({name: 'DealCreate'})
}

function edit(r) {
  router.push({name: 'DealEdit', params: {id: r.id}})
}

function applySearch() {
  page.value = 1
  load()
}

let currentAbort = null

function fullNameOf(x) {
  return x?.fullName || [x?.lastName, x?.firstName, x?.middleName].filter(Boolean).join(' ').trim()
}

function firstFullName(arr) {
  const c = Array.isArray(arr) ? arr[0] : null
  return fullNameOf(c) || ''
}

function firstPhone(arr) {
  const c = Array.isArray(arr) ? arr[0] : null
  return c?.phone || c?.phoneE164 || ''
}

async function load() {
  if (currentAbort) currentAbort.abort()
  currentAbort = new AbortController()
  const {signal} = currentAbort
  loading.value = true
  try {
    const payload = {
      page: page.value,
      pageSize: pageSize.value,
      query: q.value?.trim() || undefined,
      sortBy: sortBy.value,
      sortDir: sortDir.value,
      scope: scope.value,
      ...buildFiltersParams()
    }
    const {items, total: t} = await fetchDealsPage(payload, {signal})
    if (signal.aborted) return
    rows.value = (Array.isArray(items) ? items : []).map(it => ({
      ...it,
      touristName: firstFullName(it.tourists),
      customerName: firstFullName(it.customers),
      customerPhone: firstPhone(it.customers),
    }))
    total.value = Number.isFinite(t) ? t : 0
    const maxPage = Math.max(1, Math.ceil(total.value / pageSize.value))
    if (page.value > maxPage) page.value = maxPage
  } catch (e) {
    if (e?.name !== 'CanceledError' && e?.code !== 'ERR_CANCELED' && e?.message !== 'canceled') console.error(e)
  } finally {
    if (!signal.aborted) loading.value = false
  }
}

let searchTimer
watch(q, () => {
  clearTimeout(searchTimer)
  searchTimer = setTimeout(() => {
    page.value = 1
    load()
  }, 300)
})
watch(scope, () => {
  page.value = 1
  load()
})
watch(page, load)
watch(pageSize, () => {
  page.value = 1
  load()
})
onMounted(async () => {
  statuses.value = await fetchDealStatuses().catch(() => [])
  managers.value = await fetchManagers().catch(() => [])
  await load()
})

const STORAGE_KEY = 'deals.columns.v1'
const allColumns = [
  {key: 'dealNumber', label: 'Порядковый номер сделки', sortKey: 'dealNumber'},
  {key: 'internalNumber', label: 'Внутренний номер сделки', sortKey: 'internalNumber'},
  {key: 'bookingLink', label: 'Ссылка на лист бронирования'},
  {key: 'createdAt', label: 'Дата создания сделки', sortKey: 'createdAt'},
  {key: 'leadCreatedAt', label: 'Дата создания лида', sortKey: 'leadCreatedAt'},
  {key: 'startDate', label: 'Дата отправления', sortKey: 'startDate'},
  {key: 'nights', label: 'Количество ночей'},
  {key: 'flightOut', label: 'Дата и время авиаперелёта, отправление'},
  {key: 'endDate', label: 'Дата возвращения', sortKey: 'endDate'},
  {key: 'flightIn', label: 'Дата и время авиаперелёта, возвращение'},
  {key: 'tourName', label: 'Наименование программы путешествия', sortKey: 'tourName'},
  {key: 'requestTypeName', label: 'Тип заявки', sortKey: 'requestTypeName'},
  {key: 'clientPaymentDeadline', label: 'Dead-line по оплате', sortKey: 'clientPaymentDeadline'},
  {key: 'partnerPaymentDeadline', label: 'Dead-line по оплате с партнёром', sortKey: 'partnerPaymentDeadline'},
  {key: 'countryName', label: 'Страна в сделке'},
  {key: 'cityName', label: 'Город в сделке'},
  {key: 'hotelName', label: 'Отель в сделке'},
  {key: 'serviceType', label: 'Тип услуги в сделке'},
  {key: 'roomType', label: 'Тип номера'},
  {key: 'accommodationType', label: 'Тип размещения'},
  {key: 'customerName', label: 'Заказчик (покупатель)', sortKey: 'customerName'},
  {key: 'customerPhone', label: 'Телефон заказчика'},
  {key: 'statusName', label: 'Статус', sortKey: 'statusName'},
  {key: 'managerName', label: 'Менеджер', sortKey: 'managerName'},
  {key: 'price', label: 'Сумма', sortKey: 'price', align: 'right'},
]
const defaultKeys = [
  'dealNumber', 'internalNumber', 'createdAt',
  'startDate', 'endDate', 'nights',
  'tourName', 'requestTypeName',
  'clientPaymentDeadline', 'partnerPaymentDeadline',
  'statusName', 'managerName', 'customerName', 'price'
]
const selectedKeys = ref(loadKeys())
const visibleColumns = computed(() => allColumns.filter(c => selectedKeys.value.includes(c.key)))

function loadKeys() {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    if (!raw) return defaultKeys
    const parsed = JSON.parse(raw)
    return Array.isArray(parsed) && parsed.length ? parsed : defaultKeys
  } catch {
    return defaultKeys
  }
}

function renderCell(row, col) {
  switch (col.key) {
    case 'createdAt':
    case 'startDate':
    case 'endDate':
    case 'clientPaymentDeadline':
    case 'partnerPaymentDeadline':
    case 'leadCreatedAt':
      return formatDate(row[col.key])
    case 'price':
      return formatMoney(row.price)
    case 'nights':
      return diffNights(row.startDate, row.endDate) ?? '—'
    case 'touristName':
      return row.touristName || '—'
    case 'customerName':
      return row.customerName || '—'
    case 'customerPhone':
      return row.customerPhone || '—'
    case 'bookingLink':
      return row.bookingLink || row.bookingNumbers || '—'
    case 'flightOut':
    case 'flightIn':
    case 'countryName':
    case 'cityName':
    case 'hotelName':
    case 'serviceType':
    case 'roomType':
    case 'accommodationType':
      return row[col.key] || '—'
    default:
      return (row[col.key] ?? '') !== '' ? row[col.key] : '—'
  }
}

const columnsDialog = ref(false)
const draftSelectedKeys = ref([])

function openColumnsDialog() {
  draftSelectedKeys.value = [...selectedKeys.value]
  columnsDialog.value = true
}

function saveColumns() {
  selectedKeys.value = [...draftSelectedKeys.value]
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(selectedKeys.value))
  } catch (e) {
    console.warn('Failed to persist deals columns preferences', e)
  }
  columnsDialog.value = false
}

const filtersOpen = ref(false)
const statuses = ref([])
const managers = ref([])

const f = reactive({
  number: '', booking: '', statusId: null,
  createdFrom: null, createdTo: null,
  clientDeadlineEnabled: false,
  partnerDeadlineEnabled: false,
  clientCashType: null,
  clientCurrency: null,
  priceFrom: null, priceTo: null,
  paidFrom: null, paidTo: null,
  debtFrom: null, debtTo: null,
  partnerCashType: null,
  partnerCurrency: null,
  partnerCostFrom: null, partnerCostTo: null,
  partnerPaidFrom: null, partnerPaidTo: null,
  partnerDebtFrom: null, partnerDebtTo: null,
  country: '', city: '', hotel: '',
  roomType: null, accommodationType: null, mealPlan: null,
  customerType: null,
  tourist: '',
  customerLabel: null,
  visaType: null,
  docSigned: null,
  hasTourists: null,
  createdBy: '',
  managerId: null,
  sourceId: null, operatorId: null, partnerId: null, companyId: null,
  requestTypeId: null, serviceType: null,
  hasReviews: null, hasTasks: null,
})

const sectionKeys = {
  date: ['createdFrom', 'createdTo'],
  deadline: ['clientDeadlineEnabled', 'partnerDeadlineEnabled'],
  clientFinance: ['clientCashType', 'clientCurrency', 'priceFrom', 'priceTo', 'paidFrom', 'paidTo', 'debtFrom', 'debtTo'],
  partnerFinance: ['partnerCashType', 'partnerCurrency', 'partnerCostFrom', 'partnerCostTo', 'partnerPaidFrom', 'partnerPaidTo', 'partnerDebtFrom', 'partnerDebtTo'],
  placement: ['country', 'city', 'hotel', 'roomType', 'accommodationType', 'mealPlan'],
  customer: ['customerType', 'tourist', 'customerLabel', 'visaType'],
  manager: ['createdBy', 'managerId'],
  other: ['sourceId', 'operatorId', 'partnerId', 'companyId', 'requestTypeId', 'serviceType', 'hasReviews', 'hasTasks'],
}

function isFilled(key) {
  const v = f[key]
  if (v === null || v === undefined) return false
  if (typeof v === 'string') return v.trim().length > 0
  if (typeof v === 'number') return Number.isFinite(v)
  if (typeof v === 'boolean') return true
  if (Array.isArray(v)) return v.length > 0
  return !!v
}

function isSectionActive(name) {
  return (sectionKeys[name] || []).some(k => isFilled(k))
}

function resetFilters() {
  Object.assign(f, {
    number: '',
    booking: '',
    statusId: null,
    createdFrom: null,
    createdTo: null,
    clientDeadlineEnabled: false,
    partnerDeadlineEnabled: false,
    clientCashType: null,
    clientCurrency: null,
    priceFrom: null,
    priceTo: null,
    paidFrom: null,
    paidTo: null,
    debtFrom: null,
    debtTo: null,
    partnerCashType: null,
    partnerCurrency: null,
    partnerCostFrom: null,
    partnerCostTo: null,
    partnerPaidFrom: null,
    partnerPaidTo: null,
    partnerDebtFrom: null,
    partnerDebtTo: null,
    country: '',
    city: '',
    hotel: '',
    roomType: null,
    accommodationType: null,
    mealPlan: null,
    customerType: null,
    tourist: '',
    customerLabel: null,
    visaType: null,
    docSigned: null,
    hasTourists: null,
    createdBy: '',
    managerId: null,
    sourceId: null,
    operatorId: null,
    partnerId: null,
    companyId: null,
    requestTypeId: null,
    serviceType: null,
    hasReviews: null,
    hasTasks: null,
  })
}

function buildFiltersParams() {
  const out = {}
  if (f.number?.trim()) out.dealNumber = f.number.trim()
  if (f.booking?.trim()) out.booking = f.booking.trim()
  if (f.statusId) out.statusId = Number(f.statusId)
  if (f.createdFrom) out.createdFrom = f.createdFrom
  if (f.createdTo) out.createdTo = f.createdTo
  if (f.clientDeadlineEnabled) out.clientDeadlineEnabled = true
  if (f.partnerDeadlineEnabled) out.partnerDeadlineEnabled = true
  if (f.clientCashType) out.clientCashType = f.clientCashType
  if (f.clientCurrency) out.clientCurrency = f.clientCurrency
  if (Number.isFinite(f.priceFrom)) out.priceFrom = Number(f.priceFrom)
  if (Number.isFinite(f.priceTo)) out.priceTo = Number(f.priceTo)
  if (Number.isFinite(f.paidFrom)) out.paidFrom = Number(f.paidFrom)
  if (Number.isFinite(f.paidTo)) out.paidTo = Number(f.paidTo)
  if (Number.isFinite(f.debtFrom)) out.debtFrom = Number(f.debtFrom)
  if (Number.isFinite(f.debtTo)) out.debtTo = Number(f.debtTo)
  if (f.partnerCashType) out.partnerCashType = f.partnerCashType
  if (f.partnerCurrency) out.partnerCurrency = f.partnerCurrency
  if (Number.isFinite(f.partnerCostFrom)) out.partnerCostFrom = Number(f.partnerCostFrom)
  if (Number.isFinite(f.partnerCostTo)) out.partnerCostTo = Number(f.partnerCostTo)
  if (Number.isFinite(f.partnerPaidFrom)) out.partnerPaidFrom = Number(f.partnerPaidFrom)
  if (Number.isFinite(f.partnerPaidTo)) out.partnerPaidTo = Number(f.partnerPaidTo)
  if (Number.isFinite(f.partnerDebtFrom)) out.partnerDebtFrom = Number(f.partnerDebtFrom)
  if (Number.isFinite(f.partnerDebtTo)) out.partnerDebtTo = Number(f.partnerDebtTo)
  if (f.country?.trim()) out.country = f.country.trim()
  if (f.city?.trim()) out.city = f.city.trim()
  if (f.hotel?.trim()) out.hotel = f.hotel.trim()
  if (f.roomType) out.roomType = f.roomType
  if (f.accommodationType) out.accommodationType = f.accommodationType
  if (f.mealPlan) out.mealPlan = f.mealPlan
  if (f.customerType) out.customerType = f.customerType
  if (f.tourist?.trim()) out.tourist = f.tourist.trim()
  if (f.customerLabel) out.customerLabel = f.customerLabel
  if (f.visaType) out.visaType = f.visaType
  if (f.docSigned === 'yes') out.docSigned = true
  else if (f.docSigned === 'no') out.docSigned = false
  if (f.hasTourists === 'yes') out.hasTourists = true
  else if (f.hasTourists === 'no') out.hasTourists = false
  if (f.createdBy?.trim()) out.createdBy = f.createdBy.trim()
  if (f.managerId) out.managerId = Number(f.managerId)
  if (f.sourceId) out.sourceId = Number(f.sourceId)
  if (f.operatorId) out.operatorId = Number(f.operatorId)
  if (f.partnerId) out.partnerId = Number(f.partnerId)
  if (f.companyId) out.companyId = Number(f.companyId)
  if (f.requestTypeId) out.requestTypeId = Number(f.requestTypeId)
  if (f.serviceType) out.serviceType = f.serviceType
  if (f.hasReviews === 'yes') out.hasReviews = true
  else if (f.hasReviews === 'no') out.hasReviews = false
  if (f.hasTasks === 'yes') out.hasTasks = true
  else if (f.hasTasks === 'no') out.hasTasks = false
  return out
}

function applyFilters() {
  page.value = 1
  filtersOpen.value = false
  load()
}

const confirmArchive = reactive({open: false, loading: false, row: null})
const confirmRestore = reactive({open: false, loading: false, row: null})

function askArchive(row) {
  confirmArchive.row = row
  confirmArchive.open = true
}

function askRestore(row) {
  confirmRestore.row = row
  confirmRestore.open = true
}

async function doArchive() {
  if (!confirmArchive.row) return
  confirmArchive.loading = true
  try {
    await archiveDeal(confirmArchive.row.id)
    await load()
  } finally {
    confirmArchive.loading = false
    confirmArchive.open = false
  }
}

async function doRestore() {
  if (!confirmRestore.row) return
  confirmRestore.loading = true
  try {
    await restoreDeal(confirmRestore.row.id)
    await load()
  } finally {
    confirmRestore.loading = false
    confirmRestore.open = false
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

.segmented {
  margin-left: 6px;
  border: 1.5px solid rgba(139, 146, 109, 0.2);
  border-radius: 14px;
  overflow: hidden;
  background: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(8px);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

:deep(.segmented .v-btn) {
  min-height: 36px;
  border: none !important;
  border-radius: 0 !important;
  text-transform: none;
  font-weight: 500;
  padding: 0 16px;
  transition: all 0.3s ease;
  color: var(--brand-ink) !important;
  background: transparent !important;
}

:deep(.segmented .v-btn + .v-btn) {
  border-left: 1.5px solid rgba(139, 146, 109, 0.2) !important;
}

:deep(.segmented .v-btn.v-btn--active) {
  background: linear-gradient(135deg, var(--color-pear) 0%, rgba(206, 219, 149, 0.8) 100%) !important;
  color: var(--brand-ink) !important;
  font-weight: 600;
  box-shadow: 0 2px 6px rgba(206, 219, 149, 0.3);
}

:deep(.segmented .v-btn:hover:not(.v-btn--active)) {
  background: rgba(139, 146, 109, 0.1) !important;
}

.settings-btn {
  margin-right: 6px;
  border-radius: 10px !important;
  transition: all 0.3s ease !important;
}

.settings-btn:hover {
  background: rgba(139, 146, 109, 0.1) !important;
  transform: translateY(-1px);
}

:deep(.settings-btn .v-icon) {
  color: var(--brand-primary);
}

.search-input {
  max-width: 280px;
}

.search-input :deep(.v-field) {
  border-radius: 12px;
  background: rgba(255, 255, 255, 0.9);
  border: 1.5px solid rgba(139, 146, 109, 0.15);
  transition: all 0.3s ease;
}

.search-input :deep(.v-field:focus-within) {
  border-color: var(--brand-primary);
  box-shadow: 0 4px 15px rgba(139, 146, 109, 0.15);
}

.search-input :deep(.v-field__append-inner) {
  color: var(--brand-primary);
}

.filters-btn {
  text-transform: none;
  font-weight: 500;
  border-radius: 10px !important;
  transition: all 0.3s ease !important;
  background: rgba(255, 255, 255, 0.8) !important;
  border: 1.5px solid rgba(139, 146, 109, 0.15) !important;
}

.filters-btn:hover {
  background: rgba(139, 146, 109, 0.1) !important;
  transform: translateY(-1px);
  border-color: rgba(139, 146, 109, 0.2) !important;
}

.table-wrap {
  border: 1px solid rgba(139, 146, 109, 0.1);
  border-radius: 20px 20px 0 0;
  overflow: auto;
  margin-top: 24px;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.95) 0%, rgba(242, 243, 237, 0.3) 100%);
  backdrop-filter: blur(10px);
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.06);
  position: relative;
}

.table-wrap::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 1px;
  background: linear-gradient(90deg, transparent, var(--color-pear), transparent);
}

:deep(table) {
  width: max-content;
  min-width: 100%;
  table-layout: auto;
  border-collapse: separate;
  border-spacing: 0;
}

:deep(thead) {
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.9) 0%, rgba(242, 243, 237, 0.8) 100%);
}

:deep(thead th) {
  font-weight: 700;
  color: var(--brand-ink);
  border-bottom: 2px solid rgba(139, 146, 109, 0.2) !important;
  background: transparent;
  padding: 20px 16px;
  font-size: 14px;
  letter-spacing: 0.3px;
  position: relative;
  transition: all 0.3s ease;
  white-space: nowrap;
  vertical-align: middle;
}

:deep(thead th::after) {
  content: '';
  position: absolute;
  bottom: -2px;
  left: 0;
  width: 0;
  height: 2px;
  background: linear-gradient(90deg, var(--color-pear), var(--brand-primary));
  transition: width 0.3s ease;
}

:deep(thead th:hover::after) {
  width: 100%;
}

.sortable {
  cursor: pointer;
  user-select: none;
  transition: all 0.3s ease;
}

.sortable:hover {
  background: rgba(139, 146, 109, 0.05);
}

:deep(.sort-ico) {
  margin-left: 8px;
  vertical-align: middle;
  color: var(--brand-primary);
  opacity: 0.7;
  transition: all 0.3s ease;
}

.sortable:hover :deep(.sort-ico) {
  opacity: 1;
  transform: scale(1.1);
}

:deep(tbody tr) {
  transition: all 0.3s ease;
  background: transparent;
}

:deep(tbody tr:hover) {
  background: rgba(139, 146, 109, 0.05) !important;
  transform: translateY(-1px);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
}

:deep(tbody td) {
  border-bottom: 1px solid rgba(139, 146, 109, 0.08);
  padding: 16px;
  transition: all 0.3s ease;
  position: relative;
  white-space: nowrap;
  vertical-align: middle;
}

:deep(tbody tr:hover td) {
  border-color: rgba(139, 146, 109, 0.15);
}

:deep(tbody tr:last-child td) {
  border-bottom: none;
}

.cell-strong {
  font-weight: 700;
  color: var(--brand-ink);
}

.ta-right {
  text-align: right;
}

.ta-center {
  text-align: center;
}

.row-actions {
  display: flex;
  gap: 4px;
  justify-content: flex-end;
  align-items: center;
  white-space: nowrap;
}

:deep(.row-actions .v-btn) {
  min-width: 0;
  padding: 0;
  border-radius: 8px !important;
  transition: all 0.3s ease !important;
}

:deep(.row-actions .v-btn:hover) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(139, 146, 109, 0.3);
}

/* Диалог настройки колонок */
:deep(.v-dialog .v-card) {
  border-radius: 20px;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.98) 0%, rgba(242, 243, 237, 0.95) 100%);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(139, 146, 109, 0.15);
}

:deep(.v-dialog .v-card-title) {
  font-weight: 700;
  color: var(--brand-ink);
  padding: 24px;
}

:deep(.v-dialog .v-card-text) {
  padding: 24px;
}

.cols-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(240px, 1fr));
  gap: 8px 24px;
}

:deep(.cols-grid .v-checkbox) {
  margin: 0;
}

:deep(.cols-grid .v-checkbox .v-selection-control) {
  min-height: 40px;
}

:deep(.cols-grid .v-checkbox .v-label) {
  font-weight: 500;
  color: var(--brand-ink);
}

.btn-secondary {
  background: linear-gradient(135deg, var(--color-pear) 0%, #d4e2a8 100%) !important;
  color: var(--brand-ink) !important;
  font-weight: 600;
  border-radius: 12px !important;
  transition: all 0.3s ease !important;
  box-shadow: 0 4px 15px rgba(206, 219, 149, 0.3) !important;
  border: 1px solid rgba(206, 219, 149, 0.3) !important;
}

.btn-secondary:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(206, 219, 149, 0.5) !important;
}

/* Стили для фильтров */
.filters-drawer {
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.98) 0%, rgba(242, 243, 237, 0.95) 100%);
  backdrop-filter: blur(12px);
  border-left: 1px solid rgba(139, 146, 109, 0.15);
}

.filters-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 20px 20px 12px 24px;
  border-bottom: 1px solid rgba(139, 146, 109, 0.1);
}

.filters-header .title {
  font-weight: 700;
  font-size: 18px;
  color: var(--brand-ink);
}

.filters-scroll {
  height: calc(100% - 72px);
  overflow-y: auto;
  padding: 16px 20px 24px 24px;
}

.grid-2 {
  display: grid;
  grid-template-columns: minmax(150px, 1fr) minmax(150px, 1fr);
  gap: 12px;
}

.panel-title {
  font-weight: 600;
  color: var(--brand-ink);
  transition: all 0.3s ease;
}

.panel-title--active {
  position: relative;
  padding-left: 16px;
  color: var(--brand-primary);
}

.panel-title--active::before {
  content: '';
  position: absolute;
  left: 0;
  top: 50%;
  transform: translateY(-50%);
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--color-pear), var(--brand-primary));
  box-shadow: 0 2px 6px rgba(139, 146, 109, 0.4);
}

:deep(.is-filled .v-field__outline__start),
:deep(.is-filled .v-field__outline__end),
:deep(.is-filled .v-field__outline__notch) {
  border-color: var(--color-pear) !important;
  border-width: 2px !important;
}

:deep(.v-expansion-panel) {
  background: transparent !important;
  border-radius: 12px !important;
  margin-bottom: 8px;
}

:deep(.v-expansion-panel__shadow) {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08) !important;
}

.toggle-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 0;
  border-bottom: 1px solid rgba(139, 146, 109, 0.1);
}

.toggle-label {
  font-weight: 500;
  color: var(--brand-ink);
}

:deep(.app-pager) {
  margin-top: 24px;
  padding: 20px;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.9) 0%, rgba(242, 243, 237, 0.6) 100%);
  border-radius: 16px;
  border: 1px solid rgba(139, 146, 109, 0.1);
  backdrop-filter: blur(10px);
}

:deep(.app-add-button) {
  border-radius: 12px !important;
  background: linear-gradient(135deg, var(--color-pear) 0%, #d4e2a8 100%) !important;
  color: var(--brand-ink) !important;
  font-weight: 600;
  transition: all 0.3s ease !important;
  box-shadow: 0 4px 15px rgba(206, 219, 149, 0.3) !important;
  border: 1px solid rgba(206, 219, 149, 0.3) !important;
}

:deep(.app-add-button:hover) {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(206, 219, 149, 0.5) !important;
}

@media (max-width: 1200px) {
  .table-wrap {
    overflow-x: auto;
  }

  :deep(table) {
    min-width: 1200px;
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
  }

  .toolbar h1 {
    font-size: 20px;
  }

  .search-input,
  .filters-btn {
    max-width: 100%;
    order: 4;
    flex: 1 0 100%;
  }

  .cols-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 600px) {
  .page {
    padding-inline: 12px;
  }

  .toolbar {
    padding: 12px 16px;
    margin: 24px 0 20px 0;
  }

  .toolbar h1 {
    font-size: 18px;
  }

  .table-wrap {
    border-radius: 16px;
    margin-top: 20px;
  }

  :deep(table) {
    font-size: 14px;
  }

  :deep(thead th),
  :deep(tbody td) {
    padding: 12px 8px;
  }

  .grid-2 {
    grid-template-columns: 1fr;
  }

  .filters-drawer {
    width: 100% !important;
  }
}

:deep(.table-wrap)::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}

:deep(.table-wrap)::-webkit-scrollbar-track {
  background: rgba(139, 146, 109, 0.1);
  border-radius: 3px;
}

:deep(.table-wrap)::-webkit-scrollbar-thumb {
  background: rgba(139, 146, 109, 0.3);
  border-radius: 3px;
}

:deep(.table-wrap)::-webkit-scrollbar-thumb:hover {
  background: rgba(139, 146, 109, 0.5);
}
</style>