module WebComponents.FuiTopNav

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        background   : var(--fui-tnav-bg,     #161618);
        border-bottom: 1px solid var(--fui-tnav-border, #2A2A2E);
        display      : block;
        font-family  : var(--fui-tnav-font,   'JetBrains Mono', monospace);
        position     : sticky;
        top          : 0;
        z-index      : 100;
    }
    :host([hidden]) { display: none; }

    .bar {
        align-items    : center;
        display        : flex;
        gap            : 1.5rem;
        height         : var(--fui-tnav-height, 52px);
        margin         : 0 auto;
        max-width      : var(--fui-tnav-max-width, 1100px);
        padding        : 0 1.5rem;
    }

    .logo {
        align-items: center;
        color      : var(--fui-tnav-logo-color, #E8E8ED);
        display    : flex;
        flex-shrink: 0;
        font-size  : 1rem;
        font-weight: 700;
        gap        : 0.5rem;
        text-decoration: none;
    }
    .logo-mark {
        background   : #7C3AED;
        border-radius: 4px;
        display      : inline-block;
        height       : 20px;
        width        : 20px;
    }

    .links {
        align-items: center;
        display    : flex;
        flex       : 1;
        gap        : 0.25rem;
    }

    /* Styles for nav links placed in the links slot */
    ::slotted(a) {
        border-radius : 5px;
        color         : #6E6E76;
        font-size     : 0.875rem;
        padding       : 0.35rem 0.625rem;
        text-decoration: none;
        transition    : color 0.15s, background 0.15s;
    }
    ::slotted(a:hover)   { background: rgba(124,58,237,0.08); color: #E8E8ED; }
    ::slotted(a.active)  { color: #9B59F5; font-weight: 600; }

    .actions {
        align-items: center;
        display    : flex;
        flex-shrink: 0;
        gap        : 0.5rem;
    }

    /* Styles for action buttons placed in the actions slot */
    ::slotted(button) {
        background   : none;
        border       : 1px solid #2A2A2E;
        border-radius: 5px;
        color        : #A0A0A8;
        cursor       : pointer;
        font-family  : 'JetBrains Mono', monospace;
        font-size    : 0.8125rem;
        padding      : 0.35rem 0.625rem;
        transition   : border-color 0.15s, color 0.15s;
    }
    ::slotted(button:hover) { border-color: #7C3AED; color: #E8E8ED; }
"""

[<LitElement("fui-topnav")>]
let FuiTopNav () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            logo = Prop.Of("App", attribute = "logo")
        |}
    )

    html $"""
        <div class="bar" part="bar">
            <span class="logo" part="logo">
                <span class="logo-mark" aria-hidden="true"></span>
                {props.logo.Value}
            </span>
            <nav class="links" aria-label="Main navigation">
                <slot></slot>
            </nav>
            <div class="actions">
                <slot name="actions"></slot>
            </div>
        </div>
    """

let register () = ()
