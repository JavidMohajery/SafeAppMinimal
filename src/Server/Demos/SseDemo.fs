module Demos.SseDemo

open System
open System.Text
open System.Threading.Tasks
open Giraffe

let private rng = Random.Shared

let stream : HttpHandler = fun next ctx ->
    task {
        ctx.Response.ContentType <- "text/event-stream; charset=utf-8"
        ctx.Response.Headers["Cache-Control"]     <- Microsoft.Extensions.Primitives.StringValues("no-cache")
        ctx.Response.Headers["X-Accel-Buffering"] <- Microsoft.Extensions.Primitives.StringValues("no")

        try
            for i in 1..10 do
                let payload =
                    sprintf "{\"count\":%d,\"value\":%d,\"message\":\"Event %d of 10\",\"timestamp\":\"%s\"}"
                        i (rng.Next(1, 100)) i (DateTime.UtcNow.ToString("o"))
                let bytes = Encoding.UTF8.GetBytes($"data: {payload}\n\n")
                do! ctx.Response.Body.WriteAsync(bytes, 0, bytes.Length)
                do! ctx.Response.Body.FlushAsync()
                if i < 10 then do! Task.Delay(1000)
        with _ -> ()   // client disconnected mid-stream

        return Some ctx
    }
