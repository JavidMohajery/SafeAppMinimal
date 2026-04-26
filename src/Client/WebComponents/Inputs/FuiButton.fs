module WebComponents.FuiButton

open Lit
open Fable.Core

// css $"..." uses F# FormattableString, so { } in CSS blocks would need {{ }}.
// Fable emits JS tagged templates that keep {{ literal, breaking CSS.
// Solution: bind Lit's unsafeCSS, which accepts a plain string with real { }.
[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : inline-block;
        font-family: var(--fui-button-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    button {
        background    : var(--fui-button-bg, #7C3AED);
        color         : var(--fui-button-color, #fff);
        border        : var(--fui-button-border, none);
        border-radius : var(--fui-button-radius, 6px);
        padding       : 0.5rem 1.25rem;
        font-family   : inherit;
        font-size     : 0.875rem;
        font-weight   : 600;
        letter-spacing: -0.01em;
        cursor        : pointer;
        transition    : background 0.12s, opacity 0.12s;
        line-height   : 1;
    }
    button:hover        { background: var(--fui-button-bg-hover, #9B59F5); }
    button:focus-visible {
        outline       : 2px solid var(--fui-button-bg, #7C3AED);
        outline-offset: 2px;
    }

    :host([disabled]) button {
        opacity       : 0.42;
        cursor        : not-allowed;
        pointer-events: none;
    }

    /* ── Variants ─────────────────────────────────────────────── */
    :host([variant="secondary"]) button {
        background: transparent;
        color     : var(--fui-button-bg, #7C3AED);
        border    : 1px solid var(--fui-button-bg, #7C3AED);
    }
    :host([variant="secondary"]) button:hover {
        background: rgba(124, 58, 237, 0.10);
    }

    :host([variant="ghost"]) button {
        background: transparent;
        color     : var(--fui-button-ghost-color, #E8E8ED);
        border    : 1px solid transparent;
    }
    :host([variant="ghost"]) button:hover {
        background  : rgba(255, 255, 255, 0.07);
        border-color: rgba(255, 255, 255, 0.12);
    }

    :host([variant="danger"]) button {
        background: var(--fui-button-danger-bg, #EF4444);
    }
    :host([variant="danger"]) button:hover { background: #DC2626; }

    /* ── Sizes ────────────────────────────────────────────────── */
    :host([size="sm"]) button { font-size: 0.75rem;  padding: 0.3rem  0.75rem; }
    :host([size="md"]) button { font-size: 0.875rem; padding: 0.5rem  1.25rem; }
    :host([size="lg"]) button { font-size: 1rem;     padding: 0.75rem 1.75rem; }
"""

[<LitElement("fui-button")>]
let FuiButton() =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            variant  = Prop.Of("primary", attribute = "variant")
            disabled = Prop.Of(false,     attribute = "disabled")
            size     = Prop.Of("md",      attribute = "size")
        |}
    )

    html $"""
        <button
            part="button"
            ?disabled={props.disabled.Value}
            data-variant={props.variant.Value}>
            <slot name="prefix"></slot>
            <slot></slot>
            <slot name="suffix"></slot>
        </button>
    """

// Referencing this from ComponentRegistry forces module load,
// which triggers [<LitElement("fui-button")>] → customElements.define().
let register () = ()
