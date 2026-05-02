module Client

open Browser
open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Fable.Core
// Register all Web Components with the browser before React starts rendering.
do ComponentRegistry.registerAll()

type Page = Home | GettingStarted | CategoryIndex of string | BackendDemo of string | CounterElmish | CounterDom | Component of string * string

type FetchState = Idle | Fetching | Done of int * string | Failed of string

type Theme = Dark | Light

type CrudFormState = {
    GetId           : string
    CreateTitle     : string
    CreateCompleted : bool
    UpdateId        : string
    UpdateTitle     : string
    UpdateCompleted : bool
    DeleteId        : string
}

type PagFormState = {
    Filter   : string
    SortBy   : string
    SortDir  : string
    PageSize : string
    Page     : int
}

type AuthFormState = {
    Username : string
    Password : string
    Token    : string
}

type WsStatus = WsIdle | WsConnecting | WsOpen | WsGone

type Model = {
    Page           : Page
    Hash           : string
    ElmishCounter  : Pages.CounterElmish.Model
    BackendResults : Map<string, FetchState>
    Theme          : Theme
    SidebarOpen    : bool
    CrudForm       : CrudFormState
    PagForm        : PagFormState
    AuthForm       : AuthFormState
    RateLimitLog   : (int * string) list
    JobPolling     : bool
    JobLog         : (string * string) list   // (jobId, latestJson), newest first
    WsStatus       : WsStatus
    WsLog          : (string * string) list   // (kind, text): "sent"|"recv"|"info"|"error"
    WsInput        : string
    UploadFile     : obj option               // selected File object (JS)
    SseLog         : string list              // received event data strings, newest first
    SseActive      : bool
}

type Msg =
    | UrlChanged of string
    | ElmishCounterMsg of Pages.CounterElmish.Msg
    | BackendFetch   of string * string
    | BackendReceive of string * int * string
    | BackendError   of string * string
    | ToggleTheme
    | ToggleSidebar
    | CloseSidebar
    | CrudFormStr  of string * string
    | CrudFormBool of string * bool
    | CrudSend     of string * string * string * string
    | PagFormStr   of string * string
    | PagFetch     of int
    | AuthFormStr  of string * string
    | AuthLogin
    | AuthReceive  of string * int * string
    | AuthCallMe
    | RateLimitFire
    | RateLimitHammer
    | RateLimitReceive of int * string
    | RateLimitClear
    | JobStart
    | JobStarted    of int * string
    | JobPoll       of string
    | JobPolled     of string * int * string
    | WsConnect
    | WsDisconnect
    | WsOpened
    | WsClosed
    | WsSend
    | WsReceive     of string
    | WsNetError    of string
    | WsInputStr    of string
    | WsLogClear
    | UploadFileSelected of obj
    | UploadSend
    | UploadClear
    | SseConnect
    | SseDisconnect
    | SseReceive    of string
    | SseDone
    | SseLogClear

let urlCatLabel (slug: string) =
    match slug with
    | "inputs"       -> "Inputs & Forms"
    | "layout"       -> "Layout"
    | "navigation"   -> "Navigation"
    | "feedback"     -> "Feedback"
    | "overlay"      -> "Overlay"
    | "data-display" -> "Data Display"
    | "typography"   -> "Typography"
    | "charts"       -> "Charts"
    | other          -> other

let urlCatToRegistry (slug: string) =
    match slug with
    | "inputs"       -> "Inputs & Forms"
    | "data-display" -> "Data Display"
    | other          -> System.Char.ToUpper(other.[0]).ToString() + other.[1..]

let parsePage (hash: string) =
    match hash.TrimStart('#').TrimStart('/').Split('/') |> Array.toList with
    | "counter-elmish"  :: _ -> CounterElmish
    | "counter-dom"     :: _ -> CounterDom
    | "getting-started" :: _ -> GettingStarted
    | "category"        :: cat  :: _    -> CategoryIndex cat
    | "backend"         :: slug :: _    -> BackendDemo slug
    | "component"       :: cat :: slug :: _ -> Component(cat, slug)
    | _ -> Home

let init () =
    let counter, _ = Pages.CounterElmish.init ()
    let hash = window.location.hash
    let defaultCrud = { GetId = "1"; CreateTitle = ""; CreateCompleted = false; UpdateId = "1"; UpdateTitle = ""; UpdateCompleted = false; DeleteId = "1" }
    let defaultPag  = { Filter = ""; SortBy = "id"; SortDir = "asc"; PageSize = "5"; Page = 1 }
    let defaultAuth = { Username = "admin"; Password = "password123"; Token = "" }
    { Page = parsePage hash; Hash = hash; ElmishCounter = counter; BackendResults = Map.empty; Theme = Dark; SidebarOpen = false; CrudForm = defaultCrud; PagForm = defaultPag; AuthForm = defaultAuth; RateLimitLog = []; JobPolling = false; JobLog = []; WsStatus = WsIdle; WsLog = []; WsInput = ""; UploadFile = None; SseLog = []; SseActive = false }, Cmd.none

[<Fable.Core.Emit("fetch($0).then(r=>r.text().then(b=>({status:r.status,body:b})))")>]
let private fetchGet (url: string) : Fable.Core.JS.Promise<{| status: int; body: string |}> = jsNative

[<Fable.Core.Emit("fetch($0,(b=>Object.assign({method:$1},b?{headers:{'Content-Type':'application/json'},body:b}:{}))($2)).then(r=>r.text().then(b=>({status:r.status,body:b})))")>]
let private fetchJson (url: string) (method: string) (body: string) : Fable.Core.JS.Promise<{| status: int; body: string |}> = jsNative

[<Fable.Core.Emit("JSON.stringify({title:$0,completed:$1})")>]
let private itemJson (title: string) (completed: bool) : string = jsNative

[<Fable.Core.Emit("encodeURIComponent($0)")>]
let private encodeQuery (s: string) : string = jsNative

[<Fable.Core.Emit("(function(s){try{var o=JSON.parse(s);return{totalPages:o.totalPages||1,page:o.page||1,total:o.total||0}}catch(_){return{totalPages:1,page:1,total:0}}}($0))")>]
let private parsePagMeta (json: string) : {| totalPages: int; page: int; total: int |} = jsNative

[<Fable.Core.Emit("fetch($0,{headers:{Authorization:'Bearer '+$1}}).then(r=>r.text().then(b=>({status:r.status,body:b})))")>]
let private fetchBearer (url: string) (token: string) : Fable.Core.JS.Promise<{| status: int; body: string |}> = jsNative

[<Fable.Core.Emit("JSON.stringify({username:$0,password:$1})")>]
let private loginJson (username: string) (password: string) : string = jsNative

[<Fable.Core.Emit("(function(s){try{return JSON.parse(s).token||''}catch(_){return ''}}($0))")>]
let private extractToken (json: string) : string = jsNative

[<Fable.Core.Emit("new Promise(r=>setTimeout(r,$1)).then(()=>fetch($0).then(res=>res.text().then(b=>({status:res.status,body:b}))))")>]
let private fetchGetDelayed (url: string) (delayMs: int) : Fable.Core.JS.Promise<{| status: int; body: string |}> = jsNative

[<Fable.Core.Emit("(function(s){try{return JSON.parse(s).id||''}catch(_){return ''}}($0))")>]
let private extractJobId (json: string) : string = jsNative

[<Fable.Core.Emit("$0.key")>]
let private eventKey (e: Browser.Types.Event) : string = jsNative

[<Fable.Core.Emit("new WebSocket($0)")>]
let private newWs (url: string) : obj = jsNative

[<Fable.Core.Emit("(function(ws,onOpen,onMsg,onClose,onErr){ws.onopen=onOpen;ws.onmessage=function(e){onMsg(e.data);};ws.onclose=onClose;ws.onerror=onErr;})($0,$1,$2,$3,$4)")>]
let private attachWsHandlers (ws: obj) (onOpen: unit -> unit) (onMsg: string -> unit) (onClose: unit -> unit) (onErr: unit -> unit) : unit = jsNative

[<Fable.Core.Emit("$0.send($1)")>]
let private wsSend (ws: obj) (msg: string) : unit = jsNative

[<Fable.Core.Emit("$0.close(1000)")>]
let private wsClose (ws: obj) : unit = jsNative

let mutable private wsConn : obj option = None

[<Fable.Core.Emit("($0.target.files&&$0.target.files.length>0)?$0.target.files[0]:null")>]
let private changeEventFile (e: Browser.Types.Event) : obj = jsNative

[<Fable.Core.Emit("$0?$0.name:''")>]
let private fileName (f: obj) : string = jsNative

[<Fable.Core.Emit("$0?$0.size:0")>]
let private fileSize (f: obj) : int = jsNative

[<Fable.Core.Emit("$0?($0.type||'application/octet-stream'):''")>]
let private fileType (f: obj) : string = jsNative

[<Fable.Core.Emit("(function(url,f){var fd=new FormData();fd.append('file',f);return fetch(url,{method:'POST',body:fd}).then(r=>r.text().then(b=>({status:r.status,body:b})));})($0,$1)")>]
let private fetchUpload (url: string) (f: obj) : Fable.Core.JS.Promise<{| status: int; body: string |}> = jsNative

[<Fable.Core.Emit("new EventSource($0)")>]
let private newEventSource (url: string) : obj = jsNative

[<Fable.Core.Emit("(function(es,onMsg,onDone){var done=false;es.onmessage=function(e){onMsg(e.data);};es.onerror=function(){if(!done){done=true;es.close();onDone();}};})($0,$1,$2)")>]
let private attachSseHandlers (es: obj) (onMsg: string -> unit) (onDone: unit -> unit) : unit = jsNative

[<Fable.Core.Emit("$0.close()")>]
let private sseClose (es: obj) : unit = jsNative

let mutable private sseConn : obj option = None

let update msg model =
    match msg with
    | UrlChanged hash ->
        { model with Page = parsePage hash; Hash = hash; SidebarOpen = false }, Cmd.none
    | ElmishCounterMsg sub ->
        let m, cmd = Pages.CounterElmish.update sub model.ElmishCounter
        { model with ElmishCounter = m }, Cmd.map ElmishCounterMsg cmd
    | BackendFetch(slug, url) ->
        let cmd =
            Cmd.OfPromise.either
                fetchGet url
                (fun r -> BackendReceive(slug, r.status, r.body))
                (fun ex -> BackendError(slug, ex.Message))
        { model with BackendResults = model.BackendResults |> Map.add slug Fetching }, cmd
    | BackendReceive(slug, status, body) ->
        { model with BackendResults = model.BackendResults |> Map.add slug (Done(status, body)) }, Cmd.none
    | BackendError(slug, msg) ->
        { model with BackendResults = model.BackendResults |> Map.add slug (Failed msg) }, Cmd.none
    | ToggleTheme ->
        let newTheme = if model.Theme = Dark then Light else Dark
        document.documentElement.className <- if newTheme = Light then "theme-light" else ""
        { model with Theme = newTheme }, Cmd.none
    | ToggleSidebar ->
        { model with SidebarOpen = not model.SidebarOpen }, Cmd.none
    | CloseSidebar ->
        { model with SidebarOpen = false }, Cmd.none
    | CrudFormStr(field, value) ->
        let f = model.CrudForm
        let form =
            match field with
            | "get-id"        -> { f with GetId = value }
            | "create-title"  -> { f with CreateTitle = value }
            | "update-id"     -> { f with UpdateId = value }
            | "update-title"  -> { f with UpdateTitle = value }
            | "delete-id"     -> { f with DeleteId = value }
            | _               -> f
        { model with CrudForm = form }, Cmd.none
    | CrudFormBool(field, value) ->
        let f = model.CrudForm
        let form =
            match field with
            | "create-completed" -> { f with CreateCompleted = value }
            | "update-completed" -> { f with UpdateCompleted = value }
            | _                  -> f
        { model with CrudForm = form }, Cmd.none
    | CrudSend(key, method, url, body) ->
        let cmd =
            Cmd.OfPromise.either
                (fun () -> fetchJson url method body)
                ()
                (fun r -> BackendReceive(key, r.status, r.body))
                (fun ex -> BackendError(key, ex.Message))
        { model with BackendResults = model.BackendResults |> Map.add key Fetching }, cmd
    | PagFormStr(field, value) ->
        let p = model.PagForm
        let form =
            match field with
            | "filter"    -> { p with Filter = value; Page = 1 }
            | "sort-by"   -> { p with SortBy = value; Page = 1 }
            | "sort-dir"  -> { p with SortDir = value; Page = 1 }
            | "page-size" -> { p with PageSize = value; Page = 1 }
            | _           -> p
        { model with PagForm = form }, Cmd.none
    | PagFetch page ->
        let p   = { model.PagForm with Page = page }
        let url = $"/api/items/paged?page={page}&pageSize={p.PageSize}&filter={encodeQuery p.Filter}&sortBy={p.SortBy}&sortDir={p.SortDir}"
        let cmd =
            Cmd.OfPromise.either
                fetchGet url
                (fun r -> BackendReceive("pag-result", r.status, r.body))
                (fun ex -> BackendError("pag-result", ex.Message))
        { model with PagForm = p; BackendResults = model.BackendResults |> Map.add "pag-result" Fetching }, cmd
    | AuthFormStr(field, value) ->
        let f    = model.AuthForm
        let form =
            match field with
            | "username" -> { f with Username = value }
            | "password" -> { f with Password = value }
            | _          -> f
        { model with AuthForm = form }, Cmd.none
    | AuthLogin ->
        let f   = model.AuthForm
        let cmd =
            Cmd.OfPromise.either
                (fun () -> fetchJson "/api/demo/auth/login" "POST" (loginJson f.Username f.Password))
                ()
                (fun r -> AuthReceive("auth-login", r.status, r.body))
                (fun ex -> BackendError("auth-login", ex.Message))
        { model with BackendResults = model.BackendResults |> Map.add "auth-login" Fetching }, cmd
    | AuthReceive(key, status, body) ->
        let m1 = { model with BackendResults = model.BackendResults |> Map.add key (Done(status, body)) }
        if key = "auth-login" && status = 200 then
            { m1 with AuthForm = { m1.AuthForm with Token = extractToken body } }, Cmd.none
        else m1, Cmd.none
    | AuthCallMe ->
        let token = model.AuthForm.Token
        let cmd =
            Cmd.OfPromise.either
                (fun () -> fetchBearer "/api/demo/auth/me" token)
                ()
                (fun r -> BackendReceive("auth-me", r.status, r.body))
                (fun ex -> BackendError("auth-me", ex.Message))
        { model with BackendResults = model.BackendResults |> Map.add "auth-me" Fetching }, cmd
    | RateLimitFire ->
        let cmd =
            Cmd.OfPromise.either fetchGet "/api/demo/ratelimit/ping"
                (fun r -> RateLimitReceive(r.status, r.body))
                (fun ex -> RateLimitReceive(0, ex.Message))
        model, cmd
    | RateLimitHammer ->
        let cmd =
            List.init 10 (fun _ ->
                Cmd.OfPromise.either fetchGet "/api/demo/ratelimit/ping"
                    (fun r -> RateLimitReceive(r.status, r.body))
                    (fun ex -> RateLimitReceive(0, ex.Message)))
            |> Cmd.batch
        model, cmd
    | RateLimitReceive(status, body) ->
        let log = (status, body) :: model.RateLimitLog |> List.truncate 20
        { model with RateLimitLog = log }, Cmd.none
    | RateLimitClear ->
        { model with RateLimitLog = [] }, Cmd.none
    | JobStart ->
        let cmd =
            Cmd.OfPromise.either
                (fun () -> fetchJson "/api/demo/jobs" "POST" "")
                ()
                (fun r -> JobStarted(r.status, r.body))
                (fun ex -> BackendError("job-start", ex.Message))
        { model with BackendResults = model.BackendResults |> Map.add "job-start" Fetching }, cmd
    | JobStarted(status, body) ->
        let m1 = { model with BackendResults = model.BackendResults |> Map.add "job-start" (Done(status, body)) }
        if status = 202 then
            let id = extractJobId body
            let m2 = { m1 with JobLog = (id, body) :: m1.JobLog |> List.truncate 8 }
            m2, Cmd.ofMsg (JobPoll id)
        else m1, Cmd.none
    | JobPoll id ->
        let cmd =
            Cmd.OfPromise.either
                (fun () -> fetchGetDelayed $"/api/demo/jobs/{id}" 1000)
                ()
                (fun r -> JobPolled(id, r.status, r.body))
                (fun ex -> BackendError("job-poll", ex.Message))
        { model with JobPolling = true }, cmd
    | JobPolled(id, _, body) ->
        let updatedLog =
            model.JobLog |> List.map (fun (jid, jbody) -> if jid = id then (id, body) else (jid, jbody))
        let m1 = { model with JobLog = updatedLog; JobPolling = false }
        if body.Contains("\"queued\"") || body.Contains("\"running\"") then
            m1, Cmd.ofMsg (JobPoll id)
        else m1, Cmd.none
    | WsConnect ->
        let proto = if window.location.protocol = "https:" then "wss:" else "ws:"
        let url   = $"{proto}//{window.location.host}/api/demo/ws/echo"
        let ws    = newWs url
        wsConn <- Some ws
        let log = ("info", $"Connecting to {url}…") :: model.WsLog |> List.truncate 50
        let cmd : Cmd<Msg> = [ fun dispatch ->
            attachWsHandlers ws
                (fun ()   -> dispatch WsOpened)
                (fun data -> dispatch (WsReceive data))
                (fun ()   -> dispatch WsClosed)
                (fun ()   -> dispatch (WsNetError "Connection error")) ]
        { model with WsStatus = WsConnecting; WsLog = log }, cmd
    | WsDisconnect ->
        wsConn |> Option.iter wsClose
        wsConn <- None
        let log = ("info", "Disconnected by client") :: model.WsLog |> List.truncate 50
        { model with WsStatus = WsGone; WsLog = log }, Cmd.none
    | WsOpened ->
        let log = ("info", "Connected ✓") :: model.WsLog |> List.truncate 50
        { model with WsStatus = WsOpen; WsLog = log }, Cmd.none
    | WsClosed ->
        wsConn <- None
        let log = ("info", "Connection closed by server") :: model.WsLog |> List.truncate 50
        { model with WsStatus = WsGone; WsLog = log }, Cmd.none
    | WsSend ->
        let msg = model.WsInput.Trim()
        if msg <> "" then
            wsConn |> Option.iter (fun ws -> wsSend ws msg)
            let log = ("sent", msg) :: model.WsLog |> List.truncate 50
            { model with WsLog = log; WsInput = "" }, Cmd.none
        else model, Cmd.none
    | WsReceive data ->
        let log = ("recv", data) :: model.WsLog |> List.truncate 50
        { model with WsLog = log }, Cmd.none
    | WsNetError err ->
        wsConn <- None
        let log = ("error", err) :: model.WsLog |> List.truncate 50
        { model with WsStatus = WsGone; WsLog = log }, Cmd.none
    | WsInputStr v ->
        { model with WsInput = v }, Cmd.none
    | WsLogClear ->
        { model with WsLog = [] }, Cmd.none
    | UploadFileSelected f ->
        { model with UploadFile = Some f }, Cmd.none
    | UploadSend ->
        match model.UploadFile with
        | None -> model, Cmd.none
        | Some f ->
            let cmd =
                Cmd.OfPromise.either
                    (fun () -> fetchUpload "/api/demo/upload" f)
                    ()
                    (fun r -> BackendReceive("file-upload", r.status, r.body))
                    (fun ex -> BackendError("file-upload", ex.Message))
            { model with BackendResults = model.BackendResults |> Map.add "file-upload" Fetching }, cmd
    | UploadClear ->
        { model with UploadFile = None
                     BackendResults = model.BackendResults |> Map.remove "file-upload" }, Cmd.none
    | SseConnect ->
        let es  = newEventSource "/api/demo/sse/events"
        sseConn <- Some es
        let cmd : Cmd<Msg> = [ fun dispatch ->
            attachSseHandlers es
                (fun data -> dispatch (SseReceive data))
                (fun ()   -> dispatch SseDone) ]
        { model with SseActive = true; SseLog = [] }, cmd
    | SseDisconnect ->
        sseConn |> Option.iter sseClose
        sseConn <- None
        { model with SseActive = false }, Cmd.none
    | SseReceive data ->
        { model with SseLog = data :: model.SseLog |> List.truncate 50 }, Cmd.none
    | SseDone ->
        sseConn <- None
        { model with SseActive = false }, Cmd.none
    | SseLogClear ->
        { model with SseLog = [] }, Cmd.none

// Elmish 4 subscription: hashchange listener.
let hashSub (_model: Model) : Sub<Msg> =
    [ ["hash"], fun dispatch ->
        window.addEventListener("hashchange", fun _ ->
            dispatch (UrlChanged window.location.hash))
        { new System.IDisposable with member _.Dispose() = () } ]

// ── Helpers ───────────────────────────────────────────────────────────────────

/// Render a <fui-*> Web Component inside the React tree.
/// attrs: list of (attribute-name, value) pairs.
let wc (tag: string) (attrs: (string * string) list) (children: ReactElement list) : ReactElement =
    domEl tag (attrs |> List.map (fun (k, v) -> HTMLAttr.Custom(k, box v))) children

// ── Sidebar ───────────────────────────────────────────────────────────────────

let sidebarLink (label: string) (href: string) (currentHash: string) =
    let isActive =
        href = currentHash ||
        (href = "#/" && (currentHash = "" || currentHash = "#"))
    a [
        ClassName (if isActive then "sidebar-link active" else "sidebar-link")
        Href href
    ] [ str label ]

