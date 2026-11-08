// Book — M6: one wallet, one closed month. The simplest useful shape a
// transaction list can have. "All wallets" rides the same selector, grouped
// by currency, labeled — never a single summed number.

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import type { BookRow } from '../api/book'
import { buildTransactionRequest, rowDate, walletBook } from '../api/book'
import { buildCreateTransferRequest, buildUpdateTransferRequest, pairHeading } from '../api/transfers'
import { activeClient, errorMessage, session } from '../state/session'
import type { CategoryResponse, CreateTransferRequest, PersonResponse, TransactionResponse, UpdateTransferRequest, WalletResponse } from '../api/types'
import { monthFromValue, monthLabel, monthValue, monthWindow, monthWindowOffset } from '../api/months'
import { formatMoney } from '../api/wallets'

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

// -- transfer form state: the pair is edited as one ---------------------------
const transferFormOpen = ref(false)
const transferEditing = ref(false) // false = create, true = edit of the pair
const transferSaving = ref(false)
const transferError = ref('')
const tSourceWalletId = ref(0)
const tDestinationWalletId = ref(0)
const tSourceCategoryId = ref(0)
const tDestinationCategoryId = ref(0)
const tValue = ref('')
const tStatus = ref<'Unpaid' | 'Paid' | 'Reversed'>('Paid')
const tPaidValue = ref('')
const tDescription = ref('')
const tPaymentDetails = ref('')
const tDueDate = ref('')
const tPaymentDate = ref('')
// edit-only: the id of either leg — the update endpoint accepts both
const tEditingLegId = ref<number | null>(null)

// -- data -------------------------------------------------------------------

const window_ = computed(() => monthWindow(monthDate.value))
const label = computed(() => monthLabel(window_.value.start))

const bookRows = computed(() => walletBook(txs.value, walletId.value))

/// Transfer categories are side-locked by the server (source expense,
/// destination income) — the selects only offer legal rows
const expenseCategories = computed(() => categories.value.filter(c => c.isExpense))
const incomeCategories = computed(() => categories.value.filter(c => !c.isExpense))

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
  formOpen.value = true
}

