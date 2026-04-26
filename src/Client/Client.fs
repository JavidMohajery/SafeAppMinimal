module Client

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
    | GoTo of Page
    | ElmishCounterMsg of Pages.CounterElmish.Msg

let init () =
    let counter, _ = Pages.CounterElmish.init ()
    { Page = Home; ElmishCounter = counter }, Cmd.none

let update msg model =
    match msg with
    | GoTo page -> { model with Page = page }, Cmd.none
    | ElmishCounterMsg sub ->
        let m, cmd = Pages.CounterElmish.update sub model.ElmishCounter
        { model with ElmishCounter = m }, Cmd.map ElmishCounterMsg cmd

let navLink label page currentPage dispatch =
    a [
        ClassName (if page = currentPage then "nav-link active" else "nav-link")
        Href "#"
        OnClick (fun e -> e.preventDefault(); dispatch (GoTo page))
    ] [ str label ]

let view model dispatch =
    div [] [
        header [] [
            img [ Src "favicon.png"; Alt "logo" ]
            h1 [] [ str "SafeAppMinimal" ]
        ]
        nav [] [
            navLink "Home" Home model.Page dispatch
            navLink "Counter — Elmish" CounterElmish model.Page dispatch
            navLink "Counter — DOM" CounterDom model.Page dispatch
        ]
        main [] [
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

Program.mkProgram init update view
|> Program.withReactSynchronous "app"
|> Program.run
