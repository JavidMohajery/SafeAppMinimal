module WebComponents.FuiToast

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-toast-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .outer { display: block; }
    .outer.dismissed { display: none; }

    .toast {
        align-items  : flex-start;
        background   : var(--fui-toast-bg, #1E1E21);
        border       : 1px solid var(--fui-toast-border, #2A2A2E);
        border-radius: var(--fui-toast-radius, 8px);
        box-shadow   : 0 4px 20px rgba(0,0,0,0.4);
        display      : flex;
        gap          : 0.75rem;
        max-width    : 360px;
        padding      : 0.875rem 1rem;
        width        : 100%;
    }

    /* ── Variant accent stripe ───────────────────────────────────────────────── */
    :host([variant="success"]) .toast { border-left: 3px solid #22C55E; }
    :host([variant="warning"]) .toast { border-left: 3px solid #F59E0B; }
    :host([variant="danger"])  .toast { border-left: 3px solid #EF4444; }
    :host([variant="info"])    .toast { border-left: 3px solid #3B82F6; }

    .icon {
        flex-shrink: 0;
        font-size  : 1rem;
        line-height: 1.45;
    }
    :host([variant="success"]) .icon { color: #22C55E; }
    :host([variant="warning"]) .icon { color: #F59E0B; }
    :host([variant="danger"])  .icon { color: #EF4444; }
    :host([variant="info"])    .icon { color: #3B82F6; }

    .body {
        flex     : 1;
        min-width: 0;
    }

    .title {
        color      : var(--fui-toast-title-color, #E8E8ED);
        font-family: 'JetBrains Mono', monospace;
        font-size  : 0.8125rem;
        font-weight: 600;
        margin     : 0 0 0.2rem;
    }

    .msg {
        color      : var(--fui-toast-color, #A0A0A8);
        font-size  : 0.8125rem;
        line-height: 1.5;
        margin     : 0;
    }

    .close {
        background : none;
        border     : none;
        color      : var(--fui-toast-close-color, #6E6E76);
        cursor     : pointer;
        flex-shrink: 0;
        font-size  : 0.8rem;
        line-height: 1;
        padding    : 0.15rem;
        transition : color 0.15s;
    }
    .close:hover { color: #E8E8ED; }
    .close:focus-visible {
        border-radius : 3px;
        outline       : 2px solid #7C3AED;
        outline-offset: 2px;
    }
"""

let private iconFor = function
    | "success" -> "✓"
    | "warning" -> "⚠"
    | "danger"  -> "✕"
    | _         -> "ℹ"

[<LitElement("fui-toast")>]
let FuiToast () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            variant = Prop.Of("info", attribute = "variant")
            title   = Prop.Of("",     attribute = "title")
            message = Prop.Of("",     attribute = "message")
        |}
    )

    let dismissed, setDismissed = Hook.useState false

    let close _ =
        setDismissed true
        host.dispatchCustomEvent("fui-dismiss", detail = {| variant = props.variant.Value |})

    let outerCls = if dismissed then "outer dismissed" else "outer"

    let titlePart =
        if props.title.Value <> "" then
            html $"""<p class="title">{props.title.Value}</p>"""
        else Lit.nothing

    let msgPart =
        if props.message.Value <> "" then
            html $"""<p class="msg">{props.message.Value}</p>"""
        else
            html $"""<p class="msg"><slot></slot></p>"""

    html $"""
        <div class={outerCls}>
            <div class="toast" part="toast" role="status" aria-live="polite">
                <span class="icon" aria-hidden="true">{iconFor props.variant.Value}</span>
                <div class="body">
                    {titlePart}
                    {msgPart}
                </div>
                <button class="close" aria-label="Close notification" @click={Ev close}>✕</button>
            </div>
        </div>
    """

let register () = ()
