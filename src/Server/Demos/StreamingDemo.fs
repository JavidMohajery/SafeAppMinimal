module Demos.StreamingDemo

open System
open System.Text
open System.Threading.Tasks
open Giraffe
open Microsoft.Extensions.Primitives

let stream : HttpHandler = fun next ctx -> task {
    ctx.Response.ContentType <- "text/plain; charset=utf-8"
    // Tell the response-compression middleware not to buffer this response.
    ctx.Response.Headers["Content-Encoding"] <- StringValues("identity")
    ctx.Response.Headers["Cache-Control"]    <- StringValues("no-cache")
    ctx.Response.Headers["X-Accel-Buffering"]<- StringValues("no")

    try
        let header = Encoding.UTF8.GetBytes("row,timestamp,sensor_id,value\r\n")
        do! ctx.Response.Body.WriteAsync(header, 0, header.Length)
        do! ctx.Response.Body.FlushAsync()

        for i in 1..20 do
            let line =
                sprintf "%d,%s,sensor-%02d,%d\r\n"
                    i (DateTime.UtcNow.ToString("o")) (i % 5 + 1) (Random.Shared.Next(0, 1000))
            let bytes = Encoding.UTF8.GetBytes(line)
            do! ctx.Response.Body.WriteAsync(bytes, 0, bytes.Length)
            do! ctx.Response.Body.FlushAsync()
            if i < 20 then do! Task.Delay(150)
    with _ -> ()

    return Some ctx
}
