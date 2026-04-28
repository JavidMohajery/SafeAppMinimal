module WebComponents.FuiErrorBoundary

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-eb-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .container {
        align-items  : center;
        background   : var(--fui-eb-bg, #161618);
        border       : 1px solid var(--fui-eb-border, rgba(239,68,68,0.2));
        border-radius: var(--fui-eb-radius, 8px);
        display      : flex;
        flex-direction: column;
        gap          : 1rem;
        padding      : var(--fui-eb-padding, 2.5rem 2rem);
        text-align   : center;
    }

    .icon {
        color      : #EF4444;
        font-size  : 2.25rem;
        line-height: 1;
    }

    .title {
        color      : var(--fui-eb-title-color, #E8E8ED);
        font-family: 'JetBrains Mono', monospace;
        font-size  : 1rem;
        font-weight: 700;
        margin     : 0;
    }

    .message {
        color      : var(--fui-eb-msg-color, #6E6E76);
        font-size  : 0.875rem;
        line-height: 1.6;
        margin     : 0;
        max-width  : 42ch;
    }

    .actions {
        align-items: center;
        display    : flex;
        gap        : 0.625rem;
    }

    .retry-btn {
        background   : #7C3AED;
        border       : none;
        border-radius: 6px;
        color        : #fff;
        cursor       : pointer;
        font-family  : 'JetBrains Mono', monospace;
        font-size    : 0.875rem;
        font-weight  : 600;
        padding      : 0.45rem 1.25rem;
        transition   : background 0.15s;
    }
    .retry-btn:hover { background: #9B59F5; }
    .retry-btn:focus-visible { outline: 2px solid #9B59F5; outline-offset: 2px; }

    .code-toggle {
        background   : none;
        border       : 1px solid #2A2A2E;
        border-radius: 6px;
        color        : #6E6E76;
        cursor       : pointer;
        font-family  : 'JetBrains Mono', monospace;
        font-size    : 0.8rem;
        padding      : 0.4rem 0.875rem;
        transition   : border-color 0.15s, color 0.15s;
    }
    .code-toggle:hover { border-color: #3A3A3E; color: #A0A0A8; }
    .code-toggle:focus-visible { outline: 2px solid #3A3A3E; outline-offset: 2px; }

    .code-block {
        background  : #0D0D0F;
        border      : 1px solid #2A2A2E;
        border-radius: 6px;
        color       : #EF4444;
        font-family : 'JetBrains Mono', monospace;
        font-size   : 0.75rem;
        line-height : 1.6;
        max-height  : 160px;
        overflow-y  : auto;
        padding     : 0.875rem 1rem;
        text-align  : left;
        white-space : pre-wrap;
        width       : 100%;
    }
"""

[<LitElement("fui-error-boundary")>]
let FuiErrorBoundary () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            title     = Prop.Of("Something went wrong", attribute = "title")
            message   = Prop.Of("An unexpected error occurred. Please try again.", attribute = "message")
            code      = Prop.Of("",    attribute = "code")
            retryable = Prop.Of(false, attribute = "retryable")
        |}
    )

    let showCode, setShowCode = Hook.useState false

    let onRetry _ =
        host.dispatchCustomEvent("fui-retry", detail = {| |})

    let retryBtn =
        if props.retryable.Value then
            html $"""<button class="retry-btn" @click={Ev onRetry}>Try again</button>"""
        else Lit.nothing

    let codeToggle =
        if props.code.Value <> "" then
            let label = if showCode then "Hide details" else "Show details"
            html $"""<button class="code-toggle" @click={Ev (fun _ -> setShowCode (not showCode))}>{label}</button>"""
        else Lit.nothing

    let codeBlock =
        if props.code.Value <> "" && showCode then
            html $"""<pre class="code-block" part="code">{props.code.Value}</pre>"""
        else Lit.nothing

    let hasActions = props.retryable.Value || props.code.Value <> ""

    let actionsDiv =
        if hasActions then html $"<div class=\"actions\">{retryBtn}{codeToggle}</div>"
        else Lit.nothing

    html $"""
        <div class="container" part="container" role="alert">
            <div class="icon" aria-hidden="true">⚠</div>
            <p class="title" part="title">{props.title.Value}</p>
            <p class="message" part="message">{props.message.Value}</p>
            {actionsDiv}
            {codeBlock}
        </div>
    """

let register () = ()
