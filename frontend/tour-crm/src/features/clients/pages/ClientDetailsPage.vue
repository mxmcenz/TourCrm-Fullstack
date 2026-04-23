<template>
  <v-container class="page pt-6">
    <div class="toolbar">
      <h1 class="text-h6 text-ink">Клиент #{{ id }}</h1>
      <div class="d-flex" style="gap:12px">
        <template v-if="client && !client.isDeleted">
          <PermissionTooltip :can="canUpdate">
            <v-btn class="pill" color="secondary" :ripple="false" @click="edit" v-can.disable="'EditClients'" :disabled="!canUpdate">Редактировать</v-btn>
          </PermissionTooltip>
          <PermissionTooltip :can="canDelete">
            <v-btn class="pill" color="error" variant="tonal" :ripple="false" @click="remove" v-can.disable="'DeleteClients'" :disabled="!canDelete">Удалить</v-btn>
          </PermissionTooltip>
        </template>
        <template v-else-if="client && client.isDeleted">
          <PermissionTooltip :can="canDelete">
            <v-btn class="pill" color="secondary" :ripple="false" @click="restore" v-can.disable="'DeleteClients'" :disabled="!canDelete">Восстановить</v-btn>
          </PermissionTooltip>
        </template>
        <v-btn variant="text" :ripple="false" @click="goBack">Назад</v-btn>
      </div>
    </div>

    <v-alert
      v-if="client && client.isDeleted"
      type="warning"
      variant="tonal"
      class="mb-3"
      title="Клиент удалён"
      text="Некоторые действия недоступны. Вы можете восстановить клиента."
    />

    <div class="content-wrap bg-paper" v-if="client">
      <v-tabs v-model="tab" density="compact" class="mb-3">
        <v-tab value="details">Детали</v-tab>
        <v-tab value="history">История изменений</v-tab>
      </v-tabs>

      <v-window v-model="tab">
        <v-window-item value="details">
          <v-row>
            <v-col cols="12" md="6" class="d-flex">
              <div class="block flex-1">
                <h3 class="block-title">Основное</h3>
                <div class="kv"><span>Имя</span><b>{{ client.firstName || '—' }}</b></div>
                <div class="kv"><span>Фамилия</span><b>{{ client.lastName || '—' }}</b></div>
                <div class="kv"><span>Отчество</span><b>{{ client.middleName || '—' }}</b></div>
                <div class="kv"><span>Имя (род.)</span><b>{{ client.firstNameGenitive || '—' }}</b></div>
                <div class="kv"><span>Фамилия (род.)</span><b>{{ client.lastNameGenitive || '—' }}</b></div>
                <div class="kv"><span>Отчество (род.)</span><b>{{ client.middleNameGenitive || '—' }}</b></div>
                <div class="kv"><span>Тип</span><b>{{ clientTypeName(client.clientType) }}</b></div>
                <div class="kv"><span>Пол</span><b>{{ genderName(client.gender) }}</b></div>
                <div class="kv"><span>Турист</span><b>{{ client.isTourist ? 'Да' : 'Нет' }}</b></div>
                <div class="kv"><span>Дата рождения</span><b>{{ fmtDate(client.birthDay) }}</b></div>
                <div class="kv"><span>Телефон</span><b><a v-if="client.phoneE164" :href="`tel:${client.phoneE164}`">{{ client.phoneE164 }}</a><template v-else>—</template></b></div>
                <div class="kv"><span>Email</span><b><a v-if="client.email" :href="`mailto:${client.email}`">{{ client.email }}</a><template v-else>—</template></b></div>
                <div class="kv"><span>Подписка на рассылку</span><b>{{ yesno(client.isSubscribedToMailing) }}</b></div>
                <div class="kv"><span>Email-уведомления</span><b>{{ yesno(client.isEmailNotificationEnabled) }}</b></div>
                <div class="kv"><span>Скидка, %</span><b>{{ client.discountPercent ?? 0 }}</b></div>
                <div class="kv"><span>Удалён</span><b>{{ client.isDeleted ? 'Да' : 'Нет' }}</b></div>
                <div class="kv"><span>Примечание</span><b class="prewrap">{{ client.note || '—' }}</b></div>
              </div>
            </v-col>

            <v-col cols="12" md="6" class="d-flex">
              <div class="block flex-1">
                <h3 class="block-title">Паспорт</h3>
                <template v-if="client.passport">
                  <div class="kv"><span>Имя лат.</span><b>{{ client.passport.firstNameLatin || '—' }}</b></div>
                  <div class="kv"><span>Фамилия лат.</span><b>{{ client.passport.lastNameLatin || '—' }}</b></div>
                  <div class="kv"><span>Серия/№</span><b>{{ client.passport.serialNumber || '—' }}</b></div>
                  <div class="kv"><span>Выдан</span><b>{{ fmtDate(client.passport.issueDate) }}</b></div>
                  <div class="kv"><span>Действует до</span><b>{{ fmtDate(client.passport.expireDate) }}</b></div>
                  <div class="kv"><span>Орган</span><b class="prewrap">{{ client.passport.issuingAuthority || '—' }}</b></div>
                </template>
                <i v-else>—</i>
              </div>
            </v-col>

            <v-col cols="12" md="6" class="d-flex">
              <div class="block flex-1">
                <h3 class="block-title">Удостоверение личности</h3>
                <template v-if="client.identityDocument">
                  <div class="kv"><span>Серия/№</span><b>{{ client.identityDocument.serialNumber || '—' }}</b></div>
                  <div class="kv"><span>Документ №</span><b>{{ client.identityDocument.documentNumber || '—' }}</b></div>
                  <div class="kv"><span>Гражданство (ID)</span><b>{{ client.identityDocument.citizenshipCountryId ?? '—' }}</b></div>
                  <div class="kv"><span>Страна прож. (ID)</span><b>{{ client.identityDocument.residenceCountryId ?? '—' }}</b></div>
                  <div class="kv"><span>Город прож. (ID)</span><b>{{ client.identityDocument.residenceCityId ?? '—' }}</b></div>
                  <div class="kv"><span>Кем выдан</span><b class="prewrap">{{ client.identityDocument.issuedBy || '—' }}</b></div>
                  <div class="kv"><span>Дата выдачи</span><b>{{ fmtDate(client.identityDocument.issueDate) }}</b></div>
                  <div class="kv"><span>Рег. адрес</span><b class="prewrap">{{ client.identityDocument.registrationAddress || '—' }}</b></div>
                  <div class="kv"><span>Факт. адрес</span><b class="prewrap">{{ client.identityDocument.residentialAddress || '—' }}</b></div>
                  <div class="kv"><span>Мать</span><b class="prewrap">{{ client.identityDocument.motherFullName || '—' }}</b></div>
                  <div class="kv"><span>Отец</span><b class="prewrap">{{ client.identityDocument.fatherFullName || '—' }}</b></div>
                  <div class="kv"><span>Контакты</span><b class="prewrap">{{ client.identityDocument.contactInfo || '—' }}</b></div>
                </template>
                <i v-else>—</i>
              </div>
            </v-col>

            <v-col cols="12" md="6" class="d-flex">
              <div class="block flex-1">
                <h3 class="block-title">Свидетельство о рождении</h3>
                <template v-if="client.birthCertificate">
                  <div class="kv"><span>Серия/№</span><b>{{ client.birthCertificate.serialNumber || '—' }}</b></div>
                  <div class="kv"><span>Орган</span><b class="prewrap">{{ client.birthCertificate.issuedBy || '—' }}</b></div>
                  <div class="kv"><span>Дата выдачи</span><b>{{ fmtDate(client.birthCertificate.issueDate) }}</b></div>
                </template>
                <i v-else>—</i>
              </div>
            </v-col>

            <v-col cols="12" md="12">
              <div class="block">
                <h3 class="block-title">Страховки</h3>
                <template v-if="client.insurances?.length">
                  <v-table density="compact" class="flat-table">
                    <thead><tr><th>Выдана</th><th>Действует до</th><th>Страна (ID)</th><th>Примечание</th></tr></thead>
                    <tbody>
                    <tr v-for="(ins, idx) in client.insurances" :key="idx">
                      <td>{{ fmtDate(ins.issueDate) }}</td>
                      <td>{{ fmtDate(ins.expireDate) }}</td>
                      <td>{{ ins.countryId ?? '—' }}</td>
                      <td class="prewrap">{{ ins.note || '—' }}</td>
                    </tr>
                    </tbody>
                  </v-table>
                </template>
                <i v-else>—</i>
              </div>
            </v-col>

            <v-col cols="12" md="12">
              <div class="block">
                <h3 class="block-title">Визы</h3>
                <template v-if="client.visas?.length">
                  <v-table density="compact" class="flat-table">
                    <thead><tr><th>Выдана</th><th>Действует до</th><th>Страна (ID)</th><th>Шенген</th><th>Примечание</th></tr></thead>
                    <tbody>
                    <tr v-for="(v, idx) in client.visas" :key="idx">
                      <td>{{ fmtDate(v.issueDate) }}</td>
                      <td>{{ fmtDate(v.expireDate) }}</td>
                      <td>{{ v.countryId ?? '—' }}</td>
                      <td>{{ v.isSchengen ? 'Да' : 'Нет' }}</td>
                      <td class="prewrap">{{ v.note || '—' }}</td>
                    </tr>
                    </tbody>
                  </v-table>
                </template>
                <i v-else>—</i>
              </div>
            </v-col>
          </v-row>
        </v-window-item>

        <v-window-item value="history">
          <div class="block">
            <div class="d-flex" style="justify-content: space-between; align-items:center; margin-bottom:8px;">
              <h3 class="block-title" style="margin:0">История изменений</h3>
              <div v-if="historyTotal > 0" class="text-caption" style="color:#666">Всего: {{ historyTotal }}</div>
            </div>

            <div v-if="historyLoading" class="py-8 ta-center">Загрузка…</div>

            <template v-else>
              <template v-if="history.length">
                <v-table density="compact" class="flat-table">
                  <thead>
                  <tr>
                    <th>Дата (лок.)</th>
                    <th>Действие</th>
                    <th>User ID</th>
                    <th>Изменения</th>
                  </tr>
                  </thead>
                  <tbody>
                  <tr v-for="row in historyView" :key="row.item.id">
                    <td style="white-space:nowrap">{{ fmtLocal(row.item.atUtc) }}</td>
                    <td><v-chip size="small" :class="`act-${row.item.action.toLowerCase()}`">{{ actionName(row.item.action) }}</v-chip></td>
                    <td>{{ row.item.userId ?? '—' }}</td>
                    <td>
                      <template v-if="row.item.action === 'Insert'">
                        <i>Создан клиент</i>
                      </template>
                      <template v-else-if="row.item.action === 'Delete'">
                        <i>Клиент удалён</i>
                      </template>
                      <template v-else>
                        <template v-if="row.groups.length">
                          <ul class="diff-list">
                            <li v-for="g in row.groups" :key="g.key">
                              <template v-if="g.kind === 'scalar'">
                                <span class="path">{{ g.title }}</span>
                                <span class="arrow">→</span>
                                <span class="old">{{ g.old }}</span>
                                <span class="to">→</span>
                                <span class="new">{{ g.new }}</span>
                              </template>
                              <template v-else-if="g.kind === 'object'">
                                <div class="group-title">
                                  <span class="path">{{ g.title }}</span>
                                  <v-chip v-if="g.status" size="x-small" class="ml-2" :class="statusClass(g.status)">{{ statusName(g.status) }}</v-chip>
                                </div>
                                <ul v-if="g.items?.length" class="sub-list">
                                  <li v-for="it in g.items" :key="it.key">
                                    <span class="path">{{ it.title }}</span>
                                    <span class="arrow">→</span>
                                    <span class="old">{{ it.old }}</span>
                                    <span class="to">→</span>
                                    <span class="new">{{ it.new }}</span>
                                  </li>
                                </ul>
                              </template>
                              <template v-else-if="g.kind === 'array'">
                                <div class="group-title">
                                  <span class="path">{{ g.title }}</span>
                                  <span class="muted">было: {{ g.oldCount }} → стало: {{ g.newCount }}</span>
                                </div>
                                <ul class="sub-list">
                                  <li v-for="(it, i) in g.items" :key="i">
                                      <span v-if="g.key==='insurances'">
                                        {{ fmtDate(it.issueDate) }} → {{ fmtDate(it.expireDate) }}
                                        <span class="muted">| Страна: {{ it.countryId ?? '—' }}</span>
                                        <span v-if="it.note" class="muted">| {{ it.note }}</span>
                                      </span>
                                    <span v-else>
                                        {{ fmtDate(it.issueDate) }} → {{ fmtDate(it.expireDate) }}
                                        <span class="muted">| Страна: {{ it.countryId ?? '—' }}</span>
                                        <span class="muted">| Шенген: {{ yesno(it.isSchengen) }}</span>
                                        <span v-if="it.note" class="muted">| {{ it.note }}</span>
                                      </span>
                                  </li>
                                </ul>
                              </template>
                            </li>
                          </ul>
                        </template>
                        <i v-else>Полей не изменилось</i>
                      </template>
                    </td>
                  </tr>
                  </tbody>
                </v-table>

                <div class="d-flex" style="justify-content:flex-end; margin-top:8px" v-if="pagesCount > 1">
                  <v-pagination v-model="historyPage" :length="pagesCount" :total-visible="7" density="compact" />
                </div>
              </template>
              <i v-else>История пуста.</i>
            </template>
          </div>
        </v-window-item>
      </v-window>
    </div>

    <div v-else class="content-wrap bg-paper ta-center py-8">Загрузка…</div>
  </v-container>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useSessionStore } from '@/app/store/sessionStore'
