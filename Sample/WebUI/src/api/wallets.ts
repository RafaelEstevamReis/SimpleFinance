// Wallet pure logic — formatting and the cross-currency rule, no DOM, no
// client. The rule is a doctrine, not a preference: a balance is always in
// its wallet's own currency and two currencies are never summed.

import type { WalletBalanceResponse, WalletResponse } from './types'

/// Thousands-grouped, two decimals — the demo5 number style. The currency
/// code lives in its own column, so this is the bare amount.
export function formatMoney(value: number): string {
  const sign = value < 0 ? '−' : ''
  const abs = Math.abs(value)
  const whole = Math.trunc(abs).toString()
  const grouped = whole.replace(/\B(?=(\d{3})+(?!\d))/g, ',')
  const cents = Math.round((abs - Math.trunc(abs)) * 100)
    .toString()
    .padStart(2, '0')
  return `${sign}${grouped}.${cents}`
}

/// Client-side normalization for a currency code: the server stores what it
/// is sent (Manager only enforces Name), so the border is here — trim,
/// uppercase, collapse inner spaces. Empty stays empty and the form blocks.
export function normalizeCurrency(code: string): string {
  return code.trim().toUpperCase().replace(/\s+/g, ' ')
}

/// Balances of wallets that still count. Presentation decides what a soft
/// delete means, and here a hidden wallet is out of every sum — the net
/// worth block is a figure of the living ledger. Balances whose wallet is
/// unknown are dropped too: both rows come from the same database, an
/// orphan would be a lie.
export function liveBalances(
  wallets: WalletResponse[],
  balances: WalletBalanceResponse[],
): WalletBalanceResponse[] {
  const live = new Set(wallets.filter(w => !w.isDeleted).map(w => w.id))
  return balances.filter(b => live.has(b.walletId))
}

/// Total per currency, from balances already in their own currency.
/// The map is the honest answer; whether a single total may be shown is a
/// presentation call — see netWorthLabel.
export function sumByCurrency(balances: WalletBalanceResponse[]): Map<string, number> {
  const totals = new Map<string, number>()
  for (const b of balances) {
    totals.set(b.baseCurrency, (totals.get(b.baseCurrency) ?? 0) + b.balance)
  }
  return totals
}

/// The Home "net worth" answer. One currency: its total, money-formatted.
/// More than one: no number exists and none is invented.
export function netWorthLabel(balances: WalletBalanceResponse[]): string {
  const totals = sumByCurrency(balances)
  if (totals.size === 1) {
    return formatMoney(totals.values().next().value as number)
  }
  return '—'
}
