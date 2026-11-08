// Month arithmetic for the wire: the API speaks explicit UTC, so every
// window the client sends is built in UTC. The book (M6) is the first
// consumer that browses months instead of only reading "this one".

/// Window of the month containing `now`, as UTC ISO strings — [start, end)
export function monthWindow(now: Date = new Date()): { start: string; end: string } {
  const start = new Date(Date.UTC(now.getUTCFullYear(), now.getUTCMonth(), 1))
  const end = new Date(Date.UTC(now.getUTCFullYear(), now.getUTCMonth() + 1, 1))
  return { start: start.toISOString(), end: end.toISOString() }
}

/// Window of the month `offset` months away from `now` — the book's prev/next
export function monthWindowOffset(now: Date, offset: number): { start: string; end: string } {
  const shifted = new Date(Date.UTC(now.getUTCFullYear(), now.getUTCMonth() + offset, 1))
  return monthWindow(shifted)
}

/// "Nov 2026" — short month + year, for the browser header
export function monthLabel(start: string): string {
  const d = new Date(start)
  const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
  return `${months[d.getUTCMonth()]} ${d.getUTCFullYear()}`
}

/// "2026-11" — the value `<input type="month">` carries
export function monthValue(start: string): string {
  return start.slice(0, 7)
}

/// Parse a "2026-11" back into the first day of that month, UTC
export function monthFromValue(value: string): Date {
  const [y, m] = value.split('-').map(Number)
  return new Date(Date.UTC(y, m - 1, 1))
}
