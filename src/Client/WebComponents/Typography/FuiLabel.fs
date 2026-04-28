module WebComponents.FuiLabel

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : inline-block;
        font-family: var(--fui-label-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    label {
        color         : var(--fui-label-color, #A0A0A8);
        cursor        : default;
        display       : block;
        font-family   : inherit;
        font-size     : 0.75rem;
        font-weight   : 600;
        letter-spacing: 0.04em;
        line-height   : 1.4;
        text-transform: uppercase;
    }

    :host([size="sm"]) label { font-size: 0.65rem; }
    :host([size="lg"]) label { font-size: 0.875rem; }

    :host([required]) .required {
        color      : #EF4444;
        margin-left: 0.2em;
    }
    .required { display: none; }
    :host([required]) .required { display: inline; }
"""

[<LitElement("fui-label")>]
let FuiLabel () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            for'     = Prop.Of("", attribute = "for")
            size     = Prop.Of("md", attribute = "size")
            required = Prop.Of(false, attribute = "required")
        |}
    )

    html $"""
        <label part="label" for="{props.for'.Value}">
            <slot></slot>
            <span class="required" aria-hidden="true">*</span>
        </label>
    """

let register () = ()
