module WebComponents.FuiBreadcrumb

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("JSON.parse($0)")>]
let private parseJSON (_: string) : {| label: string; href: string |} array = jsNative

let private styles = unsafeCSS """
    :host {
        display: block;
        font-family: var(--fui-bc-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    .bc-list {
        display    : flex;
        flex-wrap  : wrap;
        align-items: center;
        gap        : 0.25rem;
        list-style : none;
        margin     : 0;
        padding    : 0;
        font-size  : 0.8125rem;
    }

    .bc-item {
        display    : flex;
        align-items: center;
        gap        : 0.25rem;
    }

    .bc-link {
        color          : var(--fui-bc-link-color, #6E6E76);
        text-decoration: none;
        transition     : color 0.15s;
    }
    .bc-link:hover {
        color: var(--fui-bc-link-hover, #E8E8ED);
    }
    .bc-link:focus-visible {
        outline       : 2px solid var(--fui-bc-accent, #7C3AED);
        outline-offset: 2px;
        border-radius : 2px;
    }

    .bc-current {
        color: var(--fui-bc-current-color, #E8E8ED);
    }

    .bc-sep {
        color      : var(--fui-bc-sep-color, #3A3A3E);
        user-select: none;
        font-size  : 0.75rem;
    }
"""

[<LitElement("fui-breadcrumb")>]
let FuiBreadcrumb () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            items     = Prop.Of("[]", attribute = "items")
            separator = Prop.Of("/",  attribute = "separator")
        |}
    )

    let opts = parseJSON props.items.Value
    let sep  = props.separator.Value

    let renderItem (i: int) (item: {| label: string; href: string |}) =
        let isLast = i = opts.Length - 1
        let content =
            if isLast then
                html $"""<span class="bc-current" aria-current="page">{item.label}</span>"""
            elif item.href <> "" then
                html $"""<a class="bc-link" href={item.href}>{item.label}</a>"""
            else
                html $"""<span class="bc-link">{item.label}</span>"""
        if i = 0 then
            html $"""<li class="bc-item">{content}</li>"""
        else
            let sepEl = html $"""<span class="bc-sep" aria-hidden="true">{sep}</span>"""
            html $"""<li class="bc-item">{sepEl}{content}</li>"""

    let items = opts |> Array.mapi renderItem

    html $"""
        <nav aria-label="Breadcrumb">
            <ol class="bc-list">
                {items}
            </ol>
        </nav>
    """

let register () = ()