let sidebar (model: Model) (dispatch: Msg -> unit) =
    aside [ ClassName (if model.SidebarOpen then "sidebar sidebar--open" else "sidebar") ] [
        div [ ClassName "sidebar-logo" ] [
            img [ Src "/favicon.png"; Alt "logo" ]
            span [] [
                str "Fable"
                span [ ClassName "logo-accent" ] [ str "UI" ]
            ]
        ]
        nav [ ClassName "sidebar-nav" ] [
            div [ ClassName "sidebar-group" ] [
                sidebarLink "Home" "#/" model.Hash
                sidebarLink "Getting Started" "#/getting-started" model.Hash
            ]
            div [ ClassName "sidebar-group" ] [
                a [ ClassName "sidebar-group-label"; Href "#/category/inputs" ] [ str "Inputs & Forms" ]
                sidebarLink "Button"   "#/component/inputs/button"   model.Hash
                sidebarLink "Input"   "#/component/inputs/input"    model.Hash
                sidebarLink "Textarea" "#/component/inputs/textarea" model.Hash
                sidebarLink "Select"    "#/component/inputs/select"   model.Hash
                sidebarLink "Checkbox"    "#/component/inputs/checkbox"    model.Hash
                sidebarLink "Radio"      "#/component/inputs/radio"       model.Hash
                sidebarLink "RadioGroup" "#/component/inputs/radio-group" model.Hash
                sidebarLink "Toggle"    "#/component/inputs/toggle"      model.Hash
                sidebarLink "Slider"      "#/component/inputs/slider"       model.Hash
                sidebarLink "DatePicker"   "#/component/inputs/date-picker"   model.Hash
                sidebarLink "ColorPicker" "#/component/inputs/color-picker" model.Hash
                sidebarLink "FileUpload"  "#/component/inputs/file-upload"  model.Hash
                sidebarLink "TimePicker" "#/component/inputs/time-picker" model.Hash
                sidebarLink "Combobox"   "#/component/inputs/combobox"    model.Hash
                sidebarLink "FormField"  "#/component/inputs/form-field"  model.Hash
                sidebarLink "Form"       "#/component/inputs/form"        model.Hash
            ]
            div [ ClassName "sidebar-group" ] [
                a [ ClassName "sidebar-group-label"; Href "#/category/layout" ] [ str "Layout" ]
                sidebarLink "Divider"      "#/component/layout/divider"      model.Hash
                sidebarLink "Stack"        "#/component/layout/stack"        model.Hash
                sidebarLink "ScrollArea"   "#/component/layout/scroll-area"  model.Hash
                sidebarLink "Container"    "#/component/layout/container"    model.Hash
                sidebarLink "Grid"         "#/component/layout/grid"         model.Hash
                sidebarLink "Spacer"       "#/component/layout/spacer"       model.Hash
                sidebarLink "AspectRatio"    "#/component/layout/aspect-ratio"    model.Hash
                sidebarLink "ResizablePanel" "#/component/layout/resizable-panel" model.Hash
            ]
            div [ ClassName "sidebar-group" ] [
                a [ ClassName "sidebar-group-label"; Href "#/category/overlay" ] [ str "Overlay" ]
                sidebarLink "Modal"         "#/component/overlay/modal"          model.Hash
                sidebarLink "Drawer"        "#/component/overlay/drawer"         model.Hash
                sidebarLink "Tooltip"       "#/component/overlay/tooltip"        model.Hash
                sidebarLink "Popover"       "#/component/overlay/popover"        model.Hash
                sidebarLink "ConfirmDialog" "#/component/overlay/confirm-dialog" model.Hash
            ]
            div [ ClassName "sidebar-group" ] [
                a [ ClassName "sidebar-group-label"; Href "#/category/feedback" ] [ str "Feedback" ]
                sidebarLink "Badge"      "#/component/feedback/badge"       model.Hash
                sidebarLink "Spinner"    "#/component/feedback/spinner"     model.Hash
                sidebarLink "Progress"   "#/component/feedback/progress"    model.Hash
                sidebarLink "Alert"      "#/component/feedback/alert"       model.Hash
                sidebarLink "Skeleton"   "#/component/feedback/skeleton"    model.Hash
                sidebarLink "Toast"      "#/component/feedback/toast"       model.Hash
                sidebarLink "EmptyState"    "#/component/feedback/empty-state"    model.Hash
                sidebarLink "ErrorBoundary" "#/component/feedback/error-boundary" model.Hash
            ]
            div [ ClassName "sidebar-group" ] [
                a [ ClassName "sidebar-group-label"; Href "#/category/data-display" ] [ str "Data Display" ]
                sidebarLink "Card"        "#/component/data-display/card"         model.Hash
                sidebarLink "Stat"        "#/component/data-display/stat"         model.Hash
                sidebarLink "Avatar"      "#/component/data-display/avatar"       model.Hash
                sidebarLink "AvatarGroup" "#/component/data-display/avatar-group" model.Hash
                sidebarLink "Tag"         "#/component/data-display/tag"          model.Hash
                sidebarLink "CodeBlock"   "#/component/data-display/code-block"   model.Hash
                sidebarLink "Callout"     "#/component/data-display/callout"      model.Hash
                sidebarLink "List"        "#/component/data-display/list"         model.Hash
                sidebarLink "Timeline"    "#/component/data-display/timeline"     model.Hash
                sidebarLink "Accordion"   "#/component/data-display/accordion"    model.Hash
                sidebarLink "Carousel"    "#/component/data-display/carousel"     model.Hash
                sidebarLink "Table"       "#/component/data-display/table"        model.Hash
            ]
            div [ ClassName "sidebar-group" ] [
                a [ ClassName "sidebar-group-label"; Href "#/category/navigation" ] [ str "Navigation" ]
                sidebarLink "Tabs"        "#/component/navigation/tabs"        model.Hash
                sidebarLink "Breadcrumb"  "#/component/navigation/breadcrumb"  model.Hash
                sidebarLink "Pagination"  "#/component/navigation/pagination"  model.Hash
                sidebarLink "Stepper"     "#/component/navigation/stepper"     model.Hash
                sidebarLink "Menu"           "#/component/navigation/menu"           model.Hash
                sidebarLink "Link"           "#/component/navigation/link"           model.Hash
                sidebarLink "ContextMenu"    "#/component/navigation/context-menu"   model.Hash
                sidebarLink "CommandPalette" "#/component/navigation/command-palette" model.Hash
                sidebarLink "SideNav"        "#/component/navigation/sidenav"        model.Hash
                sidebarLink "TopNav"         "#/component/navigation/topnav"         model.Hash
            ]
            div [ ClassName "sidebar-group" ] [
                a [ ClassName "sidebar-group-label"; Href "#/category/typography" ] [ str "Typography" ]
                sidebarLink "Heading"    "#/component/typography/heading"    model.Hash
                sidebarLink "Text"       "#/component/typography/text"       model.Hash
                sidebarLink "Label"      "#/component/typography/label"      model.Hash
                sidebarLink "Code"       "#/component/typography/code"       model.Hash
                sidebarLink "Kbd"        "#/component/typography/kbd"        model.Hash
                sidebarLink "Blockquote" "#/component/typography/blockquote" model.Hash
                sidebarLink "Prose"      "#/component/typography/prose"      model.Hash
            ]
            div [ ClassName "sidebar-group" ] [
                a [ ClassName "sidebar-group-label"; Href "#/category/charts" ] [ str "Charts" ]
                sidebarLink "BarChart"   "#/component/charts/bar-chart"   model.Hash
                sidebarLink "LineChart"  "#/component/charts/line-chart"  model.Hash
                sidebarLink "PieChart"   "#/component/charts/pie-chart"   model.Hash
                sidebarLink "Sparkline"  "#/component/charts/sparkline"   model.Hash
            ]
            div [ ClassName "sidebar-group" ] [
                div [ ClassName "sidebar-group-label" ] [ str "Backend Patterns" ]
                sidebarLink "Health Check"         "#/backend/health-check"    model.Hash
                sidebarLink "CRUD API"             "#/backend/crud-api"        model.Hash
                sidebarLink "Error Handling"       "#/backend/error-handling"  model.Hash
                sidebarLink "Pagination & Filters" "#/backend/pagination-api"  model.Hash
                sidebarLink "JWT Auth"             "#/backend/jwt-auth"        model.Hash
                sidebarLink "Rate Limiting"        "#/backend/rate-limiting"   model.Hash
                sidebarLink "Background Jobs"      "#/backend/background-jobs" model.Hash
                sidebarLink "WebSocket"            "#/backend/websocket"       model.Hash
                sidebarLink "File Upload"          "#/backend/file-upload"     model.Hash
                sidebarLink "Server-Sent Events"  "#/backend/sse"             model.Hash
            ]
            div [ ClassName "sidebar-group" ] [
                div [ ClassName "sidebar-group-label" ] [ str "Examples" ]
                sidebarLink "Counter — Elmish" "#/counter-elmish" model.Hash
                sidebarLink "Counter — DOM"    "#/counter-dom"    model.Hash
            ]
        ]
    ]

// ── Topbar ────────────────────────────────────────────────────────────────────

let breadcrumbItems (page: Page) : (string * bool) list =
    match page with
    | Home             -> [ "Home", true ]
    | GettingStarted    -> [ "Home", false; "Getting Started", true ]
    | CategoryIndex cat -> [ "Home", false; "Components", false; urlCatLabel cat, true ]
    | BackendDemo slug  ->
        let name = slug |> fun s -> System.Char.ToUpper(s.[0]).ToString() + s.[1..].Replace("-", " ")
        [ "Home", false; "Backend Patterns", false; name, true ]
    | CounterElmish    -> [ "Home", false; "Examples", false; "Counter — Elmish", true ]
    | CounterDom       -> [ "Home", false; "Examples", false; "Counter — DOM", true ]
    | Component(_, slug) ->
        let name = ComponentRegistry.bySlug slug |> Option.map (fun m -> m.Name) |> Option.defaultValue slug
        [ "Home", false; "Components", false; name, true ]

let topbar (model: Model) (dispatch: Msg -> unit) =
    let crumbs = breadcrumbItems model.Page
    header [ ClassName "topbar" ] [
        button [ ClassName "icon-btn hamburger"; OnClick (fun _ -> dispatch ToggleSidebar) ] [ str "☰" ]
        div [ ClassName "breadcrumb" ] [
            yield!
                crumbs
                |> List.mapi (fun i (label, isActive) ->
                    [
                        if i > 0 then
                            yield span [ ClassName "breadcrumb-sep"; Key $"sep-{i}" ] [ str "/" ]
                        yield span [ Key (string i); ClassName (if isActive then "breadcrumb-active" else "") ] [ str label ]
                    ])
                |> List.concat
        ]
        div [ ClassName "topbar-actions" ] [
            button [ ClassName "icon-btn"; OnClick (fun _ -> dispatch ToggleTheme); Title "Toggle theme" ] [ str (if model.Theme = Light then "☾" else "☀") ]
            a [ ClassName "icon-btn"; Href "https://github.com/safe-stack/SAFE-template"; Title "GitHub" ] [ str "GH" ]
        ]
    ]

// ── Pages ─────────────────────────────────────────────────────────────────────

let homePage =
    let catCards = [
        "inputs",       "Inputs & Forms",  16, "Buttons, inputs, selects, toggles, pickers…"
        "layout",       "Layout",           8,  "Grid, stack, container, scroll area…"
        "navigation",   "Navigation",       10, "Tabs, breadcrumb, pagination, menus…"
        "feedback",     "Feedback",         8,  "Badge, spinner, alert, toast, progress…"
        "overlay",      "Overlay",          5,  "Modal, drawer, tooltip, popover…"
        "data-display", "Data Display",     12, "Card, stat, avatar, table, timeline…"
        "typography",   "Typography",       7,  "Heading, text, label, code, prose…"
        "charts",       "Charts",           4,  "Bar, line, pie, sparkline…"
    ]
    div [] [
        // ── Hero ──────────────────────────────────────────────────────────────
        div [ ClassName "home-hero" ] [
            h1 [] [
                str "Fable"
                span [ ClassName "accent" ] [ str "UI" ]
                br []
                str "Component Library"
            ]
            p [] [ str "A comprehensive reference of web UI components built as standalone Web Components. Drop any fui-* element into any framework — or no framework at all." ]
            div [ ClassName "home-cta" ] [
                a [ ClassName "home-cta-btn--primary";   Href "#/category/inputs" ] [ str "Browse Components" ]
                a [ ClassName "home-cta-btn--secondary"; Href "#/getting-started" ] [ str "Getting Started" ]
            ]
            div [ ClassName "home-badges" ] [
                wc "fui-tag" [ "variant", "accent" ] [ str "Fable" ]
                wc "fui-tag" [] [ str "Saturn" ]
                wc "fui-tag" [] [ str "Elmish" ]
                wc "fui-tag" [] [ str "Vite" ]
                wc "fui-tag" [] [ str "Web Components" ]
            ]
        ]
        // ── Category grid ──────────────────────────────────────────────────────
        div [ ClassName "home-cats" ] [
            p [ ClassName "home-cats-title" ] [ str "Component Categories" ]
            div [ ClassName "home-cat-grid" ] [
                for (slug, name, count, desc) in catCards do
                    yield a [ ClassName "home-cat-card"; Href $"#/category/{slug}" ] [
                        span [ ClassName "cat-card-name"  ] [ str name ]
                        span [ ClassName "cat-card-count" ] [ str $"{count} components" ]
                        span [ ClassName "cat-card-desc"  ] [ str desc ]
                    ]
            ]
        ]
        // ── Sample previews ────────────────────────────────────────────────────
        div [] [
            p [ ClassName "home-samples-title" ] [ str "Component Samples" ]
            div [ ClassName "component-preview" ] [
                div [ ClassName "preview-header" ] [
                    span [ ClassName "preview-tag" ] [ str "fui-button" ]
                    span [ ClassName "preview-badge" ] [ str "Inputs & Forms" ]
                ]
                div [ ClassName "preview-body" ] [
                    div [ ClassName "preview-row" ] [
                        wc "fui-button" [ "variant", "primary"   ] [ str "Save changes" ]
                        wc "fui-button" [ "variant", "secondary" ] [ str "Cancel" ]
                        wc "fui-button" [ "variant", "ghost"     ] [ str "Menu" ]
                        wc "fui-button" [ "variant", "danger"    ] [ str "Delete" ]
                    ]
                    div [ ClassName "preview-row"; Style [ MarginTop "0.75rem" ] ] [
                        wc "fui-button" [ "variant", "primary"; "size", "sm" ] [ str "Small" ]
                        wc "fui-button" [ "variant", "primary"; "size", "md" ] [ str "Medium" ]
                        wc "fui-button" [ "variant", "primary"; "size", "lg" ] [ str "Large" ]
                    ]
                ]
            ]
            div [ ClassName "component-preview" ] [
                div [ ClassName "preview-header" ] [
                    span [ ClassName "preview-tag" ] [ str "fui-badge · fui-tag · fui-avatar" ]
                    span [ ClassName "preview-badge" ] [ str "Feedback / Data Display" ]
                ]
                div [ ClassName "preview-body" ] [
                    div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                        div [ ClassName "preview-row" ] [
                            wc "fui-badge" [] [ str "Neutral" ]
                            wc "fui-badge" [ "variant", "success" ] [ str "Success" ]
                            wc "fui-badge" [ "variant", "warning" ] [ str "Warning" ]
                            wc "fui-badge" [ "variant", "danger"  ] [ str "Danger" ]
                            wc "fui-badge" [ "variant", "accent"  ] [ str "Accent" ]
                            wc "fui-badge" [ "variant", "success"; "dot", "" ] [ str "Online" ]
                        ]
                        div [ ClassName "preview-row" ] [
                            wc "fui-tag" [] [ str "Default" ]
                            wc "fui-tag" [ "variant", "success" ] [ str "Active" ]
                            wc "fui-tag" [ "variant", "warning" ] [ str "Pending" ]
                            wc "fui-tag" [ "variant", "danger"; "removable", "" ] [ str "Remove me" ]
                            wc "fui-tag" [ "variant", "accent" ] [ str "Accent" ]
                        ]
                        div [ ClassName "preview-row" ] [
                            wc "fui-avatar" [ "initials", "JD" ] []
                            wc "fui-avatar" [ "initials", "AB"; "status", "online" ] []
                            wc "fui-avatar" [ "initials", "XY"; "size", "lg"; "status", "busy" ] []
                        ]
                    ]
                ]
            ]
            div [ ClassName "component-preview" ] [
                div [ ClassName "preview-header" ] [
                    span [ ClassName "preview-tag" ] [ str "fui-stat · fui-alert · fui-callout" ]
                    span [ ClassName "preview-badge" ] [ str "Data Display / Feedback" ]
                ]
                div [ ClassName "preview-body" ] [
                    div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
                        div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
                            wc "fui-stat" [ "label", "Revenue"; "value", "$48,200"; "change", "12.5%"; "trend", "up"; "description", "vs last month" ] []
                            wc "fui-stat" [ "label", "Users";   "value", "8,412";   "change", "3.1%";  "trend", "down" ] []
                            wc "fui-stat" [ "label", "Uptime";  "value", "99.9%" ] []
                        ]
                        wc "fui-alert" [ "variant", "info" ] [ str "Your session will expire in 10 minutes." ]
                        wc "fui-callout" [ "variant", "info"; "title", "Getting started" ] [ str "Drop any fui-* component into your HTML and it works out of the box." ]
                    ]
                ]
            ]
        ]
    ]

// ── Component detail page ─────────────────────────────────────────────────────

