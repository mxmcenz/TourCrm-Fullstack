<template>
  <v-btn
    icon
    variant="text"
    :ripple="false"
    class="icon-btn"
    rounded="circle"
    width="30"
    height="30"
    :title="title"
    @click="onClick"
  >
    <v-icon :size="size" :icon="icon"/>
  </v-btn>
</template>

<script setup>
/* global defineProps, defineEmits */
defineProps({
  icon: {type: String, required: true},
  size: {type: [Number, String], default: 18},
  title: {type: String, default: ''}
})

const emit = defineEmits(['click'])

function onClick(e) {
  emit('click', e)
}
</script>

<style scoped>
.icon-btn {
  min-width: 0;
  padding: 0;
  border: 1px solid rgba(0, 0, 0, .35);
  background: #fff;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  color: var(--color-black) !important;
  margin-left: 10px;
}

.icon-btn:hover {
  background: rgba(0, 0, 0, .06);
}
</style>