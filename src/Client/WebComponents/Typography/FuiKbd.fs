module WebComponents.FuiKbd

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : inline;
        font-family: var(--fui-kbd-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    kbd {
        background    : var(--fui-kbd-bg,     #1E1E21);
        border        : 1px solid var(--fui-kbd-border, #2A2A2E);
        border-bottom : 3px solid var(--fui-kbd-border, #2A2A2E);
        border-radius : 4px;
        color         : var(--fui-kbd-color,  #E8E8ED);
        display       : inline-block;
        font-family   : inherit;
        font-size     : 0.8em;
        font-weight   : 500;
        letter-spacing: 0.02em;
        line-height   : 1;
        padding       : 0.2em 0.5em 0.25em;
        vertical-align: baseline;
    }
"""

[<LitElement("fui-kbd")>]
let FuiKbd () =
    let _host, _props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {| |}
    )

    html $"""<kbd part="kbd"><slot></slot></kbd>"""

let register () = ()
