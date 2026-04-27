module WebComponents.FuiScrollArea

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display: block;
        width  : 100%;
    }
    :host([hidden]) { display: none; }

    .scroll-area {
        overflow          : auto;
        width             : 100%;
        height            : 100%;
        scrollbar-width   : thin;
        scrollbar-color   : var(--fui-sa-thumb, #3A3A3E) var(--fui-sa-track, transparent);
    }

    /* ── Webkit custom scrollbar ───────────────────────────────────────────── */
    .scroll-area::-webkit-scrollbar          { width: 6px; height: 6px; }
    .scroll-area::-webkit-scrollbar-track    { background: var(--fui-sa-track, transparent); border-radius: 3px; }
    .scroll-area::-webkit-scrollbar-thumb    { background: var(--fui-sa-thumb, #3A3A3E); border-radius: 3px; }
    .scroll-area::-webkit-scrollbar-thumb:hover { background: var(--fui-sa-thumb-hover, #7C3AED); }
    .scroll-area::-webkit-scrollbar-corner   { background: transparent; }

    /* ── Overflow direction ────────────────────────────────────────────────── */
    :host([direction="vertical"])   .scroll-area { overflow-x: hidden;  overflow-y: auto; }
    :host([direction="horizontal"]) .scroll-area { overflow-x: auto;    overflow-y: hidden; }
    :host([direction="both"])       .scroll-area { overflow:   auto; }
"""

[<LitElement("fui-scroll-area")>]
let FuiScrollArea() =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            height    = Prop.Of("",         attribute = "height")
            maxHeight = Prop.Of("",         attribute = "max-height")
            direction = Prop.Of("vertical", attribute = "direction")
        |}
    )

    // Build inline style only for the dimensions that are explicitly set.
    let areaStyle =
        [ if props.height.Value    <> "" then yield $"height:{props.height.Value}"
          if props.maxHeight.Value <> "" then yield $"max-height:{props.maxHeight.Value}" ]
        |> String.concat ";"

    html $"""
        <div class="scroll-area" part="scroll-area" style={areaStyle}>
            <slot></slot>
        </div>
    """

let register () = ()
