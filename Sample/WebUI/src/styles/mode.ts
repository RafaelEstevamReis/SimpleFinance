// Display mode — light/dark/system. The tokens are light-dark() pairs, so the
// only switch is the color-scheme property on <html>; a theme (accent) and a
// mode (light/dark) are independent axes by design.
// Spec: Architecture.md (Theming). Mirrors themes.ts: registry as data,
// Object.hasOwn-style guards, apply/clear pair.

export const modeNames = ['system', 'light', 'dark'] as const
export type ModeName = (typeof modeNames)[number]

/** True when value is a real mode name — used before trusting stored data */
export function isModeName(value: string): value is ModeName {
  return (modeNames as readonly string[]).includes(value)
}

/** Apply a mode: 'system' follows the OS (color-scheme removed), light/dark pin it */
export function applyMode(name: ModeName, doc: Document = document): void {
  const style = doc.documentElement.style
  if (name === 'system') {
    style.removeProperty('color-scheme')
  } else {
    style.setProperty('color-scheme', name)
  }
}

/** Back to the tokens' default (light dark) — on disconnect the device follows the OS again */
export function clearMode(doc: Document = document): void {
  doc.documentElement.style.removeProperty('color-scheme')
}
