// categories.ts — budget track math and month spend grouping.

import { describe, expect, it } from 'vitest'
import { budgetNote, budgetTrack, monthlyBudgetRow, sumSpentByCategory } from './categories'
import type { TransactionResponse } from './types'

function tx(categoryId: number, effectiveValue: number, typeOtherId = 0): TransactionResponse {
  return {
    id: 0, walletId: 1, categoryId, counterpartyId: 0, invoiceId: 0,
    type: 'Simple', typeOtherId, status: 'Paid',
    dueDate: '', paymentDate: '', effectiveDate: '',
    description: '', paymentCurrency: '', dueValue: 0, paidValue: 0,
    effectiveValue, paymentDetails: null, externalIdentifier: null,
    created: '', changed: '',
  }
}

describe('sumSpentByCategory', () => {
  it('sums the absolute value of negative rows per category', () => {
    const out = sumSpentByCategory([tx(1, -620), tx(1, -182.4), tx(2, -95)])
    expect(out.get(1)).toBe(802.4)
    expect(out.get(2)).toBe(95)
    expect(out.size).toBe(2)
  })

  it('ignores positive rows — a reversal under an expense category is not a purchase', () => {
    expect(sumSpentByCategory([tx(1, 100)]).size).toBe(0)
  })

  it('ignores zero rows', () => {
    expect(sumSpentByCategory([tx(1, 0)]).size).toBe(0)
  })
})

describe('budgetTrack', () => {
  it('scales the fill to the spent/budget ratio', () => {
    expect(budgetTrack(620, 1000)).toEqual({ pct: 62, over: false })
    expect(budgetTrack(190, 500)).toEqual({ pct: 38, over: false })
  })

  it('full bar + over flag when the limit is passed', () => {
    expect(budgetTrack(547.8, 400)).toEqual({ pct: 100, over: true })
  })

  it('exactly on budget is not over', () => {
    expect(budgetTrack(400, 400)).toEqual({ pct: 100, over: false })
  })

  it('budget 0 means no limit — no track exists', () => {
    expect(budgetTrack(300, 0)).toEqual({ pct: 100, over: false })
    expect(budgetTrack(0, 0)).toEqual({ pct: 0, over: false })
  })
})

describe('budgetNote', () => {
  it('formats the "of N" note under the value', () => {
    expect(budgetNote(1000)).toBe('of 1,000')
  })

  it('no budget, no note', () => {
    expect(budgetNote(0)).toBe('')
  })
})

describe('monthlyBudgetRow', () => {
  const cats = [
    { id: 1, isExpense: true, monthlyBudget: 1000 },
    { id: 2, isExpense: true, monthlyBudget: 900 },
    { id: 3, isExpense: false, monthlyBudget: 500 },
  ]

  it('sums spent and limit over every expense category with a budget', () => {
    expect(monthlyBudgetRow(new Map([[1, -123.4]]), cats)).toBe('123.40 of 1,900')
  })

  it('an unbudgeted expense category stays out — spend without a limit inflates nothing', () => {
    const mixed = [...cats, { id: 4, isExpense: true, monthlyBudget: 0 }]
    expect(monthlyBudgetRow(new Map([[4, 50]]), mixed)).toBe('0.00 of 1,900')
  })

  it('no budgets at all means no row', () => {
    expect(monthlyBudgetRow(new Map([[9, 10]]), [{ id: 9, isExpense: true, monthlyBudget: 0 }])).toBe('')
  })
})
