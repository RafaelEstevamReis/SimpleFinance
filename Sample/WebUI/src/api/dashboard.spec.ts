import { describe, expect, it } from 'vitest'
import { chartBars, monthSeries, recentRows, upcomingDue } from './dashboard'
import type { TransactionResponse } from './types'

/// A transaction row as the API hands it back; only the fields the
/// dashboard reads are filled
function tx(over: Partial<TransactionResponse>): TransactionResponse {
  return {
    id: 1,
    walletId: 1,
    categoryId: 1,
    counterpartyId: 0,
    invoiceId: 0,
    type: 'Simple',
    typeOtherId: 0,
    status: 'Paid',
    dueDate: '2026-11-08T00:00:00Z',
    paymentDate: '2026-11-08T00:00:00Z',
    effectiveDate: '2026-11-08T12:00:00Z',
    description: 'test',
    paymentCurrency: 'BRL',
    dueValue: -10,
    paidValue: -10,
    effectiveValue: -10,
    paymentDetails: null,
    externalIdentifier: null,
    created: '2026-11-08T12:00:00Z',
    changed: '2026-11-08T12:00:00Z',
    ...over,
  }
}

const short = (iso: string): string => iso.slice(5, 10)

describe('recentRows', () => {
  it('pairs transfers first — one line per movement', () => {
    const src = tx({ id: 4, type: 'WalletTransfer', typeOtherId: 5, categoryId: 0, effectiveValue: -200, description: 'Transfer' })
    const dst = tx({ id: 5, type: 'WalletTransfer', typeOtherId: 4, categoryId: 0, walletId: 2, effectiveValue: 200, description: 'Transfer' })
    const rows = recentRows([src, dst], 5, short)
    expect(rows).toHaveLength(1)
    expect(rows[0].kind).toBe('transfer')
    expect(rows[0].ids).toEqual([4, 5])
  })

  it('breaks same-instant ties by id, newest id first', () => {
    // smoke-caught: a wire-served Asc order leaked into "newest first"
    const rows = recentRows([
      tx({ id: 2, effectiveDate: '2026-11-08T12:00:00Z' }),
      tx({ id: 5, effectiveDate: '2026-11-08T12:00:00Z' }),
      tx({ id: 9, effectiveDate: '2026-11-08T12:00:00Z' }),
    ], 5, short)
    expect(rows.map((r) => r.tx.id)).toEqual([9, 5, 2])
  })

  it('keeps the newest n, newest first', () => {
    const rows = recentRows([
      tx({ id: 1, effectiveDate: '2026-11-01T12:00:00Z' }),
      tx({ id: 2, effectiveDate: '2026-11-08T12:00:00Z' }),
      tx({ id: 3, effectiveDate: '2026-11-05T12:00:00Z' }),
    ], 2, short)
    expect(rows.map((r) => r.tx.id)).toEqual([2, 3])
  })
})

describe('upcomingDue', () => {
  const today = new Date('2026-11-08T00:00:00Z')

  it('lists only unpaid, future by date ascending', () => {
    const rows = upcomingDue([
      tx({ id: 1, status: 'Unpaid', dueDate: '2026-11-20T00:00:00Z' }),
      tx({ id: 2, status: 'Unpaid', dueDate: '2026-11-12T00:00:00Z' }),
      tx({ id: 3, status: 'Paid', dueDate: '2026-11-15T00:00:00Z' }),
    ], today, 5, short)
    expect(rows.map((r) => r.tx.id)).toEqual([2, 1])
  })

  it('puts the overdue first, oldest debt at the top', () => {
    const rows = upcomingDue([
      tx({ id: 1, status: 'Unpaid', dueDate: '2026-11-20T00:00:00Z' }),
      tx({ id: 2, status: 'Unpaid', dueDate: '2026-11-02T00:00:00Z' }),
      tx({ id: 3, status: 'Unpaid', dueDate: '2026-10-15T00:00:00Z' }),
    ], today, 5, short)
    expect(rows.map((r) => r.tx.id)).toEqual([3, 2, 1])
    expect(rows[0].overdueBy).toBe(24)
    expect(rows[0].date).toContain('overdue')
  })

  it('keeps n rows', () => {
    const many = [1, 2, 3, 4, 5].map((id) => tx({ id, status: 'Unpaid', dueDate: `2026-11-1${id}T00:00:00Z` }))
    expect(upcomingDue(many, today, 3, short)).toHaveLength(3)
  })
})