let componentLivePreview (slug: string) : ReactElement list =
    let fwOpts   = """[{"value":"react","label":"React"},{"value":"vue","label":"Vue"},{"value":"svelte","label":"Svelte"},{"value":"solid","label":"Solid"}]"""
    let sizeOpts = """[{"value":"sm","label":"Small"},{"value":"md","label":"Medium"},{"value":"lg","label":"Large"}]"""
    match slug with
    | "button" ->
        [
            div [ ClassName "preview-row" ] [
                wc "fui-button" [ "variant", "primary"   ] [ str "Save" ]
                wc "fui-button" [ "variant", "secondary" ] [ str "Cancel" ]
                wc "fui-button" [ "variant", "ghost"     ] [ str "Menu" ]
                wc "fui-button" [ "variant", "danger"    ] [ str "Delete" ]
                wc "fui-button" [ "variant", "primary"; "disabled", "" ] [ str "Disabled" ]
            ]
            div [ ClassName "preview-row"; Style [ MarginTop "0.75rem" ] ] [
                wc "fui-button" [ "variant", "primary"; "size", "sm" ] [ str "Small" ]
                wc "fui-button" [ "variant", "primary"; "size", "md" ] [ str "Medium" ]
                wc "fui-button" [ "variant", "primary"; "size", "lg" ] [ str "Large" ]
            ]
        ]
    | "input" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-input" [ "label", "Email address"; "type", "email"; "placeholder", "you@example.com" ] []
            wc "fui-input" [ "label", "Password";      "type", "password"; "placeholder", "••••••••"     ] []
            wc "fui-input" [ "label", "Disabled";      "disabled", ""; "value", "Cannot edit this"       ] []
            wc "fui-input" [ "label", "With error";    "error", "This field is required"                 ] []
        ] ]
    | "textarea" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-textarea" [ "label", "Message";  "placeholder", "Write something..."    ] []
            wc "fui-textarea" [ "label", "Notes";    "rows", "5"; "resize", "both"          ] []
            wc "fui-textarea" [ "label", "Disabled"; "disabled", ""; "value", "Cannot edit" ] []
            wc "fui-textarea" [ "label", "Error";    "error", "This field is required"      ] []
        ] ]
    | "select" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-select" [ "label", "Framework"; "placeholder", "Pick a framework..."; "options", fwOpts ] []
            wc "fui-select" [ "label", "Size (preselected)"; "value", "md"; "options", sizeOpts ] []
            wc "fui-select" [ "label", "Disabled"; "disabled", ""; "value", "react"; "options", fwOpts ] []
            wc "fui-select" [ "label", "With error"; "placeholder", "Required..."; "error", "Please select an option"; "options", fwOpts ] []
        ] ]
    | "checkbox" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-checkbox" [ "label", "Accept terms and conditions" ] []
            wc "fui-checkbox" [ "label", "Subscribe to newsletter"; "checked", "" ] []
            wc "fui-checkbox" [ "label", "Disabled option"; "disabled", "" ] []
            wc "fui-checkbox" [ "label", "With error"; "error", "This field is required" ] []
        ] ]
    | "radio" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-radio" [ "value", "a"; "label", "Option A" ] []
            wc "fui-radio" [ "value", "b"; "label", "Option B — checked"; "checked", "" ] []
            wc "fui-radio" [ "value", "c"; "label", "Option C — disabled"; "disabled", "" ] []
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem"); CSSProp.Custom("align-items", "center") ] ] [
                wc "fui-radio" [ "value", "s"; "label", "Small"; "size", "sm" ] []
                wc "fui-radio" [ "value", "m"; "label", "Medium" ] []
                wc "fui-radio" [ "value", "l"; "label", "Large"; "size", "lg" ] []
            ]
        ] ]
    | "radio-group" ->
        let rgOpts   = """[{"value":"react","label":"React"},{"value":"vue","label":"Vue"},{"value":"svelte","label":"Svelte"}]"""
        let sizeOpts = """[{"value":"sm","label":"Small"},{"value":"md","label":"Medium"},{"value":"lg","label":"Large"}]"""
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
            wc "fui-radio-group" [ "label", "Framework";   "name", "rg1"; "options", rgOpts ] []
            wc "fui-radio-group" [ "label", "Preselected"; "name", "rg2"; "value", "vue"; "options", rgOpts ] []
            wc "fui-radio-group" [ "label", "Disabled";    "name", "rg3"; "disabled", ""; "value", "react"; "options", rgOpts ] []
            wc "fui-radio-group" [ "label", "With error";  "name", "rg4"; "error", "Please select an option"; "options", sizeOpts ] []
        ] ]
    | "toggle" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-toggle" [ "label", "Enable notifications" ] []
            wc "fui-toggle" [ "label", "Dark mode"; "checked", "" ] []
            wc "fui-toggle" [ "label", "Disabled off"; "disabled", "" ] []
            wc "fui-toggle" [ "label", "Disabled on"; "checked", ""; "disabled", "" ] []
            wc "fui-toggle" [ "label", "With error"; "error", "This setting is required" ] []
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem"); CSSProp.Custom("align-items", "center") ] ] [
                wc "fui-toggle" [ "label", "Small"; "size", "sm"; "checked", "" ] []
                wc "fui-toggle" [ "label", "Medium"; "checked", "" ] []
                wc "fui-toggle" [ "label", "Large"; "size", "lg"; "checked", "" ] []
            ]
        ] ]
    | "slider" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
            wc "fui-slider" [ "label", "Volume";       "value", "40" ] []
            wc "fui-slider" [ "label", "Price range";  "min", "100"; "max", "1000"; "step", "50"; "value", "400" ] []
            wc "fui-slider" [ "label", "Opacity";      "min", "0"; "max", "1"; "step", "0.01"; "value", "0.75" ] []
            wc "fui-slider" [ "label", "Disabled";     "value", "60"; "disabled", "" ] []
            wc "fui-slider" [ "label", "With error";   "value", "0"; "error", "Value must be greater than 0" ] []
            div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                wc "fui-slider" [ "label", "Small";  "size", "sm"; "value", "30" ] []
                wc "fui-slider" [ "label", "Medium"; "value", "50" ] []
                wc "fui-slider" [ "label", "Large";  "size", "lg"; "value", "70" ] []
            ]
        ] ]
    | "date-picker" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-date-picker" [ "label", "Date of birth" ] []
            wc "fui-date-picker" [ "label", "Check-in"; "value", "2025-06-01"; "min", "2025-01-01"; "max", "2025-12-31" ] []
            wc "fui-date-picker" [ "label", "Disabled"; "value", "2025-03-15"; "disabled", "" ] []
            wc "fui-date-picker" [ "label", "With error"; "error", "Please select a valid date" ] []
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem") ] ] [
                wc "fui-date-picker" [ "label", "Small";  "size", "sm" ] []
                wc "fui-date-picker" [ "label", "Medium" ] []
                wc "fui-date-picker" [ "label", "Large";  "size", "lg" ] []
            ]
        ] ]
    | "color-picker" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-color-picker" [ "label", "Brand colour"; "value", "#7C3AED" ] []
            wc "fui-color-picker" [ "label", "Accent";       "value", "#3B82F6" ] []
            wc "fui-color-picker" [ "label", "Success";      "value", "#22C55E" ] []
            wc "fui-color-picker" [ "label", "Danger";       "value", "#EF4444" ] []
            wc "fui-color-picker" [ "label", "Disabled";     "value", "#F59E0B"; "disabled", "" ] []
            wc "fui-color-picker" [ "label", "With error";   "error", "A colour is required" ] []
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem"); CSSProp.Custom("align-items", "flex-start") ] ] [
                wc "fui-color-picker" [ "label", "Small";  "size", "sm"; "value", "#7C3AED" ] []
                wc "fui-color-picker" [ "label", "Medium"; "value", "#7C3AED" ] []
                wc "fui-color-picker" [ "label", "Large";  "size", "lg"; "value", "#7C3AED" ] []
            ]
        ] ]
    | "file-upload" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
            wc "fui-file-upload" [ "label", "Attachment" ] []
            wc "fui-file-upload" [ "label", "Images only"; "accept", "image/*"; "multiple", "" ] []
            wc "fui-file-upload" [ "label", "Documents";   "accept", ".pdf,.docx,.xlsx" ] []
            wc "fui-file-upload" [ "label", "Disabled";    "disabled", "" ] []
            wc "fui-file-upload" [ "label", "With error";  "error", "Please upload a file" ] []
            div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                wc "fui-file-upload" [ "label", "Small";  "size", "sm" ] []
                wc "fui-file-upload" [ "label", "Medium" ] []
                wc "fui-file-upload" [ "label", "Large";  "size", "lg" ] []
            ]
        ] ]
    | "divider" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            // Plain horizontal rule
            wc "fui-divider" [] []
            // With label
            wc "fui-divider" [ "label", "or" ] []
            wc "fui-divider" [ "label", "Section title" ] []
            // Vertical inside a flex row
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("height", "48px"); CSSProp.Custom("align-items", "center"); CSSProp.Custom("gap", "1rem") ] ] [
                span [] [ str "Left content" ]
                wc "fui-divider" [ "orientation", "vertical" ] []
                span [] [ str "Right content" ]
                wc "fui-divider" [ "orientation", "vertical" ] []
                span [] [ str "More content" ]
            ]
        ] ]
    | "stack" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            // Row
            wc "fui-stack" [ "direction", "row"; "gap", "0.75rem" ] [
                wc "fui-button" [ "variant", "primary"   ] [ str "Save" ]
                wc "fui-button" [ "variant", "secondary" ] [ str "Cancel" ]
                wc "fui-button" [ "variant", "ghost"     ] [ str "Preview" ]
            ]
            // Column
            wc "fui-stack" [ "direction", "column"; "gap", "0.75rem" ] [
                wc "fui-input"  [ "label", "Email";    "type", "email";    "placeholder", "you@example.com" ] []
                wc "fui-input"  [ "label", "Password"; "type", "password"; "placeholder", "••••••••"        ] []
                wc "fui-button" [ "variant", "primary" ] [ str "Sign in" ]
            ]
            // Row wrapping
            wc "fui-stack" [ "direction", "row"; "gap", "0.5rem"; "wrap", "" ] [
                wc "fui-button" [ "variant", "secondary"; "size", "sm" ] [ str "Draft" ]
                wc "fui-button" [ "variant", "secondary"; "size", "sm" ] [ str "Review" ]
                wc "fui-button" [ "variant", "secondary"; "size", "sm" ] [ str "Approved" ]
                wc "fui-button" [ "variant", "secondary"; "size", "sm" ] [ str "Published" ]
                wc "fui-button" [ "variant", "secondary"; "size", "sm" ] [ str "Archived" ]
            ]
            // Row centred
            wc "fui-stack" [ "direction", "row"; "gap", "1rem"; "justify", "center"; "align", "center" ] [
                wc "fui-toggle"   [ "label", "Notifications" ] []
                wc "fui-divider"  [ "orientation", "vertical" ] []
                wc "fui-toggle"   [ "label", "Dark mode"; "checked", "" ] []
            ]
        ] ]
    | "scroll-area" ->
        let items = [ 1..25 ] |> List.map (fun i -> li [ Style [ Padding "0.35rem 0"; CSSProp.Custom("border-bottom", "1px solid #2A2A2E") ] ] [ str $"List item {i}" ])
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Vertical — 200px" ]
                wc "fui-scroll-area" [ "height", "200px"; "direction", "vertical" ] [
                    ul [ Style [ Margin "0"; Padding "0 0 0 0"; ListStyleType "none" ] ] items
                ]
            ]
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "max-height — 150px" ]
                wc "fui-scroll-area" [ "max-height", "150px" ] [
                    div [ Style [ Padding "0.5rem" ] ] [
                        yield! [ 1..10 ] |> List.map (fun i -> p [ Style [ Margin "0.25rem 0" ] ] [ str $"Paragraph {i} — some content that fills the scroll viewport." ])
                    ]
                ]
            ]
        ] ]
    | "container" ->
        let label txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        let frame children = div [ Style [ CSSProp.Custom("background", "#0D0D0F"); CSSProp.Custom("border", "1px dashed #2A2A2E"); CSSProp.Custom("border-radius", "8px"); CSSProp.Custom("overflow", "hidden") ] ] children
        let inner txt = div [ Style [ CSSProp.Custom("background", "#1E1E21"); Padding "0.75rem"; CSSProp.Custom("border-radius", "4px") ] ] [ span [ Style [ FontSize "0.8rem"; CSSProp.Custom("color", "#6E6E76") ] ] [ str txt ] ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1rem") ] ] [
            div [] [
                label "Default (max-width: 1100px)"
                frame [ wc "fui-container" [] [ inner "Centred content — constrained to 1100px with 2.5rem side padding." ] ]
            ]
            div [] [
                label "max-width: 480px, padding: 1rem"
                frame [ wc "fui-container" [ "max-width", "480px"; "padding", "1rem" ] [ inner "Narrower container — comfortable for articles and forms." ] ]
            ]
            div [] [
                label "max-width: 320px, padding: 0.75rem"
                frame [ wc "fui-container" [ "max-width", "320px"; "padding", "0.75rem" ] [ inner "Mobile-width column." ] ]
            ]
        ] ]
    | "grid" ->
        let cell lbl = div [ Style [ CSSProp.Custom("background", "#1E1E21"); CSSProp.Custom("border", "1px solid #2A2A2E"); CSSProp.Custom("border-radius", "6px"); Padding "0.625rem 0.875rem"; FontSize "0.8rem"; CSSProp.Custom("color", "#E8E8ED") ] ] [ str lbl ]
        let sectionLabel txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [
                sectionLabel "3 equal columns"
                wc "fui-grid" [ "cols", "3"; "gap", "0.75rem" ] [
                    cell "Column 1"; cell "Column 2"; cell "Column 3"
                ]
            ]
            div [] [
                sectionLabel "2fr + 1fr (main + sidebar)"
                wc "fui-grid" [ "cols", "2fr 1fr"; "gap", "0.75rem" ] [
                    cell "Main content area"
                    cell "Sidebar"
                ]
            ]
            div [] [
                sectionLabel "4 equal columns"
                wc "fui-grid" [ "cols", "4"; "gap", "0.75rem" ] [
                    cell "A"; cell "B"; cell "C"; cell "D"
                ]
            ]
            div [] [
                sectionLabel "Auto-fill (min 180px)"
                wc "fui-grid" [ "cols", "repeat(auto-fill, minmax(180px, 1fr))"; "gap", "0.75rem" ] [
                    cell "Card 1"; cell "Card 2"; cell "Card 3"; cell "Card 4"
                ]
            ]
        ] ]
    | "spacer" ->
        let sectionLabel txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        let row children = div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("background", "#1E1E21"); CSSProp.Custom("border-radius", "8px"); Padding "0.75rem"; CSSProp.Custom("align-items", "center") ] ] children
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
            div [] [
                sectionLabel "Flexible spacer — pushes buttons to opposite ends"
                row [
                    wc "fui-button" [ "variant", "ghost";   "size", "sm" ] [ str "← Back" ]
                    wc "fui-spacer" [] []
                    wc "fui-button" [ "variant", "primary"; "size", "sm" ] [ str "Next →" ]
                ]
            ]
            div [] [
                sectionLabel "Flexible spacer — toolbar with mixed items"
                row [
                    wc "fui-button" [ "variant", "secondary"; "size", "sm" ] [ str "File" ]
                    wc "fui-button" [ "variant", "secondary"; "size", "sm" ] [ str "Edit" ]
                    wc "fui-spacer" [] []
                    wc "fui-button" [ "variant", "ghost"; "size", "sm" ] [ str "Help" ]
                    wc "fui-button" [ "variant", "ghost"; "size", "sm" ] [ str "Settings" ]
                ]
            ]
            div [] [
                sectionLabel "Fixed spacer — size: 2rem"
                row [
                    span [ Style [ FontSize "0.8rem"; CSSProp.Custom("color", "#E8E8ED") ] ] [ str "Label" ]
                    wc "fui-spacer" [ "size", "2rem" ] []
                    span [ Style [ FontSize "0.8rem"; CSSProp.Custom("color", "#6E6E76") ] ] [ str "Value" ]
                ]
            ]
        ] ]
    | "aspect-ratio" ->
        let sectionLabel txt = p [ Style [ Margin "0 0 0.375rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76") ] ] [ str txt ]
        let placeholder lbl = div [ Style [ CSSProp.Custom("background", "#1E1E21"); CSSProp.Custom("width", "100%"); CSSProp.Custom("height", "100%"); Display DisplayOptions.Flex; CSSProp.Custom("align-items", "center"); CSSProp.Custom("justify-content", "center"); FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("font-family", "JetBrains Mono, monospace") ] ] [ str lbl ]
        [ div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1.25rem"); CSSProp.Custom("flex-wrap", "wrap"); CSSProp.Custom("align-items", "flex-start") ] ] [
            div [ Style [ CSSProp.Custom("width", "220px"); CSSProp.Custom("flex-shrink", "0") ] ] [
                sectionLabel "16 / 9 — video"
                wc "fui-aspect-ratio" [ "ratio", "16/9" ] [ placeholder "16 / 9" ]
            ]
            div [ Style [ CSSProp.Custom("width", "140px"); CSSProp.Custom("flex-shrink", "0") ] ] [
                sectionLabel "4 / 3 — traditional"
                wc "fui-aspect-ratio" [ "ratio", "4/3" ] [ placeholder "4 / 3" ]
            ]
            div [ Style [ CSSProp.Custom("width", "100px"); CSSProp.Custom("flex-shrink", "0") ] ] [
                sectionLabel "1 / 1 — square"
                wc "fui-aspect-ratio" [ "ratio", "1/1" ] [ placeholder "1 / 1" ]
            ]
            div [ Style [ CSSProp.Custom("width", "220px"); CSSProp.Custom("flex-shrink", "0") ] ] [
                sectionLabel "2 / 1 — wide banner"
                wc "fui-aspect-ratio" [ "ratio", "2/1" ] [ placeholder "2 / 1" ]
            ]
        ] ]
    | "tabs" ->
        let tabsJson = """[{"value":"overview","label":"Overview"},{"value":"settings","label":"Settings"},{"value":"logs","label":"Logs"}]"""
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "2rem") ] ] [
            div [] [
                p [ Style [ Margin "0 0 0.75rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Default — first tab active" ]
                wc "fui-tabs" [ "tabs", tabsJson ] [
                    div [ HTMLAttr.Custom("slot", "tab-overview") ] [ p [ Style [ Margin "0" ] ] [ str "Overview content — displays first by default." ] ]
                    div [ HTMLAttr.Custom("slot", "tab-settings") ] [ p [ Style [ Margin "0" ] ] [ str "Settings panel content." ] ]
                    div [ HTMLAttr.Custom("slot", "tab-logs")     ] [ p [ Style [ Margin "0" ] ] [ str "Log output panel." ] ]
                ]
            ]
            div [] [
                p [ Style [ Margin "0 0 0.75rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "active=\"settings\"" ]
                wc "fui-tabs" [ "tabs", tabsJson; "active", "settings" ] [
                    div [ HTMLAttr.Custom("slot", "tab-overview") ] [ p [ Style [ Margin "0" ] ] [ str "Overview content." ] ]
                    div [ HTMLAttr.Custom("slot", "tab-settings") ] [ p [ Style [ Margin "0" ] ] [ str "Settings opened by default via the active attribute." ] ]
                    div [ HTMLAttr.Custom("slot", "tab-logs")     ] [ p [ Style [ Margin "0" ] ] [ str "Log output panel." ] ]
                ]
            ]
        ] ]
    | "breadcrumb" ->
        let items1 = """[{"label":"Home","href":"/"},{"label":"Components","href":"/components"},{"label":"Breadcrumb","href":""}]"""
        let items2 = """[{"label":"Docs","href":"/docs"},{"label":"API Reference","href":"/docs/api"},{"label":"Endpoints","href":""}]"""
        let items3 = """[{"label":"Home","href":"/"},{"label":"Settings","href":""}]"""
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
            wc "fui-breadcrumb" [ "items", items1 ] []
            wc "fui-breadcrumb" [ "items", items2; "separator", "›" ] []
            wc "fui-breadcrumb" [ "items", items3; "separator", "▸" ] []
        ] ]
    | "pagination" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "10 pages — first page" ]
                wc "fui-pagination" [ "total", "10"; "page", "1" ] []
            ]
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "10 pages — middle (page 5)" ]
                wc "fui-pagination" [ "total", "10"; "page", "5" ] []
            ]
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "50 pages — middle (page 25), siblings: 2" ]
                wc "fui-pagination" [ "total", "50"; "page", "25"; "siblings", "2" ] []
            ]
        ] ]
    | "stepper" ->
        let stepsJson     = """[{"label":"Account","description":"Create your account"},{"label":"Profile","description":"Fill in your details"},{"label":"Review","description":"Confirm and submit"}]"""
        let stepsJsonLong = """[{"label":"Plan","description":"Choose a plan"},{"label":"Build","description":"Configure your stack"},{"label":"Test","description":"Run checks"},{"label":"Deploy","description":"Ship to production"}]"""
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "2rem") ] ] [
            div [] [
                p [ Style [ Margin "0 0 0.75rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Horizontal — step 0 (first)" ]
                wc "fui-stepper" [ "steps", stepsJson; "active", "0" ] []
            ]
            div [] [
                p [ Style [ Margin "0 0 0.75rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Horizontal — step 1 (in progress)" ]
                wc "fui-stepper" [ "steps", stepsJson; "active", "1" ] []
            ]
            div [] [
                p [ Style [ Margin "0 0 0.75rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Vertical — clickable, 4 steps" ]
                wc "fui-stepper" [ "steps", stepsJsonLong; "active", "1"; "orientation", "vertical"; "clickable", "" ] []
            ]
        ] ]
    | "menu" ->
        let items1 = """[{"value":"edit","label":"Edit"},{"value":"dup","label":"Duplicate"},{"value":"","label":"","separator":true},{"value":"delete","label":"Delete"}]"""
        let items2 = """[{"value":"profile","label":"Profile"},{"value":"settings","label":"Settings"},{"value":"","label":"","separator":true},{"value":"logout","label":"Sign out"}]"""
        let items3 = """[{"value":"asc","label":"Sort A → Z"},{"value":"desc","label":"Sort Z → A"},{"value":"","label":"","separator":true},{"value":"filter","label":"Add filter"},{"value":"group","label":"Group by","disabled":true}]"""
        [ div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.75rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
            wc "fui-menu" [ "label", "Actions";       "items", items1 ] []
            wc "fui-menu" [ "label", "Account";       "items", items2; "placement", "bottom-end" ] []
            wc "fui-menu" [ "label", "Sort & Filter"; "items", items3 ] []
        ] ]
    | "modal" ->
        let sectionLabel txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.75rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
            div [] [
                sectionLabel "Small"
                wc "fui-modal" [ "trigger-label", "Open sm"; "title", "Small modal"; "size", "sm" ] [
                    p [ Style [ Margin "0"; CSSProp.Custom("color", "#A0A0A8") ] ] [ str "A compact modal — great for quick confirmations or short forms." ]
                    wc "fui-button" [ "slot", "footer"; "variant", "primary" ] [ str "Save" ]
                    wc "fui-button" [ "slot", "footer"; "variant", "secondary" ] [ str "Cancel" ]
                ]
            ]
            div [] [
                sectionLabel "Medium (default)"
                wc "fui-modal" [ "trigger-label", "Open md"; "title", "Edit profile" ] [
                    div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                        wc "fui-input" [ "label", "Display name"; "placeholder", "Jane Smith" ] []
                        wc "fui-input" [ "label", "Email"; "type", "email"; "placeholder", "jane@example.com" ] []
                        wc "fui-textarea" [ "label", "Bio"; "rows", "3"; "placeholder", "Tell us about yourself…" ] []
                    ]
                    wc "fui-button" [ "slot", "footer"; "variant", "primary" ] [ str "Save changes" ]
                    wc "fui-button" [ "slot", "footer"; "variant", "secondary" ] [ str "Discard" ]
                ]
            ]
            div [] [
                sectionLabel "Large"
                wc "fui-modal" [ "trigger-label", "Open lg"; "title", "Preview"; "size", "lg" ] [
                    p [ Style [ Margin "0"; CSSProp.Custom("color", "#A0A0A8") ] ] [ str "Large modals work well for previews, rich editors, or data tables." ]
                ]
            ]
        ] ]
    | "drawer" ->
        let sectionLabel txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.75rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
            div [] [
                sectionLabel "Right (default)"
                wc "fui-drawer" [ "trigger-label", "Open right"; "title", "Settings"; "placement", "right" ] [
                    div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                        wc "fui-toggle" [ "label", "Enable notifications"; "checked", "" ] []
                        wc "fui-toggle" [ "label", "Dark mode" ] []
                        wc "fui-select" [ "label", "Language"; "value", "en"; "options", """[{"value":"en","label":"English"},{"value":"fr","label":"French"},{"value":"de","label":"German"}]""" ] []
                    ]
                    wc "fui-button" [ "slot", "footer"; "variant", "primary" ] [ str "Save" ]
                    wc "fui-button" [ "slot", "footer"; "variant", "secondary" ] [ str "Cancel" ]
                ]
            ]
            div [] [
                sectionLabel "Left"
                wc "fui-drawer" [ "trigger-label", "Open left"; "title", "Navigation"; "placement", "left" ] [
                    div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.5rem") ] ] [
                        wc "fui-button" [ "variant", "ghost" ] [ str "Dashboard" ]
                        wc "fui-button" [ "variant", "ghost" ] [ str "Components" ]
                        wc "fui-button" [ "variant", "ghost" ] [ str "Settings" ]
                    ]
                ]
            ]
            div [] [
                sectionLabel "Bottom"
                wc "fui-drawer" [ "trigger-label", "Open bottom"; "title", "Share"; "placement", "bottom" ] [
                    p [ Style [ Margin "0 0 0.75rem"; CSSProp.Custom("color", "#A0A0A8"); FontSize "0.875rem" ] ] [ str "Share this document with your team." ]
                    wc "fui-input" [ "label", "Email address"; "placeholder", "team@example.com" ] []
                    wc "fui-button" [ "slot", "footer"; "variant", "primary" ] [ str "Send invite" ]
                ]
            ]
        ] ]
    | "tooltip" ->
        let sectionLabel txt = p [ Style [ Margin "0 0 0.75rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "2rem") ] ] [
            div [] [
                sectionLabel "Placements"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1.5rem"); CSSProp.Custom("align-items", "center"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
                    wc "fui-tooltip" [ "content", "Top tooltip"; "placement", "top" ] [
                        wc "fui-button" [ "variant", "secondary" ] [ str "Top" ]
                    ]
                    wc "fui-tooltip" [ "content", "Bottom tooltip"; "placement", "bottom" ] [
                        wc "fui-button" [ "variant", "secondary" ] [ str "Bottom" ]
                    ]
                    wc "fui-tooltip" [ "content", "Left tooltip"; "placement", "left" ] [
                        wc "fui-button" [ "variant", "secondary" ] [ str "Left" ]
                    ]
                    wc "fui-tooltip" [ "content", "Right tooltip"; "placement", "right" ] [
                        wc "fui-button" [ "variant", "secondary" ] [ str "Right" ]
                    ]
                ]
            ]
            div [] [
                sectionLabel "On any element"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1.5rem"); CSSProp.Custom("align-items", "center"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
                    wc "fui-tooltip" [ "content", "Save your changes" ] [
                        wc "fui-button" [ "variant", "primary" ] [ str "Save" ]
                    ]
                    wc "fui-tooltip" [ "content", "This field is required"; "placement", "right" ] [
                        wc "fui-input" [ "label", "Email"; "placeholder", "you@example.com" ] []
                    ]
                    wc "fui-tooltip" [ "content", "Online"; "placement", "bottom" ] [
                        wc "fui-badge" [ "variant", "success"; "dot", "" ] [ str "Active" ]
                    ]
                ]
            ]
        ] ]
    | "popover" ->
        let sectionLabel txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1.25rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
            div [] [
                sectionLabel "Filter panel"
                wc "fui-popover" [ "trigger-label", "Filter"; "width", "260px" ] [
                    div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                        wc "fui-select" [ "label", "Status"; "placeholder", "Any"; "options", """[{"value":"active","label":"Active"},{"value":"draft","label":"Draft"},{"value":"archived","label":"Archived"}]""" ] []
                        wc "fui-select" [ "label", "Type"; "placeholder", "Any"; "options", """[{"value":"page","label":"Page"},{"value":"post","label":"Post"},{"value":"media","label":"Media"}]""" ] []
                        wc "fui-button" [ "variant", "primary" ] [ str "Apply filters" ]
                    ]
                ]
            ]
            div [] [
                sectionLabel "Share panel (bottom-end)"
                wc "fui-popover" [ "trigger-label", "Share ▾"; "placement", "bottom-end"; "width", "260px" ] [
                    p [ Style [ Margin "0 0 0.625rem"; FontSize "0.875rem"; CSSProp.Custom("color", "#A0A0A8") ] ] [ str "Invite team members" ]
                    wc "fui-input" [ "placeholder", "name@example.com" ] []
                    div [ Style [ MarginTop "0.75rem"; Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.5rem") ] ] [
                        wc "fui-button" [ "variant", "primary" ] [ str "Send" ]
                        wc "fui-button" [ "variant", "secondary" ] [ str "Copy link" ]
                    ]
                ]
            ]
            div [] [
                sectionLabel "Info popover"
                wc "fui-popover" [ "trigger-label", "What is this?"; "width", "240px" ] [
                    p [ Style [ Margin "0"; FontSize "0.875rem"; LineHeight "1.6"; CSSProp.Custom("color", "#A0A0A8") ] ] [ str "Popovers accept any slotted HTML — text, forms, lists, or custom components." ]
                ]
            ]
        ] ]
    | "confirm-dialog" ->
        let sectionLabel txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1.25rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
            div [] [
                sectionLabel "Default variant"
                wc "fui-confirm-dialog" [
                    "trigger-label",  "Archive"
                    "title",          "Archive this project?"
                    "message",        "This will move it out of your active workspace."
                    "confirm-label",  "Archive"
                    "cancel-label",   "Keep active"
                ] []
            ]
            div [] [
                sectionLabel "Danger variant"
                wc "fui-confirm-dialog" [
                    "trigger-label",  "Delete account"
                    "title",          "Delete your account?"
                    "message",        "This action is permanent. All data will be erased."
                    "confirm-label",  "Yes, delete"
                    "variant",        "danger"
                ] []
            ]
            div [] [
                sectionLabel "With slot content"
                wc "fui-confirm-dialog" [
                    "trigger-label",  "Remove member"
                    "title",          "Remove Jane Smith?"
                    "confirm-label",  "Remove"
                    "variant",        "danger"
                ] [
                    p [ Style [ Margin "0 0 0.5rem"; FontSize "0.875rem"; CSSProp.Custom("color", "#A0A0A8") ] ] [ str "Jane will lose access to:" ]
                    ul [ Style [ Margin "0"; Padding "0 0 0 1.25rem"; FontSize "0.875rem"; CSSProp.Custom("color", "#A0A0A8") ] ] [
                        li [] [ str "All shared projects" ]
                        li [] [ str "Team settings" ]
                        li [] [ str "Billing information" ]
                    ]
                ]
            ]
        ] ]
    | "badge" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Variants" ]
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.5rem"); CSSProp.Custom("flex-wrap", "wrap"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-badge" [] [ str "Neutral" ]
                    wc "fui-badge" [ "variant", "success" ] [ str "Success" ]
                    wc "fui-badge" [ "variant", "warning" ] [ str "Warning" ]
                    wc "fui-badge" [ "variant", "danger"  ] [ str "Danger" ]
                    wc "fui-badge" [ "variant", "info"    ] [ str "Info" ]
                    wc "fui-badge" [ "variant", "accent"  ] [ str "Accent" ]
                ]
            ]
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "With dot indicator" ]
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.5rem"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-badge" [ "variant", "success"; "dot", "" ] [ str "Online" ]
                    wc "fui-badge" [ "variant", "danger";  "dot", "" ] [ str "Offline" ]
                    wc "fui-badge" [ "variant", "warning"; "dot", "" ] [ str "Away" ]
                    wc "fui-badge" [ "variant", "info";    "dot", "" ] [ str "Busy" ]
                ]
            ]
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Sizes" ]
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.5rem"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-badge" [ "variant", "accent"; "size", "sm" ] [ str "Small" ]
                    wc "fui-badge" [ "variant", "accent" ] [ str "Medium" ]
                    wc "fui-badge" [ "variant", "accent"; "size", "lg" ] [ str "Large" ]
                ]
            ]
        ] ]
    | "spinner" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Sizes" ]
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1.5rem"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-spinner" [ "size", "sm" ] []
                    wc "fui-spinner" [] []
                    wc "fui-spinner" [ "size", "lg" ] []
                    wc "fui-spinner" [ "size", "xl" ] []
                ]
            ]
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "With label" ]
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1.5rem"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-spinner" [ "label", "Loading…" ] []
                    wc "fui-spinner" [ "size", "lg"; "label", "Processing…" ] []
                    wc "fui-spinner" [ "size", "xl"; "label", "Please wait" ] []
                ]
            ]
        ] ]
    | "progress" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1rem") ] ] [
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Variants" ]
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.625rem") ] ] [
                    wc "fui-progress" [ "label", "Upload";   "value", "65" ] []
                    wc "fui-progress" [ "label", "Health";   "value", "100"; "variant", "success" ] []
                    wc "fui-progress" [ "label", "Memory";   "value", "55";  "variant", "warning" ] []
                    wc "fui-progress" [ "label", "Storage";  "value", "90";  "variant", "danger"  ] []
                    wc "fui-progress" [ "label", "Network";  "value", "40";  "variant", "info"    ] []
                ]
            ]
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Indeterminate" ]
                wc "fui-progress" [ "label", "Loading…"; "indeterminate", "" ] []
            ]
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Sizes" ]
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.625rem") ] ] [
                    wc "fui-progress" [ "label", "Small";  "size", "sm"; "value", "60" ] []
                    wc "fui-progress" [ "label", "Medium"; "value", "60" ] []
                    wc "fui-progress" [ "label", "Large";  "size", "lg"; "value", "60" ] []
                ]
            ]
        ] ]
    | "alert" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-alert" [ "variant", "info" ] [ str "Your session will expire in 10 minutes." ]
            wc "fui-alert" [ "variant", "success"; "title", "Changes saved" ] [ str "Your profile has been updated." ]
            wc "fui-alert" [ "variant", "warning"; "title", "Storage almost full" ] [ str "You have used 90% of your quota." ]
            wc "fui-alert" [ "variant", "danger";  "title", "Action required"; "dismissible", "" ] [ str "Please update your billing details." ]
        ] ]
    | "skeleton" ->
        let sectionLabel txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [
                sectionLabel "Rect — block placeholder"
                wc "fui-skeleton" [ "height", "100px" ] []
            ]
            div [] [
                sectionLabel "Text — 3 lines"
                wc "fui-skeleton" [ "variant", "text"; "lines", "3" ] []
            ]
            div [] [
                sectionLabel "Circle — avatar"
                wc "fui-skeleton" [ "variant", "circle"; "width", "56px" ] []
            ]
            div [] [
                sectionLabel "Card loading pattern"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.875rem"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-skeleton" [ "variant", "circle"; "width", "48px" ] []
                    div [ Style [ CSSProp.Custom("flex", "1") ] ] [
                        wc "fui-skeleton" [ "variant", "text"; "lines", "2"; "height", "0.75rem" ] []
                    ]
                ]
            ]
        ] ]
    | "toast" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem"); CSSProp.Custom("max-width", "380px") ] ] [
            wc "fui-toast" [ "variant", "success"; "title", "Saved";   "message", "Your changes have been saved." ] []
            wc "fui-toast" [ "variant", "danger";  "title", "Error";   "message", "Failed to process the request." ] []
            wc "fui-toast" [ "variant", "warning"; "title", "Warning"; "message", "Storage is running low." ] []
            wc "fui-toast" [ "variant", "info";    "title", "Info";    "message", "A new version is available." ] []
        ] ]
    | "empty-state" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [ Style [ CSSProp.Custom("border", "1px solid #2A2A2E"); CSSProp.Custom("border-radius", "8px") ] ] [
                wc "fui-empty-state" [ "title", "No results found"; "description", "Try adjusting your search or filters." ] []
            ]
            div [ Style [ CSSProp.Custom("border", "1px solid #2A2A2E"); CSSProp.Custom("border-radius", "8px") ] ] [
                wc "fui-empty-state" [ "title", "Your inbox is empty"; "description", "Messages from your team will appear here." ] [
                    span [ HTMLAttr.Custom("slot", "icon") ] [ str "📭" ]
                    wc "fui-button" ([ "slot", "action"; "variant", "primary" ]) [ str "Compose message" ]
                ]
            ]
        ] ]
    | "card" ->
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [
                sl "Default — header / body / footer slots"
                wc "fui-card" [] [
                    span [ HTMLAttr.Custom("slot", "header") ] [ str "Card Title" ]
                    p [ Style [ Margin "0" ] ] [ str "Body content rendered in the default slot. Add anything here." ]
                    div [ HTMLAttr.Custom("slot", "footer") ] [
                        wc "fui-button" [ "variant", "primary";   "size", "sm" ] [ str "Save" ]
                        wc "fui-button" [ "variant", "secondary"; "size", "sm" ] [ str "Cancel" ]
                    ]
                ]
            ]
            div [] [
                sl "Variants"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
                    div [ Style [ CSSProp.Custom("flex", "1"); CSSProp.Custom("min-width", "160px") ] ] [
                        sl "Elevated"
                        wc "fui-card" [ "variant", "elevated" ] [
                            p [ Style [ Margin "0" ] ] [ str "Elevated — deeper shadow." ]
                        ]
                    ]
                    div [ Style [ CSSProp.Custom("flex", "1"); CSSProp.Custom("min-width", "160px") ] ] [
                        sl "Outline"
                        wc "fui-card" [ "variant", "outline" ] [
                            p [ Style [ Margin "0" ] ] [ str "Outline — transparent background." ]
                        ]
                    ]
                    div [ Style [ CSSProp.Custom("flex", "1"); CSSProp.Custom("min-width", "160px") ] ] [
                        sl "Ghost"
                        wc "fui-card" [ "variant", "ghost" ] [
                            p [ Style [ Margin "0" ] ] [ str "Ghost — no border." ]
                        ]
                    ]
                ]
            ]
            div [] [
                sl "Padding sizes"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
                    div [ Style [ CSSProp.Custom("flex", "1") ] ] [
                        wc "fui-card" [ "padding", "sm" ] [
                            span [ HTMLAttr.Custom("slot", "header") ] [ str "Small (sm)" ]
                            p [ Style [ Margin "0" ] ] [ str "padding=\"sm\"" ]
                        ]
                    ]
                    div [ Style [ CSSProp.Custom("flex", "1") ] ] [
                        wc "fui-card" [ "padding", "lg" ] [
                            span [ HTMLAttr.Custom("slot", "header") ] [ str "Large (lg)" ]
                            p [ Style [ Margin "0" ] ] [ str "padding=\"lg\"" ]
                        ]
                    ]
                    div [ Style [ CSSProp.Custom("flex", "1") ] ] [
                        wc "fui-card" [ "padding", "none" ] [
                            span [ HTMLAttr.Custom("slot", "header") ] [ str "None" ]
                            p [ Style [ Margin "0" ] ] [ str "padding=\"none\"" ]
                        ]
                    ]
                ]
            ]
        ] ]
    | "stat" ->
        [ div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
            wc "fui-stat" [ "label", "Revenue";    "value", "$48,200"; "change", "12.5%"; "trend", "up";   "description", "vs last month" ] []
            wc "fui-stat" [ "label", "Users";      "value", "8,412";   "change", "3.1%";  "trend", "down" ] []
            wc "fui-stat" [ "label", "Error rate"; "value", "0.02%";   "change", "stable"; "trend", "flat" ] []
            wc "fui-stat" [ "label", "Uptime";     "value", "99.9%" ] []
        ] ]
    | "avatar" ->
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [
                sl "Sizes"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.75rem"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-avatar" [ "initials", "XS"; "size", "xs" ] []
                    wc "fui-avatar" [ "initials", "SM"; "size", "sm" ] []
                    wc "fui-avatar" [ "initials", "MD" ] []
                    wc "fui-avatar" [ "initials", "LG"; "size", "lg" ] []
                    wc "fui-avatar" [ "initials", "XL"; "size", "xl" ] []
                ]
            ]
            div [] [
                sl "Status indicators"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.75rem"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-avatar" [ "initials", "ON"; "status", "online"  ] []
                    wc "fui-avatar" [ "initials", "OF"; "status", "offline" ] []
                    wc "fui-avatar" [ "initials", "AW"; "status", "away"    ] []
                    wc "fui-avatar" [ "initials", "BZ"; "status", "busy"    ] []
                ]
            ]
        ] ]
    | "avatar-group" ->
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        let av5 = """[{"src":"","initials":"JD"},{"src":"","initials":"AB"},{"src":"","initials":"MK"},{"src":"","initials":"RQ"},{"src":"","initials":"TW"}]"""
        let av6 = """[{"src":"","initials":"AA"},{"src":"","initials":"BB"},{"src":"","initials":"CC"},{"src":"","initials":"DD"},{"src":"","initials":"EE"},{"src":"","initials":"FF"}]"""
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [
                sl "Default — max 4"
                wc "fui-avatar-group" [ "avatars", av5 ] []
            ]
            div [] [
                sl "Max 3, 6 avatars → +3 overflow"
                wc "fui-avatar-group" [ "avatars", av6; "max", "3" ] []
            ]
            div [] [
                sl "Sizes"
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                    wc "fui-avatar-group" [ "avatars", av5; "size", "sm" ] []
                    wc "fui-avatar-group" [ "avatars", av5 ] []
                    wc "fui-avatar-group" [ "avatars", av5; "size", "lg" ] []
                ]
            ]
        ] ]
    | "tag" ->
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
            div [] [
                sl "Variants"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.5rem"); CSSProp.Custom("flex-wrap", "wrap"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-tag" [] [ str "Neutral" ]
                    wc "fui-tag" [ "variant", "success" ] [ str "Success" ]
                    wc "fui-tag" [ "variant", "warning" ] [ str "Warning" ]
                    wc "fui-tag" [ "variant", "danger"  ] [ str "Danger"  ]
                    wc "fui-tag" [ "variant", "info"    ] [ str "Info"    ]
                    wc "fui-tag" [ "variant", "accent"  ] [ str "Accent"  ]
                ]
            ]
            div [] [
                sl "Sizes"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.5rem"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-tag" [ "variant", "accent"; "size", "sm" ] [ str "Small" ]
                    wc "fui-tag" [ "variant", "accent" ] [ str "Medium" ]
                    wc "fui-tag" [ "variant", "accent"; "size", "lg" ] [ str "Large" ]
                ]
            ]
            div [] [
                sl "Removable"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.5rem"); CSSProp.Custom("flex-wrap", "wrap"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-tag" [ "removable", "" ] [ str "React" ]
                    wc "fui-tag" [ "variant", "success"; "removable", "" ] [ str "TypeScript" ]
                    wc "fui-tag" [ "variant", "accent";  "removable", "" ] [ str "F#" ]
                    wc "fui-tag" [ "variant", "info";    "removable", "" ] [ str "Fable" ]
                ]
            ]
        ] ]
    | "code-block" ->
        let fsCode  = "let add x y = x + y\nprintfn \"Result: %d\" (add 3 4)"
        let cssCode = ".button {\n    background: #7C3AED;\n    border-radius: 6px;\n    color: #fff;\n    padding: 0.5rem 1rem;\n}"
        let sqlCode = "SELECT id, email, role\nFROM users\nWHERE active = true\nORDER BY email;"
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1rem") ] ] [
            wc "fui-code-block" [ "language", "fsharp"; "code", fsCode  ] []
            wc "fui-code-block" [ "language", "css";    "code", cssCode ] []
            wc "fui-code-block" [ "language", "sql";    "code", sqlCode ] []
            wc "fui-code-block" [ "code", "npm install && npm run start" ] []
        ] ]
    | "callout" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-callout" [ "variant", "info";    "title", "Information" ] [ str "This is an informational callout with a blue accent stripe." ]
            wc "fui-callout" [ "variant", "success"; "title", "Success"     ] [ str "Operation completed successfully — all checks passed." ]
            wc "fui-callout" [ "variant", "warning"; "title", "Warning"     ] [ str "Please review before proceeding — some items need attention." ]
            wc "fui-callout" [ "variant", "danger";  "title", "Error"       ] [ str "Something went wrong. Check the logs for details." ]
            wc "fui-callout" [ "variant", "note";    "title", "Note"        ] [ str "This component uses a purple accent colour for editorial notes." ]
            wc "fui-callout" [ "variant", "info" ] [ str "Callout without a title — body content only via the default slot." ]
        ] ]
    | "list" ->
        let items = """["Build the UI components","Add backend endpoints","Write unit tests","Deploy to production"]"""
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [ sl "Default (unordered)"; wc "fui-list" [ "items", items ] [] ]
            div [] [ sl "Ordered";             wc "fui-list" [ "items", items; "ordered", "" ] [] ]
            div [] [ sl "Bordered";            wc "fui-list" [ "items", items; "variant", "bordered" ] [] ]
            div [] [ sl "Striped";             wc "fui-list" [ "items", items; "variant", "striped"  ] [] ]
            div [] [ sl "Bullet";              wc "fui-list" [ "items", items; "variant", "bullet"   ] [] ]
        ] ]
    | "timeline" ->
        let items = """[{"label":"Project started","description":"Initial planning and design phase completed.","date":"Jan 2025","icon":""},{"label":"Alpha release","description":"First internal build shipped to QA.","date":"Mar 2025","icon":""},{"label":"Beta launch","description":"Public beta released with 500 early-access users.","date":"Jun 2025","icon":""},{"label":"v1.0 shipped","description":"General availability — stable release.","date":"Sep 2025","icon":""}]"""
        [ wc "fui-timeline" [ "items", items ] [] ]
    | "accordion" ->
        let items = """[{"label":"What is FableUI?","content":"FableUI is a library of standalone Web Components built with Fable and Lit. Drop any fui-* element into any HTML page and it works immediately."},{"label":"Do I need a JavaScript framework?","content":"No — every component is a native Custom Element and works in plain HTML, React, Vue, Svelte, or any other environment."},{"label":"Can I theme the components?","content":"Yes. All components expose CSS custom properties (--fui-*) so you can override colours, fonts, radii, and spacing from a stylesheet or inline style."},{"label":"How does sorting work in fui-table?","content":"Column sorting is done client-side by the component. Set sortable on the element and mark individual columns as sortable: true in the columns JSON."}]"""
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "2rem") ] ] [
            div [] [
                sl "Single open (default)"
                wc "fui-accordion" [ "items", items ] []
            ]
            div [] [
                sl "Multiple open"
                wc "fui-accordion" [ "items", items; "multiple", "" ] []
            ]
        ] ]
    | "carousel" ->
        let items = """[{"title":"Components","description":"Drop any fui-* element into your HTML and it works immediately — no build step required."},{"title":"Theming","description":"Every component exposes CSS custom properties for full design token control."},{"title":"Accessibility","description":"All interactive elements meet WCAG 2.1 AA keyboard and ARIA requirements."}]"""
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "2rem") ] ] [
            div [] [
                sl "With dot navigation (default)"
                wc "fui-carousel" [ "items", items ] []
            ]
            div [] [
                sl "Loop enabled"
                wc "fui-carousel" [ "items", items; "loop", "" ] []
            ]
        ] ]
    | "table" ->
        let cols = """[{"key":"name","label":"Name","sortable":true},{"key":"email","label":"Email","sortable":true},{"key":"role","label":"Role","sortable":false}]"""
        let rows = """[["Alice Chen","alice@example.com","Admin"],["Bob Smith","bob@example.com","Developer"],["Carol Davis","carol@example.com","Designer"],["Dan Wilson","dan@example.com","Developer"],["Eve Martinez","eve@example.com","Admin"]]"""
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "2rem") ] ] [
            div [] [
                sl "Sortable — click Name or Email header"
                wc "fui-table" [ "columns", cols; "rows", rows; "sortable", "" ] []
            ]
            div [] [
                sl "Striped rows"
                wc "fui-table" [ "columns", cols; "rows", rows; "striped", "" ] []
            ]
            div [] [
                sl "Sortable + striped"
                wc "fui-table" [ "columns", cols; "rows", rows; "sortable", ""; "striped", "" ] []
            ]
        ] ]
    | "resizable-panel" ->
        let panelSt bg = Style [ Padding "1rem"; Background bg; Height "100%"; Display DisplayOptions.Flex; CSSProp.Custom("align-items", "center"); CSSProp.Custom("justify-content", "center"); Color "#6E6E76"; FontFamily "'Sora',sans-serif"; FontSize ".875rem" ]
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "2rem") ] ] [
            div [] [
                sl "Horizontal — drag the divider"
                div [ Style [ Height "200px" ] ] [
                    wc "fui-resizable-panel" [ "default-size", "260"; "min-size", "100"; "max-size", "500" ] [
                        domEl "div" [ HTMLAttr.Custom("slot", box "start"); panelSt "#161618" ] [ str "Start panel" ]
                        domEl "div" [ HTMLAttr.Custom("slot", box "end");   panelSt "#1E1E21" ] [ str "End panel" ]
                    ]
                ]
            ]
            div [] [
                sl "Vertical — drag the divider"
                div [ Style [ Height "280px" ] ] [
                    wc "fui-resizable-panel" [ "direction", "vertical"; "default-size", "110"; "min-size", "60"; "max-size", "180" ] [
                        domEl "div" [ HTMLAttr.Custom("slot", box "start"); Style [ Padding "1rem"; Background "#161618"; Width "100%"; Display DisplayOptions.Flex; CSSProp.Custom("align-items", "center"); CSSProp.Custom("justify-content", "center"); Color "#6E6E76"; FontFamily "'Sora',sans-serif"; FontSize ".875rem" ] ] [ str "Top panel" ]
                        domEl "div" [ HTMLAttr.Custom("slot", box "end");   Style [ Padding "1rem"; Background "#1E1E21"; Width "100%"; Display DisplayOptions.Flex; CSSProp.Custom("align-items", "center"); CSSProp.Custom("justify-content", "center"); Color "#6E6E76"; FontFamily "'Sora',sans-serif"; FontSize ".875rem"; Flex "1" ] ] [ str "Bottom panel" ]
                    ]
                ]
            ]
        ] ]
    | "error-boundary" ->
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        let stackTrace = "TypeError: Cannot read properties of undefined (reading 'map')\n  at renderList (app.js:142:18)\n  at Dashboard (app.js:89:5)\n  at async loadPage (router.js:34:3)"
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "2rem") ] ] [
            div [] [
                sl "Default"
                wc "fui-error-boundary" [] []
            ]
            div [] [
                sl "With retry button"
                wc "fui-error-boundary" [ "title", "Failed to load data"; "message", "Could not reach the server. Check your connection and try again."; "retryable", "" ] []
            ]
            div [] [
                sl "With stack trace"
                wc "fui-error-boundary" [ "title", "Unhandled exception"; "message", "An internal error occurred while rendering this component."; "retryable", ""; "code", stackTrace ] []
            ]
        ] ]
    | "time-picker" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem"); MaxWidth "320px" ] ] [
            wc "fui-time-picker" [ "label", "Meeting time" ] []
            wc "fui-time-picker" [ "label", "Business hours"; "min", "08:00"; "max", "18:00" ] []
            wc "fui-time-picker" [ "label", "Disabled"; "disabled", ""; "value", "14:30" ] []
            wc "fui-time-picker" [ "label", "With error"; "error", "Please enter a valid time" ] []
        ] ]
    | "combobox" ->
        let fwOpts = """[{"value":"react","label":"React"},{"value":"vue","label":"Vue"},{"value":"svelte","label":"Svelte"},{"value":"solid","label":"Solid"},{"value":"angular","label":"Angular"},{"value":"fable","label":"Fable"}]"""
        let langOpts = """[{"value":"fsharp","label":"F#"},{"value":"csharp","label":"C#"},{"value":"python","label":"Python"},{"value":"typescript","label":"TypeScript"},{"value":"rust","label":"Rust"}]"""
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem"); MaxWidth "320px" ] ] [
            wc "fui-combobox" [ "label", "Framework"; "placeholder", "Search or pick..."; "options", fwOpts ] []
            wc "fui-combobox" [ "label", "Language"; "placeholder", "Type to filter..."; "options", langOpts; "value", "fsharp" ] []
            wc "fui-combobox" [ "label", "Disabled"; "disabled", ""; "options", fwOpts; "value", "react" ] []
            wc "fui-combobox" [ "label", "With error"; "placeholder", "Required"; "options", fwOpts; "error", "Please select a framework" ] []
        ] ]
    | "form-field" ->
        let baseInputStyle =
            Style [ Width "100%"; CSSProp.Custom("padding", ".5rem .75rem"); CSSProp.Custom("background", "#1E1E21")
                    CSSProp.Custom("border", "1px solid #2A2A2E"); BorderRadius "6px"; Color "#E8E8ED"
                    FontFamily "'JetBrains Mono', monospace"; FontSize ".875rem"
                    CSSProp.Custom("box-sizing", "border-box"); Outline "none" ]
        let errInputStyle =
            Style [ Width "100%"; CSSProp.Custom("padding", ".5rem .75rem"); CSSProp.Custom("background", "#1E1E21")
                    CSSProp.Custom("border", "1px solid #EF4444"); BorderRadius "6px"; Color "#E8E8ED"
                    FontFamily "'JetBrains Mono', monospace"; FontSize ".875rem"
                    CSSProp.Custom("box-sizing", "border-box"); Outline "none" ]
        let taStyle =
            Style [ Width "100%"; CSSProp.Custom("padding", ".5rem .75rem"); CSSProp.Custom("background", "#1E1E21")
                    CSSProp.Custom("border", "1px solid #2A2A2E"); BorderRadius "6px"; Color "#E8E8ED"
                    FontFamily "'JetBrains Mono', monospace"; FontSize ".875rem"
                    CSSProp.Custom("box-sizing", "border-box"); Outline "none"
                    CSSProp.Custom("resize", "vertical") ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1rem"); MaxWidth "360px" ] ] [
            wc "fui-form-field" [ "label", "Username"; "hint", "Letters and numbers only"; "required", "" ] [
                domEl "input" [ HTMLAttr.Type "text"; HTMLAttr.Placeholder "jdoe"; baseInputStyle ] []
            ]
            wc "fui-form-field" [ "label", "Email"; "error", "Not a valid email address"; "required", "" ] [
                domEl "input" [ HTMLAttr.Type "email"; HTMLAttr.DefaultValue "bad-email"; errInputStyle ] []
            ]
            wc "fui-form-field" [ "label", "Bio"; "hint", "Max 160 characters" ] [
                domEl "textarea" [ HTMLAttr.Rows 3; taStyle ] []
            ]
        ] ]
    | "form" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "2rem"); MaxWidth "400px" ] ] [
            div [] [
                p [ Style [ Margin "0 0 0.75rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Default form" ]
                wc "fui-form" [] [
                    wc "fui-input" [ "label", "Email"; "type", "email"; "placeholder", "you@example.com" ] []
                    wc "fui-input" [ "label", "Password"; "type", "password"; "placeholder", "••••••••" ] []
                    wc "fui-button" [ "variant", "primary"; "type", "submit" ] [ str "Sign in" ]
                ]
            ]
            div [] [
                p [ Style [ Margin "0 0 0.75rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Error state" ]
                wc "fui-form" [ "error", "Invalid email or password. Please try again." ] [
                    wc "fui-input" [ "label", "Email"; "type", "email"; "error", "Check your email" ] []
                    wc "fui-button" [ "variant", "primary"; "type", "submit" ] [ str "Retry" ]
                ]
            ]
            div [] [
                p [ Style [ Margin "0 0 0.75rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Loading state" ]
                wc "fui-form" [ "loading", "" ] [
                    wc "fui-input" [ "label", "Email"; "type", "email" ] []
                    wc "fui-button" [ "variant", "primary"; "type", "submit" ] [ str "Submitting..." ]
                ]
            ]
        ] ]
    | "link" ->
        let row children = div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1.5rem"); CSSProp.Custom("align-items", "center"); CSSProp.Custom("flex-wrap", "wrap") ] ] children
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
            div [] [
                sl "Variants"
                row [
                    wc "fui-link" [ "href", "#" ] [ str "Default (accent)" ]
                    wc "fui-link" [ "href", "#"; "variant", "muted"  ] [ str "Muted" ]
                    wc "fui-link" [ "href", "#"; "variant", "subtle" ] [ str "Subtle" ]
                ]
            ]
            div [] [
                sl "External link"
                row [
                    wc "fui-link" [ "href", "https://fable.io"; "external", "" ] [ str "Fable.io" ]
                    wc "fui-link" [ "href", "https://lit.dev";  "external", "" ] [ str "Lit.dev" ]
                ]
            ]
            div [] [
                sl "Inline in text"
                p [ Style [ Margin "0"; CSSProp.Custom("color", "#A0A0A8"); CSSProp.Custom("font-family", "Sora, sans-serif"); CSSProp.Custom("font-size", "0.9375rem"); CSSProp.Custom("line-height", "1.6") ] ] [
                    str "Read the "
                    wc "fui-link" [ "href", "#" ] [ str "documentation" ]
                    str " to get started, then explore "
                    wc "fui-link" [ "href", "#"; "variant", "muted" ] [ str "examples" ]
                    str "."
                ]
            ]
        ] ]
    | "context-menu" ->
        let items = """[{"value":"copy","label":"Copy","disabled":false,"separator":false},{"value":"cut","label":"Cut","disabled":false,"separator":false},{"value":"paste","label":"Paste","disabled":true,"separator":false},{"value":"","label":"","disabled":false,"separator":true},{"value":"delete","label":"Delete","disabled":false,"separator":false}]"""
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1rem") ] ] [
            p [ Style [ Margin "0"; FontSize "0.875rem"; CSSProp.Custom("color", "#6E6E76") ] ] [ str "Right-click inside the dashed area to open the context menu." ]
            wc "fui-context-menu" [ "items", items ] [
                div [ Style [ Padding "3rem 2rem"; CSSProp.Custom("border", "1px dashed #2A2A2E"); CSSProp.Custom("border-radius", "6px"); CSSProp.Custom("color", "#4A4A52"); CSSProp.Custom("font-family", "Sora, sans-serif"); CSSProp.Custom("text-align", "center"); CSSProp.Custom("font-size", "0.875rem") ] ] [
                    str "Right-click anywhere here"
                ]
            ]
        ] ]
    | "command-palette" ->
        let items = """[{"id":"button","label":"Button","description":"Interactive clickable element","group":"Components"},{"id":"input","label":"Input","description":"Single-line text field","group":"Components"},{"id":"modal","label":"Modal","description":"Dialog overlay component","group":"Overlay"},{"id":"drawer","label":"Drawer","description":"Side-panel overlay","group":"Overlay"},{"id":"theme","label":"Toggle theme","description":"Switch between light and dark mode","group":"Actions"},{"id":"docs","label":"Open docs","description":"View the full documentation","group":"Actions"}]"""
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1rem") ] ] [
            p [ Style [ Margin "0"; FontSize "0.875rem"; CSSProp.Custom("color", "#6E6E76") ] ] [ str "Click the trigger button to open the palette. Use ↑↓ to navigate, ↵ to select, Esc to close." ]
            wc "fui-command-palette" [ "trigger-label", "⌘ K — Search commands"; "placeholder", "Search components or actions..."; "items", items ] []
        ] ]
    | "sidenav" ->
        let groups = """[{"label":"Inputs","links":[{"label":"Button","href":"#","active":true},{"label":"Input","href":"#","active":false},{"label":"Select","href":"#","active":false}]},{"label":"Layout","links":[{"label":"Grid","href":"#","active":false},{"label":"Stack","href":"#","active":false},{"label":"Divider","href":"#","active":false}]},{"label":"Feedback","links":[{"label":"Alert","href":"#","active":false},{"label":"Toast","href":"#","active":false}]}]"""
        [ div [ Style [ Height "360px" ] ] [
            wc "fui-sidenav" [ "logo", "FableUI"; "groups", groups ] []
        ] ]
    | "topnav" ->
        [ wc "fui-topnav" [ "logo", "FableUI" ] [
            domEl "a" [ HTMLAttr.Href "#"; HTMLAttr.ClassName "active" ] [ str "Components" ]
            domEl "a" [ HTMLAttr.Href "#" ] [ str "Docs" ]
            domEl "a" [ HTMLAttr.Href "#" ] [ str "Examples" ]
            domEl "button" [ HTMLAttr.Custom("slot", box "actions") ] [ str "⬡ Theme" ]
            domEl "button" [ HTMLAttr.Custom("slot", box "actions") ] [ str "GitHub" ]
        ] ]
    | "heading" ->
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-heading" [ "level", "1" ] [ str "Heading Level 1" ]
            wc "fui-heading" [ "level", "2" ] [ str "Heading Level 2" ]
            wc "fui-heading" [ "level", "3" ] [ str "Heading Level 3" ]
            wc "fui-heading" [ "level", "4" ] [ str "Heading Level 4" ]
            wc "fui-heading" [ "level", "5" ] [ str "Heading Level 5" ]
            wc "fui-heading" [ "level", "6" ] [ str "Heading Level 6" ]
            div [ Style [ MarginTop "0.5rem" ] ] [
                sl "Colour variants"
                wc "fui-heading" [ "level", "2"; "color", "muted"  ] [ str "Muted heading" ]
                wc "fui-heading" [ "level", "2"; "color", "accent" ] [ str "Accent heading" ]
            ]
            div [] [
                sl "Truncation"
                div [ Style [ Width "260px" ] ] [
                    wc "fui-heading" [ "level", "3"; "truncate", "" ] [ str "This heading is clipped at 260px wide" ]
                ]
            ]
        ] ]
    | "text" ->
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1rem") ] ] [
            div [] [
                sl "Sizes"
                wc "fui-text" [ "size", "xs" ] [ str "Extra small (xs) — 0.75rem" ]
                wc "fui-text" [ "size", "sm" ] [ str "Small (sm) — 0.875rem" ]
                wc "fui-text" [ "size", "md" ] [ str "Medium (md) — 0.9375rem — default" ]
                wc "fui-text" [ "size", "lg" ] [ str "Large (lg) — 1.125rem" ]
                wc "fui-text" [ "size", "xl" ] [ str "Extra large (xl) — 1.25rem" ]
            ]
            div [] [
                sl "Colour variants"
                wc "fui-text" [] [ str "Default text colour" ]
                wc "fui-text" [ "color", "muted"   ] [ str "Muted text" ]
                wc "fui-text" [ "color", "accent"  ] [ str "Accent text" ]
                wc "fui-text" [ "color", "success" ] [ str "Success text" ]
                wc "fui-text" [ "color", "warning" ] [ str "Warning text" ]
                wc "fui-text" [ "color", "danger"  ] [ str "Danger text" ]
                wc "fui-text" [ "color", "info"    ] [ str "Info text" ]
            ]
            div [] [
                sl "Weights"
                wc "fui-text" [ "weight", "light"  ] [ str "Light (300)" ]
                wc "fui-text" [ "weight", "normal" ] [ str "Normal (400)" ]
                wc "fui-text" [ "weight", "medium" ] [ str "Medium (500)" ]
                wc "fui-text" [ "weight", "semi"   ] [ str "Semi-bold (600)" ]
                wc "fui-text" [ "weight", "bold"   ] [ str "Bold (700)" ]
            ]
        ] ]
    | "label" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1rem") ] ] [
            div [] [
                wc "fui-label" [] [ str "Email address" ]
            ]
            div [] [
                wc "fui-label" [ "required", "" ] [ str "Password" ]
            ]
            div [] [
                wc "fui-label" [ "size", "sm" ] [ str "Small label" ]
                wc "fui-label" [ "size", "md" ] [ str "Medium label" ]
                wc "fui-label" [ "size", "lg" ] [ str "Large label" ]
            ]
        ] ]
    | "code" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            p [ Style [ Margin "0"; CSSProp.Custom("color", "#E8E8ED"); CSSProp.Custom("font-family", "Sora, sans-serif"); CSSProp.Custom("font-size", "0.9375rem"); CSSProp.Custom("line-height", "1.6") ] ] [
                str "Run "
                wc "fui-code" [] [ str "dotnet fable watch ." ]
                str " to start the Fable compiler watcher."
            ]
            p [ Style [ Margin "0"; CSSProp.Custom("color", "#E8E8ED"); CSSProp.Custom("font-family", "Sora, sans-serif"); CSSProp.Custom("font-size", "0.9375rem"); CSSProp.Custom("line-height", "1.6") ] ] [
                str "Set "
                wc "fui-code" [] [ str "NODE_ENV=production" ]
                str " before running "
                wc "fui-code" [] [ str "npm run build" ]
                str "."
            ]
            p [ Style [ Margin "0"; CSSProp.Custom("color", "#E8E8ED"); CSSProp.Custom("font-family", "Sora, sans-serif"); CSSProp.Custom("font-size", "0.9375rem"); CSSProp.Custom("line-height", "1.6") ] ] [
                str "The entry point is "
                wc "fui-code" [] [ str "src/Client/Client.fs" ]
                str "."
            ]
        ] ]
    | "kbd" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            p [ Style [ Margin "0"; CSSProp.Custom("color", "#E8E8ED"); CSSProp.Custom("font-family", "Sora, sans-serif"); CSSProp.Custom("font-size", "0.9375rem") ] ] [
                str "Press "
                wc "fui-kbd" [] [ str "Ctrl" ]
                str " + "
                wc "fui-kbd" [] [ str "K" ]
                str " to open the command palette."
            ]
            p [ Style [ Margin "0"; CSSProp.Custom("color", "#E8E8ED"); CSSProp.Custom("font-family", "Sora, sans-serif"); CSSProp.Custom("font-size", "0.9375rem") ] ] [
                str "Save with "
                wc "fui-kbd" [] [ str "⌘" ]
                str " "
                wc "fui-kbd" [] [ str "S" ]
                str " on Mac or "
                wc "fui-kbd" [] [ str "Ctrl" ]
                str " "
                wc "fui-kbd" [] [ str "S" ]
                str " on Windows."
            ]
            p [ Style [ Margin "0"; CSSProp.Custom("color", "#E8E8ED"); CSSProp.Custom("font-family", "Sora, sans-serif"); CSSProp.Custom("font-size", "0.9375rem") ] ] [
                str "Close overlays with "
                wc "fui-kbd" [] [ str "Esc" ]
                str "."
            ]
        ] ]
    | "blockquote" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            wc "fui-blockquote" [ "cite", "— Donald Knuth" ] [ str "Premature optimisation is the root of all evil." ]
            wc "fui-blockquote" [ "cite", "— Martin Fowler" ] [ str "Any fool can write code that a computer can understand. Good programmers write code that humans can understand." ]
            wc "fui-blockquote" [] [ str "The best tool for the job is the one you can actually ship with." ]
        ] ]
    | "prose" ->
        [ wc "fui-prose" [] [
            h2 [] [ str "Getting started" ]
            p [] [ str "Install the components and drop them into any HTML page. No framework required." ]
            h3 [] [ str "Installation" ]
            p [] [
                str "Add the script to your page, then use "
                code [] [ str "<fui-*>" ]
                str " elements anywhere in your markup."
            ]
            ul [] [
                li [] [ str "Zero dependencies at runtime" ]
                li [] [ str "Shadow DOM encapsulation — styles never leak" ]
                li [] [ str "Full keyboard and ARIA accessibility" ]
            ]
            p [] [ str "All components support CSS custom properties for theming." ]
        ] ]
    | "bar-chart" ->
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        let weekData   = """[{"label":"Mon","value":42},{"label":"Tue","value":67},{"label":"Wed","value":55},{"label":"Thu","value":80},{"label":"Fri","value":73},{"label":"Sat","value":91},{"label":"Sun","value":38}]"""
        let salesData  = """[{"label":"Jan","value":120},{"label":"Feb","value":95},{"label":"Mar","value":148},{"label":"Apr","value":132},{"label":"May","value":176},{"label":"Jun","value":155}]"""
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "2.5rem") ] ] [
            div [] [
                sl "Default — weekly visitors"
                wc "fui-bar-chart" [ "data", weekData; "height", "200" ] []
            ]
            div [] [
                sl "With value labels"
                wc "fui-bar-chart" [ "data", weekData; "height", "200"; "show-values", "" ] []
            ]
            div [] [
                sl "Custom color — monthly sales"
                wc "fui-bar-chart" [ "data", salesData; "height", "180"; "color", "#3B82F6"; "show-values", "" ] []
            ]
        ] ]
    | "line-chart" ->
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        let perfData = """[{"label":"Jan","value":30},{"label":"Feb","value":45},{"label":"Mar","value":38},{"label":"Apr","value":62},{"label":"May","value":55},{"label":"Jun","value":78},{"label":"Jul","value":70},{"label":"Aug","value":85}]"""
        let errData  = """[{"label":"Mon","value":12},{"label":"Tue","value":8},{"label":"Wed","value":15},{"label":"Thu","value":6},{"label":"Fri","value":10},{"label":"Sat","value":3},{"label":"Sun","value":7}]"""
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "2.5rem") ] ] [
            div [] [
                sl "Line only — performance score"
                wc "fui-line-chart" [ "data", perfData; "height", "160" ] []
            ]
            div [] [
                sl "With area fill"
                wc "fui-line-chart" [ "data", perfData; "height", "160"; "fill", "" ] []
            ]
            div [] [
                sl "Error rate — no dots, danger color"
                wc "fui-line-chart" [ "data", errData; "height", "140"; "color", "#EF4444"; "fill", ""; "show-dots", "false" ] []
            ]
        ] ]
    | "pie-chart" ->
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        let fwData    = """[{"label":"React","value":42},{"label":"Vue","value":28},{"label":"Svelte","value":18},{"label":"Angular","value":12}]"""
        let budgetData = """[{"label":"Dev","value":45},{"label":"Design","value":25},{"label":"Marketing","value":20},{"label":"Ops","value":10}]"""
        [ div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "3rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
            div [] [
                sl "Pie chart"
                wc "fui-pie-chart" [ "data", fwData; "size", "200" ] []
            ]
            div [] [
                sl "Donut chart"
                wc "fui-pie-chart" [ "data", fwData; "size", "200"; "donut", "" ] []
            ]
            div [] [
                sl "Custom palette"
                wc "fui-pie-chart" [ "data", budgetData; "size", "200"; "donut", ""; "colors", """["#7C3AED","#3B82F6","#22C55E","#F59E0B"]""" ] []
            ]
        ] ]
    | "sparkline" ->
        let sl txt = p [ Style [ Margin "0 0 0.25rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        let rising  = """[12,18,15,28,22,35,30,42,38,55]"""
        let falling = """[80,72,68,55,60,45,48,35,28,18]"""
        let volatile' = """[40,65,30,72,25,80,20,85,45,60]"""
        let statRow label value trend color data =
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items", "center"); CSSProp.Custom("justify-content", "space-between"); Padding "0.875rem 1rem"; Background "#161618"; BorderRadius "8px"; Border "1px solid #2A2A2E" ] ] [
                div [] [
                    p [ Style [ Margin "0 0 0.125rem"; FontSize "0.75rem"; Color "#6E6E76"; FontFamily "'JetBrains Mono',monospace"; CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str label ]
                    p [ Style [ Margin "0"; FontSize "1.25rem"; FontWeight "700"; FontFamily "'JetBrains Mono',monospace"; Color "#E8E8ED" ] ] [ str value ]
                    p [ Style [ Margin "0.125rem 0 0"; FontSize "0.75rem"; Color color ] ] [ str trend ]
                ]
                wc "fui-sparkline" [ "data", data; "width", "80"; "height", "36"; "color", color ] []
            ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem"); MaxWidth "480px" ] ] [
            sl "Inline stat cards with sparklines"
            statRow "Revenue"     "$24,512"  "+12.4% this month"  "#22C55E" rising
            statRow "Errors"      "142"      "-8.3% this week"    "#EF4444" falling
            statRow "Throughput"  "8.9k rps" "volatile"           "#F59E0B" volatile'
        ] ]
    | _ -> []

let componentPage (cat: string) (slug: string) =
    match ComponentRegistry.bySlug slug with
    | None ->
        div [ ClassName "comp-page" ] [
            h1 [] [ str $"Component not found: {slug}" ]
        ]
    | Some meta ->
        let preview = componentLivePreview meta.Slug
        div [ ClassName "comp-page" ] [
            yield div [ ClassName "comp-header" ] [
                div [ ClassName "comp-header-row" ] [
                    h1 [ ClassName "comp-name" ] [ str meta.Name ]
                    span [ ClassName "comp-tag" ] [ str meta.Tag ]
                ]
                p [ ClassName "comp-desc" ] [ str meta.Description ]
            ]
            if not preview.IsEmpty then
                yield div [ ClassName "comp-section" ] [
                    h2 [ ClassName "comp-section-title" ] [ str "Preview" ]
                    div [ ClassName "comp-preview-box" ] [
                        div [ ClassName "preview-body" ] preview
                    ]
                ]
            if not meta.Attributes.IsEmpty then
                yield div [ ClassName "comp-section" ] [
                    h2 [ ClassName "comp-section-title" ] [ str "Attributes" ]
                    div [ ClassName "attr-table-wrap" ] [
                        table [ ClassName "attr-table" ] [
                            thead [] [ tr [] [
                                th [] [ str "Attribute" ]
                                th [] [ str "Type" ]
                                th [] [ str "Default" ]
                                th [] [ str "Description" ]
                            ]]
                            tbody [] (meta.Attributes |> List.map (fun a ->
                                tr [] [
                                    td [] [ code [] [ str a.Name ] ]
                                    td [] [ str a.Type ]
                                    td [] [ code [] [ str a.Default ] ]
                                    td [] [ str a.Description ]
                                ]))
                        ]
                    ]
                ]
            if not meta.CssProps.IsEmpty then
                yield div [ ClassName "comp-section" ] [
                    h2 [ ClassName "comp-section-title" ] [ str "CSS Custom Properties" ]
                    div [ ClassName "attr-table-wrap" ] [
                        table [ ClassName "attr-table" ] [
                            thead [] [ tr [] [
                                th [] [ str "Property" ]
                                th [] [ str "Description" ]
                            ]]
                            tbody [] (meta.CssProps |> List.map (fun p ->
                                tr [] [
                                    td [] [ code [] [ str p.Name ] ]
                                    td [] [ str p.Description ]
                                ]))
                        ]
                    ]
                ]
            if not meta.Events.IsEmpty then
                yield div [ ClassName "comp-section" ] [
                    h2 [ ClassName "comp-section-title" ] [ str "Events" ]
                    div [ ClassName "attr-table-wrap" ] [
                        table [ ClassName "attr-table" ] [
                            thead [] [ tr [] [
                                th [] [ str "Event" ]
                                th [] [ str "Description" ]
                            ]]
                            tbody [] (meta.Events |> List.map (fun e ->
                                tr [] [
                                    td [] [ code [] [ str e.Name ] ]
                                    td [] [ str e.Description ]
                                ]))
                        ]
                    ]
                ]
            yield div [ ClassName "comp-section" ] [
                h2 [ ClassName "comp-section-title" ] [ str "HTML Usage" ]
                pre [ ClassName "code-block" ] [ code [] [ str meta.HtmlUsage ] ]
            ]
        ]

// ── Getting Started page ──────────────────────────────────────────────────────

let gettingStartedPage =
    let h2Style  = Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "1.05rem"; FontWeight "600"; Color "#E8E8ED"; Margin "0 0 0.625rem"; PaddingBottom "0.625rem"; CSSProp.Custom("border-bottom", "1px solid #2A2A2E") ]
    let pStyle   = Style [ FontFamily "'Sora',sans-serif"; FontSize "0.9375rem"; Color "#6E6E76"; LineHeight "1.7"; Margin "0 0 1rem" ]
    let preStyle = Style [ Background "#0D0D0F"; Border "1px solid #2A2A2E"; BorderRadius "8px"; Padding "1.125rem 1.375rem"; CSSProp.Custom("overflow", "auto"); Margin "0.625rem 0 1.75rem"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.8125rem"; Color "#E8E8ED"; LineHeight "1.7" ]
    let section title children =
        div [ Style [ MarginBottom "2.5rem" ] ] (
            h2 [ h2Style ] [ str title ] :: children
        )
    let kw s  = span [ Style [ Color "#9B59F5" ] ] [ str s ]
    let cm s  = span [ Style [ Color "#4E5A65" ] ] [ str s ]
    let st s  = span [ Style [ Color "#22C55E" ] ] [ str s ]
    let at s  = span [ Style [ Color "#F59E0B" ] ] [ str s ]
    div [ ClassName "comp-page" ] [
        div [ ClassName "comp-header" ] [
            div [ ClassName "comp-header-row" ] [
                h1 [ ClassName "comp-name" ] [ str "Getting Started" ]
            ]
            p [ Style [ FontFamily "'Sora',sans-serif"; Color "#6E6E76"; Margin "0" ] ] [
                str "How to build, embed, and theme FableUI components in any web project."
            ]
        ]
        section "1 — Build the bundle" [
            p [ pStyle ] [ str "Run the production build from the repository root. Fable compiles F# to JavaScript; Vite bundles everything into a single ES module." ]
            pre [ preStyle ] [ code [] [
                cm "# compile + bundle\n"
                str "dotnet fable src/Client --run npm run build\n\n"
                cm "# output lands in deploy/public/assets/\n"
                str "ls deploy/public/assets/\n"
                cm "# → Client-[hash].js   ← the file you distribute"
            ] ]
        ]
        section "2 — Embed in any HTML page" [
            p [ pStyle ] [ str "Copy the built JS file to your project's static assets folder and add one script tag. The tag registers all custom elements with the browser." ]
            pre [ preStyle ] [ code [] [
                str "<"; kw "script"; at " type"; str "="; st "\"module\""; at " src"; str "="; st "\"/assets/Client.js\""; str "></"; kw "script"; str ">"
            ] ]
        ]
        section "3 — Use the elements" [
            p [ pStyle ] [ str "Custom elements behave like standard HTML elements — set attributes for static values, listen for custom events the same way you listen to clicks." ]
            pre [ preStyle ] [ code [] [
                cm "<!-- button with variant -->\n"
                str "<"; kw "fui-button"; at " variant"; str "="; st "\"primary\""; str ">Save changes</"; kw "fui-button"; str ">\n\n"
                cm "<!-- chart from a data attribute -->\n"
                str "<"; kw "fui-bar-chart\n"
                str "  "; at "data"; str "="; st "'[{\"label\":\"Mon\",\"value\":42},{\"label\":\"Tue\",\"value\":67}]'\n"
                str "  "; at "height"; str "="; st "\"200\""; str ">\n"
                str "</"; kw "fui-bar-chart"; str ">\n\n"
                cm "<!-- listen for custom events -->\n"
                str "<"; kw "script"; str ">\n"
                str "  document.querySelector("; st "'fui-button'"; str ")\n"
                str "    .addEventListener("; st "'fui-click'"; str ", e => console.log("; st "'clicked'"; str "));\n"
                str "</"; kw "script"; str ">"
            ] ]
        ]
        section "4 — Theme with CSS custom properties" [
            p [ pStyle ] [ str "Every component exposes design tokens via CSS custom properties. Override them at :root for global changes, or on a selector for local ones." ]
            pre [ preStyle ] [ code [] [
                cm "/* global accent colour override */\n"
                str ":root {\n"
                str "  "; at "--fui-btn-bg"; str ": "; st "#0EA5E9"; str ";\n"
                str "  "; at "--fui-btn-bg-hover"; str ": "; st "#38BDF8"; str ";\n"
                str "}\n\n"
                cm "/* scoped — only inside .dark-panel */\n"
                str ".dark-panel "; kw "fui-input"; str " {\n"
                str "  "; at "--fui-input-bg"; str ": "; st "#0A0A0C"; str ";\n"
                str "  "; at "--fui-input-border"; str ": "; st "#3A3A3E"; str ";\n"
                str "}"
            ] ]
        ]
        section "5 — Browser support" [
            p [ pStyle ] [
                str "Custom Elements v1 is supported in all modern browsers (Chrome 67+, Firefox 63+, Safari 10.3+, Edge 79+). "
                str "No polyfills are needed for any evergreen browser."
            ]
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.75rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
                for (browser, since) in [ "Chrome", "67+"; "Firefox", "63+"; "Safari", "10.3+"; "Edge", "79+" ] do
                    div [ Style [ Background "#161618"; Border "1px solid #2A2A2E"; BorderRadius "8px"; Padding "0.625rem 1rem"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.8125rem" ] ] [
                        span [ Style [ Color "#E8E8ED"; FontWeight "600" ] ] [ str browser ]
                        span [ Style [ Color "#22C55E"; MarginLeft "0.5rem" ] ] [ str since ]
                    ]
            ]
        ]
    ]

// ── Category index page ───────────────────────────────────────────────────────

let categoryIndexPage (catSlug: string) =
    let regCat     = urlCatToRegistry catSlug
    let components = ComponentRegistry.byCategory regCat
    let label      = urlCatLabel catSlug
    div [ ClassName "comp-page" ] [
        div [ ClassName "comp-header" ] [
            div [ ClassName "comp-header-row" ] [
                h1 [ ClassName "comp-name" ] [ str label ]
                span [ ClassName "comp-badge" ] [ str $"{components.Length} components" ]
            ]
            p [ Style [ FontFamily "'Sora',sans-serif"; Color "#6E6E76"; Margin "0" ] ] [
                str $"All {label} components — click any card to open the full demo and API reference."
            ]
        ]
        div [ Style [ Display DisplayOptions.Grid; CSSProp.Custom("grid-template-columns", "repeat(auto-fill,minmax(260px,1fr))"); CSSProp.Custom("gap", "1rem"); MarginTop "2rem" ] ] [
            for comp in components do
                a [
                    Href $"#/component/{catSlug}/{comp.Slug}"
                    Style [ Display DisplayOptions.Block; Background "#161618"; Border "1px solid #2A2A2E"; BorderRadius "10px"; Padding "1.25rem 1.375rem"; CSSProp.Custom("text-decoration", "none"); CSSProp.Custom("transition", "border-color 0.15s,background 0.15s") ]
                ] [
                    span [ Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.75rem"; Color "#7C3AED"; FontWeight "600"; CSSProp.Custom("letter-spacing", "0.03em") ] ] [ str comp.Tag ]
                    h3  [ Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.9375rem"; FontWeight "700"; Color "#E8E8ED"; Margin "0.375rem 0 0.5rem" ] ] [ str comp.Name ]
                    p   [ Style [ FontFamily "'Sora',sans-serif"; FontSize "0.8rem"; Color "#6E6E76"; Margin "0"; LineHeight "1.6"; CSSProp.Custom("display", "-webkit-box"); CSSProp.Custom("-webkit-line-clamp", "2"); CSSProp.Custom("-webkit-box-orient", "vertical"); CSSProp.Custom("overflow", "hidden") ] ] [ str comp.Description ]
                ]
        ]
    ]

// ── Backend demo pages ────────────────────────────────────────────────────────

type BackendDemoMeta = {
    Slug        : string
    Name        : string
    Description : string
    Method      : string
    Url         : string
    SourceCode  : string
}

let private backendDemos = [
    {
        Slug        = "health-check"
        Name        = "Health Check"
        Description = "Returns application status, timestamp, and uptime. Used by load balancers and uptime monitors to verify the service is alive."
        Method      = "GET"
        Url         = "/api/health"
        SourceCode  =
    """let healthHandler : HttpHandler =
        fun next ctx ->
            let resp = {|
                status      = "healthy"
                timestamp   = System.DateTime.UtcNow.ToString("o")
                uptime_ms   = System.Environment.TickCount64
                environment = System.Environment.GetEnvironmentVariable(
                                "ASPNETCORE_ENVIRONMENT")
                            |> Option.ofObj
                            |> Option.defaultValue "Production"
            |}
            json resp next ctx

    // Wired into the Saturn router:
    //   get Route.health healthHandler""" }

    {
        Slug        = "crud-api"
        Name        = "CRUD API"
        Description = "In-memory REST API demonstrating all four HTTP verbs. Try GET /api/items to list all records; use curl or any HTTP client to POST, PUT, and DELETE."
        Method      = "GET"
        Url         = "/api/items"
        SourceCode  =
    """// Demos/CrudApi.fs
    [<CLIMutable>]
    type Item = { Id: int; Title: string; Completed: bool }

    [<CLIMutable>]
    type ItemDto = { Title: string; Completed: bool }

    let private _data = ConcurrentDictionary<int, Item>()
    let mutable private _counter = 0
    let private nextId () = Interlocked.Increment(&_counter)

    // GET /api/items
    let getAll : HttpHandler = fun next ctx ->
        let items = _data.Values |> Seq.sortBy _.Id |> Seq.toArray
        json items next ctx

    // GET /api/items/:id
    let getById (id: int) : HttpHandler = fun next ctx ->
        match _data.TryGetValue id with
        | true, item -> json item next ctx
        | _ -> (setStatusCode 404 >=> json {| error = "Not found" |}) next ctx

    // POST /api/items   body: { "title": "...", "completed": false }
    let create : HttpHandler = fun next ctx -> task {
        let! dto = ctx.BindJsonAsync<ItemDto>()
        let item = { Id = nextId(); Title = dto.Title; Completed = dto.Completed }
        _data.[item.Id] <- item
        return! (setStatusCode 201 >=> json item) next ctx
    }

    // PUT /api/items/:id   body: { "title": "...", "completed": true }
    let update (id: int) : HttpHandler = fun next ctx -> task {
        let! dto = ctx.BindJsonAsync<ItemDto>()
        if _data.ContainsKey id then
            let item = { Id = id; Title = dto.Title; Completed = dto.Completed }
            _data.[id] <- item
            return! json item next ctx
        else
            return! (setStatusCode 404 >=> json {| error = "Not found" |}) next ctx
    }

    // DELETE /api/items/:id
    let delete (id: int) : HttpHandler = fun next ctx ->
        match _data.TryRemove id with
        | true, _ -> setStatusCode 204 next ctx
        | _ -> (setStatusCode 404 >=> json {| error = "Not found" |}) next ctx

    // Wired into Saturn router in Server.fs:
    //   get     Route.items    Demos.CrudApi.getAll
    //   post    Route.items    Demos.CrudApi.create
    //   getf    Route.itemById Demos.CrudApi.getById
    //   putf    Route.itemById Demos.CrudApi.update
    //   deletef Route.itemById Demos.CrudApi.delete""" }

    {
        Slug        = "error-handling"
        Name        = "Error Handling"
        Description = "Demonstrates Giraffe's idiomatic error response pattern — consistent JSON body with status, error, detail, timestamp, and path. GET /api/demo/error/500 returns a live 500 response."
        Method      = "GET"
        Url         = "/api/demo/error/500"
        SourceCode  =
    """// Server.fs — shared error handler
    let private errDemo (code: int) (title: string) (detail: string) : HttpHandler =
        fun next ctx ->
            let body = {|
                status    = code
                error     = title
                detail    = detail
                timestamp = System.DateTime.UtcNow.ToString("o")
                path      = ctx.Request.Path.Value
            |}
            (setStatusCode code >=> json body) next ctx

    // Wired into the Saturn router — all HTTP status codes:
    //   get Route.err400 (errDemo 400 "Bad Request"          "...")
    //   get Route.err401 (errDemo 401 "Unauthorized"         "...")
    //   get Route.err403 (errDemo 403 "Forbidden"            "...")
    //   get Route.err404 (errDemo 404 "Not Found"            "...")
    //   get Route.err422 (errDemo 422 "Unprocessable Entity" "...")
    //   get Route.err429 (errDemo 429 "Too Many Requests"    "...")
    //   get Route.err500 (errDemo 500 "Internal Server Error""...")

    // Try the other codes:
    //   /api/demo/error/400
    //   /api/demo/error/401
    //   /api/demo/error/403
    //   /api/demo/error/404
    //   /api/demo/error/422
    //   /api/demo/error/429""" }

    {
        Slug        = "pagination-api"
        Name        = "Pagination & Filtering"
        Description = "Slice any collection with page / pageSize query params. Filter by title substring, sort by any field in either direction — all server-side with zero client state leakage."
        Method      = "GET"
        Url         = "/api/items/paged"
        SourceCode  =
    """// GET /api/items/paged?page=1&pageSize=5&filter=&sortBy=id&sortDir=asc
    let getPage : HttpHandler = fun next ctx ->
        let q s = ctx.TryGetQueryStringValue s
        let page     = q "page"     |> Option.bind tryInt |> Option.defaultValue 1
        let pageSize = q "pageSize" |> Option.bind tryInt |> Option.defaultValue 10
        let filter   = q "filter"   |> Option.defaultValue ""
        let sortBy   = q "sortBy"   |> Option.defaultValue "id"
        let sortDir  = q "sortDir"  |> Option.defaultValue "asc"

        let filtered =
            _data.Values
            |> Seq.filter (fun i ->
                filter = "" ||
                i.Title.Contains(filter, StringComparison.OrdinalIgnoreCase))

        let sorted =
            match sortBy, sortDir with
            | "title",     "asc"  -> filtered |> Seq.sortBy (_.Title)
            | "title",     _      -> filtered |> Seq.sortByDescending (_.Title)
            | "completed", "asc"  -> filtered |> Seq.sortBy (_.Completed)
            | "completed", _      -> filtered |> Seq.sortByDescending (_.Completed)
            | _,           "asc"  -> filtered |> Seq.sortBy (_.Id)
            | _,           _      -> filtered |> Seq.sortByDescending (_.Id)

        let total      = sorted |> Seq.length
        let pg         = Math.Max(1, page)
        let ps         = Math.Clamp(pageSize, 1, 50)
        let totalPages = if total = 0 then 1 else (total + ps - 1) / ps
        let items      = sorted |> Seq.skip ((pg - 1) * ps)
                                |> Seq.truncate ps |> Seq.toArray

        json {| items      = items
                total      = total
                page       = pg
                pageSize   = ps
                totalPages = totalPages |} next ctx""" }

    {
        Slug        = "jwt-auth"
        Name        = "JWT Auth"
        Description = "Stateless authentication with HS256 JSON Web Tokens. POST credentials to receive a signed token, then attach it as a Bearer header to call the protected /me endpoint."
        Method      = "POST"
        Url         = "/api/demo/auth/login"
        SourceCode  =
    """// Demos/AuthDemo.fs
    let private secretBytes = Encoding.UTF8.GetBytes("...")

    let private makeJwt (username: string) (role: string) =
        let header   = "{\"alg\":\"HS256\",\"typ\":\"JWT\"}"
        let now      = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        let payload  =
            sprintf "{\"sub\":\"%s\",\"role\":\"%s\",\"iat\":%d,\"exp\":%d}"
                username role now (now + 3600L)
        let unsigned = b64urlStr header + "." + b64urlStr payload
        use hmac     = new HMACSHA256(secretBytes)
        unsigned + "." + b64url (hmac.ComputeHash(Encoding.UTF8.GetBytes(unsigned)))

    // POST /api/demo/auth/login   body: { "username": "admin", "password": "password123" }
    let login : HttpHandler = fun next ctx -> task {
        let! dto = ctx.BindJsonAsync<LoginDto>()
        match users |> Map.tryFind dto.Username with
        | Some (pw, role) when pw = dto.Password ->
            return! json {| token = makeJwt dto.Username role
                            tokenType = "Bearer"; expiresIn = 3600 |} next ctx
        | _ ->
            return! (setStatusCode 401 >=> json {| error = "Invalid credentials" |}) next ctx
    }

    // GET /api/demo/auth/me   requires Authorization: Bearer <token>
    let whoami : HttpHandler = fun next ctx ->
        let authHeader =
            match ctx.Request.Headers.TryGetValue("Authorization") with
            | true, v -> v.ToString()
            | _ -> ""
        if authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) then
            let token = authHeader.Substring("Bearer ".Length).Trim()
            match verifyJwt token with
            | Ok claims -> json {| username = claims.sub; role = claims.role |} next ctx
            | Error e   -> (setStatusCode 401 >=> json {| error = e |}) next ctx
        else
            (setStatusCode 401 >=> json {| error = "Missing Authorization header" |}) next ctx

    // Demo users: admin / password123   or   user / letmein""" }

    {
        Slug        = "rate-limiting"
        Name        = "Rate Limiting"
        Description = "Fixed-window limiter: 5 requests per 10-second window. Excess requests get HTTP 429 with a Retry-After header. Hit \"Hammer\" to saturate the limit instantly."
        Method      = "GET"
        Url         = "/api/demo/ratelimit/ping"
        SourceCode  =
    """// Demos/RateLimit.fs — no extra NuGet packages; pure .NET BCL

    type private Bucket(maxRequests: int, windowSeconds: int) =
        let mutable count     = 0
        let mutable windowEnd = DateTimeOffset.UtcNow.AddSeconds(float windowSeconds)

        member self.TryConsume() =
            lock self (fun () ->
                let now = DateTimeOffset.UtcNow
                if now >= windowEnd then
                    count     <- 1
                    windowEnd <- now.AddSeconds(float windowSeconds)
                    true
                elif count < maxRequests then
                    count <- count + 1
                    true
                else false)

        member self.RetryAfterSeconds() =
            lock self (fun () ->
                (windowEnd - DateTimeOffset.UtcNow).TotalSeconds |> max 0.0 |> int)

    let private bucket = Bucket(5, 10)   // 5 req / 10-second window (global)

    // GET /api/demo/ratelimit/ping
    let ping : HttpHandler = fun next ctx ->
        if bucket.TryConsume() then
            json {| status = "ok"; timestamp = DateTime.UtcNow.ToString("o") |} next ctx
        else
            let retryAfter = bucket.RetryAfterSeconds()
            (setStatusCode 429
             >=> setHttpHeader "Retry-After" (string retryAfter)
             >=> json {| error = "Too Many Requests"; retryAfter = retryAfter |})
                next ctx

    // Wired in Server.fs:
    //   get Route.rateLimit Demos.RateLimit.ping""" }

    {
        Slug        = "background-jobs"
        Name        = "Background Jobs"
        Description = "Fire-and-forget processing: POST to enqueue a job, then poll GET /api/demo/jobs/{id} for live status. The server runs 5 timed steps (~4 s total) and tracks progress in memory."
        Method      = "POST"
        Url         = "/api/demo/jobs"
        SourceCode  =
    """// Demos/BackgroundJob.fs
    type private Job = {
        Id              : string
        mutable Status  : string   // queued | running | completed | failed
        mutable Progress: int
        mutable Result  : string
        CreatedAt       : DateTime
        mutable Done    : DateTime option }

    let private jobs = ConcurrentDictionary<string, Job>()

    let private runInBackground (id: string) =
        Task.Run(fun () ->
            Thread.Sleep(300)
            jobs.[id].Status <- "running"
            for step in 1..5 do
                Thread.Sleep(800)
                jobs.[id].Progress <- step * 20
            jobs.[id].Status <- "completed"
            jobs.[id].Result <- "Processed 5 steps"
            jobs.[id].Done   <- Some DateTime.UtcNow) |> ignore

    // POST /api/demo/jobs  — enqueue a job, returns 202 + { id }
    let start : HttpHandler = fun next ctx -> task {
        let id = Guid.NewGuid().ToString("N").[..7]
        jobs.[id] <- { Id=id; Status="queued"; Progress=0; Result=""; CreatedAt=DateTime.UtcNow; Done=None }
        runInBackground id
        return! (setStatusCode 202 >=> json {| id=id; status="queued" |}) next ctx }

    // GET /api/demo/jobs/{id}  — poll for status
    let getStatus (id: string) : HttpHandler = fun next ctx ->
        match jobs.TryGetValue(id) with
        | true, job -> json (toDto job) next ctx
        | _ -> (setStatusCode 404 >=> json {| error="Not found" |}) next ctx

    // Wired in Server.fs:
    //   post   Route.jobsBase           Demos.BackgroundJob.start
    //   getf   "/api/demo/jobs/%s"      Demos.BackgroundJob.getStatus
    //   get    Route.jobsBase           Demos.BackgroundJob.getAll""" }

    {
        Slug        = "websocket"
        Name        = "WebSocket Echo"
        Description = "Full-duplex messaging over a persistent TCP connection. Connect, send any text — the server echoes it back with a counter, length, and timestamp. No polling required."
        Method      = "WS"
        Url         = "/api/demo/ws/echo"
        SourceCode  =
    """// Demos/WebSocketDemo.fs
    let private echoLoop (ws: WebSocket) = task {
        let buf = Array.zeroCreate<byte> 4096
        let mutable count = 0
        while ws.State = WebSocketState.Open do
            let! result = ws.ReceiveAsync(ArraySegment(buf), CancellationToken.None)
            match result.MessageType with
            | WebSocketMessageType.Close ->
                do! ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", CancellationToken.None)
            | WebSocketMessageType.Text ->
                let text = Encoding.UTF8.GetString(buf, 0, result.Count)
                count <- count + 1
                let reply =
                    sprintf "{\"echo\":\"%s\",\"count\":%d,\"length\":%d,\"timestamp\":\"%s\"}"
                        text count text.Length (DateTime.UtcNow.ToString("o"))
                let replyBytes = Encoding.UTF8.GetBytes(reply)
                do! ws.SendAsync(ArraySegment(replyBytes), WebSocketMessageType.Text, true, CancellationToken.None)
            | _ -> () }

    // GET /api/demo/ws/echo  — clients must send a WebSocket upgrade request
    let handler : HttpHandler = fun next ctx -> task {
        if ctx.WebSockets.IsWebSocketRequest then
            let! ws = ctx.WebSockets.AcceptWebSocketAsync()
            do! echoLoop ws
            return Some ctx
        else
            return! (setStatusCode 426 >=> setHttpHeader "Upgrade" "websocket"
                     >=> text "WebSocket upgrade required") next ctx }

    // In Server.fs application builder:
    //   app_config (fun app -> app.UseWebSockets())
    // In the router:
    //   get Route.wsEcho Demos.WebSocketDemo.handler""" }

    {
        Slug        = "file-upload"
        Name        = "File Upload"
        Description = "Receive files as multipart/form-data. The server reads each file's stream, returns metadata (name, size, content-type) and a 128-byte preview — hex for binary, UTF-8 text for text files."
        Method      = "POST"
        Url         = "/api/demo/upload"
        SourceCode  =
    """// Demos/FileUpload.fs
    let upload : HttpHandler = fun next ctx -> task {
        if not ctx.Request.HasFormContentType then
            return! (setStatusCode 400 >=> json {| error = "Expected multipart/form-data" |}) next ctx
        else
            let files = ctx.Request.Form.Files
            if files.Count = 0 then
                return! (setStatusCode 400 >=> json {| error = "No file in request" |}) next ctx
            else
                let results =
                    [| for file in files do
                        use stream     = file.OpenReadStream()
                        let previewLen = int (min file.Length 128L)
                        let buf        = Array.zeroCreate<byte> previewLen
                        stream.Read(buf, 0, previewLen) |> ignore
                        let isText = file.ContentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase)
                        yield {|
                            name        = file.FileName
                            size        = file.Length
                            contentType = file.ContentType
                            previewType = if isText then "text" else "hex"
                            preview     =
                                if isText then Encoding.UTF8.GetString(buf)
                                else buf |> Array.map (sprintf "%02x") |> String.concat " "
                        |} |]
                return! (setStatusCode 200 >=> json results) next ctx }

    // Wired in Server.fs:
    //   post Route.fileUpload Demos.FileUpload.upload""" }

    {
        Slug        = "sse"
        Name        = "Server-Sent Events"
        Description = "One-way push stream over plain HTTP. The server sends 10 events (one per second) then closes the connection. The browser's EventSource API reconnects automatically — we close it on completion."
        Method      = "GET"
        Url         = "/api/demo/sse/events"
        SourceCode  =
    """// Demos/SseDemo.fs  — no special middleware required
    let stream : HttpHandler = fun next ctx -> task {
        ctx.Response.ContentType <- "text/event-stream; charset=utf-8"
        ctx.Response.Headers.Append("Cache-Control", "no-cache")
        ctx.Response.Headers.Append("X-Accel-Buffering", "no")

        try
            for i in 1..10 do
                let payload =
                    sprintf "{\"count\":%d,\"value\":%d,\"timestamp\":\"%s\"}"
                        i (rng.Next(1, 100)) (DateTime.UtcNow.ToString("o"))
                let bytes = Encoding.UTF8.GetBytes("data: " + payload + "\n\n")
                do! ctx.Response.Body.WriteAsync(bytes, 0, bytes.Length)
                do! ctx.Response.Body.FlushAsync()
                if i < 10 then do! Task.Delay(1000)
        with _ -> ()   // client disconnected mid-stream

        return Some ctx }

    // SSE wire format — each event is:
    //   data: <json>\n\n
    // Two newlines signal end of event to the browser.
    //
    // Wired in Server.fs:
    //   get Route.sseStream Demos.SseDemo.stream""" }
]

let private crudApiPage (demo: BackendDemoMeta) (model: Model) (dispatch: Msg -> unit) =
    let f = model.CrudForm
    let state key = model.BackendResults |> Map.tryFind key |> Option.defaultValue Idle
    let loading key = state key = Fetching
    let statusVariant code = if code >= 200 && code < 300 then "success" elif code >= 500 then "danger" elif code >= 400 then "warning" else "info"

    let labelStyle    = Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.65rem"; FontWeight "700"; CSSProp.Custom("text-transform","uppercase"); CSSProp.Custom("letter-spacing","0.08em"); Color "#6E6E76"; MarginBottom "0.5rem" ]
    let codePreStyle  = Style [ Margin "0"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.8rem"; Color "#E8E8ED"; LineHeight "1.7"; CSSProp.Custom("white-space","pre") ]
    let panelStyle    = Style [ Background "#0D0D0F"; Border "1px solid #2A2A2E"; BorderRadius "8px"; Padding "1.125rem 1.375rem"; CSSProp.Custom("overflow","auto") ]
    let cardStyle     = Style [ Background "#161618"; Border "1px solid #2A2A2E"; BorderRadius "10px"; Padding "1.25rem 1.5rem"; MarginBottom "1.25rem" ]
    let inputStyle    = Style [ Background "#0D0D0F"; Border "1px solid #2A2A2E"; BorderRadius "6px"; Color "#E8E8ED"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.875rem"; Padding "0.5rem 0.75rem"; Width "100%"; Outline "none"; CSSProp.Custom("box-sizing","border-box") ]
    let fieldLbl      = Style [ Display DisplayOptions.Block; FontFamily "'JetBrains Mono',monospace"; FontSize "0.72rem"; Color "#6E6E76"; MarginBottom "0.35rem" ]
    let checkboxRowSt = Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("gap","0.5rem"); FontFamily "'JetBrains Mono',monospace"; FontSize "0.8rem"; Color "#E8E8ED"; Cursor "pointer" ]

    let responsePanel key =
        div [ Style [ MarginTop "1rem" ] ] [
            p [ labelStyle ] [ str "Response" ]
            match state key with
            | Idle ->
                wc "fui-empty-state" [ "title", "No response yet"; "description", "Press Send to call the endpoint." ] []
            | Fetching ->
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("justify-content","center"); Padding "1.5rem" ] ] [
                    wc "fui-spinner" [ "label", "Sending…" ] []
                ]
            | Done(status, body) ->
                div [ panelStyle ] [
                    div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("gap","0.75rem"); MarginBottom "0.875rem" ] ] [
                        wc "fui-badge" [ "variant", statusVariant status ] [ str $"HTTP {status}" ]
                        if body <> "" then wc "fui-badge" [] [ str "application/json" ]
                    ]
                    if body <> "" then pre [ codePreStyle ] [ str body ]
                ]
            | Failed err ->
                wc "fui-alert" [ "variant", "danger"; "title", "Network error" ] [ str err ]
        ]

    let methodRow (method: string) (url: string) =
        div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("gap","0.5rem"); MarginBottom "0.875rem" ] ] [
            let v = match method with "GET" -> "success" | "POST" -> "info" | "DELETE" -> "danger" | _ -> "warning"
            wc "fui-badge" [ "variant", v ] [ str method ]
            code [ Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.85rem"; Color "#E8E8ED" ] ] [ str url ]
        ]

    div [ ClassName "comp-page" ] [
        div [ ClassName "comp-header" ] [
            div [ ClassName "comp-header-row" ] [
                h1 [ ClassName "comp-name" ] [ str demo.Name ]
                wc "fui-badge" [ "variant", "accent" ] [ str "Backend" ]
            ]
            p [ Style [ FontFamily "'Sora',sans-serif"; Color "#6E6E76"; Margin "0" ] ] [ str demo.Description ]
        ]

        // ── GET all ───────────────────────────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str "List All Items" ]
            methodRow "GET" "/api/items"
            domEl "fui-button" [
                HTMLAttr.Custom("variant", box "primary")
                HTMLAttr.Custom("size", box "sm")
                if loading "crud-get-all" then HTMLAttr.Custom("disabled", box "")
                OnClick (fun _ -> dispatch (BackendFetch("crud-get-all", "/api/items")))
            ] [ str (if loading "crud-get-all" then "Sending…" else "Send Request") ]
            responsePanel "crud-get-all"
        ]

        // ── GET by ID ─────────────────────────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str "Get Item by ID" ]
            methodRow "GET" "/api/items/:id"
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","flex-end"); CSSProp.Custom("gap","0.75rem"); CSSProp.Custom("flex-wrap","wrap") ] ] [
                div [ Style [ CSSProp.Custom("flex","0 0 110px") ] ] [
                    label [ fieldLbl ] [ str "ID" ]
                    domEl "input" [
                        HTMLAttr.Type "number"; HTMLAttr.Min "1"
                        HTMLAttr.Value f.GetId
                        inputStyle
                        OnChange (fun e -> dispatch (CrudFormStr("get-id", (e.target :?> Browser.Types.HTMLInputElement).value)))
                    ] []
                ]
                domEl "fui-button" [
                    HTMLAttr.Custom("variant", box "primary")
                    HTMLAttr.Custom("size", box "sm")
                    if loading "crud-get-id" then HTMLAttr.Custom("disabled", box "")
                    OnClick (fun _ -> dispatch (BackendFetch("crud-get-id", $"/api/items/{f.GetId}")))
                ] [ str (if loading "crud-get-id" then "Sending…" else "Send Request") ]
            ]
            responsePanel "crud-get-id"
        ]

        // ── POST create ───────────────────────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str "Create Item" ]
            methodRow "POST" "/api/items"
            div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap","0.75rem") ] ] [
                div [] [
                    label [ fieldLbl ] [ str "Title" ]
                    domEl "input" [
                        HTMLAttr.Type "text"; HTMLAttr.Placeholder "e.g. Write unit tests"
                        HTMLAttr.Value f.CreateTitle
                        inputStyle
                        OnChange (fun e -> dispatch (CrudFormStr("create-title", (e.target :?> Browser.Types.HTMLInputElement).value)))
                    ] []
                ]
                label [ checkboxRowSt ] [
                    domEl "input" [
                        HTMLAttr.Type "checkbox"
                        HTMLAttr.Checked f.CreateCompleted
                        OnChange (fun e -> dispatch (CrudFormBool("create-completed", (e.target :?> Browser.Types.HTMLInputElement).``checked``)))
                    ] []
                    str "Completed"
                ]
                domEl "fui-button" [
                    HTMLAttr.Custom("variant", box "primary")
                    HTMLAttr.Custom("size", box "sm")
                    if loading "crud-create" || f.CreateTitle.Trim() = "" then HTMLAttr.Custom("disabled", box "")
                    OnClick (fun _ -> dispatch (CrudSend("crud-create", "POST", "/api/items", itemJson f.CreateTitle f.CreateCompleted)))
                ] [ str (if loading "crud-create" then "Sending…" else "Create Item") ]
            ]
            responsePanel "crud-create"
        ]

        // ── PUT update ────────────────────────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str "Update Item" ]
            methodRow "PUT" "/api/items/:id"
            div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap","0.75rem") ] ] [
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap","0.75rem"); CSSProp.Custom("flex-wrap","wrap") ] ] [
                    div [ Style [ CSSProp.Custom("flex","0 0 110px") ] ] [
                        label [ fieldLbl ] [ str "ID" ]
                        domEl "input" [
                            HTMLAttr.Type "number"; HTMLAttr.Min "1"
                            HTMLAttr.Value f.UpdateId
                            inputStyle
                            OnChange (fun e -> dispatch (CrudFormStr("update-id", (e.target :?> Browser.Types.HTMLInputElement).value)))
                        ] []
                    ]
                    div [ Style [ CSSProp.Custom("flex","1"); CSSProp.Custom("min-width","160px") ] ] [
                        label [ fieldLbl ] [ str "Title" ]
                        domEl "input" [
                            HTMLAttr.Type "text"; HTMLAttr.Placeholder "Updated title"
                            HTMLAttr.Value f.UpdateTitle
                            inputStyle
                            OnChange (fun e -> dispatch (CrudFormStr("update-title", (e.target :?> Browser.Types.HTMLInputElement).value)))
                        ] []
                    ]
                ]
                label [ checkboxRowSt ] [
                    domEl "input" [
                        HTMLAttr.Type "checkbox"
                        HTMLAttr.Checked f.UpdateCompleted
                        OnChange (fun e -> dispatch (CrudFormBool("update-completed", (e.target :?> Browser.Types.HTMLInputElement).``checked``)))
                    ] []
                    str "Completed"
                ]
                domEl "fui-button" [
                    HTMLAttr.Custom("variant", box "primary")
                    HTMLAttr.Custom("size", box "sm")
                    if loading "crud-update" || f.UpdateTitle.Trim() = "" then HTMLAttr.Custom("disabled", box "")
                    OnClick (fun _ -> dispatch (CrudSend("crud-update", "PUT", $"/api/items/{f.UpdateId}", itemJson f.UpdateTitle f.UpdateCompleted)))
                ] [ str (if loading "crud-update" then "Sending…" else "Update Item") ]
            ]
            responsePanel "crud-update"
        ]

        // ── DELETE ────────────────────────────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str "Delete Item" ]
            methodRow "DELETE" "/api/items/:id"
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","flex-end"); CSSProp.Custom("gap","0.75rem"); CSSProp.Custom("flex-wrap","wrap") ] ] [
                div [ Style [ CSSProp.Custom("flex","0 0 110px") ] ] [
                    label [ fieldLbl ] [ str "ID" ]
                    domEl "input" [
                        HTMLAttr.Type "number"; HTMLAttr.Min "1"
                        HTMLAttr.Value f.DeleteId
                        inputStyle
                        OnChange (fun e -> dispatch (CrudFormStr("delete-id", (e.target :?> Browser.Types.HTMLInputElement).value)))
                    ] []
                ]
                domEl "fui-button" [
                    HTMLAttr.Custom("variant", box "danger")
                    HTMLAttr.Custom("size", box "sm")
                    if loading "crud-delete" then HTMLAttr.Custom("disabled", box "")
                    OnClick (fun _ -> dispatch (CrudSend("crud-delete", "DELETE", $"/api/items/{f.DeleteId}", "")))
                ] [ str (if loading "crud-delete" then "Sending…" else "Delete Item") ]
            ]
            responsePanel "crud-delete"
        ]

        // ── Source code ───────────────────────────────────────────────────────
        div [] [
            p [ labelStyle ] [ str "Server source (F#)" ]
            wc "fui-code-block" [ "language", "fsharp"; "code", demo.SourceCode ] []
        ]
    ]

