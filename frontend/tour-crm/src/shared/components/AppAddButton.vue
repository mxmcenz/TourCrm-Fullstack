<template>
  <v-btn
      class="pill"
      :color="color"
      :height="height"
      :loading="loading"
      :disabled="disabled"
      :ripple="false"
      :to="to"
      type="button"
      @click="onClick"
  >
    <slot>{{ label }}</slot>
  </v-btn>
</template>

<script setup>
/* eslint-disable */
const {label, color, height, loading, disabled, to} = defineProps({
  label: {type: String, default: '+ Добавить'},
  color: {type: String, default: 'secondary'},
  height: {type: [Number, String], default: 36},
  loading: {type: Boolean, default: false},
  disabled: {type: Boolean, default: false},
  to: {type: [String, Object], default: null},
})
const emit = defineEmits(['click'])

function onClick(e) {
  emit('click', e)
}
</script>

<style scoped>
</style>