module Client

open Browser
open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props

// Register all Web Components with the browser before React starts rendering.
do ComponentRegistry.registerAll()

type Page = Home | CounterElmish | CounterDom | Component of string * string

type Model = {
    Page          : Page
    Hash          : string
    ElmishCounter : Pages.CounterElmish.Model
}

type Msg =
    | UrlChanged of string
    | ElmishCounterMsg of Pages.CounterElmish.Msg

let parsePage (hash: string) =
    match hash.TrimStart('#').TrimStart('/').Split('/') |> Array.toList with
    | "counter-elmish" :: _ -> CounterElmish
    | "counter-dom"    :: _ -> CounterDom
    | "component" :: cat :: slug :: _ -> Component(cat, slug)
    | _ -> Home

let init () =
    let counter, _ = Pages.CounterElmish.init ()
    let hash = window.location.hash
    { Page = parsePage hash; Hash = hash; ElmishCounter = counter }, Cmd.none

let update msg model =
    match msg with
    | UrlChanged hash ->
        { model with Page = parsePage hash; Hash = hash }, Cmd.none
    | ElmishCounterMsg sub ->
        let m, cmd = Pages.CounterElmish.update sub model.ElmishCounter
        { model with ElmishCounter = m }, Cmd.map ElmishCounterMsg cmd

// Elmish 4 subscription: hashchange listener.
let hashSub (_model: Model) : Sub<Msg> =
    [ ["hash"], fun dispatch ->
        window.addEventListener("hashchange", fun _ ->
            dispatch (UrlChanged window.location.hash))
        { new System.IDisposable with member _.Dispose() = () } ]

// ── Helpers ───────────────────────────────────────────────────────────────────

/// Render a <fui-*> Web Component inside the React tree.
/// attrs: list of (attribute-name, value) pairs.
let wc (tag: string) (attrs: (string * string) list) (children: ReactElement list) : ReactElement =
    domEl tag (attrs |> List.map (fun (k, v) -> HTMLAttr.Custom(k, box v))) children

// ── Sidebar ───────────────────────────────────────────────────────────────────

let sidebarLink (label: string) (href: string) (currentHash: string) =
    let isActive =
        href = currentHash ||
        (href = "#/" && (currentHash = "" || currentHash = "#"))
    a [
        ClassName (if isActive then "sidebar-link active" else "sidebar-link")
        Href href
    ] [ str label ]

let sidebar (model: Model) =
    aside [ ClassName "sidebar" ] [
        div [ ClassName "sidebar-logo" ] [
            img [ Src "/favicon.png"; Alt "logo" ]
            span [] [
                str "Fable"
                span [ ClassName "logo-accent" ] [ str "UI" ]
            ]
        ]
        nav [ ClassName "sidebar-nav" ] [
            div [ ClassName "sidebar-group" ] [
                sidebarLink "Home" "#/" model.Hash
            ]
            div [ ClassName "sidebar-group" ] [
                div [ ClassName "sidebar-group-label" ] [ str "Inputs & Forms" ]
                sidebarLink "Button"   "#/component/inputs/button"   model.Hash
                sidebarLink "Input"   "#/component/inputs/input"    model.Hash
                sidebarLink "Textarea" "#/component/inputs/textarea" model.Hash
                sidebarLink "Select"    "#/component/inputs/select"   model.Hash
                sidebarLink "Checkbox"    "#/component/inputs/checkbox"    model.Hash
                sidebarLink "Radio"      "#/component/inputs/radio"       model.Hash
                sidebarLink "RadioGroup" "#/component/inputs/radio-group" model.Hash
            ]
            div [ ClassName "sidebar-group" ] [
                div [ ClassName "sidebar-group-label" ] [ str "Examples" ]
                sidebarLink "Counter — Elmish" "#/counter-elmish" model.Hash
                sidebarLink "Counter — DOM"    "#/counter-dom"    model.Hash
            ]
        ]
    ]

// ── Topbar ────────────────────────────────────────────────────────────────────

