module WebComponents.FuiSlider

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("parseFloat($0.target.value)")>]
let private getFloatVal (e: obj) : float = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        width      : 100%;
        min-width  : 0;
        font-family: var(--fui-slider-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    .field {
        display       : flex;
        flex-direction: column;
        gap           : 0.4rem;
    }

    .field-header {
        display        : flex;
        justify-content: space-between;
        align-items    : baseline;
    }

    .field-label {
        font-size      : 0.72rem;
        font-weight    : 600;
        letter-spacing : 0.06em;
        text-transform : uppercase;
        color          : var(--fui-slider-label-color, #6E6E76);
    }

    .value-display {
        font-size  : 0.75rem;
        font-weight: 600;
        color      : var(--fui-slider-value-color, #E8E8ED);
        min-width  : 3ch;
        text-align : right;
    }

    input[type="range"] {
        -webkit-appearance: none;
        appearance        : none;
        display           : block;
        width             : 100%;
        height            : 6px;
        border-radius     : 3px;
        outline           : none;
        cursor            : pointer;
        border            : none;
        padding           : 0;
        margin            : 0.3rem 0;
    }

    input[type="range"]:focus-visible {
        outline       : 2px solid var(--fui-slider-border-focus, #7C3AED);
        outline-offset: 4px;
        border-radius : 3px;
    }

    /* Webkit */
    input[type="range"]::-webkit-slider-runnable-track {
        height       : 6px;
        border-radius: 3px;
    }

    input[type="range"]::-webkit-slider-thumb {
        -webkit-appearance: none;
        appearance        : none;
        width             : 18px;
        height            : 18px;
        border-radius     : 50%;
        background        : var(--fui-slider-thumb, #7C3AED);
        border            : 2px solid var(--fui-slider-thumb-border, #0D0D0F);
        cursor            : pointer;
        margin-top        : -6px;
        transition        : box-shadow 0.15s;
    }

    input[type="range"]::-webkit-slider-thumb:hover {
        box-shadow: 0 0 0 4px rgba(124,58,237,0.25);
    }

    input[type="range"]:active::-webkit-slider-thumb {
        box-shadow: 0 0 0 6px rgba(124,58,237,0.2);
    }

    /* Firefox */
    input[type="range"]::-moz-range-track {
        height       : 6px;
        border-radius: 3px;
        background   : var(--fui-slider-track-bg, #2A2A2E);
    }

    input[type="range"]::-moz-range-progress {
        height       : 6px;
        border-radius: 3px;
        background   : var(--fui-slider-fill, #7C3AED);
    }

    input[type="range"]::-moz-range-thumb {
        width        : 18px;
        height       : 18px;
        border-radius: 50%;
        background   : var(--fui-slider-thumb, #7C3AED);
        border       : 2px solid var(--fui-slider-thumb-border, #0D0D0F);
        cursor       : pointer;
        transition   : box-shadow 0.15s;
    }

    input[type="range"]::-moz-range-thumb:hover {
        box-shadow: 0 0 0 4px rgba(124,58,237,0.25);
    }

    .error-msg {
        font-size  : 0.78rem;
        font-family: 'Sora', sans-serif;
        color      : #EF4444;
    }

    :host([disabled]) input[type="range"] {
        opacity: 0.45;
        cursor : not-allowed;
    }

    /* sm: track 4px, thumb 14px — margin-top = -(14-4)/2 = -5px */
    :host([size="sm"]) input[type="range"]                         { height: 4px; }
    :host([size="sm"]) input[type="range"]::-webkit-slider-runnable-track { height: 4px; }
    :host([size="sm"]) input[type="range"]::-webkit-slider-thumb   { width: 14px; height: 14px; margin-top: -5px; }
    :host([size="sm"]) input[type="range"]::-moz-range-track       { height: 4px; }
    :host([size="sm"]) input[type="range"]::-moz-range-progress    { height: 4px; }
    :host([size="sm"]) input[type="range"]::-moz-range-thumb       { width: 14px; height: 14px; }
    :host([size="sm"]) .field-label                                { font-size: 0.68rem; }
    :host([size="sm"]) .value-display                              { font-size: 0.7rem; }

    /* lg: track 8px, thumb 22px — margin-top = -(22-8)/2 = -7px */
    :host([size="lg"]) input[type="range"]                         { height: 8px; }
    :host([size="lg"]) input[type="range"]::-webkit-slider-runnable-track { height: 8px; }
    :host([size="lg"]) input[type="range"]::-webkit-slider-thumb   { width: 22px; height: 22px; margin-top: -7px; }
    :host([size="lg"]) input[type="range"]::-moz-range-track       { height: 8px; }
    :host([size="lg"]) input[type="range"]::-moz-range-progress    { height: 8px; }
    :host([size="lg"]) input[type="range"]::-moz-range-thumb       { width: 22px; height: 22px; }
    :host([size="lg"]) .field-label                                { font-size: 0.78rem; }
    :host([size="lg"]) .value-display                              { font-size: 0.82rem; }
"""

[<LitElement("fui-slider")>]
let FuiSlider() =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            value     = Prop.Of(0.0,   attribute = "value")
            min       = Prop.Of(0.0,   attribute = "min")
            max       = Prop.Of(100.0, attribute = "max")
            step      = Prop.Of(1.0,   attribute = "step")
            label     = Prop.Of("",    attribute = "label")
            disabled  = Prop.Of(false, attribute = "disabled")
            fieldName = Prop.Of("",    attribute = "name")
            size      = Prop.Of("md",  attribute = "size")
            error     = Prop.Of("",    attribute = "error")
        |}
    )

    let currentVal, setVal = Hook.useState(props.value.Value)

    let onInput = Ev (fun ev ->
        let v = getFloatVal (ev :> obj)
        setVal v
        host.dispatchCustomEvent("fui-input", detail = {| value = v |}))

    let onChange = Ev (fun ev ->
        let v = getFloatVal (ev :> obj)
        host.dispatchCustomEvent("fui-change", detail = {| value = v |}))

    let hasError = props.error.Value <> ""

    // Compute fill percentage for the webkit gradient track.
    // Firefox uses ::-moz-range-progress natively, so no gradient needed there.
    let fillPct =
        let range = props.max.Value - props.min.Value
        if range <= 0.0 then 0.0
        else System.Math.Max(0.0, System.Math.Min(100.0, (currentVal - props.min.Value) / range * 100.0))

    let trackStyle =
        sprintf
            "background: linear-gradient(to right, var(--fui-slider-fill, #7C3AED) %.2f%%, var(--fui-slider-track-bg, #2A2A2E) %.2f%%)"
            fillPct fillPct

    let headerPart =
        if props.label.Value <> "" then
            html $"""
                <div class="field-header" part="header">
                    <span class="field-label" part="label">{props.label.Value}</span>
                    <span class="value-display" part="value">{currentVal}</span>
                </div>
            """
        else
            Lit.nothing

    let errorPart =
        if hasError then
            html $"""<span class="error-msg" part="error" role="alert">{props.error.Value}</span>"""
        else
            Lit.nothing

    html $"""
        <div class={if hasError then "field has-error" else "field"} part="field">
            {headerPart}
            <input
                type="range"
                part="input"
                name={props.fieldName.Value}
                min={props.min.Value}
                max={props.max.Value}
                step={props.step.Value}
                .value={string currentVal}
                ?disabled={props.disabled.Value}
                style={trackStyle}
                @input={onInput}
                @change={onChange}
            />
            {errorPart}
        </div>
    """

let register () = ()
