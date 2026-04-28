module WebComponents.FuiConfirmDialog

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : inline-block;
        font-family: var(--fui-cd-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .trigger {
        background   : var(--fui-cd-trigger-bg, #1E1E21);
        border       : 1px solid var(--fui-cd-trigger-border, #2A2A2E);
        border-radius: 6px;
        color        : var(--fui-cd-trigger-color, #E8E8ED);
        cursor       : pointer;
        font-family  : inherit;
        font-size    : 0.875rem;
        font-weight  : 500;
        padding      : 0.5rem 1rem;
        transition   : border-color 0.15s;
    }
    .trigger:hover     { border-color: #7C3AED; }
    .trigger.danger    { background: #EF4444; border-color: #EF4444; color: #fff; }
    .trigger.danger:hover { background: #DC2626; border-color: #DC2626; }
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

    .panel {
        background    : var(--fui-cd-bg, #161618);
        border        : 1px solid var(--fui-cd-border, #2A2A2E);
        border-radius : var(--fui-cd-radius, 10px);
        box-shadow    : 0 8px 40px rgba(0,0,0,0.5);
        display       : none;
        flex-direction: column;
        left          : 50%;
        max-width     : calc(100vw - 2rem);
        position      : fixed;
        top           : 50%;
        transform     : translate(-50%, -50%);
        width         : 420px;
        z-index       : 1000;
    }
    .panel.open { display: flex; }

    .header {
        border-bottom: 1px solid var(--fui-cd-border, #2A2A2E);
        flex-shrink  : 0;
        padding      : 1rem 1.25rem;
    }

    .title {
        color      : var(--fui-cd-title-color, #E8E8ED);
        font-family: 'JetBrains Mono', monospace;
        font-size  : 1rem;
        font-weight: 600;
        margin     : 0;
    }

    .body {
        color      : var(--fui-cd-color, #A0A0A8);
        flex       : 1;
        font-size  : 0.9375rem;
        line-height: 1.6;
        padding    : 1.25rem;
    }

    .footer {
        border-top     : 1px solid var(--fui-cd-border, #2A2A2E);
        display        : flex;
        flex-shrink    : 0;
        gap            : 0.75rem;
        justify-content: flex-end;
        padding        : 1rem 1.25rem;
    }

    .cancel-btn {
        background   : transparent;
        border       : 1px solid #2A2A2E;
        border-radius: 6px;
        color        : #A0A0A8;
        cursor       : pointer;
        font-family  : inherit;
        font-size    : 0.875rem;
        padding      : 0.5rem 1rem;
        transition   : border-color 0.15s, color 0.15s;
    }
    .cancel-btn:hover { border-color: #3A3A3E; color: #E8E8ED; }
    .cancel-btn:focus-visible {
        outline       : 2px solid #7C3AED;
        outline-offset: 2px;
    }

    .confirm-btn {
        background   : #7C3AED;
        border       : none;
        border-radius: 6px;
        color        : #fff;
        cursor       : pointer;
        font-family  : inherit;
        font-size    : 0.875rem;
        font-weight  : 500;
        padding      : 0.5rem 1rem;
        transition   : background 0.15s;
    }
    .confirm-btn:hover         { background: #9B59F5; }
    .confirm-btn.danger        { background: #EF4444; }
    .confirm-btn.danger:hover  { background: #DC2626; }
    .confirm-btn:focus-visible {
        outline       : 2px solid #7C3AED;
        outline-offset: 2px;
    }
"""

[<LitElement("fui-confirm-dialog")>]
let FuiConfirmDialog () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            triggerLabel = Prop.Of("Delete",        attribute = "trigger-label")
            title        = Prop.Of("Are you sure?", attribute = "title")
            message      = Prop.Of("",              attribute = "message")
            confirmLabel = Prop.Of("Confirm",       attribute = "confirm-label")
            cancelLabel  = Prop.Of("Cancel",        attribute = "cancel-label")
            variant      = Prop.Of("default",       attribute = "variant")
        |}
    )

    let isOpen, setIsOpen = Hook.useState false

    let open_ _ =
        setIsOpen true
        host.dispatchCustomEvent("fui-open",    detail = {| |})

    let cancel _ =
        setIsOpen false
        host.dispatchCustomEvent("fui-cancel",  detail = {| |})

    let confirm _ =
        setIsOpen false
        host.dispatchCustomEvent("fui-confirm", detail = {| |})

    let isDanger    = props.variant.Value = "danger"
    let triggerCls  = if isDanger then "trigger danger" else "trigger"
    let confirmCls  = if isDanger then "confirm-btn danger" else "confirm-btn"
    let backdropCls = if isOpen then "backdrop open" else "backdrop"
    let panelCls    = if isOpen then "panel open" else "panel"

    let msgPart =
        if props.message.Value <> "" then
            html $"""<p style="margin:0 0 0.5rem">{props.message.Value}</p>"""
        else Lit.nothing

    html $"""
        <button class={triggerCls} @click={Ev open_}>{props.triggerLabel.Value}</button>
        <div class={backdropCls} @click={Ev cancel}></div>
        <div class={panelCls} role="alertdialog" aria-modal="true" aria-label={props.title.Value}>
            <div class="header">
                <h2 class="title">{props.title.Value}</h2>
            </div>
            <div class="body">
                {msgPart}
                <slot></slot>
            </div>
            <div class="footer">
                <button class="cancel-btn"  @click={Ev cancel}>{props.cancelLabel.Value}</button>
                <button class={confirmCls}  @click={Ev confirm}>{props.confirmLabel.Value}</button>
            </div>
        </div>
    """

let register () = ()
