// HTTP client over the WebApi. One instance per active server.
// Errors are classified — the Connect screen shows the reason, never a guess.

import type {
  AccountDatabaseResponse,
  AccountResponse,
  CreateAccountResponse,
  HelloResponse,
  PreferenceRequest,
  PreferenceResponse,
} from './types'

export type ApiErrorKind =
  | 'unreachable'      // network/DNS — server not there
  | 'not-a-webapi'     // answered, but not our API (no valid hello/problem shape)
  | 'unauthorized'     // 401 — Key rejected
  | 'bad-request'      // 400 — ProblemDetails, validation
  | 'not-found'        // 404 — wrong path/version
  | 'server-error'     // 5xx
  | 'unknown'

export class ApiError extends Error {
  readonly kind: ApiErrorKind
  readonly status: number | null
  /** ProblemDetails.detail when the API sent one */
  readonly detail: string | null

  constructor(kind: ApiErrorKind, message: string, status: number | null = null, detail: string | null = null) {
    super(message)
    this.name = 'ApiError'
    this.kind = kind
    this.status = status
    this.detail = detail
  }
}

interface ProblemDetails {
  title?: string
  detail?: string
  [k: string]: unknown
}

export class ApiClient {
  readonly baseUrl: string
  private readonly key: string | null

  constructor(baseUrl: string, key: string | null = null) {
    this.baseUrl = baseUrl.replace(/\/+$/, '')
    this.key = key
  }

  private async request<T>(path: string, init?: RequestInit): Promise<T> {
    let response: Response
    try {
      response = await fetch(this.baseUrl + path, {
        ...init,
        headers: {
          Accept: 'application/json',
          'Content-Type': 'application/json',
          ...(this.key !== null ? { 'X-Api-Key': this.key } : {}),
          ...(init?.headers ?? {}),
        },
      })
    } catch (e) {
      throw new ApiError(
        'unreachable',
        `Could not reach ${this.baseUrl}: ${(e as Error).message}`,
      )
    }

    if (response.ok) {
      return (await response.json()) as T
    }

    throw await ApiClient.errorFrom(response)
  }

  private static async errorFrom(response: Response): Promise<ApiError> {
    let body: ProblemDetails | null = null
    try {
      body = (await response.json()) as ProblemDetails
    } catch {
      // non-JSON error page
    }

    let kind: ApiErrorKind = 'unknown'
    if (response.status === 401) kind = 'unauthorized'
    else if (response.status === 400) kind = 'bad-request'
    else if (response.status === 404) kind = 'not-found'
    else if (response.status >= 500) kind = 'server-error'
    else if (body !== null && (typeof body.title === 'string' || typeof body.detail === 'string')) {
      kind = 'not-a-webapi'
    }

    const detail = typeof body?.detail === 'string' ? body.detail : null
    const title = typeof body?.title === 'string' ? body.title : null
    const message = detail ?? title ?? `HTTP ${response.status} ${response.statusText}`

    return new ApiError(kind, message, response.status, detail)
  }

  // -- anonymous endpoints ----------------------------------------------

  async hello(): Promise<HelloResponse> {
    return this.request<HelloResponse>('/api/hello')
  }

  async createAccount(name: string): Promise<CreateAccountResponse> {
    return this.request<CreateAccountResponse>('/api/account', {
      method: 'POST',
      body: JSON.stringify({ name }),
    })
  }

  // -- Key-authenticated endpoints --------------------------------------

  async account(): Promise<AccountResponse> {
    return this.request<AccountResponse>('/api/account')
  }

  async accountDatabase(): Promise<AccountDatabaseResponse> {
    return this.request<AccountDatabaseResponse>('/api/account/database')
  }

  async getPreferences(): Promise<PreferenceResponse[]> {
    return this.request<PreferenceResponse[]>('/api/account/preferences')
  }

  async setPreference(name: string, request: PreferenceRequest): Promise<PreferenceResponse> {
    return this.request<PreferenceResponse>(`/api/account/preferences/${name}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(request),
    })
  }
}