import { getClient, restoreClient, deleteClientSoft, getClientHistory } from '@/features/clients/services/clientsService'
import PermissionTooltip from '@/shared/components/PermissionTooltip.vue'

const route = useRoute()
const router = useRouter()
const auth = useSessionStore()

const id = Number(route.params.id)
const includeDeletedInitial = route.query.includeDeleted === '1' || route.query.includeDeleted === 'true'

const client = ref(null)
const includeDeleted = ref(includeDeletedInitial)

const canUpdate = computed(() => auth.has('EditClients'))
const canDelete = computed(() => auth.has('DeleteClients'))

const clientTypeMap = { 0: 'Физ.лицо', 1: 'Юр.лицо', 2: 'Агентство' }
const genderMap = { 0: 'Не указан', 1: 'Мужской', 2: 'Женский' }
const clientTypeName = t => clientTypeMap?.[t] ?? '—'
const genderName = g => genderMap?.[g] ?? '—'
const yesno = v => v ? 'Да' : 'Нет'
const fmtDate = v => v ? String(v).slice(0, 10) : '—'
const fmtLocal = v => v ? new Date(v).toLocaleString() : '—'
const actionName = a => ({ Insert: 'Создание', Update: 'Изменение', Delete: 'Удаление', Restore: 'Восстановление' }[a] ?? a)
const statusName = s => ({ added: 'добавлено', removed: 'удалено', changed: 'изменено' }[s] ?? s)
const statusClass = s => ({ added: 'tag-added', removed: 'tag-removed', changed: 'tag-changed' }[s] ?? '')

