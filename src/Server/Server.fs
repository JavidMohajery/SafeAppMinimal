module Server

open Giraffe
open Saturn
open Shared
open Microsoft.AspNetCore.Builder

// ── Health check ──────────────────────────────────────────────────────────────

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

// ── Error pattern demos ───────────────────────────────────────────────────────

let private errDemo (code: int) (title: string) (detail: string) : HttpHandler =
    fun next ctx ->
        let body = {|
            status    = code
            error     = title
            detail    = detail
            timestamp = System.DateTime.UtcNow.ToString("o")
            path      = ctx.Request.Path.Value
        |}
        (setStatusCode code >=> json body) next ctx

// ── Router ────────────────────────────────────────────────────────────────────

let webApp =
    router {
        // Original endpoints
        get Route.hello  (text "Hello from SAFE!")
        get Route.health healthHandler

        // Error pattern demos
        get Route.err400 (errDemo 400 "Bad Request"           "The request is malformed or missing required fields.")
        get Route.err401 (errDemo 401 "Unauthorized"          "Authentication is required. Provide a valid Bearer token.")
        get Route.err403 (errDemo 403 "Forbidden"             "You do not have permission to access this resource.")
        get Route.err404 (errDemo 404 "Not Found"             "The requested resource does not exist on this server.")
        get Route.err422 (errDemo 422 "Unprocessable Entity"  "Validation failed — the input values are semantically invalid.")
        get Route.err429 (errDemo 429 "Too Many Requests"     "Rate limit exceeded. Retry after 60 seconds.")
        get Route.err500 (errDemo 500 "Internal Server Error" "An unexpected error occurred. The server could not process your request.")

        // JWT auth
        post Route.authLogin Demos.AuthDemo.login
        get  Route.authMe    Demos.AuthDemo.whoami

        // Rate limiting
        get Route.rateLimit Demos.RateLimit.ping

        // Background jobs
        getf "/api/demo/jobs/%s" Demos.BackgroundJob.getStatus
        get  Route.jobsBase      Demos.BackgroundJob.getAll
        post Route.jobsBase      Demos.BackgroundJob.start

        // WebSocket echo
        get Route.wsEcho Demos.WebSocketDemo.handler

        // CRUD item API
        get     Route.items        Demos.CrudApi.getAll
        get     Route.itemsPaged   Demos.CrudApi.getPage
        post    Route.items        Demos.CrudApi.create
        getf    "/api/items/%i"    Demos.CrudApi.getById
        putf    "/api/items/%i"    Demos.CrudApi.update
        deletef "/api/items/%i"    Demos.CrudApi.delete
    }

let app =
    application {
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
        app_config (fun app -> app.UseWebSockets())
    }

run app
