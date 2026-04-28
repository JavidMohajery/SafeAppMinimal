module WebComponents.FuiCombobox

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("JSON.parse($0)")>]
let private parseOpts (_: string) : {| value: string; label: string |} array = jsNative

[<Emit("$0.key")>]
let private evKey (_: obj) : string = jsNative

[<Emit("$0.target.value")>]
let private evTargetValue (_: obj) : string = jsNative

[<Emit("$0.preventDefault()")>]
let private preventDefault (_: obj) : unit = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-cb-font, 'JetBrains Mono', monospace);
        min-width  : 0;
        position   : relative;
        width      : 100%;
    }
    :host([hidden]) { display: none; }

    .field {
        display       : flex;
        flex-direction: column;
        gap           : 0.35rem;
    }

    label {
        color         : var(--fui-cb-label-color, #6E6E76);
        display       : block;
        font-size     : 0.72rem;
        font-weight   : 600;
        letter-spacing: 0.06em;
        text-transform: uppercase;
    }

    .input-wrap { display: block; position: relative; }

    .cb-input {
        background   : var(--fui-cb-bg,     #1E1E21);
        border       : 1px solid var(--fui-cb-border, #2A2A2E);
        border-radius: var(--fui-cb-radius,  6px);
        box-sizing   : border-box;
        color        : var(--fui-cb-color,  #E8E8ED);
        cursor       : text;
        display      : block;
        font-family  : inherit;
        font-size    : 0.875rem;
        outline      : none;
        padding      : 0.5rem 2.5rem 0.5rem 0.75rem;
        transition   : border-color 0.12s;
        width        : 100%;
    }
    .cb-input::placeholder { color: var(--fui-cb-placeholder, #3A3A3E); }
    .cb-input:focus         { border-color: var(--fui-cb-border-focus, #7C3AED); }
    .cb-input:focus-visible { outline: 2px solid var(--fui-cb-border-focus, #7C3AED); outline-offset: 2px; }

    .arrow {
        align-items   : center;
        color         : var(--fui-cb-arrow-color, #6E6E76);
        display       : flex;
        font-size     : 0.7rem;
        pointer-events: none;
        position      : absolute;
        right         : 0.75rem;
        top           : 50%;
        transform     : translateY(-50%);
        transition    : transform 0.15s;
    }
    .arrow.open { transform: translateY(-50%) rotate(180deg); }

    /* Full-viewport overlay catches outside clicks */
    .overlay {
        display : none;
        inset   : 0;
        position: fixed;
        z-index : 9998;
    }
    .overlay.open { display: block; }

    .dropdown {
        background   : var(--fui-cb-bg,     #1E1E21);
        border       : 1px solid var(--fui-cb-border, #2A2A2E);
        border-radius: var(--fui-cb-radius,  6px);
        box-shadow   : 0 8px 24px rgba(0,0,0,0.4);
        display      : none;
        left         : 0;
        max-height   : 220px;
        overflow-y   : auto;
        padding      : 0.25rem 0;
        position     : absolute;
        right        : 0;
        top          : calc(100% + 4px);
        z-index      : 9999;
    }
    .dropdown.open { display: block; }

    .opt {
        background : none;
        border     : none;
        color      : var(--fui-cb-item-color, #E8E8ED);
        cursor     : pointer;
        display    : block;
        font-family: inherit;
        font-size  : 0.875rem;
        padding    : 0.5rem 0.875rem;
        text-align : left;
        transition : background 0.1s;
        width      : 100%;
    }
    .opt:hover, .opt.hl { background: rgba(124,58,237,0.12); color: var(--fui-cb-accent, #9B59F5); }

    .no-results {
        color    : #4A4A52;
        font-size: 0.8rem;
        padding  : 0.75rem;
        text-align: center;
    }

    :host([disabled]) .cb-input { cursor: not-allowed; opacity: 0.45; }
    .has-error .cb-input         { border-color: #EF4444; }

    .error-msg {
        color      : #EF4444;
        font-family: 'Sora', sans-serif;
        font-size  : 0.78rem;
    }

    :host([size="sm"]) .cb-input { font-size: 0.75rem;  padding: 0.3rem  2.25rem 0.3rem  0.6rem;  }
    :host([size="lg"]) .cb-input { font-size: 1rem;     padding: 0.65rem 2.75rem 0.65rem 0.9rem;  }
"""

[<LitElement("fui-combobox")>]
let FuiCombobox () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            value       = Prop.Of("",    attribute = "value")
            label       = Prop.Of("",    attribute = "label")
            placeholder = Prop.Of("",    attribute = "placeholder")
            options     = Prop.Of("[]",  attribute = "options")
            disabled    = Prop.Of(false, attribute = "disabled")
            required    = Prop.Of(false, attribute = "required")
            error       = Prop.Of("",    attribute = "error")
            fieldName   = Prop.Of("",    attribute = "name")
            size        = Prop.Of("md",  attribute = "size")
        |}
    )

    let opts : {| value: string; label: string |} array =
        try parseOpts props.options.Value with _ -> [||]

    let initLabel =
        opts |> Array.tryFind (fun o -> o.value = props.value.Value)
             |> Option.map (fun o -> o.label)
             |> Option.defaultValue ""

    let selectedVal,   setSelectedVal   = Hook.useState props.value.Value
    let selectedLabel, setSelectedLabel = Hook.useState initLabel
    let isOpen,        setIsOpen        = Hook.useState false
    let search,        setSearch        = Hook.useState ""
    let hlIdx,         setHlIdx         = Hook.useState 0

    let filtered =
        if search = "" then opts
        else
            let q = search.ToLower()
            opts |> Array.filter (fun o -> o.label.ToLower().Contains(q) || o.value.ToLower().Contains(q))

    let close () =
        setIsOpen false
        setSearch ""
        setHlIdx 0

    let select (o: {| value: string; label: string |}) =
        setSelectedVal   o.value
        setSelectedLabel o.label
        close ()
        host.dispatchCustomEvent("fui-change", detail = {| value = o.value; label = o.label |})

    let onFocus _ =
        if not props.disabled.Value then
            setIsOpen true
            setSearch ""
            setHlIdx 0

    let onInput (e: obj) =
        setSearch (evTargetValue e)
        setHlIdx 0

    let onKeyDown (e: obj) =
        match evKey e with
        | "ArrowDown" ->
            preventDefault e
            setHlIdx (min (hlIdx + 1) (filtered.Length - 1))
        | "ArrowUp"   ->
            preventDefault e
            setHlIdx (max (hlIdx - 1) 0)
        | "Enter"     ->
            preventDefault e
            if filtered.Length > 0 then select filtered.[min hlIdx (filtered.Length - 1)]
        | "Escape"    -> close ()
        | "Tab"       -> close ()
        | _ -> ()

    let hasError   = props.error.Value <> ""
    let arrowCls   = if isOpen then "arrow open" else "arrow"
    let overlayCls = if isOpen then "overlay open" else "overlay"
    let dropdownCls= if isOpen then "dropdown open" else "dropdown"

    let labelPart =
        if props.label.Value <> "" then html $"""<label part="label" for="cb">{props.label.Value}</label>"""
        else Lit.nothing

    let errorPart =
        if hasError then html $"""<span class="error-msg" part="error" role="alert">{props.error.Value}</span>"""
        else Lit.nothing

    let renderOpt (i: int) (o: {| value: string; label: string |}) =
        let cls = if i = hlIdx then "opt hl" else "opt"
        html $"""<button class={cls} role="option" aria-selected={string (o.value = selectedVal)} @click={Ev (fun _ -> select o)}>{o.label}</button>"""

    let dropdownContent =
        if filtered.Length = 0 then
            [| html $"""<div class="no-results">No results</div>""" |]
        else
            filtered |> Array.mapi renderOpt

    let displayValue = if isOpen then search else selectedLabel

    html $"""
        <div class={if hasError then "field has-error" else "field"} part="field">
            {labelPart}
            <div class="input-wrap" part="input-wrap">
                <div class={overlayCls} @click={Ev (fun _ -> close ())}></div>
                <input
                    class="cb-input"
                    part="input"
                    id="cb"
                    type="text"
                    role="combobox"
                    aria-expanded={string isOpen}
                    aria-autocomplete="list"
                    name={props.fieldName.Value}
                    placeholder={if isOpen then props.placeholder.Value else if selectedLabel = "" then props.placeholder.Value else ""}
                    .value={displayValue}
                    ?disabled={props.disabled.Value}
                    ?required={props.required.Value}
                    @focus={Ev onFocus}
                    @input={Ev onInput}
                    @keydown={Ev onKeyDown}
                    autocomplete="off" />
                <span class={arrowCls} aria-hidden="true">▾</span>
                <div class={dropdownCls} role="listbox">
                    {dropdownContent}
                </div>
            </div>
            {errorPart}
        </div>
    """

let register () = ()
