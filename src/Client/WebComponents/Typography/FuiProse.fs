module WebComponents.FuiProse

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-prose-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .prose {
        color      : var(--fui-prose-color, #E8E8ED);
        font-family: inherit;
        font-size  : var(--fui-prose-size,  0.9375rem);
        line-height: 1.7;
        max-width  : var(--fui-prose-width, 68ch);
    }

    .prose ::slotted(p)          { margin: 0 0 1em; }
    .prose ::slotted(h1),
    .prose ::slotted(h2),
    .prose ::slotted(h3),
    .prose ::slotted(h4)         { color: #E8E8ED; font-family: 'JetBrains Mono', monospace; font-weight: 700; line-height: 1.25; margin: 1.5em 0 0.5em; }
    .prose ::slotted(h1)         { font-size: 2rem; }
    .prose ::slotted(h2)         { font-size: 1.5rem; }
    .prose ::slotted(h3)         { font-size: 1.25rem; }
    .prose ::slotted(h4)         { font-size: 1.1rem; }
    .prose ::slotted(ul),
    .prose ::slotted(ol)         { margin: 0 0 1em 1.5em; padding: 0; }
    .prose ::slotted(li)         { margin-bottom: 0.35em; }
    .prose ::slotted(blockquote) { border-left: 3px solid #7C3AED; color: #A0A0A8; font-style: italic; margin: 1em 0; padding: 0.75rem 1.25rem; }
    .prose ::slotted(code)       { background: rgba(124,58,237,0.10); border: 1px solid rgba(124,58,237,0.2); border-radius: 4px; color: #9B59F5; font-size: 0.85em; padding: 0.1em 0.4em; }
    .prose ::slotted(pre)        { background: #161618; border: 1px solid #2A2A2E; border-radius: 6px; overflow-x: auto; padding: 1rem; }
    .prose ::slotted(a)          { color: #9B59F5; text-decoration: underline; text-underline-offset: 2px; }
    .prose ::slotted(hr)         { border: none; border-top: 1px solid #2A2A2E; margin: 2em 0; }
    .prose ::slotted(strong)     { color: #E8E8ED; font-weight: 700; }
    .prose ::slotted(em)         { color: #A0A0A8; font-style: italic; }
    .prose ::slotted(*:first-child) { margin-top: 0; }
    .prose ::slotted(*:last-child)  { margin-bottom: 0; }

    :host([size="sm"]) .prose { font-size: 0.875rem; }
    :host([size="lg"]) .prose { font-size: 1.0625rem; }
"""

[<LitElement("fui-prose")>]
let FuiProse () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            size = Prop.Of("md", attribute = "size")
        |}
    )

    html $"""<div class="prose" part="prose"><slot></slot></div>"""

let register () = ()
