module WebComponents.FuiStat

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-stat-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .stat {
        background   : var(--fui-stat-bg, #161618);
        border       : 1px solid var(--fui-stat-border, #2A2A2E);
        border-radius: var(--fui-stat-radius, 10px);
        padding      : 1.25rem;
    }

    .stat-label {
        color         : var(--fui-stat-label-color, #6E6E76);
        font-size     : 0.75rem;
        font-weight   : 600;
        letter-spacing: 0.06em;
        margin        : 0 0 0.5rem;
        text-transform: uppercase;
    }

    .stat-row {
        align-items: baseline;
        display    : flex;
        gap        : 0.625rem;
        flex-wrap  : wrap;
    }

    .stat-value {
        color      : var(--fui-stat-value-color, #E8E8ED);
        font-family: 'JetBrains Mono', monospace;
        font-size  : 2rem;
        font-weight: 700;
        line-height: 1;
    }

    .change {
        border-radius: 4px;
        font-size    : 0.75rem;
        font-weight  : 600;
        padding      : 0.2rem 0.45rem;
    }
    .change.up   { background: rgba(34,197,94,0.12);   color: #22C55E; }
    .change.down { background: rgba(239,68,68,0.12);   color: #EF4444; }
    .change.flat { background: rgba(110,110,118,0.12); color: #6E6E76; }

    .stat-desc {
        color    : var(--fui-stat-desc-color, #6E6E76);
        font-size: 0.8125rem;
        margin   : 0.5rem 0 0;
    }
"""

[<LitElement("fui-stat")>]
let FuiStat () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            label       = Prop.Of("",     attribute = "label")
            value       = Prop.Of("",     attribute = "value")
            change      = Prop.Of("",     attribute = "change")
            trend       = Prop.Of("flat", attribute = "trend")
            description = Prop.Of("",     attribute = "description")
        |}
    )

    let changePart =
        if props.change.Value <> "" then
            let prefix =
                if   props.trend.Value = "up"   then "↑ "
                elif props.trend.Value = "down" then "↓ "
                else ""
            html $"""<span class="change {props.trend.Value}">{prefix}{props.change.Value}</span>"""
        else Lit.nothing

    let descPart =
        if props.description.Value <> "" then
            html $"""<p class="stat-desc">{props.description.Value}</p>"""
        else Lit.nothing

    html $"""
        <div class="stat" part="stat">
            <p class="stat-label">{props.label.Value}</p>
            <div class="stat-row">
                <span class="stat-value">{props.value.Value}</span>
                {changePart}
            </div>
            {descPart}
        </div>
    """

let register () = ()
