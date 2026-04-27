module WebComponents.FuiMenu

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("JSON.parse($0)")>]
let private parseJSON (_: string) : {| value: string; label: string; disabled: bool; separator: bool |} array = jsNative

let private styles = unsafeCSS """
    :host {
        display    : inline-block;
        position   : relative;
        font-family: var(--fui-menu-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    .wrap {
        position: relative;
    }

    /* ── Trigger ────────────────────────────────────────────────────────────── */
    .trigger {
        align-items  : center;
        background   : var(--fui-menu-trigger-bg, #1E1E21);
        border       : 1px solid var(--fui-menu-trigger-border, #2A2A2E);
        border-radius: var(--fui-menu-radius, 6px);
        color        : var(--fui-menu-trigger-color, #E8E8ED);
        cursor       : pointer;
        display      : inline-flex;
        font-family  : inherit;
        font-size    : 0.875rem;
        font-weight  : 600;
        gap          : 0.375rem;
        padding      : 0.5rem 0.875rem;
        transition   : background 0.15s, border-color 0.15s;
        user-select  : none;
    }
    .trigger:hover {
        background  : var(--fui-menu-trigger-bg-hover, #2A2A2E);
        border-color: var(--fui-menu-trigger-border-hover, #3A3A3E);
    }
    .trigger:focus-visible {
        outline       : 2px solid var(--fui-menu-accent, #7C3AED);
        outline-offset: 2px;
    }
    .chevron {
        font-size : 0.6rem;
        opacity   : 0.6;
        transition: transform 0.15s;
    }
    .chevron.open { transform: rotate(180deg); }

    /* Invisible full-viewport overlay — catches outside clicks without a
       document event listener (avoids the Fable.Lit Hook.useEffect cleanup
       limitation and shadow-DOM containment checks entirely). */
    .overlay {
        display : none;
        inset   : 0;
        position: fixed;
        z-index : 9998;
    }
    .overlay.open { display: block; }

    /* ── Dropdown panel ─────────────────────────────────────────────────────── */
    .dropdown {
        background   : var(--fui-menu-bg, #1E1E21);
        border       : 1px solid var(--fui-menu-border, #2A2A2E);
        border-radius: var(--fui-menu-radius, 6px);
        box-shadow   : 0 8px 24px rgba(0,0,0,0.45);
        display      : none;
        left         : 0;
        min-width    : 180px;
        overflow     : hidden;
        padding      : 0.25rem 0;
        position     : absolute;
        top          : calc(100% + 4px);
        z-index      : 9999;
    }
    .dropdown.open { display: block; }

    /* Placement variants driven by a class on .wrap */
    .placement-bottom-end .dropdown { left: auto; right: 0; }
    .placement-top-start  .dropdown { top: auto; bottom: calc(100% + 4px); }
    .placement-top-end    .dropdown { top: auto; bottom: calc(100% + 4px); left: auto; right: 0; }

    /* ── Menu items ─────────────────────────────────────────────────────────── */
    .menu-item {
        background : none;
        border     : none;
        color      : var(--fui-menu-item-color, #E8E8ED);
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
        background: var(--fui-menu-item-bg-hover, rgba(124,58,237,0.12));
        color     : var(--fui-menu-accent, #7C3AED);
    }
    .menu-item:focus-visible {
        background: var(--fui-menu-item-bg-hover, rgba(124,58,237,0.12));
        outline   : none;
    }
    .menu-item:disabled {
        color         : var(--fui-menu-disabled-color, #3A3A3E);
        cursor        : not-allowed;
        pointer-events: none;
    }

    .menu-sep {
        border    : none;
        border-top: 1px solid var(--fui-menu-sep-color, #2A2A2E);
        margin    : 0.25rem 0;
    }
"""

[<LitElement("fui-menu")>]
let FuiMenu () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            label     = Prop.Of("Menu",         attribute = "label")
            items     = Prop.Of("[]",           attribute = "items")
            placement = Prop.Of("bottom-start", attribute = "placement")
        |}
    )

    let isOpen, setIsOpen = Hook.useState false

    let toggle _ = setIsOpen (not isOpen)
    let close  _ = setIsOpen false

    let opts = parseJSON props.items.Value

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

    let items       = opts |> Array.map renderItem
    let overlayCls  = if isOpen then "overlay open"  else "overlay"
    let chevronCls  = if isOpen then "chevron open"  else "chevron"
    let dropdownCls = if isOpen then "dropdown open" else "dropdown"
    let wrapCls     = $"wrap placement-{props.placement.Value}"

    html $"""
        <div class={wrapCls}>
            <div class={overlayCls} @click={Ev close}></div>
            <button
                class="trigger"
                aria-haspopup="true"
                aria-expanded={string isOpen}
                @click={Ev toggle}>
                {props.label.Value}
                <span class={chevronCls} aria-hidden="true">▼</span>
            </button>
            <div class={dropdownCls} role="menu">
                {items}
            </div>
        </div>
    """

let register () = ()
