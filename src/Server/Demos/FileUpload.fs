module Demos.FileUpload

open System
open System.Text
open Giraffe

let upload : HttpHandler = fun next ctx ->
    task {
        if not ctx.Request.HasFormContentType then
            return!
                (setStatusCode 400 >=>
                    json {| error = "Expected multipart/form-data"; hint = "Use a <form> or FormData with method POST" |})
                    next ctx
        else
            let files = ctx.Request.Form.Files
            if files.Count = 0 then
                return!
                    (setStatusCode 400 >=> json {| error = "No file found in request" |}) next ctx
            else
                let results =
                    [| for file in files do
                        use stream      = file.OpenReadStream()
                        let previewLen  = int (min file.Length 128L)
                        let previewBuf  = Array.zeroCreate<byte> previewLen
                        stream.Read(previewBuf, 0, previewLen) |> ignore
                        let isText      = file.ContentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase)
                        let preview     =
                            if isText then Encoding.UTF8.GetString(previewBuf)
                            else previewBuf |> Array.map (sprintf "%02x") |> String.concat " "
                        yield {|
                            name        = file.FileName
                            size        = file.Length
                            contentType = if String.IsNullOrEmpty file.ContentType then "application/octet-stream" else file.ContentType
                            previewType = if isText then "text" else "hex"
                            preview     = preview
                        |} |]
                return! (setStatusCode 200 >=> json results) next ctx
    }
