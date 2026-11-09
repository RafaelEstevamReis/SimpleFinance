// TransferForm — the transfer pair create/edit form, extracted from the book
// (M10) so every screen with transaction rows edits a transfer the same way:
// ONE linked pair, ONE form (M7 doctrine). Wallets and categories are stated
// on create and fixed after it — the server owns that rule; on edit this form
// shows the pair heading and asks only what may still change.
//
// The parent renders it with v-if and supplies either a create seed or the
// pair to edit (found by the id of either leg — the endpoint accepts both).

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { buildCreateTransferRequest, buildUpdateTransferRequest, pairHeading } from '../api/transfers'
import { activeClient, errorMessage } from '../state/session'
import type { CategoryResponse, CreateTransferRequest, UpdateTransferRequest, WalletResponse } from '../api/types'

const props = defineProps<{
  wallets: WalletResponse[]
  categories: CategoryResponse[]
  /// null = create; non-zero = edit the pair this leg id belongs to
  editingLegId: number | null
  /// wallet preselected as source when creating
  defaultWalletId: number
}>()

const emit = defineEmits<{
  /// fired after the server accepted the write — the parent reloads its own data
  saved: []
  cancel: []
}>()

const sourceWalletId = ref(0)
const destinationWalletId = ref(0)
const sourceCategoryId = ref(0)
const destinationCategoryId = ref(0)
const value = ref('')
const status = ref<'Unpaid' | 'Paid' | 'Reversed'>('Paid')
const paidValue = ref('')
const description = ref('')
const paymentDetails = ref('')
const dueDate = ref('')
const paymentDate = ref('')
const saving = ref(false)
const error = ref('')

/// Transfer categories are side-locked by the server (source expense,
/// destination income) — the selects only offer legal rows
const expenseCategories = computed(() => props.categories.filter(c => c.isExpense))
const incomeCategories = computed(() => props.categories.filter(c => !c.isExpense))

const editing = computed(() => props.editingLegId !== null)

const walletName = (id: number): string => props.wallets.find(w => w.id === id)?.name ?? '?'

watch(() => props.editingLegId, async (legId) => {
  const today = new Date().toISOString().slice(0, 10)
  if (legId === null) {
    sourceWalletId.value = props.defaultWalletId
    destinationWalletId.value = 0
    sourceCategoryId.value = 0
    destinationCategoryId.value = 0
    value.value = ''
    status.value = 'Paid'
    paidValue.value = ''
    description.value = ''
    paymentDetails.value = ''
    dueDate.value = today
    paymentDate.value = today
    error.value = ''
    return
  }
  const client = activeClient()
  if (!client) return
  try {
    const pair = await client.transfer(legId)
    sourceWalletId.value = pair.source.walletId
    destinationWalletId.value = pair.destination.walletId
    sourceCategoryId.value = pair.source.categoryId
    destinationCategoryId.value = pair.destination.categoryId
    // Stored legs carry the sign — the input is positive, abs() at the border
    value.value = String(Math.abs(pair.source.dueValue))
    status.value = pair.source.status as 'Unpaid' | 'Paid' | 'Reversed'
    description.value = pair.source.description
    paymentDetails.value = pair.source.paymentDetails ?? ''
    dueDate.value = (pair.source.dueDate ?? today).slice(0, 10)
    paymentDate.value = (pair.source.paymentDate ?? today).slice(0, 10)
    // paid value rides the same input when Paid — filled from the source leg
    paidValue.value = pair.source.status === 'Paid' ? String(Math.abs(pair.source.paidValue)) : ''
    error.value = ''
  } catch (e) {
    error.value = errorMessage(e)
  }
}, { immediate: true })

async function save(): Promise<void> {
  const client = activeClient()
  if (!client) return
  const out = editing.value
    ? buildUpdateTransferRequest({
        description: description.value,
        value: value.value,
        paidValue: paidValue.value,
        status: status.value,
        dueDate: dueDate.value,
        paymentDate: paymentDate.value,
        paymentDetails: paymentDetails.value,
      })
    : buildCreateTransferRequest({
        sourceWalletId: sourceWalletId.value,
        sourceCategoryId: sourceCategoryId.value,
        destinationWalletId: destinationWalletId.value,
        destinationCategoryId: destinationCategoryId.value,
        description: description.value,
        value: value.value,
        dueDate: dueDate.value,
        paymentDate: paymentDate.value,
        paid: status.value === 'Paid',
        paymentDetails: paymentDetails.value,
      })
  if (out.request === null) {
    error.value = out.error
    return
  }
  saving.value = true
  try {
    if (editing.value) await client.updateTransfer(props.editingLegId!, out.request as UpdateTransferRequest)
    else await client.createTransfer(out.request as CreateTransferRequest)
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
    <h3>{{ editing ? 'Edit transfer' : 'New transfer' }}</h3>

    <template v-if="editing">
      <p class="hintline">{{ pairHeading(walletName(sourceWalletId), walletName(destinationWalletId)) }} — wallets are fixed after creation.</p>
    </template>
    <template v-else>
      <label>
        Source wallet
        <select v-model.number="sourceWalletId">
          <option v-for="w in wallets" :key="w.id" :value="w.id">{{ w.name }}</option>
        </select>
      </label>
      <label>
        Destination wallet
        <select v-model.number="destinationWalletId">
          <option :value="0">— pick one —</option>
          <option v-for="w in wallets" :key="w.id" :value="w.id">{{ w.name }}</option>
        </select>
      </label>
      <div class="grid2">
        <label>
          Source category <span class="hint">(optional)</span>
          <select v-model.number="sourceCategoryId">
            <option :value="0">— none —</option>
            <option v-for="c in expenseCategories" :key="c.id" :value="c.id">− {{ c.name }}</option>
          </select>
        </label>
        <label>
          Destination category <span class="hint">(optional)</span>
          <select v-model.number="destinationCategoryId">
            <option :value="0">— none —</option>
            <option v-for="c in incomeCategories" :key="c.id" :value="c.id">+ {{ c.name }}</option>
          </select>
        </label>
      </div>
    </template>

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
        Value
        <input v-model="value" type="text" inputmode="decimal" placeholder="100">
      </label>
      <label v-if="status === 'Paid'">
        Paid value
        <input v-model="paidValue" type="text" inputmode="decimal" placeholder="100">
      </label>
    </div>
    <p class="hintline">Values are positive — the pair carries both sides.</p>
    <label>
      Description
      <input v-model="description" type="text" placeholder="What it was for" autocomplete="off">
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
