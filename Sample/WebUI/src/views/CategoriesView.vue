// Categories — M5 screen. Demo5 budget tracks: name, progress bar, spent
// value with the "of budget" note below. The month spend comes from one
// search over the effective timeline, grouped client-side — one call, not
// one per category. The budget is a limit, never money: it stays positive
// while the expenses it measures are negative.

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { activeClient, errorMessage, session } from '../state/session'
import type { CategoryRequest, CategoryResponse } from '../api/types'
import { budgetNote, budgetTrack, sumSpentByCategory } from '../api/categories'
import { formatMoney } from '../api/wallets'
import { monthWindow } from '../api/months'

const categories = ref<CategoryResponse[]>([])
const spent = ref<Map<number, number>>(new Map())
const loading = ref(true)

// form state — formOpen shows the card; editingId null = creating
const formOpen = ref(false)
const editingId = ref<number | null>(null)
const formName = ref('')
const formIsExpense = ref(true)
const formBudget = ref('')
const formDescription = ref('')
const formError = ref('')
const saving = ref(false)


async function load(): Promise<void> {
  const client = activeClient()
  if (!client) return
  loading.value = true
  try {
    const [cats, txs] = await Promise.all([
      client.categories(),
      client.searchTransactions({ ...monthWindow(), dateType: 'EffectiveDate', order: 'Asc' }),
    ])
    categories.value = cats
    spent.value = sumSpentByCategory(txs)
  } catch (e) {
    session.error.value = errorMessage(e)
  } finally {
    loading.value = false
  }
}

onMounted(() => void load())

function openCreate(): void {
  formOpen.value = true
  editingId.value = null
  formName.value = ''
  formIsExpense.value = true
  formBudget.value = ''
  formDescription.value = ''
  formError.value = ''
}

function openEdit(category: CategoryResponse): void {
  formOpen.value = true
  editingId.value = category.id
  formName.value = category.name
  formIsExpense.value = category.isExpense
  formBudget.value = category.monthlyBudget > 0 ? String(category.monthlyBudget) : ''
  formDescription.value = category.description
  formError.value = ''
}

async function save(): Promise<void> {
  const client = activeClient()
  if (!client) return
  formError.value = ''
  const name = formName.value.trim()
  if (!name) {
    formError.value = 'Category.Name is required'
    return
  }
  const budget = formBudget.value.trim() === '' ? 0 : Number(formBudget.value)
  if (Number.isNaN(budget) || budget < 0) {
    formError.value = 'MonthlyBudget must not be negative'
    return
  }
  const body: CategoryRequest = {
    isExpense: formIsExpense.value,
    name,
    description: formDescription.value.trim(),
    monthlyBudget: budget,
    isDeleted: false,
  }
  saving.value = true
  try {
    if (editingId.value === null) await client.createCategory(body)
    else await client.updateCategory(editingId.value, body)
    formOpen.value = false
    editingId.value = null
    await load()
  } catch (e) {
    formError.value = errorMessage(e)
  } finally {
    saving.value = false
  }
}

function cancelEdit(): void {
  formOpen.value = false
  editingId.value = null
  formError.value = ''
}

function trackOf(category: CategoryResponse): { pct: number; over: boolean } | null {
  if (!category.isExpense || category.monthlyBudget <= 0) return null
  return budgetTrack(spent.value.get(category.id) ?? 0, category.monthlyBudget)
}

function spentOf(category: CategoryResponse): number {
  return spent.value.get(category.id) ?? 0
}
</script>

<template>
  <section>
    <div class="sec"><h2>Categories</h2><button class="add" @click="openCreate">＋ New</button></div>

    <p v-if="loading" class="muted">Loading…</p>

    <div v-else-if="categories.length > 0" class="ledger">
      <div
        v-for="c in categories"
        :key="c.id"
        class="walletline"
        :class="{ del: c.isDeleted }"
        @click="openEdit(c)"
      >
        <span class="name">{{ c.name }}</span>
        <div v-if="trackOf(c)" class="track">
          <div class="fill" :class="{ over: trackOf(c)?.over }" :style="{ width: `${trackOf(c)?.pct}%` }"></div>
        </div>
        <div v-else class="track none"></div>
        <b :class="{ neg: trackOf(c)?.over }">
          {{ formatMoney(spentOf(c)) }}<span v-if="budgetNote(c.monthlyBudget)" class="of">{{ budgetNote(c.monthlyBudget) }}</span>
          <span class="kind" :class="{ inc: !c.isExpense }">{{ c.isExpense ? 'exp' : 'inc' }}</span>
        </b>
      </div>
    </div>

    <div v-else class="ledger empty">
      <p>No categories yet — create the first one above: what the money is for.</p>
    </div>

    <div v-if="formOpen" class="form card">
      <h3>{{ editingId === null ? 'New category' : 'Edit category' }}</h3>
      <label>
        Name
        <input v-model="formName" type="text" placeholder="Groceries" autocomplete="off">
      </label>
      <div class="lock">
        <span class="lbl">Side</span>
        <div v-if="editingId === null" class="types">
          <button :class="{ on: formIsExpense }" @click="formIsExpense = true">Expense</button>
          <button :class="{ on: !formIsExpense }" @click="formIsExpense = false">Income</button>
        </div>
        <div v-else class="locked">
          {{ formIsExpense ? 'Expense — locked after creation' : 'Income — locked after creation' }}
        </div>
      </div>
      <label v-if="formIsExpense">
        Monthly budget <span class="hint">(0 for none)</span>
        <input v-model="formBudget" type="text" inputmode="decimal" placeholder="1000" autocomplete="off">
      </label>
      <label>
        Description <span class="hint">(optional)</span>
        <input v-model="formDescription" type="text" placeholder="Free text" autocomplete="off">
      </label>
      <p v-if="formError" class="err">{{ formError }}</p>
      <div class="row">
        <button class="ghost" :disabled="saving" @click="cancelEdit">Cancel</button>
        <button class="primary" :disabled="saving" @click="save">{{ saving ? 'Saving…' : 'Save' }}</button>
      </div>
    </div>
  </section>
