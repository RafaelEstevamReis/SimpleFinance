// Home — the demo5 dashboard shape. Wallets and net worth are real (M4);
// categories and activity keep their designed empty states until M5/M6.
// Net worth follows the cross-currency rule: one currency sums, two or
// more show no number rather than a made-up one.

<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { activeClient, errorMessage, session } from '../state/session'
import type { CategoryResponse, TransactionResponse, WalletBalanceResponse, WalletResponse } from '../api/types'
import { budgetNote, budgetTrack, monthlyBudgetRow, sumSpentByCategory } from '../api/categories'
import { monthWindow, monthWindowOffset } from '../api/months'
import { formatMoney, liveBalances, netWorthLabel, sumByCurrency } from '../api/wallets'
import { chartBars, monthSeries, recentRows, upcomingDue } from '../api/dashboard'
import { rowDate } from '../api/book'
import { availableActions, payAsToday, payOnDue, reverseRow, unreverseRow, type QuickActionName } from '../api/actions'

const router = useRouter()
const wallets = ref<WalletResponse[]>([])
const balances = ref<WalletBalanceResponse[]>([])
const categories = ref<CategoryResponse[]>([])
const spent = ref<Map<number, number>>(new Map())
const transactions = ref<TransactionResponse[]>([])
const loaded = ref(false)

/// Months the chart spans — the current one plus the two before it
const CHART_MONTHS = 3

async function load(): Promise<void> {
  const client = activeClient()
  if (!client) return
  try {
    const [w, b] = await Promise.all([client.wallets(), client.walletBalances()])
    wallets.value = w
    balances.value = liveBalances(w, b)
    // one wide search feeds everything: month spend, recent, due, chart.
    // From the first day of the oldest chart month to the end of this one
    const from = monthWindowOffset(new Date(), -CHART_MONTHS + 1)
    const now = monthWindow()
    const [cats, txs] = await Promise.all([
      client.categories(),
      client.searchTransactions({ start: from.start, end: now.end, dateType: 'EffectiveDate', order: 'Asc' }),
    ])
    categories.value = cats
    transactions.value = txs
    spent.value = sumSpentByCategory(txs)
  } catch (e) {
    session.error.value = errorMessage(e)
  } finally {
    loaded.value = true
  }
}

onMounted(() => void load())

// -- quick actions on the due list (M9) — same builders the book uses ------

const quickBusy = ref(0)
const quickNotice = ref('')

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

/// Fire a due action without leaving the dashboard; one load refreshes
/// spend, chart, upcoming and recent together — they share the search
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
    await load()
  } catch (e) {
    quickNotice.value = ''
    session.error.value = errorMessage(e)
  } finally {
    quickBusy.value = 0
  }
}

const net = computed<string>(() => netWorthLabel(balances.value))
const netNote = computed<string>(() => {
  const currencies = sumByCurrency(balances.value).size
  if (!loaded.value) return 'Loading…'
  if (currencies === 0) return 'No wallets yet'
  if (currencies === 1) return 'Sum of every wallet, one currency'
  return `Balances in ${currencies} currencies — no single total`
})
const budgetRow = computed<string>(() => monthlyBudgetRow(spent.value, categories.value))

function balanceOf(walletId: number): number | null {
  return balances.value.find(b => b.walletId === walletId)?.balance ?? null
}

function trackOf(category: CategoryResponse): { pct: number; over: boolean } | null {
  if (!category.isExpense || category.monthlyBudget <= 0) return null
  return budgetTrack(spent.value.get(category.id) ?? 0, category.monthlyBudget)
}

function spentOf(category: CategoryResponse): number {
  return spent.value.get(category.id) ?? 0
}

// -- dashboard: chart, upcoming, recent ------------------------------------

/// The chart never sums across currencies — one tab per currency, the
/// first wallet's by default
const chartCurrencies = computed<string[]>(() => {
  const set = new Set<string>()
  for (const w of wallets.value) set.add(w.baseCurrency)
  return [...set]
})
const chartCurrency = ref<string | null>(null)
const activeChartCurrency = computed<string>(() => {
  if (chartCurrency.value && chartCurrencies.value.includes(chartCurrency.value)) return chartCurrency.value
  return chartCurrencies.value[0] ?? ''
})

