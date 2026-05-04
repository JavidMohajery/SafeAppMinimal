namespace Shared

module Route =
    // Original
    let hello  = "/api/hello"
    let health = "/api/health"

    // Error pattern demos
    let err400 = "/api/demo/error/400"
    let err401 = "/api/demo/error/401"
    let err403 = "/api/demo/error/403"
    let err404 = "/api/demo/error/404"
    let err422 = "/api/demo/error/422"
    let err429 = "/api/demo/error/429"
    let err500 = "/api/demo/error/500"

    // CRUD items
    let items      = "/api/items"
    let itemsPaged = "/api/items/paged"

    // JWT auth
    let authLogin = "/api/demo/auth/login"
    let authMe    = "/api/demo/auth/me"

    // Rate limiting
    let rateLimit = "/api/demo/ratelimit/ping"

    // Background jobs
    let jobsBase = "/api/demo/jobs"

    // WebSocket echo
    let wsEcho = "/api/demo/ws/echo"

    // File upload
    let fileUpload = "/api/demo/upload"

    // Server-Sent Events
    let sseStream = "/api/demo/sse/events"

    // Response caching
    let cacheDemo = "/api/demo/cache"

    // Content negotiation
    let contentNeg = "/api/demo/content-neg"

    // Request validation
    let validate = "/api/demo/validate"

    // API key auth
    let apiKeyResource = "/api/demo/apikey/resource"

    // Streaming download
    let streamData = "/api/demo/stream/data"

    // Idempotency keys
    let idempotent = "/api/demo/idempotent/order"

    // Correlation ID
    let correlationPing = "/api/demo/correlation/ping"

    // Optimistic concurrency
    let optimisticDoc   = "/api/demo/optimistic/doc"
    let optimisticForce = "/api/demo/optimistic/force"

    // Long polling
    let longPollWait    = "/api/demo/longpoll/wait"
    let longPollPublish = "/api/demo/longpoll/publish"

    // Retry with backoff
    let retryFlaky = "/api/demo/retry/flaky"
    let retryReset = "/api/demo/retry/reset"

    // PATCH / Partial Update
    let patchProfile = "/api/demo/patch/profile"

    // API Versioning
    let versionV1 = "/api/v1/demo/version/greet"
    let versionV2 = "/api/v2/demo/version/greet"

    // Bulk Operations
    let bulkItems = "/api/demo/bulk/items"

    // Soft Delete
    let softDeleteItems = "/api/demo/softdelete/items"

    // Circuit Breaker
    let cbCall  = "/api/demo/circuitbreaker/call"
    let cbReset = "/api/demo/circuitbreaker/reset"

    // Structured Logging
    let logWrite   = "/api/demo/logging/write"
    let logEntries = "/api/demo/logging/entries"
    let logClear   = "/api/demo/logging/clear"

    // Request Timeout / Cancellation
    let timeoutSlow = "/api/demo/timeout/slow"
