module WebComponents.FuiStepper

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("JSON.parse($0)")>]
let private parseJSON (_: string) : {| label: string; description: string |} array = jsNative

[<Emit("parseInt($0, 10) || 0")>]
let private parseInt (_: string) : int = jsNative

let private styles = unsafeCSS """
    :host {
        display: block;
        font-family: var(--fui-st-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    /* ── Horizontal layout ────────────────────────────────────────────────── */
    .stepper {
        display       : flex;
        flex-direction: row;
        align-items   : flex-start;
    }

    .step {
        display    : flex;
        flex: 1 1 0;
        flex-direction: column;
        align-items: center;
        position   : relative;
    }

    /* connector line between steps */
    .step:not(:last-child)::after {
        content   : '';
        position  : absolute;
        top       : 1rem;
        left      : calc(50% + 1rem);
        width     : calc(100% - 2rem);
        height    : 1px;
        background: var(--fui-st-connector, #2A2A2E);
        transition: background 0.2s;
    }
    .step.done:not(:last-child)::after {
        background: var(--fui-st-accent, #7C3AED);
    }

    .step-circle {
        align-items    : center;
        border         : 2px solid var(--fui-st-border, #2A2A2E);
        border-radius  : 50%;
        color          : var(--fui-st-num-color, #6E6E76);
        display        : flex;
        font-size      : 0.75rem;
        font-weight    : 700;
        height         : 2rem;
        justify-content: center;
        position       : relative;
        transition     : background 0.2s, border-color 0.2s, color 0.2s;
        width          : 2rem;
        z-index        : 1;
        background     : var(--fui-st-circle-bg, #1E1E21);
    }
    .step.active .step-circle {
        background  : var(--fui-st-accent, #7C3AED);
        border-color: var(--fui-st-accent, #7C3AED);
        color       : #fff;
    }
    .step.done .step-circle {
        background  : var(--fui-st-accent, #7C3AED);
        border-color: var(--fui-st-accent, #7C3AED);
        color       : #fff;
    }

    /* checkmark for done steps */
    .step.done .step-circle::after {
        content: '✓';
    }

    .step-label {
        color     : var(--fui-st-label-color, #6E6E76);
        font-size : 0.75rem;
        font-weight: 600;
        margin-top : 0.5rem;
        text-align : center;
        transition : color 0.2s;
    }
    .step.active .step-label,
    .step.done   .step-label {
        color: var(--fui-st-label-active, #E8E8ED);
    }

    .step-desc {
        color     : var(--fui-st-desc-color, #6E6E76);
        font-size : 0.7rem;
        margin-top: 0.25rem;
        text-align: center;
    }

    /* ── Vertical variant ─────────────────────────────────────────────────── */
    :host([orientation="vertical"]) .stepper {
        flex-direction: column;
        align-items   : flex-start;
    }
    :host([orientation="vertical"]) .step {
        flex-direction: row;
        align-items   : flex-start;
        flex          : none;
        gap           : 0.75rem;
        padding-bottom: 1.5rem;
    }
    :host([orientation="vertical"]) .step:not(:last-child)::after {
        top   : 2rem;
        left  : 0.9375rem;
        width : 1px;
        height: calc(100% - 2rem);
    }
    :host([orientation="vertical"]) .step-text {
        padding-top: 0.25rem;
    }
    :host([orientation="vertical"]) .step-label {
        text-align: left;
        margin-top: 0;
    }
    :host([orientation="vertical"]) .step-desc {
        text-align: left;
        margin-top: 0.125rem;
    }
"""

[<LitElement("fui-stepper")>]
let FuiStepper () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            steps       = Prop.Of("[]",        attribute = "steps")
            active      = Prop.Of("0",         attribute = "active")
            orientation = Prop.Of("horizontal",attribute = "orientation")
            clickable   = Prop.Of(false,       attribute = "clickable")
        |}
    )

    let opts        = parseJSON props.steps.Value
    let initActive  = parseInt props.active.Value |> max 0
    let activeStep, setActiveStep = Hook.useState initActive

    let goTo (i: int) =
        if props.clickable.Value then
            setActiveStep i
            host.dispatchCustomEvent("fui-change", detail = {| step = i |})

    let renderStep (i: int) (step: {| label: string; description: string |}) =
        let cls =
            if i < activeStep then "step done"
            elif i = activeStep then "step active"
            else "step"
        let numEl =
            if i < activeStep then
                html $"""<span class="step-circle"></span>"""
            else
                html $"""<span class="step-circle">{i + 1}</span>"""
        let descEl =
            if step.description <> "" then
                html $"""<span class="step-desc">{step.description}</span>"""
            else
                Lit.nothing
        let clickAttr =
            if props.clickable.Value then
                html $"""
                    <div class={cls} style="cursor:pointer" @click={Ev (fun _ -> goTo i)}>
                        {numEl}
                        <div class="step-text">
                            <span class="step-label">{step.label}</span>
                            {descEl}
                        </div>
                    </div>"""
            else
                html $"""
                    <div class={cls}>
                        {numEl}
                        <div class="step-text">
                            <span class="step-label">{step.label}</span>
                            {descEl}
                        </div>
                    </div>"""
        clickAttr

    let items = opts |> Array.mapi renderStep

    html $"""
        <div class="stepper" role="list" aria-label="Steps">
            {items}
        </div>
    """

let register () = ()