function walletCurrency(id: number): string | null {
  return wallets.value.find(w => w.id === id)?.baseCurrency ?? null
}

/// Transactions the charted currency owns — the wallet's base currency
/// is the money a transaction speaks, since PaymentCurrency must equal it
const chartTxs = computed<TransactionResponse[]>(() => {
  const cur = activeChartCurrency.value
  if (!cur) return []
  return transactions.value.filter((t) => walletCurrency(t.walletId) === cur)
})

const series = computed(() => {
  const first = monthWindowOffset(new Date(), -CHART_MONTHS + 1)
  const firstDay = new Date(first.start)
  return monthSeries(chartTxs.value, firstDay, CHART_MONTHS)
})
const CHART_W = 320
const CHART_H = 150
/// The svg is born at the card's real width (M12 tablet steps) — bars are
/// drawn for the size they render at, never stretched. 320 is the floor:
/// below that the phone column keeps its pin.
const chartW = ref(CHART_W)
const chartBox = ref<HTMLElement | null>(null)
let chartObserver: ResizeObserver | null = null
onMounted(() => {
  chartObserver = new ResizeObserver(() => {
    const w = chartBox.value?.clientWidth ?? CHART_W
    chartW.value = Math.max(CHART_W, Math.round(w))
  })
  if (chartBox.value) chartObserver.observe(chartBox.value)
})
onBeforeUnmount(() => chartObserver?.disconnect())
const geometry = computed(() => chartBars(series.value, chartW.value, CHART_H))

/// One line per movement — transfers paired first, the M6 rule
const recent = computed(() => recentRows(transactions.value, 5, rowDate))
const upcoming = computed(() => upcomingDue(transactions.value, new Date(), 5, rowDate))

function walletName(id: number): string {
  return wallets.value.find(w => w.id === id)?.name ?? '…'
}

function rowTitle(tx: TransactionResponse, other?: TransactionResponse): string {
  if (tx.type === 'WalletTransfer' && other) return `Transfer → ${walletName(other.walletId)}`
  if (tx.categoryId > 0) return categories.value.find(c => c.id === tx.categoryId)?.name ?? '…'
  return tx.description
}

function rowAmount(tx: TransactionResponse): { text: string; cls: string } {
  const text = `${tx.effectiveValue < 0 ? '−' : '+'}${formatMoney(Math.abs(tx.effectiveValue))}`
  return { text, cls: tx.effectiveValue < 0 ? 'neg' : 'pos' }
}
</script>

