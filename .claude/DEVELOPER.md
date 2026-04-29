# DEVELOPER.md

You are an expert F# developer specializing in Fable, Elmish, and Fable.Lit for browser-based UI development.

## Project Goal

Build a **comprehensive reference site** demonstrating every practical web UI control, layout pattern, and server-side pattern. Each UI component is a **Web Component (Custom Element)** written with **Fable.Lit** — framework-agnostic, embeddable in any HTML page regardless of tech stack. The showcase site shell (routing, sidebar, topbar, preview panels) uses Fable + Elmish for navigation state only.

---

## Architecture

```
┌──────────────────────────────────────────────────────────────┐
│  SHOWCASE SHELL  (Fable.React + Elmish)                      │
│  • Hash-based routing (discriminated union)                  │
│  • Sidebar, topbar, breadcrumb, preview/code/attr tabs       │
│  • Renders  <fui-*>  elements inside preview panels          │
│  • Code snippet display, attribute tables, CSS-prop tables   │
└──────────────────────┬───────────────────────────────────────┘
                       │ embeds as plain HTML elements
┌──────────────────────▼───────────────────────────────────────┐
│  COMPONENT LIBRARY  (Fable.Lit → Web Components)             │
│  • Each component = one Custom Element  <fui-*>              │
│  • Zero React / Zero Elmish inside component internals       │
│  • Lit reactive properties + Shadow DOM for encapsulation    │
│  • CSS custom properties (--fui-*)  for theming              │
│  • CustomEvent (bubbles+composed) for output signals         │
│  • Drop the compiled JS into any HTML page, it just works    │
└──────────────────────────────────────────────────────────────┘
```

---

## Tech Stack

