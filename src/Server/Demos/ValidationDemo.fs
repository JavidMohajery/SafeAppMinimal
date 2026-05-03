module Demos.ValidationDemo

open System
open Giraffe

[<CLIMutable>]
type UserRequest = { name: string; email: string; age: int }

let private validEmail (s: string) =
    not (String.IsNullOrWhiteSpace(s)) &&
    s.Contains("@") &&
    s.LastIndexOf('.') > s.IndexOf('@') + 1

let post : HttpHandler = fun next ctx -> task {
    try
        let! req = ctx.BindJsonAsync<UserRequest>()
        let errors = [|
            if String.IsNullOrWhiteSpace(req.name)  then yield {| field = "name";  message = "Name is required" |}
            if not (validEmail req.email)             then yield {| field = "email"; message = "Must be a valid email address" |}
            if req.age < 1 || req.age > 120           then yield {| field = "age";   message = "Age must be between 1 and 120" |}
        |]
        if errors.Length = 0 then
            return!
                (setStatusCode 200
                 >=> json {| valid = true; normalized = {| name = req.name.Trim(); email = req.email.Trim().ToLower(); age = req.age |} |}) next ctx
        else
            return!
                (setStatusCode 422
                 >=> json {| valid = false; errors = errors |}) next ctx
    with _ ->
        return!
            (setStatusCode 400 >=> json {| valid = false; errors = [| {| field = "body"; message = "Invalid or missing JSON body" |} |] |}) next ctx
}
