module Demos.PatchDemo

open System.Text.Json
open Giraffe

[<CLIMutable>]
type Profile = {
    name    : string
    email   : string
    bio     : string
    website : string
    version : int
}

let mutable private profile = {
    name    = "Alice Smith"
    email   = "alice@example.com"
    bio     = "Software engineer and open-source contributor."
    website = "https://alice.dev"
    version = 1
}

let private tryStr (root: JsonElement) (key: string) =
    let mutable v = Unchecked.defaultof<JsonElement>
    if root.TryGetProperty(key, &v) && v.ValueKind = JsonValueKind.String
    then Some (v.GetString())
    else None

let get : HttpHandler = json profile

let patch : HttpHandler = fun next ctx -> task {
    let! body = ctx.ReadBodyFromRequestAsync()
    use  doc  = JsonDocument.Parse body
    let  root = doc.RootElement
    let updated = {
        name    = tryStr root "name"    |> Option.defaultValue profile.name
        email   = tryStr root "email"   |> Option.defaultValue profile.email
        bio     = tryStr root "bio"     |> Option.defaultValue profile.bio
        website = tryStr root "website" |> Option.defaultValue profile.website
        version = profile.version + 1
    }
    profile <- updated
    return! json updated next ctx
}
