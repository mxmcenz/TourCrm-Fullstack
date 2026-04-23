<template>
  <v-container class="page pt-3">
    <div class="toolbar">
      <h1 class="text-h6 text-ink">Роли</h1>

      <PermissionTooltip :can="canCreate">
        <AppAddButton
          :to="canCreate ? { name: 'RolesCreate' } : undefined"
          :disabled="!canCreate"
          v-can.disable="'CreateRoles'"
        />
      </PermissionTooltip>

      <div class="grow"></div>

      <v-combobox
        v-model="search"
        v-model:search="searchText"
        :items="suggestions"
        :menu="Boolean(searchText)"
        placeholder="Поиск: название роли"
        persistent-placeholder
        density="comfortable"
        hide-details
        variant="outlined"
        clearable
        no-filter
        style="max-width: 360px"
        :append-inner-icon="'mdi-magnify'"
        @update:search="onType"
        @keyup.enter.stop.prevent="emitSearch"
        @click:append-inner="emitSearch"
        @click:clear="onClear"
      >
        <template #no-data>Введите для поиска</template>
      </v-combobox>
    </div>

    <div v-if="!canView" class="content-wrap bg-paper ta-center py-8">Нет доступа</div>

    <div v-else class="table-wrap bg-paper">
      <v-table density="comfortable">
        <thead>
        <tr>
          <th style="width:60px">#</th>
          <th class="sortable" @click="toggleSort('name')">
            <span class="d-inline-flex align-center">
              Название
              <v-icon size="18" class="ml-1">
                {{ sortBy === 'name' ? (desc ? 'mdi-arrow-down' : 'mdi-arrow-up') : 'mdi-arrow-up-down' }}
              </v-icon>
            </span>
          </th>
          <th class="ta-right" style="width:140px">Действия</th>
        </tr>
        </thead>

        <tbody>
        <tr v-for="(r, i) in roles" :key="r.id">
          <td>{{ (page - 1) * pageSize + i + 1 }}</td>
          <td class="cell-strong">
            <PermissionTooltip :can="canEdit">
              <RouterLink
                v-if="canEdit"
                class="no-underline"
                :to="{ name:'RolesEdit', params:{ id: r.id } }"
                v-can.disable="'EditRoles'"
              >{{ r.name }}</RouterLink>
            </PermissionTooltip>
            <span v-if="!canEdit">{{ r.name }}</span>
          </td>
          <td class="ta-right">
            <PermissionTooltip :can="canEdit">
              <AppIconBtn
                icon="mdi-pencil"
                aria-label="Редактировать"
                :to="canEdit ? { name:'RolesEdit', params:{ id: r.id } } : undefined"
                v-can.disable="'EditRoles'"
                :disabled="!canEdit"
              />
            </PermissionTooltip>
            <PermissionTooltip :can="canDelete">
              <AppIconBtn
                icon="mdi-trash-can"
                aria-label="Удалить"
                @click="$emit('delete', r)"
                v-can.disable="'DeleteRoles'"
                :disabled="!canDelete"
              />
            </PermissionTooltip>
          </td>
        </tr>

        <tr v-if="!roles?.length">
          <td colspan="3" class="ta-center text-muted py-8">Данные не найдены</td>
        </tr>
        </tbody>
      </v-table>
    </div>

    <AppPager
      v-if="canView"
      v-model:page="pageLocal"
      v-model:pageSize="pageSizeLocal"
      :total="totalCount"
      @update:page="onPageChange"
      @update:pageSize="onPageSizeChange"
    />
  </v-container>
</template>

<script>
import AppPager from '@/shared/components/AppPager.vue'
import AppAddButton from '@/shared/components/AppAddButton.vue'
import AppIconBtn from '@/shared/components/AppIconBtn.vue'
import PermissionTooltip from '@/shared/components/PermissionTooltip.vue'

