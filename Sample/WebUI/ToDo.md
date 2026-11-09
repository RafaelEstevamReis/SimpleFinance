# ToDo.md — Sample/WebUI implementation plan

Deliverable-based plan, from HelloWorld to everything.
Each milestone is sized to fit one working session (build, smoke,
verify, commit — then stop).

The whole plan is intentionally **not** one big build: companion UIs built in a
single push by other agents cost roughly a month of tokens. This file keeps
every milestone small, verifiable and independently shippable.

**The phone is the primary target.** The UI must be good on a phone first,
then tablets — the PC is a special case of tablet, designed last, not the
default that phones must tolerate. Every milestone's smoke includes using the
screen at phone width.

Rules for every milestone, no exceptions:

- Packages: only from the `Packages.md` curated list. Anything else goes
  through a Proposal first (see that file).
- Smoke against a real API (`dotnet run --project src/WebApi`) before commit.
- `npm run build` passes.
- Smoke includes phone width (dev tools device mode is enough).

### How logic is verified (token discipline)

**A thrown-away verification is burned tokens**: written, run once, proves
nothing ever again. So:

- **Permanent test by default.** Pure logic with a stable surface (cents
  parsing and bank-typing, month browsing, error shaping in `client.ts`,
  theme round-trip) gets a `vitest` test the moment it is written — the test
  is part of the deliverable, not a retrofit.
- **Throwaway is the exception**, named as such in the commit — allowed only
  when there is no stable surface to pin the test to.
- **Screen smoke stays the done-criterion for the visual** — that work is
  never thrown away, it *is* the commit.
- What does not deserve a test: visual layout, browser behavior a unit test
  cannot see, and anything that would just re-pin what the type system
  already proves.

---

## M0 — HelloWorld

The only milestone that starts with zero code — the kernel is already
installed, curated and locked.

- [x] Scaffold: `package.json` scripts, `vite.config.ts`, `tsconfig.json` +
      `tsconfig.node.json` (project references), `index.html`, `src/main.ts`,
      `src/App.vue` — "My Personal Finances" + version from package.json.
- [x] `npm run build` passes (vue-tsc type-check + vite build); dev server
      shows the hello screen (desktop and 390px phone, no overflow).
- [x] `.gitignore` covers `node_modules/`/`dist/`/`tsbuild/`/`*.tsbuildinfo`.

## M1 — API client + Connect screen

The WebUi becomes a real client.

- [x] `src/api/client.ts` — active server `baseUrl`, `X-Api-Key` header,
      400 `ProblemDetails` surfaced as readable errors, 401 → back to Connect.
- [x] `src/api/types.ts` — DTO mirrors copied from `DTOs/AccountDtos.cs` +
      `HelloController.cs` (hello/create/account/database).
- [x] Connect screen per `Architecture.md`: add server (validated with
      `GET /api/hello`, failure reasons surfaced), add account (paste Key or
      `POST /api/account`), stored in `localStorage` as specced
      (`webui.server`/`webui.accounts`/`webui.active`).
- [x] Switching accounts works; nothing is hardcoded. Smoke ran end-to-end
      against a real API (server → create → connect), 390px phone width
      included; 15 vitest cases cover error shaping and corrupt-JSON storage
      safety.

## M2 — Theme system

Identity comes before content: tint the shell so M3+ screens inherit it.

- [x] `src/styles/themes.ts` — registry `satisfies Record<string, Theme>`,
      ship set: `slate` (default), `azure`, `emerald`, `violet`, `amber`, `rose`.
