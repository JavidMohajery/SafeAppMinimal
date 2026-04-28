module WebComponents.FuiSideNav

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("JSON.parse($0)")>]
let private parseGroups (_: string) : {| label: string; links: {| label: string; href: string; active: bool |} array |} array = jsNative

let private styles = unsafeCSS """
    :host {
        background : var(--fui-snav-bg,     #161618);
        border-right: 1px solid var(--fui-snav-border, #2A2A2E);
        display    : block;
        font-family: var(--fui-snav-font,   'JetBrains Mono', monospace);
        height     : 100%;
        overflow-y : auto;
        width      : var(--fui-snav-width,  256px);
    }
    :host([hidden]) { display: none; }

    .logo {
        align-items  : center;
        border-bottom: 1px solid var(--fui-snav-border, #2A2A2E);
        color        : var(--fui-snav-logo-color, #E8E8ED);
        display      : flex;
        font-size    : 1rem;
        font-weight  : 700;
        gap          : 0.5rem;
        padding      : 1rem 1.25rem;
    }
    .logo-dot {
        background   : #7C3AED;
        border-radius: 4px;
        height       : 20px;
        width        : 20px;
    }

    .nav { padding: 0.75rem 0; }

    /* ── Group ──────────────────────────────────────────────────────────────── */
    .group { margin-bottom: 0.25rem; }

    .group-trigger {
        align-items : center;
        background  : none;
        border      : none;
        color       : #6E6E76;
        cursor      : pointer;
        display     : flex;
        font-family : inherit;
        font-size   : 0.65rem;
        font-weight : 700;
        gap         : 0.4rem;
        justify-content: space-between;
        letter-spacing : 0.08em;
        padding     : 0.4rem 1.25rem;
        text-align  : left;
        text-transform: uppercase;
        transition  : color 0.15s;
        width       : 100%;
    }
    .group-trigger:hover { color: #A0A0A8; }

    .chevron {
        font-size : 0.55rem;
        opacity   : 0.7;
        transition: transform 0.2s;
    }
    .chevron.open { transform: rotate(90deg); }

    .links {
        max-height: 0;
        overflow  : hidden;
        transition: max-height 0.2s ease;
    }
    .links.open { max-height: 600px; }

    /* ── Nav link ───────────────────────────────────────────────────────────── */
    a.nav-link {
        border-left    : 2px solid transparent;
        color          : var(--fui-snav-link-color, #A0A0A8);
        cursor         : pointer;
        display        : block;
        font-size      : 0.875rem;
        padding        : 0.4rem 1.25rem 0.4rem 1.5rem;
        text-decoration: none;
        transition     : color 0.15s, background 0.15s, border-color 0.15s;
    }
    a.nav-link:hover {
        background  : rgba(124,58,237,0.06);
        border-color: #4A4A52;
        color       : #E8E8ED;
    }
    a.nav-link.active {
        background  : rgba(124,58,237,0.10);
        border-color: #7C3AED;
        color       : #9B59F5;
        font-weight : 600;
    }
    a.nav-link:focus-visible {
        outline       : 2px solid #7C3AED;
        outline-offset: -2px;
    }
"""

[<LitElement("fui-sidenav")>]
let FuiSideNav () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            logo   = Prop.Of("Navigation", attribute = "logo")
            groups = Prop.Of("[]",         attribute = "groups")
        |}
    )

    let groups = parseGroups props.groups.Value

    // All groups start expanded; track collapsed ones
    let collapsed, setCollapsed = Hook.useState [| |]

    let toggleGroup (label: string) _ =
        if collapsed |> Array.contains label then
            setCollapsed (collapsed |> Array.filter (fun l -> l <> label))
        else
            setCollapsed (Array.append collapsed [| label |])

    let renderGroup (g: {| label: string; links: {| label: string; href: string; active: bool |} array |}) =
        let isOpen     = not (collapsed |> Array.contains g.label)
        let chevronCls = if isOpen then "chevron open" else "chevron"
        let linksCls   = if isOpen then "links open"   else "links"
        let links =
            g.links |> Array.map (fun l ->
                let cls = if l.active then "nav-link active" else "nav-link"
                html $"""<a class={cls} href="{l.href}" part="link">{l.label}</a>""")
        html $"""
            <div class="group">
                <button class="group-trigger" @click={Ev (toggleGroup g.label)} aria-expanded={string isOpen}>
                    {g.label}
                    <span class={chevronCls} aria-hidden="true">▶</span>
                </button>
                <div class={linksCls} role="list">
                    {links}
                </div>
            </div>"""

    let renderedGroups = groups |> Array.map renderGroup

    html $"""
        <div class="logo" part="logo">
            <span class="logo-dot" aria-hidden="true"></span>
            {props.logo.Value}
        </div>
        <nav class="nav" aria-label="{props.logo.Value} navigation">
            {renderedGroups}
        </nav>
    """

let register () = ()
