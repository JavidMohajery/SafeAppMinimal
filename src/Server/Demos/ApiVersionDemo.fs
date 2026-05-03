module Demos.ApiVersionDemo

open Giraffe

let v1 : HttpHandler =
    setHttpHeader "Deprecation" "true"
    >=> setHttpHeader "Sunset" "Sat, 01 Jan 2026 00:00:00 GMT"
    >=> setHttpHeader "Link" "</api/v2/demo/version/greet>; rel=\"successor-version\""
    >=> json {|
        greeting = "Hello from v1!"
        user     = "World"
        version  = 1
    |}

let v2 : HttpHandler =
    json {|
        message = {|
            text   = "Hello from v2!"
            locale = "en-US"
            format = "informal"
        |}
        user = {|
            id   = 42
            name = "World"
        |}
        meta = {|
            version     = 2
            releaseDate = "2025-06-01"
            deprecated  = false
        |}
    |}
