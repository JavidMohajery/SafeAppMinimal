module WebComponents.FuiList

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("JSON.parse($0)")>]
let private parseItems (_: string) : string array = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-list-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    ul, ol {
        color      : var(--fui-list-color, #A0A0A8);
        font-size  : 0.9375rem;
        line-height: 1.7;
        list-style : none;
        margin     : 0;
        padding    : 0;
    }

    li { padding: 0.3rem 0; }

    /* ── Bordered ── */
    :host([variant="bordered"]) li {
        border-bottom: 1px solid var(--fui-list-divider, #2A2A2E);
        padding      : 0.5rem 0;
    }
    :host([variant="bordered"]) li:last-child { border-bottom: none; }

    /* ── Striped ── */
    :host([variant="striped"]) li:nth-child(odd) {
        background   : rgba(255,255,255,0.025);
        border-radius: 4px;
        padding      : 0.4rem 0.625rem;
    }
    :host([variant="striped"]) li:nth-child(even) { padding: 0.4rem 0.625rem; }

    /* ── Bullet ── */
    :host([variant="bullet"]) li {
        display: flex;
        gap    : 0.5rem;
    }
    :host([variant="bullet"]) li::before {
        color      : #7C3AED;
        content    : '▸';
        flex-shrink: 0;
    }

    /* ── Ordered counter ── */
    ol { counter-reset: li-counter; }
    ol li {
        counter-increment: li-counter;
        display          : flex;
        gap              : 0.75rem;
    }
    ol li::before {
        color      : var(--fui-list-counter-color, #6E6E76);
        content    : counter(li-counter) '.';
        flex-shrink: 0;
        font-family: 'JetBrains Mono', monospace;
        font-size  : 0.8125rem;
        font-weight: 600;
        min-width  : 1.5rem;
    }
"""

[<LitElement("fui-list")>]
let FuiList () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            items   = Prop.Of("[]",      attribute = "items")
            ordered = Prop.Of(false,     attribute = "ordered")
            variant = Prop.Of("default", attribute = "variant")
        |}
    )

    let items    = parseItems props.items.Value
    let listItems = items |> Array.map (fun item -> html $"""<li>{item}</li>""")

    if props.ordered.Value then
        html $"""<ol part="list">{listItems}</ol>"""
    else
        html $"""<ul part="list">{listItems}</ul>"""

let register () = ()
