module WebComponents.FuiPopover

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : inline-block;
        font-family: var(--fui-popover-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .wrap { position: relative; }

    .trigger {
        align-items  : center;
        background   : var(--fui-popover-trigger-bg, #1E1E21);
        border       : 1px solid var(--fui-popover-trigger-border, #2A2A2E);
        border-radius: 6px;
        color        : var(--fui-popover-trigger-color, #E8E8ED);
        cursor       : pointer;
        display      : inline-flex;
        font-family  : inherit;
        font-size    : 0.875rem;
        font-weight  : 500;
        gap          : 0.375rem;
        padding      : 0.5rem 0.875rem;
        transition   : border-color 0.15s;
    }
    .trigger:hover { border-color: #7C3AED; }
    .trigger:focus-visible {
        outline       : 2px solid #7C3AED;
        outline-offset: 2px;
    }

    /* Invisible click-away overlay (same pattern as fui-menu) */
    .overlay {
        display : none;
        inset   : 0;
        position: fixed;
        z-index : 9998;
    }
    .overlay.open { display: block; }

    .panel {
        background   : var(--fui-popover-bg, #1E1E21);
        border       : 1px solid var(--fui-popover-border, #2A2A2E);
        border-radius: var(--fui-popover-radius, 8px);
        box-shadow   : 0 8px 24px rgba(0,0,0,0.4);
        display      : none;
        min-width    : 200px;
        padding      : 1rem;
        position     : absolute;
        z-index      : 9999;
    }
    .panel.open { display: block; }

    /* ── Placements ─────────────────────────────────────────────────────────── */
    .wrap.placement-bottom-start .panel { left:0;   top: calc(100% + 6px); }
    .wrap.placement-bottom-end   .panel { right:0;  top: calc(100% + 6px); }
    .wrap.placement-top-start    .panel { bottom: calc(100% + 6px); left:0; }
    .wrap.placement-top-end      .panel { bottom: calc(100% + 6px); right:0; }
"""

[<LitElement("fui-popover")>]
let FuiPopover () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            triggerLabel = Prop.Of("Options",      attribute = "trigger-label")
            placement    = Prop.Of("bottom-start", attribute = "placement")
            width        = Prop.Of("",             attribute = "width")
        |}
    )

    let isOpen, setIsOpen = Hook.useState false

    let toggle _ = setIsOpen (not isOpen)
    let close  _ = setIsOpen false

    let wrapCls    = $"wrap placement-{props.placement.Value}"
    let overlayCls = if isOpen then "overlay open" else "overlay"
    let panelCls   = if isOpen then "panel open"   else "panel"

    let widthStyle =
        if props.width.Value <> "" then $"width:{props.width.Value}" else ""

    html $"""
        <div class={wrapCls}>
            <div class={overlayCls} @click={Ev close}></div>
            <button class="trigger"
                    aria-haspopup="true"
                    aria-expanded={string isOpen}
                    @click={Ev toggle}>
                {props.triggerLabel.Value}
                <span aria-hidden="true">▾</span>
            </button>
            <div class={panelCls} style={widthStyle} role="dialog">
                <slot></slot>
            </div>
        </div>
    """

let register () = ()