const tab = ref('details')

const history = ref([])
const historyTotal = ref(0)
const historyLoading = ref(false)
const historyPage = ref(1)
const historyPageSize = 50
const pagesCount = computed(() => Math.max(1, Math.ceil(historyTotal.value / historyPageSize)))
let historyLoadedOnce = false
const historyOlderNeighbor = ref(null)

const labels = {
  firstName: 'Имя',
  lastName: 'Фамилия',
  middleName: 'Отчество',
  firstNameGenitive: 'Имя (род.)',
  lastNameGenitive: 'Фамилия (род.)',
  middleNameGenitive: 'Отчество (род.)',
  clientType: 'Тип клиента',
  gender: 'Пол',
  birthDay: 'Дата рождения',
  phoneE164: 'Телефон',
  email: 'Email',
  isSubscribedToMailing: 'Подписка на рассылку',
  isEmailNotificationEnabled: 'Email-уведомления',
  discountPercent: 'Скидка, %',
  isTourist: 'Турист',
  note: 'Примечание',
  passport: 'Паспорт',
  'passport.firstNameLatin': 'Имя лат.',
  'passport.lastNameLatin': 'Фамилия лат.',
  'passport.serialNumber': 'Серия/№',
  'passport.issueDate': 'Выдан',
  'passport.expireDate': 'Действует до',
  'passport.issuingAuthority': 'Орган',
  identityDocument: 'Удостоверение личности',
  'identityDocument.serialNumber': 'Серия/№',
  'identityDocument.documentNumber': 'Документ №',
  'identityDocument.citizenshipCountryId': 'Гражданство (ID)',
  'identityDocument.residenceCountryId': 'Страна прож. (ID)',
  'identityDocument.residenceCityId': 'Город прож. (ID)',
  'identityDocument.issuedBy': 'Кем выдан',
  'identityDocument.issueDate': 'Дата выдачи',
  'identityDocument.registrationAddress': 'Рег. адрес',
  'identityDocument.residentialAddress': 'Факт. адрес',
  'identityDocument.motherFullName': 'Мать',
  'identityDocument.fatherFullName': 'Отец',
  'identityDocument.contactInfo': 'Контакты',
  birthCertificate: 'Свидетельство о рождении',
  'birthCertificate.serialNumber': 'Серия/№',
  'birthCertificate.issuedBy': 'Орган',
  'birthCertificate.issueDate': 'Дата выдачи',
  insurances: 'Страховки',
  visas: 'Визы'
}

