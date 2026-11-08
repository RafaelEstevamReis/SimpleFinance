<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { ApiClient, ApiError } from '../api/client'
import {
  getAccounts,
  getServerUrl,
  getThemePreview,
  saveAccounts,
  saveActive,
  type StoredAccount,
} from '../api/storage'
import { loadSession, session } from '../state/session'
import { accentOf, isThemeName } from '../styles/themes'

// Account connection — assumes the server is already selected (ServerView).
const router = useRouter()

const serverUrl = getServerUrl(localStorage) ?? ''
const accounts = ref(getAccounts(localStorage))

// step 1: account on this server
const accountLabel = ref('')
const accountKey = ref('')
const creating = ref(false)
const accountError = ref('')
const createdKey = ref('')
const createdCopied = ref(false)

// primary currency (WebUI-side — the API has no such field; first wallet uses it in M3)
const browserCurrency = Intl.supportedValuesOf('currency')
const currency = ref(new Intl.NumberFormat().resolvedOptions().currency ?? 'USD')

// step 2: saved accounts list
const connecting = ref('')

const emit = defineEmits<{ connected: [] }>()

function failureMessage(e: unknown): string {
  if (e instanceof ApiError) {
    switch (e.kind) {
      case 'unreachable':
        return `Server unreachable: ${e.message}`
      case 'unauthorized':
        return 'The Key was rejected by this server'
      case 'bad-request':
        return e.message
      default:
        return e.message
    }
  }
  return (e as Error).message
}

async function addAccount(): Promise<void> {
  accountError.value = ''
  const key = accountKey.value.trim()
  const label = accountLabel.value.trim() || 'Account'
  if (key === '') { accountError.value = 'Paste an existing Key or create an account'; return }
  if (serverUrl === '') { accountError.value = 'No server selected yet'; return }
  creating.value = true
  try {
    // validate before saving — a Key that does not open is not an account
    const probe = new ApiClient(serverUrl, key)
    const info = await probe.account()
    const stored: StoredAccount = {
      label: label || info.name,
      key,
      primaryCurrency: currency.value,
    }
    accounts.value = [...accounts.value, stored]
    saveAccounts(localStorage, accounts.value)
    accountKey.value = ''
    accountLabel.value = ''
  } catch (e) {
    accountError.value = failureMessage(e)
  } finally {
    creating.value = false
  }
}

async function createAccount(): Promise<void> {
  accountError.value = ''
  creating.value = true
  try {
    if (serverUrl === '') { accountError.value = 'No server selected yet'; return }
    const probe = new ApiClient(serverUrl)
    const res = await probe.createAccount(accountLabel.value.trim() || 'Account')
    // Auto-save: the Key is shown once by the API and unrecoverable — a reload
    // between "create" and "save" would lose it forever. Store it immediately;
    // the shown-once box stays for the user to copy it elsewhere.
    const stored: StoredAccount = {
      label: accountLabel.value.trim() || res.name || 'Account',
      key: res.key,
      primaryCurrency: currency.value,
    }
    accounts.value = [...accounts.value, stored]
    saveAccounts(localStorage, accounts.value)
    createdKey.value = res.key
    createdCopied.value = false
  } catch (e) {
    accountError.value = failureMessage(e)
  } finally {
    creating.value = false
  }
}

function copyCreatedKey(): void {
  navigator.clipboard.writeText(createdKey.value)
  createdCopied.value = true
}

async function connect(acc: StoredAccount): Promise<void> {
  connecting.value = acc.label
  try {
    if (serverUrl === '') return
    // revalidate on connect — keys can be stale
    const client = new ApiClient(serverUrl, acc.key)
    await client.account()
    saveActive(localStorage, { key: acc.key })
    // session state loads identity + all preferences (theme/mode/account-type)
    // in one place; server is the truth, preview cache is the fallback
    await loadSession()
    session.ready.value = true
    emit('connected')
    router.push({ name: 'home' })
  } catch {
    // stay on connect; error surfaces per-account next to the card
  } finally {
    connecting.value = ''
  }
}

function cardTint(a: StoredAccount): Record<string, string> | undefined {
  const preview = getThemePreview(localStorage, a.label)
  if (preview && isThemeName(preview)) {
    return { borderColor: accentOf(preview) }
  }
  return undefined
}

/** Accent for the identity dot; neutral (CSS default) when no preview */
function dotColor(a: StoredAccount): string | undefined {
  const preview = getThemePreview(localStorage, a.label)
  return preview && isThemeName(preview) ? accentOf(preview) : undefined
}

function removeAccount(acc: StoredAccount): void {
  accounts.value = accounts.value.filter((a) => a.label !== acc.label || a.key !== acc.key)
  saveAccounts(localStorage, accounts.value)
}
</script>