<template>
  <section>
    <div class="dgrid">
    <div class="balance">
      <div class="lbl">Net worth</div>
      <div class="val" :class="{ muted: net === '—' }">{{ net }}</div>
      <div class="delta muted">{{ netNote }}</div>
      <div v-if="budgetRow" class="brow">
        <span class="lbl">Budget</span>
        <span class="bval">{{ budgetRow }}</span>
      </div>
    </div>

    <div class="chartcard">
      <div class="sec chartsec">
        <h2>Monthly</h2>
        <div v-if="chartCurrencies.length > 1" class="curtabs">
          <button
            v-for="c in chartCurrencies" :key="c"
            :class="{ on: c === activeChartCurrency }"
            @click="chartCurrency = c"
          >{{ c }}</button>
        </div>
      </div>
      <div ref="chartBox" class="chartwrap">
        <svg v-if="geometry.bars.length > 0" class="chart" :viewBox="`0 0 ${chartW} ${CHART_H}`" role="img" aria-label="Income and expense by month">
          <line x1="0" :y1="geometry.baseY" :x2="chartW" :y2="geometry.baseY" class="baseline" />
          <template v-for="(bar, i) in geometry.bars" :key="i">
            <rect :x="bar.x" :y="bar.y" :width="bar.width" :height="bar.height" :class="bar.side" rx="1.5" />
            <text v-if="bar.topLabel !== null" class="toplabel" :x="bar.x + 0.6" :y="bar.y - 4">{{ bar.topLabel }}</text>
          </template>
          <text v-for="(p, i) in series" :key="p.label + i" class="axis" :x="(i + 0.5) * (chartW / series.length)" :y="CHART_H - 3">{{ p.label }}</text>
        </svg>
        <div v-else class="chart empty">No movement this quarter yet</div>
      </div>
    </div>


    <div class="panel pw">
      <div class="sec"><h2>Wallets</h2><a @click="router.push({ name: 'wallets' })">manage →</a></div>
      <table v-if="wallets.length > 0">
        <tr><th>Wallet</th><th>Currency</th><th class="num">Balance</th></tr>
        <tbody>
          <tr v-for="w in wallets" :key="w.id" :class="{ del: w.isDeleted }">
            <td>{{ w.name }}</td>
            <td>{{ w.baseCurrency }}</td>
            <td class="num" :class="{ neg: (balanceOf(w.id) ?? 0) < 0 }">
              {{ balanceOf(w.id) === null ? '…' : formatMoney(balanceOf(w.id) as number) }}
            </td>
          </tr>
        </tbody>
      </table>
      <div v-else class="ledger empty">
        <p>No wallets yet — create the first one from the Wallets screen.</p>
      </div>
    </div>

    <div class="panel pc">
      <div class="sec"><h2>Categories</h2><a @click="router.push({ name: 'categories' })">manage →</a></div>
      <div v-if="categories.length > 0" class="ledger">
        <div
          v-for="c in categories"
          :key="c.id"
          class="walletline"
          :class="{ del: c.isDeleted }"
          @click="router.push({ name: 'categories' })"
        >
          <span class="name">{{ c.name }}</span>
          <div v-if="trackOf(c)" class="track">
            <div class="fill" :class="{ over: trackOf(c)?.over }" :style="{ width: `${trackOf(c)?.pct}%` }"></div>
          </div>
          <div v-else class="track none"></div>
          <b :class="{ neg: trackOf(c)?.over }">
            {{ formatMoney(spentOf(c)) }}<span v-if="budgetNote(c.monthlyBudget)" class="of">{{ budgetNote(c.monthlyBudget) }}</span>
          </b>
        </div>
      </div>
      <div v-else class="ledger empty">
        <p>No categories yet — create the first one from the Categories screen.</p>
      </div>
    </div>

    <div class="panel pu">
      <div class="sec"><h2>Upcoming</h2><a @click="router.push({ name: 'book' })">book →</a></div>
      <p v-if="quickNotice !== ''" class="notice">{{ quickNotice }}</p>
      <div v-if="upcoming.length > 0" class="ledger">
        <div v-for="r in upcoming" :key="r.tx.id" class="tx" @click="router.push({ name: 'book' })">
          <div class="ic">{{ r.kind === 'transfer' ? '⇄' : '↓' }}</div>
          <div class="who">
            <b :class="{ overdue: r.overdueBy > 0 }">{{ rowTitle(r.tx) }}</b>
            <span>{{ r.date }} · {{ walletName(r.tx.walletId) }}</span>
          </div>
          <div class="quick" v-if="availableActions(r.tx).length > 0">
            <button v-for="a in availableActions(r.tx)" :key="a" class="qbtn" :title="QUICK_LABEL[a]"
              :disabled="quickBusy !== 0" @click="quick(a, r.tx, $event)">{{ QUICK_ICON[a] }}</button>
          </div>
          <div class="amt" :class="rowAmount(r.tx).cls">{{ rowAmount(r.tx).text }}</div>
        </div>
      </div>
      <div v-else class="ledger empty">
        <p>Nothing due — the book keeps every unpaid row here.</p>
      </div>
    </div>

    <div class="panel pr">
      <div class="sec"><h2>Recent</h2><a @click="router.push({ name: 'book' })">all →</a></div>
      <div v-if="recent.length > 0" class="ledger">
        <div v-for="r in recent" :key="r.ids[0]" class="tx" @click="router.push({ name: 'book' })">
          <div class="ic">{{ r.kind === 'transfer' ? '⇄' : (r.tx.effectiveValue < 0 ? '↓' : '↑') }}</div>
          <div class="who">
            <b>{{ rowTitle(r.tx, r.other) }}</b>
            <span>{{ r.date }} · {{ walletName(r.tx.walletId) }}</span>
          </div>
          <div class="amt" :class="rowAmount(r.tx).cls">{{ rowAmount(r.tx).text }}</div>
        </div>
      </div>
      <div v-else class="ledger empty">
        <p>No movement yet — the book opens with the first transaction.</p>
      </div>
    </div>
    </div>
  </section>
