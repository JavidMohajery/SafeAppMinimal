module WebComponents.FuiCommandPalette

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("JSON.parse($0)")>]
let private parseItems (_: string) : {| id: string; label: string; description: string; group: string |} array = jsNative

[<Emit("$0.key")>]
let private evKey (_: obj) : string = jsNative

[<Emit("$0.target.value")>]
let private evTargetValue (_: obj) : string = jsNative

[<Emit("$0.preventDefault()")>]
let private preventDefault (_: obj) : unit = jsNative

let private styles = unsafeCSS """
    :host {
        display    : inline-block;
        font-family: var(--fui-cp-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    /* ── Trigger ────────────────────────────────────────────────────────────── */
    .trigger {
        align-items  : center;
        background   : #1E1E21;
        border       : 1px solid #2A2A2E;
        border-radius: 6px;
        color        : #6E6E76;
        cursor       : pointer;
        display      : inline-flex;
        font-family  : inherit;
        font-size    : 0.875rem;
        gap          : 0.5rem;
        padding      : 0.4rem 0.875rem;
        transition   : border-color 0.15s, color 0.15s;
        user-select  : none;
    }
    .trigger:hover { border-color: #7C3AED; color: #E8E8ED; }
    .trigger:focus-visible { outline: 2px solid #7C3AED; outline-offset: 2px; }
    .trigger-kbd {
        background   : #2A2A2E;
        border-radius: 3px;
        font-size    : 0.7rem;
        padding      : 0.1em 0.4em;
    }

    /* ── Backdrop ───────────────────────────────────────────────────────────── */
    .backdrop {
        background: rgba(0,0,0,0.7);
        display   : none;
        inset     : 0;
        position  : fixed;
        z-index   : 999;
    }
    .backdrop.open { display: block; }

    /* ── Panel ──────────────────────────────────────────────────────────────── */
    .panel {
        background    : #161618;
        border        : 1px solid #2A2A2E;
        border-radius : 10px;
        box-shadow    : 0 16px 48px rgba(0,0,0,0.65);
        display       : none;
        flex-direction: column;
        left          : 50%;
        max-height    : 480px;
        max-width     : calc(100vw - 2rem);
        position      : fixed;
        top           : 15vh;
        transform     : translateX(-50%);
        width         : 560px;
        z-index       : 1000;
    }
    .panel.open { display: flex; }

    /* ── Search ─────────────────────────────────────────────────────────────── */
    .search-wrap {
        align-items : center;
        border-bottom: 1px solid #2A2A2E;
        display     : flex;
        flex-shrink : 0;
        gap         : 0.625rem;
        padding     : 0.75rem 1rem;
    }
    .search-icon { color: #4A4A52; flex-shrink: 0; font-size: 1rem; }
    .search-input {
        background : none;
        border     : none;
        color      : #E8E8ED;
        flex       : 1;
        font-family: 'Sora', sans-serif;
        font-size  : 1rem;
        outline    : none;
    }
    .search-input::placeholder { color: #3A3A3E; }

    /* ── Results ────────────────────────────────────────────────────────────── */
    .results { flex: 1; overflow-y: auto; padding: 0.25rem 0; }

    .group-label {
        color         : #3A3A3E;
        font-size     : 0.65rem;
        font-weight   : 700;
        letter-spacing: 0.08em;
        padding       : 0.625rem 1rem 0.25rem;
        text-transform: uppercase;
    }

    .cp-item {
        align-items  : center;
        background   : none;
        border       : none;
        cursor       : pointer;
        display      : flex;
        gap          : 0.75rem;
        padding      : 0.5rem 1rem;
        text-align   : left;
        transition   : background 0.1s;
        width        : 100%;
    }
    .cp-item:hover,
    .cp-item.selected { background: rgba(124,58,237,0.12); outline: none; }

    .item-label {
        color      : #E8E8ED;
        font-family: 'JetBrains Mono', monospace;
        font-size  : 0.875rem;
        font-weight: 600;
    }
    .item-desc {
        color      : #6E6E76;
        font-family: 'Sora', sans-serif;
        font-size  : 0.8rem;
        margin-top : 0.1rem;
    }

    .empty {
        color     : #4A4A52;
        font-size : 0.875rem;
        padding   : 2.5rem 1rem;
        text-align: center;
    }

    /* ── Footer ─────────────────────────────────────────────────────────────── */
    .footer {
        border-top: 1px solid #2A2A2E;
        color     : #3A3A3E;
        display   : flex;
        flex-shrink: 0;
        font-size : 0.7rem;
        gap       : 1rem;
        padding   : 0.5rem 1rem;
    }
"""

