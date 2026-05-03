module Demos.CorrelationDemo

open System
open Giraffe
open Microsoft.Extensions.Primitives

let ping : HttpHandler = fun next ctx ->
    let requestId =
        match ctx.Request.Headers.TryGetValue("X-Request-ID") with
        | true, v when v.ToString() <> "" -> v.ToString()
        | _ -> Guid.NewGuid().ToString("N").[..11]

    ctx.Response.Headers["X-Request-ID"]    <- StringValues(requestId)
    ctx.Response.Headers["X-Response-Time"] <- StringValues($"{Random.Shared.Next(5, 50)}ms")

    json {|
        requestId  = requestId
        message    = "pong"
        timestamp  = DateTime.UtcNow.ToString("o")
        services   = [|
            {| service = "auth-service";  correlationId = requestId; latencyMs = Random.Shared.Next(2, 15)  |}
            {| service = "data-service";  correlationId = requestId; latencyMs = Random.Shared.Next(5, 25)  |}
            {| service = "cache-service"; correlationId = requestId; latencyMs = Random.Shared.Next(1, 5)   |}
        |]
    |} next ctx
