module WebComponents.FuiCarousel

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("JSON.parse($0)")>]
let private parseItems (_: string) : {| title: string; description: string |} array = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-carousel-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .carousel {
        background   : var(--fui-carousel-bg, #161618);
        border       : 1px solid var(--fui-carousel-border, #2A2A2E);
        border-radius: var(--fui-carousel-radius, 10px);
        overflow     : hidden;
    }

    .slide {
        display        : flex;
        flex-direction : column;
        justify-content: center;
        min-height     : 160px;
        padding        : 2rem 2rem 1.5rem;
    }

    .slide-title {
        color      : var(--fui-carousel-title-color, #E8E8ED);
        font-family: 'JetBrains Mono', monospace;
        font-size  : 1.125rem;
        font-weight: 700;
        margin     : 0 0 0.5rem;
    }

    .slide-desc {
        color      : var(--fui-carousel-desc-color, #A0A0A8);
        font-size  : 0.9375rem;
        line-height: 1.6;
        margin     : 0;
    }

    .controls {
        align-items    : center;
        border-top     : 1px solid var(--fui-carousel-border, #2A2A2E);
        display        : flex;
        justify-content: space-between;
        padding        : 0.625rem 1rem;
    }

    .nav-btn {
        background   : none;
        border       : 1px solid var(--fui-carousel-btn-border, #2A2A2E);
        border-radius: 6px;
        color        : var(--fui-carousel-btn-color, #E8E8ED);
        cursor       : pointer;
        font-family  : 'JetBrains Mono', monospace;
        font-size    : 0.8125rem;
        padding      : 0.35rem 0.75rem;
        transition   : border-color 0.15s;
    }
    .nav-btn:hover:not(:disabled) { border-color: #7C3AED; }
    .nav-btn:disabled { cursor: not-allowed; opacity: 0.3; }
    .nav-btn:focus-visible {
        outline       : 2px solid #7C3AED;
        outline-offset: 2px;
    }

    .dots {
        align-items: center;
        display    : flex;
        gap        : 0.4rem;
    }

    .dot {
        background   : var(--fui-carousel-dot, #2A2A2E);
        border       : none;
        border-radius: 50%;
        cursor       : pointer;
        height       : 7px;
        padding      : 0;
        transition   : background 0.15s, transform 0.15s;
        width        : 7px;
    }
    .dot.active { background: #7C3AED; transform: scale(1.3); }
    .dot:focus-visible {
        outline       : 2px solid #7C3AED;
        outline-offset: 2px;
    }

    .counter {
        color      : var(--fui-carousel-counter-color, #6E6E76);
        font-family: 'JetBrains Mono', monospace;
        font-size  : 0.75rem;
    }
"""

[<LitElement("fui-carousel")>]
let FuiCarousel () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            items    = Prop.Of("[]",  attribute = "items")
            showDots = Prop.Of(true,  attribute = "show-dots")
            loop     = Prop.Of(false, attribute = "loop")
        |}
    )

    let items   = parseItems props.items.Value
    let count   = items.Length
    let current, setCurrent = Hook.useState 0

    let go n _ =
        let clamped =
            if n < 0 then (if props.loop.Value then count - 1 else 0)
            elif n >= count then (if props.loop.Value then 0 else count - 1)
            else n
        setCurrent clamped
        host.dispatchCustomEvent("fui-change", detail = {| index = clamped |})

    if count = 0 then
        html $"""
            <div class="carousel">
                <div class="slide"><p class="slide-desc">No slides.</p></div>
            </div>
        """
    else
        let slide    = items.[current]
        let canPrev  = props.loop.Value || current > 0
        let canNext  = props.loop.Value || current < count - 1

        let middle =
            if props.showDots.Value then
                let dotEls =
                    [| 0 .. count - 1 |]
                    |> Array.map (fun i ->
                        let cls = if i = current then "dot active" else "dot"
                        html $"""<button class={cls} aria-label={$"Slide {i+1}"} @click={Ev (go i)}></button>""")
                html $"""<div class="dots">{dotEls}</div>"""
            else
                html $"""<span class="counter">{current + 1} / {count}</span>"""

        html $"""
            <div class="carousel" part="carousel">
                <div class="slide" part="slide">
                    <p class="slide-title">{slide.title}</p>
                    <p class="slide-desc">{slide.description}</p>
                </div>
                <div class="controls">
                    <button class="nav-btn" ?disabled={not canPrev} @click={Ev (go (current - 1))}>← Prev</button>
                    {middle}
                    <button class="nav-btn" ?disabled={not canNext} @click={Ev (go (current + 1))}>Next →</button>
                </div>
            </div>
        """

let register () = ()
