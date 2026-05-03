module Demos.RetryDemo

open System.Collections.Concurrent
open Giraffe

let private counters    = ConcurrentDictionary<string, int>()
let private maxFailures = 3

let flaky : HttpHandler = fun next ctx ->
    let session =
        match ctx.Request.Query.TryGetValue("session") with
        | true, v -> v.ToString()
        | _       -> "default"

    let attempt = counters.AddOrUpdate(session, 1, fun _ c -> c + 1)

    if attempt <= maxFailures then
        (setStatusCode 503
         >=> setHttpHeader "Retry-After" "1"
         >=> json {| error = "Service temporarily unavailable"; attempt = attempt; willSucceedAfter = maxFailures |}) next ctx
    else
        counters.[session] <- 0
        json {| success = true; attempt = attempt; message = $"Succeeded after {maxFailures} transient failures" |} next ctx

let reset : HttpHandler = fun next ctx ->
    let session =
        match ctx.Request.Query.TryGetValue("session") with
        | true, v -> v.ToString()
        | _       -> "default"
    counters.[session] <- 0
    json {| reset = true; session = session |} next ctx
