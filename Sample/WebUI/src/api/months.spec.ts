import { describe, expect, it } from 'vitest'
import { monthFromValue, monthLabel, monthValue, monthWindow, monthWindowOffset } from './months'

describe('monthWindow', () => {
  it('spans the whole month, UTC, half-open', () => {
    const w = monthWindow(new Date('2026-11-08T15:44:00Z'))
    expect(w.start).toBe('2026-11-01T00:00:00.000Z')
    expect(w.end).toBe('2026-12-01T00:00:00.000Z')
  })

  it('rolls the year on December', () => {
    const w = monthWindowOffset(new Date('2026-12-08T00:00:00Z'), 1)
    expect(w.start).toBe('2027-01-01T00:00:00.000Z')
  })

  it('prev and next are exact neighbors', () => {
    const now = new Date('2026-11-08T00:00:00Z')
    expect(monthWindowOffset(now, -1).start).toBe('2026-10-01T00:00:00.000Z')
    expect(monthWindowOffset(now, 1).start).toBe('2026-12-01T00:00:00.000Z')
  })

  it('the day inside the month does not shift the window', () => {
    const early = monthWindow(new Date('2026-11-01T00:00:00Z'))
    const late = monthWindow(new Date('2026-11-30T23:59:59Z'))
    expect(early).toEqual(late)
  })
})

describe('month label and input value round-trip', () => {
  it('labels "Nov 2026"', () => {
    expect(monthLabel('2026-11-01T00:00:00.000Z')).toBe('Nov 2026')
  })

  it('round-trips through the input value', () => {
    const start = '2026-11-01T00:00:00.000Z'
    const v = monthValue(start)
    expect(v).toBe('2026-11')
    expect(monthFromValue(v).toISOString()).toBe(start)
  })
})
