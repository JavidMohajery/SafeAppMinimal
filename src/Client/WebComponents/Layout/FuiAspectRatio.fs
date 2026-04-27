module WebComponents.FuiAspectRatio

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display: block;
        width  : 100%;
    }
    :host([hidden]) { display: none; }

    .aspect-wrap {
        position  : relative;
        width     : 100%;
        overflow  : hidden;
        background: var(--fui-ar-bg, transparent);
    }

    .aspect-inner {
        position: absolute;
        inset   : 0;
        width   : 100%;
        height  : 100%;
    }
"""

[<LitElement("fui-aspect-ratio")>]
let FuiAspectRatio() =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            ratio = Prop.Of("16/9", attribute = "ratio")
        |}
    )

    let wrapStyle = $"aspect-ratio:{props.ratio.Value}"

    html $"""
        <div class="aspect-wrap" part="aspect-wrap" style={wrapStyle}>
            <div class="aspect-inner" part="aspect-inner">
                <slot></slot>
            </div>
        </div>
    """

let register () = ()