describe('monthSeries', () => {
  it('groups by month, income positive, expense negative', () => {
    const first = new Date('2026-09-01T00:00:00Z')
    const series = monthSeries([
      tx({ id: 1, effectiveDate: '2026-09-10T12:00:00Z', effectiveValue: 9000 }),
      tx({ id: 2, effectiveDate: '2026-10-05T12:00:00Z', effectiveValue: -620 }),
      tx({ id: 3, effectiveDate: '2026-10-20T12:00:00Z', effectiveValue: -125.5 }),
      tx({ id: 4, effectiveDate: '2026-11-08T12:00:00Z', effectiveValue: 200 }),
    ], first, 3)
    expect(series).toEqual([
      { label: 'Sep', income: 9000, expense: 0 },
      { label: 'Oct', income: 0, expense: -745.5 },
      { label: 'Nov', income: 200, expense: 0 },
    ])
  })


  it('ignores rows outside the window', () => {
    const first = new Date('2026-11-01T00:00:00Z')
    const series = monthSeries([
      tx({ id: 1, effectiveDate: '2026-08-01T12:00:00Z', effectiveValue: 500 }),
      tx({ id: 2, effectiveDate: '2027-02-01T12:00:00Z', effectiveValue: -100 }),
    ], first, 3)
    expect(series.every((p) => p.income === 0 && p.expense === 0)).toBe(true)
  })
})

describe('chartBars', () => {
  it('scales both sides from one baseline', () => {
    const series = [
      { label: 'Sep', income: 9000, expense: 0 },
      { label: 'Oct', income: 0, expense: -3000 },
    ]
    const g = chartBars(series, 360, 160)
    expect(g.max).toBe(9000)
    expect(g.baseY).toBe(80)
    const income = g.bars.find((b) => b.side === 'income')
    const expense = g.bars.find((b) => b.side === 'expense')
    expect(income.y + income.height).toBeCloseTo(80)
    expect(expense.y).toBe(80)
    expect(expense.height).toBeCloseTo(income.height / 3, 5)
  })

  it('a month with no money keeps its slot', () => {
    const series = [
      { label: 'Sep', income: 0, expense: 0 },
      { label: 'Oct', income: 100, expense: 0 },
    ]
    const g = chartBars(series, 360, 160)
    // only October drew bars, but the slot width still divides by 2 months
    expect(g.bars).toHaveLength(1)
    expect(g.bars[0].x).toBeGreaterThanOrEqual(180)
  })

  it('no money anywhere draws an empty frame, never a broken path', () => {
    expect(chartBars([{ label: 'Sep', income: 0, expense: 0 }], 360, 160).bars).toHaveLength(0)
    expect(chartBars([], 360, 160).bars).toHaveLength(0)
  })

  it('labels the busiest month compactly', () => {
    const series = [
      { label: 'Sep', income: 9000, expense: 0 },
      { label: 'Oct', income: 1000, expense: -250 },
    ]
    const g = chartBars(series, 360, 160)
    expect(g.bars.find((b) => b.topLabel !== null)?.topLabel).toBe('9k')
  })

  it('the month label is the NET — expense is negative, so it is added', () => {
    // smoke-caught: income 9000, expenses −1793 must read 7.2k, not 10.8k
    const series = [{ label: 'Nov', income: 9000, expense: -1793 }]
    const g = chartBars(series, 320, 150)
    expect(g.bars.find((b) => b.topLabel !== null)?.topLabel).toBe('7.2k')
  })
})
