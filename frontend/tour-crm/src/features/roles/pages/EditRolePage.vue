<template>
  <v-container class="page pt-3">
    <div class="toolbar">
      <h1 class="text-h6 text-ink">Редактирование роли</h1>

      <div class="grow"></div>

      <v-combobox
        v-model="search"
        v-model:search="searchText"
        :items="suggestions"
        :menu="Boolean(searchText)"
        placeholder="Поиск по доступам"
        persistent-placeholder
        density="comfortable"
        hide-details
        variant="outlined"
        clearable
        no-filter
        style="max-width: 360px"
        append-inner-icon="mdi-magnify"
        @update:search="onType"
        @keyup.enter.stop.prevent="applySearch"
        @click:append-inner="applySearch"
        @click:clear="clearSearch"
      >
        <template #no-data>Введите для поиска</template>
      </v-combobox>

      <v-btn variant="text" class="ml-2" @click="goBack">Отменить</v-btn>

      <PermissionTooltip :can="canDelete">
        <v-btn
          color="error"
          variant="text"
          class="ml-1"
          :disabled="loading || !canDelete"
          v-can.disable="'DeleteRoles'"
          @click="remove"
        >Удалить</v-btn>
      </PermissionTooltip>

      <PermissionTooltip :can="canEdit">
        <v-btn
          class="btn-pear ml-1"
          :disabled="loading || !roleName.trim() || !canEdit"
          v-can.disable="'EditRoles'"
          @click="save"
        >Сохранить</v-btn>
      </PermissionTooltip>
    </div>

    <div v-if="!canEdit" class="content-wrap bg-paper ta-center py-8">Нет доступа</div>

    <template v-else>
      <v-text-field
        v-model="roleName"
        label="Название роли"
        variant="outlined"
        density="comfortable"
        class="mb-4"
        :rules="[v => !!(v && v.trim()) || 'Укажите название']"
        required
        :disabled="loading"
      />

      <div v-if="loading" class="d-flex justify-center pa-6">
        <v-progress-circular indeterminate size="36"/>
      </div>

      <div v-else class="form bg-paper">
        <div class="perms">
          <div class="perm-head">Доступы</div>

          <div v-if="!filteredCategories.length" class="text-muted">Ничего не найдено</div>

          <div v-for="cat in filteredCategories" :key="cat.key" class="perm-cat">
            <div class="perm-cat-title">
              <strong>{{ cat.name }}</strong>
              <v-spacer/>
              <v-btn size="x-small" variant="text" class="text-ink" @click="setCat(cat, true)">Выделить всё</v-btn>
              <v-btn size="x-small" variant="text" class="text-ink" @click="setCat(cat, false)">Снять всё</v-btn>
            </div>

            <div class="perm-grid">
              <v-checkbox
                v-for="p in cat.items"
                :key="p.key"
                v-model="selectedMap[p.key]"
                :label="p.name"
                density="comfortable"
                hide-details
              />
            </div>
          </div>
        </div>
      </div>

      <v-dialog v-model="confirmDelete" max-width="420">
        <v-card>
          <v-card-title>Удалить роль?</v-card-title>
          <v-card-text>Вы уверены, что хотите удалить роль «{{ roleName }}»?</v-card-text>
          <v-card-actions>
            <v-spacer/>
            <v-btn variant="text" @click="confirmDelete = false">Отмена</v-btn>
            <PermissionTooltip :can="canDelete">
              <v-btn color="error" :disabled="loading || !canDelete" v-can.disable="'DeleteRoles'" @click="confirmRemove">Удалить</v-btn>
            </PermissionTooltip>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </template>
  </v-container>
</template>

<script setup>
import {ref, computed, onMounted, watch} from 'vue'
import {useRoute, useRouter} from 'vue-router'
import {useSessionStore} from '@/app/store/sessionStore'
import PermissionTooltip from '@/shared/components/PermissionTooltip.vue'
import api from '@/shared/services/api'

const auth = useSessionStore()
const canEdit = computed(() => auth.has('EditRoles'))
const canDelete = computed(() => auth.has('DeleteRoles'))

const route = useRoute()
const router = useRouter()
const id = Number(route.params.id)