let private pagApiPage (demo: BackendDemoMeta) (model: Model) (dispatch: Msg -> unit) =
    let pag = model.PagForm
    let state   = model.BackendResults |> Map.tryFind "pag-result" |> Option.defaultValue Idle
    let loading = state = Fetching
    let statusVariant code = if code >= 200 && code < 300 then "success" elif code >= 500 then "danger" elif code >= 400 then "warning" else "info"

    let labelStyle   = Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.65rem"; FontWeight "700"; CSSProp.Custom("text-transform","uppercase"); CSSProp.Custom("letter-spacing","0.08em"); Color "#6E6E76"; MarginBottom "0.5rem" ]
    let codePreStyle = Style [ Margin "0"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.8rem"; Color "#E8E8ED"; LineHeight "1.7"; CSSProp.Custom("white-space","pre") ]
    let panelStyle   = Style [ Background "#0D0D0F"; Border "1px solid #2A2A2E"; BorderRadius "8px"; Padding "1.125rem 1.375rem"; CSSProp.Custom("overflow","auto") ]
    let cardStyle    = Style [ Background "#161618"; Border "1px solid #2A2A2E"; BorderRadius "10px"; Padding "1.25rem 1.5rem"; MarginBottom "1.25rem" ]
    let inputStyle   = Style [ Background "#0D0D0F"; Border "1px solid #2A2A2E"; BorderRadius "6px"; Color "#E8E8ED"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.875rem"; Padding "0.5rem 0.75rem"; Outline "none"; CSSProp.Custom("box-sizing","border-box") ]
    let fieldLbl     = Style [ Display DisplayOptions.Block; FontFamily "'JetBrains Mono',monospace"; FontSize "0.72rem"; Color "#6E6E76"; MarginBottom "0.35rem" ]

    let selectStyle =
        Style [ Background "#0D0D0F"; Border "1px solid #2A2A2E"; BorderRadius "6px"; Color "#E8E8ED"
                FontFamily "'JetBrains Mono',monospace"; FontSize "0.8rem"; Padding "0.5rem 0.6rem"
                Outline "none"; Cursor "pointer" ]

    div [ ClassName "comp-page" ] [
        div [ ClassName "comp-header" ] [
            div [ ClassName "comp-header-row" ] [
                h1 [ ClassName "comp-name" ] [ str demo.Name ]
                wc "fui-badge" [ "variant", "accent" ] [ str "Backend" ]
            ]
            p [ Style [ FontFamily "'Sora',sans-serif"; Color "#6E6E76"; Margin "0" ] ] [ str demo.Description ]
        ]

        // ── Query controls ────────────────────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str "Query Parameters" ]
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap","1rem"); CSSProp.Custom("flex-wrap","wrap"); CSSProp.Custom("align-items","flex-end") ] ] [
                // Filter
                div [] [
                    label [ fieldLbl ] [ str "Filter (title contains)" ]
                    domEl "input" [
                        HTMLAttr.Type "text"; HTMLAttr.Placeholder "e.g. test"
                        HTMLAttr.Value pag.Filter
                        Style [ Background "#0D0D0F"; Border "1px solid #2A2A2E"; BorderRadius "6px"; Color "#E8E8ED"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.875rem"; Padding "0.5rem 0.75rem"; Outline "none"; CSSProp.Custom("box-sizing","border-box"); CSSProp.Custom("min-width","180px") ]
                        OnChange (fun e -> dispatch (PagFormStr("filter", (e.target :?> Browser.Types.HTMLInputElement).value)))
                    ] []
                ]
                // Sort by
                div [] [
                    label [ fieldLbl ] [ str "Sort by" ]
                    domEl "select" [
                        selectStyle
                        HTMLAttr.Value pag.SortBy
                        OnChange (fun e -> dispatch (PagFormStr("sort-by", (e.target :?> Browser.Types.HTMLInputElement).value)))
                    ] [
                        domEl "option" [ HTMLAttr.Value "id"        ] [ str "ID" ]
                        domEl "option" [ HTMLAttr.Value "title"     ] [ str "Title" ]
                        domEl "option" [ HTMLAttr.Value "completed" ] [ str "Completed" ]
                    ]
                ]
                // Sort direction
                div [] [
                    label [ fieldLbl ] [ str "Direction" ]
                    domEl "select" [
                        selectStyle
                        HTMLAttr.Value pag.SortDir
                        OnChange (fun e -> dispatch (PagFormStr("sort-dir", (e.target :?> Browser.Types.HTMLInputElement).value)))
                    ] [
                        domEl "option" [ HTMLAttr.Value "asc"  ] [ str "Ascending" ]
                        domEl "option" [ HTMLAttr.Value "desc" ] [ str "Descending" ]
                    ]
                ]
                // Page size
                div [] [
                    label [ fieldLbl ] [ str "Page size" ]
                    domEl "select" [
                        selectStyle
                        HTMLAttr.Value pag.PageSize
                        OnChange (fun e -> dispatch (PagFormStr("page-size", (e.target :?> Browser.Types.HTMLInputElement).value)))
                    ] [
                        domEl "option" [ HTMLAttr.Value "3"  ] [ str "3" ]
                        domEl "option" [ HTMLAttr.Value "5"  ] [ str "5" ]
                        domEl "option" [ HTMLAttr.Value "10" ] [ str "10" ]
                    ]
                ]
                // Fetch button
                domEl "fui-button" [
                    HTMLAttr.Custom("variant", box "primary")
                    HTMLAttr.Custom("size", box "sm")
                    if loading then HTMLAttr.Custom("disabled", box "")
                    OnClick (fun _ -> dispatch (PagFetch 1))
                ] [ str (if loading then "Fetching…" else "Fetch Page 1") ]
            ]
        ]

        // ── Results ───────────────────────────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str "Response" ]
            match state with
            | Idle ->
                wc "fui-empty-state" [ "title", "No results yet"; "description", "Set your query params and press Fetch." ] []
            | Fetching ->
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("justify-content","center"); Padding "2rem" ] ] [
                    wc "fui-spinner" [ "label", "Fetching…" ] []
                ]
            | Failed err ->
                wc "fui-alert" [ "variant", "danger"; "title", "Network error" ] [ str err ]
            | Done(status, body) ->
                let meta = parsePagMeta body
                div [] [
                    // Meta row
                    div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("gap","0.75rem"); MarginBottom "0.875rem"; CSSProp.Custom("flex-wrap","wrap") ] ] [
                        wc "fui-badge" [ "variant", statusVariant status ] [ str $"HTTP {status}" ]
                        wc "fui-badge" [] [ str $"Total: {meta.total}" ]
                        wc "fui-badge" [] [ str $"Page {meta.page} of {meta.totalPages}" ]
                    ]
                    // JSON body
                    div [ panelStyle ] [
                        pre [ codePreStyle ] [ str body ]
                    ]
                    // Pagination nav
                    div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("gap","0.5rem"); MarginTop "1rem"; CSSProp.Custom("flex-wrap","wrap") ] ] [
                        yield domEl "fui-button" [
                            HTMLAttr.Custom("variant", box "secondary")
                            HTMLAttr.Custom("size", box "sm")
                            if meta.page <= 1 then HTMLAttr.Custom("disabled", box "")
                            OnClick (fun _ -> dispatch (PagFetch 1))
                        ] [ str "« First" ]
                        yield domEl "fui-button" [
                            HTMLAttr.Custom("variant", box "secondary")
                            HTMLAttr.Custom("size", box "sm")
                            if meta.page <= 1 then HTMLAttr.Custom("disabled", box "")
                            OnClick (fun _ -> dispatch (PagFetch (meta.page - 1)))
                        ] [ str "‹ Prev" ]
                        // Page number buttons — show up to 5 around current
                        let lo = System.Math.Max(1, meta.page - 2)
                        let hi = System.Math.Min(meta.totalPages, meta.page + 2)
                        for pg in lo..hi do
                            yield domEl "fui-button" [
                                HTMLAttr.Custom("variant", box (if pg = meta.page then "primary" else "secondary"))
                                HTMLAttr.Custom("size", box "sm")
                                OnClick (fun _ -> dispatch (PagFetch pg))
                            ] [ str (string pg) ]
                        yield domEl "fui-button" [
                            HTMLAttr.Custom("variant", box "secondary")
                            HTMLAttr.Custom("size", box "sm")
                            if meta.page >= meta.totalPages then HTMLAttr.Custom("disabled", box "")
                            OnClick (fun _ -> dispatch (PagFetch (meta.page + 1)))
                        ] [ str "Next ›" ]
                        yield domEl "fui-button" [
                            HTMLAttr.Custom("variant", box "secondary")
                            HTMLAttr.Custom("size", box "sm")
                            if meta.page >= meta.totalPages then HTMLAttr.Custom("disabled", box "")
                            OnClick (fun _ -> dispatch (PagFetch meta.totalPages))
                        ] [ str "Last »" ]
                    ]
                ]
        ]

        // ── Source code ───────────────────────────────────────────────────────
        div [] [
            p [ labelStyle ] [ str "Server source (F#)" ]
            wc "fui-code-block" [ "language", "fsharp"; "code", demo.SourceCode ] []
        ]
    ]

