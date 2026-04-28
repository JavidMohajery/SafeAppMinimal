module WebComponents.FuiTimePicker

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-tp-font, 'JetBrains Mono', monospace);
        min-width  : 0;
        width      : 100%;
    }
    :host([hidden]) { display: none; }

    .field {
        display       : flex;
        flex-direction: column;
        gap           : 0.35rem;
    }

    label {
        color         : var(--fui-tp-label-color, #6E6E76);
        display       : block;
        font-size     : 0.72rem;
        font-weight   : 600;
        letter-spacing: 0.06em;
        text-transform: uppercase;
    }

    .input-wrap { display: block; position: relative; }

    input[type="time"] {
        background   : var(--fui-tp-bg,     #1E1E21);
        border       : 1px solid var(--fui-tp-border, #2A2A2E);
        border-radius: var(--fui-tp-radius,  6px);
        box-sizing   : border-box;
        color        : var(--fui-tp-color,  #E8E8ED);
        color-scheme : dark;
        cursor       : pointer;
        display      : block;
        font-family  : inherit;
        font-size    : 0.875rem;
        outline      : none;
        padding      : 0.5rem 2.5rem 0.5rem 0.75rem;
        transition   : border-color 0.12s;
        width        : 100%;
    }
    input[type="time"]:focus         { border-color: var(--fui-tp-border-focus, #7C3AED); }
    input[type="time"]:focus-visible { outline: 2px solid var(--fui-tp-border-focus, #7C3AED); outline-offset: 2px; }

    input[type="time"]::-webkit-calendar-picker-indicator {
        background: transparent;
        cursor    : pointer;
        height    : auto;
        inset     : 0;
        opacity   : 0;
        position  : absolute;
        width     : auto;
    }

    .clock-icon {
        align-items   : center;
        color         : var(--fui-tp-icon-color, #6E6E76);
        display       : flex;
        pointer-events: none;
        position      : absolute;
        right         : 0.75rem;
        top           : 50%;
        transform     : translateY(-50%);
    }
    .clock-icon svg { height: 1rem; width: 1rem; }

    :host([disabled]) input[type="time"] { cursor: not-allowed; opacity: 0.45; }
    .has-error input[type="time"]        { border-color: #EF4444; }

    .error-msg {
        color      : #EF4444;
        font-family: 'Sora', sans-serif;
        font-size  : 0.78rem;
    }

    :host([size="sm"]) input[type="time"] { font-size: 0.75rem;  padding: 0.3rem  2.25rem 0.3rem  0.6rem;  }
    :host([size="lg"]) input[type="time"] { font-size: 1rem;     padding: 0.65rem 2.75rem 0.65rem 0.9rem;  }
"""

[<LitElement("fui-time-picker")>]
let FuiTimePicker () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            value     = Prop.Of("",    attribute = "value")
            min       = Prop.Of("",    attribute = "min")
            max       = Prop.Of("",    attribute = "max")
            label     = Prop.Of("",    attribute = "label")
            disabled  = Prop.Of(false, attribute = "disabled")
            required  = Prop.Of(false, attribute = "required")
            fieldName = Prop.Of("",    attribute = "name")
            size      = Prop.Of("md",  attribute = "size")
            error     = Prop.Of("",    attribute = "error")
        |}
    )

    let currentVal, setVal = Hook.useState props.value.Value

    let onChange = EvVal (fun v ->
        setVal v
        host.dispatchCustomEvent("fui-change", detail = {| value = v |}))

    let hasError = props.error.Value <> ""

    let labelPart =
        if props.label.Value <> "" then html $"""<label part="label" for="tp">{props.label.Value}</label>"""
        else Lit.nothing

    let errorPart =
        if hasError then html $"""<span class="error-msg" part="error" role="alert">{props.error.Value}</span>"""
        else Lit.nothing

    let clockIcon =
        html $"""
            <span class="clock-icon" aria-hidden="true">
                <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round">
                    <circle cx="8" cy="8" r="6.5"/>
                    <polyline points="8,4.5 8,8 10.5,10"/>
                </svg>
            </span>
        """

    html $"""
        <div class={if hasError then "field has-error" else "field"} part="field">
            {labelPart}
            <div class="input-wrap" part="input-wrap">
                <input
                    type="time"
                    part="input"
                    id="tp"
                    name={props.fieldName.Value}
                    .value={currentVal}
                    min={props.min.Value}
                    max={props.max.Value}
                    ?disabled={props.disabled.Value}
                    ?required={props.required.Value}
                    @change={onChange} />
                {clockIcon}
            </div>
            {errorPart}
        </div>
    """

let register () = ()
