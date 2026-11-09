// Search (M10) — the power tool: the /by filters with the shared shaping of
// the two search endpoints. Not a core screen: the window is arbitrary (not
// a month), the ids are tri-state (All / one / None), and every id sent
// must match (AND). Result rows behave exactly like book rows — open, edit,
// quick actions (M9) — no dead-end: the row you found is the row you act on.

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { availableActions, payAsToday, payOnDue, reverseRow, unreverseRow, type QuickActionName } from '../api/actions'
import { rowDate } from '../api/book'
import { buildSearchQuery, resultRows, type DateType, type SearchFormState } from '../api/search'
import { activeClient, errorMessage, session } from '../state/session'
import { formatMoney } from '../api/wallets'
import type { CategoryResponse, PersonResponse, TransactionResponse, WalletResponse } from '../api/types'
import MovementForm from '../components/MovementForm.vue'
import TransferForm from '../components/TransferForm.vue'

const wallets = ref<WalletResponse[]>([])
const categories = ref<CategoryResponse[]>([])
const persons = ref<PersonResponse[]>([])

const ran = ref(false)
const loading = ref(false)
const rows = ref<ReturnType<typeof resultRows>>([])
const searchError = ref('')

// -- form state: tri-state ids, always-explicit window ------------------------
const startDate = ref(new Date().toISOString().slice(0, 7) + '-01')
const endDate = ref(monthEndIso(new Date()))
const dateType = ref<DateType>('EffectiveDate')
const walletId = ref(0) // 0 = All
const categoryId = ref(-1) // -1 = All, 0 = None, >0 = that one
const counterpartyId = ref(-1)
const order = ref<'Asc' | 'Desc'>('Desc')
const limit = ref('')

function monthEndIso(now: Date): string {
  const end = new Date(Date.UTC(now.getUTCFullYear(), now.getUTCMonth() + 1, 0)) // last day of month
  return end.toISOString().slice(0, 10)
}

const formState = (): SearchFormState => ({
  start: startDate.value === '' ? '' : new Date(startDate.value + 'T00:00:00Z').toISOString(),
  end: endDate.value === '' ? '' : new Date(endDate.value + 'T23:59:59Z').toISOString(),
  dateType: dateType.value,
  walletId: walletId.value,
  categoryId: categoryId.value,
  counterpartyId: counterpartyId.value,
  order: order.value,
  limit: limit.value,
})

// -- search -------------------------------------------------------------------

async function run(): Promise<void> {
  const client = activeClient()
  if (!client) return
  const out = buildSearchQuery(formState())
  if (out.query === null) {
    searchError.value = out.error
    return
  }
  searchError.value = ''
  loading.value = true
  try {
    const txs = await client.searchTransactionsBy(out.query)
    rows.value = resultRows(txs)
    ran.value = true
  } catch (e) {
    session.error.value = errorMessage(e)
  } finally {
    loading.value = false
  }
}

// -- the results are book rows: open, edit, quick actions (M9) -----------------

const editingTx = ref<TransactionResponse | null>(null)
const editingLegId = ref<number | null>(null)
const quickBusy = ref(0)
const quickNotice = ref('')

function openEdit(row: { kind: string; ids: number[]; tx: TransactionResponse }): void {
  quickNotice.value = ''
  if (row.kind === 'transfer') {
    // A transfer is one linked pair — it opens its own form, never the
    // movement form. Either leg id finds the pair
    editingTx.value = null
    editingLegId.value = row.ids[0]
    return
  }
  editingLegId.value = null
  editingTx.value = row.tx
}

function closeForms(): void {
  editingTx.value = null
  editingLegId.value = null
}

async function afterSaved(): Promise<void> {
  closeForms()
  await run()
}

async function quick(name: QuickActionName, tx: TransactionResponse, ev: Event): Promise<void> {
  ev.stopPropagation()
  const client = activeClient()
  if (!client || quickBusy.value !== 0) return
  const action = name === 'payDue' ? payOnDue(tx)
    : name === 'payToday' ? payAsToday(tx, new Date().toISOString())
    : name === 'reverse' ? reverseRow(tx)
    : unreverseRow(tx)
  if (action.request === null && action.transferRequest === null) return
  quickBusy.value = tx.id
  try {
    if (action.request !== null) await client.updateTransaction(tx.id, action.request)
    else await client.updateTransfer(tx.id, action.transferRequest!)
    quickNotice.value = action.movesTo === null
      ? (name === 'reverse' ? 'Reversed.' : 'Saved.')
      : `Saved — the row moved to ${action.movesTo}.`
    await run()
  } catch (e) {
    quickNotice.value = ''
    session.error.value = errorMessage(e)
  } finally {
    quickBusy.value = 0
  }
}

