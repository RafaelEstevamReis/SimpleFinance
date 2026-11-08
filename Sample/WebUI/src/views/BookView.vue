// Book — M6: one wallet, one closed month. The simplest useful shape a
// transaction list can have. "All wallets" rides the same selector, grouped
// by currency, labeled — never a single summed number.

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import type { BookRow } from '../api/book'
import { buildTransactionRequest, rowDate, walletBook } from '../api/book'
import { activeClient, errorMessage, session } from '../state/session'
import type { CategoryResponse, PersonResponse, TransactionResponse, WalletResponse } from '../api/types'
import { formatMoney } from '../api/wallets'
import { monthFromValue, monthLabel, monthValue, monthWindow, monthWindowOffset } from '../api/months'

const wallets = ref<WalletResponse[]>([])
const categories = ref<CategoryResponse[]>([])
const persons = ref<PersonResponse[]>([])
const txs = ref<TransactionResponse[]>([])

const loading = ref(true)
const walletId = ref(0) // 0 = all wallets
const monthDate = ref(new Date())
const formOpen = ref(false)
const editingId = ref<number | null>(null)
const saving = ref(false)
const formError = ref('')
const transferNotice = ref('')

// -- form state -------------------------------------------------------------
const formWalletId = ref(0)
const formCategoryId = ref(0)
const formCounterpartyId = ref(0)
const formDueDate = ref('')
const formPaymentDate = ref('')
const formDescription = ref('')
const formStatus = ref<'Unpaid' | 'Paid' | 'Reversed'>('Paid')
const formDueValue = ref('')
const formPaidValue = ref('')
const formPaymentDetails = ref('')

// -- data -------------------------------------------------------------------

const window_ = computed(() => monthWindow(monthDate.value))
const label = computed(() => monthLabel(window_.value.start))

const bookRows = computed(() => walletBook(txs.value, walletId.value))

/// All-wallets view groups by currency; each group is its own list with its
/// own Net. Different currencies are never summed — a wallet reports in
/// its own money, and that is all
const groups = computed(() => {
  const byCurrency = new Map<string, { rows: BookRow[]; net: number }>()
  for (const w of wallets.value) {
    if (w.isDeleted) continue
    const rows = walletBook(txs.value, w.id)
    const g = byCurrency.get(w.baseCurrency) ?? { rows: [], net: 0 }
    g.rows.push(...rows)
    g.net += rows.at(-1)?.running ?? 0
    byCurrency.set(w.baseCurrency, g)
  }
  return [...byCurrency.entries()].map(([currency, g]) => ({ currency, rows: g.rows, net: g.net }))
})

const netOfView = computed(() => bookRows.value.at(-1)?.running ?? 0)

const walletName = (id: number): string => wallets.value.find(w => w.id === id)?.name ?? '?'
const categoryName = (id: number): string => categories.value.find(c => c.id === id)?.name ?? ''
const personName = (id: number): string => persons.value.find(p => p.id === id)?.name ?? ''

/// Amount with sign as stored; the demo shows expenses uncolored, income green
function amountClass(v: number): string {
  return v > 0 ? 'pos' : 'neg'
}

// -- loading ----------------------------------------------------------------

async function load(): Promise<void> {
  const client = activeClient()
  if (!client) return
  loading.value = true
  try {
    const [w, c, p] = await Promise.all([client.wallets(), client.categories(), client.persons()])
    wallets.value = w
    categories.value = c
    persons.value = p
    if (walletId.value === 0 && w.length > 0) walletId.value = w[0].id
    await loadBook()
  } catch (e) {
    session.error.value = errorMessage(e)
  } finally {
    loading.value = false
  }
}

async function loadBook(): Promise<void> {
  const client = activeClient()
  if (!client) return
  const w = window_.value
  // One unfiltered call for the whole month: pairing must see both legs of
  // every transfer, and each view picks its own rows from the full book
  txs.value = await client.searchTransactionsBy({ start: w.start, end: w.end, dateType: 'EffectiveDate', order: 'Asc' })
}

function shiftMonth(offset: number): void {
  monthDate.value = new Date(monthWindowOffset(monthDate.value, offset).start)
}

function jumpTo(value: string): void {
  if (value === '') return
  monthDate.value = monthFromValue(value)
}

