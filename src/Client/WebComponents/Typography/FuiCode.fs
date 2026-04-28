module WebComponents.FuiCode

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : inline;
        font-family: var(--fui-code-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    code {
        background   : var(--fui-code-bg,     rgba(124,58,237,0.10));
        border       : 1px solid var(--fui-code-border, rgba(124,58,237,0.20));
        border-radius: 4px;
        color        : var(--fui-code-color,  #9B59F5);
        font-family  : inherit;
        font-size    : 0.85em;
        font-weight  : 400;
        padding      : 0.1em 0.4em;
    }
"""

[<LitElement("fui-code")>]
let FuiCode () =
    let _host, _props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {| |}
    )

    html $"""<code part="code"><slot></slot></code>"""

let register () = ()
