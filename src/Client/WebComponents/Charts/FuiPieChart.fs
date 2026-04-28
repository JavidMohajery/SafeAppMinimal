module WebComponents.FuiPieChart

open Lit
open Fable.Core
open System

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Import("unsafeSVG", "lit/directives/unsafe-svg.js")>]
let private unsafeSVG: string -> obj = jsNative

[<Emit("JSON.parse($0)")>]
let private parseData (s: string) : {| label: string; value: float |} array = jsNative

[<Emit("JSON.parse($0)")>]
let private parseColors (s: string) : string array = jsNative

let private defaultPalette =
    [| "#7C3AED"; "#3B82F6"; "#22C55E"; "#F59E0B"; "#EF4444"; "#EC4899"; "#06B6D4"; "#8B5CF6" |]

let private polarXY (cx: float) (cy: float) (r: float) (angleDeg: float) =
    let a = (angleDeg - 90.0) * Math.PI / 180.0
    cx + r * Math.Cos a, cy + r * Math.Sin a

let private slicePath (cx: float) (cy: float) (outerR: float) (innerR: float) (startA: float) (sweepA: float) =
    let endA  = startA + min sweepA 359.9999
    let x1, y1 = polarXY cx cy outerR startA
    let x2, y2 = polarXY cx cy outerR endA
    let large  = if sweepA > 180.0 then 1 else 0
    if innerR <= 0.0 then
        sprintf "M %.4f %.4f L %.4f %.4f A %.4f %.4f 0 %d 1 %.4f %.4f Z"
            cx cy x1 y1 outerR outerR large x2 y2
    else
        let x3, y3 = polarXY cx cy innerR endA
        let x4, y4 = polarXY cx cy innerR startA
        sprintf "M %.4f %.4f A %.4f %.4f 0 %d 1 %.4f %.4f L %.4f %.4f A %.4f %.4f 0 %d 0 %.4f %.4f Z"
            x1 y1 outerR outerR large x2 y2 x3 y3 innerR innerR large x4 y4

let private styles = unsafeCSS """
    :host { display: inline-block; }
    :host([hidden]) { display: none; }
    .wrap { display: flex; flex-direction: column; align-items: center; gap: 1rem; }
    .legend { display: flex; flex-wrap: wrap; gap: 0.375rem 0.875rem; justify-content: center; }
    .legend-item { display: flex; align-items: center; gap: 0.375rem; font-family: 'Sora', sans-serif; font-size: 0.8rem; color: #6E6E76; }
    .swatch { display: inline-block; width: 10px; height: 10px; border-radius: 2px; flex-shrink: 0; }
"""

[<LitElement("fui-pie-chart")>]
let FuiPieChart () =
    let _, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props <- {|
            data       = Prop.Of("[]",  attribute = "data")
            size       = Prop.Of(200,   attribute = "size")
            donut      = Prop.Of(false, attribute = "donut")
            colors     = Prop.Of("[]",  attribute = "colors")
            showLegend = Prop.Of(true,  attribute = "show-legend")
        |}
    )

    let items = parseData props.data.Value
    let count = items.Length

    if count = 0 then html $"""<div></div>"""
    else

    let userColors = parseColors props.colors.Value
    let palette    = if userColors.Length > 0 then userColors else defaultPalette
    let sz         = float props.size.Value
    let cx         = sz / 2.0
    let cy         = sz / 2.0
    let outerR     = sz / 2.0 * 0.88
    let innerR     = if props.donut.Value then outerR * 0.55 else 0.0
    let total      = items |> Array.sumBy (fun d -> d.value)
    let safeTotal  = if total < 1e-9 then 1.0 else total

    let sweeps = items |> Array.map (fun d -> d.value / safeTotal * 360.0)
    let starts = sweeps |> Array.scan (+) 0.0 |> Array.take count

    let slicesStr =
        Array.init count (fun i ->
            let path = slicePath cx cy outerR innerR starts.[i] sweeps.[i]
            let col  = palette.[i % palette.Length]
            sprintf """<path d="%s" fill="%s" stroke="#0D0D0F" stroke-width="1.5" style="cursor:default;transition:opacity 0.15s"/>""" path col
        ) |> String.concat ""

    let legendItems =
        if props.showLegend.Value then
            items |> Array.mapi (fun i d ->
                let col = palette.[i % palette.Length]
                html $"""<span class="legend-item"><span class="swatch" style="background:{col}"></span>{d.label}</span>"""
            )
        else [||]

    let legend =
        if props.showLegend.Value then
            html $"""<div class="legend">{legendItems}</div>"""
        else Lit.nothing

    html $"""
        <div class="wrap">
            <svg viewBox="0 0 {sz} {sz}" width="{sz}" height="{sz}">
                {unsafeSVG slicesStr}
            </svg>
            {legend}
        </div>
    """

let register () = ()