const label = k => labels[k] ?? k

const historyView = computed(() => {
  return history.value.map((item, idx) => {
    const older = idx === history.value.length - 1 ? historyOlderNeighbor.value : history.value[idx + 1]
    const groups = buildGroups(older?.data, item.data, item.action)
    return { item, groups }
  })
})

function pickKey(obj, seg) {
  if (!obj || typeof obj !== 'object') return undefined
  const low = seg.toLowerCase()
  return Object.keys(obj).find(k => k.toLowerCase() === low)
}

function getPath(obj, path) {
  let cur = obj
  for (const seg of path.split('.')) {
    if (cur == null || typeof cur !== 'object') return undefined
    const key = pickKey(cur, seg)
    if (!key) return undefined
    cur = cur[key]
  }
  return cur
}

function formatValue(key, val) {
  if (val === undefined) return undefined
  if (val === null) return null
  if (typeof val === 'boolean') return val ? 'Да' : 'Нет'
  if (key === 'gender') return genderName(Number(val))
  if (key === 'clientType') return clientTypeName(Number(val))
  if (typeof val === 'string' && /^\d{4}-\d{2}-\d{2}/.test(val)) return val.slice(0, 10)
  return val
}

function scalar(prevObj, currObj, key) {
  const a = formatValue(key, getPath(prevObj, key))
  const b = formatValue(key, getPath(currObj, key))
  if (a === b) return null
  return { kind: 'scalar', key, title: label(key), old: a ?? '—', new: b ?? '—' }
}

