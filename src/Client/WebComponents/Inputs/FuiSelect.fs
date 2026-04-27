module WebComponents.FuiSelect

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

// Erased at runtime — just calls JSON.parse.
[<Emit("JSON.parse($0)")>]
let private jsonParse (s: string) : 'T = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        width      : 100%;
        min-width  : 0;
        font-family: var(--fui-select-font, 'JetBrains Mono', monospace);
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
        color          : var(--fui-select-label-color, #6E6E76);
    }

    .select-wrap {
        position: relative;
        display : block;
    }

    select {
        display          : block;
        width            : 100%;
        box-sizing       : border-box;
        background       : var(--fui-select-bg, #1E1E21);
        color            : var(--fui-select-color, #E8E8ED);
        border           : 1px solid var(--fui-select-border, #2A2A2E);
        border-radius    : var(--fui-select-radius, 6px);
        padding          : 0.5rem 2.5rem 0.5rem 0.75rem;
        font-family      : inherit;
        font-size        : 0.875rem;
        appearance       : none;
        -webkit-appearance: none;
        cursor           : pointer;
        outline          : none;
        transition       : border-color 0.12s;
    }

    select:focus {
        border-color: var(--fui-select-border-focus, #7C3AED);
    }

    select:focus-visible {
        outline       : 2px solid var(--fui-select-border-focus, #7C3AED);
        outline-offset: 2px;
    }

    .arrow {
        position      : absolute;
        right         : 0.75rem;
        top           : 50%;
        transform     : translateY(-50%);
        pointer-events: none;
        color         : var(--fui-select-arrow-color, #6E6E76);
        font-size     : 0.7rem;
        line-height   : 1;
    }

    :host([disabled]) select {
        opacity: 0.45;
        cursor : not-allowed;
    }

    :host([invalid]) select,
    .has-error select {
        border-color: #EF4444;
    }

    .error-msg {
        font-size  : 0.78rem;
        font-family: 'Sora', sans-serif;
        color      : #EF4444;
    }

    /* Sizes */
    :host([size="sm"]) select { font-size: 0.75rem;  padding: 0.3rem  2.25rem 0.3rem  0.6rem;  }
    :host([size="md"]) select { font-size: 0.875rem; padding: 0.5rem  2.5rem  0.5rem  0.75rem; }
    :host([size="lg"]) select { font-size: 1rem;     padding: 0.65rem 2.75rem 0.65rem 0.9rem;  }
"""

[<LitElement("fui-select")>]
let FuiSelect() =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            value       = Prop.Of("",   attribute = "value")
            label       = Prop.Of("",   attribute = "label")
            placeholder = Prop.Of("",   attribute = "placeholder")
            options     = Prop.Of("[]", attribute = "options")
            disabled    = Prop.Of(false, attribute = "disabled")
            required    = Prop.Of(false, attribute = "required")
            error       = Prop.Of("",   attribute = "error")
            fieldName   = Prop.Of("",   attribute = "name")
            size        = Prop.Of("md", attribute = "size")
        |}
    )

    let currentVal, setVal = Hook.useState(props.value.Value)

    let onChange = EvVal (fun v ->
        setVal v
        host.dispatchCustomEvent("fui-change", detail = {| value = v |}))

    let opts : {| value: string; label: string |} array =
        try jsonParse props.options.Value
        with _ -> [||]

    let hasError = props.error.Value <> ""

    let labelPart =
        if props.label.Value <> "" then
            html $"""<label part="label" for="sel">{props.label.Value}</label>"""
        else
            Lit.nothing

    let placeholderPart =
        if props.placeholder.Value <> "" then
            html $"""<option value="" disabled ?selected={currentVal = ""}>{props.placeholder.Value}</option>"""
        else
            Lit.nothing

    let errorPart =
        if hasError then
            html $"""<span class="error-msg" part="error" role="alert">{props.error.Value}</span>"""
        else
            Lit.nothing

    // Must be a named let — triple-quote strings cannot be nested inside another triple-quote interpolation.
    let optionTpl (o: {| value: string; label: string |}) =
        html $"<option value={o.value}>{o.label}</option>"

    let optionItems =
        Lit.mapUnique (fun (o: {| value: string; label: string |}) -> o.value) optionTpl opts

    html $"""
        <div class={if hasError then "field has-error" else "field"} part="field">
            {labelPart}
            <div class="select-wrap" part="select-wrap">
                <select
                    part="select"
                    id="sel"
                    name={props.fieldName.Value}
                    ?disabled={props.disabled.Value}
                    ?required={props.required.Value}
                    .value={currentVal}
                    @change={onChange}
                >
                    {placeholderPart}
                    {optionItems}
                </select>
                <span class="arrow" aria-hidden="true">▾</span>
            </div>
            {errorPart}
        </div>
    """

let register () = ()
