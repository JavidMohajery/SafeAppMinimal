module WebComponents.FuiModal

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : inline-block;
        font-family: var(--fui-modal-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .trigger {
        background   : var(--fui-modal-trigger-bg, #7C3AED);
        border       : none;
        border-radius: 6px;
        color        : var(--fui-modal-trigger-color, #fff);
        cursor       : pointer;
        font-family  : inherit;
        font-size    : 0.875rem;
        font-weight  : 500;
        padding      : 0.5rem 1rem;
        transition   : background 0.15s;
    }
    .trigger:hover { background: var(--fui-modal-trigger-bg-hover, #9B59F5); }
    .trigger:focus-visible {
        outline       : 2px solid #7C3AED;
        outline-offset: 2px;
    }

    /* Semitransparent backdrop — sibling of .panel so no stopPropagation needed */
    .backdrop {
        background: rgba(0,0,0,0.6);
        display   : none;
        inset     : 0;
        position  : fixed;
        z-index   : 999;
    }
    .backdrop.open { display: block; }

    /* Panel is a sibling of backdrop, rendered on top via z-index */
    .panel {
        background    : var(--fui-modal-bg, #161618);
        border        : 1px solid var(--fui-modal-border, #2A2A2E);
        border-radius : var(--fui-modal-radius, 10px);
        box-shadow    : 0 8px 40px rgba(0,0,0,0.5);
        display       : none;
        flex-direction: column;
        left          : 50%;
        max-height    : 90vh;
        max-width     : calc(100vw - 2rem);
        position      : fixed;
        top           : 50%;
        transform     : translate(-50%, -50%);
        width         : 560px;
        z-index       : 1000;
    }
    .panel.sm   { width: 400px; }
    .panel.lg   { width: 720px; }
    .panel.open { display: flex; }

    .header {
        align-items    : center;
        border-bottom  : 1px solid var(--fui-modal-border, #2A2A2E);
        display        : flex;
        flex-shrink    : 0;
        justify-content: space-between;
        padding        : 1rem 1.25rem;
    }

    .title {
        color      : var(--fui-modal-title-color, #E8E8ED);
        font-family: 'JetBrains Mono', monospace;
        font-size  : 1rem;
        font-weight: 600;
        margin     : 0;
    }

    .close {
        background : none;
        border     : none;
        color      : var(--fui-modal-close-color, #6E6E76);
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
        color      : var(--fui-modal-color, #A0A0A8);
        flex       : 1;
        font-size  : 0.9375rem;
        line-height: 1.6;
        overflow-y : auto;
        padding    : 1.25rem;
    }

    .footer {
        border-top     : 1px solid var(--fui-modal-border, #2A2A2E);
        display        : flex;
        flex-shrink    : 0;
        gap            : 0.75rem;
        justify-content: flex-end;
        padding        : 1rem 1.25rem;
    }
    .footer:not(:has(*)) { display: none; }
"""

[<LitElement("fui-modal")>]
let FuiModal () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            triggerLabel = Prop.Of("Open", attribute = "trigger-label")
            title        = Prop.Of("",     attribute = "title")
            size         = Prop.Of("md",   attribute = "size")
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
    let panelCls    = if isOpen then $"panel {props.size.Value} open" else $"panel {props.size.Value}"

    html $"""
        <button class="trigger" @click={Ev open_}>{props.triggerLabel.Value}</button>
        <div class={backdropCls} @click={Ev close}></div>
        <div class={panelCls} role="dialog" aria-modal="true" aria-label={props.title.Value}>
            <div class="header">
                <h2 class="title">{props.title.Value}</h2>
                <button class="close" aria-label="Close" @click={Ev close}>✕</button>
            </div>
            <div class="body"><slot></slot></div>
            <div class="footer"><slot name="footer"></slot></div>
        </div>
    """

let register () = ()
