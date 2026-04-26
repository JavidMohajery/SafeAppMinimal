module Pages.CounterDom

open Browser
open Browser.Types
open Fable.React
open Fable.React.Props

let private setup (container: Element) =
    let mutable count = 0

    let card = document.createElement "div"
    card.className <- "card"

    let title = document.createElement "h2"
    title.textContent <- "Counter — DOM Manipulation"
    card.appendChild title |> ignore

    let display = document.createElement "div"
    display.className <- "counter-value"
    display.textContent <- "0"
    card.appendChild display |> ignore

    let controls = document.createElement "div"
    controls.className <- "controls"
    let decBtn = document.createElement "button"
    decBtn.textContent <- "−"
    let incBtn = document.createElement "button"
    incBtn.textContent <- "+"
    controls.appendChild decBtn |> ignore
    controls.appendChild incBtn |> ignore
    card.appendChild controls |> ignore

    let resetWrap = document.createElement "div"
    resetWrap.setAttribute ("style", "margin-top:1rem")
    let resetBtn = document.createElement "button"
    resetBtn.className <- "reset-btn"
    resetBtn.textContent <- "Reset"
    resetWrap.appendChild resetBtn |> ignore
    card.appendChild resetWrap |> ignore

    let section = document.createElement "div"
    section.className <- "items-section"
    let sectionTitle = document.createElement "h3"
    sectionTitle.textContent <- "History"
    section.appendChild sectionTitle |> ignore
    let list = document.createElement "ul"
    section.appendChild list |> ignore
    card.appendChild section |> ignore

    container.appendChild card |> ignore

    let addEntry label =
        let li = document.createElement "li"
        li.textContent <- label
        list.insertBefore (li, list.firstChild) |> ignore

    let tick delta =
        count <- count + delta
        display.textContent <- string count
        addEntry $"""{(if delta > 0 then "+" else "")}{delta} → {count}"""

    let reset () =
        count <- 0
        display.textContent <- "0"
        list.innerHTML <- ""

    incBtn.addEventListener ("click", fun _ -> tick 1)
    decBtn.addEventListener ("click", fun _ -> tick -1)
    resetBtn.addEventListener ("click", fun _ -> reset ())

// Guard against React calling the ref callback again for the same mounted element.
let view () =
    div [ Ref (fun el -> if not (isNull el) && el.childNodes.length = 0 then setup el) ] []