const QUICK_ICON: Record<QuickActionName, string> = {
  payDue: '✓',
  payToday: '✓',
  reverse: '↩',
  unreverse: '↩',
}
const QUICK_LABEL: Record<QuickActionName, string> = {
  payDue: 'Pay on due date',
  payToday: 'Pay as today',
  reverse: 'Reverse',
  unreverse: 'Undo reversal',
}

const walletName = (id: number): string => wallets.value.find(w => w.id === id)?.name ?? '?'
const categoryName = (id: number): string => categories.value.find(c => c.id === id)?.name ?? ''
const personName = (id: number): string => persons.value.find(p => p.id === id)?.name ?? ''

function amountClass(v: number): string {
  return v > 0 ? 'pos' : 'neg'
}

onMounted(async () => {
  try {
    const client = activeClient()
    if (!client) return
    const [w, c, p] = await Promise.all([client.wallets(), client.categories(), client.persons()])
    wallets.value = w
    categories.value = c
    persons.value = p
  } catch (e) {
    session.error.value = errorMessage(e)
  }
})
</script>

<template>
  <section>
    <div class="sec">
      <h2>Search</h2>
    </div>

    <div class="filters card">
      <div class="grid2">
        <label>
          From
          <input v-model="startDate" type="date">
        </label>
        <label>
          To
          <input v-model="endDate" type="date">
        </label>
      </div>
      <div class="grid2">
        <label>
          Which date
          <select v-model="dateType">
            <option value="EffectiveDate">Effective (the honest one)</option>
            <option value="DueDate">Due</option>
            <option value="PaymentDate">Payment</option>
          </select>
        </label>
        <label>
          Order
          <select v-model="order">
            <option value="Desc">Newest first</option>
            <option value="Asc">Oldest first</option>
          </select>
        </label>
      </div>
      <label>
        Wallet
        <select v-model.number="walletId">
          <option :value="0">All wallets</option>
          <option v-for="w in wallets" :key="w.id" :value="w.id" :class="{ del: w.isDeleted }">{{ w.name }}</option>
        </select>
      </label>
      <label>
        Category
        <select v-model.number="categoryId">
          <option :value="-1">All categories</option>
          <option :value="0">No category (transfer legs)</option>
          <option v-for="c in categories" :key="c.id" :value="c.id">{{ c.isExpense ? '− ' : '+ ' }}{{ c.name }}</option>
        </select>
      </label>
      <label>
        Counterparty
        <select v-model.number="counterpartyId">
          <option :value="-1">All</option>
          <option :value="0">No counterparty</option>
          <option v-for="p in persons" :key="p.id" :value="p.id">{{ p.name }}</option>
        </select>
      </label>
      <label>
        Limit <span class="hint">(optional — rows kept)</span>
        <input v-model="limit" type="text" inputmode="numeric" placeholder="e.g. 25">
      </label>
      <p v-if="searchError" class="err">{{ searchError }}</p>
      <div class="row">
        <button class="primary" :disabled="loading" @click="run()">{{ loading ? 'Searching…' : 'Search' }}</button>
      </div>
    </div>

    <p v-if="quickNotice" class="notice">{{ quickNotice }}</p>

    <div v-if="rows.length > 0" class="ledger">
      <div v-for="row in rows" :key="row.ids[0]" class="tx" @click="openEdit(row)">
        <div class="ic">{{ row.kind === 'transfer' ? '⇄' : (row.tx.effectiveValue > 0 ? '↑' : '↓') }}</div>
        <div class="who">
          <b>{{ row.kind === 'transfer' ? `Transfer → ${walletName(row.other ? row.other.walletId : 0)}` : (categoryName(row.tx.categoryId) || row.tx.description) }}</b>
          <span>{{ rowDate(row.tx.effectiveDate) }} · {{ walletName(row.tx.walletId) }}<template v-if="row.kind === 'tx' && row.tx.counterpartyId !== 0"> · {{ personName(row.tx.counterpartyId) }}</template></span>
        </div>
        <div class="quick" v-if="availableActions(row.tx).length > 0">
          <button v-for="a in availableActions(row.tx)" :key="a" class="qbtn" :title="QUICK_LABEL[a]"
            :disabled="quickBusy !== 0" @click="quick(a, row.tx, $event)">{{ QUICK_ICON[a] }}</button>
        </div>
        <div class="amtbox">
          <div class="amt" :class="amountClass(row.tx.effectiveValue)">{{ formatMoney(row.tx.effectiveValue) }}</div>
        </div>
      </div>
    </div>
    <div v-else-if="ran && !loading" class="ledger empty">
      <p>No movements match — loosen a filter and try again.</p>
    </div>

    <MovementForm v-if="editingTx !== null" :wallets="wallets" :categories="categories" :persons="persons"
      :editing="editingTx" :default-wallet-id="editingTx.walletId" @saved="afterSaved" @cancel="closeForms" />
    <TransferForm v-if="editingLegId !== null" :wallets="wallets" :categories="categories"
      :editing-leg-id="editingLegId" :default-wallet-id="wallets[0]?.id ?? 0" @saved="afterSaved" @cancel="closeForms" />
  </section>
