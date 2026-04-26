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

let navLink (label: string) (href: string) (page: Page) (currentPage: Page) =
    a [
        ClassName (if page = currentPage then "nav-link active" else "nav-link")
        Href href
    ] [ str label ]

let view model dispatch =
    div [] [
        header [] [
            img [ Src "favicon.png"; Alt "logo" ]
            h1 [] [ str "SafeAppMinimal" ]
        ]
        nav [] [
            navLink "Home"             "#/"               Home          model.Page
            navLink "Counter — Elmish" "#/counter-elmish" CounterElmish model.Page
            navLink "Counter — DOM"    "#/counter-dom"    CounterDom    model.Page
        ]
        main [] [
            // Key forces React to unmount/remount the whole subtree on page change,
            // preventing DOM counter children from persisting into the Elmish counter view.
            div [ Key (string model.Page) ] [
                match model.Page with
                | Home ->
                    div [ ClassName "card" ] [
                        h2 [] [ str "Welcome" ]
                        p [ Style [ MarginTop "1rem"; Color "#555" ] ] [
                            str "Pick a counter implementation from the menu above."
                        ]
                    ]
                | CounterElmish ->
                    Pages.CounterElmish.view model.ElmishCounter (ElmishCounterMsg >> dispatch)
                | CounterDom ->
                    Pages.CounterDom.view ()
            ]
        ]
    ]

Program.mkProgram init update view
|> Program.withSubscription hashSub
|> Program.withReactSynchronous "app"
|> Program.run
