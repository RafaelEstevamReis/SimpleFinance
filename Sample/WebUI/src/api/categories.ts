// Category pure logic — the budget track math and the month spend grouping.
// The server never compares MonthlyBudget against the transactions
// (CategoryDtos: "the client decides what to do with it"), so this is where
// the doctrine lives: a budget is a spending limit, never money — it stays
// positive while the expenses it measures are negative.

import type { TransactionResponse } from './types'

/// Spent this month by categoryId. Only negative effective values count:
/// an expense category forces its transactions negative, so a positive
/// value under it is a reversal, not a purchase. Transfers (typeOtherId
/// !== 0, category-free by design) never appear here — they carry no
/// category.
export function sumSpentByCategory(transactions: TransactionResponse[]): Map<number, number> {
  const spent = new Map<number, number>()
  for (const tx of transactions) {
    if (tx.effectiveValue >= 0) continue
    spent.set(tx.categoryId, (spent.get(tx.categoryId) ?? 0) + Math.abs(tx.effectiveValue))
  }
  return spent
}

export interface BudgetTrack {
  /// fill width in percent, 0..100 — an over-budget track is full, the
  /// color change (not the width) says how far past it went
  pct: number
  /// true when the month's spend passed the limit
  over: boolean
}

/// The track geometry for one expense category. budget 0 means no limit:
/// no track exists, the caller shows the bare spend. The demo5 shape.
export function budgetTrack(spent: number, budget: number): BudgetTrack {
  if (budget <= 0) return { pct: spent > 0 ? 100 : 0, over: false }
  const ratio = budget > 0 ? (spent / budget) * 100 : 0
  return {
    pct: Math.min(100, Math.round(ratio)),
    over: spent > budget,
  }
}

/// The "of N" note under the value — empty string when there is no limit
export function budgetNote(budget: number): string {
  return budget > 0 ? `of ${budget.toLocaleString('en-US')}` : ''
}

/// The Net-worth card's budget row (tablet3 shape): "spent of limit" for
/// the current month. Empty string when no expense category carries a
/// limit — the row vanishes instead of showing a zero against nothing.
export function monthlyBudgetRow(spentByCategory: Map<number, number>, categories: { id: number; isExpense: boolean; monthlyBudget: number }[]): string {
  let spent = 0
  let limit = 0
  for (const c of categories) {
    if (!c.isExpense || c.monthlyBudget <= 0) continue
    limit += c.monthlyBudget
    spent += Math.abs(spentByCategory.get(c.id) ?? 0)
  }
  if (limit <= 0) return ''
  return `${spent.toLocaleString('en-US', { minimumFractionDigits: 2 })} of ${limit.toLocaleString('en-US')}`
}
