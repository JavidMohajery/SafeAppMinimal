module WebComponents.FuiCallout

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-callout-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .callout {
        border-left  : 3px solid #3A3A3E;
        border-radius: var(--fui-callout-radius, 6px);
        display      : flex;
        flex-direction: column;
        gap          : 0.375rem;
        padding      : 0.875rem 1.125rem;
    }

    :host([variant="info"]) .callout {
        background       : rgba(59,130,246,0.07);
        border-left-color: #3B82F6;
    }
    :host([variant="success"]) .callout {
        background       : rgba(34,197,94,0.07);
        border-left-color: #22C55E;
    }
    :host([variant="warning"]) .callout {
        background       : rgba(245,158,11,0.07);
        border-left-color: #F59E0B;
    }
    :host([variant="danger"]) .callout {
        background       : rgba(239,68,68,0.07);
        border-left-color: #EF4444;
    }
    :host([variant="note"]) .callout {
        background       : rgba(124,58,237,0.07);
        border-left-color: #7C3AED;
    }

    .callout-title {
        color      : var(--fui-callout-title-color, #E8E8ED);
        font-family: 'JetBrains Mono', monospace;
        font-size  : 0.875rem;
        font-weight: 600;
        margin     : 0;
    }

    .callout-body {
        color      : var(--fui-callout-color, #A0A0A8);
        font-size  : 0.9rem;
        line-height: 1.6;
    }
"""

let private iconFor = function
    | "info"    -> "ℹ "
    | "success" -> "✓ "
    | "warning" -> "⚠ "
    | "danger"  -> "✕ "
    | "note"    -> "✦ "
    | _         -> ""

[<LitElement("fui-callout")>]
let FuiCallout () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            variant = Prop.Of("info", attribute = "variant")
            title   = Prop.Of("",    attribute = "title")
        |}
    )

    let titlePart =
        if props.title.Value <> "" then
            let icon = iconFor props.variant.Value
            html $"""<p class="callout-title">{icon}{props.title.Value}</p>"""
        else Lit.nothing

    html $"""
        <div class="callout" part="callout" role="note">
            {titlePart}
            <div class="callout-body"><slot></slot></div>
        </div>
    """

let register () = ()
