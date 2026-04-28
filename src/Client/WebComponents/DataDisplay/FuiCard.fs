module WebComponents.FuiCard

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-card-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .card {
        background    : var(--fui-card-bg, #161618);
        border        : 1px solid var(--fui-card-border, #2A2A2E);
        border-radius : var(--fui-card-radius, 10px);
        display       : flex;
        flex-direction: column;
        overflow      : hidden;
    }

    :host([variant="elevated"]) .card { box-shadow: 0 4px 24px rgba(0,0,0,0.35); }
    :host([variant="outline"])  .card { background: transparent; border-color: #3A3A3E; }
    :host([variant="ghost"])    .card { background: rgba(255,255,255,0.03); border: none; }

    /* ── Padding ── */
    .card-header, .card-body, .card-footer { padding: 1.25rem; }
    :host([padding="sm"]) .card-header,
    :host([padding="sm"]) .card-body,
    :host([padding="sm"]) .card-footer   { padding: 0.75rem; }
    :host([padding="lg"]) .card-header,
    :host([padding="lg"]) .card-body,
    :host([padding="lg"]) .card-footer   { padding: 1.75rem; }
    :host([padding="none"]) .card-header,
    :host([padding="none"]) .card-body,
    :host([padding="none"]) .card-footer { padding: 0; }

    .card-header {
        border-bottom: 1px solid var(--fui-card-divider, #2A2A2E);
        color        : var(--fui-card-header-color, #E8E8ED);
        font-family  : 'JetBrains Mono', monospace;
        font-size    : 0.9375rem;
        font-weight  : 600;
    }
    .card-header:not(:has(*)) { display: none; }

    .card-body {
        color      : var(--fui-card-color, #A0A0A8);
        flex       : 1;
        font-size  : 0.9375rem;
        line-height: 1.6;
    }

    .card-footer {
        border-top: 1px solid var(--fui-card-divider, #2A2A2E);
    }
    .card-footer:not(:has(*)) { display: none; }
"""

[<LitElement("fui-card")>]
let FuiCard () =
    let _host, _props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            variant = Prop.Of("default", attribute = "variant")
            padding = Prop.Of("md",      attribute = "padding")
        |}
    )

    html $"""
        <div class="card" part="card">
            <div class="card-header" part="header"><slot name="header"></slot></div>
            <div class="card-body"   part="body"  ><slot></slot></div>
            <div class="card-footer" part="footer"><slot name="footer"></slot></div>
        </div>
    """

let register () = ()
