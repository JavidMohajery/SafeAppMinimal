# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**SafeAppMinimal** is a minimal [SAFE Stack](https://safe-stack.github.io/) application — a full-stack F# web app using:
- **Saturn** (ASP.NET Core wrapper) for the backend
- **Fable** (F#-to-JavaScript transpiler) for the frontend
- **Elmish** / browser DOM APIs for client-side logic
- **Vite** as the frontend dev server and bundler

## Development Commands

### Prerequisites
- .NET SDK 8.0 (pinned in `global.json`)
- Node 18–22, npm 9–10 (enforced via `.npmrc`)

### Start the backend (terminal 1)
```bash
cd src/Server
dotnet watch run
# Listens on http://localhost:5000
```

### Start the frontend — two terminals required

Fable must finish its first compilation before Vite starts, otherwise Vite can't resolve `Client.fs.js` on startup.

**Terminal 2 — Fable watcher** (start this first):
```bash
cd src/Client
npm install        # first time only
dotnet tool restore  # first time only (installs Fable 4.9)
dotnet fable watch .
# wait for "Fable compilation finished" before starting Vite
```

**Terminal 3 — Vite dev server** (start after Fable prints "compilation finished"):
```bash
cd src/Client
npm run start
# Vite dev server on http://localhost:8080
# /api/* requests are proxied to :5000
```

### Build for production (output to `deploy/`)
```bash
dotnet fable src/Client --run npm run build
dotnet publish src/Server -c Release -o deploy
```

### Run backend tests
```bash
dotnet test
```

## Architecture

Three F# projects in `src/`, connected through shared types:

```
Shared/Shared.fs      ← route constants shared by both ends
Server/Server.fs      ← Saturn router + Giraffe HTTP handlers
Client/Client.fs      ← Fable app; compiled to JS and run in the browser
```

**Key flow:**
1. `Shared.fs` defines `Route.hello = "/api/hello"` — the single source of truth for API paths used by both client and server.
2. `Server.fs` registers a Saturn `application` with a Giraffe router; the `GET /api/hello` handler returns a plain text response. Static files are served from `src/Server/public/`.
3. `Client.fs` is compiled to JavaScript by the Fable toolchain. At startup it fetches `Route.hello`, then sets `document.getElementById("header").innerText` with the response.
4. `src/Client/vite.config.mts` proxies `/api/*` to port 5000 during development.

**Adding a new endpoint** requires three coordinated changes:
- Add the route constant in `Shared.fs`
- Add the handler in `Server.fs`
- Call it in `Client.fs`

## Toolchain Notes

- `fable_modules/` and `*.fs.js` files are generated — never edit them manually.
- The Fable tool is installed locally (`.config/dotnet-tools.json`); run `dotnet tool restore` after cloning.
- Fable compiles F# directly to ES modules consumed by Vite; there is no separate TypeScript step.
- Production build outputs to `../../deploy/public` (relative to `src/Client/`) as configured in `vite.config.mts`.