let breadcrumbItems (page: Page) : (string * bool) list =
    match page with
    | Home          -> [ "Home", true ]
    | CounterElmish -> [ "Home", false; "Examples", false; "Counter — Elmish", true ]
    | CounterDom    -> [ "Home", false; "Examples", false; "Counter — DOM", true ]
    | Component(_, slug) ->
        let name = ComponentRegistry.bySlug slug |> Option.map (fun m -> m.Name) |> Option.defaultValue slug
        [ "Home", false; "Components", false; name, true ]

let topbar (model: Model) =
    let crumbs = breadcrumbItems model.Page
    header [ ClassName "topbar" ] [
        div [ ClassName "breadcrumb" ] [
            yield!
                crumbs
                |> List.mapi (fun i (label, isActive) ->
                    [
                        if i > 0 then
                            yield span [ ClassName "breadcrumb-sep"; Key $"sep-{i}" ] [ str "/" ]
                        yield span [ Key (string i); ClassName (if isActive then "breadcrumb-active" else "") ] [ str label ]
                    ])
                |> List.concat
        ]
        div [ ClassName "topbar-actions" ] [
            a [ ClassName "icon-btn"; Href "https://github.com/safe-stack/SAFE-template"; Title "GitHub" ] [ str "GH" ]
        ]
    ]

// ── Pages ─────────────────────────────────────────────────────────────────────

