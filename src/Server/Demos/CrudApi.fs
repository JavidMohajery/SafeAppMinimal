module Demos.CrudApi

open System.Collections.Concurrent
open Giraffe

[<CLIMutable>]
type Item = { Id: int; Title: string; Completed: bool }

[<CLIMutable>]
type ItemDto = { Title: string; Completed: bool }

// ── In-memory store ───────────────────────────────────────────────────────────

let private _data =
    let d = ConcurrentDictionary<int, Item>()
    d.[1] <- { Id = 1; Title = "Build the UI components"; Completed = true }
    d.[2] <- { Id = 2; Title = "Add backend endpoints";   Completed = false }
    d.[3] <- { Id = 3; Title = "Write unit tests";        Completed = false }
    d.[4] <- { Id = 4; Title = "Deploy to production";    Completed = false }
    d.[5] <- { Id = 5; Title = "Write documentation";     Completed = false }
    d

let mutable private _counter = 5

let private nextId () = System.Threading.Interlocked.Increment(&_counter)

// ── Handlers ──────────────────────────────────────────────────────────────────

let getAll : HttpHandler = fun next ctx ->
    let items = _data.Values |> Seq.sortBy (fun i -> i.Id) |> Seq.toArray
    json items next ctx

let getById (id: int) : HttpHandler = fun next ctx ->
    match _data.TryGetValue id with
    | true, item -> json item next ctx
    | _ ->
        (setStatusCode 404 >=> json {| error = "Item not found"; id = id |}) next ctx

let create : HttpHandler = fun next ctx ->
    task {
        let! dto = ctx.BindJsonAsync<ItemDto>()
        let item = { Id = nextId(); Title = dto.Title; Completed = dto.Completed }
        _data.[item.Id] <- item
        return! (setStatusCode 201 >=> json item) next ctx
    }

let update (id: int) : HttpHandler = fun next ctx ->
    task {
        let! dto = ctx.BindJsonAsync<ItemDto>()
        if _data.ContainsKey id then
            let item = { Id = id; Title = dto.Title; Completed = dto.Completed }
            _data.[id] <- item
            return! json item next ctx
        else
            return!
                (setStatusCode 404 >=> json {| error = "Item not found"; id = id |})
                    next ctx
    }

let delete (id: int) : HttpHandler = fun next ctx ->
    match _data.TryRemove id with
    | true, _ -> setStatusCode 204 next ctx
    | _ ->
        (setStatusCode 404 >=> json {| error = "Item not found"; id = id |}) next ctx
