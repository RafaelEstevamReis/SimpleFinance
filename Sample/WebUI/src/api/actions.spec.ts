// Quick actions: the request must be the row REBUILT, never a sketch —
// one lost field is silent corruption on the wire. And the transfer pair
// is reversed as ONE decision, never a leg alone.
import { describe, expect, it } from 'vitest'
import {
  availableActions,
  monthOf,
  payAsToday,
  payOnDue,
  reverseRow,
  unreverseRow,
} from './actions'
import type { TransactionResponse } from './types'

/// A minimal unpaid row with every field set — builders must keep them all
function row(over: Partial<TransactionResponse> = {}): TransactionResponse {
  return {
    id: 7,
    walletId: 1,
    categoryId: 2,
    counterpartyId: 3,
    invoiceId: 0,
    type: 'Simple',
    typeOtherId: 0,
    status: 'Unpaid',
    dueDate: '2026-11-05T00:00:00Z',
    paymentDate: '2026-11-05T00:00:00Z',
    effectiveDate: '2026-11-05T00:00:00Z',
    description: 'Electric bill',
    paymentCurrency: 'BRL',
    dueValue: -180,
    paidValue: 0,
    effectiveValue: -180,
    paymentDetails: 'auto-debit',
    externalIdentifier: 'FIT-9',
    created: '2026-11-01T00:00:00Z',
    changed: '2026-11-01T00:00:00Z',
    ...over,
  }
}

describe('pay on due date', () => {
  it('settles on the due date — paidValue is the due magnitude, sign stays the wire\'s job', () => {
    const a = payOnDue(row())
    expect(a.request).not.toBeNull()
    expect(a.request!.status).toBe('Paid')
    expect(a.request!.paidValue).toBe(180)
    expect(a.request!.paymentDate).toBe('2026-11-05T00:00:00Z')
    expect(a.movesTo).toBeNull()
  })

  it('crosses the PAIR on a transfer leg — the pay action never travels through /api/transactions', () => {
    const a = payOnDue(row({ typeOtherId: 4 }))
    expect(a.request).toBeNull()
    expect(a.transferRequest).not.toBeNull()
    expect(a.transferRequest!.status).toBe('Paid')
    expect(a.transferRequest!.paidValue).toBe(180)
    expect(a.transferRequest!.paymentDate).toBe('2026-11-05T00:00:00Z')
  })

  it('rebuilds EVERY field — one lost field is corruption on the wire', () => {
    const a = payOnDue(row())
    expect(a.request).toMatchObject({
      walletId: 1,
      categoryId: 2,
      counterpartyId: 3,
      invoiceId: 0,
      dueDate: '2026-11-05T00:00:00Z',
      description: 'Electric bill',
      paymentCurrency: 'BRL',
      dueValue: 180,
      paymentDetails: 'auto-debit',
      externalIdentifier: 'FIT-9',
    })
  })

  it('refuses a row that is not due', () => {
    const a = payOnDue(row({ status: 'Paid' }))
    expect(a.request).toBeNull()
  })
})

describe('pay as today', () => {
  it('is today: paymentDate is the given instant', () => {
    const a = payAsToday(row(), '2026-11-08T14:30:00Z')
    expect(a.request!.status).toBe('Paid')
    expect(a.request!.paymentDate).toBe('2026-11-08T14:30:00Z')
    expect(a.movesTo).toBeNull()
  })

  it('flags the month move — a row that leaves the screen says where it went', () => {
    const a = payAsToday(row(), '2026-12-01T10:00:00Z')
    expect(a.movesTo).toBe('Dec')
  })

  it('no flag when today is the same month as the due date', () => {
    const a = payAsToday(row({ dueDate: '2026-10-05T00:00:00Z' }), '2026-10-31T23:00:00Z')
    expect(a.movesTo).toBeNull()
  })
})

describe('reverse', () => {
  it('flips a simple row to Reversed without touching values or dates', () => {
    const a = reverseRow(row())
    expect(a.request!.status).toBe('Reversed')
    expect(a.request!.dueValue).toBe(180)
    expect(a.request!.paymentDate).toBe('2026-11-05T00:00:00Z')
    expect(a.transferRequest).toBeNull()
  })

  it('crosses the PAIR on a transfer leg — one body, both legs, never a lone leg', () => {
    const a = reverseRow(row({ typeOtherId: 4, dueValue: -300 }))
    expect(a.request).toBeNull()
    expect(a.transferRequest).not.toBeNull()
    expect(a.transferRequest!.status).toBe('Reversed')
    expect(a.transferRequest!.dueValue).toBe(300)
    expect(a.transferRequest!.paidValue).toBe(0)
  })

  it('refuses to reverse a settled row', () => {
    expect(reverseRow(row({ status: 'Paid' })).request).toBeNull()
  })
})

describe('unreverse', () => {
  it('is the way back — a reversed row is never a dead-end', () => {
    const a = unreverseRow(row({ status: 'Reversed', paidValue: 0 }))
    expect(a.request!.status).toBe('Unpaid')
  })

  it('undo on a transfer pair travels the pair endpoint too', () => {
    const a = unreverseRow(row({ typeOtherId: 4, status: 'Reversed' }))
    expect(a.request).toBeNull()
    expect(a.transferRequest!.status).toBe('Unpaid')
  })
})

describe('availableActions', () => {
  it('a due row gets all three power moves', () => {
    expect(availableActions(row())).toEqual(['payDue', 'payToday', 'reverse'])
  })

  it('a reversed row gets only the way back', () => {
    expect(availableActions(row({ status: 'Reversed' }))).toEqual(['unreverse'])
  })

  it('a simple row with no category gets NOTHING — the server refuses CategoryId 0 on update', () => {
    expect(availableActions(row({ categoryId: 0 }))).toEqual([])
  })

  it('a paid row is settled — the edit form is its power move, not quick actions', () => {
    expect(availableActions(row({ status: 'Paid' }))).toEqual([])
  })
})

describe('monthOf', () => {
  it('reads the short month name of a wire date', () => {
    expect(monthOf('2026-11-08T00:00:00Z')).toBe('Nov')
    expect(monthOf('2026-01-31T23:00:00Z')).toBe('Jan')
  })

  it('null on garbage', () => {
    expect(monthOf('nope')).toBeNull()
  })
})
