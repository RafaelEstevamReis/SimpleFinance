# Sample/WebUI

A Vue 3 single-page app that runs someone's finances through the WebApi —
the browser counterpart of `Sample/DemoProject` (WinForms).

- How the app is put together: **`Architecture.md`**
- Before adding any npm package, read: **`Packages.md`** (governance contract)
- The API's own rules and traps: **`src/WebApi/AGENTS.md`**

## Prerequisites

- Node.js LTS (the only non-.NET tool this sample needs).
- A running WebApi to talk to — yours (`dotnet run --project src/WebApi`) or
  any remote instance. The app does not assume a target: it asks.

## Running

```bash
cd Sample/WebUI
npm install        # first time only; see Packages.md for what this means
npm run dev        # Vite dev server, hot reload on every save
```

Open the printed URL. The first screen asks for the **server** base URL (the
app checks it is reachable and really a WebApi before saving — Select), then
the account screen: paste an existing **Key** — or create a new account on
that server (the Key is shown exactly once; store it). The selected server and
its saved accounts stay in the browser; the `Server…` button goes back to
server selection.

## Building

```bash
npm run build      # static output in dist/ — serve it from anywhere
```

The build is plain static files pointing at whatever servers the user
configures — same flow, no rebuild needed to change targets.

## Day-to-day notes

- **Multiple servers and accounts**: the connect screen keeps the list in the
  browser and switches between them — each account is one isolated database
  on one server, and each Key opens exactly one of them.
- **Errors are the money rules**: when a form is rejected with a 400, the
  server's message is the domain talking (sign forced from the category,
  non-zero values, wallet currency match). The UI shows it as-is; do not work
  around it.
- **Nothing is deduplicated**: posting the same expense twice creates two
  expenses. The UI does not retry POSTs on its own.
- **No undo exists**: money records are never deleted. Corrections are new
  records. The UI offers no delete button where the API offers no endpoint.

## Forking this repo (local use with AI agents)

This sample is expected to be forked and run locally. 
That workflow has one rule that protects everything else:

> **The repository is public material. Keys, real server addresses and
> personal data never belong in it — not in code, not in docs, not in
> commit history.**

- Nothing personal was designed to live in files: servers and accounts are
  kept in the **browser's localStorage** (see `Architecture.md`), and no
  setting in this repo points at anything real. Keep it that way in your fork.
- **Agents write docs too.** An agent "helpfully" updating this README, a
  config file or the dev proxy with *your* real server address — "so it's
  easier next time" — is the most common leak in agent-assisted forks.
  Review every diff before committing; an address that is only yours must
  never reach the repo, not even in examples, comments or commit messages.
- **A Key cannot be revoked.** The WebApi has no revocation: whoever holds a
  Key holds that account forever. If one leaks into a commit or a pushed
  fork, treat that account as public from that moment — the only response
  is a new account and new Keys.
- A fork of a public repository is itself public, and unpublished branches
  can be pushed later: what you would not say in the open, keep out of git.
- Before opening a PR upstream, check the diff for anything personal —
  checking is cheap; un-leaking is not.
