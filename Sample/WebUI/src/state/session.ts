// Session state — what the whole shell needs to know about the connected
// account, in one place. The shell (top bar, profile sheet, error area) and
// every screen inside it consume this; views never re-fetch account identity.
//
// Deliberately NOT a store package: a plain module with refs is the whole
// ceremony this app needs (Architecture.md — "no store, no pinia").
//
// Preferences flow one way: the server is the truth. read on connect/mount,
// written with PUT; localStorage keeps preview caches only (theme swatch on
// the connect cards, mode so the dark shell does not flash on reconnect).

import { computed, ref } from 'vue'
import { ApiClient, ApiError } from '../api/client'
import {
  getAccounts,
  getActive,
  getServerUrl,
  getThemePreview,
  saveActive,
  saveThemePreview,
  type StoredAccount,
} from '../api/storage'
import { isAccountTypeName, type AccountTypeName } from '../api/accountTypes'
import { applyMode, clearMode, isModeName, type ModeName } from '../styles/mode'
import { applyTheme, clearTheme, isThemeName, defaultTheme, type ThemeName } from '../styles/themes'

export interface SessionState {
  /** The account behind the Key, or null until the shell loads it */
  accountName: string
  theme: ThemeName
  mode: ModeName
  accountType: AccountTypeName
}

const state = ref<SessionState>({
  accountName: '',
  theme: defaultTheme,
  mode: 'system',
  accountType: 'personal',
})

/** True once the shell has loaded identity + preferences; false on connect screens */
const ready = ref(false)

/** Global error area message — set by any screen, dismissed by the user */
const error = ref('')

export const session = {
  state,
  ready,
  error,
  accountLabel: computed(() => state.value.accountName || 'Account'),
}

/** The saved account the active Key belongs to, if it is still in the list */
export function activeStoredAccount(): StoredAccount | null {
  const active = getActive(localStorage)
  if (!active) return null
  return getAccounts(localStorage).find((a) => a.key === active.key) ?? null
}

/** Client for the active server + Key. Null when not connected. */
export function activeClient(): ApiClient | null {
  const serverUrl = getServerUrl(localStorage)
  const active = getActive(localStorage)
  if (!serverUrl || !active) return null
  return new ApiClient(serverUrl, active.key)
}

/** Parse a GET preferences payload into typed, validated state values */
export function parsePreferences(prefs: { name: string; value: string }[]): Partial<SessionState> {
  const out: Partial<SessionState> = {}
  const theme = prefs.find((p) => p.name === 'theme')?.value
  if (theme !== undefined && isThemeName(theme)) out.theme = theme
  const mode = prefs.find((p) => p.name === 'mode')?.value
  if (mode !== undefined && isModeName(mode)) out.mode = mode
  const type = prefs.find((p) => p.name === 'account-type')?.value
  if (type !== undefined && isAccountTypeName(type)) out.accountType = type
  return out
}

/** Load account identity + all preferences from the server and apply the visuals */
export async function loadSession(): Promise<void> {
  const client = activeClient()
  if (!client) throw new Error('Not connected')
  try {
    const account = await client.account()
    const prefs = parsePreferences(await client.getPreferences())
    // theme/mode fallbacks: server preference first, then local preview, else neutral
    const label = activeStoredAccount()?.label ?? null
    const preview = label !== null ? getThemePreview(localStorage, label) : null
    const theme = prefs.theme ?? (preview !== null && isThemeName(preview) ? preview : null)
    const s = state.value
    s.accountName = account.name
    s.theme = theme ?? defaultTheme
    s.mode = prefs.mode ?? 'system'
    s.accountType = prefs.accountType ?? 'personal'
    if (theme !== null) applyTheme(theme)
    else clearTheme()
    applyMode(s.mode)
  } catch (e) {
    // identity must load for the shell to make sense — let the caller decide
    throw e
  }
}

/** Write one preference to the server and adopt it locally */
export async function setPreference(name: 'theme' | 'mode' | 'account-type', value: string): Promise<void> {
  const client = activeClient()
  if (!client) throw new Error('Not connected')
  await client.setPreference(name, { value })
  if (name === 'theme' && isThemeName(value)) {
    state.value.theme = value
    applyTheme(value)
    const label = activeStoredAccount()?.label ?? null
    if (label) saveThemePreview(localStorage, label, value)
  } else if (name === 'mode' && isModeName(value)) {
    state.value.mode = value
    applyMode(value)
  } else if (name === 'account-type' && isAccountTypeName(value)) {
    state.value.accountType = value
  }
}

/** All saved accounts, for the profile sheet */
export function savedAccounts(): StoredAccount[] {
  return getAccounts(localStorage)
}

/** Switch to another saved account — revalidate, reload, stay in place */
export async function switchAccount(acc: StoredAccount): Promise<void> {
  const serverUrl = getServerUrl(localStorage)
  if (!serverUrl) throw new Error('No server selected')
  const client = new ApiClient(serverUrl, acc.key)
  await client.account() // a stale Key must not become "connected"
  saveActive(localStorage, { key: acc.key })
  ready.value = false
  await loadSession()
  ready.value = true
}

/** Leave the account — clear identity visuals, keep server + saved list */
export function disconnect(): void {
  saveActive(localStorage, null)
  ready.value = false
  clearTheme()
  clearMode()
  error.value = ''
}

/** Initial of a label, for the profile avatar */
export function initialOf(label: string): string {
  const trimmed = label.trim()
  return trimmed === '' ? '?' : trimmed[0].toUpperCase()
}

/** Message for the global error area — ApiError detail wins, never generic */
export function errorMessage(e: unknown): string {
  if (e instanceof ApiError) return e.message
  return (e as Error).message
}