export default {
  name: 'RolesList',
  components: { AppPager, AppAddButton, AppIconBtn, PermissionTooltip },
  props: {
    roles: { type: Array, required: true },
    page: { type: Number, required: true },
    pageSize: { type: Number, required: true },
    totalCount: { type: Number, required: true },
    sortBy: { type: String, default: 'name' },
    desc: { type: Boolean, default: false },
    suggestions: { type: Array, default: () => [] },
    canCreate: { type: Boolean, default: false },
    canEdit: { type: Boolean, default: false },
    canDelete: { type: Boolean, default: false },
    canView: { type: Boolean, default: false }
  },
  emits: ['search', 'search-input', 'edit', 'delete', 'page-change', 'page-size-change', 'sort-change'],
  data() {
    return {
      search: null,
      searchText: '',
      pageLocal: this.page || 1,
      pageSizeLocal: this.pageSize || 10,
      typeTimer: null
    }
  },
  watch: {
    page(v) { this.pageLocal = v || 1 },
    pageSize(v) { this.pageSizeLocal = v || 10 }
  },
  methods: {
    onType(val) {
      const term = String(val ?? '').trim()
      clearTimeout(this.typeTimer)
      if (!term) {
        this.$emit('search-input', '')
        this.searchText = ''
        return
      }
      if (term.length < 2) return
      this.typeTimer = setTimeout(() => { this.$emit('search-input', term) }, 250)
    },
    emitSearch() {
      const term = String(this.searchText || '').trim()
      this.$emit('search', term)
      if (this.pageLocal !== 1) this.$emit('page-change', 1)
    },
    onClear() {
      this.search = null
      this.searchText = ''
      this.$emit('search-input', '')
      this.$emit('search', '')
      if (this.pageLocal !== 1) this.$emit('page-change', 1)
    },
    onPageChange(val) { this.$emit('page-change', val) },
    onPageSizeChange(val) { this.$emit('page-size-change', val) },
    toggleSort(field) {
      const nextDesc = this.sortBy === field ? !this.desc : false
      this.$emit('sort-change', { sortBy: field, desc: nextDesc })
      if (this.pageLocal !== 1) this.$emit('page-change', 1)
    }
  }
}
</script>

<style scoped>
.page {
  width: 100%;
  padding-inline: 24px;
  box-sizing: border-box;
  background: linear-gradient(135deg, var(--color-baby-powder) 0%, rgba(242, 243, 237, 0.3) 100%);
  min-height: calc(100vh - 64px);
}

.toolbar {
  display: flex;
  align-items: center;
  gap: 20px;
  margin: 32px 0 24px 0;
  position: relative;
  overflow: hidden;
}

.toolbar h1 {
  font-size: 24px;
  font-weight: 800;
  background: black;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin: 0;
  letter-spacing: -0.2px;
}

.grow {
  flex: 1;
}

/* Стили для combobox поиска */
:deep(.v-combobox .v-field) {
  border-radius: 12px;
  background: rgba(255, 255, 255, 0.9);
  border: 1.5px solid rgba(139, 146, 109, 0.15);
  transition: all 0.3s ease;
}

:deep(.v-combobox .v-field:focus-within) {
  border-color: var(--brand-primary);
  box-shadow: 0 4px 15px rgba(139, 146, 109, 0.15);
}

:deep(.v-combobox .v-field__append-inner) {
  color: var(--brand-primary);
}

:deep(.v-combobox .v-menu .v-overlay__content) {
  border-radius: 12px;
  margin-top: 4px;
}

:deep(.v-combobox .v-list) {
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.98) 0%, rgba(242, 243, 237, 0.95) 100%);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(139, 146, 109, 0.12);
  border-radius: 12px;
  box-shadow: 0 12px 35px rgba(0, 0, 0, 0.12);
}

.content-wrap {
  border: 1px solid rgba(139, 146, 109, 0.1);
  border-radius: 20px;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.95) 0%, rgba(242, 243, 237, 0.3) 100%);
  backdrop-filter: blur(10px);
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.06);
}

.table-wrap {
  border: 1px solid rgba(139, 146, 109, 0.1);
  border-radius: 20px 20px 0 0;
  overflow: hidden;
  margin-top: 24px;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.95) 0%, rgba(242, 243, 237, 0.3) 100%);
  backdrop-filter: blur(10px);
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.06);
  position: relative;
}

.table-wrap::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 1px;
  background: linear-gradient(90deg, transparent, var(--color-pear), transparent);
}

:deep(table) {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
}

:deep(thead) {
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.9) 0%, rgba(242, 243, 237, 0.8) 100%);
}

:deep(thead th) {
  font-weight: 700;
  color: var(--brand-ink);
  border-bottom: 2px solid rgba(139, 146, 109, 0.2) !important;
  background: transparent;
  padding: 20px 16px;
  font-size: 14px;
  letter-spacing: 0.3px;
  position: relative;
}

:deep(thead th::after) {
  content: '';
  position: absolute;
  bottom: -2px;
  left: 0;
  width: 0;
  height: 2px;
  background: linear-gradient(90deg, var(--color-pear), var(--brand-primary));
  transition: width 0.3s ease;
}

