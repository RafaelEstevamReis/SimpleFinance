# Packages.md — npm package governance for Sample/WebUI

This file is a **standing contract between the human owner and any AI agent**
working on this project — including whoever takes it over without having been
part of the conversation that created it. Everything needed to honor it is on
this page; nothing else is assumed.

## The golden rule

> **Agents never search for or invent package names.**
> Agents **install freely** — but only exact `name@version` pairs **copied from
> the Curated list below**.

Concretely:

- Needing something already listed? Install it, no permission required.
- The Curated list is the **only** source of package names in this project.

## Why this exists

The danger is in *naming*, not in *installing*:

1. **AI-invented names ("slopsquatting").** LLMs sometimes emit names of
   packages that do not exist. Attackers register those exact names hoping an
   agent will install them. A single `npm install` with a fabricated name runs
   a stranger's code on this machine.
2. **Lookalike names ("typosquatting").** `vue-ruter` instead of `vue-router` —
   the oldest trick in the registry.
3. **Transitive dependencies.** Every npm package pulls in others nobody
   reviews together. This risk is accepted *consciously, once, per approval* —
   which is why each approval is deliberate and the lockfile is committed.

The list converts an open-ended risk into a closed one: a wrong name can only
enter through human review of a Proposal, and installing a listed package can
only reproduce what was already vetted.

## Verifying a Proposal — human checklist

When an agent files a Proposal with two candidates, check **each name**:

1. Open `https://www.npmjs.com/package/<exact-name>` — **copy/paste** the name
   from this file; do not retype it (retyping is how *you* become the typo).
2. On that page, confirm:
   - **Publisher** is a known organization (`vuejs`, `vitejs`, `Microsoft`, ...)
     — an organization beats an unknown individual.
   - **Weekly downloads** — six figures is normal for mainstream tooling.
     High is not a guarantee; low is a rejection.
   - **Published** — years of history and a recent release. A package created
     weeks ago is a rejection, no matter how good it looks.
3. Prefer the candidate that is older, more downloaded and officially published.
4. **In doubt, reject.** A rejection costs one conversation cycle; a bad
   approval can cost the machine.

Once a candidate passes: add it to the Curated list (name, version, publisher,
why-safe, which alternative was rejected and why), delete the Proposal, then
install. From that point the pair is vetted — agents run the install.

## Installing

```bash
cd Sample/WebUI
npm install               # installs the approved package.json
npm audit                 # must end with no critical/high findings
git add package.json package-lock.json
```

- `package-lock.json` is **always committed** — it is the record of what was
  approved and makes every future install reproduce it exactly.
- A version change is the same process as a new package: a Proposal first.
  There is no casual `npm update`.

### Cooldown: fresh releases must wait

Package versions that were **published less than 7 days ago** are off-limits,
including for upgrades. When a package is attacked, malicious versions are
usually pulled from the registry within days; waiting lets someone else be the
early adopter. If a Proposal pins a version younger than 7 days, the human
waits or pins the previous stable — the checklist above checks the
**Published** date for exactly this too.

## Curated list

