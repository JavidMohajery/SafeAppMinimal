module Demos.ContentNegDemo

open Giraffe

[<CLIMutable>]
type Product = { id: int; name: string; price: float; category: string }

let private products = [|
    { id = 1; name = "Widget A"; price = 9.99;  category = "widgets" }
    { id = 2; name = "Widget B"; price = 14.99; category = "widgets" }
    { id = 3; name = "Gadget X"; price = 29.99; category = "gadgets" }
    { id = 4; name = "Gadget Y"; price = 49.99; category = "gadgets" }
|]

let private toCsv () =
    let rows = products |> Array.map (fun p -> $"{p.id},{p.name},{p.price},{p.category}")
    String.concat "\r\n" (Array.append [| "id,name,price,category" |] rows)

let private toText () =
    products
    |> Array.map (fun p -> $"[{p.id}] {p.name,-12}  ${p.price,6:F2}  ({p.category})")
    |> String.concat "\n"

let get : HttpHandler = fun next ctx ->
    let accept =
        match ctx.Request.Headers.TryGetValue("Accept") with
        | true, v -> v.ToString()
        | _       -> ""
    if accept.Contains("text/csv") then
        (setHttpHeader "Content-Type" "text/csv; charset=utf-8"
         >=> setBodyFromString (toCsv())) next ctx
    elif accept.Contains("text/plain") then
        (setHttpHeader "Content-Type" "text/plain; charset=utf-8"
         >=> setBodyFromString (toText())) next ctx
    else
        (setHttpHeader "Content-Type" "application/json; charset=utf-8"
         >=> json products) next ctx
