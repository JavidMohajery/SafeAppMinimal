module WebComponents.FuiContextMenu

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("JSON.parse($0)")>]
let private parseJSON (_: string) : {| value: string; label: string; disabled: bool; separator: bool |} array = jsNative

[<Emit("$0.clientX|0")>]
let private clientX (_: obj) : int = jsNative

[<Emit("$0.clientY|0")>]
let private clientY (_: obj) : int = jsNative

[<Emit("$0.preventDefault()")>]
let private preventDefault (_: obj) : unit = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-ctx-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    /* Invisible full-viewport overlay catches outside clicks */
    .overlay {
        display : none;
        inset   : 0;
        position: fixed;
        z-index : 9998;
    }
    .overlay.open { display: block; }

    .dropdown {
        background   : var(--fui-ctx-bg,     #1E1E21);
        border       : 1px solid var(--fui-ctx-border, #2A2A2E);
        border-radius: var(--fui-ctx-radius,  6px);
        box-shadow   : 0 8px 24px rgba(0,0,0,0.5);
        display      : none;
        min-width    : 180px;
        overflow     : hidden;
        padding      : 0.25rem 0;
        position     : fixed;
        z-index      : 9999;
    }
    .dropdown.open { display: block; }

    .menu-item {
        background : none;
        border     : none;
        color      : var(--fui-ctx-item-color, #E8E8ED);
        cursor     : pointer;
        display    : block;
        font-family: inherit;
        font-size  : 0.875rem;
        padding    : 0.5rem 0.875rem;
        text-align : left;
        transition : background 0.1s, color 0.1s;
        width      : 100%;
    }
    .menu-item:hover:not(:disabled) {
        background: var(--fui-ctx-item-hover, rgba(124,58,237,0.12));
        color     : var(--fui-ctx-accent, #9B59F5);
    }
    .menu-item:disabled { color: #3A3A3E; cursor: default; }

    .menu-sep {
        border    : none;
        border-top: 1px solid var(--fui-ctx-sep, #2A2A2E);
        margin    : 0.25rem 0;
    }
"""

[<LitElement("fui-context-menu")>]
let FuiContextMenu () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            items = Prop.Of("[]", attribute = "items")
        |}
    )

    let isOpen, setIsOpen = Hook.useState false
    let x,      setX      = Hook.useState 0
    let y,      setY      = Hook.useState 0

    let close _ = setIsOpen false

    let onContextMenu (e: obj) =
        preventDefault e
        setX (clientX e)
        setY (clientY e)
        setIsOpen true

    let selectItem (v: string) =
        setIsOpen false
        host.dispatchCustomEvent("fui-select", detail = {| value = v |})

    let renderItem (item: {| value: string; label: string; disabled: bool; separator: bool |}) =
        if item.separator then
            html $"""<hr class="menu-sep" role="separator" />"""
        else
            html $"""
                <button
                    class="menu-item"
                    role="menuitem"
                    ?disabled={item.disabled}
                    @click={Ev (fun _ -> selectItem item.value)}>
                    {item.label}
                </button>"""

    let opts        = parseJSON props.items.Value
    let items       = opts |> Array.map renderItem
    let overlayCls  = if isOpen then "overlay open"  else "overlay"
    let dropdownCls = if isOpen then "dropdown open" else "dropdown"

    html $"""
        <div @contextmenu={Ev onContextMenu}>
            <slot></slot>
        </div>
        <div class={overlayCls} @click={Ev close}></div>
        <div class={dropdownCls} style="left:{x}px;top:{y}px" role="menu">
            {items}
        </div>
    """

let register () = ()
