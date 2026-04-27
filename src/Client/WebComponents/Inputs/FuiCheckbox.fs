module WebComponents.FuiCheckbox

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
        width      : 100%;
        min-width  : 0;
        font-family: var(--fui-cb-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    .field {
        display       : flex;
        flex-direction: column;
        gap           : 0.25rem;
    }

    .cb-wrap {
        display    : inline-flex;
        align-items: center;
        gap        : 0.55rem;
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

    .cb-box {
        flex-shrink    : 0;
        position       : relative;
        width          : 18px;
        height         : 18px;
        border         : 1.5px solid var(--fui-cb-border, #2A2A2E);
        border-radius  : var(--fui-cb-radius, 4px);
        background     : var(--fui-cb-bg, #1E1E21);
        transition     : background 0.12s, border-color 0.12s;
    }

    .cb-box::after {
        content     : "";
        position    : absolute;
        display     : none;
        left        : 4px;
        top         : 1px;
        width       : 5px;
        height      : 9px;
        border      : 2px solid #fff;
        border-top  : none;
        border-left : none;
        transform   : rotate(45deg);
    }

    input:checked + .cb-box {
        background  : var(--fui-cb-checked-bg, #7C3AED);
        border-color: var(--fui-cb-checked-bg, #7C3AED);
    }

    input:checked + .cb-box::after {
        display: block;
    }

    input:focus-visible + .cb-box {
        outline       : 2px solid var(--fui-cb-border-focus, #7C3AED);
        outline-offset: 2px;
    }

    .cb-label {
        font-size  : 0.875rem;
        color      : var(--fui-cb-label-color, #E8E8ED);
        line-height: 1.4;
    }

    .error-msg {
        font-size  : 0.78rem;
        font-family: 'Sora', sans-serif;
        color      : #EF4444;
        padding-left: 1.55rem;
    }

    :host([disabled]) .cb-wrap {
        opacity       : 0.45;
        cursor        : not-allowed;
        pointer-events: none;
    }

    :host([invalid]) .cb-box,
    .has-error .cb-box {
        border-color: #EF4444;
    }

    /* Sizes */
    :host([size="sm"]) .cb-box         { width: 14px; height: 14px; }
    :host([size="sm"]) .cb-box::after  { width: 3px; height: 7px; left: 3px; top: 0px; }
    :host([size="sm"]) .cb-label       { font-size: 0.75rem; }
    :host([size="sm"]) .error-msg      { padding-left: 1.25rem; }
    :host([size="lg"]) .cb-box         { width: 22px; height: 22px; }
    :host([size="lg"]) .cb-box::after  { width: 6px; height: 11px; left: 5px; top: 2px; }
    :host([size="lg"]) .cb-label       { font-size: 1rem; }
    :host([size="lg"]) .error-msg      { padding-left: 1.9rem; }
"""

[<LitElement("fui-checkbox")>]
let FuiCheckbox() =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            isChecked = Prop.Of(false, attribute = "checked")
            label     = Prop.Of("",   attribute = "label")
            disabled  = Prop.Of(false, attribute = "disabled")
            fieldName = Prop.Of("",   attribute = "name")
            size      = Prop.Of("md", attribute = "size")
            error     = Prop.Of("",   attribute = "error")
        |}
    )

    let currentChecked, setChecked = Hook.useState(props.isChecked.Value)

    let onChange = Ev (fun ev ->
        let v = getChecked (ev :> obj)
        setChecked v
        host.dispatchCustomEvent("fui-change", detail = {| ``checked`` = v |}))

    let hasError = props.error.Value <> ""

    let labelPart =
        if props.label.Value <> "" then
            html $"""<span class="cb-label" part="label">{props.label.Value}</span>"""
        else
            Lit.nothing

    let errorPart =
        if hasError then
            html $"""<span class="error-msg" part="error" role="alert">{props.error.Value}</span>"""
        else
            Lit.nothing

    html $"""
        <div class={if hasError then "field has-error" else "field"} part="field">
            <label class="cb-wrap" part="wrap">
                <input
                    type="checkbox"
                    name={props.fieldName.Value}
                    .checked={currentChecked}
                    ?disabled={props.disabled.Value}
                    @change={onChange}
                />
                <span class="cb-box" part="box"></span>
                {labelPart}
            </label>
            {errorPart}
        </div>
    """

let register () = ()