- [x] `applyTheme(name)` sets CSS custom properties on `<html>` (clearTheme
      for neutral — disconnect drops another account's tint).
- [x] Read the account's theme from `GET /api/account/preferences` on connect;
      write with `PUT` from a picker; cache per-account for login preview.
- [x] Connect screen cards tinted by each account's accent (preview cache).
- [x] *Subtle by design* respected (see `Architecture.md` — accent only,
      never text/background/semantic colors). Smoke: pick emerald → server
      preference `theme=emerald` verified over the wire; reconnect applies it
      from the server; connect card tinted from preview cache; 390px clean.

## M3 — App shell

The frame every later screen hangs on.

- [x] Top bar: account label + accent tint, profile avatar (initial), profile
      sheet — switch between saved accounts, "Add or create" → connect.
      Smoke: sheet lists both saved accounts, "here" on the active one;
      Add or create lands on connect; a dead Key in the sheet surfaces
      the server message in the error area and leaves the session intact.
- [x] Bottom nav: Home / Wallets / Book / Settings, active state, spread to
      content width (demo5 pattern). Verified aligned at 390px.
- [x] Global error area: API errors with the server's message, not generic.
- [x] Settings screen leaves mock behind: theme swatches, Dark/Light toggle
      (`PUT mode`), account type (`PUT account-type`, display only).
      Smoke: dark + business + violet written from Settings, read back on
      the wire (`mode=dark`, `account-type=business`, `theme=violet`);
      reload restores all three from the server, header shows
      "Business Account".
- [x] Home becomes the demo5 dashboard (static content for now — M8 wires it).
      Smoke: sections Wallets / Categories / Activity with designed empty
      states; net worth placeholder; 390px no overflow (after fixing
      `.content` box-sizing).

## M4 — Wallets

First money screen, inside the shell.

- [x] Wallets screen: demo5 table (Wallet / Currency / Balance), create/edit
      form, balance per wallet, no cross-currency sums. The list loads in
      two calls (`GET /api/wallets` + `GET /api/wallets/balances`, the bulk
      endpoint — one balance per wallet without N+1 requests; the client
      also carries `walletBalance(id)` for the single-wallet question).
      Smoke: create "Main" (currency `brl` normalized to `BRL` on the
      border), row-click edit prefills and PUT renames it (verified on the
      wire), empty-name blocked client-side with the field's message.
- [x] Empty state designed; API errors with the server message. Smoke: the
      soft-deleted wallet stays in the list dimmed (`del` class — soft
      delete is presentation's call) and is out of every sum: Home showed
      "Balances in 2 currencies" with a dead USD wallet, `liveBalances`
      fixed it to the single-currency total. Net worth invents no number
      across currencies — placeholder plus the reason.

## M5 — Categories

- [x] Categories screen: demo5 budget tracks (name, progress bar, spent
      "of budget" below the value), create/edit form, `monthlyBudget` field.
      The month spend is one search over the effective timeline grouped
      client-side (`sumSpentByCategory`), not one request per category.
      Smoke: Groceries 62% of 1,000, Transport over-budget (547.80 of 500 —
      full bar, `over` fill, red value), income category shows the earned
      month value with no track, budget 0 shows no "of" note and no track.
- [x] `isExpense` locked after creation — UI shows it read-only on edit.
      Smoke: edit opens with "Expense — locked after creation" instead of
      the pills; creation offers the Expense/Income choice.
- [x] Empty state designed; API errors with the server message. Smoke:
      negative budget blocked client-side with the field's message; the
      form surfaces the server's message on wire errors (same
      `errorMessage` path proven in M3/M4). Home's categories section
      shows the same tracks and its "manage →" now points here (it
      pointed at settings since M3).

## M6 — Transaction book — the transactions screen

The book is not a later view: **it is how the WebUi displays transactions** —
one wallet, one closed month. The simplest useful shape a transaction list
can have; how the screen *designs around* that shape is open.

- [x] Wallet selector + **month browser** (prev/next/jump) over the search
      endpoints — month window, ordered by date, `Id` tiebreak. One
      unfiltered search per month (`/by`, `dateType=EffectiveDate`): pairing
      must see both legs of every transfer, each view picks its own rows
      (`walletBook`). Smoke: prev → "Oct 2026" empty state, back to Nov,
      jump via `<input type="month">` → "Dec 2025" empty, back.
- [x] Rows show: date, description, counterparty, value with sign as stored;
      transfers display as transfers. Smoke: Main sees `⇄ Transfer →
      Dollars −300.00` while Dollars sees `⇄ Transfer → Main Account
      +300.00` — one row per wallet, the leg it owns. Clicking a transfer
      answers "edited as a pair — arriving with the transfer screen" (M7).
- [x] Running totals per view; "All wallets" is a **feature, not the core** —
      behind the same selector, grouped by currency and labeled, never a
      single summed number. Smoke: Net runs −620 → −1,167.80 → … down the
      list, exactly the stored values; all-wallets shows the BRL group with
      its own Net and the never-summed note, soft-deleted wallets stay out
      of the groups (their leg is visible under their own selector entry).
- [x] Create/edit form from a row (and a "new" action): wallet, category,
      counterparty, dates, due/paid values (positive input; sign is the
      category's business — never send negative, show the server's stored
      value). Smoke: create −125.50 through the form, Net absorbed it;
      edit prefilled with abs() at the border, 8500 → 9000 on the wire,
      Net recalculated; "Due value cannot be zero" client-side; paid value
      only when Status is Paid; unpaid sends paidValue 0 (vitest-pinned).

## M7 — Transfers

- [x] Create transfer (`POST /api/transfers`) — source/destination/values/date.
      Own "⇄ Transfer" button beside "＋ New", own form (source/destination
      wallets, side-locked category selects — source offers expenses only,
      destination income only — dates, value positive, status, description).
      Client refuses same-wallet and zero before the wire. Smoke: created
      "vacation fund" 150 Main → Dollars through the form, both legs landed.
- [x] Update transfer — the linked pair, edited as one. Clicking a ⇄ row
      opens the pair form: `GET /api/transfers/{legId}` prefills it, wallets
      stated as locked text ("Main Account → Dollars — wallets are fixed
      after creation"), value/paid abs() at the border, PUT by either leg
      id. Smoke: 150 → 200 edited as one, both books recalculated (Main
      −200, Dollars +200), the untouched pair stayed at 300.
- [x] Transfer rows display as transfers, not as two mystery transactions.
      Delivered with M6's `walletBook` (one ⇄ row per wallet, its own leg) —
      M7 confirms it standing, now with the pair editable behind the row.

## M8 — Dashboard

- [x] Recent transactions + upcoming due lists, designed for glanceability
      (not two raw grids). Home's dead "Activity" placeholder became
      "Recent" (top 5, one line per movement — transfers paired first)
      and a new "Upcoming" section: unpaid rows, overdue first in red,
      then future by date. One wide search feeds both plus the chart.
- [x] Monthly chart, hand-rolled `<svg>`, series colors from `--c1..c4`.
      Bar pairs from a shared baseline — `--c2` income above, `--c3`
      expense below; geometry is pure (`chartBars`, vitest-pinned), the
      view only injects. Per-currency tabs when more than one wallet
      currency exists — the chart never sums across currencies.
- [x] Wallet totals grouped by currency (labeled, never summed across).
      Delivered with M4 (Wallets table + `netWorthLabel`/`sumByCurrency`)
      — M8 confirms it standing on the dashboard.

## M9 — Quick actions (due list)

The single-row power moves, surfaced where the money is already on screen.

- [x] Due actions on the book rows and the dashboard due list: pay on the due
      date, pay as today, reverse — inline, no dead-end. Undo reversal is the
      way back (a reversed row is never a dead-end either).

## M10 — Advanced search

A power tool, not a core screen — built after the book proves the search
endpoints' shaping.

- [x] The `/by` filters (wallet/category/person/counterparty windows) with the
      shared shaping of the two search endpoints (`order`/`limit`, date-type
      sort with `Id` tiebreak).
- [x] Result actions identical to the book rows (open, edit) — no dead-end
      results. Quick actions ride along (M9's builders, same buttons), and
      the edit forms themselves became shared components: MovementForm and
      TransferForm, extracted from the book so search and book edit rows
      through one truth.

## M11 — Bulk + Import (closing)

- [ ] Bulk transactions form (the `POST` bulk endpoint) — quick actions in
      mass, the sibling of M9's single-row moves.
- [ ] OFX/MT940 import: upload → parsed rows shown for review → post back
      (import endpoint is parse-only; the UI owns the write and the
      no-dedup warning).
- [ ] Default category pair per import (income/expense by sign, per
      `src/WebApi/AGENTS.md`).

## M12 — Beyond the DemoProject (optional, separate decisions)

Scenarios, invoices, change log viewer — the API exposes them, the
DemoProject never did. Treat each as its own milestone if wanted; do not
bundle into parity.

---

Progress log (append one line per milestone, date + one lesson):


2026-11-08 M0 — two lessons: (1) `typescript@7.0.2` broke vue-tsc 3.x
(`ERR_PACKAGE_PATH_NOT_EXPORTED` resolving the compiler binary) — pinned
`5.9.3`, the line the whole toolchain is built for; the Curated list records
the failed pair so nobody re-pins 7 blindly. (2) Composite/project-reference
tsconfigs require declaration emit, not `noEmit` — the scaffold's
`tsconfig.node.json` carries `emitDeclarationOnly`.

2026-11-08 M1 — two lessons: (1) smoke found that connect() saved the
active selection but nothing navigated — an emit on a route's root component
has no listener; navigate with `router.push` directly. (2) A created Key
lived only in a component ref: one reload between "create" and "save" lost
it forever — creation now auto-saves to localStorage before showing the
shown-once box (user-approved). Also: spec files are excluded from the app
tsconfig and type-checked by `vue-tsc -p tsconfig.test.json` (`test:types`).

2026-11-08 M1.1 (user request) — server selection split into its own screen:
single-server by design (no name, no list, button is Select). Storage lost
`serverLabel` everywhere (`webui.server` is a JSON string now); router guards
enforce the order server → account → home. Lesson: `saveServerUrl` once wrote
the raw URL while `getServerUrl` parsed it as JSON — asymmetric save/read
broke round-trip; a vitest round-trip case caught it before the smoke did.

2026-11-08 M2 — registry-as-data makes theme logic fully testable without a
browser (fake doc + mem storage), and that suite earned its keep before the
smoke: `isThemeName` used `in`, which walks the prototype chain —
`'__proto__'` counted as a theme. `Object.hasOwn` is the only correct lookup
for a string-keyed registry; the vitest case pins it.

2026-11-08 M3 — the shell loads the session, but only the connect flow marked
it ready: a direct reload on `/` (or any shell route) hung on "Loading…"
forever. Lesson: whoever loads the session owns `session.ready` — the flag
now flips in `AppShell.onMounted`, where the load actually happens. Second
lesson: `width:100%` + side padding inside a max-width column overflows
without `box-sizing:border-box` — the demo's global `*{box-sizing}` reset
hid it there; the app sets it per rule.

2026-11-08 M4 — smoke caught a doctrine bug the specs alone would have
blessed: a soft-deleted wallet kept its currency in the net-worth answer,
and "Balances in 2 currencies" was shown with one dead wallet and one live.
Lesson: "balances never cross currencies" is only half the rule — the
ledger a sum speaks for must be the living one. `liveBalances` now owns that
call as pure logic, pinned by vitest before the smoke re-run. Also: the
balances endpoint returns every wallet including hidden ones, so the
screens — not the server — decide what counts.

2026-11-08 M5 — the budget doctrine found its home in pure logic:
`MonthlyBudget` is a limit and never money, so it stays positive while the
expenses it measures are negative — `sumSpentByCategory` counts only
negative effective values, and a positive row under an expense category is
a reversal, not a purchase. One search feeds every track (grouping is
client-side), and an over-budget track is full, with the color — never
the width — saying how far past it went. The smoke also caught a routing
slip from M3: Home's categories "manage →" pointed at settings all along.

2026-11-08 M6 — the smoke caught the book's one real doctrine: filtering
the wire search by wallet cuts a transfer pair in half, because the partner
leg lives in another wallet — the pair never forms and the movement
vanishes from the month. The fix moved the cut client-side: one unfiltered
search per month, `pairTransfers` first, then `walletBook` picks the leg
each wallet owns (Main sees −300, Dollars sees +300, one row each).
Also: `monthWindow` left its duplicated copies in Home and Categories and
grew up into `months.ts` with prev/next/jump, and the edit's date-only
save (00:00Z vs the seeded 12:00Z) re-sorted the list — EffectiveDate is
the honest sort, so that reshuffle is correct, not a bug.

2026-11-08 M7 — a transfer is one decision with two legs, so it gets one
form: create writes the pair through POST /api/transfers, edit opens from
either leg and PUTs both at once, and the wallets never travel on the
update — they are stated, not asked ("Main Account → Dollars — wallets
are fixed after creation", the locked-side pattern M5 used for expense).
The category selects are side-filtered at the form (source offers expenses
only, destination income only), because the server would reject a swapped
pair one POST later. And the pair's value is a magnitude on the wire —
positive, always, the server re-signs each leg — the same "sign is the
category's business" doctrine the movement form already carried.

2026-11-08 M8 — the dashboard borrows every doctrine the earlier
milestones paid for: Recent pairs transfers first (M6's rule), Upcoming
owes the overdue first in red (never a date-only list), and the chart
inherits the net-worth rule — it never sums across currencies, so with
two wallet currencies it is one chart per tab. The smoke caught the
month label doing `income − expense` on a negative expense — 10.8k for
a 9000 month with 1793 spent — the net of a ledger where expense walks
below zero is a SUM, and that is the second time the sign doctrine paid
a test. Geometry lives in `chartBars`, pure and pinned; a month with no
money keeps its slot — a quiet month is a fact, not a gap.

2026-11-09 M9 — a quick action is the row's own truth rebuilt, not a
sketch: the wire has no patch verb, so every builder carries every field
and only what the action owns moves. The three power moves live inline
where the money already is — book rows and the dashboard due list share
one builder, never two truths — and a transfer is ONE decision even here:
pay or reverse crosses the pair through the transfer endpoint, both legs
together, because a half-flipped pair would lie. Pay as today is today:
when the honest timeline moves the row to another month the screen says
"moved to Nov", a row that just vanishes would look like a bug. And a
reversed row is not a dead-end — undo is the fourth quick action; the
simple row with no category gets none at all, because the server refuses
CategoryId 0 and the edit form is where it gets categorized first.

2026-11-09 M10 — the power tool earned its tri-state filters: every id is
"All", one, or "None" — and None is a real thing to ask for, because
"no category" is where the transfer legs live. The ids combine with AND
and the window is always explicit, the same doctrine the book already
paid for; and the server's own rule — a 'limit' never travels without
its 'order' — is said by the form first, not by a 400. The result rows
proved the deeper point: a limit cuts at the WIRE, so a pair may arrive
cut in half — the orphan leg is a fact, not a crash. And "identical to
the book rows" was taken literally: the movement and transfer forms left
the book and became components, so the found row is edited through the
same form that created it — one truth, no dead-end. The smoke caught a
preexisting hole in that form: Paid with an empty paid value wrote a
paid-of-zero row, a "payment" that paid nothing — paid-in-full is now
the empty field's meaning, pinned by a test.
