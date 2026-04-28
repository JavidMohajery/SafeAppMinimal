module WebComponents.FuiSparkline

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Import("unsafeSVG", "lit/directives/unsafe-svg.js")>]
let private unsafeSVG: string -> obj = jsNative

[<Emit("JSON.parse($0)")>]
let private parseData (s: string) : float array = jsNative

let private styles = unsafeCSS """
    :host { display: inline-block; vertical-align: middle; }
    :host([hidden]) { display: none; }
    svg { display: block; }
"""

[<LitElement("fui-sparkline")>]
let FuiSparkline () =
    let _, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props <- {|
            data        = Prop.Of("[]",      attribute = "data")
            width       = Prop.Of(120,       attribute = "width")
            height      = Prop.Of(40,        attribute = "height")
            color       = Prop.Of("#7C3AED", attribute = "color")
            fill        = Prop.Of(true,      attribute = "fill")
            strokeWidth = Prop.Of(2,         attribute = "stroke-width")
        |}
    )

    let values = parseData props.data.Value
    let count  = values.Length
    let w   = float props.width.Value
    let h   = float props.height.Value

    if count = 0 then html $"""<svg width="{w}" height="{h}"></svg>"""
    else

    let sw    = float props.strokeWidth.Value
    let maxV  = Array.max values
    let minV  = Array.min values
    let rng   = maxV - minV
    let safeR = if rng < 1e-9 then 1.0 else rng

    let xOf i = if count = 1 then w / 2.0 else sw + float i * (w - 2.0 * sw) / float (count - 1)
    let yOf v  = sw + (h - 2.0 * sw) * (1.0 - (v - minV) / safeR)

    let linePath =
        values |> Array.mapi (fun i v ->
            let cmd = if i = 0 then "M" else "L"
            sprintf "%s %.2f %.2f" cmd (xOf i) (yOf v)
        ) |> String.concat " "

    let areaPath =
        sprintf "%s L %.2f %.2f L %.2f %.2f Z" linePath (xOf (count - 1)) h (xOf 0) h

    let fillStr =
        if props.fill.Value then
            sprintf """<path d="%s" fill="%s" opacity="0.15"/>""" areaPath props.color.Value
        else ""

    let lineStr =
        sprintf """<path d="%s" fill="none" stroke="%s" stroke-width="%.1f" stroke-linejoin="round" stroke-linecap="round"/>"""
            linePath props.color.Value sw

    html $"""
        <svg viewBox="0 0 {w} {h}" width="{w}" height="{h}">
            {unsafeSVG (fillStr + lineStr)}
        </svg>
    """

let register () = ()
