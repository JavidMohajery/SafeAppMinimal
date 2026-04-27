module WebComponents.FuiTextarea

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        width      : 100%;
        min-width  : 0;
        font-family: var(--fui-textarea-font, 'JetBrains Mono', monospace);
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
        color          : var(--fui-textarea-label-color, #6E6E76);
    }

    textarea {
        display      : block;
        width        : 100%;
        box-sizing   : border-box;
        background   : var(--fui-textarea-bg, #1E1E21);
        color        : var(--fui-textarea-color, #E8E8ED);
        border       : 1px solid var(--fui-textarea-border, #2A2A2E);
        border-radius: var(--fui-textarea-radius, 6px);
        padding      : 0.5rem 0.75rem;
        font-family  : inherit;
        font-size    : 0.875rem;
        line-height  : 1.6;
        outline      : none;
        transition   : border-color 0.12s;
        min-height   : 80px;
    }

    textarea::placeholder {
        color: var(--fui-textarea-placeholder, #6E6E76);
    }

    textarea:focus {
        border-color: var(--fui-textarea-border-focus, #7C3AED);
    }

    textarea:focus-visible {
        outline       : 2px solid var(--fui-textarea-border-focus, #7C3AED);
        outline-offset: 2px;
    }

    :host([disabled]) textarea {
        opacity: 0.45;
        cursor : not-allowed;
    }

    :host([invalid]) textarea,
    .has-error textarea {
        border-color: #EF4444;
    }

    .error-msg {
        font-size  : 0.78rem;
        font-family: 'Sora', sans-serif;
        color      : #EF4444;
    }

    /* Sizes */
    :host([size="sm"]) textarea { font-size: 0.75rem;  padding: 0.3rem  0.6rem;  }
    :host([size="md"]) textarea { font-size: 0.875rem; padding: 0.5rem  0.75rem; }
    :host([size="lg"]) textarea { font-size: 1rem;     padding: 0.65rem 0.9rem;  }
"""

[<LitElement("fui-textarea")>]
let FuiTextarea() =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            value       = Prop.Of("",         attribute = "value")
            placeholder = Prop.Of("",         attribute = "placeholder")
            label       = Prop.Of("",         attribute = "label")
            rows        = Prop.Of(3,          attribute = "rows")
            resize      = Prop.Of("vertical", attribute = "resize")
            disabled    = Prop.Of(false,      attribute = "disabled")
            required    = Prop.Of(false,      attribute = "required")
            error       = Prop.Of("",         attribute = "error")
            fieldName   = Prop.Of("",         attribute = "name")
            size        = Prop.Of("md",       attribute = "size")
        |}
    )

    let currentVal, setVal = Hook.useState(props.value.Value)

    let fireEvent (eventName: string) (value: string) =
        host.dispatchCustomEvent(eventName, detail = {| value = value |})

    let onInput  = EvVal (fun v -> setVal v; fireEvent "fui-input" v)
    let onChange = EvVal (fun v -> fireEvent "fui-change" v)

    let hasError = props.error.Value <> ""

    let labelPart =
        if props.label.Value <> "" then
            html $"""<label part="label" for="ta">{props.label.Value}</label>"""
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
            <textarea
                part="textarea"
                id="ta"
                rows={string props.rows.Value}
                placeholder={props.placeholder.Value}
                name={props.fieldName.Value}
                style={Lit.styles {| resize = props.resize.Value |}}
                ?disabled={props.disabled.Value}
                ?required={props.required.Value}
                @input={onInput}
                @change={onChange}
                .value={currentVal}
            ></textarea>
            {errorPart}
        </div>
    """

let register () = ()