</template>

<style scoped>
.balance {
  background: linear-gradient(135deg, var(--surface), var(--accent-soft));
  border: 1px solid var(--border); border-radius: 14px;
  padding: 1.2rem 1.1rem; margin: 0.9rem 0;
}
.balance .lbl { font-size: 0.68rem; letter-spacing: 0.12em; text-transform: uppercase; color: var(--muted); }
.balance .val { font-family: 'SF Mono', ui-monospace, Consolas, monospace; font-size: 2.1rem; margin: 0.25rem 0; font-variant-numeric: tabular-nums; }
.balance .delta { font-family: ui-monospace, Consolas, monospace; font-size: 0.78rem; color: var(--success); }
.balance .brow { display: flex; justify-content: space-between; align-items: baseline; gap: 1rem; margin-top: 0.6rem; }
.balance .bval { font-family: ui-monospace, Consolas, monospace; font-size: 0.85rem; font-variant-numeric: tabular-nums; }
.muted { color: var(--muted); }
.sec { display: flex; justify-content: space-between; align-items: center; margin: 1.1rem 0 0.5rem; }
.sec h2 { margin: 0; font-size: 0.72rem; letter-spacing: 0.12em; text-transform: uppercase; color: var(--muted); }
.sec a { color: var(--accent); font-size: 0.75rem; text-decoration: none; cursor: pointer; }
.ledger {
  background: var(--surface); border: 1px solid var(--border);
  border-radius: 12px; padding: 0.4rem 0;
}
.ledger.empty { padding: 1rem 0.9rem; font-size: 0.85rem; color: var(--muted); }
table { width: 100%; border-collapse: separate; border-spacing: 0; background: var(--surface);
  border: 1px solid var(--border); border-radius: 12px; overflow: hidden; font-size: 0.85rem; }
th { font-size: 0.62rem; letter-spacing: 0.1em; text-transform: uppercase; color: var(--muted);
  text-align: left; padding: 0.5rem 0.8rem; border-bottom: 1px solid var(--border); font-weight: 500; }
