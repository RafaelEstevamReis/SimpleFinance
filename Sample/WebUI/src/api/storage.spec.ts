import { describe, expect, it, beforeEach } from 'vitest'
import {
  getActive,
  getAccounts,
  getServerUrl,
  saveAccounts,
  saveActive,
  saveServerUrl,
} from './storage'

function memoryStorage(): Storage {
  const map = new Map<string, string>()
  return {
    getItem: (k: string) => map.get(k) ?? null,
    setItem: (k: string, v: string) => void map.set(k, v),
    removeItem: (k: string) => void map.delete(k),
    clear: () => map.clear(),
    key: () => null,
    get length() { return map.size },
  } as Storage
}

describe('storage corrupt-JSON safety', () => {
  let s: Storage
  beforeEach(() => { s = memoryStorage() })

  it('corrupt accounts JSON downgrades to empty array', () => {
    s.setItem('webui.accounts', '{not json')
    expect(getAccounts(s)).toEqual([])
  })

  it('corrupt active JSON downgrades to null', () => {
    s.setItem('webui.active', 'garbage')
    expect(getActive(s)).toBeNull()
  })

  it('active with wrong shape downgrades to null', () => {
    s.setItem('webui.active', JSON.stringify({ serverLabel: 'x', nope: 1 }))
    expect(getActive(s)).toBeNull()
  })

  it('active with only a key survives the single-server migration', () => {
    s.setItem('webui.active', JSON.stringify({ key: 'abc' }))
    expect(getActive(s)).toEqual({ key: 'abc' })
  })

  it('round-trip server, accounts, active', () => {
    saveServerUrl(s, 'http://localhost:5080')
    saveAccounts(s, [{ name: 'main', key: 'abc', primaryCurrency: 'BRL' }])
    saveActive(s, { key: 'abc' })

    expect(getServerUrl(s)).toBe('http://localhost:5080')
    expect(getAccounts(s)[0]).toEqual({ name: 'main', key: 'abc', primaryCurrency: 'BRL' })
    expect(getActive(s)).toEqual({ key: 'abc' })
  })

  it('legacy `label` seeds the name cache — the Key is the identity', () => {
    s.setItem('webui.accounts', JSON.stringify([{ label: 'main', key: 'abc', primaryCurrency: 'BRL' }]))
    expect(getAccounts(s)).toEqual([{ name: 'main', key: 'abc', primaryCurrency: 'BRL' }])
  })

  it('empty-string server (legacy corrupt) reads as null', () => {
    s.setItem('webui.server', '')
    expect(getServerUrl(s)).toBeNull()
  })

  it('saveActive(null) removes the key entirely', () => {
    saveActive(s, { key: 'k' })
    saveActive(s, null)
    expect(s.getItem('webui.active')).toBeNull()
  })

  it('accounts stored as object (not array) downgrades to empty', () => {
    s.setItem('webui.accounts', JSON.stringify({ label: 'x' }))
    expect(getAccounts(s)).toEqual([])
  })
})
