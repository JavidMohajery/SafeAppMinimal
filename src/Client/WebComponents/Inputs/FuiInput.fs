module WebComponents.FuiInput

open Lit
open Browser.Types
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        width      : 100%;
        min-width  : 0;
        font-family: var(--fui-input-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    .field {
        display       : flex;
        flex-direction: column;
        gap           : 0.35rem;
    }

    label {
        display      : block;
        font-size    : 0.72rem;
        font-weight  : 600;
        letter-spacing: 0.06em;
        text-transform: uppercase;
        color        : var(--fui-input-label-color, #6E6E76);
    }

    input {
        display      : block;
        width        : 100%;
        box-sizing   : border-box;
        background   : var(--fui-input-bg, #1E1E21);
        color        : var(--fui-input-color, #E8E8ED);
        border       : 1px solid var(--fui-input-border, #2A2A2E);
        border-radius: var(--fui-input-radius, 6px);
        padding      : 0.5rem 0.75rem;
        font-family  : inherit;
        font-size    : 0.875rem;
        line-height  : 1.4;
        outline      : none;
        transition   : border-color 0.12s;
    }

    input::placeholder {
        color: var(--fui-input-placeholder, #6E6E76);
    }

    input:focus {
        border-color: var(--fui-input-border-focus, #7C3AED);
    }

    input:focus-visible {
        outline       : 2px solid var(--fui-input-border-focus, #7C3AED);
        outline-offset: 2px;
    }

    :host([disabled]) input {
        opacity: 0.45;
        cursor : not-allowed;
    }

    :host([invalid]) input,
    .has-error input {
        border-color: #EF4444;
    }

    .error-msg {
        font-size  : 0.78rem;
        font-family: 'Sora', sans-serif;
        color      : #EF4444;
    }

    /* Sizes */
    :host([size="sm"]) input { font-size: 0.75rem;  padding: 0.3rem  0.6rem;  }
    :host([size="md"]) input { font-size: 0.875rem; padding: 0.5rem  0.75rem; }
    :host([size="lg"]) input { font-size: 1rem;     padding: 0.65rem 0.9rem;  }
"""

[<LitElement("fui-input")>]
let FuiInput() =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            value       = Prop.Of("",     attribute = "value")
            placeholder = Prop.Of("",     attribute = "placeholder")
            label       = Prop.Of("",     attribute = "label")
            inputType   = Prop.Of("text", attribute = "type")
            disabled    = Prop.Of(false,  attribute = "disabled")
            required    = Prop.Of(false,  attribute = "required")
            error       = Prop.Of("",     attribute = "error")
            fieldName   = Prop.Of("",     attribute = "name")
            size        = Prop.Of("md",   attribute = "size")
        |}
    )

    let currentVal, setVal = Hook.useState(props.value.Value)

    let fireEvent (name: string) (value: string) =
        host.dispatchCustomEvent(name, detail = {| value = value |})

    let onInput (e: Event) =
        let v = (e.target :?> HTMLInputElement).value
        setVal v
        fireEvent "fui-input" v

    let onChange (e: Event) =
        let v = (e.target :?> HTMLInputElement).value
        fireEvent "fui-change" v

    let hasError = props.error.Value <> ""

    let labelPart =
        if props.label.Value <> "" then
            html $"""<label part="label" for="inp">{props.label.Value}</label>"""
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
            <input
                part="input"
                id="inp"
                type={props.inputType.Value}
                .value={currentVal}
                placeholder={props.placeholder.Value}
                name={props.fieldName.Value}
                ?disabled={props.disabled.Value}
                ?required={props.required.Value}
                @input={Ev onInput}
                @change={Ev onChange}
            />
            {errorPart}
        </div>
    """

let register () = ()
