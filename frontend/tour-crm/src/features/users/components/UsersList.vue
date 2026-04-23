<template>
  <v-container class="pa-4" outlined style="border: 1px solid #ccc; max-width: 1100px; margin: auto;">
    <v-row justify="space-between" align="center" class="mb-4">
      <v-btn color="primary" @click="dialog = true">+ Добавить</v-btn>
      <v-text-field
        v-model="search"
        placeholder="Введите для поиска"
        append-inner-icon="mdi-magnify"
        @click:append-inner="onSearch"
        density="compact"
        hide-details
        variant="outlined"
        style="max-width: 250px;"
      />
    </v-row>

    <v-table density="comfortable" class="mt-4" fixed-header style="border: 1px solid #eee;">
      <thead>
      <tr>
        <th style="width: 60px;">#</th>
        <th>Пользователь</th>
        <th style="width: 120px; text-align: right;">Действия</th>
      </tr>
      </thead>
      <tbody>
      <tr v-for="(user, index) in filteredUsers" :key="user.id">
        <td>{{ index + 1 }}</td>
        <td>{{ user.email }}</td>
        <td class="text-right">
          <v-btn icon="mdi-pencil" size="small" color="primary" @click="goToEdit(user.id)" />
        </td>
      </tr>
      </tbody>
    </v-table>
  </v-container>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/shared/services/api'

const users = ref([])
const search = ref('')
const dialog = ref(false)
const router = useRouter()

const fetchUsers = async () => {
  try {
    const { data } = await api.get('/users')
    users.value = data
  } catch (e) {
    console.error('Ошибка загрузки пользователей', e)
  }
}

const filteredUsers = computed(() => {
  if (!search.value) return users.value
  return users.value.filter(u =>
    u.name.toLowerCase().includes(search.value.toLowerCase())
  )
})

const onSearch = () => {}

const goToEdit = (id) => {
  router.push({ name: 'UserEdit', params: { id } })
}

onMounted(fetchUsers)
</script>
