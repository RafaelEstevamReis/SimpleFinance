// AppShell — the frame every post-connect screen hangs on (M3 spec).
// Top bar (account label + identity tint + profile avatar), the profile
// sheet (switch account / add or create), the global error area and the
// bottom nav. Router children render inside; connect/server screens are
// outside this layout by route design.
//

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import {
  disconnect,
  errorMessage,
  initialOf,
  loadSession,
  savedAccounts,
  session,
  switchAccount,
} from '../state/session'
import { labelOfAccountType } from '../api/accountTypes'
import { accentOf } from '../styles/themes'
import { getThemePreview } from '../api/storage'
import { isThemeName } from '../styles/themes'
import type { StoredAccount } from '../api/storage'

const router = useRouter()

const sheetOpen = ref(false)
const switching = ref('')
const accounts = ref<StoredAccount[]>([])
const loadError = ref('')

onMounted(async () => {
  try {
    await loadSession()
    session.ready.value = true
    accounts.value = savedAccounts()
  } catch (e) {
    loadError.value = errorMessage(e)
  }
})

function toggleSheet(): void {
  sheetOpen.value = !sheetOpen.value
}

function closeSheet(): void {
  sheetOpen.value = false
}

async function pickAccount(acc: StoredAccount): Promise<void> {
  if (switching.value !== '') return
  const active = getActive(localStorage)
  if (active !== null && active.key === acc.key) {
    // already connected — comparing KEYS, never the name (the old code
    // compared a Guid against accountName and always missed)
    closeSheet()
    return
  }
  switching.value = acc.key
  session.error.value = ''
  try {
    await switchAccount(acc)
    accounts.value = savedAccounts()
    closeSheet()
  } catch (e) {
    // stays open — the switch failed, the user must see why
    session.error.value = errorMessage(e)
  } finally {
    switching.value = ''
  }
}

function addOrCreate(): void {
  closeSheet()
  router.push({ name: 'connect' })
}

function leave(): void {
  closeSheet()
  disconnect()
  router.replace({ name: 'connect' })
}

/** Label tint for the profile sheet — preview cache, same as connect cards */
function tintOf(acc: StoredAccount): string | undefined {
  const preview = getThemePreview(localStorage, acc.key)
  return preview !== null && isThemeName(preview) ? accentOf(preview) : undefined
}

const accountTypeLabel = labelOfAccountType // template needs it exposed
</script>

<template>
  <div class="shell">
    <template v-if="loadError">
      <main class="fatal">
        <p class="err">{{ loadError }}</p>
        <button @click="leave">Back to Connect</button>
      </main>
    </template>
    <template v-else-if="session.ready.value">
      <header class="top">
        <div class="left">
          <h1>My Personal Finances</h1>
          <span class="who">{{ accountTypeLabel(session.state.value.accountType) }}</span>
        </div>
        <button class="avatar" :title="session.accountLabel.value" @click="toggleSheet">
          {{ initialOf(session.accountLabel.value) }}
        </button>
      </header>

      <!-- global error area — set by any screen, dismissed here -->
      <div v-if="session.error.value" class="alert" role="alert">
        <span>{{ session.error.value }}</span>
        <button @click="session.error.value = ''">✕</button>
      </div>

      <main class="content"><RouterView /></main>

      <transition name="fade">
        <div v-if="sheetOpen" class="profile-backdrop" @click="closeSheet">
          <div class="sheet" role="menu" aria-label="Profiles on this server" @click.stop>
            <h3>Profiles on this server</h3>
            <button
              v-for="a in accounts" :key="a.key" class="acct"
              :class="{ on: a.key === getActiveKey() }"
              :style="tintOf(a) ? { '--tint': tintOf(a) } : undefined"
              :disabled="switching !== ''"
              @click="pickAccount(a)"
            >
              <span class="mini">{{ initialOf(a.name) }}</span>
              <span class="t">{{ a.name }} <span>{{ a.primaryCurrency }}</span></span>
              <span v-if="a.key === getActiveKey()" class="here">here</span>
            </button>
            <button class="acct addnew" @click="addOrCreate">
              <span class="mini">＋</span>
              <span class="t">Add or create<span>back to login / selection</span></span>
            </button>
          </div>
        </div>
      </transition>

      <nav>
        <ul>
          <li><RouterLink to="/" class="on"><div>▤</div>Home</RouterLink></li>
          <li><RouterLink to="/wallets"><div>◈</div>Wallets</RouterLink></li>
          <li><RouterLink to="/book"><div>≡</div>Book</RouterLink></li>
          <li><RouterLink to="/search"><div>⌕</div>Search</RouterLink></li>
          <li><RouterLink to="/settings"><div>⚙</div>Settings</RouterLink></li>
        </ul>
      </nav>
    </template>
    <template v-else>
      <main class="fatal"><p class="muted">Loading…</p></main>
    </template>
  </div>
