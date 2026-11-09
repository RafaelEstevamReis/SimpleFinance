// localStorage persistence — spec in Architecture.md (Storage keys table).
// Every read is defensive: corrupt JSON downgrades to the empty state, never
// crashes the app. Keys hold the account credential; treat writes as precious.
//
// Single-server by design: the WebUI points at one API and stays there.
// No server name, no server list — baseUrl alone identifies the server.

export interface StoredAccount {
  /** Server's Account.Name — a CACHE, not a truth. The Key is the identity;
   *  every contact with the server (probe, create, connect) refreshes it */
  name: string
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

/// One-time migration: accounts saved before the name unification carried
/// a browser-side `label`. It becomes the `name` cache's seed — the next
/// contact with the server overwrites it with the server's truth
export function getAccounts(storage: Storage): StoredAccount[] {
  const v = readJson<unknown>(storage, KEYS.accounts, [])
  if (!Array.isArray(v)) return []
  return (v as Record<string, unknown>[]).map((raw) => {
    const key = typeof raw.key === 'string' ? raw.key : ''
    const name = typeof raw.name === 'string' ? raw.name : typeof raw.label === 'string' ? raw.label : ''
    const primaryCurrency = typeof raw.primaryCurrency === 'string' ? raw.primaryCurrency : 'USD'
    return { name, key, primaryCurrency }
  })
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

/** Last-seen theme of an account — preview only, server stays the truth.
 *  Keyed by the account KEY: the name is a cache, the Key is the identity */
export function getThemePreview(storage: Storage, accountKey: string): string | null {
  const v = readJson<unknown>(storage, KEYS.themePreviewPrefix + accountKey, null)
  return typeof v === 'string' && v !== '' ? v : null
}

export function saveThemePreview(storage: Storage, accountKey: string, theme: string): void {
  writeJson(storage, KEYS.themePreviewPrefix + accountKey, theme)
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
