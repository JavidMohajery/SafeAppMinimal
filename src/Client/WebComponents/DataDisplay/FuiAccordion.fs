module WebComponents.FuiAccordion

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("JSON.parse($0)")>]
let private parseItems (_: string) : {| label: string; content: string |} array = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-accordion-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .accordion {
        border       : 1px solid var(--fui-accordion-border, #2A2A2E);
        border-radius: var(--fui-accordion-radius, 8px);
        overflow     : hidden;
    }

    .item { border-bottom: 1px solid var(--fui-accordion-border, #2A2A2E); }
    .item:last-child { border-bottom: none; }

    .trigger {
        align-items    : center;
        background     : none;
        border         : none;
        color          : var(--fui-accordion-trigger-color, #E8E8ED);
        cursor         : pointer;
        display        : flex;
        font-family    : inherit;
        font-size      : 0.9375rem;
        font-weight    : 500;
        justify-content: space-between;
        padding        : 1rem 1.125rem;
        text-align     : left;
        transition     : background 0.15s;
        width          : 100%;
    }
    .trigger:hover { background: rgba(255,255,255,0.025); }
    .trigger:focus-visible {
        outline       : 2px solid #7C3AED;
        outline-offset: -2px;
    }

    .chevron {
        color      : var(--fui-accordion-icon-color, #6E6E76);
        flex-shrink: 0;
        font-size  : 0.65rem;
        transition : transform 0.2s;
    }
    .chevron.open { transform: rotate(180deg); }

    .body {
        color      : var(--fui-accordion-color, #A0A0A8);
        display    : none;
        font-size  : 0.9375rem;
        line-height: 1.65;
        padding    : 0 1.125rem 1rem;
    }
    .body.open { display: block; }
"""

[<LitElement("fui-accordion")>]
let FuiAccordion () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            items    = Prop.Of("[]",  attribute = "items")
            multiple = Prop.Of(false, attribute = "multiple")
        |}
    )

    // Single-open mode state
    let openIdx, setOpenIdx = Hook.useState -1
    // Multi-open mode state
    let openSet, setOpenSet = Hook.useState (Set.empty : Set<int>)

    let isOpen i =
        if props.multiple.Value then Set.contains i openSet
        else openIdx = i

    let toggle i _ =
        if props.multiple.Value then
            if Set.contains i openSet then setOpenSet (Set.remove i openSet)
            else setOpenSet (Set.add i openSet)
        else
            if openIdx = i then setOpenIdx -1 else setOpenIdx i

    let items = parseItems props.items.Value

    let renderItem i (item: {| label: string; content: string |}) =
        let expanded   = isOpen i
        let chevronCls = if expanded then "chevron open" else "chevron"
        let bodyCls    = if expanded then "body open"    else "body"
        html $"""
            <div class="item">
                <button class="trigger"
                        aria-expanded={string expanded}
                        @click={Ev (toggle i)}>
                    {item.label}
                    <span class={chevronCls} aria-hidden="true">▼</span>
                </button>
                <div class={bodyCls} role="region">{item.content}</div>
            </div>
        """

    let itemEls = items |> Array.mapi renderItem

    html $"""<div class="accordion" part="accordion">{itemEls}</div>"""

let register () = ()
