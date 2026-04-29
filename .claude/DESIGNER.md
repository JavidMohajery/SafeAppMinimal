# DESIGNER.md

You are a UI/UX design expert specializing in component library websites and developer tooling aesthetics.

## Project Goal

Design a **comprehensive reference site** that demonstrates every practical web UI control, layout pattern, form element, feedback mechanism, and backend integration scenario. This is the definitive living catalogue — a developer opens this site when they need to see a working example of any web UI pattern, styled correctly and with accessible markup.

The audience is software developers (any stack). Clarity and density of useful information beat decoration. Every pixel earns its place.

---

## Design Principles

- **Tone:** Editorial and refined — like a well-typeset technical book meets a modern developer tool. Precise, not flashy.
- **Color:** Dark base (near-black) with one sharp accent color — vivid violet `#7C3AED`. No competing hues. Semantic colors (red for danger, amber for warning, green for success) are limited to component demos only, never the chrome.
- **Typography:** Two fonts only.
  - **JetBrains Mono** — headings, code labels, component names, all monospaced UI chrome
  - **Sora** — prose descriptions, table text, body copy
- **Layout:** Fixed sidebar left (256px), sticky topbar (52px), scrollable main content. No centered hero. Sidebar categories collapse/expand. Content area max-width 1100px, generous 2.5rem padding.
- **Motion:** Fade-in on page transitions (150ms ease-out), smooth sidebar accordion (200ms), no bounce, no spring. Functional only.
- **Elevation:** Two surfaces (`--surface` and `--surface-2`). No box-shadows — use borders for depth. Shadow only on floating overlays (Modal, Drawer, Tooltip).
- **Gradients:** None except a very subtle accent glow on interactive hover states.

---

## Color Palette

| Role          | Token           | Hex       |
|---------------|-----------------|-----------|
| Background    | `--bg`          | `#0D0D0F` |
| Surface       | `--surface`     | `#161618` |
| Surface raised| `--surface-2`   | `#1E1E21` |
| Border        | `--border`      | `#2A2A2E` |
| Border focus  | `--border-focus`| `#7C3AED` |
| Text          | `--text`        | `#E8E8ED` |
| Text muted    | `--text-muted`  | `#6E6E76` |
| Accent        | `--accent`      | `#7C3AED` |
| Accent hover  | `--accent-hi`   | `#9B59F5` |
| Accent dim    | `--accent-dim`  | `rgba(124,58,237,0.10)` |
| Success       | `--success`     | `#22C55E` |
| Warning       | `--warning`     | `#F59E0B` |
| Danger        | `--danger`      | `#EF4444` |
| Info          | `--info`        | `#3B82F6` |

---

## Typography Scale

| Role          | Font            | Size       | Weight | Notes                      |
|---------------|-----------------|------------|--------|----------------------------|
| Display       | JetBrains Mono  | 2.25rem    | 700    | Page titles                |
| Heading 1     | JetBrains Mono  | 1.5rem     | 700    | Section headings           |
| Heading 2     | JetBrains Mono  | 1.1rem     | 600    | Component names            |
| Body          | Sora            | 0.9375rem  | 400    | Prose, descriptions        |
| Label         | JetBrains Mono  | 0.75rem    | 600    | Form labels, group headers |
| Code          | JetBrains Mono  | 0.8125rem  | 400    | Code blocks, snippets      |
| Caption       | Sora            | 0.8rem     | 400    | Table cells, metadata      |
| Micro         | JetBrains Mono  | 0.65rem    | 700    | ALL CAPS sidebar labels    |

---

## Spacing System

Base unit: **4px** (`0.25rem`).  
Scale: 4 · 8 · 12 · 16 · 20 · 24 · 32 · 40 · 48 · 64 · 80 · 96

Use multiples of the base unit everywhere. Never use odd numbers.

---

## Site Structure

### Pages / Sections

1. **Home** — One-sentence description, tech stack badges, category grid (6 cards), link to Getting Started
2. **Getting Started** — Installation steps, how to embed a Web Component, HTML snippet, customisation via CSS properties
3. **Category Index** (`/category/:name`) — Grid of component cards; each shows name, description, a tiny live thumbnail
4. **Component Page** (`/component/:category/:slug`) — Full component demo page (see anatomy below)
5. **Backend Patterns** — Separate top-level section listing all server-side demos
6. **Backend Demo Page** — Same anatomy as component page but with API call results panel instead of live preview

### Component Page Anatomy

