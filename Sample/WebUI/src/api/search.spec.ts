// Advanced search (M10): the /by filters shaped for the wire, pinned.
// Every id is tri-state (All / one / None), they combine with AND, and
// 'limit' never travels without its 'order' — the server's own rule,
// said by the form first.
import { describe, expect, it } from 'vitest'
import { buildSearchQuery, resultRows, type SearchFormState } from './search'
import type { TransactionResponse } from './types'

function state(over: Partial<SearchFormState> = {}): SearchFormState {
  return {
    start: '2026-11-01T00:00:00Z',
    end: '2026-11-30T23:59:59Z',
    dateType: 'EffectiveDate',
    walletId: 0,
    categoryId: -1,
    counterpartyId: -1,
    order: 'Asc',
    limit: '',
    ...over,
  }
}

function tx(over: Partial<TransactionResponse> = {}): TransactionResponse {
  return {
    id: 1,
    walletId: 1,
    categoryId: 2,
    counterpartyId: 0,
    invoiceId: 0,
    type: 'Simple',
    typeOtherId: 0,
    status: 'Paid',
    dueDate: '2026-11-05T00:00:00Z',
    paymentDate: '2026-11-05T00:00:00Z',
    effectiveDate: '2026-11-05T00:00:00Z',
    description: 'market run',
    paymentCurrency: 'BRL',
    dueValue: -50,
    paidValue: -50,
    effectiveValue: -50,
    paymentDetails: null,
    externalIdentifier: null,
    created: '2026-11-05T00:00:00Z',
    changed: '2026-11-05T00:00:00Z',
    ...over,
  }
}

describe('buildSearchQuery', () => {
  it('always sends the window and the date type — the wire has no defaults', () => {
    const { query, error } = buildSearchQuery(state())
    expect(error).toBeNull()
    expect(query).toEqual({
      start: '2026-11-01T00:00:00Z',
      end: '2026-11-30T23:59:59Z',
      dateType: 'EffectiveDate',
      order: 'Asc',
    })
  })

  it('walletId 0 means All — omitted from the query', () => {
    expect(buildSearchQuery(state()).query!.walletId).toBeUndefined()
  })

  it('a specific wallet travels', () => {
    expect(buildSearchQuery(state({ walletId: 3 })).query!.walletId).toBe(3)
  })

  it('categoryId 0 is NOT All — it is None, the transfer legs, and it travels as 0', () => {
    const q = buildSearchQuery(state({ categoryId: 0 })).query!
    expect(q.categoryId).toBe(0)
  })

  it('categoryId -1 is All — omitted from the query', () => {
    expect(buildSearchQuery(state()).query!.categoryId).toBeUndefined()
  })

  it('counterpartyId travels tri-state the same way', () => {
    expect(buildSearchQuery(state()).query!.counterpartyId).toBeUndefined()
    expect(buildSearchQuery(state({ counterpartyId: 0 })).query!.counterpartyId).toBe(0)
    expect(buildSearchQuery(state({ counterpartyId: 5 })).query!.counterpartyId).toBe(5)
  })

  it('the ids combine — wallet AND category travel together', () => {
    const q = buildSearchQuery(state({ walletId: 1, categoryId: 2 })).query!
    expect(q.walletId).toBe(1)
    expect(q.categoryId).toBe(2)
  })

  it('an empty limit is omitted', () => {
    expect(buildSearchQuery(state()).query!.limit).toBeUndefined()
  })

  it('a whole limit of at least 1 travels', () => {
    expect(buildSearchQuery(state({ limit: '25' })).query!.limit).toBe(25)
  })

  it('refuses a limit below 1 — the form says it, not the 400', () => {
    expect(buildSearchQuery(state({ limit: '0' })).error).toBe("'limit' must be a whole number of at least 1.")
    expect(buildSearchQuery(state({ limit: '-3' })).error).toBe("'limit' must be a whole number of at least 1.")
    expect(buildSearchQuery(state({ limit: '2.5' })).error).toBe("'limit' must be a whole number of at least 1.")
  })

  it('refuses a window without both dates', () => {
    expect(buildSearchQuery(state({ start: '' })).error).toBe('The window needs both dates.')
    expect(buildSearchQuery(state({ end: '' })).error).toBe('The window needs both dates.')
  })

  it('refuses an inverted window', () => {
    const { query, error } = buildSearchQuery(state({ start: '2026-12-01T00:00:00Z', end: '2026-11-01T00:00:00Z' }))
    expect(query).toBeNull()
    expect(error).toBe('The end of the window is before its start.')
  })
})

describe('resultRows', () => {
  it('pairs transfer legs first — one movement, one row', () => {
    const rows = resultRows([
      tx({ id: 4, typeOtherId: 5, type: 'WalletTransfer', dueValue: -300, effectiveValue: -300 }),
      tx({ id: 5, typeOtherId: 4, type: 'WalletTransfer', walletId: 2, categoryId: 0, dueValue: 300, effectiveValue: 300 }),
    ])
    expect(rows).toHaveLength(1)
    expect(rows[0].kind).toBe('transfer')
    expect(rows[0].ids).toEqual([4, 5])
  })

  it('a pair cut by the window still shows — the orphan leg is a fact, not a crash', () => {
    const rows = resultRows([tx({ id: 4, typeOtherId: 5, type: 'WalletTransfer' })])
    expect(rows).toHaveLength(1)
    expect(rows[0].other).toBeUndefined()
  })

  it('plain rows stay plain', () => {
    const rows = resultRows([tx(), tx({ id: 2 })])
    expect(rows).toHaveLength(2)
    expect(rows.every(r => r.kind === 'tx')).toBe(true)
  })
})