let private jwtAuthPage (demo: BackendDemoMeta) (model: Model) (dispatch: Msg -> unit) =
    let f              = model.AuthForm
    let loginState     = model.BackendResults |> Map.tryFind "auth-login" |> Option.defaultValue Idle
    let meState        = model.BackendResults |> Map.tryFind "auth-me"    |> Option.defaultValue Idle
    let loginLoading   = loginState = Fetching
    let meLoading      = meState    = Fetching
    let hasToken       = f.Token.Length > 0
    let statusVariant code = if code >= 200 && code < 300 then "success" elif code >= 500 then "danger" elif code >= 400 then "warning" else "info"

    let labelStyle   = Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.65rem"; FontWeight "700"; CSSProp.Custom("text-transform","uppercase"); CSSProp.Custom("letter-spacing","0.08em"); Color "#6E6E76"; MarginBottom "0.5rem" ]
    let codePreStyle = Style [ Margin "0"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.8rem"; Color "#E8E8ED"; LineHeight "1.7"; CSSProp.Custom("white-space","pre") ]
    let panelStyle   = Style [ Background "#0D0D0F"; Border "1px solid #2A2A2E"; BorderRadius "8px"; Padding "1.125rem 1.375rem"; CSSProp.Custom("overflow","auto") ]
    let cardStyle    = Style [ Background "#161618"; Border "1px solid #2A2A2E"; BorderRadius "10px"; Padding "1.25rem 1.5rem"; MarginBottom "1.25rem" ]
    let inputStyle   = Style [ Background "#0D0D0F"; Border "1px solid #2A2A2E"; BorderRadius "6px"; Color "#E8E8ED"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.875rem"; Padding "0.5rem 0.75rem"; Outline "none"; CSSProp.Custom("box-sizing","border-box"); Width "100%" ]
    let fieldLbl     = Style [ Display DisplayOptions.Block; FontFamily "'JetBrains Mono',monospace"; FontSize "0.72rem"; Color "#6E6E76"; MarginBottom "0.35rem" ]

    let responsePanel (state: FetchState) =
        match state with
        | Idle ->
            wc "fui-empty-state" [ "title", "No response yet"; "description", "Send the request to see the result." ] []
        | Fetching ->
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("justify-content","center"); Padding "1.5rem" ] ] [
                wc "fui-spinner" [ "label", "Waiting…" ] []
            ]
        | Failed err ->
            wc "fui-alert" [ "variant", "danger"; "title", "Network error" ] [ str err ]
        | Done(status, body) ->
            div [] [
                div [ Style [ MarginBottom "0.75rem" ] ] [
                    wc "fui-badge" [ "variant", statusVariant status ] [ str $"HTTP {status}" ]
                ]
                div [ panelStyle ] [ pre [ codePreStyle ] [ str body ] ]
            ]

    div [ ClassName "comp-page" ] [
        div [ ClassName "comp-header" ] [
            div [ ClassName "comp-header-row" ] [
                h1 [ ClassName "comp-name" ] [ str demo.Name ]
                wc "fui-badge" [ "variant", "accent" ] [ str "Backend" ]
            ]
            p [ Style [ FontFamily "'Sora',sans-serif"; Color "#6E6E76"; Margin "0" ] ] [ str demo.Description ]
        ]

        // ── Step 1: Login ──────────────────────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str "Step 1 — POST /api/demo/auth/login" ]
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap","1rem"); CSSProp.Custom("flex-wrap","wrap"); CSSProp.Custom("align-items","flex-end"); MarginBottom "1rem" ] ] [
                div [ Style [ CSSProp.Custom("flex","1 1 180px") ] ] [
                    label [ fieldLbl ] [ str "Username" ]
                    domEl "input" [
                        HTMLAttr.Type "text"; HTMLAttr.Value f.Username
                        inputStyle
                        OnChange (fun e -> dispatch (AuthFormStr("username", (e.target :?> Browser.Types.HTMLInputElement).value)))
                    ] []
                ]
                div [ Style [ CSSProp.Custom("flex","1 1 180px") ] ] [
                    label [ fieldLbl ] [ str "Password" ]
                    domEl "input" [
                        HTMLAttr.Type "password"; HTMLAttr.Value f.Password
                        inputStyle
                        OnChange (fun e -> dispatch (AuthFormStr("password", (e.target :?> Browser.Types.HTMLInputElement).value)))
                    ] []
                ]
                domEl "fui-button" [
                    HTMLAttr.Custom("variant", box "primary")
                    HTMLAttr.Custom("size", box "sm")
                    if loginLoading then HTMLAttr.Custom("disabled", box "")
                    OnClick (fun _ -> dispatch AuthLogin)
                ] [ str (if loginLoading then "Logging in…" else "Login") ]
            ]
            div [ Style [ MarginBottom "0.5rem" ] ] [
                span [ Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.72rem"; Color "#6E6E76" ] ] [
                    str "Demo users: "
                ]
                wc "fui-badge" [] [ str "admin / password123" ]
                span [ Style [ Margin "0 0.4rem"; Color "#6E6E76" ] ] [ str "or" ]
                wc "fui-badge" [] [ str "user / letmein" ]
            ]
            responsePanel loginState
        ]

        // ── Step 2: Call protected endpoint ────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str "Step 2 — GET /api/demo/auth/me  (requires Bearer token)" ]
            // Token display
            div [ Style [ Background "#0D0D0F"; Border "1px solid #2A2A2E"; BorderRadius "6px"; Padding "0.625rem 0.875rem"; MarginBottom "1rem"; CSSProp.Custom("overflow","auto") ] ] [
                if hasToken then
                    span [ Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.75rem"; Color "#A0E0A0"; CSSProp.Custom("word-break","break-all") ] ] [
                        str $"Bearer {f.Token}"
                    ]
                else
                    span [ Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.75rem"; Color "#6E6E76"; FontStyle "italic" ] ] [
                        str "No token yet — complete Step 1 first"
                    ]
            ]
            domEl "fui-button" [
                HTMLAttr.Custom("variant", box "primary")
                HTMLAttr.Custom("size", box "sm")
                if meLoading || not hasToken then HTMLAttr.Custom("disabled", box "")
                OnClick (fun _ -> dispatch AuthCallMe)
            ] [ str (if meLoading then "Calling…" else "Call /me") ]
            div [ Style [ MarginTop "1rem" ] ] [
                responsePanel meState
            ]
        ]

        // ── Source code ────────────────────────────────────────────────────────
        div [] [
            p [ labelStyle ] [ str "Server source (F#)" ]
            wc "fui-code-block" [ "language", "fsharp"; "code", demo.SourceCode ] []
        ]
    ]

