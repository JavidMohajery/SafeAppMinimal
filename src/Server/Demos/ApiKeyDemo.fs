module Demos.ApiKeyDemo

open System
open Giraffe

let private validKeys =
    Map.ofList [
        "sk-demo-abc123", "alice"
        "sk-demo-xyz789", "bob"
    ]

let resource : HttpHandler = fun next ctx ->
    let key =
        match ctx.Request.Headers.TryGetValue("X-Api-Key") with
        | true, v -> v.ToString()
        | _       -> ""
    if key = "" then
        (setStatusCode 401
         >=> json {| error = "Missing X-Api-Key header"
                     hint  = "Add header: X-Api-Key: sk-demo-abc123" |}) next ctx
    else
        match validKeys |> Map.tryFind key with
        | None ->
            (setStatusCode 403
             >=> json {| error = "Invalid API key"
                         hint  = "Valid demo keys: sk-demo-abc123, sk-demo-xyz789" |}) next ctx
        | Some user ->
            json {| user      = user
                    scopes    = [| "read"; "list" |]
                    secret    = "The treasure is buried under the oak tree."
                    requestId = Guid.NewGuid().ToString("N").[..7] |} next ctx
