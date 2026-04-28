module WebComponents.FuiText

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-text-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .text {
        color      : var(--fui-text-color, #E8E8ED);
        font-family: inherit;
        font-size  : var(--fui-text-size, 0.9375rem);
        font-weight: 400;
        line-height: 1.6;
        margin     : 0;
    }

    :host([size="xs"])  .text { font-size: 0.75rem;   }
    :host([size="sm"])  .text { font-size: 0.875rem;  }
    :host([size="md"])  .text { font-size: 0.9375rem; }
    :host([size="lg"])  .text { font-size: 1.125rem;  }
    :host([size="xl"])  .text { font-size: 1.25rem;   }

    :host([weight="light"])  .text { font-weight: 300; }
    :host([weight="normal"]) .text { font-weight: 400; }
    :host([weight="medium"]) .text { font-weight: 500; }
    :host([weight="semi"])   .text { font-weight: 600; }
    :host([weight="bold"])   .text { font-weight: 700; }

    :host([color="muted"])   .text { color: #6E6E76; }
    :host([color="accent"])  .text { color: #7C3AED; }
    :host([color="success"]) .text { color: #22C55E; }
    :host([color="warning"]) .text { color: #F59E0B; }
    :host([color="danger"])  .text { color: #EF4444; }
    :host([color="info"])    .text { color: #3B82F6; }

    :host([align="left"])   .text { text-align: left;   }
    :host([align="center"]) .text { text-align: center; }
    :host([align="right"])  .text { text-align: right;  }

    :host([truncate]) .text {
        overflow     : hidden;
        text-overflow: ellipsis;
        white-space  : nowrap;
    }
"""

[<LitElement("fui-text")>]
let FuiText () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            size     = Prop.Of("md",     attribute = "size")
            weight   = Prop.Of("normal", attribute = "weight")
            color    = Prop.Of("",       attribute = "color")
            align    = Prop.Of("",       attribute = "align")
            truncate = Prop.Of(false,    attribute = "truncate")
        |}
    )

    html $"""<p class="text" part="text"><slot></slot></p>"""

let register () = ()
