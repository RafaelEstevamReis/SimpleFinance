import { describe, expect, it } from 'vitest'
import { buildTransactionRequest, pairTransfers, rowDate, walletBook } from './book'
import type { TransactionResponse } from './types'

function tx(partial: Partial<TransactionResponse> & { id: number }): TransactionResponse {
  return {
    walletId: 1, categoryId: 2, counterpartyId: 0, invoiceId: 0,
    type: 'Simple', typeOtherId: 0, status: 'Paid',
    dueDate: '2026-11-05T12:00:00Z', paymentDate: '2026-11-05T12:00:00Z',
    effectiveDate: '2026-11-05T12:00:00Z',
    description: 'x', paymentCurrency: 'BRL',
    dueValue: -10, paidValue: -10, effectiveValue: -10,
    paymentDetails: null, externalIdentifier: null,
    created: '2026-11-05T12:00:00Z', changed: '2026-11-05T12:00:00Z',
    ...partial,
  }
}

const transferPair = [
  tx({ id: 7, walletId: 1, type: 'WalletTransfer', typeOtherId: 8, dueValue: -500, effectiveValue: -500 }),
  tx({ id: 8, walletId: 2, type: 'WalletTransfer', typeOtherId: 7, dueValue: 500, effectiveValue: 500 }),
]

describe('pairTransfers', () => {
  it('collapses both legs into one row owned by the negative leg', () => {
    const rows = pairTransfers(transferPair)
    expect(rows).toHaveLength(1)
    expect(rows[0].kind).toBe('transfer')
    expect(rows[0].tx.id).toBe(7)
    expect(rows[0].other?.id).toBe(8)
    expect(rows[0].ids).toEqual([7, 8])
  })

  it('order of arrival does not change ownership', () => {
    const rows = pairTransfers([...transferPair].reverse())
    expect(rows[0].tx.id).toBe(7)
  })

  it('a leg whose partner is outside the window still shows, once', () => {
    const rows = pairTransfers([
      tx({ id: 9, walletId: 1, type: 'WalletTransfer', typeOtherId: 99, dueValue: -200, effectiveValue: -200 }),
    ])
    expect(rows).toHaveLength(1)
    expect(rows[0].ids).toEqual([9])
  })

  it('plain transactions pass through untouched', () => {
    const rows = pairTransfers([tx({ id: 1 }), tx({ id: 2, dueValue: 3000, effectiveValue: 3000 })])
    expect(rows.map(r => r.kind)).toEqual(['tx', 'tx'])
  })
})

describe('walletBook', () => {
  it('shows the transfer to the source wallet with its own negative leg', () => {
    const rows = walletBook(transferPair, 1)
    expect(rows).toHaveLength(1)
    expect(rows[0].tx.id).toBe(7)
    expect(rows[0].tx.effectiveValue).toBe(-500)
    expect(rows[0].running).toBe(-500)
  })

  it('shows the transfer to the destination wallet with its own positive leg', () => {
    const rows = walletBook(transferPair, 2)
    expect(rows).toHaveLength(1)
    expect(rows[0].tx.id).toBe(8)
    expect(rows[0].tx.effectiveValue).toBe(500)
    expect(rows[0].running).toBe(500)
  })

  it('a wallet never sees the other wallets\' rows', () => {
    const rows = walletBook([
      ...transferPair,
      tx({ id: 10, walletId: 3, effectiveValue: -99 }),
    ], 3)
    expect(rows).toHaveLength(1)
    expect(rows[0].tx.id).toBe(10)
  })

  it('running Net accumulates the viewed wallet\'s own values only', () => {
    const rows = walletBook([
      tx({ id: 1, walletId: 1, effectiveValue: -620 }),
      ...transferPair,
      tx({ id: 2, walletId: 1, effectiveValue: 8500 }),
    ], 1)
    expect(rows.map(r => r.running)).toEqual([-620, -1120, 7380])
  })

  it('an unpaid row counts its DueValue — effective follows status', () => {
    const rows = walletBook([
      tx({ id: 3, walletId: 1, status: 'Unpaid', dueValue: -100, paidValue: 0, effectiveValue: -100 }),
    ], 1)
    expect(rows[0].running).toBe(-100)
  })
})

describe('rowDate', () => {
  it('formats "Oct 05" from the effective date', () => {
    expect(rowDate('2026-10-05T12:00:00Z')).toBe('Oct 05')
  })

  it('pads single-digit days', () => {
    expect(rowDate('2026-11-03T00:00:00Z')).toBe('Nov 03')
  })
})

describe('buildTransactionRequest', () => {
  const form = {
    walletId: 1, categoryId: 2, counterpartyId: 0,
    dueDate: '2026-11-05T12:00:00Z', paymentDate: '2026-11-05T12:00:00Z',
    description: 'market', status: 'Paid' as const,
    dueValue: '620', paidValue: '620', paymentDetails: '',
  }

  it('builds a positive body — the category owns the sign', () => {
    const out = buildTransactionRequest(form)
    expect(out.error).toBeNull()
    expect(out.request?.dueValue).toBe(620)
    expect(out.request?.paidValue).toBe(620)
    expect(out.request?.invoiceId).toBe(0)
  })

  it('unpaid sends paidValue zero whatever the field says', () => {
    const out = buildTransactionRequest({ ...form, status: 'Unpaid', paidValue: '999' })
    expect(out.request?.paidValue).toBe(0)
  })

  it('paid with an empty paid value assumes the due — "paid in full" is one keystroke fewer', () => {
    const out = buildTransactionRequest({ ...form, paidValue: '' })
    expect(out.request?.paidValue).toBe(620)
  })

  it('refuses zero — a movement without a side', () => {
    expect(buildTransactionRequest({ ...form, dueValue: '0' }).error).toBe('Due value cannot be zero')
  })

  it('refuses negative — the side is the category\'s, never the input\'s', () => {
    expect(buildTransactionRequest({ ...form, dueValue: '-5' }).error).toContain('positive')
    expect(buildTransactionRequest({ ...form, paidValue: '-5' }).error).toContain('positive')
  })

  it('refuses missing wallet, category and description', () => {
    expect(buildTransactionRequest({ ...form, walletId: 0 }).error).toBe('Wallet is required')
    expect(buildTransactionRequest({ ...form, categoryId: 0 }).error).toBe('Category is required')
    expect(buildTransactionRequest({ ...form, description: '   ' }).error).toBe('Description is required')
  })

  it('trims and nulls empty optionals', () => {
    const out = buildTransactionRequest({ ...form, description: ' market ', paymentDetails: '  ' })
    expect(out.request?.description).toBe('market')
    expect(out.request?.paymentDetails).toBeNull()
  })
})
