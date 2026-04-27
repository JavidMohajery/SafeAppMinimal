module WebComponents.FuiStack

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display: block;
    }
    :host([hidden]) { display: none; }

    .stack {
        display: flex;
    }
"""

[<LitElement("fui-stack")>]
let FuiStack() =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            direction = Prop.Of("column",     attribute = "direction")
            gap       = Prop.Of("1rem",       attribute = "gap")
            align     = Prop.Of("stretch",    attribute = "align")
            justify   = Prop.Of("flex-start", attribute = "justify")
            wrap      = Prop.Of(false,        attribute = "wrap")
        |}
    )

    let flexDir  = if props.direction.Value = "row" then "row" else "column"
    let flexWrap = if props.wrap.Value then "wrap" else "nowrap"

    // CSS values here are plain identifiers and simple measures — no commas,
    // so F# $"" interpolation is safe (no var(--prop, fallback) patterns).
    let stackStyle =
        $"flex-direction:{flexDir};gap:{props.gap.Value};align-items:{props.align.Value};justify-content:{props.justify.Value};flex-wrap:{flexWrap}"

    html $"""
        <div class="stack" part="stack" style={stackStyle}>
            <slot></slot>
        </div>
    """

let register () = ()
