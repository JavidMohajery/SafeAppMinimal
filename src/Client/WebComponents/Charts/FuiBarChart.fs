module WebComponents.FuiBarChart

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Import("unsafeSVG", "lit/directives/unsafe-svg.js")>]
let private unsafeSVG: string -> obj = jsNative

[<Emit("JSON.parse($0)")>]
let private parseData (s: string) : {| label: string; value: float |} array = jsNative

let private xmlEncode (s: string) =
    s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")

let private styles = unsafeCSS """
    :host { display: block; }
    :host([hidden]) { display: none; }
    svg { display: block; width: 100%; overflow: visible; }
"""

[<LitElement("fui-bar-chart")>]
let FuiBarChart () =
    let _, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props <- {|
            data       = Prop.Of("[]",      attribute = "data")
            height     = Prop.Of(200,       attribute = "height")
            color      = Prop.Of("#7C3AED", attribute = "color")
            showLabels = Prop.Of(true,      attribute = "show-labels")
            showValues = Prop.Of(false,     attribute = "show-values")
            gap        = Prop.Of(8,         attribute = "gap")
        |}
    )

    let items = parseData props.data.Value
    let count = items.Length

    if count = 0 then html $"""<svg></svg>"""
    else

    let svgW   = 600.0
    let chartH = float props.height.Value
    let labelH = if props.showLabels.Value then 22.0 else 0.0
    let valPad = if props.showValues.Value then 18.0 else 4.0
    let totalH = chartH + labelH + valPad
    let gapPx  = float props.gap.Value
    let barW   = (svgW - gapPx * float (count - 1)) / float count
    let maxV   = items |> Array.map (fun d -> d.value) |> Array.max

    let gridLines =
        [| 0.25; 0.5; 0.75; 1.0 |]
        |> Array.map (fun f ->
            let y = valPad + chartH * (1.0 - f)
            sprintf """<line x1="0" y1="%.2f" x2="%.2f" y2="%.2f" stroke="#2A2A2E" stroke-dasharray="3 3" stroke-width="1"/>""" y svgW y
        ) |> String.concat ""

    let bars =
        items |> Array.mapi (fun i d ->
            let x  = float i * (barW + gapPx)
            let bh = if maxV > 0.0 then d.value / maxV * chartH else 0.0
            let y  = valPad + chartH - bh
            let cx = x + barW / 2.0
            let valStr =
                if props.showValues.Value && bh > 0.0 then
                    sprintf """<text fill="#E8E8ED" font-family="'JetBrains Mono',monospace" font-weight="600" x="%.2f" y="%.2f" text-anchor="middle" font-size="10">%g</text>""" cx (y - 5.0) d.value
                else ""
            let lblStr =
                if props.showLabels.Value then
                    sprintf """<text fill="#6E6E76" font-family="'Sora',sans-serif" x="%.2f" y="%.2f" text-anchor="middle" font-size="11">%s</text>""" cx (valPad + chartH + 16.0) (xmlEncode d.label)
                else ""
            sprintf """<rect x="%.2f" y="%.2f" width="%.2f" height="%.2f" fill="%s" rx="3"/>%s%s"""
                x y barW bh props.color.Value valStr lblStr
        ) |> String.concat ""

    let inner = gridLines + bars

    html $"""
        <svg viewBox="0 0 {svgW} {totalH}" height="{totalH}" preserveAspectRatio="none">
            {unsafeSVG inner}
        </svg>
    """

let register () = ()
