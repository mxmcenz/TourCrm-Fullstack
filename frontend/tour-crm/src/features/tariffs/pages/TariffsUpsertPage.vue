<template>
  <v-container class="page pt-3">
    <div class="toolbar">
      <h1 class="text-h6 text-ink">{{ isEdit ? 'Редактировать тариф' : 'Создать тариф' }}</h1>
      <v-spacer/>
      <v-btn variant="text" class="text-ink" @click="$router.push({name:'Tariffs'})">Назад</v-btn>
      <v-btn class="btn-pear" @click="save">Сохранить</v-btn>
    </div>

    <div class="form bg-paper">
      <v-row class="form-row" dense>
        <v-col cols="12" md="4">
          <v-text-field
            v-model="m.name"
            label="Название"
            variant="outlined"
            density="comfortable"
            :rules="[v => !!(v && v.trim()) || 'Укажите название']"
            required
          />
        </v-col>
        <v-col cols="6" md="4">
          <v-text-field v-model.number="m.minEmployees" type="number" min="1" label="Мин. сотрудников"
                        variant="outlined" density="comfortable"/>
        </v-col>
        <v-col cols="6" md="4">
          <v-text-field v-model.number="m.maxEmployees" type="number" min="1" label="Макс. сотрудников"
                        variant="outlined" density="comfortable"/>
        </v-col>
      </v-row>

      <v-row class="form-row" dense>
        <v-col cols="12" md="4">
          <v-text-field v-model.number="m.monthlyPrice" type="number" min="0" label="Цена / месяц" variant="outlined"
                        density="comfortable"/>
        </v-col>
        <v-col cols="12" md="4">
          <v-text-field v-model.number="m.halfYearPrice" type="number" min="0" label="Цена / полгода" variant="outlined"
                        density="comfortable"/>
        </v-col>
        <v-col cols="12" md="4">
          <v-text-field v-model.number="m.yearlyPrice" type="number" min="0" label="Цена / год" variant="outlined"
                        density="comfortable"/>
        </v-col>
      </v-row>

      <div class="perms">
        <div class="perm-head">Доступы</div>

        <div v-if="!categories.length" class="text-muted">Загрузка доступов…</div>

        <div v-for="cat in categories" :key="cat.key" class="perm-cat">
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
              v-model="selected[p.key]"
              :label="p.name"
              density="comfortable"
              hide-details
            />
          </div>
        </div>
      </div>
    </div>
  </v-container>
</template>

<script setup>
import {ref, onMounted, computed} from 'vue'
import {useRoute, useRouter} from 'vue-router'
import {useSessionStore} from '@/app/store/sessionStore'
import {fetchTariff, createTariff, updateTariff, fetchPermissionTree} from '../services/tariffsService'

const route = useRoute()
const router = useRouter()
const auth = useSessionStore()

const id = Number(route.params.id)
const isEdit = computed(() => Number.isFinite(id))

const m = ref({
  name: '',
  minEmployees: 1,
  maxEmployees: 1,
  monthlyPrice: 0,
  halfYearPrice: null,
  yearlyPrice: null,
  permissions: []
})

const categories = ref([])
const selected = ref({})

onMounted(async () => {
  if (!auth.isSuperAdmin) {
    await router.replace({name: 'Tariffs'})
    return
  }

  categories.value = await fetchPermissionTree()

  if (isEdit.value) {
    const t = await fetchTariff(id)
    if (t) {
      m.value = t
      for (const p of (t.permissions ?? [])) selected.value[p.permissionKey] = !!p.isGranted
    }
  } else {
    for (const cat of categories.value)
      for (const p of cat.items)
        if (!(p.key in selected.value)) selected.value[p.key] = false
  }
})

function setCat(cat, val) {
  for (const p of cat.items) selected.value[p.key] = !!val
}

async function save() {
  const name = (m.value.name ?? '').trim();
  if (!name) { alert('Укажите название тарифа'); return; }

  const payload = {
    name: m.value.name?.trim(),
    minEmployees: Number(m.value.minEmployees) || 0,
    maxEmployees: Number(m.value.maxEmployees) || 0,
    monthlyPrice: Number(m.value.monthlyPrice) || 0,
    halfYearPrice: m.value.halfYearPrice == null || m.value.halfYearPrice === '' ? null : Number(m.value.halfYearPrice),
    yearlyPrice: m.value.yearlyPrice == null || m.value.yearlyPrice === '' ? null : Number(m.value.yearlyPrice),
    permissions: Object.entries(selected.value)
      .filter((e) => !!e[1])
      .map(([key]) => ({permissionKey: key, isGranted: true}))
  }

  if (isEdit.value) await updateTariff(id, payload)
  else await createTariff(payload)

  await router.push({name: 'Tariffs'})
}
</script>

<style scoped>
.page {
  width: 100%;
  padding-inline: 16px;
  box-sizing: border-box;
  margin-top: 60px
}

.toolbar {
  display: flex;
  align-items: center;
  gap: 12px;
  background: var(--color-baby-powder);
  padding: 0 0 14px
}

.form {
  border: 1px solid rgba(0, 0, 0, .12);
  border-radius: 12px;
  padding: 16px
}

.form-row {
  margin-top: 12px
}

.perms {
  margin-top: 16px
}

.perm-head {
  font-weight: 700;
  margin-bottom: 8px
}

.perm-cat {
  border: 1px solid rgba(0, 0, 0, .08);
  border-radius: 10px;
  padding: 12px;
  margin-bottom: 12px
}

.perm-cat-title {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 8px
}

.perm-grid {
  display: grid;
  grid-template-columns:repeat(auto-fit, minmax(240px, 1fr));
  gap: 6px 16px
}
</style>
