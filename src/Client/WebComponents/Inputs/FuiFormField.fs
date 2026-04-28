module WebComponents.FuiFormField

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-ff-font, 'JetBrains Mono', monospace);
        min-width  : 0;
        width      : 100%;
    }
    :host([hidden]) { display: none; }

    .field {
        display       : flex;
        flex-direction: column;
        gap           : 0.35rem;
    }

    .label-row {
        align-items: baseline;
        display    : flex;
        gap        : 0.25rem;
    }

    .label {
        color         : var(--fui-ff-label-color, #6E6E76);
        display       : block;
        font-size     : 0.72rem;
        font-weight   : 600;
        letter-spacing: 0.06em;
        text-transform: uppercase;
    }

    .required {
        color    : #EF4444;
        font-size: 0.72rem;
    }

    .control { display: block; }

    .hint {
        color      : var(--fui-ff-hint-color, #6E6E76);
        font-family: 'Sora', sans-serif;
        font-size  : 0.78rem;
        line-height: 1.4;
    }

    .error-msg {
        color      : var(--fui-ff-error-color, #EF4444);
        font-family: 'Sora', sans-serif;
        font-size  : 0.78rem;
        line-height: 1.4;
    }
"""

[<LitElement("fui-form-field")>]
let FuiFormField () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            label    = Prop.Of("",    attribute = "label")
            hint     = Prop.Of("",    attribute = "hint")
            error    = Prop.Of("",    attribute = "error")
            required = Prop.Of(false, attribute = "required")
        |}
    )

    let labelPart =
        if props.label.Value <> "" then
            let req =
                if props.required.Value then html $"""<span class="required" aria-hidden="true">*</span>"""
                else Lit.nothing
            html $"""
                <div class="label-row">
                    <span class="label" part="label">{props.label.Value}</span>
                    {req}
                </div>"""
        else Lit.nothing

    let belowPart =
        if props.error.Value <> "" then
            html $"""<span class="error-msg" part="error" role="alert">{props.error.Value}</span>"""
        elif props.hint.Value <> "" then
            html $"""<span class="hint" part="hint">{props.hint.Value}</span>"""
        else Lit.nothing

    html $"""
        <div class="field" part="field">
            {labelPart}
            <div class="control" part="control">
                <slot></slot>
            </div>
            {belowPart}
        </div>
    """

let register () = ()
