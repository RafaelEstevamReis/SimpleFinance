// Book logic: one wallet, one closed month. Everything here is pure — the
// screen wires it to the client, the tests pin the doctrine.
//
// Doctrine: the search is fetched UNFILTERED by wallet (one call, whole
// month), transfers are paired FIRST, and only then does the view pick the
// leg each wallet sees. Filtering at the wire would cut a transfer pair in
// half — the partner lives in another wallet.

import type { TransactionRequest, TransactionResponse } from './types'

/// One display row: a single transaction, or a transfer seen from one of
/// its legs. `tx` is the leg the viewed wallet owns; `other` is the far leg
export interface BookRow {
  kind: 'tx' | 'transfer'
  ids: number[]
  tx: TransactionResponse
  /// The other leg, only on a transfer row
  other?: TransactionResponse
  /// Running Net of the viewed wallet, after this row
  running: number
}

/// Collapse transfer pairs: the search returns both legs as plain rows,
/// the book shows one line per movement. `typeOtherId` cross-links them.
/// A leg whose partner is missing (its month window differs) still shows
export function pairTransfers(txs: TransactionResponse[]): Omit<BookRow, 'running'>[] {
  const rows: Omit<BookRow, 'running'>[] = []
  const seen = new Set<number>()

  for (const tx of txs) {
    if (seen.has(tx.id)) continue

    if (tx.type !== 'WalletTransfer' || tx.typeOtherId === 0) {
      rows.push({ kind: 'tx', ids: [tx.id], tx })
      continue
    }

    const partner = txs.find(t => t.id === tx.typeOtherId)
    if (partner === undefined) {
      rows.push({ kind: 'transfer', ids: [tx.id], tx })
      continue
    }

    seen.add(partner.id)
    // Source is negative, destination positive — the negative leg owns the row
    const [source, destination] = tx.dueValue < 0 ? [tx, partner] : [partner, tx]
    rows.push({ kind: 'transfer', ids: [source.id, destination.id], tx: source, other: destination })
  }

  return rows
}

/// The book of one wallet: pair first, then keep the rows this wallet
/// takes part in, with `tx` set to ITS OWN leg. A transfer shows once per
/// wallet it touches, with the value that wallet saw — the source leg is
/// negative, the destination leg positive
export function walletBook(txs: TransactionResponse[], walletId: number): BookRow[] {
  const rows: Omit<BookRow, 'running'>[] = []
  for (const r of pairTransfers(txs)) {
    if (r.kind === 'tx') {
      if (r.tx.walletId !== walletId) continue
      rows.push(r)
      continue
    }
    // transfer: pick the leg this wallet owns
    if (r.tx.walletId === walletId) {
      rows.push(r)
    } else if (r.other !== undefined && r.other.walletId === walletId) {
      rows.push({ kind: 'transfer', ids: r.ids, tx: r.other, other: r.tx })
    }
  }

  let net = 0
  return rows.map(row => {
    net += row.tx.effectiveValue
    return { ...row, running: net }
  })
}

/// "Oct 05" — short month + day of the row's effective date, en-US style
export function rowDate(iso: string): string {
  const d = new Date(iso)
  const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
  return `${months[d.getUTCMonth()]} ${String(d.getUTCDate()).padStart(2, '0')}`
}

/// Build the request body from the form's positive inputs. The sign is the
/// category's business — never send negative. Returns the error message
/// instead of throwing: the form owns the wording surface
export function buildTransactionRequest(form: {
  walletId: number
  categoryId: number
  counterpartyId: number
  dueDate: string
  paymentDate: string
  description: string
  status: 'Unpaid' | 'Paid' | 'Reversed'
  dueValue: string
  paidValue: string
  paymentDetails: string
}): { request: null; error: string } | { request: TransactionRequest; error: null } {
  if (form.walletId === 0) return { request: null, error: 'Wallet is required' }
  if (form.categoryId === 0) return { request: null, error: 'Category is required' }
  if (form.description.trim() === '') return { request: null, error: 'Description is required' }

  const due = Number(form.dueValue)
  if (!Number.isFinite(due)) return { request: null, error: 'Due value must be a number' }
  if (due < 0) return { request: null, error: 'Due value is sent positive — the category decides the side' }
  if (due === 0) return { request: null, error: 'Due value cannot be zero' }

  // Paid with an empty paid value assumes the due — the common "paid in
  // full"; a partial payment types its own number
  const paid = form.status === 'Paid'
    ? (form.paidValue.trim() === '' ? due : Number(form.paidValue))
    : 0
  if (form.status === 'Paid' && !Number.isFinite(paid)) return { request: null, error: 'Paid value must be a number' }
  if (form.status === 'Paid' && paid < 0) return { request: null, error: 'Paid value is sent positive — the category decides the side' }

  return {
    request: {
      walletId: form.walletId,
      categoryId: form.categoryId,
      counterpartyId: form.counterpartyId,
      invoiceId: 0,
      dueDate: form.dueDate,
      paymentDate: form.paymentDate,
      description: form.description.trim(),
      status: form.status,
      paymentCurrency: '',
      dueValue: due,
      paidValue: paid,
      paymentDetails: form.paymentDetails.trim() === '' ? null : form.paymentDetails.trim(),
      externalIdentifier: null,
    },
    error: null,
  }
}
