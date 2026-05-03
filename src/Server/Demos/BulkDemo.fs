module Demos.BulkDemo

open System.Text.Json
open Giraffe

[<CLIMutable>]
type BulkItem = {
    id    : string
    name  : string
    value : float
}

let bulkImport : HttpHandler = fun next ctx -> task {
    let! body  = ctx.ReadBodyFromRequestAsync()
    let  opts  = JsonSerializerOptions(PropertyNameCaseInsensitive = true)
    let  items = JsonSerializer.Deserialize<BulkItem[]>(body, opts)
    let results =
        items |> Array.mapi (fun i item ->
            let errs = [
                if System.String.IsNullOrWhiteSpace item.name then "name is required"
                if item.value < 0.0                           then "value must be ≥ 0"
            ]
            {| index  = i
               id     = item.id
               status = if errs.IsEmpty then 201 else 422
               ok     = errs.IsEmpty
               errors = errs |})
    let nOk  = results |> Array.sumBy (fun r -> if r.ok then 1 else 0)
    let nErr = results |> Array.sumBy (fun r -> if r.ok then 0 else 1)
    let code = if nOk > 0 && nErr > 0 then 207 elif nErr > 0 then 422 else 201
    return! (setStatusCode code >=> json {|
        results = results
        summary = {| created = nOk; failed = nErr |}
    |}) next ctx
}
