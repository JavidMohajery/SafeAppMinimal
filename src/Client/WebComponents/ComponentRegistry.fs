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
    {
        Tag         = "fui-input"
        Name        = "Input"
        Slug        = "input"
        Category    = "Inputs & Forms"
        Description = "Single-line text entry field. Supports text, number, email, password, and search types with optional label, error message, and sm/md/lg sizes."
        Attributes  = [
            { Name="type";        Type="string";  Default="text";  Description="text | number | email | password | search" }
            { Name="value";       Type="string";  Default="";      Description="Current input value (reflected)" }
            { Name="placeholder"; Type="string";  Default="";      Description="Placeholder text shown when empty" }
            { Name="label";       Type="string";  Default="";      Description="Visible label rendered above the input" }
            { Name="error";       Type="string";  Default="";      Description="Error message shown below the input; also sets invalid border" }
            { Name="name";        Type="string";  Default="";      Description="Form field name for native form submission" }
            { Name="disabled";    Type="boolean"; Default="false"; Description="Disables the input — non-interactive, reduced opacity" }
            { Name="required";    Type="boolean"; Default="false"; Description="Marks the field as required for native form validation" }
            { Name="size";        Type="string";  Default="md";    Description="sm | md | lg" }
        ]
        CssProps = [
            { Name="--fui-input-bg";           Description="Input background color" }
            { Name="--fui-input-color";        Description="Input text color" }
            { Name="--fui-input-border";       Description="Border color (idle)" }
            { Name="--fui-input-border-focus"; Description="Border color on focus" }
            { Name="--fui-input-placeholder";  Description="Placeholder text color" }
            { Name="--fui-input-radius";       Description="Border radius" }
            { Name="--fui-input-font";         Description="Font family" }
            { Name="--fui-input-label-color";  Description="Label text color" }
        ]
        Events = [
            { Name="fui-input";  Description="Fired on every keystroke — detail: { value: string }" }
            { Name="fui-change"; Description="Fired on blur/change — detail: { value: string }" }
        ]
        HtmlUsage = """<fui-input label="Email" type="email" placeholder="you@example.com"></fui-input>
<fui-input label="Password" type="password" size="lg"></fui-input>
<fui-input label="Search" type="search" placeholder="Search..."></fui-input>
<fui-input label="Quantity" type="number" value="1" size="sm"></fui-input>
<fui-input label="Username" error="Username is already taken"></fui-input>
<fui-input label="Disabled" disabled value="Can't touch this"></fui-input>"""
    }
    {
        Tag         = "fui-textarea"
        Name        = "Textarea"
        Slug        = "textarea"
        Category    = "Inputs & Forms"
        Description = "Multi-line text entry field with optional label, error message, row count, and resize control."
        Attributes  = [
            { Name="value";       Type="string";  Default="";         Description="Current text value (reflected)" }
            { Name="placeholder"; Type="string";  Default="";         Description="Placeholder shown when empty" }
            { Name="label";       Type="string";  Default="";         Description="Visible label rendered above the textarea" }
            { Name="rows";        Type="number";  Default="3";        Description="Number of visible text rows" }
            { Name="resize";      Type="string";  Default="vertical"; Description="CSS resize behaviour: none | vertical | horizontal | both" }
            { Name="error";       Type="string";  Default="";         Description="Error message below the field; also sets invalid border" }
            { Name="name";        Type="string";  Default="";         Description="Form field name for native form submission" }
            { Name="disabled";    Type="boolean"; Default="false";    Description="Disables the textarea" }
            { Name="required";    Type="boolean"; Default="false";    Description="Marks the field as required" }
            { Name="size";        Type="string";  Default="md";       Description="sm | md | lg" }
        ]
        CssProps = [
            { Name="--fui-textarea-bg";           Description="Background color" }
            { Name="--fui-textarea-color";        Description="Text color" }
            { Name="--fui-textarea-border";       Description="Border color (idle)" }
            { Name="--fui-textarea-border-focus"; Description="Border color on focus" }
            { Name="--fui-textarea-placeholder";  Description="Placeholder text color" }
            { Name="--fui-textarea-radius";       Description="Border radius" }
            { Name="--fui-textarea-font";         Description="Font family" }
            { Name="--fui-textarea-label-color";  Description="Label text color" }
        ]
        Events = [
            { Name="fui-input";  Description="Fired on every keystroke — detail: { value: string }" }
            { Name="fui-change"; Description="Fired on blur/change — detail: { value: string }" }
        ]
        HtmlUsage = """<fui-textarea label="Message" placeholder="Write something..."></fui-textarea>
<fui-textarea label="Notes" rows="6" resize="both"></fui-textarea>
<fui-textarea label="Bio" error="Too short — minimum 20 characters"></fui-textarea>
<fui-textarea label="Locked" disabled value="Cannot edit"></fui-textarea>"""
    }
    {
        Tag         = "fui-select"
        Name        = "Select"
        Slug        = "select"
        Category    = "Inputs & Forms"
        Description = "Native single-select dropdown. Options are supplied as a JSON array via the `options` attribute."
        Attributes  = [
            { Name="options";     Type="string";  Default="[]";  Description="JSON array of {value,label} objects — e.g. '[{\"value\":\"a\",\"label\":\"Alpha\"}]'" }
            { Name="value";       Type="string";  Default="";    Description="Selected option value (reflected)" }
            { Name="label";       Type="string";  Default="";    Description="Visible label above the select" }
            { Name="placeholder"; Type="string";  Default="";    Description="Disabled first option shown when nothing is selected" }
            { Name="error";       Type="string";  Default="";    Description="Error message below the field" }
            { Name="name";        Type="string";  Default="";    Description="Form field name for native form submission" }
            { Name="disabled";    Type="boolean"; Default="false"; Description="Disables the select" }
            { Name="required";    Type="boolean"; Default="false"; Description="Marks the field as required" }
            { Name="size";        Type="string";  Default="md";  Description="sm | md | lg" }
        ]
        CssProps = [
            { Name="--fui-select-bg";           Description="Select background color" }
            { Name="--fui-select-color";        Description="Selected text color" }
            { Name="--fui-select-border";       Description="Border color (idle)" }
            { Name="--fui-select-border-focus"; Description="Border color on focus" }
            { Name="--fui-select-radius";       Description="Border radius" }
            { Name="--fui-select-font";         Description="Font family" }
            { Name="--fui-select-label-color";  Description="Label text color" }
            { Name="--fui-select-arrow-color";  Description="Chevron arrow color" }
        ]
        Events = [
            { Name="fui-change"; Description="Fired on selection change — detail: { value: string }" }
        ]
        HtmlUsage = """<fui-select
  label="Framework"
  placeholder="Pick one..."
  options='[{"value":"react","label":"React"},{"value":"vue","label":"Vue"},{"value":"svelte","label":"Svelte"}]'>
</fui-select>
<fui-select label="Preselected" value="vue"
  options='[{"value":"react","label":"React"},{"value":"vue","label":"Vue"}]'>
</fui-select>
<fui-select label="Disabled" disabled value="react"
  options='[{"value":"react","label":"React"}]'>
</fui-select>"""
    }
    {
        Tag         = "fui-checkbox"
        Name        = "Checkbox"
        Slug        = "checkbox"
        Category    = "Inputs & Forms"
        Description = "Boolean toggle rendered as a custom-styled checkbox. Supports label, error message, disabled state, and sm/md/lg sizes."
        Attributes  = [
            { Name="checked";  Type="boolean"; Default="false"; Description="Whether the checkbox is checked (reflected as property)" }
            { Name="label";    Type="string";  Default="";      Description="Text label rendered beside the checkbox" }
            { Name="error";    Type="string";  Default="";      Description="Error message shown below; also sets invalid border" }
            { Name="name";     Type="string";  Default="";      Description="Form field name for native form submission" }
            { Name="disabled"; Type="boolean"; Default="false"; Description="Disables the checkbox — non-interactive, reduced opacity" }
            { Name="size";     Type="string";  Default="md";    Description="sm | md | lg" }
        ]
        CssProps = [
            { Name="--fui-cb-bg";           Description="Unchecked background color" }
            { Name="--fui-cb-border";       Description="Border color (unchecked)" }
            { Name="--fui-cb-checked-bg";   Description="Background and border when checked" }
            { Name="--fui-cb-border-focus"; Description="Focus ring color" }
            { Name="--fui-cb-radius";       Description="Border radius of the checkbox box" }
            { Name="--fui-cb-label-color";  Description="Label text color" }
            { Name="--fui-cb-font";         Description="Font family" }
        ]
        Events = [
            { Name="fui-change"; Description="Fired on toggle — detail: { checked: bool }" }
        ]
        HtmlUsage = """<fui-checkbox label="Accept terms and conditions"></fui-checkbox>
<fui-checkbox label="Subscribe to newsletter" checked></fui-checkbox>
<fui-checkbox label="Disabled option" disabled></fui-checkbox>
<fui-checkbox label="With error" error="This field is required"></fui-checkbox>"""
    }
    {
        Tag         = "fui-radio"
        Name        = "Radio"
        Slug        = "radio"
        Category    = "Inputs & Forms"
        Description = "Single radio button with label, checked state, disabled support, and sm/md/lg sizes. Use fui-radio-group for coordinated multi-option selection."
        Attributes  = [
            { Name="value";    Type="string";  Default="";     Description="The value this radio represents" }
            { Name="label";    Type="string";  Default="";     Description="Text label rendered beside the radio" }
            { Name="checked";  Type="boolean"; Default="false"; Description="Whether this radio is selected" }
            { Name="name";     Type="string";  Default="";     Description="Radio group name for native grouping within the same shadow root" }
            { Name="disabled"; Type="boolean"; Default="false"; Description="Disables the radio — non-interactive, reduced opacity" }
            { Name="size";     Type="string";  Default="md";   Description="sm | md | lg" }
        ]
        CssProps = [
            { Name="--fui-radio-bg";           Description="Unchecked background color" }
            { Name="--fui-radio-border";       Description="Border color (unchecked)" }
            { Name="--fui-radio-checked-bg";   Description="Background and border color when checked" }
            { Name="--fui-radio-border-focus"; Description="Focus ring color" }
            { Name="--fui-radio-label-color";  Description="Label text color" }
            { Name="--fui-radio-font";         Description="Font family" }
        ]
        Events = [
            { Name="fui-change"; Description="Fired on selection — detail: { value: string, checked: bool }" }
        ]
        HtmlUsage = """<fui-radio name="color" value="red"   label="Red"></fui-radio>
<fui-radio name="color" value="green" label="Green" checked></fui-radio>
<fui-radio name="color" value="blue"  label="Blue"  disabled></fui-radio>"""
    }
    {
        Tag         = "fui-radio-group"
        Name        = "RadioGroup"
        Slug        = "radio-group"
        Category    = "Inputs & Forms"
        Description = "Coordinated set of radio buttons supplied as a JSON options array. Manages selection state and fires fui-change when the selection changes."
        Attributes  = [
            { Name="options";  Type="string";  Default="[]";   Description="JSON array of {value, label} objects" }
            { Name="value";    Type="string";  Default="";     Description="Currently selected value (reflected)" }
            { Name="label";    Type="string";  Default="";     Description="Visible group label above the options" }
            { Name="name";     Type="string";  Default="";     Description="Native radio input group name (defaults to 'rg')" }
            { Name="disabled"; Type="boolean"; Default="false"; Description="Disables all options" }
            { Name="error";    Type="string";  Default="";     Description="Error message shown below the options" }
            { Name="size";     Type="string";  Default="md";   Description="sm | md | lg" }
        ]
        CssProps = [
            { Name="--fui-rg-bg";           Description="Radio circle background (unchecked)" }
            { Name="--fui-rg-border";       Description="Circle border color (unchecked)" }
            { Name="--fui-rg-checked-bg";   Description="Circle background when checked" }
            { Name="--fui-rg-border-focus"; Description="Focus ring color" }
            { Name="--fui-rg-label-color";  Description="Group label color" }
            { Name="--fui-rg-option-color"; Description="Option label text color" }
            { Name="--fui-rg-font";         Description="Font family" }
        ]
        Events = [
            { Name="fui-change"; Description="Fired on selection change — detail: { value: string }" }
        ]
        HtmlUsage = """<fui-radio-group
  label="Framework"
  name="framework"
  options='[{"value":"react","label":"React"},{"value":"vue","label":"Vue"},{"value":"svelte","label":"Svelte"}]'>
</fui-radio-group>
<fui-radio-group
  label="Preselected"
  name="size"
  value="md"
  options='[{"value":"sm","label":"Small"},{"value":"md","label":"Medium"},{"value":"lg","label":"Large"}]'>
</fui-radio-group>
<fui-radio-group
  label="Disabled"
  disabled
  value="react"
  options='[{"value":"react","label":"React"},{"value":"vue","label":"Vue"}]'>
</fui-radio-group>"""
    }
    {
        Tag         = "fui-toggle"
        Name        = "Toggle"
        Slug        = "toggle"
        Category    = "Inputs & Forms"
        Description = "On/off switch rendered as a sliding pill. Fires fui-change on every toggle. Supports label, error message, disabled state, and sm/md/lg sizes."
        Attributes  = [
            { Name="checked";  Type="boolean"; Default="false"; Description="Whether the toggle is on" }
            { Name="label";    Type="string";  Default="";      Description="Text label rendered beside the track" }
            { Name="error";    Type="string";  Default="";      Description="Error message shown below the toggle" }
            { Name="name";     Type="string";  Default="";      Description="Form field name for native form submission" }
            { Name="disabled"; Type="boolean"; Default="false"; Description="Disables the toggle — non-interactive, reduced opacity" }
            { Name="size";     Type="string";  Default="md";    Description="sm | md | lg" }
        ]
        CssProps = [
            { Name="--fui-toggle-track-bg";      Description="Track background when off" }
            { Name="--fui-toggle-border";        Description="Track border color when off" }
            { Name="--fui-toggle-track-checked"; Description="Track background and border when on" }
            { Name="--fui-toggle-thumb-off";     Description="Thumb color when off" }
            { Name="--fui-toggle-thumb-on";      Description="Thumb color when on" }
            { Name="--fui-toggle-border-focus";  Description="Focus ring color" }
            { Name="--fui-toggle-label-color";   Description="Label text color" }
            { Name="--fui-toggle-font";          Description="Font family" }
        ]
        Events = [
            { Name="fui-change"; Description="Fired on every toggle — detail: { checked: bool }" }
        ]
        HtmlUsage = """<fui-toggle label="Enable notifications"></fui-toggle>
<fui-toggle label="Dark mode" checked></fui-toggle>
<fui-toggle label="Disabled off" disabled></fui-toggle>
<fui-toggle label="Disabled on" checked disabled></fui-toggle>
<fui-toggle label="With error" error="This setting is required"></fui-toggle>"""
    }
    {
        Tag         = "fui-slider"
        Name        = "Slider"
        Slug        = "slider"
        Category    = "Inputs & Forms"
        Description = "Range slider with a filled track that follows the thumb. Supports min, max, step, label with live value readout, error message, disabled state, and sm/md/lg sizes."
        Attributes  = [
            { Name="value";    Type="number";  Default="0";    Description="Current value" }
            { Name="min";      Type="number";  Default="0";    Description="Minimum value" }
            { Name="max";      Type="number";  Default="100";  Description="Maximum value" }
            { Name="step";     Type="number";  Default="1";    Description="Step increment" }
            { Name="label";    Type="string";  Default="";     Description="Visible label rendered above the slider; also shows the live value on the right" }
            { Name="disabled"; Type="boolean"; Default="false"; Description="Disables the slider — non-interactive, reduced opacity" }
            { Name="name";     Type="string";  Default="";     Description="Form field name for native form submission" }
            { Name="error";    Type="string";  Default="";     Description="Error message shown below the slider" }
            { Name="size";     Type="string";  Default="md";   Description="sm | md | lg" }
        ]
        CssProps = [
            { Name="--fui-slider-track-bg";    Description="Track background (unfilled portion)" }
            { Name="--fui-slider-fill";        Description="Track fill color (filled portion)" }
            { Name="--fui-slider-thumb";       Description="Thumb circle color" }
            { Name="--fui-slider-thumb-border";Description="Thumb border color" }
            { Name="--fui-slider-border-focus";Description="Focus ring color" }
            { Name="--fui-slider-label-color"; Description="Label text color" }
            { Name="--fui-slider-value-color"; Description="Live value readout color" }
            { Name="--fui-slider-font";        Description="Font family" }
        ]
        Events = [
            { Name="fui-input";  Description="Fired on every thumb movement — detail: { value: number }" }
            { Name="fui-change"; Description="Fired when the thumb is released — detail: { value: number }" }
        ]
        HtmlUsage = """<fui-slider label="Volume" value="40"></fui-slider>
<fui-slider label="Price range" min="100" max="1000" step="50" value="400"></fui-slider>
<fui-slider label="Opacity" min="0" max="1" step="0.01" value="0.75"></fui-slider>
<fui-slider label="Disabled" value="60" disabled></fui-slider>"""
    }
    {
        Tag         = "fui-date-picker"
        Name        = "DatePicker"
        Slug        = "date-picker"
        Category    = "Inputs & Forms"
        Description = "Date selection field backed by a native <input type=\"date\">. The browser provides an accessible calendar popup; the component wraps it with consistent label, error, and size styling."
        Attributes  = [
            { Name="value";    Type="string";  Default="";     Description="Selected date in YYYY-MM-DD format (reflected)" }
            { Name="min";      Type="string";  Default="";     Description="Earliest selectable date (YYYY-MM-DD)" }
            { Name="max";      Type="string";  Default="";     Description="Latest selectable date (YYYY-MM-DD)" }
            { Name="label";    Type="string";  Default="";     Description="Visible label rendered above the field" }
            { Name="disabled"; Type="boolean"; Default="false"; Description="Disables the picker — non-interactive, reduced opacity" }
            { Name="required"; Type="boolean"; Default="false"; Description="Marks the field as required for native form validation" }
            { Name="name";     Type="string";  Default="";     Description="Form field name for native form submission" }
            { Name="error";    Type="string";  Default="";     Description="Error message shown below the field; also sets invalid border" }
            { Name="size";     Type="string";  Default="md";   Description="sm | md | lg" }
        ]
        CssProps = [
            { Name="--fui-dp-bg";           Description="Input background color" }
            { Name="--fui-dp-color";        Description="Input text color" }
            { Name="--fui-dp-border";       Description="Border color (idle)" }
            { Name="--fui-dp-border-focus"; Description="Border color on focus" }
            { Name="--fui-dp-icon-color";   Description="Calendar icon color" }
            { Name="--fui-dp-label-color";  Description="Label text color" }
            { Name="--fui-dp-radius";       Description="Border radius" }
            { Name="--fui-dp-font";         Description="Font family" }
        ]
        Events = [
            { Name="fui-change"; Description="Fired on date selection — detail: { value: string } (YYYY-MM-DD)" }
        ]
        HtmlUsage = """<fui-date-picker label="Date of birth"></fui-date-picker>
<fui-date-picker label="Check-in" value="2025-06-01" min="2025-01-01" max="2025-12-31"></fui-date-picker>
<fui-date-picker label="Disabled" value="2025-03-15" disabled></fui-date-picker>
<fui-date-picker label="With error" error="Please select a valid date"></fui-date-picker>"""
    }
    {
        Tag         = "fui-color-picker"
        Name        = "ColorPicker"
        Slug        = "color-picker"
        Category    = "Inputs & Forms"
        Description = "Color selection control showing a live swatch and uppercase hex value. Backed by a native <input type=\"color\"> stretched invisibly over the wrapper so any click opens the browser color picker."
        Attributes  = [
            { Name="value";    Type="string";  Default="#7C3AED"; Description="Selected color as a hex string (e.g. #FF5733)" }
            { Name="label";    Type="string";  Default="";         Description="Visible label rendered above the control" }
            { Name="disabled"; Type="boolean"; Default="false";    Description="Disables the picker — non-interactive, reduced opacity" }
            { Name="name";     Type="string";  Default="";         Description="Form field name for native form submission" }
            { Name="error";    Type="string";  Default="";         Description="Error message shown below the control; also sets invalid border" }
            { Name="size";     Type="string";  Default="md";       Description="sm | md | lg" }
        ]
        CssProps = [
            { Name="--fui-cp-bg";           Description="Control background color" }
            { Name="--fui-cp-border";       Description="Border color (idle)" }
            { Name="--fui-cp-border-focus"; Description="Border and focus ring color" }
            { Name="--fui-cp-value-color";  Description="Hex value text color" }
            { Name="--fui-cp-label-color";  Description="Label text color" }
            { Name="--fui-cp-radius";       Description="Border radius" }
            { Name="--fui-cp-font";         Description="Font family" }
        ]
        Events = [
            { Name="fui-input";  Description="Fired continuously while dragging — detail: { value: string }" }
            { Name="fui-change"; Description="Fired on confirmed selection — detail: { value: string }" }
        ]
        HtmlUsage = """<fui-color-picker label="Brand colour" value="#7C3AED"></fui-color-picker>
<fui-color-picker label="Accent" value="#3B82F6"></fui-color-picker>
<fui-color-picker label="Disabled" value="#22C55E" disabled></fui-color-picker>
<fui-color-picker label="With error" error="A colour is required"></fui-color-picker>"""
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
    WebComponents.FuiInput.register ()
    WebComponents.FuiTextarea.register ()
    WebComponents.FuiSelect.register ()
    WebComponents.FuiCheckbox.register ()
    WebComponents.FuiRadioGroup.register ()
    WebComponents.FuiToggle.register ()
    WebComponents.FuiSlider.register ()
    WebComponents.FuiDatePicker.register ()
    WebComponents.FuiColorPicker.register ()
