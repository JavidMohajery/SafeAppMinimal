module Demos.RateLimit

open System
open Giraffe

// ── Fixed-window rate limiter ─────────────────────────────────────────────────

type private Bucket(maxRequests: int, windowSeconds: int) =
    let mutable count     = 0
    let mutable windowEnd = DateTimeOffset.UtcNow.AddSeconds(float windowSeconds)

    member self.TryConsume() =
        lock self (fun () ->
            let now = DateTimeOffset.UtcNow
            if now >= windowEnd then
                count     <- 1
                windowEnd <- now.AddSeconds(float windowSeconds)
                true
            elif count < maxRequests then
                count <- count + 1
                true
            else
                false)

    member self.RetryAfterSeconds() =
        lock self (fun () ->
            (windowEnd - DateTimeOffset.UtcNow).TotalSeconds |> max 0.0 |> int)

let private bucket = Bucket(5, 10)   // 5 requests per 10-second window (global)

// ── Handler ───────────────────────────────────────────────────────────────────

let ping : HttpHandler = fun next ctx ->
    if bucket.TryConsume() then
        json {|
            status    = "ok"
            message   = "Request accepted"
            timestamp = DateTime.UtcNow.ToString("o")
        |} next ctx
    else
        let retryAfter = bucket.RetryAfterSeconds()
        (setStatusCode 429
         >=> setHttpHeader "Retry-After" (string retryAfter)
         >=> json {|
                error      = "Too Many Requests"
                retryAfter = retryAfter
                message    = "Rate limit: 5 requests per 10-second window"
             |}) next ctx
