module WebComponents.FuiSkeleton

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display: block;
    }
    :host([hidden]) { display: none; }

    @keyframes fui-shimmer {
        0%   { background-position: -200% 0; }
        100% { background-position:  200% 0; }
    }

    .sk {
        animation      : fui-shimmer 1.6s ease-in-out infinite;
        background     : linear-gradient(90deg,
            var(--fui-sk-base, #1E1E21) 25%,
            var(--fui-sk-shine, #2A2A2E) 50%,
            var(--fui-sk-base, #1E1E21) 75%);
        background-size: 200% 100%;
        border-radius  : var(--fui-sk-radius, 4px);
        display        : block;
    }

    /* circle variant */
    :host([variant="circle"]) .sk { border-radius: 50%; }

    /* text variant — multiple lines */
    .sk-line {
        display: block;
    }
    .sk-line + .sk-line {
        margin-top: 0.5rem;
    }
    /* last line of a multi-line block is shorter */
    .sk-line:last-child:not(:first-child) {
        width: 65% !important;
    }

    @media (prefers-reduced-motion: reduce) {
        .sk { animation: none; background: var(--fui-sk-base, #1E1E21); }
    }
"""

[<LitElement("fui-skeleton")>]
let FuiSkeleton () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            variant = Prop.Of("rect", attribute = "variant")
            width   = Prop.Of("",     attribute = "width")
            height  = Prop.Of("",     attribute = "height")
            lines   = Prop.Of(1,      attribute = "lines")
        |}
    )

    match props.variant.Value with
    | "text" ->
        let count = max 1 props.lines.Value
        let lineH = if props.height.Value <> "" then props.height.Value else "0.875rem"
        let lineW = if props.width.Value   <> "" then props.width.Value  else "100%"
        let items =
            [| 1..count |]
            |> Array.map (fun _ ->
                html $"""<span class="sk sk-line" style="height:{lineH};width:{lineW}"></span>""")
        html $"""<div part="skeleton">{items}</div>"""
    | "circle" ->
        let sz = if props.width.Value <> "" then props.width.Value else "40px"
        html $"""<span class="sk" part="skeleton" style="width:{sz};height:{sz}"></span>"""
    | _ ->
        let w = if props.width.Value  <> "" then props.width.Value  else "100%"
        let h = if props.height.Value <> "" then props.height.Value else "1rem"
        html $"""<span class="sk" part="skeleton" style="width:{w};height:{h}"></span>"""

let register () = ()
