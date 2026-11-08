// Home — the demo5 dashboard shape, wired with real data where the M3 scope
// has it (nothing money-shaped is live yet: that is M4 wallets, M6 book,
// M8 dashboard). Every section shows its designed empty state for now.

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { activeClient, errorMessage, session } from '../state/session'
import type { AccountDatabaseResponse } from '../api/types'

const router = useRouter()
const database = ref<AccountDatabaseResponse | null>(null)

async function loadDb(): Promise<void> {
  const client = activeClient()
  if (!client) return
  try {
    database.value = await client.accountDatabase()
  } catch (e) {
    session.error.value = errorMessage(e)
  }
}
loadDb()
</script>

<template>
  <section>
    <div class="balance">
      <div class="lbl">Net worth</div>
      <div class="val muted">—</div>
      <div class="delta muted">
        <template v-if="database">Balances arrive with the Wallets screen (M4)</template>
        <template v-else>Not connected to a database yet</template>
      </div>
    </div>

    <div class="sec"><h2>Wallets</h2><a @click="router.push({ name: 'wallets' })">manage →</a></div>
    <div class="ledger empty">
      <p v-if="database && database.walletCount > 0">
        {{ database.walletCount }} wallet{{ database.walletCount === 1 ? '' : 's' }} on this account —
        the list arrives with M4.
      </p>
      <p v-else>No wallets yet — create the first one from the Wallets screen.</p>
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
</style>
