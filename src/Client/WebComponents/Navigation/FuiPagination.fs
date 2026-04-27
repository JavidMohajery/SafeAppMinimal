module WebComponents.FuiPagination

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("parseInt($0, 10) || 1")>]
let private parseInt (_: string) : int = jsNative

let private styles = unsafeCSS """
    :host {
        display: block;
        font-family: var(--fui-pg-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    .pg-wrap {
        display    : flex;
        align-items: center;
        gap        : 0.25rem;
        flex-wrap  : wrap;
    }

    .pg-btn {
        background   : var(--fui-pg-bg, #1E1E21);
        border       : 1px solid var(--fui-pg-border, #2A2A2E);
        border-radius: var(--fui-pg-radius, 6px);
        color        : var(--fui-pg-color, #6E6E76);
        cursor       : pointer;
        font-family  : inherit;
        font-size    : 0.8125rem;
        font-weight  : 600;
        height       : 2rem;
        min-width    : 2rem;
        padding      : 0 0.5rem;
        transition   : background 0.15s, color 0.15s, border-color 0.15s;
    }
    .pg-btn:hover:not(:disabled) {
        background  : var(--fui-pg-bg-hover, #2A2A2E);
        border-color: var(--fui-pg-border-hover, #3A3A3E);
        color       : var(--fui-pg-color-hover, #E8E8ED);
    }
    .pg-btn.active {
        background  : var(--fui-pg-accent, #7C3AED);
        border-color: var(--fui-pg-accent, #7C3AED);
        color       : #fff;
        cursor      : default;
    }
    .pg-btn:focus-visible {
        outline       : 2px solid var(--fui-pg-accent, #7C3AED);
        outline-offset: 2px;
    }
    .pg-btn:disabled {
        opacity       : 0.35;
        cursor        : not-allowed;
        pointer-events: none;
    }

    .pg-ellipsis {
        color      : var(--fui-pg-muted, #3A3A3E);
        font-size  : 0.875rem;
        padding    : 0 0.125rem;
        user-select: none;
    }
"""

// Compute which page numbers (and ellipsis gaps) to show.
// Returns (int option) list where None = ellipsis.
let private buildRange (total: int) (current: int) (siblings: int) : int option list =
    if total <= 1 then [ Some 1 ]
    else
        let left  = max 1     (current - siblings)
        let right = min total (current + siblings)
        [
            yield Some 1
            if left > 2 then yield None
            for p in left..right do
                if p > 1 && p < total then yield Some p
            if right < total - 1 then yield None
            yield Some total
        ]

[<LitElement("fui-pagination")>]
let FuiPagination () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            total    = Prop.Of("1",  attribute = "total")
            page     = Prop.Of("1",  attribute = "page")
            siblings = Prop.Of("1",  attribute = "siblings")
        |}
    )

    let totalPages = parseInt props.total.Value    |> max 1
    let initPage   = parseInt props.page.Value     |> max 1 |> min totalPages
    let siblings   = parseInt props.siblings.Value |> max 0

    let currentPage, setCurrentPage = Hook.useState initPage

    let goTo (p: int) =
        let clamped = max 1 (min totalPages p)
        setCurrentPage clamped
        host.dispatchCustomEvent("fui-change", detail = {| page = clamped |})

    let range = buildRange totalPages currentPage siblings

    let renderItem (item: int option) : TemplateResult =
        match item with
        | None ->
            html $"""<span class="pg-ellipsis">…</span>"""
        | Some p ->
            let cls = if p = currentPage then "pg-btn active" else "pg-btn"
            html $"""
                <button
                    class={cls}
                    aria-label={$"Page {p}"}
                    aria-current={if p = currentPage then "page" else "false"}
                    @click={Ev (fun _ -> if p <> currentPage then goTo p)}>
                    {p}
                </button>"""

    let items = range |> List.map renderItem

    html $"""
        <nav aria-label="Pagination" class="pg-wrap">
            <button
                class="pg-btn"
                aria-label="Previous page"
                ?disabled={currentPage <= 1}
                @click={Ev (fun _ -> goTo (currentPage - 1))}>
                ‹
            </button>
            {items}
            <button
                class="pg-btn"
                aria-label="Next page"
                ?disabled={currentPage >= totalPages}
                @click={Ev (fun _ -> goTo (currentPage + 1))}>
                ›
            </button>
        </nav>
    """

let register () = ()
