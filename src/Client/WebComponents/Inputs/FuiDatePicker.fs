module WebComponents.FuiDatePicker

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        width      : 100%;
        min-width  : 0;
        font-family: var(--fui-dp-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    .field {
        display       : flex;
        flex-direction: column;
        gap           : 0.35rem;
    }

    label {
        display        : block;
        font-size      : 0.72rem;
        font-weight    : 600;
        letter-spacing : 0.06em;
        text-transform : uppercase;
        color          : var(--fui-dp-label-color, #6E6E76);
    }

    .input-wrap {
        position: relative;
        display : block;
    }

    input[type="date"] {
        display      : block;
        width        : 100%;
        box-sizing   : border-box;
        background   : var(--fui-dp-bg, #1E1E21);
        color        : var(--fui-dp-color, #E8E8ED);
        border       : 1px solid var(--fui-dp-border, #2A2A2E);
        border-radius: var(--fui-dp-radius, 6px);
        padding      : 0.5rem 2.5rem 0.5rem 0.75rem;
        font-family  : inherit;
        font-size    : 0.875rem;
        outline      : none;
        cursor       : pointer;
        transition   : border-color 0.12s;
        color-scheme : dark;
    }

    input[type="date"]:focus {
        border-color: var(--fui-dp-border-focus, #7C3AED);
    }

    input[type="date"]:focus-visible {
        outline       : 2px solid var(--fui-dp-border-focus, #7C3AED);
        outline-offset: 2px;
    }

    /* Expand the webkit picker indicator to cover the full input so clicking
       anywhere opens the calendar, then show our own icon on top. */
    input[type="date"]::-webkit-calendar-picker-indicator {
        position  : absolute;
        inset     : 0;
        width     : auto;
        height    : auto;
        background: transparent;
        cursor    : pointer;
        opacity   : 0;
    }

    .cal-icon {
        position      : absolute;
        right         : 0.75rem;
        top           : 50%;
        transform     : translateY(-50%);
        pointer-events: none;
        color         : var(--fui-dp-icon-color, #6E6E76);
        display       : flex;
        align-items   : center;
    }

    .cal-icon svg {
        width : 1rem;
        height: 1rem;
    }

    :host([disabled]) input[type="date"] {
        opacity: 0.45;
        cursor : not-allowed;
    }

    :host([invalid]) input[type="date"],
    .has-error input[type="date"] {
        border-color: #EF4444;
    }

    .error-msg {
        font-size  : 0.78rem;
        font-family: 'Sora', sans-serif;
        color      : #EF4444;
    }

    /* Sizes */
    :host([size="sm"]) input[type="date"] { font-size: 0.75rem;  padding: 0.3rem  2.25rem 0.3rem  0.6rem;  }
    :host([size="md"]) input[type="date"] { font-size: 0.875rem; padding: 0.5rem  2.5rem  0.5rem  0.75rem; }
    :host([size="lg"]) input[type="date"] { font-size: 1rem;     padding: 0.65rem 2.75rem 0.65rem 0.9rem;  }
"""

[<LitElement("fui-date-picker")>]
let FuiDatePicker() =
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

    let currentVal, setVal = Hook.useState(props.value.Value)

    let onChange = EvVal (fun v ->
        setVal v
        host.dispatchCustomEvent("fui-change", detail = {| value = v |}))

    let hasError = props.error.Value <> ""

    let labelPart =
        if props.label.Value <> "" then
            html $"""<label part="label" for="dp">{props.label.Value}</label>"""
        else
            Lit.nothing

    let errorPart =
        if hasError then
            html $"""<span class="error-msg" part="error" role="alert">{props.error.Value}</span>"""
        else
            Lit.nothing

    // Extracted to a named let — SVG cannot be nested inside a triple-quote interpolation hole.
    let calIcon =
        html $"""
            <span class="cal-icon" aria-hidden="true">
                <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
                    <rect x="1.5" y="3" width="13" height="11.5" rx="1.5"/>
                    <line x1="1.5" y1="6.5" x2="14.5" y2="6.5"/>
                    <line x1="5" y1="1.5" x2="5" y2="4.5"/>
                    <line x1="11" y1="1.5" x2="11" y2="4.5"/>
                </svg>
            </span>
        """

    html $"""
        <div class={if hasError then "field has-error" else "field"} part="field">
            {labelPart}
            <div class="input-wrap" part="input-wrap">
                <input
                    type="date"
                    part="input"
                    id="dp"
                    name={props.fieldName.Value}
                    .value={currentVal}
                    min={props.min.Value}
                    max={props.max.Value}
                    ?disabled={props.disabled.Value}
                    ?required={props.required.Value}
                    @change={onChange}
                />
                {calIcon}
            </div>
            {errorPart}
        </div>
    """

let register () = ()
