// Transfer logic: money moving between two wallets, one linked pair edited
// as one. Pure builders in the book's style — the form owns the wording
// surface, the tests pin the doctrine.
//
// Doctrine: the value is always positive on the wire. The server writes the
// negative leg on the source and the positive one on the destination, and
// the pair's wallets/categories are frozen after creation — the update
// request carries neither.

import type { CreateTransferRequest, UpdateTransferRequest } from './types'

/// Build the create body from the form's positive inputs. Category ids are
/// 0 for none; when set, the server demands the right side (source expense,
/// destination income) — the form filters its selects, this only refuses
/// the impossible rest
export function buildCreateTransferRequest(form: {
  sourceWalletId: number
  sourceCategoryId: number
  destinationWalletId: number
  destinationCategoryId: number
  description: string
  value: string
  dueDate: string
  paymentDate: string
  paid: boolean
  paymentDetails: string
}): { request: null; error: string } | { request: CreateTransferRequest; error: null } {
  if (form.sourceWalletId === 0) return { request: null, error: 'Source wallet is required' }
  if (form.destinationWalletId === 0) return { request: null, error: 'Destination wallet is required' }
  if (form.sourceWalletId === form.destinationWalletId) return { request: null, error: 'Source and destination must be different wallets' }
  if (form.description.trim() === '') return { request: null, error: 'Description is required' }

  const value = Number(form.value)
  if (!Number.isFinite(value)) return { request: null, error: 'Value must be a number' }
  if (value < 0) return { request: null, error: 'Value is sent positive — the pair carries both sides' }
  if (value === 0) return { request: null, error: 'Value cannot be zero' }

  return {
    request: {
      sourceWalletId: form.sourceWalletId,
      sourceCategoryId: form.sourceCategoryId,
      destinationWalletId: form.destinationWalletId,
      destinationCategoryId: form.destinationCategoryId,
      description: form.description.trim(),
      value,
      dueDate: form.dueDate,
      paymentDate: form.paymentDate,
      paid: form.paid,
      paymentDetails: form.paymentDetails.trim() === '' ? null : form.paymentDetails.trim(),
    },
    error: null,
  }
}

/// Build the update body from the form's positive inputs. The server
/// re-signs both legs from these magnitudes; wallets and categories are
/// not editable here — they were frozen at creation
export function buildUpdateTransferRequest(form: {
  description: string
  value: string
  paidValue: string
  status: 'Unpaid' | 'Paid' | 'Reversed'
  dueDate: string
  paymentDate: string
  paymentDetails: string
}): { request: null; error: string } | { request: UpdateTransferRequest; error: null } {
  if (form.description.trim() === '') return { request: null, error: 'Description is required' }

  const due = Number(form.value)
  if (!Number.isFinite(due)) return { request: null, error: 'Value must be a number' }
  if (due < 0) return { request: null, error: 'Value is sent positive — the pair carries both sides' }
  if (due === 0) return { request: null, error: 'Value cannot be zero' }

  const paid = form.status === 'Paid' ? Number(form.paidValue) : 0
  if (form.status === 'Paid' && !Number.isFinite(paid)) return { request: null, error: 'Paid value must be a number' }
  if (form.status === 'Paid' && paid < 0) return { request: null, error: 'Paid value is sent positive — the pair carries both sides' }

  return {
    request: {
      dueValue: due,
      paidValue: paid,
      dueDate: form.dueDate,
      paymentDate: form.paymentDate,
      description: form.description.trim(),
      paymentDetails: form.paymentDetails.trim() === '' ? null : form.paymentDetails.trim(),
      status: form.status,
    },
    error: null,
  }
}

/// The pair's locked heading, e.g. "Main Account → Dollars". Wallets are
/// frozen after creation, so the edit form states them instead of asking
export function pairHeading(sourceName: string, destinationName: string): string {
  return `${sourceName} → ${destinationName}`
}
