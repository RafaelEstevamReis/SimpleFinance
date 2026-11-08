# LOCAL.md — unversioned space for local notes

Copy this file to `LOCAL.md` (same folder). That copy is **gitignored** (you must check) and
never leaves your machine: it is a scratch space for whatever the owner of
this fork wants to keep locally about their setup.

## The one rule

> **Whatever is in `LOCAL.md` stays in `LOCAL.md`.**
>
> An AI agent may read it for context, but must never copy its contents into
> tracked files, commit messages, branch names or pull request descriptions.
> "It's just a note" is how something private becomes public.

## How the ignore works

- `.gitignore` covers `LOCAL.md` in any letter case (`[Ll][Oo][Cc][Aa][Ll].md`)
  — on Windows the filesystem is case-insensitive and an agent might create
  `Local.md`; the rule still catches it.
- Git only ignores files it is not already tracking. If you ever see
  `LOCAL.md` in `git status`, **stop and check before committing** — something
  is wrong, and committing it would defeat its whole purpose.
