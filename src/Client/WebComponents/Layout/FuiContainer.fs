module WebComponents.FuiContainer

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display   : block;
        width     : 100%;
        box-sizing: border-box;
    }
    :host([hidden]) { display: none; }

    .container {
        width     : 100%;
        max-width : var(--fui-container-max-width, 1100px);
        margin    : 0 auto;
        padding   : 0 var(--fui-container-px, 2.5rem);
        box-sizing: border-box;
    }
"""

[<LitElement("fui-container")>]
let FuiContainer() =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            maxWidth = Prop.Of("", attribute = "max-width")
            padding  = Prop.Of("", attribute = "padding")
        |}
    )

    let parts =
        [ if props.maxWidth.Value <> "" then yield $"max-width:{props.maxWidth.Value}"
          if props.padding.Value  <> "" then yield $"padding:0 {props.padding.Value}" ]

    let containerStyle = parts |> String.concat ";"

    html $"""
        <div class="container" part="container" style={containerStyle}>
            <slot></slot>
        </div>
    """

let register () = ()
