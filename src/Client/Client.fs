module Client

open Browser
open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props

type Page = Home | CounterElmish | CounterDom

type Model = {
    Page: Page
    ElmishCounter: Pages.CounterElmish.Model
}

type Msg =
    | UrlChanged of string
    | ElmishCounterMsg of Pages.CounterElmish.Msg

let parsePage (hash: string) =
    match hash.TrimStart('#').TrimStart('/') with
    | "counter-elmish" -> CounterElmish
    | "counter-dom"    -> CounterDom
    | _                -> Home

let init () =
    let counter, _ = Pages.CounterElmish.init ()
    { Page = parsePage window.location.hash; ElmishCounter = counter }, Cmd.none

let update msg model =
    match msg with
    | UrlChanged hash ->
        { model with Page = parsePage hash }, Cmd.none
    | ElmishCounterMsg sub ->
        let m, cmd = Pages.CounterElmish.update sub model.ElmishCounter
        { model with ElmishCounter = m }, Cmd.map ElmishCounterMsg cmd

// Elmish 4 subscription: starts a hashchange listener, returns IDisposable to stop it.
let hashSub (_model: Model) : Sub<Msg> =
    [ ["hash"], fun dispatch ->
        window.addEventListener("hashchange", fun _ ->
            dispatch (UrlChanged window.location.hash))
        { new System.IDisposable with member _.Dispose() = () } ]

// ── Sidebar ───────────────────────────────────────────────────────────────────

let sidebarLink (label: string) (href: string) (page: Page) (currentPage: Page) =
    a [
        ClassName (if page = currentPage then "sidebar-link active" else "sidebar-link")
        Href href
    ] [ str label ]

let sidebar (model: Model) =
    aside [ ClassName "sidebar" ] [
        div [ ClassName "sidebar-logo" ] [
            img [ Src "/favicon.png"; Alt "logo" ]
            span [] [
                str "Fable"
                span [ ClassName "logo-accent" ] [ str "UI" ]
            ]
        ]
        nav [ ClassName "sidebar-nav" ] [
            div [ ClassName "sidebar-group" ] [
                sidebarLink "Home" "#/" Home model.Page
            ]
            div [ ClassName "sidebar-group" ] [
                div [ ClassName "sidebar-group-label" ] [ str "Examples" ]
                sidebarLink "Counter — Elmish" "#/counter-elmish" CounterElmish model.Page
                sidebarLink "Counter — DOM"    "#/counter-dom"    CounterDom    model.Page
            ]
        ]
    ]

// ── Topbar ────────────────────────────────────────────────────────────────────

let breadcrumbItems (page: Page) : (string * bool) list =
    match page with
    | Home          -> [ "Home", true ]
    | CounterElmish -> [ "Home", false; "Examples", false; "Counter — Elmish", true ]
    | CounterDom    -> [ "Home", false; "Examples", false; "Counter — DOM", true ]

let topbar (model: Model) =
    let crumbs = breadcrumbItems model.Page
    header [ ClassName "topbar" ] [
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
            a [ ClassName "icon-btn"; Href "https://github.com/safe-stack/SAFE-template"; Title "GitHub" ] [ str "GH" ]
        ]
    ]

// ── Pages ─────────────────────────────────────────────────────────────────────

let homePage =
    div [ ClassName "home-welcome" ] [
        h1 [] [
            str "Fable"
            span [ ClassName "hi" ] [ str "UI" ]
        ]
        p [] [ str "A component showcase built with F#, Fable, and Elmish." ]
        p [] [ str "Select an example from the sidebar to get started." ]
    ]

// ── Root view ─────────────────────────────────────────────────────────────────

let view (model: Model) (dispatch: Msg -> unit) =
    div [ ClassName "shell" ] [
        sidebar model
        div [ ClassName "shell-main" ] [
            topbar model
            main [ ClassName "page-content" ] [
                div [ Key (string model.Page) ] (
                    match model.Page with
                    | Home          -> [ homePage ]
                    | CounterElmish -> [ Pages.CounterElmish.view model.ElmishCounter (ElmishCounterMsg >> dispatch) ]
                    | CounterDom    -> [ Pages.CounterDom.view () ]
                )
            ]
        ]
    ]

Program.mkProgram init update view
|> Program.withSubscription hashSub
|> Program.withReactSynchronous "app"
|> Program.run
