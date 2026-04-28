module WebComponents.FuiTimeline

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("JSON.parse($0)")>]
let private parseItems (_: string) : {| label: string; description: string; date: string; icon: string |} array = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-tl-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .timeline {
        list-style: none;
        margin    : 0;
        padding   : 0;
    }

    .item {
        display      : flex;
        gap          : 1rem;
        padding-bottom: 1.5rem;
    }
    .item:last-child { padding-bottom: 0; }

    /* ── Dot column ── */
    .dot-col {
        align-items   : flex-start;
        display       : flex;
        flex-direction: column;
        flex-shrink   : 0;
        position      : relative;
        width         : 20px;
    }

    /* Vertical connector line */
    .item:not(:last-child) .dot-col::after {
        background: var(--fui-tl-line, #2A2A2E);
        bottom    : -1.5rem;
        content   : '';
        left      : 50%;
        position  : absolute;
        top       : 20px;
        transform : translateX(-50%);
        width     : 1px;
    }

    .dot {
        align-items    : center;
        background     : var(--fui-tl-dot-bg, #161618);
        border         : 2px solid var(--fui-tl-dot-border, #7C3AED);
        border-radius  : 50%;
        color          : var(--fui-tl-dot-color, #7C3AED);
        display        : flex;
        flex-shrink    : 0;
        font-size      : 0.55rem;
        height         : 20px;
        justify-content: center;
        line-height    : 1;
        width          : 20px;
        z-index        : 1;
    }

    /* ── Content ── */
    .content { flex: 1; min-width: 0; padding-top: 0.1rem; }

    .item-header {
        align-items    : baseline;
        display        : flex;
        gap            : 0.5rem;
        justify-content: space-between;
    }

    .item-label {
        color      : var(--fui-tl-label-color, #E8E8ED);
        font-family: 'JetBrains Mono', monospace;
        font-size  : 0.875rem;
        font-weight: 600;
        margin     : 0;
    }

    .item-date {
        color      : var(--fui-tl-date-color, #6E6E76);
        flex-shrink: 0;
        font-size  : 0.75rem;
        white-space: nowrap;
    }

    .item-desc {
        color      : var(--fui-tl-desc-color, #A0A0A8);
        font-size  : 0.875rem;
        line-height: 1.55;
        margin     : 0.25rem 0 0;
    }
"""

[<LitElement("fui-timeline")>]
let FuiTimeline () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            items = Prop.Of("[]", attribute = "items")
        |}
    )

    let items = parseItems props.items.Value

    let renderItem (item: {| label: string; description: string; date: string; icon: string |}) =
        let dotContent = if item.icon <> "" then item.icon else "●"

        let datePart =
            if item.date <> "" then
                html $"""<span class="item-date">{item.date}</span>"""
            else Lit.nothing

        let descPart =
            if item.description <> "" then
                html $"""<p class="item-desc">{item.description}</p>"""
            else Lit.nothing

        html $"""
            <li class="item">
                <div class="dot-col">
                    <span class="dot" aria-hidden="true">{dotContent}</span>
                </div>
                <div class="content">
                    <div class="item-header">
                        <p class="item-label">{item.label}</p>
                        {datePart}
                    </div>
                    {descPart}
                </div>
            </li>
        """

    let itemEls = items |> Array.map renderItem

    html $"""<ul class="timeline" part="timeline">{itemEls}</ul>"""

let register () = ()
