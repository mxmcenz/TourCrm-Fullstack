<template>
  <div v-if="!canView" class="page pt-3">
    <div class="content-wrap bg-paper ta-center py-8" style="margin-top:60px">Нет доступа</div>
  </div>
  <div v-else>
    <RolesList
      :roles="roles"
      :page="page"
      :page-size="pageSize"
      :total-count="totalCount"
      :sort-by="sortBy"
      :desc="desc"
      :suggestions="suggestions"
      :can-create="canCreate"
      :can-edit="canEdit"
      :can-delete="canDelete"
      :can-view="canView"
      @search="onSearch"
      @search-input="onSearchInput"
      @sort-change="onSortChange"
      @edit="onEditRole"
      @delete="openDeleteConfirm"
      @page-change="onPageChange"
      @page-size-change="onPageSizeChange"
    />

    <v-dialog v-model="confirmDelete" max-width="420">
      <v-card>
        <v-card-title>Удалить роль?</v-card-title>
        <v-card-text>Вы уверены, что хотите удалить роль «{{ roleToDelete?.name }}»?</v-card-text>
        <v-card-actions>
          <v-spacer/>
          <v-btn variant="text" @click="confirmDelete = false">Отмена</v-btn>
          <PermissionTooltip :can="canDelete">
            <v-btn color="error" :disabled="loading || !canDelete" v-can.disable="'DeleteRoles'" @click="confirmRemove">Удалить</v-btn>
          </PermissionTooltip>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import {ref, onMounted, computed, watch} from 'vue'
import {useRouter} from 'vue-router'
import {useSessionStore} from '@/app/store/sessionStore'
import PermissionTooltip from '@/shared/components/PermissionTooltip.vue'
import api from '@/shared/services/api'
import RolesList from '@/features/roles/components/RolesList.vue'

const auth = useSessionStore()
const canView = computed(() => auth.has('ViewRoles'))
const canCreate = computed(() => auth.has('CreateRoles'))
const canEdit = computed(() => auth.has('EditRoles'))
const canDelete = computed(() => auth.has('DeleteRoles'))

const router = useRouter()
const roles = ref([])
const page = ref(1)
const pageSize = ref(10)
const totalCount = ref(0)
const search = ref('')
const sortBy = ref('name')
const desc = ref(false)
const suggestions = ref([])
const confirmDelete = ref(false)
const roleToDelete = ref(null)
const loading = ref(false)

async function fetchRoles () {
  if (!canView.value) { roles.value = []; totalCount.value = 0; return }
  const res = await api.get('/roles/paged', {
    params: { page: page.value, pageSize: pageSize.value, search: search.value || undefined, sortBy: sortBy.value, desc: desc.value }
  })
  roles.value = res.data.items
  totalCount.value = res.data.totalCount
}

const onPageChange = async (p) => { page.value = p; await fetchRoles() }
const onPageSizeChange = async (size) => { pageSize.value = size; page.value = 1; await fetchRoles() }
const onSearch = async (q) => { search.value = q; page.value = 1; await fetchRoles() }
const onSearchInput = async (term) => {
  if (!term) { suggestions.value = []; return }
  try {
    const res = await api.get('/roles/suggest', {params: {term, take: 10}})
    const items = Array.isArray(res.data) ? res.data : []
    suggestions.value = items.map(x => (typeof x === 'string' ? x : x.name)).filter(Boolean)
  } catch { suggestions.value = [] }
}
const onSortChange = async ({sortBy: s, desc: d}) => { sortBy.value = s; desc.value = d; page.value = 1; await fetchRoles() }
const onEditRole = (role) => { if (canEdit.value && role?.id) router.push({name: 'RolesEdit', params: {id: role.id}}) }
const openDeleteConfirm = (role) => { if (!canDelete.value) return; roleToDelete.value = role; confirmDelete.value = true }
const confirmRemove = async () => {
  if (!canDelete.value || !roleToDelete.value?.id) return
  loading.value = true
  try {
    await api.delete(`/roles/${roleToDelete.value.id}`)
    confirmDelete.value = false
    roleToDelete.value = null
    await fetchRoles()
  } finally { loading.value = false }
}

onMounted(fetchRoles)
watch(canView, (v, o) => { if (v && !o) fetchRoles() })

</script>

<style scoped>
.page{ width:100%; padding-inline:16px; box-sizing:border-box; }
.content-wrap{ border:1px solid rgba(0,0,0,.12); border-radius:12px; }
</style>