td { padding: 0.55rem 0.8rem; border-bottom: 1px solid var(--border); }
tr:last-child td { border-bottom: none; }
td.num, th.num { text-align: right; font-family: var(--mono); font-variant-numeric: tabular-nums; font-size: 0.82rem; }
td.neg { color: var(--error); }
tr.del td { opacity: 0.5; }
.walletline { display: flex; align-items: center; gap: 0.6rem; padding: 0.55rem 0.8rem; font-size: 0.85rem; cursor: pointer; }
.walletline + .walletline { border-top: 1px solid var(--border); }
.walletline .name { flex: 0 1 auto; min-width: 0; max-width: 60%; color: var(--fg); overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.walletline .track { flex: 1; height: 0.5rem; border-radius: 99px; background: var(--bg); border: 1px solid var(--border); overflow: hidden; }
.walletline .track.none { visibility: hidden; }
.walletline .fill { height: 100%; background: var(--accent); border-radius: 99px; }
.walletline .fill.over { background: var(--error); }
.walletline b { display: block; font-family: var(--mono); font-variant-numeric: tabular-nums; font-size: 0.82rem; text-align: right; }
.walletline b.neg { color: var(--error); }
.walletline .of { display: block; font-size: 0.6rem; color: var(--muted); font-family: var(--mono); }
.walletline.del { opacity: 0.5; }
.chartcard { background: var(--surface); border: 1px solid var(--border); border-radius: 14px;
  padding: 0.4rem 0.9rem 0.5rem; margin: 0.9rem 0; }
.chartcard .chartsec { margin: 0.4rem 0 0.3rem; }
.curtabs { display: flex; gap: 0.4rem; }
.curtabs button { border: 1px solid var(--border); border-radius: 999px; background: transparent;
  color: var(--muted); font: inherit; font-size: 0.72rem; padding: 0.15rem 0.7rem; cursor: pointer; }
.curtabs button.on { background: var(--accent-soft); border-color: var(--accent); color: var(--accent); }
.chart { width: 100%; height: auto; display: block; }
.chart rect.income { fill: var(--c2); }
.chart rect.expense { fill: var(--c3); }
.chart .baseline { stroke: var(--border); stroke-width: 1; }
.chart .toplabel { fill: var(--muted); font-size: 9px; font-family: var(--mono); }
.chart .axis { fill: var(--muted); font-size: 9px; text-anchor: middle; }
.chart.empty { padding: 1.4rem 0; text-align: center; font-size: 0.85rem; color: var(--muted); }
.tx { display: flex; align-items: center; gap: 0.6rem; padding: 0.55rem 0.8rem; font-size: 0.85rem; cursor: pointer; }
.tx + .tx { border-top: 1px solid var(--border); }
.tx .ic { width: 1.4rem; height: 1.4rem; border-radius: 999px; background: var(--accent-soft);
  color: var(--accent); display: flex; align-items: center; justify-content: center; font-size: 0.8rem; flex-shrink: 0; }
.tx .who { flex: 1; min-width: 0; }
.tx .who b { display: block; font-weight: 600; font-size: 0.85rem; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.tx .who b.overdue { color: var(--error); }
.tx .who span { font-size: 0.7rem; color: var(--muted); }
.tx .amt { font-family: var(--mono); font-variant-numeric: tabular-nums; font-size: 0.82rem; flex-shrink: 0; }
.tx .amt.pos { color: var(--success); }
.tx .amt.neg { color: var(--fg); }
.notice { color: var(--accent); font-size: 0.8rem; margin: 0.4rem 0; }
.quick { display: flex; gap: 0.3rem; flex: none; }
.qbtn {
  width: 1.8rem; height: 1.8rem; border-radius: 8px; border: 1px solid var(--border);
  background: var(--surface); color: var(--accent); font: inherit; font-size: 0.8rem;
  cursor: pointer; display: grid; place-items: center; padding: 0;
}
.qbtn:hover { border-color: var(--accent); }
.qbtn[title='Pay as today'] { font-weight: 700; }

/* M12 tablet steps — the grid owns the layout; the phone column below
   720px is untouched (block flow, the original stack).
   Step 2 (≥720px, tablet2 pairs): balance beside chart, panels in pairs.
   Step 3 (≥900px, tablet3 canvas): chart takes the wide rail (2fr) with
   balance beside it, three panels in one row, Recent in two columns. */
.dgrid { display: block; }
.chartwrap { min-width: 0; }
.dgrid .notice { margin: 0 0 0.4rem; }
/* panels become cards (tablet2/3 pattern): header inside the card,
   equal-width tracks — minmax(0,…) so a wide table cannot push
   neighbours and steal width from them */
.dgrid .panel .sec { margin-top: 0; }
.panel { min-width: 0; }
@media (min-width: 720px) {
  .dgrid .panel {
    background: var(--surface); border: 1px solid var(--border);
    border-radius: 14px; padding: 0.8rem 0.4rem 0.4rem; overflow: hidden;
  }
  .dgrid .panel .sec { padding: 0 0.4rem; }
  .dgrid .panel table, .dgrid .panel .ledger { border: 0; border-radius: 0; }
  .dgrid {
    display: grid;
    grid-template-columns: minmax(0, 1fr) minmax(0, 1fr);
    /* default stretch — panels in a row share one height, as in tablet2/3 */
  }
  .dgrid .balance { margin: 0.9rem 0 0; }
  .dgrid .chartcard { margin: 0.9rem 0 0; }
}
@media (min-width: 900px) {
  .dgrid { grid-template-columns: repeat(6, minmax(0, 1fr)); gap: 0.6rem 1.5rem; }
  .dgrid .chartcard { grid-column: 1 / 5; grid-row: 1; margin: 0.9rem 0 0; }
  .dgrid .balance { grid-column: 5 / 7; grid-row: 1; margin: 0.9rem 0 0; }
  .panel.pw { grid-column: 1 / 3; }
  .panel.pc { grid-column: 3 / 5; }
  .panel.pu { grid-column: 5 / 7; }
  .panel.pr { grid-column: 1 / 7; }
  .panel.pr .sec { padding: 0 0.4rem; }
  .panel.pr .ledger { display: grid; grid-template-columns: 1fr 1fr; gap: 0; }
}
</style>
