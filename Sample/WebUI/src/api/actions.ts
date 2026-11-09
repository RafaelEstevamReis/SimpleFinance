// Quick actions — the single-row power moves of a due row, built where the
// money is already on screen. Everything here is pure: builders take a row
// and give back the exact request body; the views only fire it.
//
// Doctrine: the update travels through the FULL request — the wire has no
// patch verb, so the row's every field is rebuilt and only what the action
// owns is touched. A transfer is ONE decision: every quick action on a
// transfer leg crosses the PAIR via UpdateTransferRequest, because
// transfers never travel through /api/transactions (M7). A simple row with
// no category cannot be updated at all — the server refuses CategoryId 0 —
// so it gets no quick moves until it is categorized. "Pay as today" is
// today: the row may move to another month, and `movesTo` says where —
// a row that vanishes would look like a bug, not a feature.

import type {
  TransactionRequest,
  TransactionResponse,
  UpdateTransferRequest,
} from './types'

/// What one action does to a row — the exact body, plus what to tell the
/// user afterwards
export interface QuickAction {
  /// null ⇒ the row is a transfer leg, fire `updateTransfer` instead
  request: TransactionRequest | null
  /// Transfer body — both legs in one call
  transferRequest: UpdateTransferRequest | null
  /// Human month name ("Oct") when the action changes the row's month
  movesTo: string | null
}

const NONE: QuickAction = { request: null, transferRequest: null, movesTo: null }

/// Rebuild the full wire request from a row, changing only what the action
/// owns. The sign is the category's business on the server; the wire
/// carries magnitudes, so due/paid keep the row's own absolute value
function requestFromRow(
  tx: TransactionResponse,
  changes: Partial<TransactionRequest>,
): TransactionRequest {
  return {
    walletId: tx.walletId,
    categoryId: tx.categoryId,
    counterpartyId: tx.counterpartyId,
    invoiceId: tx.invoiceId,
    dueDate: tx.dueDate,
    paymentDate: tx.paymentDate,
    description: tx.description,
    status: tx.status as TransactionRequest['status'],
    paymentCurrency: tx.paymentCurrency,
    dueValue: Math.abs(tx.dueValue),
    paidValue: Math.abs(tx.paidValue),
    paymentDetails: tx.paymentDetails,
    externalIdentifier: tx.externalIdentifier,
    ...changes,
  }
}

/// Transfer body from one leg — the server finds the pair by either leg id
/// and re-signs both. paidValue 0 unless the action pays
function transferFromRow(
  tx: TransactionResponse,
  changes: Partial<UpdateTransferRequest>,
): UpdateTransferRequest {
  return {
    dueValue: Math.abs(tx.dueValue),
    paidValue: Math.abs(tx.paidValue),
    dueDate: tx.dueDate,
    paymentDate: tx.paymentDate,
    description: tx.description,
    paymentDetails: tx.paymentDetails,
    status: tx.status as UpdateTransferRequest['status'],
    ...changes,
  }
}

const MONTHS = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']

/// "Oct" for a wire date — null when it does not parse
export function monthOf(iso: string): string | null {
  const d = new Date(iso)
  if (Number.isNaN(d.getTime())) return null
  return MONTHS[d.getMonth()]
}

/// Pay on the due date: settled without moving a cent of the timeline —
/// the row stays in the month it was always in
export function payOnDue(tx: TransactionResponse): QuickAction {
  if (tx.status !== 'Unpaid') return NONE
  const changes = {
    status: 'Paid' as const,
    paidValue: Math.abs(tx.dueValue),
    paymentDate: tx.dueDate,
  }
  if (tx.typeOtherId !== 0) {
    return { request: null, transferRequest: transferFromRow(tx, changes), movesTo: null }
  }
  return { request: requestFromRow(tx, changes), transferRequest: null, movesTo: null }
}

/// Pay as today: `now` is an ISO instant. EffectiveDate becomes today, so a
/// row due last month lands in this month — `movesTo` tells the user where
export function payAsToday(tx: TransactionResponse, now: string): QuickAction {
  if (tx.status !== 'Unpaid') return NONE
  const movesTo = monthOf(now) !== monthOf(tx.dueDate) ? monthOf(now) : null
  const changes = {
    status: 'Paid' as const,
    paidValue: Math.abs(tx.dueValue),
    paymentDate: now,
  }
  if (tx.typeOtherId !== 0) {
    return { request: null, transferRequest: transferFromRow(tx, changes), movesTo }
  }
  return { request: requestFromRow(tx, changes), transferRequest: null, movesTo }
}

/// Reverse a due row. Transfers cross the pair in one call — both legs
/// flip to Reversed together, or the pair would lie
export function reverseRow(tx: TransactionResponse): QuickAction {
  if (tx.status !== 'Unpaid') return NONE
  if (tx.typeOtherId !== 0) {
    return {
      request: null,
      transferRequest: transferFromRow(tx, { status: 'Reversed', paidValue: 0 }),
      movesTo: null,
    }
  }
  return { request: requestFromRow(tx, { status: 'Reversed' }), transferRequest: null, movesTo: null }
}

/// Undo a reversal — the enum is terminal on the server but the update is
/// a legal way back; a reversed-in-error row must not be a dead-end
export function unreverseRow(tx: TransactionResponse): QuickAction {
  if (tx.status !== 'Reversed') return NONE
  if (tx.typeOtherId !== 0) {
    return {
      request: null,
      transferRequest: transferFromRow(tx, { status: 'Unpaid', paidValue: 0 }),
      movesTo: null,
    }
  }
  return { request: requestFromRow(tx, { status: 'Unpaid' }), transferRequest: null, movesTo: null }
}

/// Which quick moves a row offers. Due rows get all three; a reversed row
/// gets only the way back; a paid row is settled — its power move is the
/// edit form. A simple row with no category gets NOTHING: the server
/// refuses CategoryId 0 on update, so it must be categorized first
export type QuickActionName = 'payDue' | 'payToday' | 'reverse' | 'unreverse'

export function availableActions(tx: TransactionResponse): QuickActionName[] {
  if (tx.status === 'Unpaid' && tx.typeOtherId === 0 && tx.categoryId === 0) return []
  if (tx.status === 'Unpaid') return ['payDue', 'payToday', 'reverse']
  if (tx.status === 'Reversed') return ['unreverse']
  return []
}
