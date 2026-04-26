module Client

open Browser
open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props

// Register all Web Components with the browser before React starts rendering.
do ComponentRegistry.registerAll()

type Page = Home | CounterElmish | CounterDom

type Model = {
    Page          : Page
    ElmishCounter : Pages.CounterElmish.Model
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
                div [ ClassName "sidebar-group-label" ] [ str "Inputs & Forms" ]
                sidebarLink "Button" "#/component/inputs/button" Home model.Page
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
        p [] [ str "A comprehensive reference of web UI components and backend patterns." ]
        p [] [ str "Every component is a standalone Web Component — drop it into any HTML page." ]

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
                    wc "fui-button" [ "variant", "primary"; "disabled", "" ] [ str "Disabled" ]
                ]
                div [ ClassName "preview-row"; Style [ MarginTop "0.75rem" ] ] [
                    wc "fui-button" [ "variant", "primary"; "size", "sm" ] [ str "Small" ]
                    wc "fui-button" [ "variant", "primary"; "size", "md" ] [ str "Medium" ]
                    wc "fui-button" [ "variant", "primary"; "size", "lg" ] [ str "Large" ]
                ]
            ]
        ]
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
