// MovementForm — the transaction create/edit form, extracted from the book
// (M10) so every screen that shows transaction rows can edit them the same
// way: the book and the advanced search share one form, never two truths.
// A transfer row never opens this form — it opens TransferForm (M6/M7
// doctrine: the pair is edited as one).
//
// The parent owns the surface: it renders this with v-if and supplies the
// editing row; this component owns the inputs, the request build, the save.

<script setup lang="ts">
import { ref, watch } from 'vue'
import { buildTransactionRequest } from '../api/book'
import { activeClient, errorMessage } from '../state/session'
import type { CategoryResponse, PersonResponse, TransactionResponse, WalletResponse } from '../api/types'

const props = defineProps<{
  wallets: WalletResponse[]
  categories: CategoryResponse[]
  persons: PersonResponse[]
  /// null = create; a row = edit it, inputs prefilled from what is stored
  editing: TransactionResponse | null
  /// wallet preselected when creating
  defaultWalletId: number
}>()

const emit = defineEmits<{
  /// fired after the server accepted the write — the parent reloads its own data
  saved: []
  cancel: []
}>()

const walletId = ref(0)
const categoryId = ref(0)
const counterpartyId = ref(0)
const dueDate = ref('')
const paymentDate = ref('')
const description = ref('')
const status = ref<'Unpaid' | 'Paid' | 'Reversed'>('Paid')
const dueValue = ref('')
const paidValue = ref('')
const paymentDetails = ref('')
const saving = ref(false)
const error = ref('')

watch(() => props.editing, (tx) => {
  if (tx === null) {
    walletId.value = props.defaultWalletId
    categoryId.value = 0
    counterpartyId.value = 0
    const today = new Date().toISOString().slice(0, 10)
    dueDate.value = today
    paymentDate.value = today
    description.value = ''
    status.value = 'Paid'
    dueValue.value = ''
    paidValue.value = ''
    paymentDetails.value = ''
  } else {
    walletId.value = tx.walletId
    categoryId.value = tx.categoryId
    counterpartyId.value = tx.counterpartyId
    dueDate.value = tx.dueDate.slice(0, 10)
    paymentDate.value = tx.paymentDate.slice(0, 10)
    description.value = tx.description
    status.value = tx.status as 'Unpaid' | 'Paid' | 'Reversed'
    // Stored values carry the sign — the input is positive, abs() at the border
    dueValue.value = String(Math.abs(tx.dueValue))
    paidValue.value = String(Math.abs(tx.paidValue))
    paymentDetails.value = tx.paymentDetails ?? ''
  }
  error.value = ''
}, { immediate: true })

async function save(): Promise<void> {
  const client = activeClient()
  if (!client) return
  const out = buildTransactionRequest({
    walletId: walletId.value,
    categoryId: categoryId.value,
    counterpartyId: counterpartyId.value,
    dueDate: dueDate.value,
    paymentDate: paymentDate.value,
    description: description.value,
    status: status.value,
    dueValue: dueValue.value,
    paidValue: paidValue.value,
    paymentDetails: paymentDetails.value,
  })
  if (out.request === null) {
    error.value = out.error
    return
  }
  saving.value = true
  try {
    if (props.editing === null) await client.createTransaction(out.request)
    else await client.updateTransaction(props.editing.id, out.request)
    emit('saved')
  } catch (e) {
    error.value = errorMessage(e)
  } finally {
    saving.value = false
  }
}

function cancel(): void {
  emit('cancel')
}
</script>

<template>
  <div class="form card">
    <h3>{{ editing === null ? 'New movement' : 'Edit movement' }}</h3>

    <label>
      Wallet
      <select v-model.number="walletId">
        <option v-for="w in wallets" :key="w.id" :value="w.id">{{ w.name }}</option>
      </select>
    </label>
    <label>
      Category
      <select v-model.number="categoryId">
        <option :value="0">— pick one —</option>
        <option v-for="c in categories" :key="c.id" :value="c.id">{{ c.isExpense ? '− ' : '+ ' }}{{ c.name }}</option>
      </select>
    </label>
    <label>
      Counterparty <span class="hint">(optional)</span>
      <select v-model.number="counterpartyId">
        <option :value="0">— none —</option>
        <option v-for="p in persons" :key="p.id" :value="p.id">{{ p.name }}</option>
      </select>
    </label>
    <div class="grid2">
      <label>
        Due date
        <input v-model="dueDate" type="date">
      </label>
      <label>
        Payment date
        <input v-model="paymentDate" type="date">
      </label>
    </div>
    <div class="lock">
      <span class="lbl">Status</span>
      <div class="types">
        <button :class="{ on: status === 'Paid' }" @click="status = 'Paid'">Paid</button>
        <button :class="{ on: status === 'Unpaid' }" @click="status = 'Unpaid'">Unpaid</button>
        <button :class="{ on: status === 'Reversed' }" @click="status = 'Reversed'">Reversed</button>
      </div>
    </div>
    <div class="grid2">
      <label>
        Due value
        <input v-model="dueValue" type="text" inputmode="decimal" placeholder="100">
      </label>
      <label v-if="status === 'Paid'">
        Paid value
        <input v-model="paidValue" type="text" inputmode="decimal" placeholder="100">
      </label>
    </div>
    <p class="hintline">Values are positive — the category decides the side.</p>
    <label>
      Description
      <input v-model="description" type="text" placeholder="What it was" autocomplete="off">
    </label>
    <label>
      Payment details <span class="hint">(optional)</span>
      <input v-model="paymentDetails" type="text" placeholder="Free text" autocomplete="off">
    </label>
    <p v-if="error" class="err">{{ error }}</p>
    <div class="row">
      <button class="ghost" :disabled="saving" @click="cancel">Cancel</button>
      <button class="primary" :disabled="saving" @click="save">{{ saving ? 'Saving…' : 'Save' }}</button>
    </div>
  </div>
</template>

<style scoped>
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
