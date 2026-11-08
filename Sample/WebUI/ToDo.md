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

- [ ] `src/styles/themes.ts` — registry `satisfies Record<string, Theme>`,
      ship set: `slate` (default), `azure`, `emerald`, `violet`, `amber`, `rose`.
- [ ] `applyTheme(name)` sets CSS custom properties on `<html>`.
- [ ] Read the account's theme from `GET /api/account/preferences` on connect;
      write with `PUT` from a picker; cache per-account for login preview.
- [ ] Connect screen cards tinted by each account's accent (preview cache).
- [ ] *Subtle by design* respected (see `Architecture.md` — accent only,
      never text/background/semantic colors).

## M3 — App shell + Wallets + Categories

First money screens.

- [ ] Shell: top bar (account label + accent tint), nav, error area.
- [ ] Wallets screen: list, create/edit form, balance per wallet
      (`GET /api/wallets/{id}/balance`), no cross-currency sums.
- [ ] Categories screen: list, create/edit, `isExpense` locked after creation
      (UI shows it read-only), `monthlyBudget` field.
- [ ] Empty states designed; API errors shown with server message.

## M4 — Transaction book — the transactions screen

The book is not a later view: **it is how the WebUi displays transactions** —
one wallet, one closed month. The simplest useful shape a transaction list
can have; how the screen *designs around* that shape is open.

- [ ] Wallet selector + **month browser** (prev/next/jump) over the search
      endpoints — month window, ordered by date, `Id` tiebreak.
- [ ] Rows show: date, description, counterparty, value with sign as stored;
      transfers display as transfers.
- [ ] Running totals per view; "All wallets" is a **feature, not the core** —
      behind the same selector, grouped by currency and labeled, never a
      single summed number.
- [ ] Create/edit form from a row (and a "new" action): wallet, category,
      counterparty, dates, due/paid values (positive input; sign is the
      category's business — never send negative, show the server's stored
      value).

## M5 — Transfers

- [ ] Create transfer (`POST /api/transfers`) — source/destination/values/date.
- [ ] Update transfer — the linked pair, edited as one.
- [ ] Transfer rows display as transfers, not as two mystery transactions.

## M6 — Dashboard

- [ ] Recent transactions + upcoming due lists, designed for glanceability
      (not two raw grids).
- [ ] Monthly chart, hand-rolled `<svg>`, series colors from `--c1..c4`.
- [ ] Wallet totals grouped by currency (labeled, never summed across).

## M7 — Quick actions (due list)

The single-row power moves, surfaced where the money is already on screen.

- [ ] Due actions on the book rows and the dashboard due list: pay on the due
      date, pay as today, reverse — inline, no dead-end.

## M8 — Advanced search

A power tool, not a core screen — built after the book proves the search
endpoints' shaping.

- [ ] The `/by` filters (wallet/category/person/counterparty windows) with the
      shared shaping of the two search endpoints (`order`/`limit`, date-type
      sort with `Id` tiebreak).
- [ ] Result actions identical to the book rows (open, edit) — no dead-end
      results.

## M9 — Bulk + Import (closing)

- [ ] Bulk transactions form (the `POST` bulk endpoint) — quick actions in
      mass, the sibling of M7's single-row moves.
- [ ] OFX/MT940 import: upload → parsed rows shown for review → post back
      (import endpoint is parse-only; the UI owns the write and the
      no-dedup warning).
- [ ] Default category pair per import (income/expense by sign, per
      `src/WebApi/AGENTS.md`).

## M10 — Beyond the DemoProject (optional, separate decisions)

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