| Layer             | Technology                                         |
|-------------------|----------------------------------------------------|
| Language          | F# (latest stable)                                 |
| Compiler          | Fable 4.x → ES modules                             |
| UI Components     | **Fable.Lit** (F# bindings for Lit)                |
| Component base    | **Lit 2.x** (`LitElement`, `html`, `css` from npm) |
| Showcase shell    | Fable.React 9.x + Elmish 4.x                       |
| Styling           | Plain CSS (design tokens) + CSS custom props       |
| Build             | Vite 5 (no extra plugin needed for Lit)            |
| Backend           | Saturn (ASP.NET Core) + Giraffe                    |

---

## Fable.Lit — How It Works

**Fable.Lit** is a Fable binding library for [Lit](https://lit.dev) by Google. Lit provides a tiny base class (`LitElement`) that turns a plain JS class into a fully encapsulated Web Component. Fable.Lit exposes this through a function-component style with the `[<LitElement>]` attribute.

- The `[<LitElement("tag-name")>]` attribute registers the custom element and wires up Lit's reactive system.
- `LitElement.init` configures styles (scoped to shadow DOM) and reactive props (mapped to HTML attributes).
- `html $"..."` is Lit's tagged template literal, expressed in F# as string interpolation. Lit efficiently diffs and patches only changed parts of the DOM.
- `css $"..."` produces a `CSSResult` that Lit injects into the shadow root — styles are fully encapsulated.
- `Hook.useState`, `Hook.useEffect` etc. work like React hooks for local state that is not exposed as an attribute.

### Dependencies

**npm** (`src/Client/package.json`):
```json
"dependencies": {
    "lit": "^2.8.0"
}
```

**NuGet** (`src/Client/Client.fsproj`):
```xml
<PackageReference Include="Fable.Lit" Version="1.4.2" />
```

No extra Vite plugin is required — Lit is plain ES modules and Vite bundles it natively.

---

## Component Pattern (Fable.Lit)

Every Web Component follows this exact structure:

```fsharp
// WebComponents/Inputs/FuiButton.fs
module WebComponents.FuiButton

open Lit
open Browser.Types
open Fable.Core.JsInterop

// ── Styles ────────────────────────────────────────────────────────────────────
// Written once, injected into shadow root by Lit. Fully scoped — no leakage.
// Use --fui-<tag>-<prop> custom properties for every overridable value.

let private styles = css """
    :host {
        display: inline-block;
        font-family: var(--fui-button-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    button {
        background  : var(--fui-button-bg,     #7C3AED);
        color       : var(--fui-button-color,  #fff);
        border      : var(--fui-button-border, none);
        border-radius: var(--fui-button-radius, 6px);
        padding     : 0.5rem 1.25rem;
        font-family : inherit;
        font-size   : 0.875rem;
        font-weight : 600;
        cursor      : pointer;
        transition  : opacity 0.15s, background 0.15s;
    }
    button:hover   { background: var(--fui-button-bg-hover, #9B59F5); }
    button:focus-visible { outline: 2px solid var(--fui-button-bg, #7C3AED); outline-offset: 2px; }
    :host([disabled]) button { opacity: 0.45; cursor: not-allowed; pointer-events: none; }
"""

// ── Component ─────────────────────────────────────────────────────────────────

[<LitElement("fui-button")>]
let FuiButton() =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            variant  = Prop.Of("primary", attribute = "variant")
            disabled = Prop.Of(false,     attribute = "disabled")
        |}
    )

    html $"""
        <button
            part="button"
            ?disabled={props.disabled.Value}
            data-variant={props.variant.Value}>
            <slot></slot>
        </button>
    """
```

### Key Lit Template Syntax in F# Strings

| Pattern | Meaning |
|---------|---------|
| `{expr}` | Bind a value (attribute value, text content) |
| `?disabled={boolExpr}` | Boolean attribute: present when true, absent when false |
| `@click={handler}` | Event listener binding |
| `<slot></slot>` | Default slot (child content goes here) |
| `<slot name="icon"></slot>` | Named slot |
| `.value={expr}` | DOM property binding (bypasses attribute serialization) |

### Local State with Hooks

For state that is not an HTML attribute (e.g. open/closed toggle, current value):

```fsharp
[<LitElement("fui-toggle")>]
let FuiToggle() =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {| checked = Prop.Of(false, attribute = "checked") |}
    )

    // Local reactive state — triggers re-render on change
    let isOn, setIsOn = Hook.useState(props.checked.Value)

    let toggle _ =
        let next = not isOn
        setIsOn next
        // dispatchCustomEvent defaults to bubbles=true, composed=true
        host.dispatchCustomEvent("fui-change", detail = {| value = next |})

    html $"""
        <button
            part="track"
            role="switch"
            aria-checked={string isOn}
            @click={toggle}>
            <span part="thumb" class={if isOn then "on" else ""}></span>
        </button>
    """
```

### Firing Custom Events

Use the `dispatchCustomEvent` extension method provided by Fable.Lit — it defaults to `bubbles = true` and `composed = true`:

```fsharp
// Inside any LitElement component where `host: LitElement`
host.dispatchCustomEvent("fui-change", detail = {| value = next |})
```

### Embedding in the Elmish Preview Panel

The showcase shell renders `<fui-*>` elements as regular HTML tags via `domEl` from Fable.React:

```fsharp
open Fable.React
open Fable.React.Props

let preview =
    div [ ClassName "preview-panel" ] [
        // React renders the custom element tag; Lit's custom element handles the rest
        domEl "fui-button" [ HTMLAttr.Custom("variant", "primary") ] [ str "Save changes" ]
        domEl "fui-button" [ HTMLAttr.Custom("variant", "ghost"); HTMLAttr.Custom("disabled","") ] [ str "Cancel" ]
    ]
```

---

## Project Structure

```
src/
  Client/
    Client.fs                     -- Showcase app: Elmish program, routing, shell view
    Pages/
      Home.fs                     -- Home page
      GettingStarted.fs           -- Getting started guide
      CategoryIndex.fs            -- Category grid
      ComponentPage.fs            -- Generic component demo page shell
      BackendDemoPage.fs          -- Backend pattern demo page
    WebComponents/
      Events.fs                   -- Shared: fire() helper for CustomEvent
      Inputs/
        FuiButton.fs              -- <fui-button>
        FuiInput.fs               -- <fui-input>
        FuiTextarea.fs            -- <fui-textarea>
        FuiSelect.fs              -- <fui-select>
        FuiCheckbox.fs            -- <fui-checkbox>
        FuiRadioGroup.fs          -- <fui-radio-group>  +  <fui-radio>
        FuiToggle.fs              -- <fui-toggle>
        FuiSlider.fs              -- <fui-slider>
        FuiDatePicker.fs          -- <fui-date-picker>
        FuiColorPicker.fs         -- <fui-color-picker>
        FuiFileUpload.fs          -- <fui-file-upload>
        FuiCombobox.fs            -- <fui-combobox>
      Layout/
        FuiDivider.fs             -- <fui-divider>
        FuiStack.fs               -- <fui-stack>
        FuiScrollArea.fs          -- <fui-scroll-area>
      Navigation/
        FuiTabs.fs                -- <fui-tabs>  +  <fui-tab>
        FuiBreadcrumb.fs          -- <fui-breadcrumb>
        FuiPagination.fs          -- <fui-pagination>
        FuiStepper.fs             -- <fui-stepper>
        FuiMenu.fs                -- <fui-menu>
      Feedback/
        FuiAlert.fs               -- <fui-alert>
        FuiToast.fs               -- <fui-toast>
        FuiBadge.fs               -- <fui-badge>
        FuiSpinner.fs             -- <fui-spinner>
        FuiProgress.fs            -- <fui-progress>
        FuiSkeleton.fs            -- <fui-skeleton>
      Overlay/
        FuiModal.fs               -- <fui-modal>
        FuiDrawer.fs              -- <fui-drawer>
        FuiTooltip.fs             -- <fui-tooltip>
        FuiPopover.fs             -- <fui-popover>
      DataDisplay/
        FuiTable.fs               -- <fui-table>
        FuiCard.fs                -- <fui-card>
        FuiAvatar.fs              -- <fui-avatar>
        FuiTag.fs                 -- <fui-tag>
        FuiCodeBlock.fs           -- <fui-code-block>
        FuiAccordion.fs           -- <fui-accordion>
      Typography/
        FuiKbd.fs                 -- <fui-kbd>
        FuiCallout.fs             -- <fui-callout>
      ComponentRegistry.fs        -- All ComponentMeta records + registerAll()
    UI/
      PreviewPanel.fs             -- Preview wrapper (Fable.React)
      CodeTab.fs                  -- Syntax-highlighted <pre><code> (Fable.React)
      AttributeTable.fs           -- Props/API table (Fable.React)
  Server/
    Server.fs                     -- Saturn router + all demo handlers
    Demos/
      CrudApi.fs
      AuthApi.fs
      WebSocketHub.fs
      FileApi.fs
      PaginationApi.fs
      RateLimitApi.fs
      HealthApi.fs
  Shared/
    Shared.fs                     -- Route constants, shared DTO types
```

---

## Adding a New Component — Workflow Note

After adding a `<Compile>` entry to `Client.fsproj`, **restart `dotnet fable watch .`** before testing. The watcher reads the project graph at startup; new files added mid-session are invisible to the running watcher.

---

## Compile Order in Client.fsproj

F# files must be listed in dependency order — a file may only reference files listed above it:

```xml
<ItemGroup>
  <!-- Web Components -->
  <Compile Include="WebComponents/Events.fs" />
  <!-- Inputs -->
  <Compile Include="WebComponents/Inputs/FuiButton.fs" />
  <Compile Include="WebComponents/Inputs/FuiInput.fs" />
  <!-- ... all other WC files in dependency order ... -->
  <Compile Include="WebComponents/ComponentRegistry.fs" />
  <!-- Showcase UI helpers -->
  <Compile Include="UI/AttributeTable.fs" />
  <Compile Include="UI/CodeTab.fs" />
  <Compile Include="UI/PreviewPanel.fs" />
  <!-- Pages -->
  <Compile Include="Pages/Home.fs" />
  <Compile Include="Pages/GettingStarted.fs" />
  <Compile Include="Pages/CategoryIndex.fs" />
  <Compile Include="Pages/ComponentPage.fs" />
  <Compile Include="Pages/BackendDemoPage.fs" />
  <!-- Root -->
  <Compile Include="Client.fs" />
</ItemGroup>
```

---

## Component Registry

```fsharp
// WebComponents/ComponentRegistry.fs
module ComponentRegistry

type AttrDef    = { Name: string; Type: string; Default: string; Description: string }
type CssPropDef = { Name: string; Description: string }
type EventDef   = { Name: string; Description: string }

type ComponentMeta = {
    Tag          : string          // "fui-button"
    Name         : string          // "Button"
    Slug         : string          // "button"
    Category     : string          // "Inputs & Forms"
    Description  : string
    Attributes   : AttrDef list
    CssProps     : CssPropDef list
    Events       : EventDef list
    FSharpSource : string          // raw F# shown in the code tab
    HtmlUsage    : string          // minimal HTML snippet
}

let all : ComponentMeta list = [
    {
        Tag         = "fui-button"
        Name        = "Button"
        Slug        = "button"
        Category    = "Inputs & Forms"
        Description = "Clickable button element with variant, disabled, and full keyboard support."
        Attributes  = [
            { Name="variant";  Type="string";  Default="primary"; Description="primary | secondary | ghost | danger" }
            { Name="disabled"; Type="boolean"; Default="false";   Description="Disables the button" }
        ]
        CssProps = [
            { Name="--fui-button-bg";       Description="Background color" }
            { Name="--fui-button-bg-hover"; Description="Background on hover" }
            { Name="--fui-button-color";    Description="Text color" }
            { Name="--fui-button-radius";   Description="Border radius" }
            { Name="--fui-button-font";     Description="Font family" }
        ]
        Events = [
            { Name="click"; Description="Native click (no custom wrapper needed)" }
        ]
        FSharpSource = "(see WebComponents/Inputs/FuiButton.fs)"
        HtmlUsage    = """<fui-button variant="primary">Save changes</fui-button>"""
    }
    // ... one record per component
]

// Called once at app startup — registers all custom elements with the browser
let registerAll () =
    // Each FuiXxx module auto-registers via [<LitElement("fui-xxx")>]
    // This function exists to trigger module initialization by referencing each type.
    // Fable.Lit registers the element when the module is first evaluated.
    WebComponents.FuiButton.FuiButton |> ignore
    // ... repeat for every component
```

---

## Routing (Showcase Shell)

```fsharp
type Route =
    | Home
    | GettingStarted
    | Category  of name: string
    | Component of category: string * slug: string
    | Backend   of slug: string
    | NotFound

let parsePage (hash: string) : Route =
    match hash.TrimStart('#').TrimStart('/').Split('/') |> Array.toList with
    | [] | [""]                        -> Home
    | ["getting-started"]              -> GettingStarted
    | ["category"; cat]                -> Category cat
    | ["component"; cat; slug]         -> Component(cat, slug)
    | ["backend"; slug]                -> Backend slug
    | _                                -> NotFound
```

---

## Implementation Rules

### Fable.Lit / Web Component rules
- One `[<LitElement("fui-*")>]` function per file — the function name matches the tag in PascalCase (`FuiButton` for `fui-button`)
- **Never nest `html $"""..."""` inside another `html $"""..."""` interpolation hole.** F# forbids triple-quote literals inside triple-quote interpolated strings. Extract the inner template to a named `let` binding first, then reference it in the outer hole.
- **Always annotate lambda parameters when using `Lit.mapUnique` with anonymous record types.** F# can't infer `o`'s type from the seq alone — write `(fun (o: {| value: string; label: string |}) -> ...)` or extract to a named `let` with a type annotation.
- **Never use `name` as a prop field name.** Fable compiles F# `name` → JS `__name`, which collides with Fable.Lit's HMR `get __name()` getter and throws at element construction. Use `fieldName = Prop.Of("", attribute = "name")` instead. Same caution applies to any identifier Fable might mangle (e.g. `type` → use `inputType`).
- All overridable visual values are CSS custom properties with `--fui-<tag>-<prop>` naming
- Every `:host` style starts with `display: block` or `display: inline-block`
- Boolean HTML attributes use Lit's `?attr={bool}` binding — never set `"true"/"false"` strings
- Custom events always use `bubbles = true; composed = true` so they cross the shadow boundary
- `part="..."` attributes expose internals for host-page `::part()` styling
- `role` and `aria-*` are mandatory for interactive components
- `Hook.useEffect` for any imperative setup (timers, resize observers, global listeners)

### F# style rules
- Pipe operators, pattern matching, discriminated unions — always idiomatic F#
- No mutable variables except inside closures where the DOM requires it
- Template strings (`html $"..."`) use F# string interpolation — escape `{` as `{{` for literal braces
- Group styles in a named `let private styles = css "..."` binding, not inline

### Showcase shell rules
- All navigation state through Elmish `Model` / `Msg` / `dispatch`
- `ComponentPage.fs` is generic — it looks up the slug in `ComponentRegistry.all` and renders the meta
- Never duplicate component logic in the showcase — always embed the real `<fui-*>` element

---

## Backend Pattern Structure

```fsharp
// Server/Demos/CrudApi.fs
module Demos.CrudApi

open Giraffe
open Saturn

let routes : HttpHandler =
    router {
        get    "/api/items"     getAll
        post   "/api/items"     create
        getf   "/api/items/%i"  getById
        putf   "/api/items/%i"  update
        deletef"/api/items/%i"  delete
    }
```

Each backend demo has:
1. A `routes` value wired into `Server.fs`
2. A matching entry in `ComponentRegistry` (backend section) with endpoint URL, request/response shapes
3. A "Try it" panel in `BackendDemoPage.fs` that calls the live API and shows the result

---

## Deliverables Per Request

**For a Web Component:**
1. `WebComponents/<Category>/FuiXxx.fs` — complete Fable.Lit implementation
2. CSS inside the `let private styles = css "..."` binding (shadow-scoped, `--fui-*` custom props)
3. `ComponentRegistry.fs` entry — full `ComponentMeta` record
4. HTML usage snippet for the "HTML Usage" tab
5. Notes on event bubble behaviour, slot usage, accessibility

**For a backend demo:**
1. `Server/Demos/XxxApi.fs` — Saturn/Giraffe handlers
2. `BackendDemoPage.fs` entry — "Try it" UI panel
3. `Shared/Shared.fs` additions — new route constants or shared DTOs

Be precise, idiomatic, and production-quality.

---

## Implementation Status

### Inputs & Forms

| Component | File | Status |
|-----------|------|--------|
| `fui-button` | `WebComponents/Inputs/FuiButton.fs` | ✅ Done |
| `fui-input` | `WebComponents/Inputs/FuiInput.fs` | ✅ Done |
| `fui-textarea` | `WebComponents/Inputs/FuiTextarea.fs` | ✅ Done |
| `fui-select` | `WebComponents/Inputs/FuiSelect.fs` | ✅ Done |
| `fui-checkbox` | `WebComponents/Inputs/FuiCheckbox.fs` | ⬜ Next |
| `fui-radio-group` + `fui-radio` | `WebComponents/Inputs/FuiRadioGroup.fs` | ⬜ |
| `fui-toggle` | `WebComponents/Inputs/FuiToggle.fs` | ⬜ |
| `fui-slider` | `WebComponents/Inputs/FuiSlider.fs` | ⬜ |
| `fui-date-picker` | `WebComponents/Inputs/FuiDatePicker.fs` | ⬜ |
| `fui-color-picker` | `WebComponents/Inputs/FuiColorPicker.fs` | ⬜ |
| `fui-file-upload` | `WebComponents/Inputs/FuiFileUpload.fs` | ⬜ |
| `fui-combobox` | `WebComponents/Inputs/FuiCombobox.fs` | ⬜ |

### Layout, Navigation, Feedback, Overlay, Data Display, Typography
All pending — see DESIGNER.md for full inventory.
