module Client

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props

type Model = { Count: int; History: string list }

type Msg = Increment | Decrement | Reset

let init () = { Count = 0; History = [] }, Cmd.none

let update msg model =
    match msg with
    | Increment ->
        let n = model.Count + 1
        { model with Count = n; History = $"+1 → {n}" :: model.History }, Cmd.none
    | Decrement ->
        let n = model.Count - 1
        { model with Count = n; History = $"-1 → {n}" :: model.History }, Cmd.none
    | Reset ->
        { Count = 0; History = [] }, Cmd.none

let view model dispatch =
    div [] [
        header [] [
            img [ Src "favicon.png"; Alt "logo" ]
            h1 [] [ str "SafeAppMinimal" ]
        ]
        main [] [
            div [ ClassName "card" ] [
                h2 [] [ str "Sample Counter Page" ]
                div [ ClassName "counter-value" ] [ str (string model.Count) ]
                div [ ClassName "controls" ] [
                    button [ OnClick (fun _ -> dispatch Decrement) ] [ str "−" ]
                    button [ OnClick (fun _ -> dispatch Increment) ] [ str "+" ]
                ]
                div [ Style [ MarginTop "1rem" ] ] [
                    button [ Id "reset-btn"; OnClick (fun _ -> dispatch Reset) ] [ str "Reset" ]
                ]
                div [ ClassName "items-section" ] [
                    h3 [] [ str "History" ]
                    ul [ Id "item-list" ] [
                        yield! model.History |> List.mapi (fun i e ->
                            li [ Key (string i) ] [ str e ])
                    ]
                ]
            ]
        ]
    ]

Program.mkProgram init update view
|> Program.withReactSynchronous "app"
|> Program.run