watch(walletId, () => { loadBook().catch(e => { session.error.value = errorMessage(e) }) })
watch(monthDate, () => { loadBook().catch(e => { session.error.value = errorMessage(e) }) })

onMounted(load)

// -- form -------------------------------------------------------------------

function openCreate(): void {
  editingId.value = null
  formWalletId.value = walletId.value === 0 ? (wallets.value[0]?.id ?? 0) : walletId.value
  formCategoryId.value = 0
  formCounterpartyId.value = 0
  const today = new Date().toISOString().slice(0, 10)
  formDueDate.value = today
  formPaymentDate.value = today
  formDescription.value = ''
  formStatus.value = 'Paid'
  formDueValue.value = ''
  formPaidValue.value = ''
  formPaymentDetails.value = ''
  formError.value = ''
  transferNotice.value = ''
  formOpen.value = true
}

function openEdit(row: { kind: string; ids: number[]; tx: TransactionResponse }): void {
  if (row.kind === 'transfer') {
    // Transfers are edited as the linked pair, which is M7's business — the
    // book shows them, it does not edit them yet
    transferNotice.value = 'Transfers are edited as a pair — arriving with the transfer screen.'
    formOpen.value = false
    return
  }
  const tx = row.tx
  editingId.value = tx.id
  formWalletId.value = tx.walletId
  formCategoryId.value = tx.categoryId
  formCounterpartyId.value = tx.counterpartyId
  formDueDate.value = tx.dueDate.slice(0, 10)
  formPaymentDate.value = tx.paymentDate.slice(0, 10)
  formDescription.value = tx.description
  formStatus.value = tx.status as 'Unpaid' | 'Paid' | 'Reversed'
  // Stored values carry the sign — the input is positive, abs() at the border
  formDueValue.value = String(Math.abs(tx.dueValue))
  formPaidValue.value = String(Math.abs(tx.paidValue))
  formPaymentDetails.value = tx.paymentDetails ?? ''
  formError.value = ''
  transferNotice.value = ''
  formOpen.value = true
}

function cancelEdit(): void {
  formOpen.value = false
  editingId.value = null
}

