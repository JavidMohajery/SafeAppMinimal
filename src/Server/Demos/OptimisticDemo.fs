module Demos.OptimisticDemo

open System
open System.Text
open System.Text.Json
open System.Security.Cryptography
open Giraffe
open Microsoft.Extensions.Primitives

[<CLIMutable>]
type Doc = { version: int; content: string; updatedAt: string; updatedBy: string }

[<CLIMutable>]
type PutRequest = { content: string; author: string }

let mutable private doc =
    { version = 1; content = "The quick brown fox jumps over the lazy dog."; updatedAt = DateTime.UtcNow.ToString("o"); updatedBy = "system" }

let private etag (d: Doc) =
    let h = SHA256.HashData(Encoding.UTF8.GetBytes($"{d.version}:{d.content}"))
    "\"" + Convert.ToBase64String(h).[..7] + "\""

let getDoc : HttpHandler = fun next ctx ->
    let e = etag doc
    (setHttpHeader "ETag" e >=> json doc) next ctx

let putDoc : HttpHandler = fun next ctx -> task {
    let ifMatch =
        match ctx.Request.Headers.TryGetValue("If-Match") with
        | true, v -> v.ToString()
        | _       -> ""
    if ifMatch = "" then
        return!
            (setStatusCode 428
             >=> json {| error = "Precondition Required"
                         detail = "Include If-Match header with the current ETag to prevent overwriting concurrent edits." |}) next ctx
    elif ifMatch <> etag doc then
        return!
            (setStatusCode 412
             >=> json {| error = "Precondition Failed"
                         detail = "The document was modified by another client since you fetched it. Re-fetch and retry." |}) next ctx
    else
        let! req = ctx.BindJsonAsync<PutRequest>()
        doc <- { version = doc.version + 1; content = req.content; updatedAt = DateTime.UtcNow.ToString("o"); updatedBy = req.author }
        let newEtag = etag doc
        return! (setHttpHeader "ETag" newEtag >=> json doc) next ctx
}

let forceUpdate : HttpHandler = fun next ctx ->
    doc <- { doc with version = doc.version + 1; content = "Another client edited this — " + Guid.NewGuid().ToString("N").[..5]; updatedAt = DateTime.UtcNow.ToString("o"); updatedBy = "other-client" }
    json {| message = "Document mutated by another client"; newVersion = doc.version |} next ctx
