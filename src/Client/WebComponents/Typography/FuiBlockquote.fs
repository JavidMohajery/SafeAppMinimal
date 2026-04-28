module WebComponents.FuiBlockquote

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-bq-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    blockquote {
        border-left: 3px solid var(--fui-bq-border, #7C3AED);
        color      : var(--fui-bq-color,  #A0A0A8);
        font-family: inherit;
        font-size  : var(--fui-bq-size,   0.9375rem);
        font-style : italic;
        line-height: 1.65;
        margin     : 0;
        padding    : 0.75rem 1.25rem;
    }

    .cite {
        color      : #6E6E76;
        display    : block;
        font-size  : 0.8rem;
        font-style : normal;
        font-weight: 500;
        margin-top : 0.5rem;
    }
    .cite:empty { display: none; }
"""

[<LitElement("fui-blockquote")>]
let FuiBlockquote () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            cite = Prop.Of("", attribute = "cite")
        |}
    )

    html $"""
        <blockquote part="blockquote">
            <slot></slot>
            <span class="cite">{props.cite.Value}</span>
        </blockquote>
    """

let register () = ()
