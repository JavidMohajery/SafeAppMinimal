module ComponentRegistry

// ── Metadata types ────────────────────────────────────────────────────────────

type AttrDef = {
    Name        : string
    Type        : string
    Default     : string
    Description : string
}

type CssPropDef = {
    Name        : string
    Description : string
}

type EventDef = {
    Name        : string
    Description : string
}

type ComponentMeta = {
    Tag         : string
    Name        : string
    Slug        : string
    Category    : string
    Description : string
    Attributes  : AttrDef list
    CssProps    : CssPropDef list
    Events      : EventDef list
    HtmlUsage   : string
}

// ── Registry ──────────────────────────────────────────────────────────────────

let all : ComponentMeta list = [
    {
        Tag         = "fui-button"
        Name        = "Button"
        Slug        = "button"
        Category    = "Inputs & Forms"
        Description = "Clickable button with primary, secondary, ghost, and danger variants plus sm/md/lg sizes. Fully keyboard-accessible; supports prefix and suffix slots."
        Attributes  = [
            { Name="variant";  Type="string";  Default="primary"; Description="primary | secondary | ghost | danger" }
            { Name="disabled"; Type="boolean"; Default="false";   Description="Disables the button — non-interactive, reduced opacity" }
            { Name="size";     Type="string";  Default="md";      Description="sm | md | lg" }
        ]
        CssProps = [
            { Name="--fui-button-bg";         Description="Primary background color" }
            { Name="--fui-button-bg-hover";   Description="Background on hover" }
            { Name="--fui-button-color";      Description="Text color" }
            { Name="--fui-button-border";     Description="Border shorthand (default: none)" }
            { Name="--fui-button-radius";     Description="Border radius" }
            { Name="--fui-button-font";       Description="Font family" }
            { Name="--fui-button-danger-bg";  Description="Danger variant background" }
            { Name="--fui-button-ghost-color";Description="Ghost variant text color" }
        ]
        Events  = [
            { Name="click"; Description="Native browser click event — no custom wrapper needed" }
        ]
        HtmlUsage = """<fui-button variant="primary">Save changes</fui-button>
<fui-button variant="secondary">Cancel</fui-button>
<fui-button variant="ghost" size="sm">
  <span slot="prefix">←</span> Back
</fui-button>
<fui-button variant="danger" disabled>Delete</fui-button>"""
    }
]

// ── Lookup helpers ────────────────────────────────────────────────────────────

let bySlug (slug: string) =
    all |> List.tryFind (fun m -> m.Slug = slug)

let byCategory (cat: string) =
    all |> List.filter (fun m -> m.Category = cat)

let categories =
    all |> List.map (fun m -> m.Category) |> List.distinct

// ── Registration ──────────────────────────────────────────────────────────────
// Calling register() on each module forces its [<LitElement>] decorator to run,
// which calls customElements.define() in the browser.

let registerAll () =
    WebComponents.FuiButton.register ()
