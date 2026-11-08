# Sample/WebUI — Vue 3 SPA over the WebApi

This is a **sample consumer** of the WebApi, in the same spirit as `Sample/DemoProject`
(a WinForms app): a browser front-end proving the API is complete enough to run
someone's finances from. It is a product only as far as it stays useful to its owner.

```mermaid
graph LR
  B["Browser<br/>Vue 3 SPA<br/>(static files)"] -->|"fetch + X-Api-Key<br/>header per call"| A["src/WebApi"]
  A --> M["ManagerCache<br/>(one SQLite per account)"]
```

## Ground rules

- **The WebUi is a plain HTTP client.** It never touches SQLite, never references
  the `Simple.Finance` library, never references the WebApi project. Everything
  goes through the REST endpoints — that is the point of the sample: it documents
  what the API surface can really do.

### API instructions

Any instruction about the API (endpoint shapes, parameters, statuses, traps)
comes from, in order:

1. **The repository** — the WebApi source under `src/WebApi/` is the truth.
2. **The live swagger** — if the WebApi is running, `http://<host>:<port>/swagger`
   documents the API as deployed. Use it when working against a remote server.

For how to *consume* the API properly (money rules, three dates, traps), read
`src/WebApi/AGENTS.md` first — it is written for exactly this kind of client.

- **The Key lives in the browser** (`localStorage`), one entry per saved account,
  with a switcher. This is by design: the Key *is* the whole credential and the
   API is stateless-multi-session — any number of clients can hold it at once.
  Consequence, accepted on purpose: JavaScript on the page can read it, so the
  SPA carries **zero third-party runtime code** (see `Packages.md`) to keep the
  XSS surface as small as the attack surface can be in a browser.
- **Read `Packages.md` before adding any npm package.** It is a governance
  contract, not a wish list.
- **English everywhere**, like the rest of the repository.

## Project layout (target)

```
Sample/WebUI/
├── Architecture.md        # this file — how the app is put together
├── Packages.md            # npm governance: the golden rule, checklists, curated list
├── README.md              # how to run it, day-to-day instructions
├── package.json           # only packages from the Packages.md curated list
├── vite.config.ts         # dev proxy — where /api forwards in dev is the developer's choice
├── index.html
└── src/
    ├── main.ts            # mount + router
    ├── api/
    │   ├── client.ts      # fetch wrapper: X-Api-Key header, error shaping, 401 handling
    │   └── types.ts       # DTO mirrors of the WebApi request/response shapes
    ├── components/        # hand-written, no UI kit — table, form fields, money display
    └── views/             # one file per screen: Login, Wallets, Transactions, ...
```

## Application state

All client-side, all trivial on purpose:

| Concern | Where | Notes |
|---|---|---|
| Saved servers | `localStorage["webui.servers"]` | `[{ label, baseUrl }]` — user-configured; nothing is baked in |
| Accounts per server | `localStorage["webui.accounts"]` | `[{ serverLabel, label, key }]`; a Key opens exactly one account on exactly one server |
| Active server + account | `localStorage["webui.active"]` | `{ serverLabel, key }`; switching re-navigates to the dashboard |
| Server data | component-local | no store, no pinia — each view fetches what it shows; the app is small enough that a store would be ceremony |

### Connecting to a server

The UI assists the connection instead of assuming a target:

1. **Add server** — user gives a base URL (e.g. `http://localhost:5080`). The UI
   validates with `GET /api/hello` (anonymous) and reports reachability/version
   before saving.
2. **Then an account** — paste an existing Key or create one (`POST /api/account`,
   anonymous on that server). Validated with `GET /api/account/database`.
3. Failed connection attempts surface the reason (unreachable, not-a-WebApi,
   invalid Key) — the user should never guess why a server does not respond.

Every API call goes to the **active server's** `baseUrl`; the client layer
prepends it and attaches the `X-Api-Key` header. The dev proxy (`vite.config.ts`)
is only a dev convenience — the production app always calls the configured
server directly (CORS on the WebApi is open).

## API integration details that matter

- Header: `X-Api-Key: <guid>` on every call except `POST /api/account` (anonymous,
  creates an account and returns the Key **once** — the UI must treat it as a
  secret shown exactly once, with a copy button and a "store it" warning).
- Dev proxy: Vite may forward `/api` to a server under development — a dev
  convenience only; the app itself always talks to the configured `baseUrl`.
- The API validates by throwing; the UI must surface the `400` `ProblemDetails`
  message as a readable error — it is the domain talking (category forces sign,
  `categoryId != 0`, `dueValue != 0`, wallet-currency mismatch, ...). Never
  "work around" a 400: they are the money rules, documented in `src/WebApi/AGENTS.md`.
- Dates on the wire are explicit UTC; the UI renders in the browser's locale
  but never re-sends what it parsed — it sends back what it received.
- Enums travel as names (`"Paid"`, `"WalletTransfer"`, ...) — display them as-is.

## Screens (initial scope)

1. **Connect** — add a server (base URL, validated with `GET /api/hello`), then
   paste an existing Key or create an account (`POST /api/account`). Validated
   with `GET /api/account/database`.
2. **Wallets** — list with balances; `GET /api/wallets`, one
   `GET /api/wallets/{id}/balance` per wallet (balances are per-wallet and never
   cross currencies — the screen must not sum them; if it ever wants a "total",
   group by currency and label it, never a single number).
3. **Transactions** — paginated table (`GET /api/transactions` with
   `start/end/kind/order/limit`), create form, transfer form, import OFX/MT940
   (parse-only endpoint — the UI posts back what it parsed, nothing is written
   by the import itself).

Beyond that: categories, persons, scenarios, invoices, change log — same
patterns, added screen by screen.

## What "done" means for a screen

- Fetches work against a live API (smoke: `dotnet run --project src/WebApi`).
- Errors from the API are shown with the server's message, not swallowed.
- Empty states are designed, not accidental.
- The build passes: `npm run build` with zero new packages outside `Packages.md`.
