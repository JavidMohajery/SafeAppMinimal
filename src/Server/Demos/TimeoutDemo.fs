module Demos.TimeoutDemo

open System
open System.Threading
open System.Threading.Tasks
open System.Diagnostics
open Giraffe

let slow : HttpHandler = fun next ctx -> task {
    let tryInt (key: string) (def: int) =
        ctx.TryGetQueryStringValue key
        |> Option.bind (fun s -> match Int32.TryParse s with true, v -> Some v | _ -> None)
        |> Option.defaultValue def

    let workMs  = tryInt "workMs"  2000 |> max 0 |> min 30_000
    let limitMs = tryInt "limitMs" 1000 |> max 50 |> min 30_000

    let sw = Stopwatch.StartNew()
    use cts = CancellationTokenSource.CreateLinkedTokenSource(ctx.RequestAborted)
    cts.CancelAfter(limitMs)

    try
        do! Task.Delay(workMs, cts.Token)
        return! json {| outcome = "completed"; elapsed_ms = int sw.ElapsedMilliseconds; work_ms = workMs; limit_ms = limitMs |} next ctx
    with :? OperationCanceledException ->
        let outcome =
            if ctx.RequestAborted.IsCancellationRequested then "client_abort"
            else "timeout"
        return!
            (setStatusCode 408 >=> json {| outcome = outcome; elapsed_ms = int sw.ElapsedMilliseconds; work_ms = workMs; limit_ms = limitMs |})
                next ctx
}