let homePage =
    div [ ClassName "home-welcome" ] [
        h1 [] [
            str "Fable"
            span [ ClassName "hi" ] [ str "UI" ]
        ]
        p [] [ str "A comprehensive reference of web UI components and backend patterns." ]
        p [] [ str "Every component is a standalone Web Component — drop it into any HTML page." ]

        div [ ClassName "component-preview" ] [
            div [ ClassName "preview-header" ] [
                span [ ClassName "preview-tag" ] [ str "fui-button" ]
                span [ ClassName "preview-badge" ] [ str "Inputs & Forms" ]
            ]
            div [ ClassName "preview-body" ] [
                div [ ClassName "preview-row" ] [
                    wc "fui-button" [ "variant", "primary"   ] [ str "Save changes" ]
                    wc "fui-button" [ "variant", "secondary" ] [ str "Cancel" ]
                    wc "fui-button" [ "variant", "ghost"     ] [ str "Menu" ]
                    wc "fui-button" [ "variant", "danger"    ] [ str "Delete" ]
                    wc "fui-button" [ "variant", "primary"; "disabled", "" ] [ str "Disabled" ]
                ]
                div [ ClassName "preview-row"; Style [ MarginTop "0.75rem" ] ] [
                    wc "fui-button" [ "variant", "primary"; "size", "sm" ] [ str "Small" ]
                    wc "fui-button" [ "variant", "primary"; "size", "md" ] [ str "Medium" ]
                    wc "fui-button" [ "variant", "primary"; "size", "lg" ] [ str "Large" ]
                ]
            ]
        ]

        div [ ClassName "component-preview" ] [
            div [ ClassName "preview-header" ] [
                span [ ClassName "preview-tag" ] [ str "fui-input" ]
                span [ ClassName "preview-badge" ] [ str "Inputs & Forms" ]
            ]
            div [ ClassName "preview-body" ] [
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                    wc "fui-input" [ "label", "Email address"; "type", "email"; "placeholder", "you@example.com" ] []
                    wc "fui-input" [ "label", "Password";      "type", "password"; "placeholder", "••••••••"     ] []
                    wc "fui-input" [ "label", "Disabled";      "disabled", ""; "value", "Cannot edit this"       ] []
                    wc "fui-input" [ "label", "With error";    "error", "This field is required"                 ] []
                ]
            ]
        ]
        div [ ClassName "component-preview" ] [
            div [ ClassName "preview-header" ] [
                span [ ClassName "preview-tag" ] [ str "fui-textarea" ]
                span [ ClassName "preview-badge" ] [ str "Inputs & Forms" ]
            ]
            div [ ClassName "preview-body" ] [
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                    wc "fui-textarea" [ "label", "Message";  "placeholder", "Write something..."    ] []
                    wc "fui-textarea" [ "label", "Notes";    "rows", "5"; "resize", "both"          ] []
                    wc "fui-textarea" [ "label", "Disabled"; "disabled", ""; "value", "Cannot edit" ] []
                    wc "fui-textarea" [ "label", "Error";    "error", "This field is required"      ] []
                ]
            ]
        ]

        div [ ClassName "component-preview" ] [
            div [ ClassName "preview-header" ] [
                span [ ClassName "preview-tag" ] [ str "fui-select" ]
                span [ ClassName "preview-badge" ] [ str "Inputs & Forms" ]
            ]
            div [ ClassName "preview-body" ] [
                let fwOpts = """[{"value":"react","label":"React"},{"value":"vue","label":"Vue"},{"value":"svelte","label":"Svelte"},{"value":"solid","label":"Solid"}]"""
                let sizeOpts = """[{"value":"sm","label":"Small"},{"value":"md","label":"Medium"},{"value":"lg","label":"Large"}]"""
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                    wc "fui-select" [ "label", "Framework"; "placeholder", "Pick a framework..."; "options", fwOpts ] []
                    wc "fui-select" [ "label", "Size (preselected)"; "value", "md"; "options", sizeOpts ] []
                    wc "fui-select" [ "label", "Disabled"; "disabled", ""; "value", "react"; "options", fwOpts ] []
                    wc "fui-select" [ "label", "With error"; "placeholder", "Required..."; "error", "Please select an option"; "options", fwOpts ] []
                ]
            ]
        ]

        div [ ClassName "component-preview" ] [
            div [ ClassName "preview-header" ] [
                span [ ClassName "preview-tag" ] [ str "fui-checkbox" ]
                span [ ClassName "preview-badge" ] [ str "Inputs & Forms" ]
            ]
            div [ ClassName "preview-body" ] [
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                    wc "fui-checkbox" [ "label", "Accept terms and conditions" ] []
                    wc "fui-checkbox" [ "label", "Subscribe to newsletter"; "checked", "" ] []
                    wc "fui-checkbox" [ "label", "Disabled option"; "disabled", "" ] []
                    wc "fui-checkbox" [ "label", "With error"; "error", "This field is required" ] []
                ]
            ]
        ]

        div [ ClassName "component-preview" ] [
            div [ ClassName "preview-header" ] [
                span [ ClassName "preview-tag" ] [ str "fui-radio-group" ]
                span [ ClassName "preview-badge" ] [ str "Inputs & Forms" ]
            ]
            div [ ClassName "preview-body" ] [
                let rgOpts = """[{"value":"react","label":"React"},{"value":"vue","label":"Vue"},{"value":"svelte","label":"Svelte"}]"""
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
                    wc "fui-radio-group" [ "label", "Framework";   "name", "hw1"; "options", rgOpts ] []
                    wc "fui-radio-group" [ "label", "Preselected"; "name", "hw2"; "value", "vue"; "options", rgOpts ] []
                    wc "fui-radio-group" [ "label", "Disabled";    "name", "hw3"; "disabled", ""; "value", "react"; "options", rgOpts ] []
                ]
            ]
        ]
    ]

// ── Component detail page ─────────────────────────────────────────────────────