:deep(thead th:hover::after) {
  width: 100%;
}

.sortable {
  cursor: pointer;
  user-select: none;
  transition: all 0.3s ease;
}

.sortable:hover {
  background: rgba(139, 146, 109, 0.05);
}

:deep(tbody tr) {
  transition: all 0.3s ease;
  background: transparent;
}

:deep(tbody tr:hover) {
  background: rgba(139, 146, 109, 0.05) !important;
  transform: translateY(-1px);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
}

:deep(tbody td) {
  border-bottom: 1px solid rgba(139, 146, 109, 0.08);
  padding: 16px;
  transition: all 0.3s ease;
  position: relative;
}

:deep(tbody tr:hover td) {
  border-color: rgba(139, 146, 109, 0.15);
}

:deep(tbody tr:last-child td) {
  border-bottom: none;
}

.cell-strong {
  font-weight: 700;
}

.ta-right {
  text-align: right;
}

.ta-center {
  text-align: center;
}

.no-underline {
  text-decoration: none;
  color: inherit;
  font-weight: 600;
  padding: 8px 12px;
  border-radius: 10px;
  transition: all 0.3s ease;
  background: rgba(139, 146, 109, 0.08);
  display: inline-block;
  position: relative;
  overflow: hidden;
}

.no-underline::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.4), transparent);
  transition: left 0.5s ease;
}

.no-underline:hover::before {
  left: 100%;
}

.no-underline:hover {
  background: rgba(139, 146, 109, 0.15);
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(139, 146, 109, 0.2);
}

:deep(.app-icon-btn) {
  border-radius: 10px !important;
  transition: all 0.3s ease !important;
  margin: 0 2px;
}

:deep(.app-icon-btn:hover) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(139, 146, 109, 0.3);
}

:deep(.app-pager) {
  margin-top: 24px;
  padding: 20px;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.9) 0%, rgba(242, 243, 237, 0.6) 100%);
  border-radius: 16px;
  border: 1px solid rgba(139, 146, 109, 0.1);
  backdrop-filter: blur(10px);
}

:deep(.app-add-button) {
  border-radius: 12px !important;
  background: linear-gradient(135deg, var(--color-pear) 0%, #d4e2a8 100%) !important;
  color: var(--brand-ink) !important;
  font-weight: 600;
  transition: all 0.3s ease !important;
  box-shadow: 0 4px 15px rgba(206, 219, 149, 0.3) !important;
  border: 1px solid rgba(206, 219, 149, 0.3) !important;
}

:deep(.app-add-button:hover) {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(206, 219, 149, 0.5) !important;
}

:deep(.permission-tooltip) {
  display: inline-flex;
}

.text-muted {
  color: var(--color-gray);
  opacity: 0.8;
}

/* Стили для иконок сортировки */
:deep(.sortable .v-icon) {
  color: var(--brand-primary);
  transition: all 0.3s ease;
}

:deep(.sortable:hover .v-icon) {
  transform: scale(1.1);
}

@media (max-width: 960px) {
  .page {
    padding-inline: 16px;
  }

  .toolbar {
    padding: 16px 20px;
    flex-wrap: wrap;
    gap: 16px;
  }

  .toolbar h1 {
    font-size: 20px;
  }

  :deep(.v-combobox) {
    max-width: 100%;
    order: 3;
    flex: 1 0 100%;
  }
}

@media (max-width: 600px) {
  .page {
    padding-inline: 12px;
  }

  .toolbar {
    padding: 12px 16px;
    margin: 24px 0 20px 0;
  }

  .toolbar h1 {
    font-size: 18px;
  }

  .table-wrap {
    border-radius: 16px;
    margin-top: 20px;
  }

  :deep(table) {
    font-size: 14px;
  }

  :deep(thead th),
  :deep(tbody td) {
    padding: 12px 8px;
  }

  .no-underline {
    padding: 6px 10px;
    font-size: 14px;
  }

  :deep(.sortable .v-icon) {
    font-size: 16px;
  }
}

:deep(.table-wrap)::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}

:deep(.table-wrap)::-webkit-scrollbar-track {
  background: rgba(139, 146, 109, 0.1);
  border-radius: 3px;
}

:deep(.table-wrap)::-webkit-scrollbar-thumb {
  background: rgba(139, 146, 109, 0.3);
  border-radius: 3px;
}

:deep(.table-wrap)::-webkit-scrollbar-thumb:hover {
  background: rgba(139, 146, 109, 0.5);
}
</style>