let private rateLimitPage (demo: BackendDemoMeta) (model: Model) (dispatch: Msg -> unit) =
    let log = model.RateLimitLog

    let labelStyle   = Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.65rem"; FontWeight "700"; CSSProp.Custom("text-transform","uppercase"); CSSProp.Custom("letter-spacing","0.08em"); Color "#6E6E76"; MarginBottom "0.5rem" ]
    let codePreStyle = Style [ Margin "0"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.8rem"; Color "#E8E8ED"; LineHeight "1.7"; CSSProp.Custom("white-space","pre") ]
    let panelStyle   = Style [ Background "#0D0D0F"; Border "1px solid #2A2A2E"; BorderRadius "8px"; Padding "1.125rem 1.375rem"; CSSProp.Custom("overflow","auto") ]
    let cardStyle    = Style [ Background "#161618"; Border "1px solid #2A2A2E"; BorderRadius "10px"; Padding "1.25rem 1.5rem"; MarginBottom "1.25rem" ]

    div [ ClassName "comp-page" ] [
        div [ ClassName "comp-header" ] [
            div [ ClassName "comp-header-row" ] [
                h1 [ ClassName "comp-name" ] [ str demo.Name ]
                wc "fui-badge" [ "variant", "accent" ] [ str "Backend" ]
            ]
            p [ Style [ FontFamily "'Sora',sans-serif"; Color "#6E6E76"; Margin "0" ] ] [ str demo.Description ]
        ]

        // ── Policy card ────────────────────────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str "Active Policy" ]
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("gap","0.625rem"); CSSProp.Custom("flex-wrap","wrap") ] ] [
                wc "fui-badge" [ "variant", "success" ] [ str "5 requests" ]
                span [ Style [ Color "#6E6E76"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.8rem" ] ] [ str "/" ]
                wc "fui-badge" [] [ str "10-second window" ]
                span [ Style [ Color "#6E6E76"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.8rem" ] ] [ str "·" ]
                wc "fui-badge" [ "variant", "warning" ] [ str "Fixed Window" ]
                span [ Style [ Color "#6E6E76"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.8rem" ] ] [ str "·" ]
                wc "fui-badge" [] [ str "Global counter (shared across all visitors)" ]
            ]
        ]

        // ── Controls card ──────────────────────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str "Fire Requests" ]
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("gap","0.75rem"); CSSProp.Custom("flex-wrap","wrap") ] ] [
                domEl "fui-button" [
                    HTMLAttr.Custom("variant", box "primary")
                    HTMLAttr.Custom("size", box "sm")
                    OnClick (fun _ -> dispatch RateLimitFire)
                ] [ str "Fire One" ]
                domEl "fui-button" [
                    HTMLAttr.Custom("variant", box "danger")
                    HTMLAttr.Custom("size", box "sm")
                    OnClick (fun _ -> dispatch RateLimitHammer)
                ] [ str "Hammer ×10" ]
                if not log.IsEmpty then
                    domEl "fui-button" [
                        HTMLAttr.Custom("variant", box "secondary")
                        HTMLAttr.Custom("size", box "sm")
                        OnClick (fun _ -> dispatch RateLimitClear)
                    ] [ str "Clear Log" ]
            ]
        ]

        // ── Response log ───────────────────────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str $"Response Log ({log.Length} / 20)" ]
            if log.IsEmpty then
                wc "fui-empty-state"
                    [ "title", "No requests yet"
                      "description", "Press \"Fire One\" or \"Hammer ×10\" to start." ] []
            else
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("flex-direction","column"); CSSProp.Custom("gap","0.5rem") ] ] [
                    for (status, body) in log do
                        yield div [ panelStyle ] [
                            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("gap","0.625rem"); MarginBottom "0.5rem" ] ] [
                                wc "fui-badge" [ "variant", (if status = 200 then "success" elif status = 429 then "warning" else "danger") ] [
                                    str (if status = 0 then "ERR" else $"HTTP {status}")
                                ]
                            ]
                            pre [ codePreStyle ] [ str body ]
                        ]
                ]
        ]

        // ── Source code ────────────────────────────────────────────────────────
        div [] [
            p [ labelStyle ] [ str "Server source (F#)" ]
            wc "fui-code-block" [ "language", "fsharp"; "code", demo.SourceCode ] []
        ]
    ]

