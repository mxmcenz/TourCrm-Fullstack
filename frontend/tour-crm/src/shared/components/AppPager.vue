<template>
  <div class="pager">
    <div></div>

    <div class="pager-center">
      <v-btn variant="text" :ripple="false" class="pager-arrow" @click="prev" :disabled="isPrevDisabled">◄</v-btn>

      <button
          v-for="n in pageNumbers"
          :key="n"
          type="button"
          class="pager-num"
          :class="{ active: n === page }"
          @click="setPage(n)"
      >
        {{ n }}
      </button>

      <v-btn variant="text" :ripple="false" class="pager-arrow" @click="next" :disabled="isNextDisabled">►</v-btn>
    </div>

    <div class="pager-size">
      <v-select
          :model-value="pageSize"
          @update:model-value="setSize"
          :items="sizeOptions"
          density="compact"
          variant="outlined"
          hide-details
          class="size-select"
          menu-icon="mdi-chevron-down"
          style="width:77px"
      />
    </div>
  </div>
</template>

<script setup>
/* global defineProps, defineEmits */
import { computed } from 'vue'

const props = defineProps({
  page: { type: Number, default: 1 },
  pageSize: { type: Number, default: 10 },
  total: { type: Number, default: 0 },
  sizeOptions: { type: Array, default: () => [5, 10, 20, 50] }
})
const emit = defineEmits(['update:page', 'update:pageSize'])

const pages = computed(() => Math.max(1, Math.ceil(props.total / props.pageSize)))
const pageNumbers = computed(() => Array.from({ length: pages.value }, (_, i) => i + 1))
const isPrevDisabled = computed(() => props.page <= 1)
const isNextDisabled = computed(() => props.page >= pages.value)

function prev(){ if (props.page > 1) emit('update:page', props.page - 1) }
function next(){ if (props.page < pages.value) emit('update:page', props.page + 1) }
function setPage(n){ if (n >= 1 && n <= pages.value) emit('update:page', n) }
function setSize(val){ emit('update:pageSize', Number(val)); emit('update:page', 1) }

const page = computed(() => props.page)
const pageSize = computed(() => props.pageSize)
</script>

<style scoped>
.pager{
  display:grid; grid-template-columns:1fr auto 1fr; align-items:center;
  background: var(--color-baby-powder);
  border:1px solid rgba(0,0,0,.12); border-top:none;
  border-radius:0 0 12px 12px; padding:10px 12px; margin-top:-1px;
}
.pager-center{ justify-self:center; display:inline-flex; align-items:center; gap:8px; }
.pager-arrow{ min-width:0; height:28px; padding:0 6px; }
.pager-num{ background:transparent; border:0; padding:2px 4px; font:inherit; color:var(--brand-ink); cursor:pointer; opacity:.8; }
.pager-num.active{ font-weight:700; opacity:1; }
.pager-size{ justify-self:end; }

.size-select{ width:79px !important; }
.size-select :deep(.v-field){ height:32px; border-radius:9999px; }
.size-select :deep(.v-field__input){ min-height:32px; padding:0 10px!important; text-align:center; justify-content:center; }
.size-select :deep(.v-select__selection-text){ margin:0 auto; }
.size-select :deep(.v-field__append-inner .v-icon){ opacity:.9; }
</style>