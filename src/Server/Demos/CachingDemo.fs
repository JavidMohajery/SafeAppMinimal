module Demos.CachingDemo

open System
open System.Text
open System.Text.Json
open System.Security.Cryptography
open Giraffe

[<CLIMutable>]
type Resource = { id: int; value: int; updatedAt: string }

let mutable private resource =
    { id = 1; value = Random.Shared.Next(100, 999); updatedAt = DateTime.UtcNow.ToString("o") }

let private computeEtag (r: Resource) =
    let hash = SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(r)))
    "\"" + Convert.ToBase64String(hash).[..7] + "\""

let get : HttpHandler = fun next ctx ->
    let etag = computeEtag resource
    let inm  =
        match ctx.Request.Headers.TryGetValue("If-None-Match") with
        | true, v -> v.ToString()
        | _       -> ""
    if inm <> "" && inm = etag then
        setStatusCode 304 next ctx
    else
        (setHttpHeader "ETag" etag
         >=> setHttpHeader "Cache-Control" "max-age=10, must-revalidate"
         >=> json resource) next ctx

let mutate : HttpHandler = fun next ctx ->
    resource <- { id = 1; value = Random.Shared.Next(100, 999); updatedAt = DateTime.UtcNow.ToString("o") }
    json {| updated = true; newValue = resource.value |} next ctx