function objectGroup(prevObj, currObj, base, keys) {
  const was = getPath(prevObj, base)
  const now = getPath(currObj, base)
  const items = []
  let status = null
  if (!was && now) status = 'added'
  else if (was && !now) status = 'removed'
  else if (was && now) status = 'changed'
  if (was && now) {
    for (const k of keys) {
      const full = `${base}.${k}`
      const s = scalar(was, now, k)
      if (s) items.push({ key: full, title: label(full), old: s.old, new: s.new })
    }
  }
  if (!status && !items.length) return null
  return { kind: 'object', key: base, title: label(base), status, items }
}

function arrayGroup(prevObj, currObj, key, mapItem) {
  const oldA = getPath(prevObj, key)
  const newA = getPath(currObj, key)
  const A = Array.isArray(oldA) ? oldA : []
  const B = Array.isArray(newA) ? newA : []
  if (JSON.stringify(A) === JSON.stringify(B)) return null
  return { kind: 'array', key, title: label(key), oldCount: A.length, newCount: B.length, items: B.map(mapItem) }
}

function buildGroups(prevData, currData, action) {
  if (action === 'Insert') return []
  if (!currData) return []
  const groups = []
  const scalarKeys = [
    'firstName','lastName','middleName',
    'firstNameGenitive','lastNameGenitive','middleNameGenitive',
    'clientType','gender','birthDay',
    'phoneE164','email',
    'isSubscribedToMailing','isEmailNotificationEnabled',
    'discountPercent','isTourist','note'
  ]
  for (const k of scalarKeys) {
    const s = scalar(prevData, currData, k)
    if (s) groups.push(s)
  }
  const gPass = objectGroup(prevData, currData, 'passport', ['firstNameLatin','lastNameLatin','serialNumber','issueDate','expireDate','issuingAuthority'])
  const gId = objectGroup(prevData, currData, 'identityDocument', ['serialNumber','documentNumber','citizenshipCountryId','residenceCountryId','residenceCityId','issuedBy','issueDate','registrationAddress','residentialAddress','motherFullName','fatherFullName','contactInfo'])
  const gBc = objectGroup(prevData, currData, 'birthCertificate', ['serialNumber','issuedBy','issueDate'])
  if (gPass) groups.push(gPass)
  if (gId) groups.push(gId)
  if (gBc) groups.push(gBc)
  const ins = arrayGroup(prevData, currData, 'insurances', x => ({
    issueDate: getPath(x, 'IssueDate') ?? getPath(x, 'issueDate') ?? null,
    expireDate: getPath(x, 'ExpireDate') ?? getPath(x, 'expireDate') ?? null,
    countryId: getPath(x, 'CountryId') ?? getPath(x, 'countryId') ?? null,
    note: getPath(x, 'Note') ?? getPath(x, 'note') ?? null
  }))
  const vis = arrayGroup(prevData, currData, 'visas', x => ({
    issueDate: getPath(x, 'IssueDate') ?? getPath(x, 'issueDate') ?? null,
    expireDate: getPath(x, 'ExpireDate') ?? getPath(x, 'expireDate') ?? null,
    countryId: getPath(x, 'CountryId') ?? getPath(x, 'countryId') ?? null,
    isSchengen: getPath(x, 'IsSchengen') ?? getPath(x, 'isSchengen') ?? null,
    note: getPath(x, 'Note') ?? getPath(x, 'note') ?? null
  }))
  if (ins) groups.push(ins)
  if (vis) groups.push(vis)
  return groups
}

