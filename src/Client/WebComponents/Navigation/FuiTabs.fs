module WebComponents.FuiTabs

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("JSON.parse($0)")>]
let private parseJSON (_: string) : {| value: string; label: string |} array = jsNative

// ── fui-tab ───────────────────────────────────────────────────────────────────

let private tabStyles = unsafeCSS """
    :host {
        display: block;
    }
"""

[<LitElement("fui-tab")>]
let FuiTab () =
    let _host, _props = LitElement.init(fun init ->
        init.styles <- [ tabStyles ]
    )
    html $"""<slot></slot>"""

// ── fui-tabs ──────────────────────────────────────────────────────────────────

let private styles = unsafeCSS """
    :host {
        display: block;
        font-family: var(--fui-tabs-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    .tabs-wrap {
        display: flex;
        flex-direction: column;
    }

    .tab-bar {
        display        : flex;
        border-bottom  : 1px solid var(--fui-tabs-border, #2A2A2E);
        gap            : 0;
        overflow-x     : auto;
        scrollbar-width: none;
    }
    .tab-bar::-webkit-scrollbar { display: none; }

    .tab-btn {
        background   : none;
        border       : none;
        border-bottom: 2px solid transparent;
        color        : var(--fui-tabs-color, #6E6E76);
        cursor       : pointer;
        font-family  : inherit;
        font-size    : 0.8125rem;
        font-weight  : 600;
        letter-spacing: 0.02em;
        margin-bottom: -1px;
        padding      : 0.625rem 1rem;
        transition   : color 0.15s, border-color 0.15s;
        white-space  : nowrap;
    }
    .tab-btn:hover {
        color: var(--fui-tabs-color-hover, #E8E8ED);
    }
    .tab-btn.active {
        border-bottom-color: var(--fui-tabs-accent, #7C3AED);
        color              : var(--fui-tabs-active-color, #E8E8ED);
    }
    .tab-btn:focus-visible {
        outline       : 2px solid var(--fui-tabs-accent, #7C3AED);
        outline-offset: -2px;
        border-radius : 4px 4px 0 0;
    }

    .tab-content {
        padding: var(--fui-tabs-panel-padding, 1.25rem 0 0 0);
    }

    .panel         { display: none; }
    .panel.active  { display: block; }
"""

[<LitElement("fui-tabs")>]
let FuiTabs () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            tabs   = Prop.Of("[]", attribute = "tabs")
            active = Prop.Of("",   attribute = "active")
        |}
    )

    let opts = parseJSON props.tabs.Value
    let firstVal = if opts.Length > 0 then opts.[0].value else ""
    let initVal  = if props.active.Value <> "" then props.active.Value else firstVal

    let activeTab, setActiveTab = Hook.useState initVal

    let setTab (v: string) =
        setActiveTab v
        host.dispatchCustomEvent("fui-change", detail = {| value = v |})

    let renderBtn (t: {| value: string; label: string |}) =
        let cls = if t.value = activeTab then "tab-btn active" else "tab-btn"
        html $"""
            <button
                class={cls}
                role="tab"
                aria-selected={string (t.value = activeTab)}
                @click={Ev (fun _ -> setTab t.value)}>
                {t.label}
            </button>"""

    let renderPanel (t: {| value: string; label: string |}) =
        let cls = if t.value = activeTab then "panel active" else "panel"
        html $"""<div class={cls} role="tabpanel"><slot name={$"tab-{t.value}"}></slot></div>"""

    let btns   = Lit.mapUnique (fun (t: {| value: string; label: string |}) -> t.value) renderBtn   opts
    let panels = Lit.mapUnique (fun (t: {| value: string; label: string |}) -> t.value) renderPanel opts

    html $"""
        <div class="tabs-wrap">
            <div class="tab-bar" role="tablist">{btns}</div>
            <div class="tab-content">{panels}</div>
        </div>
    """

let register () = ()
