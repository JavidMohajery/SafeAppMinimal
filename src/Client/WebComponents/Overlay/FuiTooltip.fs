module WebComponents.FuiTooltip

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : inline-block;
        font-family: var(--fui-tooltip-font, 'JetBrains Mono', monospace);
    }

    .wrap {
        display : inline-block;
        position: relative;
    }

    .tip {
        background    : var(--fui-tooltip-bg, #E8E8ED);
        border-radius : var(--fui-tooltip-radius, 5px);
        color         : var(--fui-tooltip-color, #0D0D0F);
        font-size     : 0.75rem;
        font-weight   : 500;
        left          : 50%;
        max-width     : 220px;
        opacity       : 0;
        padding       : 0.35rem 0.625rem;
        pointer-events: none;
        position      : absolute;
        transform     : translateX(-50%);
        transition    : opacity 0.12s;
        white-space   : nowrap;
        z-index       : 9999;
    }
    .tip.visible { opacity: 1; }

    /* ── Placements ─────────────────────────────────────────────────────────── */
    .tip.top    { bottom: calc(100% + 6px); }
    .tip.bottom { bottom: auto; top: calc(100% + 6px); }
    .tip.left   {
        bottom   : auto;
        left     : auto;
        right    : calc(100% + 6px);
        top      : 50%;
        transform: translateY(-50%);
    }
    .tip.right  {
        bottom   : auto;
        left     : calc(100% + 6px);
        right    : auto;
        top      : 50%;
        transform: translateY(-50%);
    }

    @media (prefers-reduced-motion: reduce) {
        .tip { transition: none; }
    }
"""

[<LitElement("fui-tooltip")>]
let FuiTooltip () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            content   = Prop.Of("", attribute = "content")
            placement = Prop.Of("top", attribute = "placement")
        |}
    )

    let isVisible, setIsVisible = Hook.useState false

    let show _ = setIsVisible true
    let hide _ = setIsVisible false

    let tipCls = if isVisible then $"tip {props.placement.Value} visible" else $"tip {props.placement.Value}"

    html $"""
        <div class="wrap"
             @mouseenter={Ev show}
             @mouseleave={Ev hide}
             @focusin={Ev show}
             @focusout={Ev hide}>
            <slot></slot>
            <div class={tipCls} role="tooltip">{props.content.Value}</div>
        </div>
    """

let register () = ()
