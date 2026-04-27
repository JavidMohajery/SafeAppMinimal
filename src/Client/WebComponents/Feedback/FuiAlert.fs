module WebComponents.FuiAlert

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-alert-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .outer { display: block; }
    .outer.dismissed { display: none; }

    .alert {
        border       : 1px solid rgba(110,110,118,0.25);
        border-left  : 3px solid #6E6E76;
        border-radius: var(--fui-alert-radius, 8px);
        display      : flex;
        gap          : 0.75rem;
        padding      : 0.875rem 1rem;
    }

    /* ── Variants ───────────────────────────────────────────────────────────── */
    :host([variant="info"]) .alert {
        background       : rgba(59,130,246,0.07);
        border-color     : rgba(59,130,246,0.22);
        border-left-color: #3B82F6;
    }
    :host([variant="success"]) .alert {
        background       : rgba(34,197,94,0.07);
        border-color     : rgba(34,197,94,0.22);
        border-left-color: #22C55E;
    }
    :host([variant="warning"]) .alert {
        background       : rgba(245,158,11,0.07);
        border-color     : rgba(245,158,11,0.22);
        border-left-color: #F59E0B;
    }
    :host([variant="danger"]) .alert {
        background       : rgba(239,68,68,0.07);
        border-color     : rgba(239,68,68,0.22);
        border-left-color: #EF4444;
    }

    .icon {
        flex-shrink: 0;
        font-size  : 1rem;
        line-height: 1.45;
    }
    :host([variant="info"])    .icon { color: #3B82F6; }
    :host([variant="success"]) .icon { color: #22C55E; }
    :host([variant="warning"]) .icon { color: #F59E0B; }
    :host([variant="danger"])  .icon { color: #EF4444; }

    .body {
        flex     : 1;
        min-width: 0;
    }

    .title {
        color      : var(--fui-alert-title-color, #E8E8ED);
        font-family: 'JetBrains Mono', monospace;
        font-size  : 0.8125rem;
        font-weight: 600;
        margin     : 0 0 0.25rem;
    }

    .message {
        color      : var(--fui-alert-color, #A0A0A8);
        font-size  : 0.875rem;
        line-height: 1.55;
        margin     : 0;
    }

    .dismiss {
        background : none;
        border     : none;
        color      : var(--fui-alert-dismiss-color, #6E6E76);
        cursor     : pointer;
        flex-shrink: 0;
        font-size  : 0.875rem;
        line-height: 1;
        padding    : 0.15rem;
        transition : color 0.15s;
    }
    .dismiss:hover { color: #E8E8ED; }
    .dismiss:focus-visible {
        border-radius : 3px;
        outline       : 2px solid var(--fui-alert-focus, #7C3AED);
        outline-offset: 2px;
    }
"""

let private iconFor = function
    | "success" -> "✓"
    | "warning" -> "⚠"
    | "danger"  -> "✕"
    | _         -> "ℹ"

[<LitElement("fui-alert")>]
let FuiAlert () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            variant     = Prop.Of("info",  attribute = "variant")
            title       = Prop.Of("",      attribute = "title")
            dismissible = Prop.Of(false,   attribute = "dismissible")
        |}
    )

    let dismissed, setDismissed = Hook.useState false

    let dismiss _ =
        setDismissed true
        host.dispatchCustomEvent("fui-dismiss", detail = {| variant = props.variant.Value |})

    let outerCls = if dismissed then "outer dismissed" else "outer"

    let titlePart =
        if props.title.Value <> "" then
            html $"""<p class="title">{props.title.Value}</p>"""
        else Lit.nothing

    let dismissBtn =
        if props.dismissible.Value then
            html $"""
                <button class="dismiss" aria-label="Dismiss alert" @click={Ev dismiss}>✕</button>"""
        else Lit.nothing

    html $"""
        <div class={outerCls}>
            <div class="alert" part="alert" role="alert">
                <span class="icon" aria-hidden="true">{iconFor props.variant.Value}</span>
                <div class="body">
                    {titlePart}
                    <p class="message"><slot></slot></p>
                </div>
                {dismissBtn}
            </div>
        </div>
    """

let register () = ()
