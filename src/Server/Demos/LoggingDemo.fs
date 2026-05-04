module Demos.LoggingDemo

open System
open System.Collections.Concurrent
open System.Threading
open Microsoft.Extensions.Logging
open Giraffe

[<CLIMutable>]
type WriteReq = { level: string; template: string; args: string[] }

type LogEntry = { id: int; ts: string; level: string; tmpl: string; args: string[]; msg: string }

let mutable private nextId = 0
let private store = ConcurrentQueue<LogEntry>()

let private toLogLevel = function
    | "Trace"    -> LogLevel.Trace
    | "Debug"    -> LogLevel.Debug
    | "Warning"  -> LogLevel.Warning
    | "Error"    -> LogLevel.Error
    | "Critical" -> LogLevel.Critical
    | _          -> LogLevel.Information

let private render (template: string) (args: string[]) =
    args
    |> Array.fold
        (fun (acc: string) arg ->
            let i0 = acc.IndexOf('{')
            let i1 = if i0 >= 0 then acc.IndexOf('}', i0) else -1
            if i0 >= 0 && i1 > i0 then acc.[0..i0-1] + arg + acc.[i1+1..]
            else acc)
        template

let private trimTo50 () =
    let rec loop () =
        if store.Count > 50 then
            store.TryDequeue() |> ignore
            loop ()
    loop ()

let private logAt (logger: ILogger) (level: LogLevel) (tmpl: string) (args: obj[]) =
    match level with
    | LogLevel.Trace    -> LoggerExtensions.LogTrace(logger, tmpl, args)
    | LogLevel.Debug    -> LoggerExtensions.LogDebug(logger, tmpl, args)
    | LogLevel.Warning  -> LoggerExtensions.LogWarning(logger, tmpl, args)
    | LogLevel.Error    -> LoggerExtensions.LogError(logger, tmpl, args)
    | LogLevel.Critical -> LoggerExtensions.LogCritical(logger, tmpl, args)
    | _                 -> LoggerExtensions.LogInformation(logger, tmpl, args)

let write : HttpHandler = fun next ctx -> task {
    let! req  = ctx.BindJsonAsync<WriteReq>()
    let args  = req.args     |> Option.ofObj |> Option.defaultValue [||]
    let tmpl  = req.template |> Option.ofObj |> Option.defaultValue ""
    let lvl   = req.level    |> Option.ofObj |> Option.defaultValue "Information"
    let level = toLogLevel lvl
    let id    = Interlocked.Increment(&nextId)
    let entry = { id = id; ts = DateTime.UtcNow.ToString("HH:mm:ss.fff")
                  level = lvl; tmpl = tmpl; args = args; msg = render tmpl args }
    store.Enqueue(entry)
    trimTo50 ()
    let logger = ctx.GetLogger("Demo.Logging")
    logAt logger level tmpl (args |> Array.map (fun a -> a :> obj))
    return! json entry next ctx
}

let getAll : HttpHandler = fun next ctx ->
    let entries = store.ToArray() |> Array.rev
    json entries next ctx

let clear : HttpHandler = fun next ctx ->
    let rec drain () =
        if store.Count > 0 then
            store.TryDequeue() |> ignore
            drain ()
    drain ()
    Interlocked.Exchange(&nextId, 0) |> ignore
    json {| cleared = true |} next ctx
