module WebComponents.FuiSpacer

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display: block;
    }
    :host([hidden]) { display: none; }

    .spacer {
        flex      : 1 1 auto;
        min-width : 0;
        min-height: 0;
    }
"""

[<LitElement("fui-spacer")>]
let FuiSpacer() =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            size = Prop.Of("", attribute = "size")
        |}
    )

    let spacerStyle =
        if props.size.Value <> "" then
            $"flex:0 0 {props.size.Value};width:{props.size.Value};height:{props.size.Value}"
        else ""

    html $"""<div class="spacer" part="spacer" style={spacerStyle}></div>"""

let register () = ()
