module Demos.CircuitBreakerDemo

open System
open Giraffe

type private CBState = Closed | Open | HalfOpen

let mutable private cbState    = Closed
let mutable private failures   = 0
let mutable private successes  = 0
let mutable private lastOpened = DateTime.MinValue
let mutable private totalCalls = 0

let private failThreshold = 3
let private passThreshold = 2
let private openDuration  = TimeSpan.FromSeconds 10.0

let private stateStr () =
    match cbState with
    | Closed   -> "Closed"
    | Open     -> "Open"
    | HalfOpen -> "HalfOpen"

let call : HttpHandler = fun next ctx ->
    let failRate =
        match ctx.TryGetQueryStringValue "failRate" with
        | Some v ->
            match Double.TryParse(v, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture) with
            | true, f -> min 1.0 (max 0.0 f)
            | _       -> 0.5
        | None -> 0.5

    totalCalls <- totalCalls + 1

    if cbState = Open && DateTime.UtcNow - lastOpened > openDuration then
        cbState   <- HalfOpen
        successes <- 0

    match cbState with
    | Open ->
        let sec = max 0 (int (openDuration - (DateTime.UtcNow - lastOpened)).TotalSeconds)
        (setStatusCode 503
         >=> setHttpHeader "Retry-After" (string sec)
         >=> json {| state = "Open"; message = $"Circuit OPEN — retry in {sec}s"; failures = failures; successes = successes; totalCalls = totalCalls |}) next ctx
    | Closed | HalfOpen ->
        let failed = Random.Shared.NextDouble() < failRate
        if failed then
            failures  <- failures + 1
            successes <- 0
            if cbState = HalfOpen || failures >= failThreshold then
                cbState    <- Open
                lastOpened <- DateTime.UtcNow
                failures   <- 0
            (setStatusCode 503 >=> json {|
                state      = stateStr ()
                message    = "Upstream call failed"
                failures   = failures; successes = successes; totalCalls = totalCalls
            |}) next ctx
        else
            successes <- successes + 1
            failures  <- 0
            if cbState = HalfOpen && successes >= passThreshold then
                cbState   <- Closed
                successes <- 0
            (setStatusCode 200 >=> json {|
                state      = stateStr ()
                message    = "Upstream call succeeded"
                failures   = failures; successes = successes; totalCalls = totalCalls
            |}) next ctx

let reset : HttpHandler = fun next ctx ->
    cbState    <- Closed
    failures   <- 0
    successes  <- 0
    totalCalls <- 0
    json {| reset = true; state = "Closed" |} next ctx
