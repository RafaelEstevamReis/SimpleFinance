// localStorage persistence — spec in Architecture.md (Storage keys table).
// Every read is defensive: corrupt JSON downgrades to the empty state, never
// crashes the app. Keys hold the account credential; treat writes as precious.
//
// Single-server by design: the WebUI points at one API and stays there.
// No server name, no server list — baseUrl alone identifies the server.

export interface StoredAccount {
  label: string
  key: string
  /** Primary currency asked at account creation (ISO 4217), WebUI-side only */
  primaryCurrency: string
}

export interface ActiveSelection {
  key: string
}

const KEYS = {
  server: 'webui.server',
  accounts: 'webui.accounts',
  active: 'webui.active',
  themePreviewPrefix: 'webui.theme.',
} as const

function readJson<T>(storage: Storage, key: string, fallback: T): T {
  try {
    const raw = storage.getItem(key)
    if (raw === null) return fallback
    return JSON.parse(raw) as T
  } catch {
    return fallback
  }
}

function writeJson(storage: Storage, key: string, value: unknown): void {
  storage.setItem(key, JSON.stringify(value))
}

export function getServerUrl(storage: Storage): string | null {
  const v = readJson<unknown>(storage, KEYS.server, null)
  return typeof v === 'string' && v !== '' ? v : null
}

export function getAccounts(storage: Storage): StoredAccount[] {
  const v = readJson<unknown>(storage, KEYS.accounts, [])
  return Array.isArray(v) ? (v as StoredAccount[]) : []
}

export function getActive(storage: Storage): ActiveSelection | null {
  const v = readJson<unknown>(storage, KEYS.active, null)
  if (v === null || typeof v !== 'object') return null
  const s = v as Record<string, unknown>
  if (typeof s.key === 'string') {
    return { key: s.key }
  }
  return null
}

/** Last-seen theme of an account — preview only, server stays the truth */
export function getThemePreview(storage: Storage, accountLabel: string): string | null {
  const v = readJson<unknown>(storage, KEYS.themePreviewPrefix + accountLabel, null)
  return typeof v === 'string' && v !== '' ? v : null
}

export function saveThemePreview(storage: Storage, accountLabel: string, theme: string): void {
  writeJson(storage, KEYS.themePreviewPrefix + accountLabel, theme)
}


export function saveServerUrl(storage: Storage, baseUrl: string): void {
  writeJson(storage, KEYS.server, baseUrl)
}

export function saveAccounts(storage: Storage, accounts: StoredAccount[]): void {
  writeJson(storage, KEYS.accounts, accounts)
}

export function saveActive(storage: Storage, active: ActiveSelection | null): void {
  if (active === null) {
    storage.removeItem(KEYS.active)
  } else {
    writeJson(storage, KEYS.active, active)
  }
}
