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
