// HTTP client over the WebApi. One instance per active server.
// Errors are classified — the Connect screen shows the reason, never a guess.

import type {
  AccountDatabaseResponse,
  AccountResponse,
  CategoryRequest,
  CategoryResponse,
  CreateAccountResponse,
  HelloResponse,
  PreferenceRequest,
  PreferenceResponse,
  TransactionResponse,
  WalletBalanceResponse,
  WalletRequest,
  WalletResponse,
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

  // -- wallets -------------------------------------------------------------

  /// GET /api/wallets — every wallet, including soft-deleted ones
  async wallets(): Promise<WalletResponse[]> {
    return this.request<WalletResponse[]>('/api/wallets')
  }

  /// GET /api/wallets/{id} — a single wallet
  async wallet(id: number): Promise<WalletResponse> {
    return this.request<WalletResponse>(`/api/wallets/${id}`)
  }

  /// POST /api/wallets — creates a wallet (201)
  async createWallet(request: WalletRequest): Promise<WalletResponse> {
    return this.request<WalletResponse>('/api/wallets', { method: 'POST', body: JSON.stringify(request) })
  }

  /// PUT /api/wallets/{id} — updates a wallet
  async updateWallet(id: number, request: WalletRequest): Promise<WalletResponse> {
    return this.request<WalletResponse>(`/api/wallets/${id}`, { method: 'PUT', body: JSON.stringify(request) })
  }

  /// GET /api/wallets/{id}/balance — settled balance of one wallet
  async walletBalance(id: number): Promise<WalletBalanceResponse> {
    return this.request<WalletBalanceResponse>(`/api/wallets/${id}/balance`)
  }

  /// GET /api/wallets/balances — every wallet's balance in one call,
  /// each in its own currency; the list screen uses this, not N+1 calls
  async walletBalances(): Promise<WalletBalanceResponse[]> {
    return this.request<WalletBalanceResponse[]>('/api/wallets/balances')
  }

  // -- categories ------------------------------------------------------------

  /// GET /api/categories — every category, including soft-deleted ones
  async categories(): Promise<CategoryResponse[]> {
    return this.request<CategoryResponse[]>('/api/categories')
  }

  /// GET /api/categories/{id} — a single category
  async category(id: number): Promise<CategoryResponse> {
    return this.request<CategoryResponse>(`/api/categories/${id}`)
  }

  /// POST /api/categories — creates a category (201)
  async createCategory(request: CategoryRequest): Promise<CategoryResponse> {
    return this.request<CategoryResponse>('/api/categories', { method: 'POST', body: JSON.stringify(request) })
  }

  /// PUT /api/categories/{id} — updates a category. isExpense never changes
  /// after creation — the server refuses it, the UI keeps it read-only
  async updateCategory(id: number, request: CategoryRequest): Promise<CategoryResponse> {
    return this.request<CategoryResponse>(`/api/categories/${id}`, { method: 'PUT', body: JSON.stringify(request) })
  }

  // -- transactions ----------------------------------------------------------

  /// GET /api/transactions — search by window. dateType picks the ruling
  /// date, kind narrows it, order sorts by that same date, limit truncates
  async searchTransactions(params: {
    start: string
    end: string
    dateType?: 'DueDate' | 'PaymentDate' | 'EffectiveDate'
    kind?: 'Wallet' | 'Category' | 'Counterparty'
    kindId?: number
    order?: 'Asc' | 'Desc'
    limit?: number
  }): Promise<TransactionResponse[]> {
    const query = new URLSearchParams({ start: params.start, end: params.end })
    if (params.dateType) query.set('dateType', params.dateType)
    if (params.kind) query.set('kind', params.kind)
    if (params.kindId !== undefined) query.set('kindId', String(params.kindId))
    if (params.order) query.set('order', params.order)
    if (params.limit !== undefined) query.set('limit', String(params.limit))
    return this.request<TransactionResponse[]>(`/api/transactions?${query.toString()}`)
  }
}
