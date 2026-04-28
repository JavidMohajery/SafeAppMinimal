module WebComponents.FuiTag

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : inline-block;
        font-family: var(--fui-tag-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .tag {
        align-items  : center;
        background   : var(--fui-tag-bg, #1E1E21);
        border       : 1px solid var(--fui-tag-border, #2A2A2E);
        border-radius: var(--fui-tag-radius, 4px);
        color        : var(--fui-tag-color, #A0A0A8);
        display      : inline-flex;
        font-size    : 0.8125rem;
        font-weight  : 500;
        gap          : 0.375rem;
        padding      : 0.25rem 0.625rem;
    }

    /* ── Sizes ── */
    :host([size="sm"]) .tag { font-size: 0.6875rem; padding: 0.125rem 0.5rem; }
    :host([size="lg"]) .tag { font-size: 0.9375rem; padding: 0.375rem 0.875rem; }

    /* ── Variants ── */
    :host([variant="success"]) .tag { background:rgba(34,197,94,0.10);  border-color:rgba(34,197,94,0.28);  color:#22C55E; }
    :host([variant="warning"]) .tag { background:rgba(245,158,11,0.10); border-color:rgba(245,158,11,0.28); color:#F59E0B; }
    :host([variant="danger"])  .tag { background:rgba(239,68,68,0.10);  border-color:rgba(239,68,68,0.28);  color:#EF4444; }
    :host([variant="info"])    .tag { background:rgba(59,130,246,0.10); border-color:rgba(59,130,246,0.28); color:#3B82F6; }
    :host([variant="accent"])  .tag { background:rgba(124,58,237,0.10); border-color:rgba(124,58,237,0.28); color:#7C3AED; }

    .remove {
        background : none;
        border     : none;
        border-radius: 2px;
        color      : inherit;
        cursor     : pointer;
        font-size  : 0.65rem;
        line-height: 1;
        opacity    : 0.65;
        padding    : 0;
        transition : opacity 0.15s;
    }
    .remove:hover { opacity: 1; }
    .remove:focus-visible {
        outline       : 2px solid #7C3AED;
        outline-offset: 1px;
    }
"""

[<LitElement("fui-tag")>]
let FuiTag () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            variant   = Prop.Of("neutral", attribute = "variant")
            size      = Prop.Of("md",      attribute = "size")
            removable = Prop.Of(false,     attribute = "removable")
        |}
    )

    let remove _ =
        host.dispatchCustomEvent("fui-remove", detail = {| |})

    let removeBtn =
        if props.removable.Value then
            html $"""<button class="remove" aria-label="Remove" @click={Ev remove}>✕</button>"""
        else Lit.nothing

    html $"""
        <span class="tag" part="tag">
            <slot></slot>
            {removeBtn}
        </span>
    """

let register () = ()
