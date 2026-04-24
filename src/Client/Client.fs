module Client

open Browser
open Browser.Types

let counter = document.getElementById "counter-display"
let itemList = document.getElementById "item-list"

let mutable count = 0

let addHistoryEntry (label: string) =
    let li = document.createElement "li"
    li.textContent <- label
    itemList.insertBefore(li, itemList.firstChild) |> ignore

let update delta =
    count <- count + delta
    counter.textContent <- string count
    let sign = if delta > 0 then "+" else ""
    addHistoryEntry $"{sign}{delta} → {count}"

let reset () =
    count <- 0
    counter.textContent <- "0"
    itemList.innerHTML <- ""

(document.getElementById "inc-btn").addEventListener("click", fun _ -> update 1)
(document.getElementById "dec-btn").addEventListener("click", fun _ -> update -1)
(document.getElementById "reset-btn").addEventListener("click", fun _ -> reset ())
