module Demos.SoftDeleteDemo

open System
open System.Collections.Concurrent
open System.Text.Json
open Giraffe

type private Item = { id: int; name: string; deletedAt: DateTime option }

let private store = ConcurrentDictionary<int, Item>()
let mutable private nextId = 5

do
    for (id, name) in [ 1,"Alpha"; 2,"Beta"; 3,"Gamma"; 4,"Delta" ] do
        store.[id] <- { id = id; name = name; deletedAt = None }

let private toDto (i: Item) =
    {| id        = i.id
       name      = i.name
       deleted   = i.deletedAt.IsSome
       deletedAt = i.deletedAt |> Option.map (fun d -> d.ToString "o") |> Option.defaultValue null |}

let getAll : HttpHandler = fun next ctx ->
    let incDel =
        match ctx.TryGetQueryStringValue "includeDeleted" with
        | Some v -> v = "true"
        | None   -> false
    let items =
        store.Values
        |> Seq.filter (fun i -> incDel || i.deletedAt.IsNone)
        |> Seq.sortBy  (fun i -> i.id)
        |> Seq.map toDto
        |> Seq.toArray
    json items next ctx

let add : HttpHandler = fun next ctx -> task {
    let! body = ctx.ReadBodyFromRequestAsync()
    use  doc  = JsonDocument.Parse body
    let  name = doc.RootElement.GetProperty("name").GetString()
    let  id   = nextId
    nextId <- nextId + 1
    store.[id] <- { id = id; name = name; deletedAt = None }
    return! (setStatusCode 201 >=> json (toDto store.[id])) next ctx
}

let delete (id: int) : HttpHandler = fun next ctx ->
    match store.TryGetValue id with
    | false, _ ->
        (setStatusCode 404 >=> json {| error = "Not found" |}) next ctx
    | true, item when item.deletedAt.IsSome ->
        (setStatusCode 409 >=> json {| error = "Already deleted" |}) next ctx
    | true, item ->
        store.[id] <- { item with deletedAt = Some DateTime.UtcNow }
        json (toDto store.[id]) next ctx

let restore (id: int) : HttpHandler = fun next ctx ->
    match store.TryGetValue id with
    | false, _ ->
        (setStatusCode 404 >=> json {| error = "Not found" |}) next ctx
    | true, item when item.deletedAt.IsNone ->
        (setStatusCode 409 >=> json {| error = "Not deleted" |}) next ctx
    | true, item ->
        store.[id] <- { item with deletedAt = None }
        json (toDto store.[id]) next ctx
