module WebComponents.FuiForm

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("$0.type === 'submit'")>]
let private isSubmitType (_: obj) : bool = jsNative

[<Emit("$0.closest('[type=\"submit\"],[data-submit]')")>]
let private closestSubmit (_: obj) : obj = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-form-font, 'Sora', sans-serif);
        width      : 100%;
    }
    :host([hidden]) { display: none; }

    .form {
        display       : flex;
        flex-direction: column;
        gap           : var(--fui-form-gap, 1.25rem);
        position      : relative;
        width         : 100%;
    }

    /* Error banner */
    .form-error {
        background   : rgba(239,68,68,0.08);
        border       : 1px solid rgba(239,68,68,0.3);
        border-radius: 6px;
        color        : #EF4444;
        font-size    : 0.875rem;
        line-height  : 1.5;
        padding      : 0.75rem 1rem;
    }

    /* Loading overlay — semi-transparent, prevents interaction */
    .loading-veil {
        background   : rgba(13,13,15,0.5);
        border-radius: inherit;
        display      : none;
        inset        : 0;
        position     : absolute;
        z-index      : 10;
    }
    :host([loading]) .loading-veil { display: block; }

    .spinner {
        animation    : spin 0.8s linear infinite;
        border       : 2px solid rgba(124,58,237,0.25);
        border-radius: 50%;
        border-top   : 2px solid #7C3AED;
        height       : 1.5rem;
        left         : 50%;
        position     : absolute;
        top          : 50%;
        transform    : translate(-50%, -50%);
        width        : 1.5rem;
    }
    @keyframes spin { to { transform: translate(-50%,-50%) rotate(360deg); } }
"""

[<LitElement("fui-form")>]
let FuiForm () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            error   = Prop.Of("",    attribute = "error")
            loading = Prop.Of(false, attribute = "loading")
        |}
    )

    // Intercept clicks on submit-type buttons in slotted content.
    // Native form submit doesn't cross the shadow boundary, so we detect
    // submit-button clicks via composed event bubbling and fire fui-submit.
    let onWrapClick (e: obj) =
        let target = closestSubmit e
        if not (isNull target) then
            host.dispatchCustomEvent("fui-submit", detail = {| |})

    let errorBanner =
        if props.error.Value <> "" then
            html $"""<div class="form-error" role="alert">{props.error.Value}</div>"""
        else Lit.nothing

    html $"""
        <div class="form" part="form" @click={Ev onWrapClick}>
            <div class="loading-veil" aria-hidden="true">
                <div class="spinner"></div>
            </div>
            {errorBanner}
            <slot></slot>
        </div>
    """

let register () = ()
