module WebComponents.FuiDivider

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-divider-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    /* ── Horizontal (default) ──────────────────────────────────────────────── */
    .divider {
        display    : flex;
        align-items: center;
        gap        : 0.75rem;
        width      : 100%;
    }

    /* When there is no label the two pseudo-elements would have a gap between
       them. Setting gap:0 collapses them into one unbroken line. */
    .divider.no-label { gap: 0; }

    .divider::before,
    .divider::after {
        content   : "";
        flex      : 1 1 0;
        height    : var(--fui-divider-thickness, 1px);
        background: var(--fui-divider-color, #2A2A2E);
        min-width : 0;
    }

    .label {
        font-size     : 0.72rem;
        font-weight   : 600;
        letter-spacing: 0.06em;
        text-transform: uppercase;
        color         : var(--fui-divider-label-color, #6E6E76);
        white-space   : nowrap;
        flex-shrink   : 0;
    }

    /* ── Vertical ──────────────────────────────────────────────────────────── */
    :host([orientation="vertical"]) {
        display: inline-flex;
        height : 100%;
    }

    :host([orientation="vertical"]) .divider {
        flex-direction: column;
        width         : auto;
        height        : 100%;
        gap           : 0.75rem;
    }

    :host([orientation="vertical"]) .divider.no-label { gap: 0; }

    :host([orientation="vertical"]) .divider::before,
    :host([orientation="vertical"]) .divider::after {
        width    : var(--fui-divider-thickness, 1px);
        height   : auto;
        flex     : 1 1 0;
        min-width: 0;
    }
"""

[<LitElement("fui-divider")>]
let FuiDivider() =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            label       = Prop.Of("",           attribute = "label")
            orientation = Prop.Of("horizontal", attribute = "orientation")
        |}
    )

    let labelPart =
        if props.label.Value <> "" then
            html $"""<span class="label" part="label">{props.label.Value}</span>"""
        else
            Lit.nothing

    let divClass = if props.label.Value = "" then "divider no-label" else "divider"

    html $"""
        <div class={divClass} part="divider" role="separator" aria-orientation={props.orientation.Value}>
            {labelPart}
        </div>
    """

let register () = ()