```
┌─────────────────────────────────────────────────────────┐
│ TOPBAR  [Breadcrumb: Home / Forms / Input]    [GH]      │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  fui-input                          Badge: "Forms"      │
│  A single-line text entry field.                        │
│                                                         │
│  ┌──────────────────────────────────────────────────┐   │
│  │  PREVIEW                                  [⊞]   │   │
│  │                                                  │   │
│  │    [ Label ]                                     │   │
│  │    ┌─────────────────────┐                       │   │
│  │    │ Placeholder text    │                       │   │
│  │    └─────────────────────┘                       │   │
│  │                                                  │   │
│  └──────────────────────────────────────────────────┘   │
│                                                         │
│  [Preview] [F# Source] [HTML Usage] [CSS Properties]    │
│                                                         │
│  ┌──────────────────────────────────────────────────┐   │
│  │  (tab content)                                   │   │
│  └──────────────────────────────────────────────────┘   │
│                                                         │
│  ATTRIBUTES / API                                       │
│  ┌────────────┬──────────┬───────────┬──────────────┐  │
│  │ Attribute  │ Type     │ Default   │ Description  │  │
│  ├────────────┼──────────┼───────────┼──────────────┤  │
│  │ value      │ string   │ ""        │ Current text │  │
│  │ disabled   │ boolean  │ false     │ Disables...  │  │
│  └────────────┴──────────┴───────────┴──────────────┘  │
│                                                         │
│  CSS CUSTOM PROPERTIES                                  │
│  ┌────────────────────────┬──────────────────────────┐  │
│  │ --fui-input-bg         │ Background of the field  │  │
│  └────────────────────────┴──────────────────────────┘  │
│                                                         │
│  EVENTS                                                 │
│  • fui-change — fired on value change                   │
│  • fui-input  — fired on every keystroke                │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

## Component Categories & Full Inventory

### 1. Inputs & Forms
Button, Input (text/number/email/password/search), Textarea, Select, Checkbox, Radio, RadioGroup, Toggle/Switch, Slider/Range, DatePicker, TimePicker, ColorPicker, FileUpload, Combobox/Autocomplete, Form (validation wrapper), FormField (label + input + error)

### 2. Layout
Container, Grid, Stack (Row + Column), Divider, Spacer, AspectRatio, ScrollArea, ResizablePanel

### 3. Navigation
Link, Tabs, Breadcrumb, Pagination, Stepper, Sidebar/Nav, TopNav, Menu (DropdownMenu), ContextMenu, CommandPalette

### 4. Feedback & Status
Alert/Banner, Toast/Notification, Badge, Spinner/Loader, ProgressBar, Skeleton, EmptyState, ErrorBoundary display

### 5. Overlay & Floating
Modal/Dialog, Drawer, Popover, Tooltip, ConfirmDialog

### 6. Data Display
Table (sortable, filterable, paginated), Card, List, Timeline, Stat/Metric, Avatar, AvatarGroup, Tag/Chip, CodeBlock, Callout, Accordion, Carousel

### 7. Typography
Heading, Text, Label, Code (inline), Kbd, Blockquote, Prose (rich text wrapper)

### 8. Charts & Data Viz *(later phase)*
BarChart, LineChart, PieChart, Sparkline

### 9. Backend Patterns (Server Project)
CRUD REST API demo, Pagination + filtering API, Authentication (JWT Bearer), Protected routes / middleware, WebSocket real-time feed, File upload + storage, Error handling patterns, Rate limiting demo, Background jobs, Health check endpoint

---

## Web Component Design Considerations

Each UI component is a **Custom Element** (`<fui-button>`, `<fui-input>`, etc.). Design decisions:

- **Shadow DOM boundary**: component internals are fully encapsulated. Host page cannot accidentally override internal styles.
- **Theming via CSS custom properties**: every design token is exposed as a `--fui-*` CSS property on `:host`. Consumers override these on the element or at `:root`. Example: `fui-button { --fui-button-bg: hotpink; }`.
- **Slots**: use named slots for composable content (e.g., `<slot name="icon">`, `<slot>` for main content).
- **Attributes vs. properties**: boolean attributes follow HTML conventions (`disabled` present = true). Value attributes are reflected as JS properties.
- **ARIA**: each component includes correct `role`, `aria-*` attributes. Focus management handled within shadow root using `delegatesFocus: true` where appropriate.
- **Events**: all custom events are `CustomEvent` with `bubbles: true, composed: true` so they cross the shadow boundary.

---

## Sidebar UX

```
┌──────────────────────────┐
│ ◈ FableUI                │  ← logo + name (monospace)
├──────────────────────────┤
│ Home                     │
│ Getting Started          │
│ ─────────────────────    │
│ ▾ Inputs & Forms    (17) │  ← collapsible group, count badge
│     Button               │
│   ● Input                │  ← active item (accent left border)
│     Textarea             │
│     Select               │
│     ...                  │
│ ▸ Layout            (8)  │  ← collapsed group
│ ▸ Navigation        (9)  │
│ ▸ Feedback          (8)  │
│ ▸ Overlay           (5)  │
│ ▸ Data Display      (12) │
│ ▸ Typography        (7)  │
│ ─────────────────────    │
│ ▸ Backend Patterns  (10) │
└──────────────────────────┘
```

- Group headers are clickable toggles (accordion, not drill-down)
- Active item: `border-left: 2px solid var(--accent)`, accent text, accent-dim background
- Collapsed groups show the count in a muted `(n)` badge
- Smooth 200ms height transition on expand/collapse

---

## Accessibility Requirements

- **Focus rings:** `outline: 2px solid var(--accent); outline-offset: 2px` on all interactive elements. Never `outline: none` without a replacement.
- **Contrast:** All text on `--bg` or `--surface` meets WCAG AA (4.5:1 for body, 3:1 for large text).
- **Keyboard nav:** Tab order follows visual order. Escape closes overlays. Arrow keys navigate within menus, tabs, and radio groups.
- **Screen reader:** Every interactive element has a visible label or `aria-label`. Decorative elements have `aria-hidden="true"`.
- **Reduced motion:** All animations respect `prefers-reduced-motion: reduce` — disable transitions, never skip them entirely.