async function load() {
  client.value = await getClient(id, { includeDeleted: includeDeleted.value })
}

async function loadHistory() {
  historyLoading.value = true
  try {
    const { items, total } = await getClientHistory(id, { page: historyPage.value, pageSize: historyPageSize })
    history.value = items
    historyTotal.value = total
    if (historyPage.value < Math.ceil(total / historyPageSize)) {
      const older = await getClientHistory(id, { page: historyPage.value + 1, pageSize: 1 })
      historyOlderNeighbor.value = older.items?.[0] ?? null
    } else {
      historyOlderNeighbor.value = null
    }
    historyLoadedOnce = true
  } finally {
    historyLoading.value = false
  }
}

watch(tab, async v => { if (v === 'history' && !historyLoadedOnce) await loadHistory() })
watch(historyPage, async () => { if (tab.value === 'history') await loadHistory() })

async function restore() {
  if (!canDelete.value) return
  await restoreClient(id)
  includeDeleted.value = true
  await load()
}

async function remove() {
  if (!canDelete.value) return
  if (!confirm('Удалить клиента?')) return
  await deleteClientSoft(id)
  includeDeleted.value = true
  await load()
  router.replace({ name: route.name, params: route.params, query: { ...route.query, includeDeleted: '1' } })
}

function edit() {
  if (!canUpdate.value) return
  router.push({ name: 'ClientEdit', params: { id } })
}

function goBack() { router.push({ name: 'ClientList' }) }

onMounted(load)
</script>

<style scoped>
.page { width: 100%; padding-inline: 16px; box-sizing: border-box; margin-top: 60px; }
.toolbar { display: flex; align-items: center; gap: 16px; background: var(--color-baby-powder); padding: 14px 0; }
.content-wrap { border: 1px solid rgba(0, 0, 0, .12); border-radius: 12px; padding: 16px; }
.block { border: 1px solid rgba(0, 0, 0, .08); border-radius: 10px; padding: 12px; margin-bottom: 12px; display: flex; flex-direction: column; }
.block-title { font-weight: 600; margin: 0 0 8px 0; }
.kv { display: grid; grid-template-columns: 220px 1fr; column-gap: 12px; row-gap: 4px; padding: 4px 0; align-items: start; }
.kv span { color: #666; }
.kv b { font-weight: 600; }
.prewrap { white-space: pre-wrap; word-break: break-word; }
.pill { border-radius: 9999px; text-transform: none; }
.ta-center { text-align: center; }
.py-8 { padding: 32px 0; }
.mb-3 { margin-bottom: 12px; }
.flat-table :deep(table) { width: 100%; border-collapse: collapse; }
.flat-table :deep(th), .flat-table :deep(td) { padding: 8px 10px; border-bottom: 1px solid rgba(0, 0, 0, .06); vertical-align: top; }
.flat-table :deep(th) { text-align: left; color: #666; font-weight: 600; }
.small { font-size: 12px; line-height: 1.35; }
.diff-list { padding-left: 16px; margin: 0; }
.diff-list li { list-style: disc; margin: 8px 0; }
.sub-list { margin: 6px 0 0 18px; padding: 0; }
.sub-list li { list-style: circle; margin: 4px 0; }
.group-title { display: inline-flex; align-items: center; gap: 8px; }
.path { font-weight: 600; margin-right: 6px; }
.arrow, .to { margin: 0 6px; color: #888; }
.muted { color: #666; margin-left: 8px; }
.act-insert { background: #e8f5e9; color: #1b5e20; }
.act-update { background: #e3f2fd; color: #0d47a1; }
.act-delete { background: #ffebee; color: #b71c1c; }
.act-restore { background: #f3e5f5; color: #4a148c; }
.tag-added { background: #e8f5e9; color: #1b5e20; }
.tag-removed { background: #ffebee; color: #b71c1c; }
.tag-changed { background: #fff3e0; color: #e65100; }
</style>
