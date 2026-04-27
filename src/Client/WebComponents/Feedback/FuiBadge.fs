module WebComponents.FuiBadge

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : inline-flex;
        font-family: var(--fui-badge-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    .badge {
        align-items   : center;
        background    : var(--fui-badge-bg,     rgba(110,110,118,0.15));
        border        : 1px solid var(--fui-badge-border, rgba(110,110,118,0.3));
        border-radius : 99px;
        color         : var(--fui-badge-color,  #6E6E76);
        display       : inline-flex;
        font-size     : 0.7rem;
        font-weight   : 600;
        gap           : 0.35rem;
        letter-spacing: 0.04em;
        line-height   : 1;
        padding       : 0.25rem 0.625rem;
        text-transform: uppercase;
        white-space   : nowrap;
    }

    /* ── Sizes ──────────────────────────────────────────────────────────────── */
    :host([size="sm"]) .badge { font-size: 0.6rem;  padding: 0.175rem 0.45rem; }
    :host([size="lg"]) .badge { font-size: 0.8rem;  padding: 0.3rem  0.75rem; }

    /* ── Variants ───────────────────────────────────────────────────────────── */
    :host([variant="success"]) .badge {
        background  : rgba(34,197,94,0.12);
        border-color: rgba(34,197,94,0.3);
        color       : #22C55E;
    }
    :host([variant="warning"]) .badge {
        background  : rgba(245,158,11,0.12);
        border-color: rgba(245,158,11,0.3);
        color       : #F59E0B;
    }
    :host([variant="danger"]) .badge {
        background  : rgba(239,68,68,0.12);
        border-color: rgba(239,68,68,0.3);
        color       : #EF4444;
    }
    :host([variant="info"]) .badge {
        background  : rgba(59,130,246,0.12);
        border-color: rgba(59,130,246,0.3);
        color       : #3B82F6;
    }
    :host([variant="accent"]) .badge {
        background  : rgba(124,58,237,0.15);
        border-color: rgba(124,58,237,0.35);
        color       : #9B59F5;
    }

    /* ── Dot ────────────────────────────────────────────────────────────────── */
    .dot {
        background   : currentColor;
        border-radius: 50%;
        display      : inline-block;
        flex-shrink  : 0;
        height       : 6px;
        width        : 6px;
    }
"""

[<LitElement("fui-badge")>]
let FuiBadge () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            variant = Prop.Of("neutral", attribute = "variant")
            size    = Prop.Of("md",      attribute = "size")
            dot     = Prop.Of(false,     attribute = "dot")
        |}
    )

    let dotPart =
        if props.dot.Value then html $"""<span class="dot" aria-hidden="true"></span>"""
        else Lit.nothing

    html $"""
        <span class="badge" part="badge">
            {dotPart}
            <slot></slot>
        </span>
    """

let register () = ()