| Package | Version | Publisher | Why it's safe | Rejected alternative |
|---|---|---|---|---|
| `vue` | `3.5.45` | vuejs | The framework; frozen public API since 2020; SLSA provenance | `svelte` — runes API young in agent training |
| `vite` | `8.3.6` | vitejs | De-facto standard dev server/build; provenance | raw `esbuild` — loses HMR/plugins |
| `@vitejs/plugin-vue` | `6.0.10` | vitejs | Official Vue-SFC plugin for Vite | — |
| `typescript` | `7.0.2` | Microsoft | Type checking; dev-only, not shipped | JSDoc — loses compile-time checks |
| `vue-router` | `5.4.0` | vuejs | Official router; guards account connection | `vue-router@4` — fallback if 5.x proves unfamiliar |
| `vue-tsc` | `3.3.14` | vuejs/language-tools | Type-checks `.vue` SFCs — `vite build` alone does not; provenance | — |
| `@types/node` | `26.6.6` | DefinitelyTyped (Microsoft) | Node types for `vite.config.ts`; dev-only | — |
| `@phosphor-icons/vue` | `2.2.1` | phosphor community | 9k SVG icons, tree-shakeable; no provenance, evaluated knowingly | `lucide-vue-next` — **dead name**: deprecated at latest in favor of `@lucide/vue` |
| `@tabler/icons-vue` | `3.50.0` | tabler | 5.9k SVG icons, SLSA provenance; peer-dep free | `@mdi/font` — webfont, not components; loads whole font for a few icons |
| `chart.js` | `4.6.1` | chartjs org | Stable API since 2.x; SLSA provenance | `echarts` — ~1MB for one chart |
| `vue-chartjs` | `5.3.4` | apertureless | Thin wrapper over chart.js; no provenance, vetted knowingly | `d3` — toolkit-scale API, largest hallucination surface |
| `vue-i18n` | `11.4.18` | intlify (kazupon) | Official i18n plugin, provenance; repo stays EN, the *user's* UI language is theirs | hard-coded strings — loses user-chosen language |
| `vite-plugin-pwa` | `2.0.0` | vite-pwa (antfu, userquin) | Installable-on-phone via manifest+SW; provenance; pulls workbox-build/window | hand `manifest.json` — enough for install, loses update/offline story |
| `file-saver` | `2.0.5` | eligrey | `saveAs()` Blob download; tiny, ancient and stable | — |
| `jspdf` | `4.2.1` | parallax (mrjameshall) | PDF generation client-side; no provenance, maintainer-published, evaluated knowingly | — |
| `dinero.js` | `2.0.2` | dinerojs (sarahdayan) | Immutable money objects, provenance; fallback if the hand-written money component ever needs arithmetic helpers | `currency.js` — unmaintained since 2021 |
| `vitest` | `5.1.0` | vitest-dev (Vite team; antfu, yyx990803) | Unit/component testing; SLSA provenance; peer-dep vite ^6.4/^7/^8 matches our kernel; engines node ^22.12 || ^24 || >=26 | `playwright` — e2e; heavier, needs browser binaries; smoke-by-milestone already covers the same ground |

## Open Proposals

*(none — all discussion-time proposals were approved and moved to the
Curated list above. Future proposals land here until evaluated:)*

> `### Proposal N — <what it is for>` · Option A: `name@version` — why ·
> Option B: `name@version` — why (often the rejected alternative) ·
> Reviewed ___ by ___ → approved / rejected — reason.

### Historical: evaluation notes behind the Curated list

- **Icons**: `lucide-vue-next` was the training-data favorite — the registry
  shows its latest as **deprecated, pointing to `@lucide/vue`** (a dead-name
  trap caught by the checklist; live name: `@lucide/vue@1.70.0`, provenance).
  Approved pair: Phosphor + Tabler. `@mdi/font` (Material) rejected: it is a
  **webfont**, not tree-shakeable SVG components — the whole font loads for a
  handful of icons.
- **Money**: hand-written component (see `Architecture.md`); `currency.js`
  rejected (unmaintained since 2021), `dinero.js` is the pre-agreed fallback
  already in the Curated list.
- **Testing**: `vitest` approved (Curated list) for unit/component tests —
  the money component (cents parsing, bank-typing) is the natural first
  target. `playwright` rejected for now: e2e duplicates the smoke-by-milestone
  contract and drags browser binaries; re-propose with new evidence.
  **axios, pinia, date-fns/luxon/dayjs, zod/vee-validate, @vueuse/core,
  eslint/prettier, unplugin-auto-import, lodash**: not needed —
  state is view-local; dates/formatting are `Intl`; validation is the
  API's 400s. Re-proposing requires new evidence (e.g. a CI step for this
  sample).

## Red flags

- An agent insisting on installing a package that is not listed.
- A proposal whose name is a near-miss of a famous package.
- "Just this once, skip Packages.md."
- A Curated-list entry that appeared without a matching reviewed Proposal.
