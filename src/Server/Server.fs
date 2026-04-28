module Server

open Giraffe
open Saturn
open Shared

let healthHandler : HttpHandler =
    fun next ctx ->
        let resp = {|
            status      = "healthy"
            timestamp   = System.DateTime.UtcNow.ToString("o")
            uptime_ms   = System.Environment.TickCount64
            environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                          |> Option.ofObj
                          |> Option.defaultValue "Production"
        |}
        json resp next ctx

let webApp =
    router {
        get Route.hello  (text "Hello from SAFE!")
        get Route.health healthHandler
    }

let app =
    application {
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

run app