let private backgroundJobPage (demo: BackendDemoMeta) (model: Model) (dispatch: Msg -> unit) =
    let labelStyle   = Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.65rem"; FontWeight "700"; CSSProp.Custom("text-transform","uppercase"); CSSProp.Custom("letter-spacing","0.08em"); Color "#6E6E76"; MarginBottom "0.5rem" ]
    let codePreStyle = Style [ Margin "0"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.8rem"; Color "#E8E8ED"; LineHeight "1.7"; CSSProp.Custom("white-space","pre") ]
    let panelStyle   = Style [ Background "#0D0D0F"; Border "1px solid #2A2A2E"; BorderRadius "8px"; Padding "1.125rem 1.375rem"; CSSProp.Custom("overflow","auto") ]
    let cardStyle    = Style [ Background "#161618"; Border "1px solid #2A2A2E"; BorderRadius "10px"; Padding "1.25rem 1.5rem"; MarginBottom "1.25rem" ]

    let statusVariant = function
        | "completed" -> "success"
        | "failed"    -> "danger"
        | "running"   -> "info"
        | _           -> "default"

    let progressBar (pct: int) (status: string) =
        let colour =
            match status with
            | "completed" -> "#30A46C"
            | "failed"    -> "#E54D2E"
            | _           -> "#0091FF"
        div [ Style [ Height "6px"; Background "#2A2A2E"; BorderRadius "3px"; CSSProp.Custom("overflow","hidden"); MarginTop "0.5rem" ] ] [
            div [ Style [ Height "100%"; Background colour; Width $"{pct}%%"; CSSProp.Custom("transition","width 0.6s ease") ] ] []
        ]

    let parseField (key: string) (json: string) =
        let marker = $"\"{key}\":"
        let idx = json.IndexOf(marker)
        if idx < 0 then ""
        else
            let rest = json.[idx + marker.Length..].TrimStart()
            if rest.StartsWith("\"") then
                let inner = rest.[1..]
                let endQ  = inner.IndexOf('"')
                if endQ >= 0 then inner.[..endQ - 1] else ""
            else
                let endIdx = rest.IndexOfAny([|','; '}'; ']'|])
                if endIdx >= 0 then rest.[..endIdx - 1] else rest

    div [ ClassName "comp-page" ] [
        div [ ClassName "comp-header" ] [
            div [ ClassName "comp-header-row" ] [
                h1 [ ClassName "comp-name" ] [ str demo.Name ]
                wc "fui-badge" [ "variant", "accent" ] [ str "Backend" ]
            ]
            p [ Style [ FontFamily "'Sora',sans-serif"; Color "#6E6E76"; Margin "0" ] ] [ str demo.Description ]
        ]

        // ── Enqueue ────────────────────────────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str "POST /api/demo/jobs — enqueue a new job" ]
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("gap","0.75rem"); CSSProp.Custom("flex-wrap","wrap") ] ] [
                domEl "fui-button" [
                    HTMLAttr.Custom("variant", box "primary")
                    HTMLAttr.Custom("size", box "sm")
                    if model.BackendResults |> Map.tryFind "job-start" = Some Fetching then
                        HTMLAttr.Custom("disabled", box "")
                    OnClick (fun _ -> dispatch JobStart)
                ] [ str "Start Job" ]
                span [ Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.75rem"; Color "#6E6E76" ] ] [
                    str "Each job runs 5 steps over ~4 seconds"
                ]
            ]
            match model.BackendResults |> Map.tryFind "job-start" with
            | Some(Done(status, body)) ->
                div [ Style [ MarginTop "0.875rem" ] ] [
                    div [ Style [ MarginBottom "0.5rem" ] ] [
                        wc "fui-badge" [ "variant", "success" ] [ str $"HTTP {status}" ]
                    ]
                    div [ panelStyle ] [ pre [ codePreStyle ] [ str body ] ]
                ]
            | Some(Failed err) ->
                div [ Style [ MarginTop "0.875rem" ] ] [
                    wc "fui-alert" [ "variant", "danger"; "title", "Error" ] [ str err ]
                ]
            | _ -> ()
        ]

        // ── Job queue ──────────────────────────────────────────────────────────
        div [ cardStyle ] [
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("justify-content","space-between"); MarginBottom "0.875rem" ] ] [
                p [ labelStyle ] [ str $"Job Queue ({model.JobLog.Length})" ]
                if model.JobPolling then
                    wc "fui-spinner" [ "size", "tiny"; "label", "Polling…" ] []
            ]
            if model.JobLog.IsEmpty then
                wc "fui-empty-state"
                    [ "title", "No jobs yet"
                      "description", "Press \"Start Job\" to enqueue one." ] []
            else
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("flex-direction","column"); CSSProp.Custom("gap","0.75rem") ] ] [
                    for (jobId, json) in model.JobLog do
                        let status   = parseField "status"   json
                        let progress = parseField "progress" json |> (fun s -> match System.Int32.TryParse s with true, v -> v | _ -> 0)
                        let result   = parseField "result"   json
                        yield div [ panelStyle ] [
                            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("gap","0.625rem"); CSSProp.Custom("flex-wrap","wrap") ] ] [
                                span [ Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.75rem"; Color "#9D9DAA" ] ] [ str $"#{jobId}" ]
                                wc "fui-badge" [ "variant", statusVariant status ] [ str status ]
                                span [ Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.72rem"; Color "#6E6E76" ] ] [ str $"{progress}%%" ]
                            ]
                            progressBar progress status
                            if result <> "" then
                                p [ Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.75rem"; Color "#A0E0A0"; MarginTop "0.5rem"; Margin "0.5rem 0 0" ] ] [ str result ]
                        ]
                ]
        ]

        // ── Source code ────────────────────────────────────────────────────────
        div [] [
            p [ labelStyle ] [ str "Server source (F#)" ]
            wc "fui-code-block" [ "language", "fsharp"; "code", demo.SourceCode ] []
        ]
    ]

