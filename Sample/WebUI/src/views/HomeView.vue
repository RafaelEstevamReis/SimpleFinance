// Home — the demo5 dashboard shape. Wallets and net worth are real (M4);
// categories and activity keep their designed empty states until M5/M6.
// Net worth follows the cross-currency rule: one currency sums, two or
// more show no number rather than a made-up one.

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { activeClient, errorMessage, session } from '../state/session'
import type { WalletBalanceResponse, WalletResponse } from '../api/types'
import { formatMoney, liveBalances, netWorthLabel, sumByCurrency } from '../api/wallets'

const router = useRouter()
const wallets = ref<WalletResponse[]>([])
const balances = ref<WalletBalanceResponse[]>([])
const loaded = ref(false)

async function load(): Promise<void> {
  const client = activeClient()
  if (!client) return
  try {
    const [w, b] = await Promise.all([client.wallets(), client.walletBalances()])
    wallets.value = w
    balances.value = liveBalances(w, b)
  } catch (e) {
    session.error.value = errorMessage(e)
  } finally {
    loaded.value = true
  }
}

onMounted(() => void load())

const net = computed<string>(() => netWorthLabel(balances.value))
const netNote = computed<string>(() => {
  const currencies = sumByCurrency(balances.value).size
  if (!loaded.value) return 'Loading…'
  if (currencies === 0) return 'No wallets yet'
  if (currencies === 1) return 'Sum of every wallet, one currency'
  return `Balances in ${currencies} currencies — no single total`
})

function balanceOf(walletId: number): number | null {
  return balances.value.find(b => b.walletId === walletId)?.balance ?? null
}
</script>

<template>
  <section>
    <div class="balance">
      <div class="lbl">Net worth</div>
      <div class="val" :class="{ muted: net === '—' }">{{ net }}</div>
      <div class="delta muted">{{ netNote }}</div>
    </div>

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

    <div class="sec"><h2>Categories</h2><a @click="router.push({ name: 'settings' })">manage →</a></div>
    <div class="ledger empty">
      <p>Budget tracks arrive with M5 — each category shows the month's spend against its budget.</p>
    </div>

    <div class="sec"><h2>Activity</h2><a @click="router.push({ name: 'book' })">all →</a></div>
    <div class="ledger empty">
      <p>The book opens with M6 — one wallet, one month.</p>
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
</style>
