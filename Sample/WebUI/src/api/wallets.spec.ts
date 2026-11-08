// wallets.ts — pure money formatting and the cross-currency rule.

import { describe, expect, it } from 'vitest'
import { formatMoney, liveBalances, netWorthLabel, normalizeCurrency, sumByCurrency } from './wallets'
import type { WalletBalanceResponse, WalletResponse } from './types'

function bal(walletId: number, baseCurrency: string, balance: number): WalletBalanceResponse {
  return { walletId, name: `w${walletId}`, baseCurrency, balance }
}

function w(id: number, isDeleted = false): WalletResponse {
  return { id, externallType: 0, name: `w${id}`, description: '', baseCurrency: '', isDeleted }
}

describe('formatMoney', () => {
  it('groups thousands with two decimals', () => {
    expect(formatMoney(4231.5)).toBe('4,231.50')
    expect(formatMoney(12000)).toBe('12,000.00')
  })

  it('uses the typographic minus for negatives', () => {
    expect(formatMoney(-890.2)).toBe('−890.20')
  })

  it('carries rounding through, never a half-truncated cent', () => {
    expect(formatMoney(0.005)).toBe('0.01')
    expect(formatMoney(0.004)).toBe('0.00')
  })

  it('does not group a three-digit whole', () => {
    expect(formatMoney(123.45)).toBe('123.45')
  })
})

describe('normalizeCurrency', () => {
  it('trims, uppercases and collapses inner spaces', () => {
    expect(normalizeCurrency('  brl ')).toBe('BRL')
    expect(normalizeCurrency(' sat ')).toBe('SAT')
  })

  it('keeps empty empty — the form decides, not the normalizer', () => {
    expect(normalizeCurrency('   ')).toBe('')
  })
})

describe('liveBalances', () => {
  it('drops soft-deleted wallets and unknown ids', () => {
    const out = liveBalances([w(1), w(2, true)], [bal(1, 'BRL', 10), bal(2, 'BRL', 99), bal(7, 'USD', 5)])
    expect(out).toEqual([bal(1, 'BRL', 10)])
  })

  it('empty wallets means no balances survive', () => {
    expect(liveBalances([], [bal(1, 'BRL', 10)])).toEqual([])
  })
})

describe('sumByCurrency', () => {
  it('sums same-currency balances into one entry', () => {
    const out = sumByCurrency([bal(1, 'BRL', 100), bal(2, 'BRL', -40.5), bal(3, 'USD', 25)])
    expect(out.get('BRL')).toBe(59.5)
    expect(out.get('USD')).toBe(25)
    expect(out.size).toBe(2)
  })

  it('empty balances yield an empty map', () => {
    expect(sumByCurrency([]).size).toBe(0)
  })
})

describe('netWorthLabel', () => {
  it('one currency: the total, money-formatted', () => {
    expect(netWorthLabel([bal(1, 'BRL', 4231.5), bal(2, 'BRL', -890.2)])).toBe('3,341.30')
  })

  it('two currencies: no number is invented', () => {
    expect(netWorthLabel([bal(1, 'BRL', 100), bal(2, 'USD', 25)])).toBe('—')
  })

  it('no balances: placeholder, not zero', () => {
    expect(netWorthLabel([])).toBe('—')
  })
})