let private wsPage (demo: BackendDemoMeta) (model: Model) (dispatch: Msg -> unit) =
    let labelStyle   = Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.65rem"; FontWeight "700"; CSSProp.Custom("text-transform","uppercase"); CSSProp.Custom("letter-spacing","0.08em"); Color "#6E6E76"; MarginBottom "0.5rem" ]
    let codePreStyle = Style [ Margin "0"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.8rem"; Color "#E8E8ED"; LineHeight "1.7"; CSSProp.Custom("white-space","pre") ]
    let cardStyle    = Style [ Background "#161618"; Border "1px solid #2A2A2E"; BorderRadius "10px"; Padding "1.25rem 1.5rem"; MarginBottom "1.25rem" ]
    let inputStyle   = Style [ Background "#0D0D0F"; Border "1px solid #2A2A2E"; BorderRadius "6px"; Color "#E8E8ED"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.875rem"; Padding "0.5rem 0.75rem"; Outline "none"; CSSProp.Custom("box-sizing","border-box"); CSSProp.Custom("flex","1") ]

    let isOpen       = model.WsStatus = WsOpen
    let isConnecting = model.WsStatus = WsConnecting

    let statusBadge =
        match model.WsStatus with
        | WsIdle       -> wc "fui-badge" [] [ str "Idle" ]
        | WsConnecting -> wc "fui-badge" [ "variant", "warning" ] [ str "Connecting…" ]
        | WsOpen       -> wc "fui-badge" [ "variant", "success" ] [ str "Connected" ]
        | WsGone       -> wc "fui-badge" [ "variant", "danger"  ] [ str "Closed" ]

    let kindStyle kind =
        match kind with
        | "sent"  -> Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.72rem"; Color "#A0C4FF"; FontWeight "700" ]
        | "recv"  -> Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.72rem"; Color "#A0E0A0"; FontWeight "700" ]
        | "error" -> Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.72rem"; Color "#FF9999"; FontWeight "700" ]
        | _       -> Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.72rem"; Color "#6E6E76"; FontWeight "700" ]

    div [ ClassName "comp-page" ] [
        div [ ClassName "comp-header" ] [
            div [ ClassName "comp-header-row" ] [
                h1 [ ClassName "comp-name" ] [ str demo.Name ]
                wc "fui-badge" [ "variant", "accent" ] [ str "Backend" ]
            ]
            p [ Style [ FontFamily "'Sora',sans-serif"; Color "#6E6E76"; Margin "0" ] ] [ str demo.Description ]
        ]

        // ── Connection card ────────────────────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str "Connection" ]
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("gap","0.875rem"); CSSProp.Custom("flex-wrap","wrap") ] ] [
                statusBadge
                span [ Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.75rem"; Color "#6E6E76" ] ] [
                    str "ws://localhost/api/demo/ws/echo"
                ]
                if isOpen then
                    domEl "fui-button" [
                        HTMLAttr.Custom("variant", box "danger")
                        HTMLAttr.Custom("size", box "sm")
                        OnClick (fun _ -> dispatch WsDisconnect)
                    ] [ str "Disconnect" ]
                else
                    domEl "fui-button" [
                        HTMLAttr.Custom("variant", box "primary")
                        HTMLAttr.Custom("size", box "sm")
                        if isConnecting then HTMLAttr.Custom("disabled", box "")
                        OnClick (fun _ -> dispatch WsConnect)
                    ] [ str (if isConnecting then "Connecting…" else "Connect") ]
            ]
        ]

        // ── Send card (only when connected) ────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str "Send Message" ]
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("gap","0.625rem") ] ] [
                domEl "input" [
                    HTMLAttr.Type "text"
                    HTMLAttr.Placeholder (if isOpen then "Type a message and press Enter or Send…" else "Connect first")
                    HTMLAttr.Value model.WsInput
                    inputStyle
                    HTMLAttr.Disabled (not isOpen)
                    OnChange (fun e -> dispatch (WsInputStr (e.target :?> Browser.Types.HTMLInputElement).value))
                    OnKeyDown (fun e ->
                        if eventKey e = "Enter" then dispatch WsSend)
                ] []
                domEl "fui-button" [
                    HTMLAttr.Custom("variant", box "primary")
                    HTMLAttr.Custom("size", box "sm")
                    if not isOpen || model.WsInput.Trim() = "" then HTMLAttr.Custom("disabled", box "")
                    OnClick (fun _ -> dispatch WsSend)
                ] [ str "Send" ]
            ]
        ]

        // ── Message log ────────────────────────────────────────────────────────
        div [ cardStyle ] [
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("justify-content","space-between"); MarginBottom "0.875rem" ] ] [
                p [ labelStyle ] [ str $"Message Log ({model.WsLog.Length})" ]
                if not model.WsLog.IsEmpty then
                    domEl "fui-button" [
                        HTMLAttr.Custom("variant", box "secondary")
                        HTMLAttr.Custom("size", box "xs")
                        OnClick (fun _ -> dispatch WsLogClear)
                    ] [ str "Clear" ]
            ]
            if model.WsLog.IsEmpty then
                wc "fui-empty-state"
                    [ "title", "No messages yet"
                      "description", "Connect and send a message to see the echo." ] []
            else
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("flex-direction","column"); CSSProp.Custom("gap","0.375rem"); MaxHeight "360px"; CSSProp.Custom("overflow-y","auto") ] ] [
                    for (kind, text) in model.WsLog do
                        yield div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","flex-start"); CSSProp.Custom("gap","0.625rem"); Padding "0.5rem 0.75rem"; Background "#0D0D0F"; BorderRadius "6px"; Border "1px solid #1E1E21" ] ] [
                            span [ kindStyle kind ] [ str (kind.ToUpper()) ]
                            pre [ Style [ Margin "0"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.8rem"; Color "#E8E8ED"; CSSProp.Custom("white-space","pre-wrap"); CSSProp.Custom("word-break","break-all"); CSSProp.Custom("flex","1") ] ] [
                                str text
                            ]
                        ]
                ]
        ]

        // ── Source code ────────────────────────────────────────────────────────
        div [] [
            p [ labelStyle ] [ str "Server source (F#)" ]
            wc "fui-code-block" [ "language", "fsharp"; "code", demo.SourceCode ] []
        ]
    ]

let private fileUploadPage (demo: BackendDemoMeta) (model: Model) (dispatch: Msg -> unit) =
    let labelStyle   = Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.65rem"; FontWeight "700"; CSSProp.Custom("text-transform","uppercase"); CSSProp.Custom("letter-spacing","0.08em"); Color "#6E6E76"; MarginBottom "0.5rem" ]
    let codePreStyle = Style [ Margin "0"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.8rem"; Color "#E8E8ED"; LineHeight "1.7"; CSSProp.Custom("white-space","pre-wrap"); CSSProp.Custom("word-break","break-all") ]
    let panelStyle   = Style [ Background "#0D0D0F"; Border "1px solid #2A2A2E"; BorderRadius "8px"; Padding "1.125rem 1.375rem"; CSSProp.Custom("overflow","auto") ]
    let cardStyle    = Style [ Background "#161618"; Border "1px solid #2A2A2E"; BorderRadius "10px"; Padding "1.25rem 1.5rem"; MarginBottom "1.25rem" ]

    let uploadState  = model.BackendResults |> Map.tryFind "file-upload" |> Option.defaultValue Idle
    let uploading    = uploadState = Fetching
    let statusVariant code = if code >= 200 && code < 300 then "success" elif code >= 400 then "danger" else "info"

    let formatBytes n =
        if n < 1024 then $"{n} B"
        elif n < 1024 * 1024 then $"{n / 1024} KB"
        else $"{n / 1024 / 1024} MB"

    div [ ClassName "comp-page" ] [
        div [ ClassName "comp-header" ] [
            div [ ClassName "comp-header-row" ] [
                h1 [ ClassName "comp-name" ] [ str demo.Name ]
                wc "fui-badge" [ "variant", "accent" ] [ str "Backend" ]
            ]
            p [ Style [ FontFamily "'Sora',sans-serif"; Color "#6E6E76"; Margin "0" ] ] [ str demo.Description ]
        ]

        // ── File selector ──────────────────────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str "Select File" ]
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("gap","0.875rem"); CSSProp.Custom("flex-wrap","wrap") ] ] [
                domEl "input" [
                    HTMLAttr.Type "file"
                    Style [ Background "#0D0D0F"; Border "1px solid #2A2A2E"; BorderRadius "6px"; Color "#E8E8ED"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.8rem"; Padding "0.5rem 0.75rem"; CSSProp.Custom("cursor","pointer") ]
                    OnChange (fun e ->
                        let f = changeEventFile e
                        if isNull f then dispatch UploadClear
                        else dispatch (UploadFileSelected f))
                ] []
                match model.UploadFile with
                | Some f ->
                    div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("gap","0.5rem"); CSSProp.Custom("flex-wrap","wrap") ] ] [
                        wc "fui-badge" [ "variant", "info" ] [ str (fileName f) ]
                        wc "fui-badge" [] [ str (formatBytes (fileSize f)) ]
                        wc "fui-badge" [] [ str (let t = fileType f in if t = "" then "unknown" else t) ]
                    ]
                | None ->
                    span [ Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.75rem"; Color "#6E6E76"; FontStyle "italic" ] ] [
                        str "No file selected"
                    ]
            ]
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap","0.625rem"); MarginTop "1rem" ] ] [
                domEl "fui-button" [
                    HTMLAttr.Custom("variant", box "primary")
                    HTMLAttr.Custom("size", box "sm")
                    if model.UploadFile.IsNone || uploading then HTMLAttr.Custom("disabled", box "")
                    OnClick (fun _ -> dispatch UploadSend)
                ] [ str (if uploading then "Uploading…" else "Upload") ]
                if model.UploadFile.IsSome || uploadState <> Idle then
                    domEl "fui-button" [
                        HTMLAttr.Custom("variant", box "secondary")
                        HTMLAttr.Custom("size", box "sm")
                        OnClick (fun _ -> dispatch UploadClear)
                    ] [ str "Clear" ]
            ]
        ]

        // ── Response ───────────────────────────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str "Server Response" ]
            match uploadState with
            | Idle ->
                wc "fui-empty-state"
                    [ "title", "No response yet"
                      "description", "Select a file and press Upload." ] []
            | Fetching ->
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("justify-content","center"); Padding "1.5rem" ] ] [
                    wc "fui-spinner" [ "label", "Uploading…" ] []
                ]
            | Failed err ->
                wc "fui-alert" [ "variant", "danger"; "title", "Network error" ] [ str err ]
            | Done(status, body) ->
                div [] [
                    div [ Style [ MarginBottom "0.75rem" ] ] [
                        wc "fui-badge" [ "variant", statusVariant status ] [ str $"HTTP {status}" ]
                    ]
                    div [ panelStyle ] [ pre [ codePreStyle ] [ str body ] ]
                ]
        ]

        // ── Source code ────────────────────────────────────────────────────────
        div [] [
            p [ labelStyle ] [ str "Server source (F#)" ]
            wc "fui-code-block" [ "language", "fsharp"; "code", demo.SourceCode ] []
        ]
    ]

let private ssePage (demo: BackendDemoMeta) (model: Model) (dispatch: Msg -> unit) =
    let labelStyle   = Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.65rem"; FontWeight "700"; CSSProp.Custom("text-transform","uppercase"); CSSProp.Custom("letter-spacing","0.08em"); Color "#6E6E76"; MarginBottom "0.5rem" ]
    let codePreStyle = Style [ Margin "0"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.8rem"; Color "#E8E8ED"; LineHeight "1.7"; CSSProp.Custom("white-space","pre-wrap"); CSSProp.Custom("word-break","break-all") ]
    let cardStyle    = Style [ Background "#161618"; Border "1px solid #2A2A2E"; BorderRadius "10px"; Padding "1.25rem 1.5rem"; MarginBottom "1.25rem" ]
    let rowStyle     = Style [ Background "#0D0D0F"; Border "1px solid #1E1E21"; BorderRadius "6px"; Padding "0.5rem 0.75rem"; Display DisplayOptions.Flex; CSSProp.Custom("align-items","flex-start"); CSSProp.Custom("gap","0.625rem") ]

    let statusBadge =
        if model.SseActive then
            wc "fui-badge" [ "variant", "success" ] [ str "Streaming…" ]
        elif model.SseLog.IsEmpty then
            wc "fui-badge" [] [ str "Idle" ]
        else
            wc "fui-badge" [ "variant", "info" ] [ str "Complete" ]

    div [ ClassName "comp-page" ] [
        div [ ClassName "comp-header" ] [
            div [ ClassName "comp-header-row" ] [
                h1 [ ClassName "comp-name" ] [ str demo.Name ]
                wc "fui-badge" [ "variant", "accent" ] [ str "Backend" ]
            ]
            p [ Style [ FontFamily "'Sora',sans-serif"; Color "#6E6E76"; Margin "0" ] ] [ str demo.Description ]
        ]

        // ── Connection card ────────────────────────────────────────────────────
        div [ cardStyle ] [
            p [ labelStyle ] [ str "Stream Control" ]
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("gap","0.875rem"); CSSProp.Custom("flex-wrap","wrap") ] ] [
                statusBadge
                if model.SseActive then
                    domEl "fui-button" [
                        HTMLAttr.Custom("variant", box "danger")
                        HTMLAttr.Custom("size", box "sm")
                        OnClick (fun _ -> dispatch SseDisconnect)
                    ] [ str "Disconnect" ]
                else
                    domEl "fui-button" [
                        HTMLAttr.Custom("variant", box "primary")
                        HTMLAttr.Custom("size", box "sm")
                        OnClick (fun _ -> dispatch SseConnect)
                    ] [ str "Start Stream" ]
                span [ Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.75rem"; Color "#6E6E76" ] ] [
                    str "10 events · 1 per second · ~10 s total"
                ]
            ]
        ]

        // ── Event log ──────────────────────────────────────────────────────────
        div [ cardStyle ] [
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items","center"); CSSProp.Custom("justify-content","space-between"); MarginBottom "0.875rem" ] ] [
                p [ labelStyle ] [ str $"Event Log ({model.SseLog.Length} / 10)" ]
                if not model.SseLog.IsEmpty then
                    domEl "fui-button" [
                        HTMLAttr.Custom("variant", box "secondary")
                        HTMLAttr.Custom("size", box "xs")
                        OnClick (fun _ -> dispatch SseLogClear)
                    ] [ str "Clear" ]
            ]
            if model.SseLog.IsEmpty then
                wc "fui-empty-state"
                    [ "title", "No events yet"
                      "description", "Press \"Start Stream\" to open the EventSource connection." ] []
            else
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("flex-direction","column"); CSSProp.Custom("gap","0.375rem"); MaxHeight "400px"; CSSProp.Custom("overflow-y","auto") ] ] [
                    for (i, data) in model.SseLog |> List.mapi (fun i d -> i, d) do
                        yield div [ rowStyle ] [
                            span [ Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.7rem"; Color "#6E6E76"; CSSProp.Custom("white-space","nowrap"); PaddingTop "1px" ] ] [
                                str $"#{model.SseLog.Length - i}"
                            ]
                            pre [ codePreStyle ] [ str data ]
                        ]
                ]
        ]

        // ── Source code ────────────────────────────────────────────────────────
        div [] [
            p [ labelStyle ] [ str "Server source (F#)" ]
            wc "fui-code-block" [ "language", "fsharp"; "code", demo.SourceCode ] []
        ]
    ]

let backendDemoPage (slug: string) (model: Model) (dispatch: Msg -> unit) =
    match backendDemos |> List.tryFind (fun d -> d.Slug = slug) with
    | None ->
        div [ ClassName "comp-page" ] [ h1 [] [ str $"Demo not found: {slug}" ] ]
    | Some demo when demo.Slug = "crud-api" ->
        crudApiPage demo model dispatch
    | Some demo when demo.Slug = "pagination-api" ->
        pagApiPage demo model dispatch
    | Some demo when demo.Slug = "jwt-auth" ->
        jwtAuthPage demo model dispatch
    | Some demo when demo.Slug = "rate-limiting" ->
        rateLimitPage demo model dispatch
    | Some demo when demo.Slug = "background-jobs" ->
        backgroundJobPage demo model dispatch
    | Some demo when demo.Slug = "websocket" ->
        wsPage demo model dispatch
    | Some demo when demo.Slug = "file-upload" ->
        fileUploadPage demo model dispatch
    | Some demo when demo.Slug = "sse" ->
        ssePage demo model dispatch
    | Some demo ->
        let state   = model.BackendResults |> Map.tryFind slug |> Option.defaultValue Idle
        let loading = state = Fetching
        let methodVariant = match demo.Method with "GET" -> "success" | "POST" -> "info" | "DELETE" -> "danger" | _ -> "warning"
        let statusVariant code = if code >= 200 && code < 300 then "success" elif code >= 500 then "danger" elif code >= 400 then "warning" else "info"
        let labelStyle   = Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.65rem"; FontWeight "700"; CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.08em"); Color "#6E6E76"; MarginBottom "0.5rem" ]
        let codePreStyle = Style [ Margin "0"; FontFamily "'JetBrains Mono',monospace"; FontSize "0.8rem"; Color "#E8E8ED"; LineHeight "1.7"; CSSProp.Custom("white-space", "pre") ]
        let panelStyle   = Style [ Background "#0D0D0F"; Border "1px solid #2A2A2E"; BorderRadius "8px"; Padding "1.125rem 1.375rem"; CSSProp.Custom("overflow", "auto") ]

        div [ ClassName "comp-page" ] [
            // Header
            div [ ClassName "comp-header" ] [
                div [ ClassName "comp-header-row" ] [
                    h1 [ ClassName "comp-name" ] [ str demo.Name ]
                    wc "fui-badge" [ "variant", "accent" ] [ str "Backend" ]
                ]
                p [ Style [ FontFamily "'Sora',sans-serif"; Color "#6E6E76"; Margin "0" ] ] [ str demo.Description ]
            ]

            // Endpoint + Send button
            div [ Style [ Background "#161618"; Border "1px solid #2A2A2E"; BorderRadius "10px"; Padding "1.25rem 1.5rem"; MarginBottom "1.5rem" ] ] [
                p [ labelStyle ] [ str "Endpoint" ]
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items", "center"); CSSProp.Custom("gap", "0.75rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
                    wc "fui-badge" [ "variant", methodVariant ] [ str demo.Method ]
                    code [ Style [ FontFamily "'JetBrains Mono',monospace"; FontSize "0.9rem"; Color "#E8E8ED"; CSSProp.Custom("flex", "1") ] ] [ str demo.Url ]
                    domEl "fui-button" [
                        HTMLAttr.Custom("variant", box "primary")
                        if loading then HTMLAttr.Custom("disabled", box "")
                        OnClick (fun _ -> dispatch (BackendFetch(demo.Slug, demo.Url)))
                    ] [ str (if loading then "Sending…" else "Send Request") ]
                ]
            ]

            // Response panel
            div [ Style [ MarginBottom "1.5rem" ] ] [
                p [ labelStyle ] [ str "Response" ]
                match state with
                | Idle ->
                    wc "fui-empty-state" [
                        "title",       "No response yet"
                        "description", "Press Send Request to call the live endpoint."
                    ] []
                | Fetching ->
                    div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("justify-content", "center"); Padding "2rem" ] ] [
                        wc "fui-spinner" [ "label", "Sending request…" ] []
                    ]
                | Done(status, body) ->
                    div [ panelStyle ] [
                        div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("align-items", "center"); CSSProp.Custom("gap", "0.75rem"); MarginBottom "0.875rem" ] ] [
                            wc "fui-badge" [ "variant", statusVariant status ] [ str $"HTTP {status}" ]
                            wc "fui-badge" [] [ str "application/json" ]
                        ]
                        pre [ codePreStyle ] [ str body ]
                    ]
                | Failed err ->
                    wc "fui-alert" [ "variant", "danger"; "title", "Network error" ] [ str err ]
            ]

            // Source code
            div [] [
                p [ labelStyle ] [ str "Server source (F#)" ]
                wc "fui-code-block" [ "language", "fsharp"; "code", demo.SourceCode ] []
            ]
        ]

// ── Root view ─────────────────────────────────────────────────────────────────

let view (model: Model) (dispatch: Msg -> unit) =
    div [ ClassName "shell" ] [
        div [ ClassName (if model.SidebarOpen then "sidebar-overlay visible" else "sidebar-overlay"); OnClick (fun _ -> dispatch CloseSidebar) ] []
        sidebar model dispatch
        div [ ClassName "shell-main" ] [
            topbar model dispatch
            main [ ClassName "page-content" ] [
                div [ Key (string model.Page) ] (
                    match model.Page with
                    | Home              -> [ homePage ]
                    | GettingStarted    -> [ gettingStartedPage ]
                    | CategoryIndex cat -> [ categoryIndexPage cat ]
                    | BackendDemo slug  -> [ backendDemoPage slug model dispatch ]
                    | CounterElmish     -> [ Pages.CounterElmish.view model.ElmishCounter (ElmishCounterMsg >> dispatch) ]
                    | CounterDom        -> [ Pages.CounterDom.view () ]
                    | Component(cat, slug) -> [ componentPage cat slug ]
                )
            ]
        ]
    ]

Program.mkProgram init update view
|> Program.withSubscription hashSub
|> Program.withReactSynchronous "app"
|> Program.run