function openEdit(row: { kind: string; ids: number[]; tx: TransactionResponse }): void {
  if (row.kind === 'transfer') {
    // A transfer is one linked pair — it opens its own form, never the
    // movement form. Either leg id finds the pair
    openTransferEdit(row.ids[0])
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

// -- transfer form: create a pair, edit a pair --------------------------------

function openTransferCreate(): void {
  transferEditing.value = false
  tEditingLegId.value = null
  const today = new Date().toISOString().slice(0, 10)
  tSourceWalletId.value = walletId.value === 0 ? (wallets.value[0]?.id ?? 0) : walletId.value
  tDestinationWalletId.value = 0
  tSourceCategoryId.value = 0
  tDestinationCategoryId.value = 0
  tValue.value = ''
  tStatus.value = 'Paid'
  tDescription.value = ''
  tPaymentDetails.value = ''
  tDueDate.value = today
  tPaymentDate.value = today
  transferError.value = ''
  formOpen.value = false
  transferFormOpen.value = true
}

async function openTransferEdit(legId: number): Promise<void> {
  const client = activeClient()
  if (!client) return
  const today = new Date().toISOString().slice(0, 10)
  try {
    const pair = await client.transfer(legId)
    transferEditing.value = true
    tEditingLegId.value = legId
    tSourceWalletId.value = pair.source.walletId
    tDestinationWalletId.value = pair.destination.walletId
    tSourceCategoryId.value = pair.source.categoryId
    tDestinationCategoryId.value = pair.destination.categoryId
    // Stored legs carry the sign — the input is positive, abs() at the border
    tValue.value = String(Math.abs(pair.source.dueValue))
    tStatus.value = pair.source.status as 'Unpaid' | 'Paid' | 'Reversed'
    tDescription.value = pair.source.description
    tPaymentDetails.value = pair.source.paymentDetails ?? ''
    tDueDate.value = (pair.source.dueDate ?? today).slice(0, 10)
    tPaymentDate.value = (pair.source.paymentDate ?? today).slice(0, 10)
    transferError.value = ''
    formOpen.value = false
    transferFormOpen.value = true
    // paid value rides the same input when Paid — filled from the source leg
    if (pair.source.status === 'Paid') tPaidValue.value = String(Math.abs(pair.source.paidValue))
    else tPaidValue.value = ''
  } catch (e) {
    session.error.value = errorMessage(e)
  }
}

function cancelTransfer(): void {
  transferFormOpen.value = false
  tEditingLegId.value = null
}

async function saveTransfer(): Promise<void> {
  const client = activeClient()
  if (!client) return
  const out = transferEditing.value
    ? buildUpdateTransferRequest({
        description: tDescription.value,
        value: tValue.value,
        paidValue: tPaidValue.value,
        status: tStatus.value,
        dueDate: tDueDate.value,
        paymentDate: tPaymentDate.value,
        paymentDetails: tPaymentDetails.value,
      })
    : buildCreateTransferRequest({
        sourceWalletId: tSourceWalletId.value,
        sourceCategoryId: tSourceCategoryId.value,
        destinationWalletId: tDestinationWalletId.value,
        destinationCategoryId: tDestinationCategoryId.value,
        description: tDescription.value,
        value: tValue.value,
        dueDate: tDueDate.value,
        paymentDate: tPaymentDate.value,
        paid: tStatus.value === 'Paid',
        paymentDetails: tPaymentDetails.value,
      })
  if (out.request === null) {
    transferError.value = out.error
    return
  }
  transferSaving.value = true
  try {
    if (transferEditing.value) await client.updateTransfer(tEditingLegId.value!, out.request as UpdateTransferRequest)
    else await client.createTransfer(out.request as CreateTransferRequest)
    transferFormOpen.value = false
    tEditingLegId.value = null
    await loadBook()
  } catch (e) {
    transferError.value = errorMessage(e)
  } finally {
    transferSaving.value = false
  }
}
</script>

<template>
  <section>
    <div class="sec">
      <h2>Book</h2>
      <div class="actions">
        <button class="add" @click="openCreate">＋ New</button>
        <button class="add" @click="openTransferCreate">⇄ Transfer</button>
      </div>
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

    <!-- transfer form: one linked pair, created and edited as one -->
    <div v-if="transferFormOpen" class="form card">
      <h3>{{ transferEditing ? 'Edit transfer' : 'New transfer' }}</h3>

      <template v-if="transferEditing">
        <p class="hintline">{{ pairHeading(walletName(tSourceWalletId), walletName(tDestinationWalletId)) }} — wallets are fixed after creation.</p>
      </template>
      <template v-else>
        <label>
          Source wallet
          <select v-model.number="tSourceWalletId">
            <option v-for="w in wallets" :key="w.id" :value="w.id">{{ w.name }}</option>
          </select>
        </label>
        <label>
          Destination wallet
          <select v-model.number="tDestinationWalletId">
            <option :value="0">— pick one —</option>
            <option v-for="w in wallets" :key="w.id" :value="w.id">{{ w.name }}</option>
          </select>
        </label>
        <div class="grid2">
          <label>
            Source category <span class="hint">(optional)</span>
            <select v-model.number="tSourceCategoryId">
              <option :value="0">— none —</option>
              <option v-for="c in expenseCategories" :key="c.id" :value="c.id">− {{ c.name }}</option>
            </select>
          </label>
          <label>
            Destination category <span class="hint">(optional)</span>
            <select v-model.number="tDestinationCategoryId">
              <option :value="0">— none —</option>
              <option v-for="c in incomeCategories" :key="c.id" :value="c.id">+ {{ c.name }}</option>
            </select>
          </label>
        </div>
      </template>

      <div class="grid2">
        <label>
          Due date
          <input v-model="tDueDate" type="date">
        </label>
        <label>
          Payment date
          <input v-model="tPaymentDate" type="date">
        </label>
      </div>
      <div class="lock">
        <span class="lbl">Status</span>
        <div class="types">
          <button :class="{ on: tStatus === 'Paid' }" @click="tStatus = 'Paid'">Paid</button>
          <button :class="{ on: tStatus === 'Unpaid' }" @click="tStatus = 'Unpaid'">Unpaid</button>
          <button :class="{ on: tStatus === 'Reversed' }" @click="tStatus = 'Reversed'">Reversed</button>
        </div>
      </div>
      <div class="grid2">
        <label>
          Value
          <input v-model="tValue" type="text" inputmode="decimal" placeholder="100">
        </label>
        <label v-if="tStatus === 'Paid'">
          Paid value
          <input v-model="tPaidValue" type="text" inputmode="decimal" placeholder="100">
        </label>
      </div>
      <p class="hintline">Values are positive — the pair carries both sides.</p>
      <label>
        Description
        <input v-model="tDescription" type="text" placeholder="What it was for" autocomplete="off">
      </label>
      <label>
        Payment details <span class="hint">(optional)</span>
        <input v-model="tPaymentDetails" type="text" placeholder="Free text" autocomplete="off">
      </label>
      <p v-if="transferError" class="err">{{ transferError }}</p>
      <div class="row">
        <button class="ghost" :disabled="transferSaving" @click="cancelTransfer">Cancel</button>
        <button class="primary" :disabled="transferSaving" @click="saveTransfer">{{ transferSaving ? 'Saving…' : 'Save' }}</button>
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
.actions { display: flex; gap: 0.4rem; }
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