const roleName = ref('')
const loading = ref(false)
const confirmDelete = ref(false)

const categories = ref([])
const selectedMap = ref({})
const search = ref(null)
const searchText = ref('')
const suggestions = ref([])
let t = null

const flatPool = computed(() =>
  categories.value.flatMap(c => c.items?.map(i => ({key: i.key, name: i.name})) ?? [])
)
const filteredCategories = computed(() => {
  const q = String(search.value || '').trim().toLowerCase()
  if (!q) return categories.value
  return categories.value
    .map(cat => ({
      ...cat,
      items: (cat.items || []).filter(p =>
        String(p.name || '').toLowerCase().includes(q) ||
        String(p.key || '').toLowerCase().includes(q)
      )
    }))
    .filter(cat => cat.items.length)
})

function onType(val) {
  const term = String(val ?? '').trim()
  clearTimeout(t)
  if (!term) { suggestions.value = []; searchText.value = ''; return }
  if (term.length < 2) return
  t = setTimeout(() => {
    const lower = term.toLowerCase()
    const pool = flatPool.value.map(p => p.name).filter(Boolean)
    suggestions.value = Array.from(new Set(pool.filter(x => String(x).toLowerCase().includes(lower)))).slice(0, 10)
  }, 200)
}
const applySearch = () => { search.value = (searchText.value || '').trim() || null }
const clearSearch = () => { search.value = null; searchText.value = ''; suggestions.value = [] }
function setCat(cat, val) { for (const p of (cat.items || [])) selectedMap.value[p.key] = !!val }

async function load() {
  loading.value = true
  try {
    const [treeRes, permRes, infoRes] = await Promise.all([
      api.get('/permissions/tree'),
      api.get(`/roles/${id}/permissions`),
      api.get(`/roles/${id}`)
    ])
    categories.value = treeRes.data || []
    roleName.value = infoRes.data?.name || ''
    const granted = (permRes.data || []).filter(x => x.isGranted).map(x => x.key)
    const map = {}
    for (const c of categories.value) for (const p of c.items) map[p.key] = granted.includes(p.key)
    selectedMap.value = map
  } finally { loading.value = false }
}

async function save() {
  if (!canEdit.value) return
  const name = roleName.value.trim()
  if (!name) return
  loading.value = true
  try {
    await api.put('/roles', {id, name})
    const keys = Object.entries(selectedMap.value).filter(([, v]) => !!v).map(([k]) => k)
    await api.put(`/roles/${id}/permissions`, {keys})
    await router.push({name: 'Roles'})
  } finally { loading.value = false }
}

function remove() { if (canDelete.value) confirmDelete.value = true }
async function confirmRemove() {
  if (!canDelete.value) return
  loading.value = true
  try {
    await api.delete(`/roles/${id}`)
    confirmDelete.value = false
    await router.push({name: 'Roles'})
  } finally { loading.value = false }
}

const goBack = () => router.push({name: 'Roles'})

onMounted(() => { if (canEdit.value) load() })
watch(canEdit, (v, o) => { if (v && !o && !categories.value.length) load() })
</script>

<style scoped>
.page{ width:100%; padding-inline:16px; box-sizing:border-box; margin-top:60px }
.toolbar{ display:flex; align-items:center; gap:12px; background:var(--color-baby-powder); padding:0 0 14px }
.grow{ flex:1 }
.form{ border:1px solid rgba(0,0,0,.12); border-radius:12px; padding:16px }
.perms{ margin-top:8px }
.perm-head{ font-weight:700; margin-bottom:8px }
.perm-cat{ border:1px solid rgba(0,0,0,.08); border-radius:10px; padding:12px; margin-bottom:12px }
.perm-cat-title{ display:flex; align-items:center; gap:8px; margin-bottom:8px }
.perm-grid{ display:grid; grid-template-columns:repeat(auto-fit, minmax(240px, 1fr)); gap:6px 16px }
.text-muted{ color:#8a8a8a }
.btn-pear{ background:var(--color-pear)!important; color:black!important; border-radius:var(--radius-pill)!important; }
.content-wrap{ border:1px solid rgba(0,0,0,.12); border-radius:12px; }
</style>
