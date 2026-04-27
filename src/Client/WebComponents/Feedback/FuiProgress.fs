module WebComponents.FuiProgress

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-progress-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    .progress-wrap {
        display       : flex;
        flex-direction: column;
        gap           : 0.375rem;
    }

    .header {
        align-items    : center;
        display        : flex;
        justify-content: space-between;
    }

    .label-text {
        color      : var(--fui-progress-label-color, #E8E8ED);
        font-size  : 0.775rem;
        font-weight: 500;
    }

    .value-text {
        color    : var(--fui-progress-value-color, #6E6E76);
        font-size: 0.725rem;
    }

    .track {
        background   : var(--fui-progress-track, #2A2A2E);
        border-radius: 99px;
        height       : 6px;
        overflow     : hidden;
        width        : 100%;
    }

    /* ── Sizes ──────────────────────────────────────────────────────────────── */
    :host([size="sm"]) .track { height: 3px; }
    :host([size="md"]) .track { height: 6px; }
    :host([size="lg"]) .track { height: 10px; }

    .fill {
        background   : var(--fui-progress-fill, #7C3AED);
        border-radius: 99px;
        height       : 100%;
        transition   : width 0.3s ease;
        width        : 0%;
    }

    /* ── Variants ───────────────────────────────────────────────────────────── */
    :host([variant="success"]) .fill { background: #22C55E; }
    :host([variant="warning"]) .fill { background: #F59E0B; }
    :host([variant="danger"])  .fill { background: #EF4444; }
    :host([variant="info"])    .fill { background: #3B82F6; }

    /* ── Indeterminate ──────────────────────────────────────────────────────── */
    @keyframes fui-progress-slide {
        0%   { transform: translateX(-250%); }
        100% { transform: translateX(400%); }
    }

    :host([indeterminate]) .fill {
        animation : fui-progress-slide 1.4s ease-in-out infinite;
        width     : 40%;
        will-change: transform;
    }

    @media (prefers-reduced-motion: reduce) {
        .fill { transition: none; }
        :host([indeterminate]) .fill { animation-duration: 2.5s; }
    }
"""

[<LitElement("fui-progress")>]
let FuiProgress () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            value         = Prop.Of(0,         attribute = "value")
            max           = Prop.Of(100,        attribute = "max")
            label         = Prop.Of("",         attribute = "label")
            size          = Prop.Of("md",       attribute = "size")
            variant       = Prop.Of("default",  attribute = "variant")
            indeterminate = Prop.Of(false,       attribute = "indeterminate")
        |}
    )

    let maxVal = if props.max.Value > 0 then props.max.Value else 100
    let pct    = int (float props.value.Value / float maxVal * 100.0)

    let fillStyle =
        if props.indeterminate.Value then ""
        else "width:" + string pct + "%"

    let header =
        if props.label.Value <> "" then
            let valStr = if props.indeterminate.Value then "—" else string pct + "%"
            html $"""
                <div class="header">
                    <span class="label-text">{props.label.Value}</span>
                    <span class="value-text">{valStr}</span>
                </div>"""
        else Lit.nothing

    html $"""
        <div class="progress-wrap" part="progress-wrap">
            {header}
            <div
                class="track"
                part="track"
                role="progressbar"
                aria-valuenow={string props.value.Value}
                aria-valuemin="0"
                aria-valuemax={string maxVal}>
                <div class="fill" part="fill" style={fillStyle}></div>
            </div>
        </div>
    """

let register () = ()
