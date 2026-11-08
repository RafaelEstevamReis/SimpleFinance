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
