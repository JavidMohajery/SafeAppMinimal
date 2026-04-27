module WebComponents.FuiGrid

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

    .grid {
        display: grid;
        width  : 100%;
    }
"""

[<LitElement("fui-grid")>]
let FuiGrid() =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            cols   = Prop.Of("2",    attribute = "cols")
            gap    = Prop.Of("1rem", attribute = "gap")
            rowGap = Prop.Of("",     attribute = "row-gap")
            colGap = Prop.Of("",     attribute = "col-gap")
        |}
    )

    let colsVal =
        let c = props.cols.Value
        if c <> "" && c |> Seq.forall System.Char.IsDigit then $"repeat({c}, 1fr)"
        else c

    let gapStyles =
        if props.rowGap.Value = "" && props.colGap.Value = "" then
            [ $"gap:{props.gap.Value}" ]
        else
            [ if props.rowGap.Value <> "" then yield $"row-gap:{props.rowGap.Value}"
              if props.colGap.Value <> "" then yield $"column-gap:{props.colGap.Value}" ]

    let gridStyle =
        ($"grid-template-columns:{colsVal}" :: gapStyles) |> String.concat ";"

    html $"""
        <div class="grid" part="grid" style={gridStyle}>
            <slot></slot>
        </div>
    """

let register () = ()
