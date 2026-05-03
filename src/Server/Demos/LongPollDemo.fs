module Demos.LongPollDemo

open System
open System.Threading.Tasks
open Giraffe

let mutable private eventId   = 0
let mutable private eventData = "Server started"
let private lockObj            = obj()

let private currentEvent () =
    lock lockObj (fun () -> eventId, eventData)

let private publishNew (msg: string) =
    lock lockObj (fun () ->
        eventId   <- eventId + 1
        eventData <- msg
        eventId)

[<CLIMutable>]
type PublishRequest = { message: string }

let wait : HttpHandler = fun next ctx -> task {
    let lastId =
        match ctx.Request.Query.TryGetValue("lastEventId") with
        | true, v -> match Int32.TryParse(v.ToString()) with true, n -> n | _ -> 0
        | _       -> 0

    let deadline = DateTime.UtcNow.AddSeconds(25.0)
    let mutable result = None

    while result.IsNone && DateTime.UtcNow < deadline && not ctx.RequestAborted.IsCancellationRequested do
        let (id, data) = currentEvent()
        if id > lastId then
            result <- Some {| eventId = id; data = data; timestamp = DateTime.UtcNow.ToString("o") |}
        else
            try  do! Task.Delay(500, ctx.RequestAborted)
            with :? OperationCanceledException -> ()

    match result with
    | Some r -> return! json r next ctx
    | None   -> return! setStatusCode 204 next ctx
}

let publish : HttpHandler = fun next ctx -> task {
    let! req = ctx.BindJsonAsync<PublishRequest>()
    let newId = publishNew req.message
    return! json {| published = true; eventId = newId |} next ctx
}
