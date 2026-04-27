module WebComponents.FuiToggle

open Lit
open Fable.Core
open Browser.Types

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("$0.target.checked")>]
let private getChecked (e: obj) : bool = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-toggle-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    .field {
        display       : flex;
        flex-direction: column;
        gap           : 0.25rem;
    }

    .toggle-wrap {
        display    : inline-flex;
        align-items: center;
        gap        : 0.65rem;
        cursor     : pointer;
        user-select: none;
    }

    input[type="checkbox"] {
        position: absolute;
        opacity : 0;
        width   : 0;
        height  : 0;
        margin  : 0;
    }

    .track {
        position     : relative;
        display      : inline-block;
        flex-shrink  : 0;
        width        : 44px;
        height       : 24px;
        border-radius: 12px;
        background   : var(--fui-toggle-track-bg, #2A2A2E);
        border       : 1.5px solid var(--fui-toggle-border, #3A3A3E);
        transition   : background 0.2s, border-color 0.2s;
    }

    .thumb {
        position     : absolute;
        top          : 50%;
        left         : 2px;
        width        : 18px;
        height       : 18px;
        border-radius: 50%;
        background   : var(--fui-toggle-thumb-off, #6E6E76);
        transform    : translateY(-50%);
        transition   : left 0.2s, background 0.2s;
    }

    input:checked + .track {
        background  : var(--fui-toggle-track-checked, #7C3AED);
        border-color: var(--fui-toggle-track-checked, #7C3AED);
    }

    input:checked + .track .thumb {
        left      : calc(100% - 20px);
        background: var(--fui-toggle-thumb-on, #fff);
    }

    input:focus-visible + .track {
        outline       : 2px solid var(--fui-toggle-border-focus, #7C3AED);
        outline-offset: 2px;
    }

    .toggle-label {
        font-size  : 0.875rem;
        color      : var(--fui-toggle-label-color, #E8E8ED);
        line-height: 1.4;
    }

    .error-msg {
        font-size   : 0.78rem;
        font-family : 'Sora', sans-serif;
        color       : #EF4444;
        padding-left: calc(44px + 0.65rem);
    }

    :host([disabled]) .toggle-wrap {
        opacity       : 0.45;
        cursor        : not-allowed;
        pointer-events: none;
    }

    /* sm: track 32×18, thumb 12 */
    :host([size="sm"]) .track                               { width: 32px; height: 18px; border-radius: 9px; }
    :host([size="sm"]) .thumb                               { width: 12px; height: 12px; }
    :host([size="sm"]) input:checked + .track .thumb        { left: calc(100% - 14px); }
    :host([size="sm"]) .toggle-label                        { font-size: 0.75rem; }
    :host([size="sm"]) .error-msg                           { padding-left: calc(32px + 0.65rem); }

    /* lg: track 56×30, thumb 24 */
    :host([size="lg"]) .track                               { width: 56px; height: 30px; border-radius: 15px; }
    :host([size="lg"]) .thumb                               { width: 24px; height: 24px; }
    :host([size="lg"]) input:checked + .track .thumb        { left: calc(100% - 26px); }
    :host([size="lg"]) .toggle-label                        { font-size: 1rem; }
    :host([size="lg"]) .error-msg                           { padding-left: calc(56px + 0.65rem); }
"""

[<LitElement("fui-toggle")>]
let FuiToggle() =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            isChecked = Prop.Of(false, attribute = "checked")
            label     = Prop.Of("",    attribute = "label")
            disabled  = Prop.Of(false, attribute = "disabled")
            fieldName = Prop.Of("",    attribute = "name")
            size      = Prop.Of("md",  attribute = "size")
            error     = Prop.Of("",    attribute = "error")
        |}
    )

    let isOn, setIsOn = Hook.useState(props.isChecked.Value)

    let onChange (e: Event) =
        let v = getChecked (e :> obj)
        setIsOn v
        host.dispatchCustomEvent("fui-change", detail = {| ``checked`` = v |})

    let hasError = props.error.Value <> ""

    let labelPart =
        if props.label.Value <> "" then
            html $"""<span class="toggle-label" part="label">{props.label.Value}</span>"""
        else
            Lit.nothing

    let errorPart =
        if hasError then
            html $"""<span class="error-msg" part="error" role="alert">{props.error.Value}</span>"""
        else
            Lit.nothing

    html $"""
        <div class="field" part="field">
            <label class="toggle-wrap" part="wrap">
                <input
                    type="checkbox"
                    role="switch"
                    name={props.fieldName.Value}
                    .checked={isOn}
                    ?disabled={props.disabled.Value}
                    @change={Ev onChange}
                />
                <span class="track" part="track">
                    <span class="thumb" part="thumb"></span>
                </span>
                {labelPart}
            </label>
            {errorPart}
        </div>
    """

let register () = ()
