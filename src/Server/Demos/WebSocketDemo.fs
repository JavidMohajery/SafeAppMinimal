module Demos.WebSocketDemo

open System
open System.Net.WebSockets
open System.Text
open System.Threading
open Giraffe

// ── Echo loop ─────────────────────────────────────────────────────────────────

let private echoLoop (ws: WebSocket) =
    task {
        let buf = Array.zeroCreate<byte> 4096
        let seg = ArraySegment<byte>(buf)
        let mutable count   = 0
        let mutable running = true
        while running && ws.State = WebSocketState.Open do
            let! result = ws.ReceiveAsync(seg, CancellationToken.None)
            match result.MessageType with
            | WebSocketMessageType.Close ->
                do! ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", CancellationToken.None)
                running <- false
            | WebSocketMessageType.Text ->
                let text = Encoding.UTF8.GetString(buf, 0, result.Count)
                count <- count + 1
                let escaped = text.Replace("\\", "\\\\").Replace("\"", "\\\"")
                let reply =
                    sprintf "{\"echo\":\"%s\",\"count\":%d,\"length\":%d,\"timestamp\":\"%s\"}"
                        escaped count text.Length (DateTime.UtcNow.ToString("o"))
                let replyBytes = Encoding.UTF8.GetBytes(reply)
                do! ws.SendAsync(ArraySegment<byte>(replyBytes), WebSocketMessageType.Text, true, CancellationToken.None)
            | _ -> ()
    }

// ── Handler ───────────────────────────────────────────────────────────────────

let handler : HttpHandler = fun next ctx ->
    task {
        if ctx.WebSockets.IsWebSocketRequest then
            let! ws = ctx.WebSockets.AcceptWebSocketAsync()
            do! echoLoop ws
            return Some ctx
        else
            return!
                (setStatusCode 426
                 >=> setHttpHeader "Upgrade" "websocket"
                 >=> text "This endpoint requires a WebSocket connection") next ctx
    }
