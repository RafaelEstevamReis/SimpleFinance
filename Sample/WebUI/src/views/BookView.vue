// Book — M6: one wallet, one closed month. The simplest useful shape a
// transaction list can have. "All wallets" rides the same selector, grouped
// by currency, labeled — never a single summed number.

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import type { BookRow } from '../api/book'
import { rowDate, walletBook } from '../api/book'
import { availableActions, payAsToday, payOnDue, reverseRow, unreverseRow, type QuickActionName } from '../api/actions'
import { activeClient, errorMessage, session } from '../state/session'
import type { CategoryResponse, PersonResponse, TransactionResponse, WalletResponse } from '../api/types'
import { monthFromValue, monthLabel, monthValue, monthWindow, monthWindowOffset } from '../api/months'
import { formatMoney } from '../api/wallets'
import MovementForm from '../components/MovementForm.vue'
import TransferForm from '../components/TransferForm.vue'

const wallets = ref<WalletResponse[]>([])
const categories = ref<CategoryResponse[]>([])
const persons = ref<PersonResponse[]>([])
const txs = ref<TransactionResponse[]>([])

const loading = ref(true)
const walletId = ref(0) // 0 = all wallets
const monthDate = ref(new Date())
const editingTx = ref<TransactionResponse | null>(null) // a row = edit; null = closed
const editingLegId = ref<number | null>(null) // transfer pair: id of either leg
const creatingTransfer = ref(false)
const formOpen = ref(false) // the create form (edit rides editingTx)
const formCreateArm = ref(false) // toggled per open: re-mounts the form clean

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

// -- form: the movement/transfer forms are shared components (M10) -----------

function openCreate(): void {
  editingLegId.value = null
  creatingTransfer.value = false
  editingTx.value = null
  // MovementForm with editing=null is "create"; it reads defaultWalletId
  // at mount, so toggling the ref re-arms it
  formCreateArm.value = !formCreateArm.value
  formOpen.value = true
}

function openEdit(row: { kind: string; ids: number[]; tx: TransactionResponse }): void {
  quickNotice.value = ''
  formOpen.value = false
  creatingTransfer.value = false
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
  creatingTransfer.value = false
  formOpen.value = false
}

async function afterSaved(): Promise<void> {
  closeForms()
  await loadBook()
}

function openTransferCreate(): void {
  editingTx.value = null
  editingLegId.value = null
  creatingTransfer.value = true
}

// -- transfer form: create a pair, edit a pair --------------------------------


// -- quick actions (M9) -------------------------------------------------------

const quickBusy = ref(0)
const quickNotice = ref('')

/// Fire a quick action on a row's own leg — inline, no form. The builder
/// decides the endpoint (transfer legs cross the pair); the view only
/// reports where the money went and reloads the honest book
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
    await loadBook()
  } catch (e) {
    quickNotice.value = ''
    session.error.value = errorMessage(e)
  } finally {
    quickBusy.value = 0
  }
}

/// Quick-action glyphs and titles — three power moves on a due row
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
      <p v-if="quickNotice !== ''" class="notice">{{ quickNotice }}</p>
      <p class="muted net">Net of the month: <b>{{ formatMoney(netOfView) }}</b></p>
      <div v-if="bookRows.length > 0" class="ledger">
        <div v-for="row in bookRows" :key="row.ids[0]" class="tx" @click="openEdit(row)">
          <div class="ic">{{ row.kind === 'transfer' ? '⇄' : (row.tx.effectiveValue > 0 ? '↑' : '↓') }}</div>
          <div class="who">
            <b>{{ row.kind === 'transfer' ? `Transfer → ${walletName(row.other!.walletId)}` : (categoryName(row.tx.categoryId) || row.tx.description) }}</b>
            <span>{{ rowDate(row.tx.effectiveDate) }} · {{ walletName(row.tx.walletId) }}<template v-if="row.kind === 'tx' && row.tx.counterpartyId !== 0"> · {{ personName(row.tx.counterpartyId) }}</template></span>
          </div>
          <div class="quick" v-if="availableActions(row.tx).length > 0">
            <button v-for="a in availableActions(row.tx)" :key="a" class="qbtn" :title="QUICK_LABEL[a]"
              :disabled="quickBusy !== 0" @click="quick(a, row.tx, $event)">{{ QUICK_ICON[a] }}</button>
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
            <div class="quick" v-if="availableActions(row.tx).length > 0">
              <button v-for="a in availableActions(row.tx)" :key="a" class="qbtn" :title="QUICK_LABEL[a]"
                :disabled="quickBusy !== 0" @click="quick(a, row.tx, $event)">{{ QUICK_ICON[a] }}</button>
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

    <MovementForm v-if="formOpen" :key="'create-' + formCreateArm" :wallets="wallets" :categories="categories"
      :persons="persons" :editing="null" :default-wallet-id="walletId === 0 ? (wallets[0]?.id ?? 0) : walletId"
      @saved="afterSaved" @cancel="closeForms" />

    <MovementForm v-if="editingTx !== null" :key="'edit-' + editingTx.id" :wallets="wallets" :categories="categories"
      :persons="persons" :editing="editingTx" :default-wallet-id="editingTx.walletId"
      @saved="afterSaved" @cancel="closeForms" />

    <!-- transfer form: one linked pair, created and edited as one -->
    <TransferForm v-if="editingLegId !== null || creatingTransfer" :wallets="wallets" :categories="categories"
      :editing-leg-id="editingLegId" :default-wallet-id="walletId === 0 ? (wallets[0]?.id ?? 0) : walletId"
      @saved="afterSaved" @cancel="closeForms" />
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
  width: 100%; box-sizing: border-box; font: inherit; font-size: 0.95rem;
  /* the native field text ("November 2026") is hidden on purpose: the
     .mlabel below is this field's value. Without this, both render and
     the month appears twice in the same box */
  color: transparent;
  background: var(--surface); border: 1px solid var(--border); border-radius: 10px; padding: 0.55rem 0.7rem;
}
.mlabel {
  position: absolute; left: 0.8rem; top: 50%; transform: translateY(-50%);
  font-size: 0.95rem; color: var(--fg); font-family: var(--mono);
  pointer-events: none;
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

.ghost { border: 1px solid var(--border); border-radius: 999px; background: transparent;
  color: var(--fg); font: inherit; font-size: 0.8rem; padding: 0.45rem 1rem; cursor: pointer; }
.quick { display: flex; gap: 0.3rem; flex: none; }
.qbtn {
  width: 2rem; height: 2rem; border-radius: 9px; border: 1px solid var(--border);
  background: var(--surface); color: var(--accent); font: inherit; font-size: 0.85rem;
  cursor: pointer; display: grid; place-items: center; padding: 0;
}
.qbtn:hover { border-color: var(--accent); }
.qbtn[title='Pay as today'] { font-weight: 700; }
button:disabled { opacity: 0.55; }
</style>
