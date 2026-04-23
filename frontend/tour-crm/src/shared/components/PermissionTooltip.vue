<template>
  <v-tooltip v-if="!can" :text="text" location="top">
    <template #activator="{ props }">
      <span v-bind="props" style="display:inline-flex">
        <slot />
      </span>
    </template>
  </v-tooltip>
  <slot v-else />
</template>

<script setup>
// eslint-disable-next-line no-undef
/* global defineProps */
defineProps({
  can: { type: Boolean, default: true },
  text: { type: String, default: 'Нет доступа' },
})
</script>
