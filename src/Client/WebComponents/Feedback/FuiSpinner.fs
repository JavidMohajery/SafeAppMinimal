module WebComponents.FuiSpinner

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : inline-flex;
        align-items: center;
        gap        : 0.5rem;
        font-family: var(--fui-spinner-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    @keyframes fui-spin {
        to { transform: rotate(360deg); }
    }

    .ring {
        animation       : fui-spin 0.7s linear infinite;
        border          : 2px solid var(--fui-spinner-track, rgba(124,58,237,0.2));
        border-radius   : 50%;
        border-top-color: var(--fui-spinner-color, #7C3AED);
        box-sizing      : border-box;
        display         : block;
        flex-shrink     : 0;
        height          : 20px;
        width           : 20px;
    }

    /* ── Sizes ──────────────────────────────────────────────────────────────── */
    :host([size="sm"]) .ring { height: 14px; width: 14px; border-width: 2px; }
    :host([size="md"]) .ring { height: 20px; width: 20px; border-width: 2px; }
    :host([size="lg"]) .ring { height: 28px; width: 28px; border-width: 3px; }
    :host([size="xl"]) .ring { height: 40px; width: 40px; border-width: 3px; }

    .label {
        color    : var(--fui-spinner-label-color, #6E6E76);
        font-size: 0.8125rem;
    }

    @media (prefers-reduced-motion: reduce) {
        .ring { animation-duration: 1.5s; }
    }
"""

[<LitElement("fui-spinner")>]
let FuiSpinner () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            size  = Prop.Of("md", attribute = "size")
            label = Prop.Of("",   attribute = "label")
        |}
    )

    let ariaLabel = if props.label.Value <> "" then props.label.Value else "Loading"

    let labelPart =
        if props.label.Value <> "" then
            html $"""<span class="label">{props.label.Value}</span>"""
        else Lit.nothing

    html $"""
        <span class="ring" role="status" aria-label={ariaLabel}></span>
        {labelPart}
    """

let register () = ()
