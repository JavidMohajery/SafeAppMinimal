module Demos.IdempotencyDemo

open System
open System.Collections.Concurrent
open Giraffe

type OrderResult = { orderId: string; amount: int; status: string; createdAt: string }

let private cache = ConcurrentDictionary<string, OrderResult>()

let placeOrder : HttpHandler = fun next ctx -> task {
    let key =
        match ctx.Request.Headers.TryGetValue("Idempotency-Key") with
        | true, v -> v.ToString().Trim()
        | _       -> ""
    if key = "" then
        return!
            (setStatusCode 400
             >=> json {| error = "Missing Idempotency-Key request header" |}) next ctx
    else
        match cache.TryGetValue(key) with
        | true, existing ->
            return!
                (setStatusCode 200
                 >=> setHttpHeader "X-Idempotent-Replayed" "true"
                 >=> json existing) next ctx
        | _ ->
            let result = {
                orderId   = Guid.NewGuid().ToString("N").[..7]
                amount    = Random.Shared.Next(10, 500)
                status    = "confirmed"
                createdAt = DateTime.UtcNow.ToString("o")
            }
            cache.[key] <- result
            return!
                (setStatusCode 201
                 >=> setHttpHeader "X-Idempotent-Replayed" "false"
                 >=> json result) next ctx
}
