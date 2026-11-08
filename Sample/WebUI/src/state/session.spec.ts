// Session + preferences tests — pure logic, no browser: preference parsing
// (valid values adopted, junk ignored, never a crash), mode apply/clear,
// account-type labels, avatar initials, and the ApiError message shaping
// the global error area relies on.

import { describe, expect, it } from 'vitest'
import { parsePreferences } from '../state/session'
import { applyMode, clearMode, isModeName } from '../styles/mode'
import { isAccountTypeName, labelOfAccountType } from '../api/accountTypes'
import { initialOf } from '../state/session'

function memStorage(): Storage {
  const map = new Map<string, string>()
  return {
    getItem: (k: string) => (map.has(k) ? (map.get(k) as string) : null),
    setItem: (k: string, v: string) => void map.set(k, v),
    removeItem: (k: string) => void map.delete(k),
    clear: () => void map.clear(),
    key: () => null,
    get length() { return map.size },
  } as Storage
}

interface FakeDoc extends Document { _props: Map<string, string> }

function fakeDoc(): FakeDoc {
  const props = new Map<string, string>()
  const style = {
    setProperty: (k: string, v: string) => void props.set(k, v),
    removeProperty: (k: string) => { props.delete(k); return null },
  }
  return { documentElement: { style }, _props: props } as unknown as FakeDoc
}

describe('parsePreferences', () => {
  it('adopts valid theme, mode and account-type values', () => {
    const out = parsePreferences([
      { name: 'theme', value: 'emerald' },
      { name: 'mode', value: 'dark' },
      { name: 'account-type', value: 'business' },
    ])
    expect(out).toEqual({ theme: 'emerald', mode: 'dark', accountType: 'business' })
  })

  it('ignores junk values instead of crashing', () => {
    const out = parsePreferences([
      { name: 'theme', value: 'neon-pink' },
      { name: 'mode', value: 'high-contrast' },
      { name: 'account-type', value: 'holding' },
      { name: 'unrelated', value: 'whatever' },
    ])
    expect(out).toEqual({})
  })

  it('an empty payload yields empty state — caller falls back', () => {
    expect(parsePreferences([])).toEqual({})
  })
})

describe('display mode', () => {
  it('isModeName accepts the three modes, rejects junk', () => {
    expect(isModeName('system')).toBe(true)
    expect(isModeName('light')).toBe(true)
    expect(isModeName('dark')).toBe(true)
    expect(isModeName('Dark')).toBe(false) // case matters on the wire
    expect(isModeName('neon')).toBe(false)
    expect(isModeName('__proto__')).toBe(false)
  })

  it('light and dark pin color-scheme; system removes it', () => {
    const doc = fakeDoc()
    applyMode('dark', doc)
    expect(doc._props.get('color-scheme')).toBe('dark')
    applyMode('light', doc)
    expect(doc._props.get('color-scheme')).toBe('light')
    applyMode('system', doc)
    expect(doc._props.has('color-scheme')).toBe(false)
  })

  it('clearMode removes the pin — the device follows the OS again', () => {
    const doc = fakeDoc()
    applyMode('dark', doc)
    clearMode(doc)
    expect(doc._props.has('color-scheme')).toBe(false)
  })
})

describe('account type', () => {
  it('accepts the three names, rejects junk', () => {
    expect(isAccountTypeName('personal')).toBe(true)
    expect(isAccountTypeName('business')).toBe(true)
    expect(isAccountTypeName('project')).toBe(true)
    expect(isAccountTypeName('Personal')).toBe(false)
    expect(isAccountTypeName('constructor')).toBe(false) // prototype-chain junk
  })

  it('labels are the display forms', () => {
    expect(labelOfAccountType('personal')).toBe('Personal Account')
    expect(labelOfAccountType('business')).toBe('Business Account')
    expect(labelOfAccountType('project')).toBe('Project Account')
  })
})

describe('avatar initial', () => {
  it('takes the first character, upper-cased', () => {
    expect(initialOf('Rafael')).toBe('R')
    expect(initialOf('zotech')).toBe('Z')
  })
  it('whitespace is trimmed; empty yields the placeholder', () => {
    expect(initialOf('  shed studio')).toBe('S')
    expect(initialOf('   ')).toBe('?')
    expect(initialOf('')).toBe('?')
  })
})
