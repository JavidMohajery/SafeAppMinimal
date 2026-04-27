module WebComponents.FuiRadioGroup

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("JSON.parse($0)")>]
let private jsonParse (s: string) : 'T = jsNative

[<Emit("$0.target.checked")>]
let private getChecked (e: obj) : bool = jsNative

[<Emit("$0.target.value")>]
let private getValue (e: obj) : string = jsNative

// ── fui-radio ─────────────────────────────────────────────────────────────────

let private radioStyles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-radio-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    .rb-wrap {
        display    : inline-flex;
        align-items: center;
        gap        : 0.55rem;
        cursor     : pointer;
        user-select: none;
    }

    input[type="radio"] {
        position: absolute;
        opacity : 0;
        width   : 0;
        height  : 0;
        margin  : 0;
    }

    .rb-circle {
        flex-shrink  : 0;
        position     : relative;
        width        : 18px;
        height       : 18px;
        border       : 1.5px solid var(--fui-radio-border, #2A2A2E);
        border-radius: 50%;
        background   : var(--fui-radio-bg, #1E1E21);
        transition   : background 0.12s, border-color 0.12s;
    }

    .rb-circle::after {
        content      : "";
        position     : absolute;
        display      : none;
        top          : 50%;
        left         : 50%;
        width        : 8px;
        height       : 8px;
        border-radius: 50%;
        background   : #fff;
        transform    : translate(-50%, -50%);
    }

    input:checked + .rb-circle {
        background  : var(--fui-radio-checked-bg, #7C3AED);
        border-color: var(--fui-radio-checked-bg, #7C3AED);
    }

    input:checked + .rb-circle::after { display: block; }

    input:focus-visible + .rb-circle {
        outline       : 2px solid var(--fui-radio-border-focus, #7C3AED);
        outline-offset: 2px;
    }

    .rb-label {
        font-size  : 0.875rem;
        color      : var(--fui-radio-label-color, #E8E8ED);
        line-height: 1.4;
    }

    :host([disabled]) .rb-wrap {
        opacity       : 0.45;
        cursor        : not-allowed;
        pointer-events: none;
    }

    :host([size="sm"]) .rb-circle        { width: 14px; height: 14px; }
    :host([size="sm"]) .rb-circle::after { width:  6px; height:  6px; }
    :host([size="sm"]) .rb-label         { font-size: 0.75rem; }
    :host([size="lg"]) .rb-circle        { width: 22px; height: 22px; }
    :host([size="lg"]) .rb-circle::after { width: 10px; height: 10px; }
    :host([size="lg"]) .rb-label         { font-size: 1rem; }
"""

[<LitElement("fui-radio")>]
let FuiRadio() =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ radioStyles ]
        init.props  <- {|
            value     = Prop.Of("",    attribute = "value")
            label     = Prop.Of("",    attribute = "label")
            isChecked = Prop.Of(false, attribute = "checked")
            fieldName = Prop.Of("",    attribute = "name")
            disabled  = Prop.Of(false, attribute = "disabled")
            size      = Prop.Of("md",  attribute = "size")
        |}
    )

    let currentChecked, setChecked = Hook.useState(props.isChecked.Value)

    let onChange = Ev (fun ev ->
        let v = getChecked (ev :> obj)
        setChecked v
        host.dispatchCustomEvent("fui-change", detail = {| value = props.value.Value; ``checked`` = v |}))

    let labelPart =
        if props.label.Value <> "" then
            html $"""<span class="rb-label" part="label">{props.label.Value}</span>"""
        else
            Lit.nothing

    html $"""
        <label class="rb-wrap" part="wrap">
            <input
                type="radio"
                name={props.fieldName.Value}
                value={props.value.Value}
                .checked={currentChecked}
                ?disabled={props.disabled.Value}
                @change={onChange}
            />
            <span class="rb-circle" part="circle"></span>
            {labelPart}
        </label>
    """

// ── fui-radio-group ───────────────────────────────────────────────────────────

let private groupStyles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-rg-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    .field {
        display       : flex;
        flex-direction: column;
        gap           : 0.5rem;
    }

    .field-label {
        display        : block;
        font-size      : 0.72rem;
        font-weight    : 600;
        letter-spacing : 0.06em;
        text-transform : uppercase;
        color          : var(--fui-rg-label-color, #6E6E76);
    }

    .options {
        display       : flex;
        flex-direction: column;
        gap           : 0.5rem;
    }

    .rb-wrap {
        display    : inline-flex;
        align-items: center;
        gap        : 0.55rem;
        cursor     : pointer;
        user-select: none;
    }

    input[type="radio"] {
        position: absolute;
        opacity : 0;
        width   : 0;
        height  : 0;
        margin  : 0;
    }

    .rb-circle {
        flex-shrink  : 0;
        position     : relative;
        width        : 18px;
        height       : 18px;
        border       : 1.5px solid var(--fui-rg-border, #2A2A2E);
        border-radius: 50%;
        background   : var(--fui-rg-bg, #1E1E21);
        transition   : background 0.12s, border-color 0.12s;
    }

    .rb-circle::after {
        content      : "";
        position     : absolute;
        display      : none;
        top          : 50%;
        left         : 50%;
        width        : 8px;
        height       : 8px;
        border-radius: 50%;
        background   : #fff;
        transform    : translate(-50%, -50%);
    }

    input:checked + .rb-circle {
        background  : var(--fui-rg-checked-bg, #7C3AED);
        border-color: var(--fui-rg-checked-bg, #7C3AED);
    }

    input:checked + .rb-circle::after { display: block; }

    input:focus-visible + .rb-circle {
        outline       : 2px solid var(--fui-rg-border-focus, #7C3AED);
        outline-offset: 2px;
    }

    .rb-label {
        font-size  : 0.875rem;
        color      : var(--fui-rg-option-color, #E8E8ED);
        line-height: 1.4;
    }

    .error-msg {
        font-size  : 0.78rem;
        font-family: 'Sora', sans-serif;
        color      : #EF4444;
    }

    :host([disabled]) .rb-wrap {
        opacity       : 0.45;
        cursor        : not-allowed;
        pointer-events: none;
    }

    :host([invalid]) .rb-circle,
    .has-error .rb-circle {
        border-color: #EF4444;
    }

    :host([size="sm"]) .rb-circle        { width: 14px; height: 14px; }
    :host([size="sm"]) .rb-circle::after { width:  6px; height:  6px; }
    :host([size="sm"]) .rb-label         { font-size: 0.75rem; }
    :host([size="lg"]) .rb-circle        { width: 22px; height: 22px; }
    :host([size="lg"]) .rb-circle::after { width: 10px; height: 10px; }
    :host([size="lg"]) .rb-label         { font-size: 1rem; }
"""

[<LitElement("fui-radio-group")>]
let FuiRadioGroup() =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ groupStyles ]
        init.props  <- {|
            options   = Prop.Of("[]",  attribute = "options")
            value     = Prop.Of("",    attribute = "value")
            label     = Prop.Of("",    attribute = "label")
            fieldName = Prop.Of("",    attribute = "name")
            disabled  = Prop.Of(false, attribute = "disabled")
            error     = Prop.Of("",    attribute = "error")
            size      = Prop.Of("md",  attribute = "size")
        |}
    )

    let currentValue, setValue = Hook.useState(props.value.Value)

    let opts : {| value: string; label: string |} array =
        try jsonParse props.options.Value
        with _ -> [||]

    let hasError = props.error.Value <> ""
    // All inputs share this name so the browser coordinates them within the shadow root.
    let groupName = if props.fieldName.Value <> "" then props.fieldName.Value else "rg"

    let onChange = Ev (fun ev ->
        let v = getValue (ev :> obj)
        setValue v
        host.dispatchCustomEvent("fui-change", detail = {| value = v |}))

    let fieldLabel =
        if props.label.Value <> "" then
            html $"""<span class="field-label" part="label">{props.label.Value}</span>"""
        else
            Lit.nothing

    let errorPart =
        if hasError then
            html $"""<span class="error-msg" part="error" role="alert">{props.error.Value}</span>"""
        else
            Lit.nothing

    let renderOpt (o: {| value: string; label: string |}) =
        html $"""
            <label class="rb-wrap" part="option">
                <input
                    type="radio"
                    name={groupName}
                    value={o.value}
                    .checked={currentValue = o.value}
                    ?disabled={props.disabled.Value}
                    @change={onChange}
                />
                <span class="rb-circle" part="circle"></span>
                <span class="rb-label">{o.label}</span>
            </label>
        """

    let optItems =
        Lit.mapUnique (fun (o: {| value: string; label: string |}) -> o.value) renderOpt opts

    html $"""
        <div class={if hasError then "field has-error" else "field"} part="field"
             role="radiogroup" aria-label={props.label.Value}>
            {fieldLabel}
            <div class="options" part="options">
                {optItems}
            </div>
            {errorPart}
        </div>
    """

let register () = ()
