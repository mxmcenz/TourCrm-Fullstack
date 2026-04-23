<template>
  <v-container class="pa-6" style="max-width: 700px; margin: auto;">
    <v-card class="pa-6" elevation="2">
      <h2 class="mb-4">Редактирование пользователя</h2>

      <v-text-field v-model="user.email" label="Email" readonly />

      <div class="mt-6">
        <h3 class="mb-2">Текущие роли</h3>
        <div class="d-flex flex-wrap gap-2">
          <v-chip
            v-for="role in userRoles"
            :key="role.id"
            closable
            color="primary"
            @click:close="removeRole(role.id)"
          >
            {{ role.name }}
          </v-chip>
          <div v-if="!userRoles.length" class="text-medium-emphasis">Нет назначенных ролей</div>
        </div>
      </div>

      <div class="mt-6">
        <h3 class="mb-2">Добавить роли</h3>
        <v-select
          v-model="selectedRoles"
          :items="roles"
          item-title="name"
          item-value="id"
          label="Выберите роли"
          multiple
          chips
        />
        <v-btn color="primary" class="mt-4" :loading="assigning" :disabled="!selectedRoles.length" @click="assignRoles">
          Сохранить
        </v-btn>
      </div>
    </v-card>
  </v-container>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import api from '@/shared/services/api'

const route = useRoute()
const user = ref({})
const roles = ref([])
const userRoles = ref([])
const selectedRoles = ref([])
const assigning = ref(false)

const fetchUser = async () => {
  const { data } = await api.get(`/users/${route.params.id}`)
  user.value = data
}

const fetchAllRoles = async () => {
  const { data } = await api.get('/roles')
  roles.value = data
}

const fetchUserRoles = async () => {
  const { data } = await api.get(`/roles/user/${route.params.id}`)
  userRoles.value = data
}

const assignRoles = async () => {
  assigning.value = true
  const current = new Set(userRoles.value.map(r => r.id))
  const toAssign = selectedRoles.value.filter(id => !current.has(id))
  await Promise.all(toAssign.map(roleId => api.post(`/roles/assign?userId=${user.value.id}&roleId=${roleId}`)))
  selectedRoles.value = []
  await fetchUserRoles()
  assigning.value = false
}

const removeRole = async (roleId) => {
  await api.delete(`/roles/remove?userId=${user.value.id}&roleId=${roleId}`)
  await fetchUserRoles()
}

onMounted(async () => {
  await Promise.all([fetchUser(), fetchAllRoles(), fetchUserRoles()])
})
</script>
