module Demos.FeatureFlagsDemo

open System.Collections.Concurrent
open Giraffe

let private flags =
    [| "feature-x", false; "maintenance", false; "beta-endpoint", true |]
    |> Array.map System.Collections.Generic.KeyValuePair
    |> ConcurrentDictionary<string, bool>

let private flag name = flags.TryGetValue name |> function true, v -> v | _ -> false

let private setFlag (name: string) (value: bool) =
    flags.AddOrUpdate(name, value, fun _ _ -> value) |> ignore

let getAll : HttpHandler = fun next ctx ->
    let result =
        flags.Keys
        |> Seq.sort
        |> Seq.map (fun k -> {| name = k; enabled = flags.[k] |})
        |> Seq.toArray
    json result next ctx

let enable (name: string) : HttpHandler = fun next ctx ->
    setFlag name true
    json {| name = name; enabled = true |} next ctx

let disable (name: string) : HttpHandler = fun next ctx ->
    setFlag name false
    json {| name = name; enabled = false |} next ctx

let resource : HttpHandler = fun next ctx ->
    match () with
    | _ when flag "maintenance"         ->
        (setStatusCode 503 >=> json {| error = "Service under maintenance"; trigger = "maintenance" |}) next ctx
    | _ when not (flag "beta-endpoint") ->
        (setStatusCode 404 >=> json {| error = "Endpoint not available";    trigger = "beta-endpoint" |}) next ctx
    | _ when flag "feature-x"           ->
        json {| message = "Enhanced response — feature-x is ON"; extras = {| version = "2.0"; premium = true |} |} next ctx
    | _                                 ->
        json {| message = "Standard response — feature-x is OFF" |} next ctx
