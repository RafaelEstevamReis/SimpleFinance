// Theme registry — a theme is data, applying it is setting CSS vars.
// Spec: Architecture.md (Theme section). Subtle by design: a theme touches
// accent + accent-soft only; fg/surface/semantic colors never change.

export interface Theme {
  /** Accent color — light-mode value; dark mode derives automatically */
  accent: string
  /** Soft background derivative — the tint for cards, focus rings, bars */
  accentSoft: string
  /** Dark-mode accent — slightly lifted so it reads on the dark surface */
  accentDark?: string
  /** Dark-mode soft derivative */
  accentSoftDark?: string
}

// `satisfies` forces every theme to carry the required tokens; a typo in a
// theme name is a compile error, a missing token is a compile error.
export const themes = {
  slate: {
    accent: '#3b4b63',
    accentSoft: '#e3e8f0',
    accentDark: '#8fa3bf',
    accentSoftDark: '#232a35',
  },
  azure: {
    accent: '#1f6fd0',
    accentSoft: '#d9e8fa',
    accentDark: '#6aa5e8',
    accentSoftDark: '#16233a',
  },
  emerald: {
    accent: '#1e7f4f',
    accentSoft: '#d9f0e3',
    accentDark: '#4cbb87',
    accentSoftDark: '#12271d',
  },
  violet: {
    accent: '#6b4bb5',
    accentSoft: '#e7defa',
    accentDark: '#a58cd9',
    accentSoftDark: '#231a35',
  },
  amber: {
    accent: '#a06a00',
    accentSoft: '#f8ecd4',
    accentDark: '#d9a441',
    accentSoftDark: '#2e2310',
  },
  rose: {
    accent: '#b0435c',
    accentSoft: '#fadde3',
    accentDark: '#d97c8f',
    accentSoftDark: '#331820',
  },
} satisfies Record<string, Theme>

export type ThemeName = keyof typeof themes

export const themeNames = Object.keys(themes) as ThemeName[]
export const defaultTheme: ThemeName = 'slate'

/** True when name is in the registry — used before trusting stored data */
export function isThemeName(name: string): name is ThemeName {
  // Object.hasOwn, not `in` — `in` walks the prototype chain and would
  // bless '__proto__' & friends with a theme that does not exist.
  return Object.hasOwn(themes, name)
}

/** Apply a theme: set the identity tokens on <html>. Everything else stays. */
export function applyTheme(name: ThemeName, doc: Document = document): void {
  const t = themes[name]
  const style = doc.documentElement.style
  style.setProperty('--accent', `light-dark(${t.accent}, ${t.accentDark ?? t.accent})`)
  style.setProperty('--accent-soft', `light-dark(${t.accentSoft}, ${t.accentSoftDark ?? t.accentSoft})`)
}

/** Back to neutral — on disconnect the shell must not keep another account's tint */
export function clearTheme(doc: Document = document): void {
  const style = doc.documentElement.style
  style.removeProperty('--accent')
  style.removeProperty('--accent-soft')
}

/** Accent value for a preview swatch (Connect cards, picker) — light-mode value */
export function accentOf(name: ThemeName): string {
  return themes[name].accent
}
