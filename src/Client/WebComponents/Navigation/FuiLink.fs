module WebComponents.FuiLink

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : inline;
        font-family: var(--fui-link-font, inherit);
    }
    :host([hidden]) { display: none; }

    a {
        color                    : var(--fui-link-color, #9B59F5);
        cursor                   : pointer;
        text-decoration          : underline;
        text-decoration-color    : rgba(155,89,245,0.35);
        text-decoration-thickness: 1px;
        text-underline-offset    : 2px;
        transition               : color 0.15s, text-decoration-color 0.15s;
    }
    a:hover {
        color                : var(--fui-link-hover, #7C3AED);
        text-decoration-color: currentColor;
    }
    a:focus-visible {
        border-radius : 2px;
        outline       : 2px solid var(--fui-link-color, #9B59F5);
        outline-offset: 2px;
    }

    :host([variant="muted"]) a {
        color                : #6E6E76;
        text-decoration-color: rgba(110,110,118,0.35);
    }
    :host([variant="muted"]) a:hover { color: #E8E8ED; text-decoration-color: currentColor; }

    :host([variant="subtle"]) a {
        color                : #E8E8ED;
        text-decoration-color: rgba(255,255,255,0.18);
    }
    :host([variant="subtle"]) a:hover { text-decoration-color: rgba(255,255,255,0.6); }

    .ext { display: none; }
    :host([external]) .ext {
        display      : inline;
        font-size    : 0.7em;
        margin-left  : 0.15em;
        opacity      : 0.6;
        vertical-align: super;
    }
"""

[<LitElement("fui-link")>]
let FuiLink () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            href       = Prop.Of("",    attribute = "href")
            target     = Prop.Of("",    attribute = "target")
            rel        = Prop.Of("",    attribute = "rel")
            variant    = Prop.Of("",    attribute = "variant")
            isExternal = Prop.Of(false, attribute = "external")
        |}
    )

    let target = if props.isExternal.Value then "_blank" else props.target.Value
    let rel    = if props.isExternal.Value then "noopener noreferrer" else props.rel.Value

    html $"""
        <a href="{props.href.Value}" target="{target}" rel="{rel}" part="link">
            <slot></slot>
            <span class="ext" aria-label="(opens in new tab)">↗</span>
        </a>
    """

let register () = ()