</template>

<style scoped>
.sec { display: flex; justify-content: space-between; align-items: center; margin: 1.1rem 0 0.5rem; }
.sec h2 { margin: 0; font-size: 0.72rem; letter-spacing: 0.12em; text-transform: uppercase; color: var(--muted); }
.add {
  border: 1px solid var(--border); border-radius: 999px; background: transparent;
  color: var(--accent); font: inherit; font-size: 0.78rem; padding: 0.35rem 0.9rem; cursor: pointer;
}
.muted { color: var(--muted); font-size: 0.9rem; }

.ledger { background: var(--surface); border: 1px solid var(--border); border-radius: 12px; }
.ledger.empty { padding: 1rem 0.9rem; font-size: 0.85rem; color: var(--muted); }

.walletline { display: flex; align-items: center; gap: 0.6rem; padding: 0.55rem 0.8rem; font-size: 0.85rem; cursor: pointer; }
.walletline + .walletline { border-top: 1px solid var(--border); }
.walletline .name { width: 6em; color: var(--fg); overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.walletline .track { flex: 1; height: 0.5rem; border-radius: 99px; background: var(--bg); border: 1px solid var(--border); overflow: hidden; }
.walletline .track.none { visibility: hidden; }
.walletline .fill { height: 100%; background: var(--accent); border-radius: 99px; }
.walletline .fill.over { background: var(--error); }
.walletline b { display: block; font-family: var(--mono); font-variant-numeric: tabular-nums; font-size: 0.82rem; text-align: right; }
.walletline b.neg { color: var(--error); }
.walletline .of { display: block; font-size: 0.6rem; color: var(--muted); font-family: var(--mono); }
.walletline .kind { display: block; font-size: 0.55rem; color: var(--muted); letter-spacing: 0.1em; text-transform: uppercase; font-family: inherit; }
.walletline .kind.inc { color: var(--success); }
.walletline.del { opacity: 0.5; }

.card { background: var(--surface); border: 1px solid var(--border); border-radius: 14px;
  padding: 1rem 1.1rem; margin-top: 0.9rem; }
h3 { margin: 0 0 0.8rem; font-size: 0.95rem; }
label { display: block; font-size: 0.72rem; letter-spacing: 0.1em; text-transform: uppercase;
  color: var(--muted); margin-bottom: 0.8rem; }
label input { display: block; width: 100%; box-sizing: border-box; margin-top: 0.3rem;
  font: inherit; font-size: 0.95rem; text-transform: none; letter-spacing: normal; color: var(--fg);
  background: transparent; border: 1px solid var(--border); border-radius: 10px; padding: 0.55rem 0.7rem; }
label .hint { text-transform: none; letter-spacing: normal; }
.lock { margin-bottom: 0.8rem; }
.lock .lbl { display: block; font-size: 0.72rem; letter-spacing: 0.1em; text-transform: uppercase; color: var(--muted); margin-bottom: 0.3rem; }
.lock .locked { font-size: 0.85rem; color: var(--fg); opacity: 0.7; padding: 0.4rem 0; }
.types { display: flex; gap: 0.5rem; }
.types button { border: 1px solid var(--border); border-radius: 999px; background: transparent;
  color: var(--fg); font: inherit; font-size: 0.8rem; padding: 0.4rem 0.85rem; cursor: pointer; }
.types button.on { background: var(--accent-soft); border-color: var(--accent); color: var(--accent); }
.err { color: var(--error); font-size: 0.8rem; margin: 0 0 0.6rem; }
.row { display: flex; gap: 0.6rem; justify-content: flex-end; }
.ghost { border: 1px solid var(--border); border-radius: 999px; background: transparent;
  color: var(--fg); font: inherit; font-size: 0.8rem; padding: 0.45rem 1rem; cursor: pointer; }
.primary { border: 1px solid var(--accent); border-radius: 999px; background: var(--accent);
  color: var(--on-accent, #fff); font: inherit; font-size: 0.8rem; padding: 0.45rem 1.2rem; cursor: pointer; }
button:disabled { opacity: 0.55; }
</style>
