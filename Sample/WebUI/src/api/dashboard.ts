// Dashboard logic: the Home screen's glanceable answers, all pure. The
// screen wires fetches, the tests pin the doctrine:
//   - a transfer is one line (pair first — the M6 rule)
//   - upcoming lists the OVERDUE first, in red, then the future by date
//   - the chart never sums across currencies — it is per currency
//   - a chart with no money draws an empty frame, never a broken path

import { pairTransfers } from './book'
import type { TransactionResponse } from './types'

export interface DashboardRow {
  /// "Nov 08" or "Nov 08 · overdue" — the label under the title
  date: string
  /// Transfer rows carry both leg ids; a plain tx only its own
  ids: number[]
  kind: 'tx' | 'transfer'
  tx: TransactionResponse
  /// The far leg, only on a transfer row
  other?: TransactionResponse
}

export interface DueRow extends DashboardRow {
  /// Days past its due date, only when it is late
  overdueBy: number
}

/// Recent activity as one line per decision: transfers are paired FIRST
/// (a pair is one movement, not two mystery rows), then the newest N are
/// kept, newest first. `date` is the effective date the row happened on
export function recentRows(txs: TransactionResponse[], n: number, shortDate: (iso: string) => string): DashboardRow[] {
  const rows = pairTransfers(txs)
  const sorted = [...rows].sort((a, b) => {
    const dt = Date.parse(b.tx.effectiveDate) - Date.parse(a.tx.effectiveDate)
    // same instant ties break by id, so a wire-served Asc order does not
    // leak into "newest first"
    return dt !== 0 ? dt : b.tx.id - a.tx.id
  })
  return sorted.slice(0, n).map((r) => ({
    date: shortDate(r.tx.effectiveDate ?? r.tx.dueDate),
    ids: r.ids,
    kind: r.kind,
    tx: r.tx,
    other: r.other,
  }))
}

/// What is still owed: unpaid rows due from `today` forward, plus the
/// ones already past — those come FIRST and carry `overdueBy`. Sorted by
/// due date, newest first inside a day, kept to N
export function upcomingDue(txs: TransactionResponse[], today: Date, n: number, shortDate: (iso: string) => string): DueRow[] {
  const t0 = Date.UTC(today.getUTCFullYear(), today.getUTCMonth(), today.getUTCDate())
  const due = txs.filter((t) => t.status === 'Unpaid')
  const rows = due.map((t) => {
    const when = Date.parse(t.dueDate)
    return { tx: t, when, overdueBy: Math.max(0, Math.floor((t0 - when) / 86_400_000)) }
  })
  rows.sort((a, b) => {
    // overdue blocks ahead of everything, oldest debt first
    const ao = a.overdueBy > 0 ? 0 : 1
    const bo = b.overdueBy > 0 ? 0 : 1
    if (ao !== bo) return ao - bo
    if (ao === 0) return b.overdueBy - a.overdueBy
    return a.when - b.when
  })
  return rows.slice(0, n).map((r) => ({
    date: r.overdueBy > 0 ? `${shortDate(r.tx.dueDate)} · overdue` : shortDate(r.tx.dueDate),
    ids: [r.tx.id],
    kind: r.tx.type === 'WalletTransfer' ? 'transfer' : 'tx',
    tx: r.tx,
    overdueBy: r.overdueBy,
  }))
}

export interface MonthPoint {
  /// "Aug" — short month label for the axis
  label: string
  /// Sum of positive effective values, expenses below zero
  income: number
  expense: number
}
export function monthSeries(txs: TransactionResponse[], first: Date, months: number): MonthPoint[] {
  const start = Date.UTC(first.getUTCFullYear(), first.getUTCMonth(), 1)
  const labels = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
  const points: MonthPoint[] = []
  for (let i = 0; i < months; i++) {
    const month = new Date(start)
    month.setUTCMonth(month.getUTCMonth() + i)
    points.push({ label: labels[month.getUTCMonth()], income: 0, expense: 0 })
  }
  for (const tx of txs) {
    const when = new Date(tx.effectiveDate)
    const i = (when.getUTCFullYear() - first.getUTCFullYear()) * 12 + when.getUTCMonth() - first.getUTCMonth()
    if (i < 0 || i >= months) continue
    // effectiveValue — the same figure the book's running net speaks
    if (tx.effectiveValue > 0) points[i].income += tx.effectiveValue
    else if (tx.effectiveValue < 0) points[i].expense += tx.effectiveValue
  }
  return points
}

export interface ChartBar {
  /// SVG rect for a bar: x, y, width, height — y/height clamped so a
  /// zero-height bar stays a zero-height bar, never a negative one
  x: number
  y: number
  width: number
  height: number
  /// "9,000" — compact value above the tallest bar of the pair
  topLabel: string | null
  /// Which side of the pair this bar is
  side: 'income' | 'expense'
}

export interface ChartGeometry {
  bars: ChartBar[]
  /// Baseline y (income sits above it, expense below)
  baseY: number
  /// 0 when the series has no money at all — the frame draws empty
  max: number
}

/// Bar geometry for the monthly chart. `width` is the svg inner width,
/// `height` the plot height; the caller owns the frame, this owns the
/// math. A month with no activity keeps its slot (empty, labeled) — a
/// quiet month is a fact, not a gap
export function chartBars(series: MonthPoint[], width: number, height: number): ChartGeometry {
  const baseY = height / 2
  const max = series.reduce((m, p) => Math.max(m, p.income, -p.expense), 0)
  if (series.length === 0 || max <= 0) return { bars: [], baseY, max: 0 }
  const slot = width / series.length
  const gap = Math.min(8, slot * 0.25)
  const barW = Math.max(2, (slot - gap * 2) / 2)
  const scale = (height / 2 - 2) / max
  const compact = (v: number): string => {
    const abs = Math.abs(v)
    if (abs >= 1000) return `${+(abs / 1000).toFixed(1)}k`
    return `${+abs.toFixed(1)}`
  }
  const bars: ChartBar[] = []
  series.forEach((p, i) => {
    const left = i * slot + gap
    const inH = p.income * scale
    const exH = -p.expense * scale
    if (p.income > 0) bars.push({ x: left, y: baseY - inH, width: barW, height: inH, topLabel: null, side: 'income' })
    if (p.expense < 0) bars.push({ x: left + barW, y: baseY, width: barW, height: exH, topLabel: null, side: 'expense' })
  })
  // value above the busiest month — the glanceable one
  let busiest = -1
  let busiestTotal = 0
  series.forEach((p, i) => {
    const total = p.income + p.expense // expense is negative — net is a sum
    if (total > busiestTotal) { busiestTotal = total; busiest = i }
  })
  if (busiest >= 0) {
    const b = bars.find((x) => x.side === 'income' && x.x >= busiest * slot && x.x < (busiest + 1) * slot)
    if (b) b.topLabel = compact(busiestTotal)
  }
  return { bars, baseY, max }
}
