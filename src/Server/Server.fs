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

        // File upload
        post Route.fileUpload Demos.FileUpload.upload

        // Server-Sent Events
        get Route.sseStream Demos.SseDemo.stream

        // Correlation ID
        get Route.correlationPing Demos.CorrelationDemo.ping

        // Optimistic concurrency
        get  Route.optimisticDoc   Demos.OptimisticDemo.getDoc
        put  Route.optimisticDoc   Demos.OptimisticDemo.putDoc
        post Route.optimisticForce Demos.OptimisticDemo.forceUpdate

        // Long polling
        get  Route.longPollWait    Demos.LongPollDemo.wait
        post Route.longPollPublish Demos.LongPollDemo.publish

        // Retry with exponential backoff
        get  Route.retryFlaky Demos.RetryDemo.flaky
        post Route.retryReset Demos.RetryDemo.reset

        // PATCH / Partial Update
        get   Route.patchProfile Demos.PatchDemo.get
        patch Route.patchProfile Demos.PatchDemo.patch

        // API Versioning
        get Route.versionV1 Demos.ApiVersionDemo.v1
        get Route.versionV2 Demos.ApiVersionDemo.v2

        // Bulk Operations (207 Multi-Status)
        post Route.bulkItems Demos.BulkDemo.bulkImport

        // Soft Delete & Restore
        get     Route.softDeleteItems                           Demos.SoftDeleteDemo.getAll
        post    Route.softDeleteItems                           Demos.SoftDeleteDemo.add
        deletef "/api/demo/softdelete/items/%i"                 Demos.SoftDeleteDemo.delete
        postf   "/api/demo/softdelete/items/%i/restore"         Demos.SoftDeleteDemo.restore

        // Circuit Breaker
        get  Route.cbCall  Demos.CircuitBreakerDemo.call
        post Route.cbReset Demos.CircuitBreakerDemo.reset

        // Structured Logging
        post   Route.logWrite   Demos.LoggingDemo.write
        get    Route.logEntries Demos.LoggingDemo.getAll
        delete Route.logClear   Demos.LoggingDemo.clear

        // API key auth
        get Route.apiKeyResource Demos.ApiKeyDemo.resource

        // Streaming download (chunked transfer)
        get Route.streamData Demos.StreamingDemo.stream

        // Idempotency keys
        post Route.idempotent Demos.IdempotencyDemo.placeOrder

        // Response caching (ETag + conditional GET)
        get  Route.cacheDemo Demos.CachingDemo.get
        post Route.cacheDemo Demos.CachingDemo.mutate

        // Content negotiation (JSON / CSV / plain text)
        get Route.contentNeg Demos.ContentNegDemo.get

        // Request validation (422 structured errors)
        post Route.validate Demos.ValidationDemo.post

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
