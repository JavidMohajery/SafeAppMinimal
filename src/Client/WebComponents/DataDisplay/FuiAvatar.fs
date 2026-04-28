module WebComponents.FuiAvatar

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

let private styles = unsafeCSS """
    :host {
        display    : inline-block;
        font-family: var(--fui-avatar-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    .wrap {
        border-radius: 50%;
        display      : inline-flex;
        flex-shrink  : 0;
        position     : relative;
    }

    /* ── Sizes ── */
    .wrap.xs { height:24px; width:24px; font-size:0.6rem;   }
    .wrap.sm { height:32px; width:32px; font-size:0.75rem;  }
    .wrap.md { height:40px; width:40px; font-size:0.875rem; }
    .wrap.lg { height:56px; width:56px; font-size:1.1rem;   }
    .wrap.xl { height:72px; width:72px; font-size:1.35rem;  }

    .avatar-img {
        border-radius: 50%;
        display      : block;
        height       : 100%;
        object-fit   : cover;
        width        : 100%;
    }

    .avatar-initials {
        align-items    : center;
        background     : var(--fui-avatar-bg, #2A2A2E);
        border-radius  : 50%;
        color          : var(--fui-avatar-color, #E8E8ED);
        display        : flex;
        font-weight    : 600;
        height         : 100%;
        justify-content: center;
        letter-spacing : 0.02em;
        text-transform : uppercase;
        width          : 100%;
    }

    .status {
        border       : 2px solid var(--fui-avatar-status-border, #0D0D0F);
        border-radius: 50%;
        bottom       : 0;
        height       : 28%;
        position     : absolute;
        right        : 0;
        width        : 28%;
    }
    .status.online  { background: #22C55E; }
    .status.offline { background: #6E6E76; }
    .status.away    { background: #F59E0B; }
    .status.busy    { background: #EF4444; }
"""

[<LitElement("fui-avatar")>]
let FuiAvatar () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            src      = Prop.Of("",   attribute = "src")
            initials = Prop.Of("?",  attribute = "initials")
            size     = Prop.Of("md", attribute = "size")
            status   = Prop.Of("",   attribute = "status")
        |}
    )

    let inner =
        if props.src.Value <> "" then
            html $"""<img class="avatar-img" src={props.src.Value} alt={props.initials.Value} />"""
        else
            html $"""<div class="avatar-initials">{props.initials.Value}</div>"""

    let statusDot =
        if props.status.Value <> "" then
            html $"""<span class="status {props.status.Value}" aria-label={props.status.Value}></span>"""
        else Lit.nothing

    html $"""
        <div class="wrap {props.size.Value}" part="avatar">
            {inner}
            {statusDot}
        </div>
    """

let register () = ()