</template>

<style scoped>
.sec { display: flex; justify-content: space-between; align-items: center; margin: 1.1rem 0 0.5rem; }
.sec h2 { margin: 0; font-size: 0.72rem; letter-spacing: 0.12em; text-transform: uppercase; color: var(--muted); }

.card { background: var(--surface); border: 1px solid var(--border); border-radius: 14px;
  padding: 1rem 1.1rem; margin-top: 0.9rem; }
label { display: block; font-size: 0.72rem; letter-spacing: 0.1em; text-transform: uppercase;
  color: var(--muted); margin-bottom: 0.8rem; }
label input { display: block; width: 100%; box-sizing: border-box; margin-top: 0.3rem;
  font: inherit; font-size: 0.95rem; text-transform: none; letter-spacing: normal; color: var(--fg);
  background: transparent; border: 1px solid var(--border); border-radius: 10px; padding: 0.55rem 0.7rem; }
label .hint { text-transform: none; letter-spacing: normal; }
select {
  display: block; width: 100%; box-sizing: border-box; margin-top: 0.3rem;
  font: inherit; font-size: 0.95rem; color: var(--fg);
  background: var(--surface); border: 1px solid var(--border); border-radius: 10px; padding: 0.55rem 0.7rem;
}
option.del { opacity: 0.5; }
.grid2 { display: grid; grid-template-columns: 1fr 1fr; gap: 0.6rem; }
.err { color: var(--error); font-size: 0.8rem; margin: 0 0 0.6rem; }
.row { display: flex; gap: 0.6rem; justify-content: flex-end; }
.primary { border: 1px solid var(--accent); border-radius: 999px; background: var(--accent);
  color: var(--on-accent, #fff); font: inherit; font-size: 0.8rem; padding: 0.45rem 1.2rem; cursor: pointer; }
button:disabled { opacity: 0.55; }

.notice { color: var(--accent); font-size: 0.8rem; margin: 0.6rem 0; }

.ledger { background: var(--surface); border: 1px solid var(--border); border-radius: 12px; margin-top: 0.6rem; }
.ledger.empty { padding: 1rem 0.9rem; font-size: 0.85rem; color: var(--muted); }
.tx { display: flex; align-items: center; gap: 0.7rem; padding: 0.65rem 0.5rem; border-bottom: 1px solid var(--border); cursor: pointer; }
.tx:last-child { border-bottom: 0; }
.tx .ic { width: 2rem; height: 2rem; border-radius: 9px; background: var(--accent-soft); display: grid; place-items: center; font-family: var(--mono); font-size: 0.7rem; color: var(--accent-dim, var(--accent)); flex: none; }
.tx .who { flex: 1; min-width: 0; }
.tx .who b { display: block; font-size: 0.88rem; font-weight: 500; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.tx .who span { font-size: 0.72rem; color: var(--muted); font-family: var(--mono); }
.amtbox { text-align: right; }
.amt { font-family: var(--mono); font-variant-numeric: tabular-nums; font-size: 0.9rem; }
.amt.pos { color: var(--success); }
.amt.neg { color: var(--fg); }
.quick { display: flex; gap: 0.3rem; flex: none; }
.qbtn {
  width: 2rem; height: 2rem; border-radius: 9px; border: 1px solid var(--border);
  background: var(--surface); color: var(--accent); font: inherit; font-size: 0.85rem;
  cursor: pointer; display: grid; place-items: center; padding: 0;
}
.qbtn:hover { border-color: var(--accent); }
.qbtn[title='Pay as today'] { font-weight: 700; }
</style>
