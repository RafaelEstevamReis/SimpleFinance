// Wallets — first money screen (M4). Demo5 table (Wallet / Currency /
// Balance) fed by GET /api/wallets + GET /api/wallets/balances in two calls.
// Balances are each in their own currency; nothing here ever sums across
// currencies. Create/edit is one inline form; the server's message reaches
// the global error area verbatim.

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { activeClient, errorMessage, session } from '../state/session'
import type { WalletBalanceResponse, WalletRequest, WalletResponse } from '../api/types'
import { formatMoney, normalizeCurrency } from '../api/wallets'

const wallets = ref<WalletResponse[]>([])
const balances = ref<WalletBalanceResponse[]>([])
const loading = ref(true)
// form state — formOpen shows the card; editingId null = creating,
// a number = editing that wallet
const formOpen = ref(false)
const editingId = ref<number | null>(null)
const formName = ref('')
const formCurrency = ref('')

const formDescription = ref('')
const formError = ref('')
const saving = ref(false)

async function load(): Promise<void> {
  const client = activeClient()
  if (!client) return
  loading.value = true
  try {
    const [w, b] = await Promise.all([client.wallets(), client.walletBalances()])
    wallets.value = w
    balances.value = b
  } catch (e) {
    session.error.value = errorMessage(e)
  } finally {
    loading.value = false
  }
}

onMounted(() => void load())

function balanceOf(walletId: number): number | null {
  return balances.value.find(b => b.walletId === walletId)?.balance ?? null
}

function openCreate(): void {
  formOpen.value = true
  editingId.value = null
  formName.value = ''
  formCurrency.value = ''
  formDescription.value = ''
  formError.value = ''
}

function openEdit(wallet: WalletResponse): void {
  formOpen.value = true
  editingId.value = wallet.id
  formName.value = wallet.name
  formCurrency.value = wallet.baseCurrency
  formDescription.value = wallet.description
  formError.value = ''
}

async function save(): Promise<void> {
  const client = activeClient()
  if (!client) return
  formError.value = ''
  const name = formName.value.trim()
  const baseCurrency = normalizeCurrency(formCurrency.value)
  if (!name) {
    formError.value = 'Wallet.Name is required'
    return
  }
  if (!baseCurrency) {
    formError.value = 'Currency is required'
    return
  }
  const body: WalletRequest = {
    externallType: 0,
    name,
    description: formDescription.value.trim(),
    baseCurrency,
    isDeleted: false,
  }
  saving.value = true
  try {
    if (editingId.value === null) await client.createWallet(body)
    else await client.updateWallet(editingId.value, body)
    editingId.value = null
    formOpen.value = false
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
</script>

<template>
  <section>
    <div class="sec"><h2>Wallets</h2><button class="add" @click="openCreate">＋ New</button></div>

    <p v-if="loading" class="muted">Loading…</p>

    <table v-else-if="wallets.length > 0">
      <tr>
        <th>Wallet</th>
        <th>Currency</th>
        <th class="num">Balance</th>
      </tr>
      <tbody>
        <tr
          v-for="w in wallets"
          :key="w.id"
          :class="{ del: w.isDeleted }"
          @click="openEdit(w)"
        >
          <td>{{ w.name }}</td>
          <td>{{ w.baseCurrency }}</td>
          <td class="num" :class="{ neg: (balanceOf(w.id) ?? 0) < 0 }">
            {{ balanceOf(w.id) === null ? '…' : formatMoney(balanceOf(w.id) as number) }}
          </td>
        </tr>
      </tbody>
    </table>

    <div v-else class="ledger empty">
      <p>No wallets yet — create the first one above: an account, a card, cash or a broker.</p>
    </div>

    <div v-if="formOpen" class="form card">
      <h3>{{ editingId === null ? 'New wallet' : 'Edit wallet' }}</h3>
      <label>
        Name
        <input v-model="formName" type="text" placeholder="Main" autocomplete="off">
      </label>
      <label>
        Currency
        <input v-model="formCurrency" type="text" placeholder="BRL" autocomplete="off">
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

table { width: 100%; border-collapse: separate; border-spacing: 0; background: var(--surface);
  border: 1px solid var(--border); border-radius: 12px; overflow: hidden; font-size: 0.85rem; }
th { font-size: 0.62rem; letter-spacing: 0.1em; text-transform: uppercase; color: var(--muted);
  text-align: left; padding: 0.5rem 0.8rem; border-bottom: 1px solid var(--border); font-weight: 500; }
td { padding: 0.55rem 0.8rem; border-bottom: 1px solid var(--border); }
tr:last-child td { border-bottom: none; }
tr { cursor: pointer; }
td.num, th.num { text-align: right; font-family: var(--mono); font-variant-numeric: tabular-nums; font-size: 0.82rem; }
td.neg { color: var(--error); }
tr.del td { opacity: 0.5; }

.ledger { background: var(--surface); border: 1px solid var(--border); border-radius: 12px; padding: 0.4rem 0; }
.ledger.empty { padding: 1rem 0.9rem; font-size: 0.85rem; color: var(--muted); }

.card { background: var(--surface); border: 1px solid var(--border); border-radius: 14px;
  padding: 1rem 1.1rem; margin-top: 0.9rem; }
h3 { margin: 0 0 0.8rem; font-size: 0.95rem; }
label { display: block; font-size: 0.72rem; letter-spacing: 0.1em; text-transform: uppercase;
  color: var(--muted); margin-bottom: 0.8rem; }
label input { display: block; width: 100%; box-sizing: border-box; margin-top: 0.3rem;
  font: inherit; font-size: 0.95rem; text-transform: none; letter-spacing: normal; color: var(--fg);
  background: transparent; border: 1px solid var(--border); border-radius: 10px; padding: 0.55rem 0.7rem; }
label .hint { text-transform: none; letter-spacing: normal; }
.err { color: var(--error); font-size: 0.8rem; margin: 0 0 0.6rem; }
.row { display: flex; gap: 0.6rem; justify-content: flex-end; }
.ghost { border: 1px solid var(--border); border-radius: 999px; background: transparent;
  color: var(--fg); font: inherit; font-size: 0.8rem; padding: 0.45rem 1rem; cursor: pointer; }
.primary { border: 1px solid var(--accent); border-radius: 999px; background: var(--accent);
  color: var(--on-accent, #fff); font: inherit; font-size: 0.8rem; padding: 0.45rem 1.2rem; cursor: pointer; }
button:disabled { opacity: 0.55; }
</style>
