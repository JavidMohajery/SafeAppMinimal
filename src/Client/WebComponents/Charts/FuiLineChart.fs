module WebComponents.FuiLineChart

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

[<LitElement("fui-line-chart")>]
let FuiLineChart () =
    let _, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props <- {|
            data       = Prop.Of("[]",      attribute = "data")
            height     = Prop.Of(160,       attribute = "height")
            color      = Prop.Of("#7C3AED", attribute = "color")
            fill       = Prop.Of(false,     attribute = "fill")
            showDots   = Prop.Of(true,      attribute = "show-dots")
            showLabels = Prop.Of(true,      attribute = "show-labels")
        |}
    )

    let items = parseData props.data.Value
    let count = items.Length

    if count = 0 then html $"""<svg></svg>"""
    else

    let svgW    = 600.0
    let chartH  = float props.height.Value
    let labelH  = if props.showLabels.Value then 22.0 else 0.0
    let padL    = 6.0
    let padR    = 6.0
    let totalH  = chartH + labelH
    let usableW = svgW - padL - padR
    let values  = items |> Array.map (fun d -> d.value)
    let maxV    = Array.max values
    let minV    = Array.min values
    let range   = maxV - minV
    let safeR   = if range < 1e-9 then 1.0 else range

    let xOf i = padL + (if count = 1 then usableW / 2.0 else float i * usableW / float (count - 1))
    let yOf v  = 4.0 + (chartH - 8.0) * (1.0 - (v - minV) / safeR)

    let linePath =
        items |> Array.mapi (fun i d ->
            let cmd = if i = 0 then "M" else "L"
            sprintf "%s %.2f %.2f" cmd (xOf i) (yOf d.value)
        ) |> String.concat " "

    let areaPath =
        sprintf "%s L %.2f %.2f L %.2f %.2f Z" linePath (xOf (count - 1)) chartH (xOf 0) chartH

    let gridLines =
        [| 0.0; 0.5; 1.0 |]
        |> Array.map (fun f ->
            let y = yOf (minV + f * range)
            sprintf """<line x1="0" y1="%.2f" x2="%.2f" y2="%.2f" stroke="#2A2A2E" stroke-dasharray="3 3" stroke-width="1"/>""" y svgW y
        ) |> String.concat ""

    let fillStr =
        if props.fill.Value then
            sprintf """<path d="%s" fill="%s" opacity="0.12"/>""" areaPath props.color.Value
        else ""

    let dots =
        if props.showDots.Value then
            items |> Array.mapi (fun i d ->
                sprintf """<circle cx="%.2f" cy="%.2f" r="4" fill="%s" stroke="#0D0D0F" stroke-width="2"/>"""
                    (xOf i) (yOf d.value) props.color.Value
            ) |> String.concat ""
        else ""

    let labels =
        if props.showLabels.Value then
            items |> Array.mapi (fun i d ->
                sprintf """<text fill="#6E6E76" font-family="'Sora',sans-serif" x="%.2f" y="%.2f" text-anchor="middle" font-size="11">%s</text>"""
                    (xOf i) (totalH - 4.0) (xmlEncode d.label)
            ) |> String.concat ""
        else ""

    let lineStr =
        sprintf """<path d="%s" fill="none" stroke="%s" stroke-width="2.5" stroke-linejoin="round" stroke-linecap="round"/>"""
            linePath props.color.Value

    let inner = gridLines + fillStr + lineStr + dots + labels

    html $"""
        <svg viewBox="0 0 {svgW} {totalH}" height="{totalH}" preserveAspectRatio="none">
            {unsafeSVG inner}
        </svg>
    """

let register () = ()