</template>

<script lang="ts">
import { getActive } from '../api/storage'
function getActiveKey(): string | null {
  return getActive(localStorage)?.key ?? null
}
export default { name: 'AppShell' }
</script>

<style scoped>
.shell {
  max-width: 420px;
  margin: 0 auto;
  min-height: 100dvh;
  display: flex;
  flex-direction: column;
}
header.top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.9rem 1rem 0.7rem;
  border-bottom: 2px solid var(--accent);
}
header.top .left h1 {
  margin: 0;
  font-family: Georgia, 'Times New Roman', serif;
  font-size: 1.15rem;
  letter-spacing: 0.02em;
}
header.top .who { color: var(--muted); font-style: italic; font-size: 0.8rem; font-family: Georgia, serif; }
.avatar {
  width: 2.4rem; height: 2.4rem; border-radius: 50%;
  background: var(--accent-soft); color: var(--accent);
  display: grid; place-items: center;
  font-family: Georgia, serif; font-weight: 700; font-size: 1.1rem;
  border: 1px solid var(--border); cursor: pointer; flex: none;
}
.avatar:hover { border-color: var(--accent); }

/* global error area */
.alert {
  display: flex; justify-content: space-between; align-items: center; gap: 0.5rem;
  border: 1px solid var(--error); color: var(--error);
  border-radius: 10px; padding: 0.5rem 0.75rem; margin: 0.75rem 1rem 0;
  font-size: 0.85rem;
}
.alert button { border: 0; background: transparent; color: inherit; cursor: pointer; padding: 0.2rem; }

/* profile sheet */
.profile-backdrop {
  position: fixed; inset: 0; z-index: 40;
  background: color-mix(in srgb, #000 35%, transparent);
}
.sheet {
  position: absolute; right: 0.5rem; top: 3.6rem; width: 16rem;
  background: var(--surface); border: 1px solid var(--border);
  border-radius: 12px; box-shadow: 0 12px 32px rgba(0, 0, 0, 0.28);
  padding: 0.5rem; z-index: 50;
}
.sheet h3 {
  margin: 0.2rem 0.5rem 0.4rem; font-size: 0.66rem;
  letter-spacing: 0.12em; text-transform: uppercase; color: var(--muted);
}
.acct {
  display: flex; align-items: center; gap: 0.6rem; width: 100%;
  padding: 0.5rem; border: 0; border-radius: 9px;
  background: transparent; color: var(--fg);
  font: inherit; font-size: 0.85rem; text-align: left; cursor: pointer;
}
.acct:hover { background: var(--accent-soft); }
.acct.on { background: var(--accent-soft); }
.acct .mini {
  width: 1.7rem; height: 1.7rem; border-radius: 50%;
  background: var(--accent-soft); border: 1px solid var(--border);
  display: grid; place-items: center; font-size: 0.75rem; font-weight: 700; flex: none;
}
.acct .t { flex: 1; }
.acct .t span { display: block; font-size: 0.68rem; color: var(--muted); }
.acct .here { color: var(--accent); font-size: 0.7rem; font-variant: small-caps; }
.acct.addnew {
  border-top: 1px solid var(--border); border-radius: 0;
  margin-top: 0.25rem; padding-top: 0.6rem; color: var(--accent);
}
.acct.addnew .mini { color: var(--accent); }

/* content */
.content { flex: 1; padding: 0 1rem 5rem; width: 100%; box-sizing: border-box; }

/* bottom nav — spread to content width (demo5 pattern) */
nav {
  position: fixed; bottom: 0; left: 0; right: 0;
  background: light-dark(rgba(246, 247, 249, 0.92), rgba(14, 18, 24, 0.92));
  backdrop-filter: blur(10px);
  border-top: 1px solid var(--border);
  z-index: 30;
}
nav ul { display: flex; list-style: none; margin: 0 auto; padding: 0.55rem 0.35rem 0.75rem; max-width: 420px; }
nav li { flex: 1; }
nav a {
  display: flex; flex-direction: column; align-items: center; gap: 0.18rem;
  color: var(--muted); text-decoration: none;
  font-size: 0.62rem; letter-spacing: 0.06em; text-transform: uppercase;
}
nav a div { font-size: 1.2rem; line-height: 1; }
nav a.router-link-active { color: var(--accent); }

.fatal { padding: 2rem 1rem; display: flex; flex-direction: column; gap: 0.75rem; }
.muted { color: var(--muted); }
.err { color: var(--error); }
.fatal button {
  font: inherit; border: 1px solid var(--border); border-radius: 8px;
  background: var(--surface); color: inherit; padding: 0.5rem 0.9rem; cursor: pointer;
}

.fade-enter-active, .fade-leave-active { transition: opacity 0.15s ease; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
</style>
