// DTO mirrors — copied from the WebApi source, never guessed.
// Source: src/WebApi/DTOs/AccountDtos.cs + Controllers/HelloController.cs
// Dates arrive as explicit UTC (UtcDateTimeConverter); here they are strings
// until a consumer needs a Date — parse at the edge, keep the wire shape raw.

export interface HelloResponse {
  message: string
  service: string
  version: string
  utcNow: string
}

/// POST /api/account — request body (anonymous)
export interface CreateAccountRequest {
  name: string
}

/// POST /api/account — response. The Key appears here and nowhere else.
export interface CreateAccountResponse {
  key: string
  name: string
  created: string
}

/// GET /api/account — the account behind the current Key
export interface AccountResponse {
  key: string
  name: string
  created: string
  lastAccess: string
}

/// GET /api/account/database — finance database state of an account
export interface AccountDatabaseResponse {
  fileName: string
  sizeBytes: number
  walletCount: number
  lastBackup: string | null
  lastBackupUtc: string | null
}

/// GET /api/account/preferences — one row per preference
export interface PreferenceResponse {
  name: string
  value: string
}

/// PUT /api/account/preferences/{name} — request body
export interface PreferenceRequest {
  value: string
}

/// GET /api/wallets — one wallet row
export interface WalletResponse {
  id: number
  externallType: number
  name: string
  description: string
  baseCurrency: string
  isDeleted: boolean
}

/// POST /api/wallets + PUT /api/wallets/{id} — request body
export interface WalletRequest {
  externallType: number
  name: string
  description: string
  baseCurrency: string
  isDeleted: boolean
}

/// GET /api/wallets/{id}/balance + /api/wallets/balances — always in the
/// wallet's own BaseCurrency; different currencies are never summed
export interface WalletBalanceResponse {
  walletId: number
  name: string
  baseCurrency: string
  balance: number
}

/// GET /api/categories — one category row
export interface CategoryResponse {
  id: number
  isExpense: boolean
  name: string
  description: string
  monthlyBudget: number
  isDeleted: boolean
}

/// POST /api/categories + PUT /api/categories/{id} — request body
export interface CategoryRequest {
  isExpense: boolean
  name: string
  description: string
  monthlyBudget: number
  isDeleted: boolean
}

/// GET /api/transactions — one transaction row. Enums travel as names
export interface TransactionResponse {
  id: number
  walletId: number
  categoryId: number
  counterpartyId: number
  invoiceId: number
  type: string
  typeOtherId: number
  status: string
  dueDate: string
  paymentDate: string
  effectiveDate: string
  description: string
  paymentCurrency: string
  dueValue: number
  paidValue: number
  effectiveValue: number
  paymentDetails: string | null
  externalIdentifier: string | null
  created: string
  changed: string
}
