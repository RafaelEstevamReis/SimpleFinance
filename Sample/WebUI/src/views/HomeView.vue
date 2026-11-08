<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ApiClient } from '../api/client'
import { getAccounts, getActive, getServerUrl, getThemePreview, saveActive, saveThemePreview } from '../api/storage'
import type { AccountDatabaseResponse, AccountResponse } from '../api/types'
import { applyTheme, clearTheme, isThemeName, accentOf, themeNames, defaultTheme, type ThemeName } from '../styles/themes'

// Placeholder home — proves the guarded area + Key round-trip. Real dashboard is M6.
const router = useRouter()
const account = ref<AccountResponse | null>(null)
const database = ref<AccountDatabaseResponse | null>(null)
const error = ref('')
const loading = ref(true)
const theme = ref<ThemeName>(defaultTheme)
const savingTheme = ref(false)

onMounted(async () => {
  const active = getActive(localStorage)
  if (!active) { router.replace({ name: 'server' }); return }
  const serverUrl = getServerUrl(localStorage)
  if (!serverUrl) { router.replace({ name: 'server' }); return }
  try {
    const client = new ApiClient(serverUrl, active.key)
    account.value = await client.account()
    database.value = await client.accountDatabase()
    // Theme: server is the truth — read the preference, fall back to preview,
    // then to neutral. Applied so the shell carries the account's identity.
    const prefs = await client.getPreferences()
    const stored = prefs.find((p) => p.name === 'theme')?.value
    if (stored && isThemeName(stored)) {
      theme.value = stored
      applyTheme(stored)
    } else {
      // no server preference — fall back to the local preview, else neutral
      const label = accountsLabel()
      const preview = label !== null ? getThemePreview(localStorage, label) : null
      if (preview && isThemeName(preview)) { theme.value = preview; applyTheme(preview) }
      else clearTheme()
    }
  } catch (e) {
    error.value = (e as Error).message
  } finally {
    loading.value = false
  }
})

function disconnect(): void {
  saveActive(localStorage, null)
  clearTheme()
  router.replace({ name: 'connect' })
}

async function pickTheme(name: ThemeName): Promise<void> {
  theme.value = name
  applyTheme(name)
  // server is the truth; preview cache follows the server write
  savingTheme.value = true
  try {
    const active = getActive(localStorage)
    const serverUrl = getServerUrl(localStorage)
    if (!active || !serverUrl) return
    const client = new ApiClient(serverUrl, active.key)
    await client.setPreference('theme', { value: name })
    const label = accountsLabel()
    if (label) saveThemePreview(localStorage, label, name)
  } finally {
    savingTheme.value = false
  }
}

/** Label of the active account, for the preview cache key */
function accountsLabel(): string | null {
  const active = getActive(localStorage)
  if (!active) return null
  const acc = getAccounts(localStorage).find((a) => a.key === active.key)
  return acc ? acc.label : null
}
</script>

<template>
  <main class="home">
    <template v-if="loading">
      <p class="muted">Loading…</p>
    </template>
    <template v-else-if="error">
      <p class="err">{{ error }}</p>
      <button @click="disconnect">Back to Connect</button>
    </template>
    <template v-else-if="account && database">
      <header>
        <h1>{{ account.name || 'Account' }}</h1>
        <button @click="disconnect">Disconnect</button>
      </header>
      <p class="muted">Connected — placeholder until M3+ ships real screens.</p>
      <dl>
        <dt>Database</dt><dd>{{ database.fileName }}</dd>
        <dt>Size</dt><dd>{{ (database.sizeBytes / 1024).toFixed(1) }} KB</dd>
        <dt>Wallets</dt><dd>{{ database.walletCount }}</dd>
        <dt>Last access</dt><dd>{{ account.lastAccess }}</dd>
      </dl>
      <h2>Theme</h2>
      <p class="muted">Identity tint — travels with the account on the server.</p>
      <div class="swatches">
        <button v-for="name in themeNames" :key="name"
                class="swatch" :class="{ active: theme === name }"
                :style="{ '--swatch': accentOf(name) }"
                :disabled="savingTheme" @click="pickTheme(name)">
          <span class="chip"></span>{{ name }}
        </button>
      </div>
    </template>
  </main>
</template>

<style scoped>
.home {
  max-width: 560px;
  margin: 0 auto;
  padding: 1.5rem 1rem 4rem;
}
header { display: flex; justify-content: space-between; align-items: center; }
h1 { margin: 0; font-size: 1.2rem; }
.muted { opacity: 0.65; }
.err { color: light-dark(#b91c1c, #f87171); }
dl { display: grid; grid-template-columns: auto 1fr; gap: 0.3rem 1rem; }
dt { opacity: 0.65; }
dd { margin: 0; overflow-wrap: anywhere; }
button {
  font: inherit;
  border: 1px solid light-dark(#c4c9cf, #3a4148);
  border-radius: 6px;
  background: light-dark(#e9edf1, #262c33);
  color: inherit;
  padding: 0.4rem 0.8rem;
  cursor: pointer;
}
h2 { margin: 1.2rem 0 0.3rem; font-size: 1rem; }
.swatches { display: flex; flex-wrap: wrap; gap: 0.5rem; margin-top: 0.5rem; }
.swatch {
  display: inline-flex;
  align-items: center;
  gap: 0.45rem;
  border: 1px solid var(--border);
  border-radius: 999px;
  background: var(--surface);
  color: inherit;
  padding: 0.35rem 0.85rem 0.35rem 0.45rem;
  cursor: pointer;
}
.swatch.active { border-color: var(--accent); box-shadow: 0 0 0 1px var(--accent-soft); }
.swatch .chip { width: 1rem; height: 1rem; border-radius: 50%; background: var(--swatch); }
</style>
