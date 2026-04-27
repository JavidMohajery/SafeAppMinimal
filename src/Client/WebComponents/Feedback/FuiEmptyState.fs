module WebComponents.FuiEmptyState

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-es-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .empty {
        align-items   : center;
        display       : flex;
        flex-direction: column;
        gap           : 1rem;
        padding       : var(--fui-es-padding, 3rem 1.5rem);
        text-align    : center;
    }

    .icon-wrap {
        align-items    : center;
        background     : var(--fui-es-icon-bg, #1E1E21);
        border         : 1px solid var(--fui-es-icon-border, #2A2A2E);
        border-radius  : 50%;
        display        : flex;
        flex-shrink    : 0;
        height         : 64px;
        justify-content: center;
        width          : 64px;
    }

    .icon-default {
        color    : var(--fui-es-icon-color, #3A3A3E);
        font-size: 1.75rem;
        line-height: 1;
    }

    .title {
        color         : var(--fui-es-title-color, #E8E8ED);
        font-family   : 'JetBrains Mono', monospace;
        font-size     : 1rem;
        font-weight   : 600;
        letter-spacing: 0.01em;
        margin        : 0;
    }

    .description {
        color      : var(--fui-es-desc-color, #6E6E76);
        font-size  : 0.875rem;
        line-height: 1.6;
        margin     : 0;
        max-width  : 30ch;
    }

    .action {
        display   : block;
        margin-top: 0.25rem;
    }

    /* hide the action wrapper when the slot is empty */
    .action:not(:has(*)) { display: none; }
"""

[<LitElement("fui-empty-state")>]
let FuiEmptyState () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            title       = Prop.Of("Nothing here yet", attribute = "title")
            description = Prop.Of("",                  attribute = "description")
        |}
    )

    let descPart =
        if props.description.Value <> "" then
            html $"""<p class="description" part="description">{props.description.Value}</p>"""
        else Lit.nothing

    html $"""
        <div class="empty" part="empty">
            <div class="icon-wrap" part="icon-wrap">
                <slot name="icon">
                    <span class="icon-default" aria-hidden="true">◎</span>
                </slot>
            </div>
            <p class="title" part="title">{props.title.Value}</p>
            {descPart}
            <div class="action" part="action">
                <slot name="action"></slot>
            </div>
            <slot></slot>
        </div>
    """

let register () = ()
