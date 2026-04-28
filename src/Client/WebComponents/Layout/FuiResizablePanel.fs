module WebComponents.FuiResizablePanel

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

// Pointer position helpers
[<Emit("$0.clientX")>]
let private clientX (_: obj) : float = jsNative
[<Emit("$0.clientY")>]
let private clientY (_: obj) : float = jsNative

// Pointer capture — keeps receiving events even when cursor leaves the element
[<Emit("($0.currentTarget||$0.target).setPointerCapture($0.pointerId)")>]
let private capturePointer (_: obj) : unit = jsNative
[<Emit("($0.currentTarget||$0.target).releasePointerCapture($0.pointerId)")>]
let private releasePointer (_: obj) : unit = jsNative

// Store drag-start values on the handle element itself to avoid stale-closure issues.
// These are write-once on pointerdown and read on each pointermove — no re-render needed.
[<Emit("($0.currentTarget||$0.target).__sp = $1")>]
let private storeStartPos (_: obj) (v: float) : unit = jsNative
[<Emit("($0.currentTarget||$0.target).__ss = $1")>]
let private storeStartSize (_: obj) (v: int) : unit = jsNative
[<Emit("($0.currentTarget||$0.target).__sp")>]
let private loadStartPos (_: obj) : float = jsNative
[<Emit("($0.currentTarget||$0.target).__ss")>]
let private loadStartSize (_: obj) : int = jsNative

let private styles = unsafeCSS """
    :host {
        display : block;
        height  : 100%;
        overflow: hidden;
        width   : 100%;
    }
    :host([hidden]) { display: none; }

    .container {
        display: flex;
        height : 100%;
        width  : 100%;
    }
    .container.vertical { flex-direction: column; }

    .panel {
        min-height: 0;
        min-width : 0;
        overflow  : auto;
    }
    .panel-end { flex: 1; }

    /* ── Drag handle ────────────────────────────────────────────────────────── */
    .handle {
        align-items    : center;
        background     : var(--fui-rp-handle-bg, transparent);
        cursor         : col-resize;
        display        : flex;
        flex-shrink    : 0;
        justify-content: center;
        padding        : 0 4px;
        touch-action   : none;
        transition     : background 0.15s;
        z-index        : 1;
    }
    .container.vertical .handle { cursor: row-resize; padding: 4px 0; }

    .handle:hover,
    .handle.dragging { background: var(--fui-rp-handle-hover, rgba(124,58,237,0.08)); }

    .handle-bar {
        background   : var(--fui-rp-bar-color, #2A2A2E);
        border-radius: 2px;
        height       : 32px;
        pointer-events: none;
        transition   : background 0.15s;
        width        : 3px;
    }
    .container.vertical .handle-bar { height: 3px; width: 32px; }

    .handle:hover .handle-bar,
    .handle.dragging .handle-bar { background: var(--fui-rp-bar-hover, #7C3AED); }

    /* Focus ring on keyboard access */
    .handle:focus-visible {
        background    : rgba(124,58,237,0.08);
        outline       : 2px solid #7C3AED;
        outline-offset: -2px;
    }
"""

[<LitElement("fui-resizable-panel")>]
let FuiResizablePanel () =
    let host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            defaultSize = Prop.Of(300,          attribute = "default-size")
            minSize     = Prop.Of(80,           attribute = "min-size")
            maxSize     = Prop.Of(800,          attribute = "max-size")
            direction   = Prop.Of("horizontal", attribute = "direction")
        |}
    )

    let size,       setSize      = Hook.useState props.defaultSize.Value
    let isDragging, setDragging  = Hook.useState false

    let isVertical = props.direction.Value = "vertical"
    let getPos (e: obj) = if isVertical then clientY e else clientX e

    let onPointerDown (e: obj) =
        capturePointer e
        storeStartPos  e (getPos e)
        storeStartSize e size
        setDragging true

    let onPointerMove (e: obj) =
        let delta   = getPos e - loadStartPos e
        let raw     = loadStartSize e + int delta
        let clamped = max props.minSize.Value (min props.maxSize.Value raw)
        setSize clamped

    let onPointerUp (e: obj) =
        releasePointer e
        setDragging false
        host.dispatchCustomEvent("fui-resize", detail = {| size = size |})

    let containerCls = if isVertical then "container vertical" else "container"
    let handleCls    = if isDragging then "handle dragging" else "handle"
    let panelStyle   = if isVertical then $"height:{size}px" else $"width:{size}px"

    html $"""
        <div class={containerCls} part="container">
            <div class="panel panel-start" style={panelStyle} part="start">
                <slot name="start"></slot>
            </div>
            <div class={handleCls}
                 part="handle"
                 role="separator"
                 aria-orientation={if isVertical then "horizontal" else "vertical"}
                 aria-label="Resize panels"
                 tabindex="0"
                 @pointerdown={Ev onPointerDown}
                 @pointermove={Ev onPointerMove}
                 @pointerup={Ev onPointerUp}>
                <div class="handle-bar"></div>
            </div>
            <div class="panel panel-end" part="end">
                <slot name="end"></slot>
            </div>
        </div>
    """

let register () = ()
