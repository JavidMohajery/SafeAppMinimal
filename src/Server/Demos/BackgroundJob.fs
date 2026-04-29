module Demos.BackgroundJob

open System
open System.Collections.Concurrent
open System.Threading
open Giraffe

// ── Job model ─────────────────────────────────────────────────────────────────

type private Job = {
    Id              : string
    mutable Status  : string   // queued | running | completed | failed
    mutable Progress: int      // 0–100
    mutable Result  : string
    CreatedAt       : DateTime
    mutable Done    : DateTime option
}

let private jobs = ConcurrentDictionary<string, Job>()

let private toDto (j: Job) =
    {| id          = j.Id
       status      = j.Status
       progress    = j.Progress
       result      = j.Result
       createdAt   = j.CreatedAt.ToString("o")
       completedAt = j.Done |> Option.map (fun d -> d.ToString("o")) |> Option.defaultValue "" |}

// ── Background worker ─────────────────────────────────────────────────────────

let private runInBackground (id: string) =
    Tasks.Task.Run(fun () ->
        Thread.Sleep(300)
        jobs.[id].Status   <- "running"
        jobs.[id].Progress <- 0
        for step in 1..5 do
            Thread.Sleep(800)
            jobs.[id].Progress <- step * 20
        Thread.Sleep(200)
        jobs.[id].Status <- "completed"
        jobs.[id].Result <- "Processed 5 steps — all done"
        jobs.[id].Done   <- Some DateTime.UtcNow)
    |> ignore

// ── Handlers ──────────────────────────────────────────────────────────────────

let start : HttpHandler = fun next ctx ->
    task {
        let id  = Guid.NewGuid().ToString("N").[..7]
        let job = { Id = id; Status = "queued"; Progress = 0; Result = ""; CreatedAt = DateTime.UtcNow; Done = None }
        jobs.[id] <- job
        runInBackground id
        return!
            (setStatusCode 202 >=>
                json {| id = id; status = "queued"; message = "Job accepted — poll /api/demo/jobs/{id} for status" |})
                next ctx
    }

let getStatus (id: string) : HttpHandler = fun next ctx ->
    match jobs.TryGetValue(id) with
    | true, job -> json (toDto job) next ctx
    | _ ->
        (setStatusCode 404 >=> json {| error = "Job not found"; id = id |}) next ctx

let getAll : HttpHandler = fun next ctx ->
    let dtos =
        jobs.Values
        |> Seq.sortByDescending (fun j -> j.CreatedAt)
        |> Seq.map toDto
        |> Seq.toArray
    json dtos next ctx