let componentLivePreview (slug: string) : ReactElement list =
    let fwOpts   = """[{"value":"react","label":"React"},{"value":"vue","label":"Vue"},{"value":"svelte","label":"Svelte"},{"value":"solid","label":"Solid"}]"""
    let sizeOpts = """[{"value":"sm","label":"Small"},{"value":"md","label":"Medium"},{"value":"lg","label":"Large"}]"""
    match slug with
    | "button" ->
        [
            div [ ClassName "preview-row" ] [
                wc "fui-button" [ "variant", "primary"   ] [ str "Save" ]
                wc "fui-button" [ "variant", "secondary" ] [ str "Cancel" ]
                wc "fui-button" [ "variant", "ghost"     ] [ str "Menu" ]
                wc "fui-button" [ "variant", "danger"    ] [ str "Delete" ]
                wc "fui-button" [ "variant", "primary"; "disabled", "" ] [ str "Disabled" ]
            ]
            div [ ClassName "preview-row"; Style [ MarginTop "0.75rem" ] ] [
                wc "fui-button" [ "variant", "primary"; "size", "sm" ] [ str "Small" ]
                wc "fui-button" [ "variant", "primary"; "size", "md" ] [ str "Medium" ]
                wc "fui-button" [ "variant", "primary"; "size", "lg" ] [ str "Large" ]
            ]
        ]
    | "input" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-input" [ "label", "Email address"; "type", "email"; "placeholder", "you@example.com" ] []
            wc "fui-input" [ "label", "Password";      "type", "password"; "placeholder", "••••••••"     ] []
            wc "fui-input" [ "label", "Disabled";      "disabled", ""; "value", "Cannot edit this"       ] []
            wc "fui-input" [ "label", "With error";    "error", "This field is required"                 ] []
        ] ]
    | "textarea" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-textarea" [ "label", "Message";  "placeholder", "Write something..."    ] []
            wc "fui-textarea" [ "label", "Notes";    "rows", "5"; "resize", "both"          ] []
            wc "fui-textarea" [ "label", "Disabled"; "disabled", ""; "value", "Cannot edit" ] []
            wc "fui-textarea" [ "label", "Error";    "error", "This field is required"      ] []
        ] ]
    | "select" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-select" [ "label", "Framework"; "placeholder", "Pick a framework..."; "options", fwOpts ] []
            wc "fui-select" [ "label", "Size (preselected)"; "value", "md"; "options", sizeOpts ] []
            wc "fui-select" [ "label", "Disabled"; "disabled", ""; "value", "react"; "options", fwOpts ] []
            wc "fui-select" [ "label", "With error"; "placeholder", "Required..."; "error", "Please select an option"; "options", fwOpts ] []
        ] ]
    | "checkbox" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-checkbox" [ "label", "Accept terms and conditions" ] []
            wc "fui-checkbox" [ "label", "Subscribe to newsletter"; "checked", "" ] []
            wc "fui-checkbox" [ "label", "Disabled option"; "disabled", "" ] []
            wc "fui-checkbox" [ "label", "With error"; "error", "This field is required" ] []
        ] ]
    | "radio" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-radio" [ "value", "a"; "label", "Option A" ] []
            wc "fui-radio" [ "value", "b"; "label", "Option B — checked"; "checked", "" ] []
            wc "fui-radio" [ "value", "c"; "label", "Option C — disabled"; "disabled", "" ] []
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem"); CSSProp.Custom("align-items", "center") ] ] [
                wc "fui-radio" [ "value", "s"; "label", "Small"; "size", "sm" ] []
                wc "fui-radio" [ "value", "m"; "label", "Medium" ] []
                wc "fui-radio" [ "value", "l"; "label", "Large"; "size", "lg" ] []
            ]
        ] ]
    | "radio-group" ->
        let rgOpts   = """[{"value":"react","label":"React"},{"value":"vue","label":"Vue"},{"value":"svelte","label":"Svelte"}]"""
        let sizeOpts = """[{"value":"sm","label":"Small"},{"value":"md","label":"Medium"},{"value":"lg","label":"Large"}]"""
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
            wc "fui-radio-group" [ "label", "Framework";   "name", "rg1"; "options", rgOpts ] []
            wc "fui-radio-group" [ "label", "Preselected"; "name", "rg2"; "value", "vue"; "options", rgOpts ] []
            wc "fui-radio-group" [ "label", "Disabled";    "name", "rg3"; "disabled", ""; "value", "react"; "options", rgOpts ] []
            wc "fui-radio-group" [ "label", "With error";  "name", "rg4"; "error", "Please select an option"; "options", sizeOpts ] []
        ] ]
    | _ -> []

