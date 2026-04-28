module WebComponents.FuiHeading

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-heading-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    .heading {
        color         : var(--fui-heading-color, #E8E8ED);
        font-family   : inherit;
        font-weight   : 700;
        letter-spacing: -0.01em;
        line-height   : 1.25;
        margin        : 0;
    }

    h1.heading { font-size: var(--fui-h1, 2.25rem); }
    h2.heading { font-size: var(--fui-h2, 1.5rem);   }
    h3.heading { font-size: var(--fui-h3, 1.25rem);  }
    h4.heading { font-size: var(--fui-h4, 1.1rem);   font-weight: 600; }
    h5.heading { font-size: var(--fui-h5, 0.9375rem); font-weight: 600; }
    h6.heading { font-size: var(--fui-h6, 0.875rem);  font-weight: 600; color: var(--fui-heading-muted, #A0A0A8); }

    :host([color="muted"])  .heading { color: #6E6E76; }
    :host([color="accent"]) .heading { color: #7C3AED; }

    :host([truncate]) .heading {
        overflow     : hidden;
        text-overflow: ellipsis;
        white-space  : nowrap;
    }
"""

[<LitElement("fui-heading")>]
let FuiHeading () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            level    = Prop.Of("2",      attribute = "level")
            color    = Prop.Of("",       attribute = "color")
            truncate = Prop.Of(false,    attribute = "truncate")
        |}
    )

    match props.level.Value with
    | "1" -> html $"""<h1 class="heading" part="heading"><slot></slot></h1>"""
    | "3" -> html $"""<h3 class="heading" part="heading"><slot></slot></h3>"""
    | "4" -> html $"""<h4 class="heading" part="heading"><slot></slot></h4>"""
    | "5" -> html $"""<h5 class="heading" part="heading"><slot></slot></h5>"""
    | "6" -> html $"""<h6 class="heading" part="heading"><slot></slot></h6>"""
    | _   -> html $"""<h2 class="heading" part="heading"><slot></slot></h2>"""

let register () = ()
