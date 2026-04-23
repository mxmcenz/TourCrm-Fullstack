<template>
  <v-card class="tariff-card" variant="outlined">
    <div class="card-head">
      <div class="name">{{ tariff.name }}</div>
      <div class="range text-muted">{{ tariff.minEmployees }}–{{ tariff.maxEmployees }} сотрудников</div>
    </div>

    <div class="period-row">
      <v-select
        v-model="period"
        :items="periodOptions"
        item-title="label"
        item-value="value"
        density="comfortable"
        variant="outlined"
        hide-details
        class="select-dark"
        style="max-width: 160px"
        @update:modelValue="load"
      />
    </div>

    <div class="price-wrap">
      <div class="price" :class="{ dim: loading }">
        <template v-if="price != null">{{ fmt(price) }} ₸</template>
        <template v-else>—</template>
      </div>
      <div class="period-label text-muted">за {{ currentLabel }}</div>
      <div v-if="monthly != null" class="per-month text-muted">≈ {{ fmt(monthly) }} ₸ / мес</div>
    </div>

    <v-divider/>

    <div class="actions-row">
      <slot name="actions"/>
    </div>
  </v-card>
</template>

<script setup>
// eslint-disable-next-line no-undef
/* global defineProps */
import {ref, computed, onMounted} from 'vue'
import {fetchTariffPrice, fetchTariffMonthly} from '@/features/tariffs/services/tariffsService'

const props = defineProps({tariff: {type: Object, required: true}})

const periodOptions = [
  {label: 'Месяц', value: 'Month'},
  {label: 'Полгода', value: 'HalfYear'},
  {label: 'Год', value: 'Year'},
]

const period = ref('Month')
const price = ref(null)
const monthly = ref(null)
const loading = ref(false)
const cache = ref({})

const currentLabel = computed(() => periodOptions.find(p => p.value === period.value)?.label ?? 'период')
const fmt = v => new Intl.NumberFormat(undefined, {maximumFractionDigits: 0}).format(v)

async function load() {
  const k = period.value
  if (cache.value[k]) {
    price.value = cache.value[k].price
    monthly.value = cache.value[k].monthly
    return
  }
  loading.value = true
  try {
    const [p, m] = await Promise.all([
      fetchTariffPrice(props.tariff.id, k),
      fetchTariffMonthly(props.tariff.id, k),
    ])
    price.value = p
    monthly.value = m
    cache.value[k] = {price: p, monthly: m}
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<style scoped>
.tariff-card {
  display: flex;
  flex-direction: column;
  gap: 12px;
  height: 100%;
  border-radius: 16px !important;
  border: 1px solid var(--color-silver) !important;
  transition: border-color .15s ease, box-shadow .15s ease;
}

.tariff-card:hover {
  border-color: #a8a8a8 !important;
  box-shadow: 0 1px 0 rgba(0, 0, 0, .04), 0 2px 8px rgba(0, 0, 0, .06);
}

.tariff-card :deep(.v-divider) {
  background: rgba(0, 0, 0, .12) !important;
}

.card-head {
  padding: 16px 16px 0 16px
}

.name {
  font-weight: 800;
  font-size: 18px;
  color: var(--brand-ink)
}

.range {
  margin-top: 4px
}

.period-row {
  padding: 0 16px
}

.price-wrap {
  padding: 6px 16px 8px 16px;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.price {
  font-size: 28px;
  font-weight: 800;
  line-height: 1.2;
  color: var(--brand-ink)
}

.period-label, .per-month {
  font-size: 12px
}

.dim {
  opacity: .55
}

.actions-row {
  padding: 12px 16px 16px 16px
}

.select-dark :deep(.v-field__input),
.select-dark :deep(.v-select__selection-text),
.select-dark :deep(.v-field-label) {
  color: var(--brand-ink) !important;
}

.select-dark :deep(.v-field__append-inner .v-icon) {
  color: var(--brand-ink) !important;
  opacity: 1 !important;
}

.select-dark :deep(.v-field--variant-outlined .v-field__outline) {
  display: none !important;
}

.select-dark :deep(.v-field) {
  border: 1px solid var(--color-silver) !important;
  border-radius: 12px !important;
  background: transparent !important;
}

.select-dark :deep(.v-field:hover) {
  border-color: #a8a8a8 !important;
}

.select-dark :deep(.v-field.v-field--focused) {
  border-color: var(--brand-primary) !important;
}
</style>