let componentPage (cat: string) (slug: string) =
    match ComponentRegistry.bySlug slug with
    | None ->
        div [ ClassName "comp-page" ] [
            h1 [] [ str $"Component not found: {slug}" ]
        ]
    | Some meta ->
        let preview = componentLivePreview meta.Slug
        div [ ClassName "comp-page" ] [
            yield div [ ClassName "comp-header" ] [
                div [ ClassName "comp-header-row" ] [
                    h1 [ ClassName "comp-name" ] [ str meta.Name ]
                    span [ ClassName "comp-tag" ] [ str meta.Tag ]
                ]
                p [ ClassName "comp-desc" ] [ str meta.Description ]
            ]
            if not preview.IsEmpty then
                yield div [ ClassName "comp-section" ] [
                    h2 [ ClassName "comp-section-title" ] [ str "Preview" ]
                    div [ ClassName "comp-preview-box" ] [
                        div [ ClassName "preview-body" ] preview
                    ]
                ]
            if not meta.Attributes.IsEmpty then
                yield div [ ClassName "comp-section" ] [
                    h2 [ ClassName "comp-section-title" ] [ str "Attributes" ]
                    div [ ClassName "attr-table-wrap" ] [
                        table [ ClassName "attr-table" ] [
                            thead [] [ tr [] [
                                th [] [ str "Attribute" ]
                                th [] [ str "Type" ]
                                th [] [ str "Default" ]
                                th [] [ str "Description" ]
                            ]]
                            tbody [] (meta.Attributes |> List.map (fun a ->
                                tr [] [
                                    td [] [ code [] [ str a.Name ] ]
                                    td [] [ str a.Type ]
                                    td [] [ code [] [ str a.Default ] ]
                                    td [] [ str a.Description ]
                                ]))
                        ]
                    ]
                ]
            if not meta.CssProps.IsEmpty then
                yield div [ ClassName "comp-section" ] [
                    h2 [ ClassName "comp-section-title" ] [ str "CSS Custom Properties" ]
                    div [ ClassName "attr-table-wrap" ] [
                        table [ ClassName "attr-table" ] [
                            thead [] [ tr [] [
                                th [] [ str "Property" ]
                                th [] [ str "Description" ]
                            ]]
                            tbody [] (meta.CssProps |> List.map (fun p ->
                                tr [] [
                                    td [] [ code [] [ str p.Name ] ]
                                    td [] [ str p.Description ]
                                ]))
                        ]
                    ]
                ]
            if not meta.Events.IsEmpty then
                yield div [ ClassName "comp-section" ] [
                    h2 [ ClassName "comp-section-title" ] [ str "Events" ]
                    div [ ClassName "attr-table-wrap" ] [
                        table [ ClassName "attr-table" ] [
                            thead [] [ tr [] [
                                th [] [ str "Event" ]
                                th [] [ str "Description" ]
                            ]]
                            tbody [] (meta.Events |> List.map (fun e ->
                                tr [] [
                                    td [] [ code [] [ str e.Name ] ]
                                    td [] [ str e.Description ]
                                ]))
                        ]
                    ]
                ]
            yield div [ ClassName "comp-section" ] [
                h2 [ ClassName "comp-section-title" ] [ str "HTML Usage" ]
                pre [ ClassName "code-block" ] [ code [] [ str meta.HtmlUsage ] ]
            ]
        ]

// ── Root view ─────────────────────────────────────────────────────────────────

let view (model: Model) (dispatch: Msg -> unit) =
    div [ ClassName "shell" ] [
        sidebar model
        div [ ClassName "shell-main" ] [
            topbar model
            main [ ClassName "page-content" ] [
                div [ Key (string model.Page) ] (
                    match model.Page with
                    | Home          -> [ homePage ]
                    | CounterElmish -> [ Pages.CounterElmish.view model.ElmishCounter (ElmishCounterMsg >> dispatch) ]
                    | CounterDom    -> [ Pages.CounterDom.view () ]
                    | Component(cat, slug) -> [ componentPage cat slug ]
                )
            ]
        ]
    ]

Program.mkProgram init update view
|> Program.withSubscription hashSub
|> Program.withReactSynchronous "app"
|> Program.run
