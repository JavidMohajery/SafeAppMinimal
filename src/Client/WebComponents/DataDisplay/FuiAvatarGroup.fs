module WebComponents.FuiAvatarGroup

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("JSON.parse($0)")>]
let private parseAvatars (_: string) : {| src: string; initials: string |} array = jsNative

let private styles = unsafeCSS """
    :host {
        display    : inline-block;
        font-family: var(--fui-ag-font, 'JetBrains Mono', monospace);
    }
    :host([hidden]) { display: none; }

    .group {
        align-items: center;
        display    : flex;
    }

    .group.sm { --av-size:28px; --av-font:0.625rem; }
    .group.md { --av-size:36px; --av-font:0.75rem;  }
    .group.lg { --av-size:48px; --av-font:0.9rem;   }

    .avatar {
        align-items    : center;
        background     : var(--fui-ag-bg, #2A2A2E);
        border         : 2px solid var(--fui-ag-ring, #0D0D0F);
        border-radius  : 50%;
        color          : var(--fui-ag-color, #E8E8ED);
        display        : flex;
        flex-shrink    : 0;
        font-size      : var(--av-font, 0.75rem);
        font-weight    : 600;
        height         : var(--av-size, 36px);
        justify-content: center;
        margin-left    : -8px;
        overflow       : hidden;
        text-transform : uppercase;
        width          : var(--av-size, 36px);
    }
    .avatar:first-child { margin-left: 0; }

    .avatar img {
        height    : 100%;
        object-fit: cover;
        width     : 100%;
    }

    .overflow {
        align-items    : center;
        background     : var(--fui-ag-overflow-bg, #1E1E21);
        border         : 2px solid var(--fui-ag-ring, #0D0D0F);
        border-radius  : 50%;
        color          : var(--fui-ag-overflow-color, #6E6E76);
        display        : flex;
        flex-shrink    : 0;
        font-size      : var(--av-font, 0.75rem);
        font-weight    : 600;
        height         : var(--av-size, 36px);
        justify-content: center;
        margin-left    : -8px;
        width          : var(--av-size, 36px);
    }
"""

[<LitElement("fui-avatar-group")>]
let FuiAvatarGroup () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            avatars = Prop.Of("[]", attribute = "avatars")
            max     = Prop.Of(4,    attribute = "max")
            size    = Prop.Of("md", attribute = "size")
        |}
    )

    let all     = parseAvatars props.avatars.Value
    let maxShow = max 1 props.max.Value
    let visible = all |> Array.truncate maxShow
    let extra   = all.Length - visible.Length

    let renderAvatar (av: {| src: string; initials: string |}) =
        if av.src <> "" then
            html $"""<div class="avatar"><img src={av.src} alt={av.initials} /></div>"""
        else
            html $"""<div class="avatar">{av.initials}</div>"""

    let avatarEls  = visible |> Array.map renderAvatar
    let overflowEl =
        if extra > 0 then html $"""<div class="overflow">+{extra}</div>"""
        else Lit.nothing

    html $"""
        <div class="group {props.size.Value}" part="group">
            {avatarEls}
            {overflowEl}
        </div>
    """

let register () = ()
