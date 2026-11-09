// Advanced search logic (M10): the /by filters shaped for the wire. Pure —
// the screen wires it to the client, the tests pin the doctrine.
//
// Doctrine: every id filter has THREE states — omitted ("All"), a specific
// id, and 0 ("None" — the rows that carry none; for category that is the
// transfer legs). They combine with AND: every id sent must match. The
// window is always explicit (start/end are non-nullable on the server), and
// 'limit' never travels without its 'order' — the server's own Rejection
// rule, enforced here so the user hears it from the form, not a 400.

import type { TransactionResponse } from './types'
import type { BookRow } from './book'
import { pairTransfers } from './book'

export type DateType = 'DueDate' | 'PaymentDate' | 'EffectiveDate'
export type Order = 'Asc' | 'Desc'

/// The form state, verbatim: id 0 in walletId means "All" (the select's
/// own "All" value), but categoryId/counterpartyId distinguish 0 = "None"
/// from -1 = "All" — they are tri-state by design
export interface SearchFormState {
  start: string
  end: string
  dateType: DateType
  walletId: number
  /// -1 = All, 0 = None (rows with no category — the transfer legs), >0 = that one
  categoryId: number
  /// -1 = All, 0 = None (rows with no counterparty), >0 = that one
  counterpartyId: number
  order: Order
  /// '' = unlimited; anything else is the number the user typed
  limit: string
}

/// What the wire gets: exactly the params the server accepts, nothing else
export type SearchQuery = {
  start: string
  end: string
  dateType: DateType
  walletId?: number
  categoryId?: number
  counterpartyId?: number
  order?: Order
  limit?: number
}

/// Shape the form state into the wire query, or the reason it is not
/// answerable. `limit` follows the server's own Rejection rules so the
/// form says it first
export function buildSearchQuery(state: SearchFormState): { query: null; error: string } | { query: SearchQuery; error: null } {
  if (state.start === '' || state.end === '') {
    return { query: null, error: 'The window needs both dates.' }
  }
  if (state.end < state.start) {
    return { query: null, error: 'The end of the window is before its start.' }
  }
  let limit: number | undefined
  if (state.limit !== '') {
    limit = Number(state.limit)
    if (!Number.isInteger(limit) || limit < 1) {
      return { query: null, error: "'limit' must be a whole number of at least 1." }
    }
  }
  // the state type always carries an order — the server's "limit needs an
  // order" rule cannot be broken by a form whose order select has no "off"
  const query: SearchQuery = {
    start: state.start,
    end: state.end,
    dateType: state.dateType,
    order: state.order,
  }
  if (state.walletId !== 0) query.walletId = state.walletId
  if (state.categoryId !== -1) query.categoryId = state.categoryId
  if (state.counterpartyId !== -1) query.counterpartyId = state.counterpartyId
  if (limit !== undefined) query.limit = limit
  return { query, error: null }
}

/// Result rows: the search answers plain transactions, transfers arrive as
/// two cross-linked legs — they are paired FIRST (the M6 doctrine: a
/// transfer is one movement), and a window or limit may cut a pair in half
/// (the partner fell outside) — the orphan still shows, alone
export function resultRows(txs: TransactionResponse[]): Omit<BookRow, 'running'>[] {
  return pairTransfers(txs)
}