async function save(): Promise<void> {
  const client = activeClient()
  if (!client) return
  const out = buildTransactionRequest({
    walletId: formWalletId.value,
    categoryId: formCategoryId.value,
    counterpartyId: formCounterpartyId.value,
    dueDate: formDueDate.value,
    paymentDate: formPaymentDate.value,
    description: formDescription.value,
    status: formStatus.value,
    dueValue: formDueValue.value,
    paidValue: formPaidValue.value,
    paymentDetails: formPaymentDetails.value,
  })
  if (out.request === null) {
    formError.value = out.error
    return
  }
  saving.value = true
  try {
    if (editingId.value === null) await client.createTransaction(out.request)
    else await client.updateTransaction(editingId.value, out.request)
    formOpen.value = false
    editingId.value = null
    await loadBook()
  } catch (e) {
    formError.value = errorMessage(e)
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <section>
    <div class="sec">
      <h2>Book</h2>
      <button class="add" @click="openCreate">＋ New</button>
    </div>

    <div class="bar">
      <label class="wl">
        Wallet
        <select v-model.number="walletId">
          <option v-for="w in wallets" :key="w.id" :value="w.id" :class="{ del: w.isDeleted }">{{ w.name }}</option>
          <option :value="0">All wallets</option>
        </select>
      </label>
      <div class="months">
        <button class="ghost" @click="shiftMonth(-1)">‹</button>
        <label class="ml">
          <input type="month" :value="monthValue(window_.start)" @change="jumpTo(($event.target as HTMLInputElement).value)">
          <span class="mlabel">{{ label }}</span>
        </label>
        <button class="ghost" @click="shiftMonth(1)">›</button>
      </div>
    </div>

    <p v-if="transferNotice" class="notice">{{ transferNotice }}</p>
    <p v-if="loading" class="muted">Loading…</p>

    <!-- one wallet: one flat list -->
    <template v-if="walletId !== 0">
      <p class="muted net">Net of the month: <b>{{ formatMoney(netOfView) }}</b></p>
      <div v-if="bookRows.length > 0" class="ledger">
        <div v-for="row in bookRows" :key="row.ids[0]" class="tx" @click="openEdit(row)">
          <div class="ic">{{ row.kind === 'transfer' ? '⇄' : (row.tx.effectiveValue > 0 ? '↑' : '↓') }}</div>
          <div class="who">
            <b>{{ row.kind === 'transfer' ? `Transfer → ${walletName(row.other!.walletId)}` : (categoryName(row.tx.categoryId) || row.tx.description) }}</b>
            <span>{{ rowDate(row.tx.effectiveDate) }} · {{ walletName(row.tx.walletId) }}<template v-if="row.kind === 'tx' && row.tx.counterpartyId !== 0"> · {{ personName(row.tx.counterpartyId) }}</template></span>
          </div>
          <div class="amtbox">
            <div class="amt" :class="amountClass(row.tx.effectiveValue)">{{ formatMoney(row.tx.effectiveValue) }}</div>
            <div class="run">Net {{ formatMoney(row.running) }}</div>
          </div>
        </div>
      </div>
      <div v-else class="ledger empty">
        <p>No movements in this month.</p>
      </div>
    </template>

    <!-- all wallets: grouped by currency, each with its own Net -->
    <template v-else>
      <p class="muted small">Grouped by currency — different currencies are never summed.</p>
      <template v-for="g in groups" :key="g.currency">
        <div class="sec"><h2>{{ g.currency }}</h2></div>
        <p class="muted net">Net of the month: <b>{{ formatMoney(g.net) }}</b></p>
        <div v-if="g.rows.length > 0" class="ledger">
          <div v-for="row in g.rows" :key="row.ids[0]" class="tx" @click="openEdit(row)">
            <div class="ic">{{ row.kind === 'transfer' ? '⇄' : (row.tx.effectiveValue > 0 ? '↑' : '↓') }}</div>
            <div class="who">
              <b>{{ row.kind === 'transfer' ? `Transfer → ${walletName(row.other!.walletId)}` : (categoryName(row.tx.categoryId) || row.tx.description) }}</b>
              <span>{{ rowDate(row.tx.effectiveDate) }} · {{ walletName(row.tx.walletId) }}</span>
            </div>
            <div class="amtbox">
              <div class="amt" :class="amountClass(row.tx.effectiveValue)">{{ formatMoney(row.tx.effectiveValue) }}</div>
            </div>
          </div>
        </div>
        <div v-else class="ledger empty">
          <p>No movements in this currency this month.</p>
        </div>
      </template>
    </template>

    <div v-if="formOpen" class="form card">
      <h3>{{ editingId === null ? 'New movement' : 'Edit movement' }}</h3>

      <label>
        Wallet
        <select v-model.number="formWalletId">
          <option v-for="w in wallets" :key="w.id" :value="w.id">{{ w.name }}</option>
        </select>
      </label>
      <label>
        Category
        <select v-model.number="formCategoryId">
          <option :value="0">— pick one —</option>
          <option v-for="c in categories" :key="c.id" :value="c.id">{{ c.isExpense ? '− ' : '+ ' }}{{ c.name }}</option>
        </select>
      </label>
      <label>
        Counterparty <span class="hint">(optional)</span>
        <select v-model.number="formCounterpartyId">
          <option :value="0">— none —</option>
          <option v-for="p in persons" :key="p.id" :value="p.id">{{ p.name }}</option>
        </select>
      </label>
      <div class="grid2">
        <label>
          Due date
          <input v-model="formDueDate" type="date">
        </label>
        <label>
          Payment date
          <input v-model="formPaymentDate" type="date">
        </label>
      </div>
      <div class="lock">
        <span class="lbl">Status</span>
        <div class="types">
          <button :class="{ on: formStatus === 'Paid' }" @click="formStatus = 'Paid'">Paid</button>
          <button :class="{ on: formStatus === 'Unpaid' }" @click="formStatus = 'Unpaid'">Unpaid</button>
          <button :class="{ on: formStatus === 'Reversed' }" @click="formStatus = 'Reversed'">Reversed</button>
        </div>
      </div>
      <div class="grid2">
        <label>
          Due value
          <input v-model="formDueValue" type="text" inputmode="decimal" placeholder="100">
        </label>
        <label v-if="formStatus === 'Paid'">
          Paid value
          <input v-model="formPaidValue" type="text" inputmode="decimal" placeholder="100">
        </label>
      </div>
      <p class="hintline">Values are positive — the category decides the side.</p>
      <label>
        Description
        <input v-model="formDescription" type="text" placeholder="What it was" autocomplete="off">
      </label>
      <label>
        Payment details <span class="hint">(optional)</span>
        <input v-model="formPaymentDetails" type="text" placeholder="Free text" autocomplete="off">
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
.muted.small { font-size: 0.75rem; }
.muted.net { margin: 0.4rem 0; font-size: 0.8rem; }
.muted.net b { font-family: var(--mono); font-variant-numeric: tabular-nums; color: var(--fg); }
.notice { color: var(--accent); font-size: 0.8rem; margin: 0.6rem 0; }

.bar { display: flex; flex-direction: column; gap: 0.6rem; margin-bottom: 0.6rem; }
.bar .wl { flex: 1; }
select {
  display: block; width: 100%; box-sizing: border-box; margin-top: 0.3rem;
  font: inherit; font-size: 0.95rem; color: var(--fg);
  background: var(--surface); border: 1px solid var(--border); border-radius: 10px; padding: 0.55rem 0.7rem;
}
option.del { opacity: 0.5; }

.months { display: flex; align-items: end; gap: 0.5rem; }
.months .ghost { padding: 0.45rem 0.8rem; }
.ml { position: relative; flex: 1; }
.ml input {
  width: 100%; box-sizing: border-box; font: inherit; font-size: 0.95rem; color: var(--fg);
  background: var(--surface); border: 1px solid var(--border); border-radius: 10px; padding: 0.55rem 0.7rem;
}
.mlabel {
  position: absolute; left: 0.8rem; top: 0.35rem; font-size: 0.72rem; color: var(--muted);
  pointer-events: none; font-family: var(--mono);
}

.ledger { background: var(--surface); border: 1px solid var(--border); border-radius: 12px; }
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
.run { font-size: 0.62rem; color: var(--muted); font-family: var(--mono); font-variant-numeric: tabular-nums; }

.card { background: var(--surface); border: 1px solid var(--border); border-radius: 14px;
  padding: 1rem 1.1rem; margin-top: 0.9rem; }
h3 { margin: 0 0 0.8rem; font-size: 0.95rem; }
label { display: block; font-size: 0.72rem; letter-spacing: 0.1em; text-transform: uppercase;
  color: var(--muted); margin-bottom: 0.8rem; }
label input { display: block; width: 100%; box-sizing: border-box; margin-top: 0.3rem;
  font: inherit; font-size: 0.95rem; text-transform: none; letter-spacing: normal; color: var(--fg);
  background: transparent; border: 1px solid var(--border); border-radius: 10px; padding: 0.55rem 0.7rem; }
label .hint { text-transform: none; letter-spacing: normal; }
.grid2 { display: grid; grid-template-columns: 1fr 1fr; gap: 0.6rem; }
.lock { margin-bottom: 0.8rem; }
.lock .lbl { display: block; font-size: 0.72rem; letter-spacing: 0.1em; text-transform: uppercase; color: var(--muted); margin-bottom: 0.3rem; }
.types { display: flex; gap: 0.5rem; flex-wrap: wrap; }
.types button { border: 1px solid var(--border); border-radius: 999px; background: transparent;
  color: var(--fg); font: inherit; font-size: 0.8rem; padding: 0.4rem 0.85rem; cursor: pointer; }
.types button.on { background: var(--accent-soft); border-color: var(--accent); color: var(--accent); }
.hintline { font-size: 0.72rem; color: var(--muted); margin: 0 0 0.8rem; }
.err { color: var(--error); font-size: 0.8rem; margin: 0 0 0.6rem; }
.row { display: flex; gap: 0.6rem; justify-content: flex-end; }
.ghost { border: 1px solid var(--border); border-radius: 999px; background: transparent;
  color: var(--fg); font: inherit; font-size: 0.8rem; padding: 0.45rem 1rem; cursor: pointer; }
.primary { border: 1px solid var(--accent); border-radius: 999px; background: var(--accent);
  color: var(--on-accent, #fff); font: inherit; font-size: 0.8rem; padding: 0.45rem 1.2rem; cursor: pointer; }
button:disabled { opacity: 0.55; }
</style>
