module WebComponents.FuiFileUpload

open Lit
open Fable.Core
open Browser.Types

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("Array.from(($0.target&&$0.target.files)||[]).map(function(f){return f.name})")>]
let private getInputFileNames (e: obj) : string array = jsNative

[<Emit("Array.from(($0.dataTransfer&&$0.dataTransfer.files)||[]).map(function(f){return f.name})")>]
let private getDropFileNames (e: obj) : string array = jsNative

// True when the cursor has left the dropzone itself (not just moved to a child element).
[<Emit("!$0.currentTarget.contains($0.relatedTarget)")>]
let private isLeavingDropzone (e: obj) : bool = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-fu-font, 'JetBrains Mono', monospace);
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
        color          : var(--fui-fu-label-color, #6E6E76);
    }

    /* Styled as the drop target; <label for="fu-inp"> so any click opens the picker. */
    .dropzone {
        display      : block;
        border       : 2px dashed var(--fui-fu-border, #2A2A2E);
        border-radius: var(--fui-fu-radius, 8px);
        padding      : 2rem 1.5rem;
        text-align   : center;
        cursor       : pointer;
        transition   : border-color 0.15s, background 0.15s;
        background   : var(--fui-fu-bg, transparent);
    }

    .dropzone:hover {
        border-color: var(--fui-fu-border-hover, #3D3D43);
    }

    .dropzone.drag-over {
        border-color: var(--fui-fu-border-active, #7C3AED);
        background  : var(--fui-fu-bg-active, rgba(124,58,237,0.06));
    }

    .has-error .dropzone {
        border-color: #EF4444;
    }

    .dropzone-icon {
        color          : var(--fui-fu-icon-color, #6E6E76);
        margin         : 0 auto 0.75rem;
        display        : flex;
        align-items    : center;
        justify-content: center;
    }

    .dropzone-icon svg { width: 2rem; height: 2rem; }

    .dropzone-primary {
        font-size  : 0.875rem;
        font-weight: 600;
        color      : var(--fui-fu-text-color, #E8E8ED);
        margin     : 0 0 0.25rem;
        line-height: 1.5;
    }

    .browse-link { color: var(--fui-fu-accent, #7C3AED); }

    .accept-hint {
        font-size  : 0.72rem;
        font-family: 'Sora', sans-serif;
        color      : var(--fui-fu-hint-color, #4A4A52);
        margin     : 0;
    }

    /* Visually hidden; triggered through the <label for> association. */
    input[type="file"] {
        position      : absolute;
        opacity       : 0;
        width         : 0;
        height        : 0;
        pointer-events: none;
    }

    .file-list {
        list-style    : none;
        margin        : 0;
        padding       : 0;
        display       : flex;
        flex-direction: column;
        gap           : 0.3rem;
    }

    .file-item {
        display      : flex;
        align-items  : center;
        gap          : 0.5rem;
        font-size    : 0.8rem;
        font-family  : 'Sora', sans-serif;
        color        : var(--fui-fu-file-color, #E8E8ED);
        background   : var(--fui-fu-file-bg, #1E1E21);
        border       : 1px solid var(--fui-fu-border, #2A2A2E);
        border-radius: 4px;
        padding      : 0.3rem 0.65rem;
        word-break   : break-all;
    }

    .file-item::before {
        content    : "·";
        color      : #7C3AED;
        font-size  : 1.4rem;
        line-height: 0;
        flex-shrink: 0;
    }

    .error-msg {
        font-size  : 0.78rem;
        font-family: 'Sora', sans-serif;
        color      : #EF4444;
    }

    :host([disabled]) .dropzone {
        opacity       : 0.45;
        cursor        : not-allowed;
        pointer-events: none;
    }

    /* sm */
    :host([size="sm"]) .dropzone          { padding: 1.25rem 1rem; }
    :host([size="sm"]) .dropzone-icon svg { width: 1.5rem; height: 1.5rem; }
    :host([size="sm"]) .dropzone-primary  { font-size: 0.78rem; }
    :host([size="sm"]) .accept-hint       { font-size: 0.68rem; }

    /* lg */
    :host([size="lg"]) .dropzone          { padding: 3rem 2rem; }
    :host([size="lg"]) .dropzone-icon svg { width: 2.5rem; height: 2.5rem; }
    :host([size="lg"]) .dropzone-primary  { font-size: 1rem; }
    :host([size="lg"]) .accept-hint       { font-size: 0.78rem; }
"""

[<LitElement("fui-file-upload")>]
let FuiFileUpload() =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            label     = Prop.Of("",    attribute = "label")
            accept    = Prop.Of("",    attribute = "accept")
            multiple  = Prop.Of(false, attribute = "multiple")
            disabled  = Prop.Of(false, attribute = "disabled")
            fieldName = Prop.Of("",    attribute = "name")
            size      = Prop.Of("md",  attribute = "size")
            error     = Prop.Of("",    attribute = "error")
        |}
    )

    let isDragOver, setDragOver = Hook.useState false
    let fileNames,  setFileNames = Hook.useState ([] : string list)

    let onDragOver = Ev (fun (ev: Event) ->
        ev.preventDefault()
        if not isDragOver then setDragOver true)

    let onDragLeave = Ev (fun (ev: Event) ->
        if isLeavingDropzone (ev :> obj) then setDragOver false)

    let onDrop = Ev (fun (ev: Event) ->
        ev.preventDefault()
        setDragOver false
        let names = getDropFileNames (ev :> obj)
        setFileNames (names |> Array.toList)
        host.dispatchCustomEvent("fui-change", detail = {| files = names; count = names.Length |}))

    let onFileChange = Ev (fun (ev: Event) ->
        let names = getInputFileNames (ev :> obj)
        setFileNames (names |> Array.toList)
        host.dispatchCustomEvent("fui-change", detail = {| files = names; count = names.Length |}))

    let hasError   = props.error.Value <> ""
    let dropClass  = if isDragOver then "dropzone drag-over" else "dropzone"
    let fieldClass = if hasError then "field has-error" else "field"

    let labelPart =
        if props.label.Value <> "" then
            html $"""<span class="field-label" part="label">{props.label.Value}</span>"""
        else Lit.nothing

    let acceptHint =
        if props.accept.Value <> "" then
            html $"""<p class="accept-hint">{props.accept.Value}</p>"""
        else Lit.nothing

    let errorPart =
        if hasError then
            html $"""<span class="error-msg" part="error" role="alert">{props.error.Value}</span>"""
        else Lit.nothing

    let renderFile (name: string) =
        html $"""<li class="file-item" part="file-item">{name}</li>"""

    let fileListPart =
        if fileNames.IsEmpty then Lit.nothing
        else
            let items = Lit.mapUnique (fun (n: string) -> n) renderFile (fileNames |> List.toArray)
            html $"""<ul class="file-list" part="file-list">{items}</ul>"""

    // Upload arrow icon — extracted to a named let (cannot nest html $"" inside an interpolation hole).
    let uploadIcon =
        html $"""
            <span class="dropzone-icon" aria-hidden="true">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
                    <path d="M12 15V3m-4 4 4-4 4 4"/>
                    <path d="M20 16v2a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2v-2"/>
                </svg>
            </span>
        """

    html $"""
        <div class={fieldClass} part="field">
            {labelPart}
            <label
                for="fu-inp"
                class={dropClass}
                part="dropzone"
                @dragover={onDragOver}
                @dragleave={onDragLeave}
                @drop={onDrop}
            >
                {uploadIcon}
                <p class="dropzone-primary">
                    <span class="browse-link">Click to browse</span> or drag and drop
                </p>
                {acceptHint}
            </label>
            <input
                type="file"
                id="fu-inp"
                part="input"
                name={props.fieldName.Value}
                accept={props.accept.Value}
                ?multiple={props.multiple.Value}
                ?disabled={props.disabled.Value}
                @change={onFileChange}
            />
            {fileListPart}
            {errorPart}
        </div>
    """

let register () = ()