<template>
  <main class="connect">
    <header>
      <h1>Accounts</h1>
      <button class="ghost" @click="router.push({ name: 'server' })">Server…</button>
    </header>
    <p class="muted">on {{ serverUrl }}</p>

    <!-- Step 1: account -->
    <section>
      <h2>1 · Account</h2>
      <div class="row">
        <input v-model="accountLabel" placeholder="Account name" />
      </div>
      <div class="row">
        <input v-model="accountKey" placeholder="Paste existing Key" autocomplete="off" />
        <button :disabled="creating" @click="addAccount">Add existing</button>
        <button :disabled="creating" @click="createAccount">Create new</button>
      </div>
      <label class="currency">
        Primary currency
        <select v-model="currency">
          <option v-for="c in browserCurrency" :key="c" :value="c">{{ c }}</option>
        </select>
      </label>
      <p v-if="accountError" class="err">{{ accountError }}</p>

      <div v-if="createdKey" class="created">
        <p><strong>Key created and saved — copy it now, it is shown once and cannot be recovered:</strong></p>
        <code>{{ createdKey }}</code>
        <div class="row">
          <button class="primary" @click="copyCreatedKey">
            {{ createdCopied ? 'Copied ✓' : 'Copy Key' }}
          </button>
          <button @click="createdKey = ''">Done</button>
        </div>
      </div>
    </section>

    <!-- Step 2: saved accounts -->
    <section>
      <h2>2 · Saved accounts</h2>
      <ul v-if="accounts.length" class="cards">
        <li v-for="a in accounts" :key="a.key" class="card"
            :style="cardTint(a)">
          <div>
            <span class="dot" :style="{ background: dotColor(a) }"></span>
            <strong>{{ a.label }}</strong>
            <span class="muted"> · {{ a.primaryCurrency }}</span>
          </div>
          <div class="row tight">
            <button class="primary" :disabled="connecting === a.label"
                    @click="connect(a)">
              {{ connecting === a.label ? 'Connecting…' : 'Connect' }}
            </button>
            <button @click="removeAccount(a)">Remove</button>
          </div>
        </li>
      </ul>
      <p v-else class="muted">No saved accounts yet.</p>
    </section>
  </main>
</template>

<style scoped>
.connect {
  max-width: 560px;
  margin: 0 auto;
  padding: 1.5rem 1rem 4rem;
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}
header { display: flex; justify-content: space-between; align-items: center; }
h1 { margin: 0; font-size: 1.4rem; }
h2 { margin: 0 0 0.5rem; font-size: 1rem; }
.muted { opacity: 0.65; font-size: 0.9rem; margin: 0.15rem 0; }
section {
  border: 1px solid light-dark(#d0d4d9, #2a2f36);
  border-radius: 8px;
  padding: 0.9rem;
}
.row {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
}
.row.tight { margin: 0.5rem 0 0; }
input, select {
  background: light-dark(#fff, #1c2128);
  color: inherit;
  border: 1px solid light-dark(#c4c9cf, #3a4148);
  border-radius: 6px;
  padding: 0.5rem 0.6rem;
}
button {
  border: 1px solid light-dark(#c4c9cf, #3a4148);
  border-radius: 6px;
  background: light-dark(#e9edf1, #262c33);
  color: inherit;
  padding: 0.5rem 0.9rem;
  cursor: pointer;
}
button.primary {
  background: light-dark(#2563eb, #3b82f6);
  border-color: transparent;
  color: #fff;
}
button.ghost {
  background: transparent;
  font-size: 0.85rem;
  padding: 0.3rem 0.6rem;
}
button:disabled { opacity: 0.55; }
.err { color: light-dark(#b91c1c, #f87171); margin: 0.3rem 0; }
.currency { display: inline-flex; gap: 0.5rem; align-items: center; font-size: 0.9rem; }
.created {
  border: 1px solid light-dark(#f59e0b, #b45309);
  border-radius: 8px;
  padding: 0.75rem;
  margin-top: 0.5rem;
}
.created code {
  display: block;
  overflow-wrap: anywhere;
  font-size: 0.85rem;
  margin: 0.4rem 0;
}
.cards { list-style: none; margin: 0; padding: 0; display: flex; flex-direction: column; gap: 0.5rem; }
.card {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 0.5rem;
  flex-wrap: wrap;
  border: 1px solid light-dark(#d0d4d9, #2a2f36);
  border-radius: 8px;
  padding: 0.6rem 0.75rem;
}
/* identity tint — preview only; the server holds the truth after connect */
.card {
  border-color: var(--border);
}
.card .dot {
  display: inline-block;
  width: 0.7rem;
  height: 0.7rem;
  border-radius: 50%;
  background: var(--accent);
  margin-right: 0.4rem;
}
</style>
