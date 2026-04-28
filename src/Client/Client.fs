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
                sidebarLink "Toggle"    "#/component/inputs/toggle"      model.Hash
                sidebarLink "Slider"      "#/component/inputs/slider"       model.Hash
                sidebarLink "DatePicker"   "#/component/inputs/date-picker"   model.Hash
                sidebarLink "ColorPicker" "#/component/inputs/color-picker" model.Hash
                sidebarLink "FileUpload"  "#/component/inputs/file-upload"  model.Hash
            ]
            div [ ClassName "sidebar-group" ] [
                div [ ClassName "sidebar-group-label" ] [ str "Layout" ]
                sidebarLink "Divider"      "#/component/layout/divider"      model.Hash
                sidebarLink "Stack"        "#/component/layout/stack"        model.Hash
                sidebarLink "ScrollArea"   "#/component/layout/scroll-area"  model.Hash
                sidebarLink "Container"    "#/component/layout/container"    model.Hash
                sidebarLink "Grid"         "#/component/layout/grid"         model.Hash
                sidebarLink "Spacer"       "#/component/layout/spacer"       model.Hash
                sidebarLink "AspectRatio"  "#/component/layout/aspect-ratio" model.Hash
            ]
            div [ ClassName "sidebar-group" ] [
                div [ ClassName "sidebar-group-label" ] [ str "Overlay" ]
                sidebarLink "Modal"         "#/component/overlay/modal"          model.Hash
                sidebarLink "Drawer"        "#/component/overlay/drawer"         model.Hash
                sidebarLink "Tooltip"       "#/component/overlay/tooltip"        model.Hash
                sidebarLink "Popover"       "#/component/overlay/popover"        model.Hash
                sidebarLink "ConfirmDialog" "#/component/overlay/confirm-dialog" model.Hash
            ]
            div [ ClassName "sidebar-group" ] [
                div [ ClassName "sidebar-group-label" ] [ str "Feedback" ]
                sidebarLink "Badge"      "#/component/feedback/badge"       model.Hash
                sidebarLink "Spinner"    "#/component/feedback/spinner"     model.Hash
                sidebarLink "Progress"   "#/component/feedback/progress"    model.Hash
                sidebarLink "Alert"      "#/component/feedback/alert"       model.Hash
                sidebarLink "Skeleton"   "#/component/feedback/skeleton"    model.Hash
                sidebarLink "Toast"      "#/component/feedback/toast"       model.Hash
                sidebarLink "EmptyState" "#/component/feedback/empty-state" model.Hash
            ]
            div [ ClassName "sidebar-group" ] [
                div [ ClassName "sidebar-group-label" ] [ str "Data Display" ]
                sidebarLink "Card"        "#/component/data-display/card"         model.Hash
                sidebarLink "Stat"        "#/component/data-display/stat"         model.Hash
                sidebarLink "Avatar"      "#/component/data-display/avatar"       model.Hash
                sidebarLink "AvatarGroup" "#/component/data-display/avatar-group" model.Hash
                sidebarLink "Tag"         "#/component/data-display/tag"          model.Hash
                sidebarLink "CodeBlock"   "#/component/data-display/code-block"   model.Hash
                sidebarLink "Callout"     "#/component/data-display/callout"      model.Hash
                sidebarLink "List"        "#/component/data-display/list"         model.Hash
                sidebarLink "Timeline"    "#/component/data-display/timeline"     model.Hash
                sidebarLink "Accordion"   "#/component/data-display/accordion"    model.Hash
                sidebarLink "Carousel"    "#/component/data-display/carousel"     model.Hash
                sidebarLink "Table"       "#/component/data-display/table"        model.Hash
            ]
            div [ ClassName "sidebar-group" ] [
                div [ ClassName "sidebar-group-label" ] [ str "Navigation" ]
                sidebarLink "Tabs"        "#/component/navigation/tabs"        model.Hash
                sidebarLink "Breadcrumb"  "#/component/navigation/breadcrumb"  model.Hash
                sidebarLink "Pagination"  "#/component/navigation/pagination"  model.Hash
                sidebarLink "Stepper"     "#/component/navigation/stepper"     model.Hash
                sidebarLink "Menu"        "#/component/navigation/menu"        model.Hash
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

        div [ ClassName "component-preview" ] [
            div [ ClassName "preview-header" ] [
                span [ ClassName "preview-tag" ] [ str "fui-toggle" ]
                span [ ClassName "preview-badge" ] [ str "Inputs & Forms" ]
            ]
            div [ ClassName "preview-body" ] [
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                    wc "fui-toggle" [ "label", "Enable notifications" ] []
                    wc "fui-toggle" [ "label", "Dark mode"; "checked", "" ] []
                    wc "fui-toggle" [ "label", "Disabled"; "disabled", "" ] []
                    wc "fui-toggle" [ "label", "With error"; "error", "This setting is required" ] []
                ]
            ]
        ]

        div [ ClassName "component-preview" ] [
            div [ ClassName "preview-header" ] [
                span [ ClassName "preview-tag" ] [ str "fui-slider" ]
                span [ ClassName "preview-badge" ] [ str "Inputs & Forms" ]
            ]
            div [ ClassName "preview-body" ] [
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1rem") ] ] [
                    wc "fui-slider" [ "label", "Volume";   "value", "40" ] []
                    wc "fui-slider" [ "label", "Price";    "min", "100"; "max", "1000"; "step", "50"; "value", "400" ] []
                    wc "fui-slider" [ "label", "Disabled"; "value", "60"; "disabled", "" ] []
                ]
            ]
        ]

        div [ ClassName "component-preview" ] [
            div [ ClassName "preview-header" ] [
                span [ ClassName "preview-tag" ] [ str "fui-date-picker" ]
                span [ ClassName "preview-badge" ] [ str "Inputs & Forms" ]
            ]
            div [ ClassName "preview-body" ] [
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                    wc "fui-date-picker" [ "label", "Date of birth" ] []
                    wc "fui-date-picker" [ "label", "Check-in"; "value", "2025-06-01"; "min", "2025-01-01"; "max", "2025-12-31" ] []
                    wc "fui-date-picker" [ "label", "Disabled"; "value", "2025-03-15"; "disabled", "" ] []
                    wc "fui-date-picker" [ "label", "With error"; "error", "Please select a valid date" ] []
                ]
            ]
        ]

        div [ ClassName "component-preview" ] [
            div [ ClassName "preview-header" ] [
                span [ ClassName "preview-tag" ] [ str "fui-color-picker" ]
                span [ ClassName "preview-badge" ] [ str "Inputs & Forms" ]
            ]
            div [ ClassName "preview-body" ] [
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                    wc "fui-color-picker" [ "label", "Brand colour"; "value", "#7C3AED" ] []
                    wc "fui-color-picker" [ "label", "Accent";       "value", "#3B82F6" ] []
                    wc "fui-color-picker" [ "label", "Disabled";     "value", "#22C55E"; "disabled", "" ] []
                    wc "fui-color-picker" [ "label", "With error";   "error", "A colour is required" ] []
                ]
            ]
        ]

        div [ ClassName "component-preview" ] [
            div [ ClassName "preview-header" ] [
                span [ ClassName "preview-tag" ] [ str "fui-file-upload" ]
                span [ ClassName "preview-badge" ] [ str "Inputs & Forms" ]
            ]
            div [ ClassName "preview-body" ] [
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1rem") ] ] [
                    wc "fui-file-upload" [ "label", "Attachment" ] []
                    wc "fui-file-upload" [ "label", "Images only"; "accept", "image/*"; "multiple", "" ] []
                    wc "fui-file-upload" [ "label", "Disabled"; "disabled", "" ] []
                ]
            ]
        ]

        div [ ClassName "component-preview" ] [
            div [ ClassName "preview-header" ] [
                span [ ClassName "preview-tag" ] [ str "fui-tabs · fui-breadcrumb · fui-pagination · fui-stepper · fui-menu" ]
                span [ ClassName "preview-badge" ] [ str "Navigation" ]
            ]
            div [ ClassName "preview-body" ] [
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
                    let tabsJson = """[{"value":"a","label":"Overview"},{"value":"b","label":"Settings"},{"value":"c","label":"Logs"}]"""
                    wc "fui-tabs" [ "tabs", tabsJson ] [
                        div [ HTMLAttr.Custom("slot","tab-a") ] [ str "Overview content." ]
                        div [ HTMLAttr.Custom("slot","tab-b") ] [ str "Settings panel." ]
                        div [ HTMLAttr.Custom("slot","tab-c") ] [ str "Log output." ]
                    ]
                    let bcItems = """[{"label":"Home","href":"/"},{"label":"Components","href":""},{"label":"Navigation","href":""}]"""
                    wc "fui-breadcrumb" [ "items", bcItems ] []
                    wc "fui-pagination" [ "total", "10"; "page", "1" ] []
                    let stepsJson = """[{"label":"Account","description":"Create your account"},{"label":"Profile","description":"Fill in your details"},{"label":"Review","description":"Confirm and submit"}]"""
                    wc "fui-stepper" [ "steps", stepsJson; "active", "1" ] []
                    let menuItems = """[{"value":"edit","label":"Edit","disabled":false,"separator":false},{"value":"dup","label":"Duplicate","disabled":false,"separator":false},{"value":"","label":"","disabled":false,"separator":true},{"value":"delete","label":"Delete","disabled":false,"separator":false}]"""
                    wc "fui-menu" [ "label", "Actions"; "items", menuItems ] []
                ]
            ]
        ]

        div [ ClassName "component-preview" ] [
            div [ ClassName "preview-header" ] [
                span [ ClassName "preview-tag" ] [ str "fui-divider · fui-stack" ]
                span [ ClassName "preview-badge" ] [ str "Layout" ]
            ]
            div [ ClassName "preview-body" ] [
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1rem") ] ] [
                    wc "fui-divider" [] []
                    wc "fui-divider" [ "label", "or" ] []
                    wc "fui-divider" [ "label", "Section title" ] []
                    wc "fui-stack" [ "direction", "row"; "gap", "0.75rem" ] [
                        wc "fui-button" [ "variant", "primary"   ] [ str "Save changes" ]
                        wc "fui-button" [ "variant", "secondary" ] [ str "Cancel" ]
                        wc "fui-button" [ "variant", "ghost"     ] [ str "Preview" ]
                    ]
                ]
            ]
        ]

        div [ ClassName "component-preview" ] [
            div [ ClassName "preview-header" ] [
                span [ ClassName "preview-tag" ] [ str "fui-grid · fui-spacer · fui-aspect-ratio · fui-container" ]
                span [ ClassName "preview-badge" ] [ str "Layout" ]
            ]
            div [ ClassName "preview-body" ] [
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
                    // Grid
                    wc "fui-grid" [ "cols", "3"; "gap", "0.75rem" ] [
                        div [ Style [ CSSProp.Custom("background", "#1E1E21"); CSSProp.Custom("border", "1px solid #2A2A2E"); CSSProp.Custom("border-radius", "6px"); Padding "0.625rem 0.875rem"; FontSize "0.8rem"; CSSProp.Custom("color", "#E8E8ED") ] ] [ str "Grid col 1" ]
                        div [ Style [ CSSProp.Custom("background", "#1E1E21"); CSSProp.Custom("border", "1px solid #2A2A2E"); CSSProp.Custom("border-radius", "6px"); Padding "0.625rem 0.875rem"; FontSize "0.8rem"; CSSProp.Custom("color", "#E8E8ED") ] ] [ str "Grid col 2" ]
                        div [ Style [ CSSProp.Custom("background", "#1E1E21"); CSSProp.Custom("border", "1px solid #2A2A2E"); CSSProp.Custom("border-radius", "6px"); Padding "0.625rem 0.875rem"; FontSize "0.8rem"; CSSProp.Custom("color", "#E8E8ED") ] ] [ str "Grid col 3" ]
                    ]
                    // Spacer pushing buttons apart
                    div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("background", "#1E1E21"); CSSProp.Custom("border-radius", "8px"); Padding "0.75rem"; CSSProp.Custom("align-items", "center") ] ] [
                        wc "fui-button" [ "variant", "ghost";   "size", "sm" ] [ str "← Back" ]
                        wc "fui-spacer" [] []
                        wc "fui-button" [ "variant", "primary"; "size", "sm" ] [ str "Next →" ]
                    ]
                    // AspectRatio demos
                    div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem"); CSSProp.Custom("align-items", "flex-end") ] ] [
                        div [ Style [ CSSProp.Custom("width", "180px"); CSSProp.Custom("flex-shrink", "0") ] ] [
                            p [ Style [ Margin "0 0 0.35rem"; FontSize "0.7rem"; CSSProp.Custom("color", "#6E6E76") ] ] [ str "16 / 9" ]
                            wc "fui-aspect-ratio" [ "ratio", "16/9" ] [
                                div [ Style [ CSSProp.Custom("background", "#1E1E21"); CSSProp.Custom("width", "100%"); CSSProp.Custom("height", "100%"); Display DisplayOptions.Flex; CSSProp.Custom("align-items", "center"); CSSProp.Custom("justify-content", "center"); FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76") ] ] [ str "16 / 9" ]
                            ]
                        ]
                        div [ Style [ CSSProp.Custom("width", "100px"); CSSProp.Custom("flex-shrink", "0") ] ] [
                            p [ Style [ Margin "0 0 0.35rem"; FontSize "0.7rem"; CSSProp.Custom("color", "#6E6E76") ] ] [ str "1 / 1" ]
                            wc "fui-aspect-ratio" [ "ratio", "1/1" ] [
                                div [ Style [ CSSProp.Custom("background", "#1E1E21"); CSSProp.Custom("width", "100%"); CSSProp.Custom("height", "100%"); Display DisplayOptions.Flex; CSSProp.Custom("align-items", "center"); CSSProp.Custom("justify-content", "center"); FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76") ] ] [ str "1 / 1" ]
                            ]
                        ]
                        div [ Style [ CSSProp.Custom("width", "160px"); CSSProp.Custom("flex-shrink", "0") ] ] [
                            p [ Style [ Margin "0 0 0.35rem"; FontSize "0.7rem"; CSSProp.Custom("color", "#6E6E76") ] ] [ str "2 / 1" ]
                            wc "fui-aspect-ratio" [ "ratio", "2/1" ] [
                                div [ Style [ CSSProp.Custom("background", "#1E1E21"); CSSProp.Custom("width", "100%"); CSSProp.Custom("height", "100%"); Display DisplayOptions.Flex; CSSProp.Custom("align-items", "center"); CSSProp.Custom("justify-content", "center"); FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76") ] ] [ str "2 / 1" ]
                            ]
                        ]
                    ]
                ]
            ]
        ]

        div [ ClassName "component-preview" ] [
            div [ ClassName "preview-header" ] [
                span [ ClassName "preview-tag" ] [ str "fui-modal · fui-drawer · fui-tooltip · fui-popover · fui-confirm-dialog" ]
                span [ ClassName "preview-badge" ] [ str "Overlay" ]
            ]
            div [ ClassName "preview-body" ] [
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.75rem"); CSSProp.Custom("flex-wrap", "wrap"); CSSProp.Custom("align-items", "flex-start") ] ] [
                    wc "fui-modal" [ "trigger-label", "Open modal"; "title", "Example Modal"; "size", "sm" ] [
                        p [ Style [ Margin "0"; CSSProp.Custom("color", "#A0A0A8") ] ] [ str "Modal body — slot content goes here." ]
                        wc "fui-button" [ "slot", "footer"; "variant", "primary" ] [ str "Confirm" ]
                        wc "fui-button" [ "slot", "footer"; "variant", "secondary" ] [ str "Cancel" ]
                    ]
                    wc "fui-drawer" [ "trigger-label", "Open drawer"; "title", "Settings"; "placement", "right" ] [
                        p [ Style [ Margin "0"; CSSProp.Custom("color", "#A0A0A8") ] ] [ str "Drawer body content." ]
                    ]
                    wc "fui-tooltip" [ "content", "Hover me!"; "placement", "top" ] [
                        wc "fui-button" [ "variant", "secondary" ] [ str "Hover for tip" ]
                    ]
                    wc "fui-popover" [ "trigger-label", "Filter"; "width", "200px" ] [
                        p [ Style [ Margin "0 0 0.5rem"; FontSize "0.875rem"; CSSProp.Custom("color", "#A0A0A8") ] ] [ str "Popover content" ]
                        wc "fui-button" [ "variant", "primary" ] [ str "Apply" ]
                    ]
                    wc "fui-confirm-dialog" [ "trigger-label", "Delete"; "title", "Delete item?"; "message", "This cannot be undone."; "confirm-label", "Delete"; "variant", "danger" ] []
                ]
            ]
        ]

        div [ ClassName "component-preview" ] [
            div [ ClassName "preview-header" ] [
                span [ ClassName "preview-tag" ] [ str "fui-badge · fui-spinner · fui-progress · fui-alert · fui-toast" ]
                span [ ClassName "preview-badge" ] [ str "Feedback" ]
            ]
            div [ ClassName "preview-body" ] [
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
                    div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.5rem"); CSSProp.Custom("flex-wrap", "wrap"); CSSProp.Custom("align-items", "center") ] ] [
                        wc "fui-badge" [] [ str "Neutral" ]
                        wc "fui-badge" [ "variant", "success" ] [ str "Success" ]
                        wc "fui-badge" [ "variant", "warning" ] [ str "Warning" ]
                        wc "fui-badge" [ "variant", "danger"  ] [ str "Danger" ]
                        wc "fui-badge" [ "variant", "info"    ] [ str "Info" ]
                        wc "fui-badge" [ "variant", "accent"  ] [ str "Accent" ]
                        wc "fui-badge" [ "variant", "success"; "dot", "" ] [ str "Online" ]
                        wc "fui-badge" [ "variant", "danger";  "dot", "" ] [ str "Offline" ]
                    ]
                    div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem"); CSSProp.Custom("align-items", "center") ] ] [
                        wc "fui-spinner" [ "size", "sm" ] []
                        wc "fui-spinner" [] []
                        wc "fui-spinner" [ "size", "lg" ] []
                        wc "fui-spinner" [ "size", "xl"; "label", "Loading…" ] []
                    ]
                    div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.5rem") ] ] [
                        wc "fui-progress" [ "label", "Upload";   "value", "65" ] []
                        wc "fui-progress" [ "label", "Storage";  "value", "90"; "variant", "danger"  ] []
                        wc "fui-progress" [ "label", "Loading…"; "indeterminate", "" ] []
                    ]
                    wc "fui-alert" [ "variant", "info" ] [ str "Your session will expire in 10 minutes." ]
                    wc "fui-alert" [ "variant", "success"; "title", "Changes saved" ] [ str "Your profile has been updated." ]
                    wc "fui-toast" [ "variant", "success"; "title", "Saved"; "message", "Your changes have been saved." ] []
                ]
            ]
        ]

        div [ ClassName "component-preview" ] [
            div [ ClassName "preview-header" ] [
                span [ ClassName "preview-tag" ] [ str "fui-card · fui-stat · fui-avatar · fui-tag · fui-callout · fui-code-block" ]
                span [ ClassName "preview-badge" ] [ str "Data Display" ]
            ]
            div [ ClassName "preview-body" ] [
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
                    div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
                        wc "fui-stat" [ "label", "Revenue"; "value", "$48,200"; "change", "12.5%"; "trend", "up"; "description", "vs last month" ] []
                        wc "fui-stat" [ "label", "Users";   "value", "8,412";   "change", "3.1%";  "trend", "down" ] []
                        wc "fui-stat" [ "label", "Uptime";  "value", "99.9%" ] []
                    ]
                    div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.5rem"); CSSProp.Custom("align-items", "center"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
                        wc "fui-avatar" [ "initials", "JD" ] []
                        wc "fui-avatar" [ "initials", "AB"; "status", "online" ] []
                        wc "fui-avatar" [ "initials", "XY"; "size", "lg"; "status", "busy" ] []
                        let agItems = """[{"src":"","initials":"JD"},{"src":"","initials":"AB"},{"src":"","initials":"MK"},{"src":"","initials":"RQ"},{"src":"","initials":"TW"}]"""
                        wc "fui-avatar-group" [ "avatars", agItems ] []
                    ]
                    div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.5rem"); CSSProp.Custom("flex-wrap", "wrap"); CSSProp.Custom("align-items", "center") ] ] [
                        wc "fui-tag" [] [ str "Default" ]
                        wc "fui-tag" [ "variant", "success" ] [ str "Active" ]
                        wc "fui-tag" [ "variant", "warning" ] [ str "Pending" ]
                        wc "fui-tag" [ "variant", "danger"; "removable", "" ] [ str "Remove me" ]
                        wc "fui-tag" [ "variant", "info"   ] [ str "Info" ]
                        wc "fui-tag" [ "variant", "accent" ] [ str "Accent" ]
                    ]
                    wc "fui-callout" [ "variant", "info"; "title", "Getting started" ] [ str "Drop any fui-* component into your HTML and it works out of the box." ]
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
    | "toggle" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-toggle" [ "label", "Enable notifications" ] []
            wc "fui-toggle" [ "label", "Dark mode"; "checked", "" ] []
            wc "fui-toggle" [ "label", "Disabled off"; "disabled", "" ] []
            wc "fui-toggle" [ "label", "Disabled on"; "checked", ""; "disabled", "" ] []
            wc "fui-toggle" [ "label", "With error"; "error", "This setting is required" ] []
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem"); CSSProp.Custom("align-items", "center") ] ] [
                wc "fui-toggle" [ "label", "Small"; "size", "sm"; "checked", "" ] []
                wc "fui-toggle" [ "label", "Medium"; "checked", "" ] []
                wc "fui-toggle" [ "label", "Large"; "size", "lg"; "checked", "" ] []
            ]
        ] ]
    | "slider" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
            wc "fui-slider" [ "label", "Volume";       "value", "40" ] []
            wc "fui-slider" [ "label", "Price range";  "min", "100"; "max", "1000"; "step", "50"; "value", "400" ] []
            wc "fui-slider" [ "label", "Opacity";      "min", "0"; "max", "1"; "step", "0.01"; "value", "0.75" ] []
            wc "fui-slider" [ "label", "Disabled";     "value", "60"; "disabled", "" ] []
            wc "fui-slider" [ "label", "With error";   "value", "0"; "error", "Value must be greater than 0" ] []
            div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                wc "fui-slider" [ "label", "Small";  "size", "sm"; "value", "30" ] []
                wc "fui-slider" [ "label", "Medium"; "value", "50" ] []
                wc "fui-slider" [ "label", "Large";  "size", "lg"; "value", "70" ] []
            ]
        ] ]
    | "date-picker" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-date-picker" [ "label", "Date of birth" ] []
            wc "fui-date-picker" [ "label", "Check-in"; "value", "2025-06-01"; "min", "2025-01-01"; "max", "2025-12-31" ] []
            wc "fui-date-picker" [ "label", "Disabled"; "value", "2025-03-15"; "disabled", "" ] []
            wc "fui-date-picker" [ "label", "With error"; "error", "Please select a valid date" ] []
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem") ] ] [
                wc "fui-date-picker" [ "label", "Small";  "size", "sm" ] []
                wc "fui-date-picker" [ "label", "Medium" ] []
                wc "fui-date-picker" [ "label", "Large";  "size", "lg" ] []
            ]
        ] ]
    | "color-picker" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-color-picker" [ "label", "Brand colour"; "value", "#7C3AED" ] []
            wc "fui-color-picker" [ "label", "Accent";       "value", "#3B82F6" ] []
            wc "fui-color-picker" [ "label", "Success";      "value", "#22C55E" ] []
            wc "fui-color-picker" [ "label", "Danger";       "value", "#EF4444" ] []
            wc "fui-color-picker" [ "label", "Disabled";     "value", "#F59E0B"; "disabled", "" ] []
            wc "fui-color-picker" [ "label", "With error";   "error", "A colour is required" ] []
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem"); CSSProp.Custom("align-items", "flex-start") ] ] [
                wc "fui-color-picker" [ "label", "Small";  "size", "sm"; "value", "#7C3AED" ] []
                wc "fui-color-picker" [ "label", "Medium"; "value", "#7C3AED" ] []
                wc "fui-color-picker" [ "label", "Large";  "size", "lg"; "value", "#7C3AED" ] []
            ]
        ] ]
    | "file-upload" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
            wc "fui-file-upload" [ "label", "Attachment" ] []
            wc "fui-file-upload" [ "label", "Images only"; "accept", "image/*"; "multiple", "" ] []
            wc "fui-file-upload" [ "label", "Documents";   "accept", ".pdf,.docx,.xlsx" ] []
            wc "fui-file-upload" [ "label", "Disabled";    "disabled", "" ] []
            wc "fui-file-upload" [ "label", "With error";  "error", "Please upload a file" ] []
            div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                wc "fui-file-upload" [ "label", "Small";  "size", "sm" ] []
                wc "fui-file-upload" [ "label", "Medium" ] []
                wc "fui-file-upload" [ "label", "Large";  "size", "lg" ] []
            ]
        ] ]
    | "divider" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            // Plain horizontal rule
            wc "fui-divider" [] []
            // With label
            wc "fui-divider" [ "label", "or" ] []
            wc "fui-divider" [ "label", "Section title" ] []
            // Vertical inside a flex row
            div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("height", "48px"); CSSProp.Custom("align-items", "center"); CSSProp.Custom("gap", "1rem") ] ] [
                span [] [ str "Left content" ]
                wc "fui-divider" [ "orientation", "vertical" ] []
                span [] [ str "Right content" ]
                wc "fui-divider" [ "orientation", "vertical" ] []
                span [] [ str "More content" ]
            ]
        ] ]
    | "stack" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            // Row
            wc "fui-stack" [ "direction", "row"; "gap", "0.75rem" ] [
                wc "fui-button" [ "variant", "primary"   ] [ str "Save" ]
                wc "fui-button" [ "variant", "secondary" ] [ str "Cancel" ]
                wc "fui-button" [ "variant", "ghost"     ] [ str "Preview" ]
            ]
            // Column
            wc "fui-stack" [ "direction", "column"; "gap", "0.75rem" ] [
                wc "fui-input"  [ "label", "Email";    "type", "email";    "placeholder", "you@example.com" ] []
                wc "fui-input"  [ "label", "Password"; "type", "password"; "placeholder", "••••••••"        ] []
                wc "fui-button" [ "variant", "primary" ] [ str "Sign in" ]
            ]
            // Row wrapping
            wc "fui-stack" [ "direction", "row"; "gap", "0.5rem"; "wrap", "" ] [
                wc "fui-button" [ "variant", "secondary"; "size", "sm" ] [ str "Draft" ]
                wc "fui-button" [ "variant", "secondary"; "size", "sm" ] [ str "Review" ]
                wc "fui-button" [ "variant", "secondary"; "size", "sm" ] [ str "Approved" ]
                wc "fui-button" [ "variant", "secondary"; "size", "sm" ] [ str "Published" ]
                wc "fui-button" [ "variant", "secondary"; "size", "sm" ] [ str "Archived" ]
            ]
            // Row centred
            wc "fui-stack" [ "direction", "row"; "gap", "1rem"; "justify", "center"; "align", "center" ] [
                wc "fui-toggle"   [ "label", "Notifications" ] []
                wc "fui-divider"  [ "orientation", "vertical" ] []
                wc "fui-toggle"   [ "label", "Dark mode"; "checked", "" ] []
            ]
        ] ]
    | "scroll-area" ->
        let items = [ 1..25 ] |> List.map (fun i -> li [ Style [ Padding "0.35rem 0"; CSSProp.Custom("border-bottom", "1px solid #2A2A2E") ] ] [ str $"List item {i}" ])
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Vertical — 200px" ]
                wc "fui-scroll-area" [ "height", "200px"; "direction", "vertical" ] [
                    ul [ Style [ Margin "0"; Padding "0 0 0 0"; ListStyleType "none" ] ] items
                ]
            ]
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "max-height — 150px" ]
                wc "fui-scroll-area" [ "max-height", "150px" ] [
                    div [ Style [ Padding "0.5rem" ] ] [
                        yield! [ 1..10 ] |> List.map (fun i -> p [ Style [ Margin "0.25rem 0" ] ] [ str $"Paragraph {i} — some content that fills the scroll viewport." ])
                    ]
                ]
            ]
        ] ]
    | "container" ->
        let label txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        let frame children = div [ Style [ CSSProp.Custom("background", "#0D0D0F"); CSSProp.Custom("border", "1px dashed #2A2A2E"); CSSProp.Custom("border-radius", "8px"); CSSProp.Custom("overflow", "hidden") ] ] children
        let inner txt = div [ Style [ CSSProp.Custom("background", "#1E1E21"); Padding "0.75rem"; CSSProp.Custom("border-radius", "4px") ] ] [ span [ Style [ FontSize "0.8rem"; CSSProp.Custom("color", "#6E6E76") ] ] [ str txt ] ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1rem") ] ] [
            div [] [
                label "Default (max-width: 1100px)"
                frame [ wc "fui-container" [] [ inner "Centred content — constrained to 1100px with 2.5rem side padding." ] ]
            ]
            div [] [
                label "max-width: 480px, padding: 1rem"
                frame [ wc "fui-container" [ "max-width", "480px"; "padding", "1rem" ] [ inner "Narrower container — comfortable for articles and forms." ] ]
            ]
            div [] [
                label "max-width: 320px, padding: 0.75rem"
                frame [ wc "fui-container" [ "max-width", "320px"; "padding", "0.75rem" ] [ inner "Mobile-width column." ] ]
            ]
        ] ]
    | "grid" ->
        let cell lbl = div [ Style [ CSSProp.Custom("background", "#1E1E21"); CSSProp.Custom("border", "1px solid #2A2A2E"); CSSProp.Custom("border-radius", "6px"); Padding "0.625rem 0.875rem"; FontSize "0.8rem"; CSSProp.Custom("color", "#E8E8ED") ] ] [ str lbl ]
        let sectionLabel txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [
                sectionLabel "3 equal columns"
                wc "fui-grid" [ "cols", "3"; "gap", "0.75rem" ] [
                    cell "Column 1"; cell "Column 2"; cell "Column 3"
                ]
            ]
            div [] [
                sectionLabel "2fr + 1fr (main + sidebar)"
                wc "fui-grid" [ "cols", "2fr 1fr"; "gap", "0.75rem" ] [
                    cell "Main content area"
                    cell "Sidebar"
                ]
            ]
            div [] [
                sectionLabel "4 equal columns"
                wc "fui-grid" [ "cols", "4"; "gap", "0.75rem" ] [
                    cell "A"; cell "B"; cell "C"; cell "D"
                ]
            ]
            div [] [
                sectionLabel "Auto-fill (min 180px)"
                wc "fui-grid" [ "cols", "repeat(auto-fill, minmax(180px, 1fr))"; "gap", "0.75rem" ] [
                    cell "Card 1"; cell "Card 2"; cell "Card 3"; cell "Card 4"
                ]
            ]
        ] ]
    | "spacer" ->
        let sectionLabel txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        let row children = div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("background", "#1E1E21"); CSSProp.Custom("border-radius", "8px"); Padding "0.75rem"; CSSProp.Custom("align-items", "center") ] ] children
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
            div [] [
                sectionLabel "Flexible spacer — pushes buttons to opposite ends"
                row [
                    wc "fui-button" [ "variant", "ghost";   "size", "sm" ] [ str "← Back" ]
                    wc "fui-spacer" [] []
                    wc "fui-button" [ "variant", "primary"; "size", "sm" ] [ str "Next →" ]
                ]
            ]
            div [] [
                sectionLabel "Flexible spacer — toolbar with mixed items"
                row [
                    wc "fui-button" [ "variant", "secondary"; "size", "sm" ] [ str "File" ]
                    wc "fui-button" [ "variant", "secondary"; "size", "sm" ] [ str "Edit" ]
                    wc "fui-spacer" [] []
                    wc "fui-button" [ "variant", "ghost"; "size", "sm" ] [ str "Help" ]
                    wc "fui-button" [ "variant", "ghost"; "size", "sm" ] [ str "Settings" ]
                ]
            ]
            div [] [
                sectionLabel "Fixed spacer — size: 2rem"
                row [
                    span [ Style [ FontSize "0.8rem"; CSSProp.Custom("color", "#E8E8ED") ] ] [ str "Label" ]
                    wc "fui-spacer" [ "size", "2rem" ] []
                    span [ Style [ FontSize "0.8rem"; CSSProp.Custom("color", "#6E6E76") ] ] [ str "Value" ]
                ]
            ]
        ] ]
    | "aspect-ratio" ->
        let sectionLabel txt = p [ Style [ Margin "0 0 0.375rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76") ] ] [ str txt ]
        let placeholder lbl = div [ Style [ CSSProp.Custom("background", "#1E1E21"); CSSProp.Custom("width", "100%"); CSSProp.Custom("height", "100%"); Display DisplayOptions.Flex; CSSProp.Custom("align-items", "center"); CSSProp.Custom("justify-content", "center"); FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("font-family", "JetBrains Mono, monospace") ] ] [ str lbl ]
        [ div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1.25rem"); CSSProp.Custom("flex-wrap", "wrap"); CSSProp.Custom("align-items", "flex-start") ] ] [
            div [ Style [ CSSProp.Custom("width", "220px"); CSSProp.Custom("flex-shrink", "0") ] ] [
                sectionLabel "16 / 9 — video"
                wc "fui-aspect-ratio" [ "ratio", "16/9" ] [ placeholder "16 / 9" ]
            ]
            div [ Style [ CSSProp.Custom("width", "140px"); CSSProp.Custom("flex-shrink", "0") ] ] [
                sectionLabel "4 / 3 — traditional"
                wc "fui-aspect-ratio" [ "ratio", "4/3" ] [ placeholder "4 / 3" ]
            ]
            div [ Style [ CSSProp.Custom("width", "100px"); CSSProp.Custom("flex-shrink", "0") ] ] [
                sectionLabel "1 / 1 — square"
                wc "fui-aspect-ratio" [ "ratio", "1/1" ] [ placeholder "1 / 1" ]
            ]
            div [ Style [ CSSProp.Custom("width", "220px"); CSSProp.Custom("flex-shrink", "0") ] ] [
                sectionLabel "2 / 1 — wide banner"
                wc "fui-aspect-ratio" [ "ratio", "2/1" ] [ placeholder "2 / 1" ]
            ]
        ] ]
    | "tabs" ->
        let tabsJson = """[{"value":"overview","label":"Overview"},{"value":"settings","label":"Settings"},{"value":"logs","label":"Logs"}]"""
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "2rem") ] ] [
            div [] [
                p [ Style [ Margin "0 0 0.75rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Default — first tab active" ]
                wc "fui-tabs" [ "tabs", tabsJson ] [
                    div [ HTMLAttr.Custom("slot", "tab-overview") ] [ p [ Style [ Margin "0" ] ] [ str "Overview content — displays first by default." ] ]
                    div [ HTMLAttr.Custom("slot", "tab-settings") ] [ p [ Style [ Margin "0" ] ] [ str "Settings panel content." ] ]
                    div [ HTMLAttr.Custom("slot", "tab-logs")     ] [ p [ Style [ Margin "0" ] ] [ str "Log output panel." ] ]
                ]
            ]
            div [] [
                p [ Style [ Margin "0 0 0.75rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "active=\"settings\"" ]
                wc "fui-tabs" [ "tabs", tabsJson; "active", "settings" ] [
                    div [ HTMLAttr.Custom("slot", "tab-overview") ] [ p [ Style [ Margin "0" ] ] [ str "Overview content." ] ]
                    div [ HTMLAttr.Custom("slot", "tab-settings") ] [ p [ Style [ Margin "0" ] ] [ str "Settings opened by default via the active attribute." ] ]
                    div [ HTMLAttr.Custom("slot", "tab-logs")     ] [ p [ Style [ Margin "0" ] ] [ str "Log output panel." ] ]
                ]
            ]
        ] ]
    | "breadcrumb" ->
        let items1 = """[{"label":"Home","href":"/"},{"label":"Components","href":"/components"},{"label":"Breadcrumb","href":""}]"""
        let items2 = """[{"label":"Docs","href":"/docs"},{"label":"API Reference","href":"/docs/api"},{"label":"Endpoints","href":""}]"""
        let items3 = """[{"label":"Home","href":"/"},{"label":"Settings","href":""}]"""
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
            wc "fui-breadcrumb" [ "items", items1 ] []
            wc "fui-breadcrumb" [ "items", items2; "separator", "›" ] []
            wc "fui-breadcrumb" [ "items", items3; "separator", "▸" ] []
        ] ]
    | "pagination" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "10 pages — first page" ]
                wc "fui-pagination" [ "total", "10"; "page", "1" ] []
            ]
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "10 pages — middle (page 5)" ]
                wc "fui-pagination" [ "total", "10"; "page", "5" ] []
            ]
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "50 pages — middle (page 25), siblings: 2" ]
                wc "fui-pagination" [ "total", "50"; "page", "25"; "siblings", "2" ] []
            ]
        ] ]
    | "stepper" ->
        let stepsJson     = """[{"label":"Account","description":"Create your account"},{"label":"Profile","description":"Fill in your details"},{"label":"Review","description":"Confirm and submit"}]"""
        let stepsJsonLong = """[{"label":"Plan","description":"Choose a plan"},{"label":"Build","description":"Configure your stack"},{"label":"Test","description":"Run checks"},{"label":"Deploy","description":"Ship to production"}]"""
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "2rem") ] ] [
            div [] [
                p [ Style [ Margin "0 0 0.75rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Horizontal — step 0 (first)" ]
                wc "fui-stepper" [ "steps", stepsJson; "active", "0" ] []
            ]
            div [] [
                p [ Style [ Margin "0 0 0.75rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Horizontal — step 1 (in progress)" ]
                wc "fui-stepper" [ "steps", stepsJson; "active", "1" ] []
            ]
            div [] [
                p [ Style [ Margin "0 0 0.75rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Vertical — clickable, 4 steps" ]
                wc "fui-stepper" [ "steps", stepsJsonLong; "active", "1"; "orientation", "vertical"; "clickable", "" ] []
            ]
        ] ]
    | "menu" ->
        let items1 = """[{"value":"edit","label":"Edit"},{"value":"dup","label":"Duplicate"},{"value":"","label":"","separator":true},{"value":"delete","label":"Delete"}]"""
        let items2 = """[{"value":"profile","label":"Profile"},{"value":"settings","label":"Settings"},{"value":"","label":"","separator":true},{"value":"logout","label":"Sign out"}]"""
        let items3 = """[{"value":"asc","label":"Sort A → Z"},{"value":"desc","label":"Sort Z → A"},{"value":"","label":"","separator":true},{"value":"filter","label":"Add filter"},{"value":"group","label":"Group by","disabled":true}]"""
        [ div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.75rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
            wc "fui-menu" [ "label", "Actions";       "items", items1 ] []
            wc "fui-menu" [ "label", "Account";       "items", items2; "placement", "bottom-end" ] []
            wc "fui-menu" [ "label", "Sort & Filter"; "items", items3 ] []
        ] ]
    | "modal" ->
        let sectionLabel txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.75rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
            div [] [
                sectionLabel "Small"
                wc "fui-modal" [ "trigger-label", "Open sm"; "title", "Small modal"; "size", "sm" ] [
                    p [ Style [ Margin "0"; CSSProp.Custom("color", "#A0A0A8") ] ] [ str "A compact modal — great for quick confirmations or short forms." ]
                    wc "fui-button" [ "slot", "footer"; "variant", "primary" ] [ str "Save" ]
                    wc "fui-button" [ "slot", "footer"; "variant", "secondary" ] [ str "Cancel" ]
                ]
            ]
            div [] [
                sectionLabel "Medium (default)"
                wc "fui-modal" [ "trigger-label", "Open md"; "title", "Edit profile" ] [
                    div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                        wc "fui-input" [ "label", "Display name"; "placeholder", "Jane Smith" ] []
                        wc "fui-input" [ "label", "Email"; "type", "email"; "placeholder", "jane@example.com" ] []
                        wc "fui-textarea" [ "label", "Bio"; "rows", "3"; "placeholder", "Tell us about yourself…" ] []
                    ]
                    wc "fui-button" [ "slot", "footer"; "variant", "primary" ] [ str "Save changes" ]
                    wc "fui-button" [ "slot", "footer"; "variant", "secondary" ] [ str "Discard" ]
                ]
            ]
            div [] [
                sectionLabel "Large"
                wc "fui-modal" [ "trigger-label", "Open lg"; "title", "Preview"; "size", "lg" ] [
                    p [ Style [ Margin "0"; CSSProp.Custom("color", "#A0A0A8") ] ] [ str "Large modals work well for previews, rich editors, or data tables." ]
                ]
            ]
        ] ]
    | "drawer" ->
        let sectionLabel txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.75rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
            div [] [
                sectionLabel "Right (default)"
                wc "fui-drawer" [ "trigger-label", "Open right"; "title", "Settings"; "placement", "right" ] [
                    div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                        wc "fui-toggle" [ "label", "Enable notifications"; "checked", "" ] []
                        wc "fui-toggle" [ "label", "Dark mode" ] []
                        wc "fui-select" [ "label", "Language"; "value", "en"; "options", """[{"value":"en","label":"English"},{"value":"fr","label":"French"},{"value":"de","label":"German"}]""" ] []
                    ]
                    wc "fui-button" [ "slot", "footer"; "variant", "primary" ] [ str "Save" ]
                    wc "fui-button" [ "slot", "footer"; "variant", "secondary" ] [ str "Cancel" ]
                ]
            ]
            div [] [
                sectionLabel "Left"
                wc "fui-drawer" [ "trigger-label", "Open left"; "title", "Navigation"; "placement", "left" ] [
                    div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.5rem") ] ] [
                        wc "fui-button" [ "variant", "ghost" ] [ str "Dashboard" ]
                        wc "fui-button" [ "variant", "ghost" ] [ str "Components" ]
                        wc "fui-button" [ "variant", "ghost" ] [ str "Settings" ]
                    ]
                ]
            ]
            div [] [
                sectionLabel "Bottom"
                wc "fui-drawer" [ "trigger-label", "Open bottom"; "title", "Share"; "placement", "bottom" ] [
                    p [ Style [ Margin "0 0 0.75rem"; CSSProp.Custom("color", "#A0A0A8"); FontSize "0.875rem" ] ] [ str "Share this document with your team." ]
                    wc "fui-input" [ "label", "Email address"; "placeholder", "team@example.com" ] []
                    wc "fui-button" [ "slot", "footer"; "variant", "primary" ] [ str "Send invite" ]
                ]
            ]
        ] ]
    | "tooltip" ->
        let sectionLabel txt = p [ Style [ Margin "0 0 0.75rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "2rem") ] ] [
            div [] [
                sectionLabel "Placements"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1.5rem"); CSSProp.Custom("align-items", "center"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
                    wc "fui-tooltip" [ "content", "Top tooltip"; "placement", "top" ] [
                        wc "fui-button" [ "variant", "secondary" ] [ str "Top" ]
                    ]
                    wc "fui-tooltip" [ "content", "Bottom tooltip"; "placement", "bottom" ] [
                        wc "fui-button" [ "variant", "secondary" ] [ str "Bottom" ]
                    ]
                    wc "fui-tooltip" [ "content", "Left tooltip"; "placement", "left" ] [
                        wc "fui-button" [ "variant", "secondary" ] [ str "Left" ]
                    ]
                    wc "fui-tooltip" [ "content", "Right tooltip"; "placement", "right" ] [
                        wc "fui-button" [ "variant", "secondary" ] [ str "Right" ]
                    ]
                ]
            ]
            div [] [
                sectionLabel "On any element"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1.5rem"); CSSProp.Custom("align-items", "center"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
                    wc "fui-tooltip" [ "content", "Save your changes" ] [
                        wc "fui-button" [ "variant", "primary" ] [ str "Save" ]
                    ]
                    wc "fui-tooltip" [ "content", "This field is required"; "placement", "right" ] [
                        wc "fui-input" [ "label", "Email"; "placeholder", "you@example.com" ] []
                    ]
                    wc "fui-tooltip" [ "content", "Online"; "placement", "bottom" ] [
                        wc "fui-badge" [ "variant", "success"; "dot", "" ] [ str "Active" ]
                    ]
                ]
            ]
        ] ]
    | "popover" ->
        let sectionLabel txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1.25rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
            div [] [
                sectionLabel "Filter panel"
                wc "fui-popover" [ "trigger-label", "Filter"; "width", "260px" ] [
                    div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                        wc "fui-select" [ "label", "Status"; "placeholder", "Any"; "options", """[{"value":"active","label":"Active"},{"value":"draft","label":"Draft"},{"value":"archived","label":"Archived"}]""" ] []
                        wc "fui-select" [ "label", "Type"; "placeholder", "Any"; "options", """[{"value":"page","label":"Page"},{"value":"post","label":"Post"},{"value":"media","label":"Media"}]""" ] []
                        wc "fui-button" [ "variant", "primary" ] [ str "Apply filters" ]
                    ]
                ]
            ]
            div [] [
                sectionLabel "Share panel (bottom-end)"
                wc "fui-popover" [ "trigger-label", "Share ▾"; "placement", "bottom-end"; "width", "260px" ] [
                    p [ Style [ Margin "0 0 0.625rem"; FontSize "0.875rem"; CSSProp.Custom("color", "#A0A0A8") ] ] [ str "Invite team members" ]
                    wc "fui-input" [ "placeholder", "name@example.com" ] []
                    div [ Style [ MarginTop "0.75rem"; Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.5rem") ] ] [
                        wc "fui-button" [ "variant", "primary" ] [ str "Send" ]
                        wc "fui-button" [ "variant", "secondary" ] [ str "Copy link" ]
                    ]
                ]
            ]
            div [] [
                sectionLabel "Info popover"
                wc "fui-popover" [ "trigger-label", "What is this?"; "width", "240px" ] [
                    p [ Style [ Margin "0"; FontSize "0.875rem"; LineHeight "1.6"; CSSProp.Custom("color", "#A0A0A8") ] ] [ str "Popovers accept any slotted HTML — text, forms, lists, or custom components." ]
                ]
            ]
        ] ]
    | "confirm-dialog" ->
        let sectionLabel txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1.25rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
            div [] [
                sectionLabel "Default variant"
                wc "fui-confirm-dialog" [
                    "trigger-label",  "Archive"
                    "title",          "Archive this project?"
                    "message",        "This will move it out of your active workspace."
                    "confirm-label",  "Archive"
                    "cancel-label",   "Keep active"
                ] []
            ]
            div [] [
                sectionLabel "Danger variant"
                wc "fui-confirm-dialog" [
                    "trigger-label",  "Delete account"
                    "title",          "Delete your account?"
                    "message",        "This action is permanent. All data will be erased."
                    "confirm-label",  "Yes, delete"
                    "variant",        "danger"
                ] []
            ]
            div [] [
                sectionLabel "With slot content"
                wc "fui-confirm-dialog" [
                    "trigger-label",  "Remove member"
                    "title",          "Remove Jane Smith?"
                    "confirm-label",  "Remove"
                    "variant",        "danger"
                ] [
                    p [ Style [ Margin "0 0 0.5rem"; FontSize "0.875rem"; CSSProp.Custom("color", "#A0A0A8") ] ] [ str "Jane will lose access to:" ]
                    ul [ Style [ Margin "0"; Padding "0 0 0 1.25rem"; FontSize "0.875rem"; CSSProp.Custom("color", "#A0A0A8") ] ] [
                        li [] [ str "All shared projects" ]
                        li [] [ str "Team settings" ]
                        li [] [ str "Billing information" ]
                    ]
                ]
            ]
        ] ]
    | "badge" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Variants" ]
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.5rem"); CSSProp.Custom("flex-wrap", "wrap"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-badge" [] [ str "Neutral" ]
                    wc "fui-badge" [ "variant", "success" ] [ str "Success" ]
                    wc "fui-badge" [ "variant", "warning" ] [ str "Warning" ]
                    wc "fui-badge" [ "variant", "danger"  ] [ str "Danger" ]
                    wc "fui-badge" [ "variant", "info"    ] [ str "Info" ]
                    wc "fui-badge" [ "variant", "accent"  ] [ str "Accent" ]
                ]
            ]
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "With dot indicator" ]
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.5rem"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-badge" [ "variant", "success"; "dot", "" ] [ str "Online" ]
                    wc "fui-badge" [ "variant", "danger";  "dot", "" ] [ str "Offline" ]
                    wc "fui-badge" [ "variant", "warning"; "dot", "" ] [ str "Away" ]
                    wc "fui-badge" [ "variant", "info";    "dot", "" ] [ str "Busy" ]
                ]
            ]
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Sizes" ]
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.5rem"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-badge" [ "variant", "accent"; "size", "sm" ] [ str "Small" ]
                    wc "fui-badge" [ "variant", "accent" ] [ str "Medium" ]
                    wc "fui-badge" [ "variant", "accent"; "size", "lg" ] [ str "Large" ]
                ]
            ]
        ] ]
    | "spinner" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Sizes" ]
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1.5rem"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-spinner" [ "size", "sm" ] []
                    wc "fui-spinner" [] []
                    wc "fui-spinner" [ "size", "lg" ] []
                    wc "fui-spinner" [ "size", "xl" ] []
                ]
            ]
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "With label" ]
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1.5rem"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-spinner" [ "label", "Loading…" ] []
                    wc "fui-spinner" [ "size", "lg"; "label", "Processing…" ] []
                    wc "fui-spinner" [ "size", "xl"; "label", "Please wait" ] []
                ]
            ]
        ] ]
    | "progress" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1rem") ] ] [
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Variants" ]
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.625rem") ] ] [
                    wc "fui-progress" [ "label", "Upload";   "value", "65" ] []
                    wc "fui-progress" [ "label", "Health";   "value", "100"; "variant", "success" ] []
                    wc "fui-progress" [ "label", "Memory";   "value", "55";  "variant", "warning" ] []
                    wc "fui-progress" [ "label", "Storage";  "value", "90";  "variant", "danger"  ] []
                    wc "fui-progress" [ "label", "Network";  "value", "40";  "variant", "info"    ] []
                ]
            ]
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Indeterminate" ]
                wc "fui-progress" [ "label", "Loading…"; "indeterminate", "" ] []
            ]
            div [] [
                p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str "Sizes" ]
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.625rem") ] ] [
                    wc "fui-progress" [ "label", "Small";  "size", "sm"; "value", "60" ] []
                    wc "fui-progress" [ "label", "Medium"; "value", "60" ] []
                    wc "fui-progress" [ "label", "Large";  "size", "lg"; "value", "60" ] []
                ]
            ]
        ] ]
    | "alert" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-alert" [ "variant", "info" ] [ str "Your session will expire in 10 minutes." ]
            wc "fui-alert" [ "variant", "success"; "title", "Changes saved" ] [ str "Your profile has been updated." ]
            wc "fui-alert" [ "variant", "warning"; "title", "Storage almost full" ] [ str "You have used 90% of your quota." ]
            wc "fui-alert" [ "variant", "danger";  "title", "Action required"; "dismissible", "" ] [ str "Please update your billing details." ]
        ] ]
    | "skeleton" ->
        let sectionLabel txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [
                sectionLabel "Rect — block placeholder"
                wc "fui-skeleton" [ "height", "100px" ] []
            ]
            div [] [
                sectionLabel "Text — 3 lines"
                wc "fui-skeleton" [ "variant", "text"; "lines", "3" ] []
            ]
            div [] [
                sectionLabel "Circle — avatar"
                wc "fui-skeleton" [ "variant", "circle"; "width", "56px" ] []
            ]
            div [] [
                sectionLabel "Card loading pattern"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.875rem"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-skeleton" [ "variant", "circle"; "width", "48px" ] []
                    div [ Style [ CSSProp.Custom("flex", "1") ] ] [
                        wc "fui-skeleton" [ "variant", "text"; "lines", "2"; "height", "0.75rem" ] []
                    ]
                ]
            ]
        ] ]
    | "toast" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem"); CSSProp.Custom("max-width", "380px") ] ] [
            wc "fui-toast" [ "variant", "success"; "title", "Saved";   "message", "Your changes have been saved." ] []
            wc "fui-toast" [ "variant", "danger";  "title", "Error";   "message", "Failed to process the request." ] []
            wc "fui-toast" [ "variant", "warning"; "title", "Warning"; "message", "Storage is running low." ] []
            wc "fui-toast" [ "variant", "info";    "title", "Info";    "message", "A new version is available." ] []
        ] ]
    | "empty-state" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [ Style [ CSSProp.Custom("border", "1px solid #2A2A2E"); CSSProp.Custom("border-radius", "8px") ] ] [
                wc "fui-empty-state" [ "title", "No results found"; "description", "Try adjusting your search or filters." ] []
            ]
            div [ Style [ CSSProp.Custom("border", "1px solid #2A2A2E"); CSSProp.Custom("border-radius", "8px") ] ] [
                wc "fui-empty-state" [ "title", "Your inbox is empty"; "description", "Messages from your team will appear here." ] [
                    span [ HTMLAttr.Custom("slot", "icon") ] [ str "📭" ]
                    wc "fui-button" ([ "slot", "action"; "variant", "primary" ]) [ str "Compose message" ]
                ]
            ]
        ] ]
    | "card" ->
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [
                sl "Default — header / body / footer slots"
                wc "fui-card" [] [
                    span [ HTMLAttr.Custom("slot", "header") ] [ str "Card Title" ]
                    p [ Style [ Margin "0" ] ] [ str "Body content rendered in the default slot. Add anything here." ]
                    div [ HTMLAttr.Custom("slot", "footer") ] [
                        wc "fui-button" [ "variant", "primary";   "size", "sm" ] [ str "Save" ]
                        wc "fui-button" [ "variant", "secondary"; "size", "sm" ] [ str "Cancel" ]
                    ]
                ]
            ]
            div [] [
                sl "Variants"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
                    div [ Style [ CSSProp.Custom("flex", "1"); CSSProp.Custom("min-width", "160px") ] ] [
                        sl "Elevated"
                        wc "fui-card" [ "variant", "elevated" ] [
                            p [ Style [ Margin "0" ] ] [ str "Elevated — deeper shadow." ]
                        ]
                    ]
                    div [ Style [ CSSProp.Custom("flex", "1"); CSSProp.Custom("min-width", "160px") ] ] [
                        sl "Outline"
                        wc "fui-card" [ "variant", "outline" ] [
                            p [ Style [ Margin "0" ] ] [ str "Outline — transparent background." ]
                        ]
                    ]
                    div [ Style [ CSSProp.Custom("flex", "1"); CSSProp.Custom("min-width", "160px") ] ] [
                        sl "Ghost"
                        wc "fui-card" [ "variant", "ghost" ] [
                            p [ Style [ Margin "0" ] ] [ str "Ghost — no border." ]
                        ]
                    ]
                ]
            ]
            div [] [
                sl "Padding sizes"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
                    div [ Style [ CSSProp.Custom("flex", "1") ] ] [
                        wc "fui-card" [ "padding", "sm" ] [
                            span [ HTMLAttr.Custom("slot", "header") ] [ str "Small (sm)" ]
                            p [ Style [ Margin "0" ] ] [ str "padding=\"sm\"" ]
                        ]
                    ]
                    div [ Style [ CSSProp.Custom("flex", "1") ] ] [
                        wc "fui-card" [ "padding", "lg" ] [
                            span [ HTMLAttr.Custom("slot", "header") ] [ str "Large (lg)" ]
                            p [ Style [ Margin "0" ] ] [ str "padding=\"lg\"" ]
                        ]
                    ]
                    div [ Style [ CSSProp.Custom("flex", "1") ] ] [
                        wc "fui-card" [ "padding", "none" ] [
                            span [ HTMLAttr.Custom("slot", "header") ] [ str "None" ]
                            p [ Style [ Margin "0" ] ] [ str "padding=\"none\"" ]
                        ]
                    ]
                ]
            ]
        ] ]
    | "stat" ->
        [ div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "1rem"); CSSProp.Custom("flex-wrap", "wrap") ] ] [
            wc "fui-stat" [ "label", "Revenue";    "value", "$48,200"; "change", "12.5%"; "trend", "up";   "description", "vs last month" ] []
            wc "fui-stat" [ "label", "Users";      "value", "8,412";   "change", "3.1%";  "trend", "down" ] []
            wc "fui-stat" [ "label", "Error rate"; "value", "0.02%";   "change", "stable"; "trend", "flat" ] []
            wc "fui-stat" [ "label", "Uptime";     "value", "99.9%" ] []
        ] ]
    | "avatar" ->
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [
                sl "Sizes"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.75rem"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-avatar" [ "initials", "XS"; "size", "xs" ] []
                    wc "fui-avatar" [ "initials", "SM"; "size", "sm" ] []
                    wc "fui-avatar" [ "initials", "MD" ] []
                    wc "fui-avatar" [ "initials", "LG"; "size", "lg" ] []
                    wc "fui-avatar" [ "initials", "XL"; "size", "xl" ] []
                ]
            ]
            div [] [
                sl "Status indicators"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.75rem"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-avatar" [ "initials", "ON"; "status", "online"  ] []
                    wc "fui-avatar" [ "initials", "OF"; "status", "offline" ] []
                    wc "fui-avatar" [ "initials", "AW"; "status", "away"    ] []
                    wc "fui-avatar" [ "initials", "BZ"; "status", "busy"    ] []
                ]
            ]
        ] ]
    | "avatar-group" ->
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        let av5 = """[{"src":"","initials":"JD"},{"src":"","initials":"AB"},{"src":"","initials":"MK"},{"src":"","initials":"RQ"},{"src":"","initials":"TW"}]"""
        let av6 = """[{"src":"","initials":"AA"},{"src":"","initials":"BB"},{"src":"","initials":"CC"},{"src":"","initials":"DD"},{"src":"","initials":"EE"},{"src":"","initials":"FF"}]"""
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [
                sl "Default — max 4"
                wc "fui-avatar-group" [ "avatars", av5 ] []
            ]
            div [] [
                sl "Max 3, 6 avatars → +3 overflow"
                wc "fui-avatar-group" [ "avatars", av6; "max", "3" ] []
            ]
            div [] [
                sl "Sizes"
                div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
                    wc "fui-avatar-group" [ "avatars", av5; "size", "sm" ] []
                    wc "fui-avatar-group" [ "avatars", av5 ] []
                    wc "fui-avatar-group" [ "avatars", av5; "size", "lg" ] []
                ]
            ]
        ] ]
    | "tag" ->
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.25rem") ] ] [
            div [] [
                sl "Variants"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.5rem"); CSSProp.Custom("flex-wrap", "wrap"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-tag" [] [ str "Neutral" ]
                    wc "fui-tag" [ "variant", "success" ] [ str "Success" ]
                    wc "fui-tag" [ "variant", "warning" ] [ str "Warning" ]
                    wc "fui-tag" [ "variant", "danger"  ] [ str "Danger"  ]
                    wc "fui-tag" [ "variant", "info"    ] [ str "Info"    ]
                    wc "fui-tag" [ "variant", "accent"  ] [ str "Accent"  ]
                ]
            ]
            div [] [
                sl "Sizes"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.5rem"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-tag" [ "variant", "accent"; "size", "sm" ] [ str "Small" ]
                    wc "fui-tag" [ "variant", "accent" ] [ str "Medium" ]
                    wc "fui-tag" [ "variant", "accent"; "size", "lg" ] [ str "Large" ]
                ]
            ]
            div [] [
                sl "Removable"
                div [ Style [ Display DisplayOptions.Flex; CSSProp.Custom("gap", "0.5rem"); CSSProp.Custom("flex-wrap", "wrap"); CSSProp.Custom("align-items", "center") ] ] [
                    wc "fui-tag" [ "removable", "" ] [ str "React" ]
                    wc "fui-tag" [ "variant", "success"; "removable", "" ] [ str "TypeScript" ]
                    wc "fui-tag" [ "variant", "accent";  "removable", "" ] [ str "F#" ]
                    wc "fui-tag" [ "variant", "info";    "removable", "" ] [ str "Fable" ]
                ]
            ]
        ] ]
    | "code-block" ->
        let fsCode  = "let add x y = x + y\nprintfn \"Result: %d\" (add 3 4)"
        let cssCode = ".button {\n    background: #7C3AED;\n    border-radius: 6px;\n    color: #fff;\n    padding: 0.5rem 1rem;\n}"
        let sqlCode = "SELECT id, email, role\nFROM users\nWHERE active = true\nORDER BY email;"
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1rem") ] ] [
            wc "fui-code-block" [ "language", "fsharp"; "code", fsCode  ] []
            wc "fui-code-block" [ "language", "css";    "code", cssCode ] []
            wc "fui-code-block" [ "language", "sql";    "code", sqlCode ] []
            wc "fui-code-block" [ "code", "npm install && npm run start" ] []
        ] ]
    | "callout" ->
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "0.75rem") ] ] [
            wc "fui-callout" [ "variant", "info";    "title", "Information" ] [ str "This is an informational callout with a blue accent stripe." ]
            wc "fui-callout" [ "variant", "success"; "title", "Success"     ] [ str "Operation completed successfully — all checks passed." ]
            wc "fui-callout" [ "variant", "warning"; "title", "Warning"     ] [ str "Please review before proceeding — some items need attention." ]
            wc "fui-callout" [ "variant", "danger";  "title", "Error"       ] [ str "Something went wrong. Check the logs for details." ]
            wc "fui-callout" [ "variant", "note";    "title", "Note"        ] [ str "This component uses a purple accent colour for editorial notes." ]
            wc "fui-callout" [ "variant", "info" ] [ str "Callout without a title — body content only via the default slot." ]
        ] ]
    | "list" ->
        let items = """["Build the UI components","Add backend endpoints","Write unit tests","Deploy to production"]"""
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "1.5rem") ] ] [
            div [] [ sl "Default (unordered)"; wc "fui-list" [ "items", items ] [] ]
            div [] [ sl "Ordered";             wc "fui-list" [ "items", items; "ordered", "" ] [] ]
            div [] [ sl "Bordered";            wc "fui-list" [ "items", items; "variant", "bordered" ] [] ]
            div [] [ sl "Striped";             wc "fui-list" [ "items", items; "variant", "striped"  ] [] ]
            div [] [ sl "Bullet";              wc "fui-list" [ "items", items; "variant", "bullet"   ] [] ]
        ] ]
    | "timeline" ->
        let items = """[{"label":"Project started","description":"Initial planning and design phase completed.","date":"Jan 2025","icon":""},{"label":"Alpha release","description":"First internal build shipped to QA.","date":"Mar 2025","icon":""},{"label":"Beta launch","description":"Public beta released with 500 early-access users.","date":"Jun 2025","icon":""},{"label":"v1.0 shipped","description":"General availability — stable release.","date":"Sep 2025","icon":""}]"""
        [ wc "fui-timeline" [ "items", items ] [] ]
    | "accordion" ->
        let items = """[{"label":"What is FableUI?","content":"FableUI is a library of standalone Web Components built with Fable and Lit. Drop any fui-* element into any HTML page and it works immediately."},{"label":"Do I need a JavaScript framework?","content":"No — every component is a native Custom Element and works in plain HTML, React, Vue, Svelte, or any other environment."},{"label":"Can I theme the components?","content":"Yes. All components expose CSS custom properties (--fui-*) so you can override colours, fonts, radii, and spacing from a stylesheet or inline style."},{"label":"How does sorting work in fui-table?","content":"Column sorting is done client-side by the component. Set sortable on the element and mark individual columns as sortable: true in the columns JSON."}]"""
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "2rem") ] ] [
            div [] [
                sl "Single open (default)"
                wc "fui-accordion" [ "items", items ] []
            ]
            div [] [
                sl "Multiple open"
                wc "fui-accordion" [ "items", items; "multiple", "" ] []
            ]
        ] ]
    | "carousel" ->
        let items = """[{"title":"Components","description":"Drop any fui-* element into your HTML and it works immediately — no build step required."},{"title":"Theming","description":"Every component exposes CSS custom properties for full design token control."},{"title":"Accessibility","description":"All interactive elements meet WCAG 2.1 AA keyboard and ARIA requirements."}]"""
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "2rem") ] ] [
            div [] [
                sl "With dot navigation (default)"
                wc "fui-carousel" [ "items", items ] []
            ]
            div [] [
                sl "Loop enabled"
                wc "fui-carousel" [ "items", items; "loop", "" ] []
            ]
        ] ]
    | "table" ->
        let cols = """[{"key":"name","label":"Name","sortable":true},{"key":"email","label":"Email","sortable":true},{"key":"role","label":"Role","sortable":false}]"""
        let rows = """[["Alice Chen","alice@example.com","Admin"],["Bob Smith","bob@example.com","Developer"],["Carol Davis","carol@example.com","Designer"],["Dan Wilson","dan@example.com","Developer"],["Eve Martinez","eve@example.com","Admin"]]"""
        let sl txt = p [ Style [ Margin "0 0 0.5rem"; FontSize "0.75rem"; CSSProp.Custom("color", "#6E6E76"); CSSProp.Custom("text-transform", "uppercase"); CSSProp.Custom("letter-spacing", "0.06em") ] ] [ str txt ]
        [ div [ Style [ Display DisplayOptions.Flex; FlexDirection "column"; CSSProp.Custom("gap", "2rem") ] ] [
            div [] [
                sl "Sortable — click Name or Email header"
                wc "fui-table" [ "columns", cols; "rows", rows; "sortable", "" ] []
            ]
            div [] [
                sl "Striped rows"
                wc "fui-table" [ "columns", cols; "rows", rows; "striped", "" ] []
            ]
            div [] [
                sl "Sortable + striped"
                wc "fui-table" [ "columns", cols; "rows", rows; "sortable", ""; "striped", "" ] []
            ]
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
