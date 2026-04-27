module WebComponents.FuiColorPicker

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-cp-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    .field {
        display       : flex;
        flex-direction: column;
        gap           : 0.35rem;
    }

    .field-label {
        display        : block;
        font-size      : 0.72rem;
        font-weight    : 600;
        letter-spacing : 0.06em;
        text-transform : uppercase;
        color          : var(--fui-cp-label-color, #6E6E76);
    }

    /* The visible control — swatch + hex text. The native color input sits on
       top of this at opacity:0 so any click opens the browser color picker. */
    .picker-wrap {
        position     : relative;
        display      : inline-flex;
        align-items  : center;
        gap          : 0.6rem;
        background   : var(--fui-cp-bg, #1E1E21);
        border       : 1px solid var(--fui-cp-border, #2A2A2E);
        border-radius: var(--fui-cp-radius, 6px);
        padding      : 0.4rem 0.75rem;
        cursor       : pointer;
        transition   : border-color 0.12s;
        user-select  : none;
    }

    .picker-wrap:focus-within {
        border-color : var(--fui-cp-border-focus, #7C3AED);
        outline      : 2px solid var(--fui-cp-border-focus, #7C3AED);
        outline-offset: 2px;
    }

    .swatch {
        flex-shrink  : 0;
        width        : 20px;
        height       : 20px;
        border-radius: 4px;
        border       : 1px solid rgba(255,255,255,0.12);
    }

    .hex-value {
        font-size     : 0.8125rem;
        font-weight   : 600;
        letter-spacing: 0.04em;
        color         : var(--fui-cp-value-color, #E8E8ED);
    }

    input[type="color"] {
        position: absolute;
        inset   : 0;
        width   : 100%;
        height  : 100%;
        opacity : 0;
        cursor  : pointer;
        padding : 0;
        border  : none;
    }

    .error-msg {
        font-size  : 0.78rem;
        font-family: 'Sora', sans-serif;
        color      : #EF4444;
    }

    :host([disabled]) .picker-wrap {
        opacity       : 0.45;
        cursor        : not-allowed;
        pointer-events: none;
    }

    :host([invalid]) .picker-wrap,
    .has-error .picker-wrap {
        border-color: #EF4444;
    }

    /* sm */
    :host([size="sm"]) .picker-wrap { padding: 0.25rem 0.6rem;  gap: 0.45rem; }
    :host([size="sm"]) .swatch      { width: 16px; height: 16px; }
    :host([size="sm"]) .hex-value   { font-size: 0.72rem; }

    /* lg */
    :host([size="lg"]) .picker-wrap { padding: 0.55rem 0.9rem;  gap: 0.75rem; }
    :host([size="lg"]) .swatch      { width: 24px; height: 24px; }
    :host([size="lg"]) .hex-value   { font-size: 0.9rem; }
"""

[<LitElement("fui-color-picker")>]
let FuiColorPicker() =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            value     = Prop.Of("#7C3AED", attribute = "value")
            label     = Prop.Of("",        attribute = "label")
            disabled  = Prop.Of(false,     attribute = "disabled")
            fieldName = Prop.Of("",        attribute = "name")
            size      = Prop.Of("md",      attribute = "size")
            error     = Prop.Of("",        attribute = "error")
        |}
    )

    let currentVal, setVal = Hook.useState(props.value.Value)

    let onInput = EvVal (fun v ->
        setVal v
        host.dispatchCustomEvent("fui-input", detail = {| value = v |}))

    let onChange = EvVal (fun v ->
        setVal v
        host.dispatchCustomEvent("fui-change", detail = {| value = v |}))

    let hasError = props.error.Value <> ""
    let swatchStyle = $"background-color: {currentVal}"
    let hexDisplay  = currentVal.ToUpper()

    let labelPart =
        if props.label.Value <> "" then
            html $"""<span class="field-label" part="label">{props.label.Value}</span>"""
        else
            Lit.nothing

    let errorPart =
        if hasError then
            html $"""<span class="error-msg" part="error" role="alert">{props.error.Value}</span>"""
        else
            Lit.nothing

    html $"""
        <div class={if hasError then "field has-error" else "field"} part="field">
            {labelPart}
            <div class="picker-wrap" part="picker-wrap">
                <span class="swatch" part="swatch" style={swatchStyle}></span>
                <span class="hex-value" part="value">{hexDisplay}</span>
                <input
                    type="color"
                    part="input"
                    name={props.fieldName.Value}
                    .value={currentVal}
                    ?disabled={props.disabled.Value}
                    @input={onInput}
                    @change={onChange}
                />
            </div>
            {errorPart}
        </div>
    """

let register () = ()
