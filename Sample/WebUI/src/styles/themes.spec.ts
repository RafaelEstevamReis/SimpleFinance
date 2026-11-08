// Theme registry tests — the registry is data, so its contract is testable
// without a browser: names, tokens, apply/clear round-trip against a fake
// documentElement, and the preview-cache parse (corrupt → null, never crash).

import { describe, expect, it } from 'vitest'
import { getThemePreview, saveThemePreview } from '../api/storage'
import {
  applyTheme,
  accentOf,
  clearTheme,
  defaultTheme,
  isThemeName,
  themes,
  themeNames,
} from './themes'

function memStorage(): Storage {
  const map = new Map<string, string>()
  return {
    get length() { return map.size },
    clear: () => map.clear(),
    getItem: (k) => map.get(k) ?? null,
    key: (i) => [...map.keys()][i] ?? null,
    removeItem: (k) => map.delete(k),
    setItem: (k, v) => map.set(k, v),
  } as Storage
}

/** Fake Document exposing the property map its style writes to */
interface FakeDoc extends Document {
  _props: Map<string, string>
}

function fakeDoc(): FakeDoc {
  const props = new Map<string, string>()
  const style = {
    setProperty: (n: string, v: string) => props.set(n, v),
    removeProperty: (n: string) => { props.delete(n); return '' },
  }
  return {
    documentElement: { style },
    _props: props,
  } as FakeDoc
}

describe('theme registry', () => {
  it('ships the six spec names, slate as default', () => {
    expect(themeNames).toEqual(['slate', 'azure', 'emerald', 'violet', 'amber', 'rose'])
    expect(defaultTheme).toBe('slate')
  })

  it('every theme carries the identity tokens', () => {
    for (const name of themeNames) {
      const t = themes[name]
      expect(t.accent, `${name}.accent`).toMatch(/^#[0-9a-f]{6}$/i)
      expect(t.accentSoft, `${name}.accentSoft`).toMatch(/^#[0-9a-f]{6}$/i)
      if (t.accentDark) expect(t.accentDark).toMatch(/^#[0-9a-f]{6}$/i)
      if (t.accentSoftDark) expect(t.accentSoftDark).toMatch(/^#[0-9a-f]{6}$/i)
    }
  })

  it('accents are distinct — identity tints must not collide', () => {
    const accents = themeNames.map((n) => accentOf(n))
    expect(new Set(accents).size).toBe(accents.length)
  })

  it('isThemeName accepts registry names, rejects junk', () => {
    expect(isThemeName('slate')).toBe(true)
    expect(isThemeName('não-existe')).toBe(false)
    expect(isThemeName('')).toBe(false)
    // `in` would walk the prototype chain and accept this — hasOwn must not
    expect(isThemeName('__proto__')).toBe(false)
    expect(isThemeName('toString')).toBe(false)
  })
})

describe('applyTheme / clearTheme', () => {
  it('apply sets both identity vars with the light-dark pair', () => {
    const doc = fakeDoc()
    applyTheme('azure', doc)
    expect(doc._props.get('--accent'))
      .toBe(`light-dark(${themes.azure.accent}, ${themes.azure.accentDark})`)
    expect(doc._props.get('--accent-soft'))
      .toBe(`light-dark(${themes.azure.accentSoft}, ${themes.azure.accentSoftDark})`)
  })

  it('every registry theme produces the full light-dark pair', () => {
    const doc = fakeDoc()
    for (const name of themeNames) {
      const t = themes[name]
      applyTheme(name, doc)
      // registry themes all carry accentDark; the `?? t.accent` fallback
      // exists for the Theme type, not for registry data
      expect(doc._props.get('--accent'))
        .toBe(`light-dark(${t.accent}, ${t.accentDark ?? t.accent})`)
    }
  })

  it('clear removes both identity vars — neutral again', () => {
    const doc = fakeDoc()
    applyTheme('rose', doc)
    clearTheme(doc)
    expect(doc._props.has('--accent')).toBe(false)
    expect(doc._props.has('--accent-soft')).toBe(false)
  })
})

describe('theme preview cache', () => {
  it('round-trips per account label', () => {
    const s = memStorage()
    saveThemePreview(s, 'main', 'emerald')
    saveThemePreview(s, 'other', 'rose')
    expect(getThemePreview(s, 'main')).toBe('emerald')
    expect(getThemePreview(s, 'other')).toBe('rose')
  })

  it('corrupt JSON downgrades to null, never crashes', () => {
    const s = memStorage()
    s.setItem('webui.theme.main', '{not json')
    expect(getThemePreview(s, 'main')).toBeNull()
  })

  it('non-string stored value downgrades to null', () => {
    const s = memStorage()
    s.setItem('webui.theme.main', JSON.stringify({ theme: 'slate' }))
    expect(getThemePreview(s, 'main')).toBeNull()
  })

  it('empty label never collides with a real account key', () => {
    const s = memStorage()
    saveThemePreview(s, '', 'slate')
    expect(getThemePreview(s, 'real')).toBeNull()
  })
})
