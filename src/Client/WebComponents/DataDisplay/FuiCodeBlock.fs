module WebComponents.FuiCodeBlock

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("navigator.clipboard.writeText($0).then(function(){}).catch(function(){});")>]
let private copyToClipboard (_: string) : unit = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: 'JetBrains Mono', monospace;
    }
    :host([hidden]) { display: none; }

    .wrap {
        background   : var(--fui-code-bg, #0D0D0F);
        border       : 1px solid var(--fui-code-border, #2A2A2E);
        border-radius: var(--fui-code-radius, 8px);
        overflow     : hidden;
    }

    .toolbar {
        align-items    : center;
        border-bottom  : 1px solid var(--fui-code-border, #2A2A2E);
        display        : flex;
        justify-content: space-between;
        padding        : 0.5rem 0.875rem;
    }

    .lang {
        color         : var(--fui-code-lang-color, #6E6E76);
        font-size     : 0.7rem;
        font-weight   : 600;
        letter-spacing: 0.06em;
        text-transform: uppercase;
    }

    .copy-btn {
        background   : none;
        border       : 1px solid transparent;
        border-radius: 4px;
        color        : var(--fui-code-copy-color, #6E6E76);
        cursor       : pointer;
        font-family  : inherit;
        font-size    : 0.7rem;
        padding      : 0.2rem 0.5rem;
        transition   : color 0.15s, border-color 0.15s;
    }
    .copy-btn:hover { color: #E8E8ED; border-color: #3A3A3E; }
    .copy-btn.copied { color: #22C55E; border-color: rgba(34,197,94,0.3); }
    .copy-btn:focus-visible {
        outline       : 2px solid #7C3AED;
        outline-offset: 2px;
    }

    pre {
        color      : var(--fui-code-color, #E8E8ED);
        font-size  : var(--fui-code-font-size, 0.8125rem);
        line-height: 1.65;
        margin     : 0;
        overflow-x : auto;
        padding    : 1rem 1.125rem;
        white-space: pre;
    }
"""

[<LitElement("fui-code-block")>]
let FuiCodeBlock () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            language = Prop.Of("", attribute = "language")
            code     = Prop.Of("", attribute = "code")
        |}
    )

    let copied, setCopied = Hook.useState false

    let copy _ =
        copyToClipboard props.code.Value
        setCopied true

    let langPart =
        if props.language.Value <> "" then
            html $"""<span class="lang">{props.language.Value}</span>"""
        else
            html $"""<span></span>"""

    let copyLabel = if copied then "Copied!" else "Copy"
    let copyCls   = if copied then "copy-btn copied" else "copy-btn"

    html $"""
        <div class="wrap" part="wrap">
            <div class="toolbar">
                {langPart}
                <button class={copyCls} @click={Ev copy}>{copyLabel}</button>
            </div>
            <pre part="code">{props.code.Value}</pre>
        </div>
    """

let register () = ()