[<LitElement("fui-command-palette")>]
let FuiCommandPalette () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            triggerLabel = Prop.Of("Open palette", attribute = "trigger-label")
            placeholder  = Prop.Of("Search...",    attribute = "placeholder")
            items        = Prop.Of("[]",           attribute = "items")
        |}
    )

    let isOpen, setIsOpen = Hook.useState false
    let search,  setSearch  = Hook.useState ""
    let selIdx,  setSelIdx  = Hook.useState 0

    let allItems = parseItems props.items.Value

    let filtered =
        if search = "" then allItems
        else
            let q = search.ToLower()
            allItems |> Array.filter (fun it ->
                it.label.ToLower().Contains(q) || it.description.ToLower().Contains(q))

    let close _ = setIsOpen false; setSearch ""; setSelIdx 0

    let open_ _ = setIsOpen true; setSearch ""; setSelIdx 0

    let onInput (e: obj) =
        setSearch (evTargetValue e)
        setSelIdx 0

    let onKeyDown (e: obj) =
        match evKey e with
        | "ArrowDown" ->
            preventDefault e
            setSelIdx (min (selIdx + 1) (filtered.Length - 1))
        | "ArrowUp" ->
            preventDefault e
            setSelIdx (max (selIdx - 1) 0)
        | "Enter" ->
            if filtered.Length > 0 then
                let item = filtered.[min selIdx (filtered.Length - 1)]
                setIsOpen false
                host.dispatchCustomEvent("fui-select", detail = {| id = item.id; label = item.label |})
        | "Escape" -> close ()
        | _ -> ()

    let renderResults () =
        if filtered.Length = 0 then
            [| html $"""<div class="empty">No results for "{search}"</div>""" |]
        else
            filtered
            |> Array.indexed
            |> Array.collect (fun (i, item) ->
                let cls      = if i = selIdx then "cp-item selected" else "cp-item"
                let onSelect _ =
                    setIsOpen false
                    host.dispatchCustomEvent("fui-select", detail = {| id = item.id; label = item.label |})
                let descEl =
                    if item.description <> "" then html $"<div class=\"item-desc\">{item.description}</div>"
                    else Lit.nothing
                [|
                    if item.group <> "" && (i = 0 || filtered.[i-1].group <> item.group) then
                        yield html $"<div class=\"group-label\">{item.group}</div>"
                    yield html $"""
                        <button class={cls} role="option" @click={Ev onSelect}>
                            <div>
                                <div class="item-label">{item.label}</div>
                                {descEl}
                            </div>
                        </button>"""
                |])

    let backdropCls = if isOpen then "backdrop open" else "backdrop"
    let panelCls    = if isOpen then "panel open"    else "panel"

    html $"""
        <button class="trigger" @click={Ev open_} aria-haspopup="dialog">
            {props.triggerLabel.Value}
            <span class="trigger-kbd">⌘K</span>
        </button>
        <div class={backdropCls} @click={Ev close}></div>
        <div class={panelCls} role="dialog" aria-modal="true" aria-label="Command palette">
            <div class="search-wrap">
                <span class="search-icon" aria-hidden="true">⌕</span>
                <input
                    class="search-input"
                    type="text"
                    placeholder={props.placeholder.Value}
                    .value={search}
                    @input={Ev onInput}
                    @keydown={Ev onKeyDown}
                    aria-label="Search"
                    autocomplete="off" />
            </div>
            <div class="results" role="listbox">
                {renderResults ()}
            </div>
            <div class="footer">
                <span>↑↓ navigate</span>
                <span>↵ select</span>
                <span>Esc close</span>
            </div>
        </div>
    """

let register () = ()
