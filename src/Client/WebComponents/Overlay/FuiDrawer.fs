module WebComponents.FuiDrawer

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : inline-block;
        font-family: var(--fui-drawer-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .trigger {
        background   : var(--fui-drawer-trigger-bg, #1E1E21);
        border       : 1px solid var(--fui-drawer-trigger-border, #2A2A2E);
        border-radius: 6px;
        color        : var(--fui-drawer-trigger-color, #E8E8ED);
        cursor       : pointer;
        font-family  : inherit;
        font-size    : 0.875rem;
        font-weight  : 500;
        padding      : 0.5rem 1rem;
        transition   : border-color 0.15s;
    }
    .trigger:hover { border-color: #7C3AED; }
    .trigger:focus-visible {
        outline       : 2px solid #7C3AED;
        outline-offset: 2px;
    }

    .backdrop {
        background: rgba(0,0,0,0.6);
        display   : none;
        inset     : 0;
        position  : fixed;
        z-index   : 999;
    }
    .backdrop.open { display: block; }

    /* Panel base — placement classes supply position + initial transform */
    .panel {
        background    : var(--fui-drawer-bg, #161618);
        border        : 1px solid var(--fui-drawer-border, #2A2A2E);
        display       : flex;
        flex-direction: column;
        overflow      : hidden;
        position      : fixed;
        transition    : transform 0.25s ease;
        z-index       : 1000;
    }

    .panel.right  { bottom:0; right:0; top:0; width:var(--fui-drawer-size,400px); transform:translateX(100%); }
    .panel.left   { bottom:0; left:0;  top:0; width:var(--fui-drawer-size,400px); transform:translateX(-100%); }
    .panel.bottom { bottom:0; left:0; right:0; height:var(--fui-drawer-size,360px); transform:translateY(100%); }
    .panel.top    { left:0; right:0; top:0; height:var(--fui-drawer-size,360px); transform:translateY(-100%); }
    .panel.open   { transform: none; }

    .header {
        align-items    : center;
        border-bottom  : 1px solid var(--fui-drawer-border, #2A2A2E);
        display        : flex;
        flex-shrink    : 0;
        justify-content: space-between;
        padding        : 1rem 1.25rem;
    }

    .title {
        color      : var(--fui-drawer-title-color, #E8E8ED);
        font-family: 'JetBrains Mono', monospace;
        font-size  : 1rem;
        font-weight: 600;
        margin     : 0;
    }

    .close {
        background : none;
        border     : none;
        color      : var(--fui-drawer-close-color, #6E6E76);
        cursor     : pointer;
        font-size  : 1rem;
        line-height: 1;
        padding    : 0.25rem;
        transition : color 0.15s;
    }
    .close:hover { color: #E8E8ED; }
    .close:focus-visible {
        border-radius : 3px;
        outline       : 2px solid #7C3AED;
        outline-offset: 2px;
    }

    .body {
        color      : var(--fui-drawer-color, #A0A0A8);
        flex       : 1;
        font-size  : 0.9375rem;
        line-height: 1.6;
        overflow-y : auto;
        padding    : 1.25rem;
    }

    .footer {
        border-top     : 1px solid var(--fui-drawer-border, #2A2A2E);
        display        : flex;
        flex-shrink    : 0;
        gap            : 0.75rem;
        justify-content: flex-end;
        padding        : 1rem 1.25rem;
    }
    .footer:not(:has(*)) { display: none; }
"""

[<LitElement("fui-drawer")>]
let FuiDrawer () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            triggerLabel = Prop.Of("Open drawer", attribute = "trigger-label")
            title        = Prop.Of("",            attribute = "title")
            placement    = Prop.Of("right",        attribute = "placement")
        |}
    )

    let isOpen, setIsOpen = Hook.useState false

    let open_ _ =
        setIsOpen true
        host.dispatchCustomEvent("fui-open",  detail = {| |})

    let close _ =
        setIsOpen false
        host.dispatchCustomEvent("fui-close", detail = {| |})

    let backdropCls = if isOpen then "backdrop open" else "backdrop"
    let panelCls    = if isOpen then $"panel {props.placement.Value} open" else $"panel {props.placement.Value}"

    html $"""
        <button class="trigger" @click={Ev open_}>{props.triggerLabel.Value}</button>
        <div class={backdropCls} @click={Ev close}></div>
        <div class={panelCls} role="dialog" aria-modal="true">
            <div class="header">
                <h2 class="title">{props.title.Value}</h2>
                <button class="close" aria-label="Close" @click={Ev close}>✕</button>
            </div>
            <div class="body"><slot></slot></div>
            <div class="footer"><slot name="footer"></slot></div>
        </div>
    """

let register () = ()
