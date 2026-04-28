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
    {
        Tag         = "fui-file-upload"
        Name        = "FileUpload"
        Slug        = "file-upload"
        Category    = "Inputs & Forms"
        Description = "Drag-and-drop file upload zone. Click to open the system file picker or drop files onto the zone. Selected file names are listed below the zone. Supports accept filter, multiple selection, disabled state, and sm/md/lg sizes."
        Attributes  = [
            { Name="label";    Type="string";  Default="";     Description="Visible label rendered above the dropzone" }
            { Name="accept";   Type="string";  Default="";     Description="Accepted file types — passed to the native input (e.g. image/* or .pdf,.docx)" }
            { Name="multiple"; Type="boolean"; Default="false"; Description="Allows selecting more than one file at a time" }
            { Name="disabled"; Type="boolean"; Default="false"; Description="Disables the zone — non-interactive, reduced opacity" }
            { Name="name";     Type="string";  Default="";     Description="Form field name for native form submission" }
            { Name="error";    Type="string";  Default="";     Description="Error message shown below the file list" }
            { Name="size";     Type="string";  Default="md";   Description="sm | md | lg" }
        ]
        CssProps = [
            { Name="--fui-fu-bg";           Description="Dropzone background (idle)" }
            { Name="--fui-fu-bg-active";    Description="Dropzone background while dragging over" }
            { Name="--fui-fu-border";       Description="Dashed border color (idle)" }
            { Name="--fui-fu-border-hover"; Description="Border color on hover" }
            { Name="--fui-fu-border-active";Description="Border color while dragging over" }
            { Name="--fui-fu-icon-color";   Description="Upload arrow icon color" }
            { Name="--fui-fu-text-color";   Description="Primary dropzone text color" }
            { Name="--fui-fu-accent";       Description="'Click to browse' link color" }
            { Name="--fui-fu-hint-color";   Description="Accept hint text color" }
            { Name="--fui-fu-file-bg";      Description="File pill background" }
            { Name="--fui-fu-file-color";   Description="File pill text color" }
            { Name="--fui-fu-label-color";  Description="Label text color" }
            { Name="--fui-fu-radius";       Description="Dropzone border radius" }
            { Name="--fui-fu-font";         Description="Font family" }
        ]
        Events = [
            { Name="fui-change"; Description="Fired after file selection or drop — detail: { files: string[], count: number }" }
        ]
        HtmlUsage = """<fui-file-upload label="Attachment"></fui-file-upload>
<fui-file-upload label="Images only" accept="image/*" multiple></fui-file-upload>
<fui-file-upload label="Documents" accept=".pdf,.docx,.xlsx"></fui-file-upload>
<fui-file-upload label="Disabled" disabled></fui-file-upload>
<fui-file-upload label="With error" error="Please upload a file"></fui-file-upload>"""
    }
    // ── Feedback & Status ─────────────────────────────────────────────────────
    {
        Tag         = "fui-badge"
        Name        = "Badge"
        Slug        = "badge"
        Category    = "Feedback"
        Description = "Compact pill label for status, counts, and category tags. Six colour variants, three sizes, and an optional dot indicator."
        Attributes  = [
            { Name="variant"; Type="string";  Default="neutral"; Description="neutral | success | warning | danger | info | accent" }
            { Name="size";    Type="string";  Default="md";      Description="sm | md | lg" }
            { Name="dot";     Type="boolean"; Default="false";   Description="Prefix a filled circle dot before the label text" }
        ]
        CssProps = [
            { Name="--fui-badge-bg";     Description="Background colour (neutral default)" }
            { Name="--fui-badge-border"; Description="Border colour (neutral default)" }
            { Name="--fui-badge-color";  Description="Text colour (neutral default)" }
            { Name="--fui-badge-font";   Description="Font family" }
        ]
        Events   = []
        HtmlUsage = """<fui-badge>Neutral</fui-badge>
<fui-badge variant="success">Success</fui-badge>
<fui-badge variant="warning">Warning</fui-badge>
<fui-badge variant="danger">Danger</fui-badge>
<fui-badge variant="info">Info</fui-badge>
<fui-badge variant="accent">Accent</fui-badge>

<!-- With dot indicator -->
<fui-badge variant="success" dot>Online</fui-badge>
<fui-badge variant="danger"  dot>Offline</fui-badge>

<!-- Sizes -->
<fui-badge variant="accent" size="sm">Small</fui-badge>
<fui-badge variant="accent" size="lg">Large</fui-badge>"""
    }
    {
        Tag         = "fui-spinner"
        Name        = "Spinner"
        Slug        = "spinner"
        Category    = "Feedback"
        Description = "Animated loading indicator — a rotating ring with an optional visible label. Four sizes (sm / md / lg / xl). Respects prefers-reduced-motion."
        Attributes  = [
            { Name="size";  Type="string"; Default="md"; Description="sm | md | lg | xl" }
            { Name="label"; Type="string"; Default="";   Description="Visible text label shown beside the ring; also used as aria-label" }
        ]
        CssProps = [
            { Name="--fui-spinner-color";       Description="Ring foreground colour (the spinning arc)" }
            { Name="--fui-spinner-track";       Description="Ring track colour (the static background arc)" }
            { Name="--fui-spinner-label-color"; Description="Label text colour" }
            { Name="--fui-spinner-font";        Description="Font family" }
        ]
        Events   = []
        HtmlUsage = """<fui-spinner></fui-spinner>
<fui-spinner size="lg"></fui-spinner>
<fui-spinner size="xl" label="Loading data…"></fui-spinner>"""
    }
    {
        Tag         = "fui-progress"
        Name        = "Progress"
        Slug        = "progress"
        Category    = "Feedback"
        Description = "Horizontal progress bar with optional label and live value readout. Supports determinate (value/max) and indeterminate modes, five variants, and three track sizes."
        Attributes  = [
            { Name="value";         Type="number";  Default="0";       Description="Current progress value" }
            { Name="max";           Type="number";  Default="100";     Description="Maximum value (default 100)" }
            { Name="label";         Type="string";  Default="";        Description="Label shown above the bar; also renders a live percentage on the right" }
            { Name="size";          Type="string";  Default="md";      Description="sm (3px) | md (6px) | lg (10px)" }
            { Name="variant";       Type="string";  Default="default"; Description="default | success | warning | danger | info" }
            { Name="indeterminate"; Type="boolean"; Default="false";   Description="Animated sliding bar — use when total duration is unknown" }
        ]
        CssProps = [
            { Name="--fui-progress-track"; Description="Track (background) colour" }
            { Name="--fui-progress-fill";  Description="Fill colour (default variant)" }
            { Name="--fui-progress-label-color"; Description="Label text colour" }
            { Name="--fui-progress-value-color"; Description="Percentage value text colour" }
            { Name="--fui-progress-font";        Description="Font family" }
        ]
        Events   = []
        HtmlUsage = """<fui-progress label="Upload" value="65"></fui-progress>
<fui-progress label="Storage" value="90" variant="danger"></fui-progress>
<fui-progress label="Memory"  value="40" variant="warning"></fui-progress>
<fui-progress label="Health"  value="100" variant="success"></fui-progress>

<!-- Indeterminate -->
<fui-progress label="Loading…" indeterminate></fui-progress>"""
    }
    {
        Tag         = "fui-alert"
        Name        = "Alert"
        Slug        = "alert"
        Category    = "Feedback"
        Description = "Inline alert banner for contextual feedback. Four semantic variants each with a matching icon, left accent stripe, and background tint. Optionally dismissible."
        Attributes  = [
            { Name="variant";     Type="string";  Default="info";  Description="info | success | warning | danger" }
            { Name="title";       Type="string";  Default="";      Description="Optional bold heading above the message" }
            { Name="dismissible"; Type="boolean"; Default="false"; Description="Shows a × close button; fires fui-dismiss on click" }
        ]
        CssProps = [
            { Name="--fui-alert-color";        Description="Message text colour" }
            { Name="--fui-alert-title-color";  Description="Title text colour" }
            { Name="--fui-alert-dismiss-color";Description="Dismiss button colour" }
            { Name="--fui-alert-radius";       Description="Border radius" }
            { Name="--fui-alert-font";         Description="Font family" }
        ]
        Events = [
            { Name="fui-dismiss"; Description="Fired when the dismiss button is clicked — detail: { variant: string }" }
        ]
        HtmlUsage = """<fui-alert variant="info">Your session will expire in 10 minutes.</fui-alert>
<fui-alert variant="success" title="Changes saved">Your profile has been updated.</fui-alert>
<fui-alert variant="warning" title="Storage almost full">You have used 90% of your quota.</fui-alert>
<fui-alert variant="danger"  title="Action required" dismissible>
  Please update your billing details.
</fui-alert>"""
    }
    {
        Tag         = "fui-skeleton"
        Name        = "Skeleton"
        Slug        = "skeleton"
        Category    = "Feedback"
        Description = "Animated shimmer placeholder used while content is loading. Three variants: rect (default), text (one or more lines with a shorter last line), and circle. Respects prefers-reduced-motion."
        Attributes  = [
            { Name="variant"; Type="string"; Default="rect"; Description="rect | text | circle" }
            { Name="width";   Type="string"; Default="";     Description="Explicit width (e.g. 200px). Defaults to 100% for rect/text, 40px for circle." }
            { Name="height";  Type="string"; Default="";     Description="Explicit height (e.g. 80px). Defaults to 1rem for rect, 0.875rem for text lines." }
            { Name="lines";   Type="number"; Default="1";    Description="Number of text lines (text variant only)" }
        ]
        CssProps = [
            { Name="--fui-sk-base";   Description="Base skeleton colour" }
            { Name="--fui-sk-shine";  Description="Shimmer highlight colour" }
            { Name="--fui-sk-radius"; Description="Border radius (rect variant)" }
        ]
        Events   = []
        HtmlUsage = """<!-- Rect placeholder -->
<fui-skeleton height="120px"></fui-skeleton>

<!-- Text — 3 lines -->
<fui-skeleton variant="text" lines="3"></fui-skeleton>

<!-- Circle avatar -->
<fui-skeleton variant="circle" width="48px"></fui-skeleton>

<!-- Card loading pattern -->
<div style="display:flex;gap:0.75rem;align-items:center">
  <fui-skeleton variant="circle" width="40px"></fui-skeleton>
  <div style="flex:1">
    <fui-skeleton variant="text" lines="2" height="0.75rem"></fui-skeleton>
  </div>
</div>"""
    }
    {
        Tag         = "fui-toast"
        Name        = "Toast"
        Slug        = "toast"
        Category    = "Feedback"
        Description = "Ephemeral notification card with a left accent stripe, icon, title, message, and close button. Place inside a positioned container (e.g. bottom-right corner) for toast stacks."
        Attributes  = [
            { Name="variant"; Type="string"; Default="info"; Description="info | success | warning | danger" }
            { Name="title";   Type="string"; Default="";     Description="Optional bold heading above the message" }
            { Name="message"; Type="string"; Default="";     Description="Notification text — or use default slot for rich content" }
        ]
        CssProps = [
            { Name="--fui-toast-bg";          Description="Card background colour" }
            { Name="--fui-toast-border";      Description="Card border colour" }
            { Name="--fui-toast-title-color"; Description="Title text colour" }
            { Name="--fui-toast-color";       Description="Message text colour" }
            { Name="--fui-toast-close-color"; Description="Close button colour" }
            { Name="--fui-toast-radius";      Description="Border radius" }
            { Name="--fui-toast-font";        Description="Font family" }
        ]
        Events = [
            { Name="fui-dismiss"; Description="Fired when the close button is clicked — detail: { variant: string }" }
        ]
        HtmlUsage = """<fui-toast variant="success" title="Saved" message="Your changes have been saved."></fui-toast>
<fui-toast variant="danger"  title="Error"  message="Failed to process the request."></fui-toast>
<fui-toast variant="warning" title="Warning">Storage is running low.</fui-toast>"""
    }
    {
        Tag         = "fui-empty-state"
        Name        = "EmptyState"
        Slug        = "empty-state"
        Category    = "Feedback"
        Description = "Zero-data placeholder displayed when a list, table, or search result set is empty. Composed of an icon slot, title, description, and an action slot for a primary CTA."
        Attributes  = [
            { Name="title";       Type="string"; Default="Nothing here yet"; Description="Primary heading text" }
            { Name="description"; Type="string"; Default="";                  Description="Supporting text shown below the title" }
        ]
        CssProps = [
            { Name="--fui-es-icon-bg";     Description="Icon wrapper background" }
            { Name="--fui-es-icon-border"; Description="Icon wrapper border colour" }
            { Name="--fui-es-icon-color";  Description="Default icon colour" }
            { Name="--fui-es-title-color"; Description="Title text colour" }
            { Name="--fui-es-desc-color";  Description="Description text colour" }
            { Name="--fui-es-padding";     Description="Outer padding (default 3rem 1.5rem)" }
            { Name="--fui-es-font";        Description="Font family" }
        ]
        Events   = []
        HtmlUsage = """<!-- Minimal -->
<fui-empty-state title="No results found"></fui-empty-state>

<!-- With description -->
<fui-empty-state
  title="No files yet"
  description="Upload your first file to get started.">
</fui-empty-state>

<!-- With custom icon and action -->
<fui-empty-state
  title="Your inbox is empty"
  description="Messages from your team will appear here.">
  <span slot="icon">📭</span>
  <fui-button slot="action" variant="primary">Compose message</fui-button>
</fui-empty-state>"""
    }
    // ── Overlay & Floating ────────────────────────────────────────────────────
    {
        Tag         = "fui-modal"
        Name        = "Modal"
        Slug        = "modal"
        Category    = "Overlay"
        Description = "A modal dialog with a backdrop, header, scrollable body, and footer slot. Click the trigger button to open; close by clicking the backdrop or the × button. Fires fui-open and fui-close events."
        Attributes  = [
            { Name="trigger-label"; Type="string"; Default="Open"; Description="Label on the trigger button that opens the dialog" }
            { Name="title";         Type="string"; Default="";     Description="Heading rendered in the modal header" }
            { Name="size";          Type="string"; Default="md";   Description="sm (400px) | md (560px) | lg (720px)" }
        ]
        CssProps = [
            { Name="--fui-modal-bg";           Description="Panel background colour" }
            { Name="--fui-modal-border";       Description="Panel and divider border colour" }
            { Name="--fui-modal-title-color";  Description="Header title colour" }
            { Name="--fui-modal-color";        Description="Body text colour" }
            { Name="--fui-modal-close-color";  Description="Close button colour" }
            { Name="--fui-modal-radius";       Description="Panel border radius" }
            { Name="--fui-modal-trigger-bg";   Description="Trigger button background" }
            { Name="--fui-modal-trigger-color";Description="Trigger button text colour" }
            { Name="--fui-modal-font";         Description="Font family" }
        ]
        Events = [
            { Name="fui-open";  Description="Fired when the dialog opens" }
            { Name="fui-close"; Description="Fired when the dialog closes (backdrop or × button)" }
        ]
        HtmlUsage = """<fui-modal trigger-label="Open modal" title="Confirm action" size="md">
  <p>Modal body content goes here. The footer slot accepts action buttons.</p>
  <fui-button slot="footer" variant="primary">Save</fui-button>
  <fui-button slot="footer" variant="secondary">Cancel</fui-button>
</fui-modal>

<!-- Small modal -->
<fui-modal trigger-label="Quick view" title="Details" size="sm">
  <p>Compact content.</p>
</fui-modal>"""
    }
    {
        Tag         = "fui-drawer"
        Name        = "Drawer"
        Slug        = "drawer"
        Category    = "Overlay"
        Description = "A slide-in panel anchored to one edge of the viewport. Four placement options (right, left, top, bottom). Closes on backdrop click or × button. Fires fui-open and fui-close events."
        Attributes  = [
            { Name="trigger-label"; Type="string"; Default="Open drawer"; Description="Label on the trigger button that opens the drawer" }
            { Name="title";         Type="string"; Default="";            Description="Heading rendered in the drawer header" }
            { Name="placement";     Type="string"; Default="right";       Description="right | left | top | bottom" }
        ]
        CssProps = [
            { Name="--fui-drawer-bg";             Description="Panel background colour" }
            { Name="--fui-drawer-border";         Description="Panel and divider border colour" }
            { Name="--fui-drawer-title-color";    Description="Header title colour" }
            { Name="--fui-drawer-color";          Description="Body text colour" }
            { Name="--fui-drawer-close-color";    Description="Close button colour" }
            { Name="--fui-drawer-size";           Description="Panel width (left/right) or height (top/bottom) — default 400px / 360px" }
            { Name="--fui-drawer-trigger-bg";     Description="Trigger button background" }
            { Name="--fui-drawer-trigger-border"; Description="Trigger button border colour" }
            { Name="--fui-drawer-trigger-color";  Description="Trigger button text colour" }
            { Name="--fui-drawer-font";           Description="Font family" }
        ]
        Events = [
            { Name="fui-open";  Description="Fired when the drawer opens" }
            { Name="fui-close"; Description="Fired when the drawer closes" }
        ]
        HtmlUsage = """<!-- Right drawer (default) -->
<fui-drawer trigger-label="Open settings" title="Settings" placement="right">
  <p>Drawer body content — scrollable when tall.</p>
  <fui-button slot="footer" variant="primary">Apply</fui-button>
</fui-drawer>

<!-- Left navigation drawer -->
<fui-drawer trigger-label="Open nav" title="Navigation" placement="left">
  <p>Nav links go here.</p>
</fui-drawer>

<!-- Bottom sheet -->
<fui-drawer trigger-label="Open sheet" title="Options" placement="bottom">
  <p>Mobile-style bottom sheet.</p>
</fui-drawer>"""
    }
    {
        Tag         = "fui-tooltip"
        Name        = "Tooltip"
        Slug        = "tooltip"
        Category    = "Overlay"
        Description = "Lightweight text hint shown on hover or focus above, below, left, or right of any slotted trigger element. No JavaScript positioning — uses CSS absolute placement."
        Attributes  = [
            { Name="content";   Type="string"; Default="";    Description="Tooltip text to display" }
            { Name="placement"; Type="string"; Default="top"; Description="top | bottom | left | right" }
        ]
        CssProps = [
            { Name="--fui-tooltip-bg";     Description="Tooltip background colour" }
            { Name="--fui-tooltip-color";  Description="Tooltip text colour" }
            { Name="--fui-tooltip-radius"; Description="Tooltip border radius" }
            { Name="--fui-tooltip-font";   Description="Font family" }
        ]
        Events   = []
        HtmlUsage = """<!-- Top (default) -->
<fui-tooltip content="Save your changes">
  <fui-button variant="primary">Save</fui-button>
</fui-tooltip>

<!-- Bottom -->
<fui-tooltip content="Opens in a new tab" placement="bottom">
  <a href="#">Docs ↗</a>
</fui-tooltip>

<!-- Right -->
<fui-tooltip content="This field is required" placement="right">
  <fui-input label="Email"></fui-input>
</fui-tooltip>"""
    }
    {
        Tag         = "fui-popover"
        Name        = "Popover"
        Slug        = "popover"
        Category    = "Overlay"
        Description = "A floating panel anchored below (or above) a trigger button. Unlike Menu, the popover body accepts arbitrary slotted HTML — rich content, forms, or lists. Closes on outside click."
        Attributes  = [
            { Name="trigger-label"; Type="string"; Default="Options";      Description="Label on the trigger button" }
            { Name="placement";     Type="string"; Default="bottom-start"; Description="bottom-start | bottom-end | top-start | top-end" }
            { Name="width";         Type="string"; Default="";             Description="Explicit panel width (e.g. 280px) — defaults to content width" }
        ]
        CssProps = [
            { Name="--fui-popover-bg";             Description="Panel background colour" }
            { Name="--fui-popover-border";         Description="Panel border colour" }
            { Name="--fui-popover-radius";         Description="Panel border radius" }
            { Name="--fui-popover-trigger-bg";     Description="Trigger button background" }
            { Name="--fui-popover-trigger-border"; Description="Trigger button border colour" }
            { Name="--fui-popover-trigger-color";  Description="Trigger button text colour" }
            { Name="--fui-popover-font";           Description="Font family" }
        ]
        Events = [
            { Name="fui-open";  Description="Fired when the panel opens" }
            { Name="fui-close"; Description="Fired when the panel closes" }
        ]
        HtmlUsage = """<fui-popover trigger-label="Filter" width="260px">
  <fui-stack direction="column" gap="0.75rem">
    <fui-select label="Status" options='[{"value":"active","label":"Active"},{"value":"archived","label":"Archived"}]'></fui-select>
    <fui-button variant="primary">Apply</fui-button>
  </fui-stack>
</fui-popover>

<fui-popover trigger-label="Share" placement="bottom-end" width="240px">
  <p style="margin:0 0 0.75rem;font-size:0.875rem;color:#A0A0A8">Share this document</p>
  <fui-input placeholder="Enter email..."></fui-input>
</fui-popover>"""
    }
    {
        Tag         = "fui-confirm-dialog"
        Name        = "ConfirmDialog"
        Slug        = "confirm-dialog"
        Category    = "Overlay"
        Description = "A focused confirmation dialog with title, optional message, and two action buttons. Use variant=\"danger\" for destructive confirmations — the trigger and confirm button both turn red. Fires fui-confirm or fui-cancel."
        Attributes  = [
            { Name="trigger-label";  Type="string"; Default="Delete";        Description="Label on the trigger button" }
            { Name="title";          Type="string"; Default="Are you sure?"; Description="Dialog heading" }
            { Name="message";        Type="string"; Default="";              Description="Supporting text below the title; or use the default slot for rich content" }
            { Name="confirm-label";  Type="string"; Default="Confirm";       Description="Label on the confirm (primary action) button" }
            { Name="cancel-label";   Type="string"; Default="Cancel";        Description="Label on the cancel button" }
            { Name="variant";        Type="string"; Default="default";       Description="default | danger — danger styles trigger and confirm button red" }
        ]
        CssProps = [
            { Name="--fui-cd-bg";           Description="Panel background colour" }
            { Name="--fui-cd-border";       Description="Panel and divider border colour" }
            { Name="--fui-cd-title-color";  Description="Title text colour" }
            { Name="--fui-cd-color";        Description="Body text colour" }
            { Name="--fui-cd-radius";       Description="Panel border radius" }
            { Name="--fui-cd-font";         Description="Font family" }
        ]
        Events = [
            { Name="fui-open";    Description="Fired when the dialog opens" }
            { Name="fui-confirm"; Description="Fired when the confirm button is clicked" }
            { Name="fui-cancel";  Description="Fired when the cancel button is clicked or the backdrop is clicked" }
        ]
        HtmlUsage = """<!-- Default confirm -->
<fui-confirm-dialog
  trigger-label="Archive"
  title="Archive this project?"
  message="This will remove it from your active list."
  confirm-label="Archive"
  cancel-label="Keep it">
</fui-confirm-dialog>

<!-- Danger variant — destructive action -->
<fui-confirm-dialog
  trigger-label="Delete account"
  title="Delete your account?"
  message="This action cannot be undone. All data will be permanently removed."
  confirm-label="Yes, delete"
  variant="danger">
</fui-confirm-dialog>"""
    }
    // ── Data Display ──────────────────────────────────────────────────────────────
    {
        Tag         = "fui-card"
        Name        = "Card"
        Slug        = "card"
        Category    = "Data Display"
        Description = "A flexible container with optional header, body, and footer slots. Four variants (default, elevated, outline, ghost) and four padding sizes control visual weight."
        Attributes  = [
            { Name="variant"; Type="string"; Default="default"; Description="default | elevated | outline | ghost" }
            { Name="padding"; Type="string"; Default="md";      Description="sm | md | lg | none" }
        ]
        CssProps = [
            { Name="--fui-card-bg";           Description="Card background colour" }
            { Name="--fui-card-border";       Description="Card border colour" }
            { Name="--fui-card-divider";      Description="Header and footer divider line colour" }
            { Name="--fui-card-radius";       Description="Border radius" }
            { Name="--fui-card-header-color"; Description="Header text colour" }
            { Name="--fui-card-color";        Description="Body text colour" }
            { Name="--fui-card-font";         Description="Font family" }
        ]
        Events   = []
        HtmlUsage = """<fui-card>
  <span slot="header">Card title</span>
  <p>Body content — default slot.</p>
  <div slot="footer">
    <fui-button variant="primary" size="sm">Save</fui-button>
  </div>
</fui-card>

<!-- Variants -->
<fui-card variant="elevated"><p>Elevated — deeper shadow</p></fui-card>
<fui-card variant="outline"><p>Outline — transparent background</p></fui-card>
<fui-card variant="ghost"><p>Ghost — no border</p></fui-card>

<!-- Padding sizes -->
<fui-card padding="sm"><p>Small padding</p></fui-card>
<fui-card padding="lg"><p>Large padding</p></fui-card>"""
    }
    {
        Tag         = "fui-stat"
        Name        = "Stat"
        Slug        = "stat"
        Category    = "Data Display"
        Description = "Compact metric card showing a label, large value, optional change badge coloured by trend direction, and a supporting description line."
        Attributes  = [
            { Name="label";       Type="string"; Default="";     Description="Metric label (e.g. Revenue)" }
            { Name="value";       Type="string"; Default="";     Description="Primary value in large type (e.g. $48,200)" }
            { Name="change";      Type="string"; Default="";     Description="Change amount or percentage for the badge (e.g. 12.5%)" }
            { Name="trend";       Type="string"; Default="flat"; Description="up | down | flat — colours the badge green, red, or neutral" }
            { Name="description"; Type="string"; Default="";     Description="Small supporting text below the value row (e.g. vs last month)" }
        ]
        CssProps = [
            { Name="--fui-stat-bg";          Description="Card background colour" }
            { Name="--fui-stat-border";      Description="Card border colour" }
            { Name="--fui-stat-radius";      Description="Card border radius" }
            { Name="--fui-stat-label-color"; Description="Label text colour" }
            { Name="--fui-stat-value-color"; Description="Value text colour" }
            { Name="--fui-stat-desc-color";  Description="Description text colour" }
            { Name="--fui-stat-font";        Description="Font family" }
        ]
        Events   = []
        HtmlUsage = """<fui-stat label="Revenue" value="$48,200" change="12.5%" trend="up" description="vs last month"></fui-stat>
<fui-stat label="Users"   value="8,412"  change="3.1%"  trend="down"></fui-stat>
<fui-stat label="Uptime"  value="99.9%"></fui-stat>"""
    }
    {
        Tag         = "fui-avatar"
        Name        = "Avatar"
        Slug        = "avatar"
        Category    = "Data Display"
        Description = "Circular user representation. Shows an image when src is provided; falls back to styled initials. Five sizes and four status-indicator dots (online, offline, away, busy)."
        Attributes  = [
            { Name="src";      Type="string"; Default="";   Description="Image URL — empty triggers the initials fallback" }
            { Name="initials"; Type="string"; Default="?";  Description="One or two letters shown when no image is available" }
            { Name="size";     Type="string"; Default="md"; Description="xs | sm | md | lg | xl" }
            { Name="status";   Type="string"; Default="";   Description="online | offline | away | busy — shows a colour dot; omit to hide" }
        ]
        CssProps = [
            { Name="--fui-avatar-bg";            Description="Initials background colour" }
            { Name="--fui-avatar-color";         Description="Initials text colour" }
            { Name="--fui-avatar-status-border"; Description="Status dot border colour (match page background)" }
            { Name="--fui-avatar-font";          Description="Font family" }
        ]
        Events   = []
        HtmlUsage = """<!-- Initials fallback -->
<fui-avatar initials="JD"></fui-avatar>
<fui-avatar initials="AB" size="lg" status="online"></fui-avatar>

<!-- All status indicators -->
<fui-avatar initials="ON" status="online"></fui-avatar>
<fui-avatar initials="OF" status="offline"></fui-avatar>
<fui-avatar initials="AW" status="away"></fui-avatar>
<fui-avatar initials="BZ" status="busy"></fui-avatar>"""
    }
    {
        Tag         = "fui-avatar-group"
        Name        = "AvatarGroup"
        Slug        = "avatar-group"
        Category    = "Data Display"
        Description = "Stacked row of overlapping avatar circles from a JSON array. Clips the list at max and shows a +N overflow bubble for additional members. Three sizes (sm, md, lg)."
        Attributes  = [
            { Name="avatars"; Type="string"; Default="[]"; Description="JSON array of {src, initials} objects" }
            { Name="max";     Type="number"; Default="4";  Description="Maximum avatars shown before the +N overflow bubble" }
            { Name="size";    Type="string"; Default="md"; Description="sm | md | lg" }
        ]
        CssProps = [
            { Name="--fui-ag-bg";             Description="Avatar circle background (initials)" }
            { Name="--fui-ag-color";          Description="Initials text colour" }
            { Name="--fui-ag-ring";           Description="Gap border between stacked circles" }
            { Name="--fui-ag-overflow-bg";    Description="Overflow bubble background" }
            { Name="--fui-ag-overflow-color"; Description="Overflow bubble text colour" }
            { Name="--fui-ag-font";           Description="Font family" }
        ]
        Events   = []
        HtmlUsage = """<fui-avatar-group
  avatars='[{"src":"","initials":"JD"},{"src":"","initials":"AB"},{"src":"","initials":"MK"},{"src":"","initials":"RQ"},{"src":"","initials":"TW"}]'>
</fui-avatar-group>

<!-- Large, max 3 -->
<fui-avatar-group size="lg" max="3"
  avatars='[{"src":"","initials":"AA"},{"src":"","initials":"BB"},{"src":"","initials":"CC"},{"src":"","initials":"DD"}]'>
</fui-avatar-group>"""
    }
    {
        Tag         = "fui-tag"
        Name        = "Tag"
        Slug        = "tag"
        Category    = "Data Display"
        Description = "Compact inline label for categories, statuses, and facets. Six colour variants, three sizes, and an optional removable × button that fires fui-remove."
        Attributes  = [
            { Name="variant";   Type="string";  Default="neutral"; Description="neutral | success | warning | danger | info | accent" }
            { Name="size";      Type="string";  Default="md";      Description="sm | md | lg" }
            { Name="removable"; Type="boolean"; Default="false";   Description="Shows a × button; fires fui-remove when clicked" }
        ]
        CssProps = [
            { Name="--fui-tag-bg";     Description="Neutral background colour" }
            { Name="--fui-tag-border"; Description="Neutral border colour" }
            { Name="--fui-tag-color";  Description="Neutral text colour" }
            { Name="--fui-tag-radius"; Description="Border radius" }
            { Name="--fui-tag-font";   Description="Font family" }
        ]
        Events = [
            { Name="fui-remove"; Description="Fired when the × button is clicked — no detail payload" }
        ]
        HtmlUsage = """<fui-tag>Default</fui-tag>
<fui-tag variant="success">Active</fui-tag>
<fui-tag variant="warning">Pending</fui-tag>
<fui-tag variant="danger">Error</fui-tag>
<fui-tag variant="info">Beta</fui-tag>
<fui-tag variant="accent">New</fui-tag>

<!-- Removable -->
<fui-tag variant="accent" removable>F#</fui-tag>

<!-- Sizes -->
<fui-tag size="sm">Small</fui-tag>
<fui-tag size="lg">Large</fui-tag>"""
    }
    {
        Tag         = "fui-code-block"
        Name        = "CodeBlock"
        Slug        = "code-block"
        Category    = "Data Display"
        Description = "Syntax-styled code display block with a language label in the toolbar and a copy-to-clipboard button. Lit auto-escapes angle brackets so HTML-heavy snippets render safely."
        Attributes  = [
            { Name="language"; Type="string"; Default=""; Description="Language label shown in the toolbar (e.g. fsharp, css, bash). Omit to hide." }
            { Name="code";     Type="string"; Default=""; Description="Source code to display — HTML special characters are automatically escaped." }
        ]
        CssProps = [
            { Name="--fui-code-bg";         Description="Code block background colour" }
            { Name="--fui-code-border";     Description="Border colour" }
            { Name="--fui-code-radius";     Description="Border radius" }
            { Name="--fui-code-color";      Description="Code text colour" }
            { Name="--fui-code-lang-color"; Description="Language label colour" }
            { Name="--fui-code-copy-color"; Description="Copy button colour (idle)" }
            { Name="--fui-code-font-size";  Description="Code font size (default 0.8125rem)" }
        ]
        Events   = []
        HtmlUsage = """<fui-code-block language="fsharp" code="let greet name =
    printfn &quot;Hello, %s!&quot; name"></fui-code-block>

<fui-code-block language="bash" code="npm install && npm run start"></fui-code-block>

<fui-code-block code="SELECT * FROM users WHERE active = true;"></fui-code-block>"""
    }
    {
        Tag         = "fui-callout"
        Name        = "Callout"
        Slug        = "callout"
        Category    = "Data Display"
        Description = "Editorial callout box with a left accent stripe and optional icon-prefixed title. Five variants: info, success, warning, danger, and note (accent purple). Body content is slotted."
        Attributes  = [
            { Name="variant"; Type="string"; Default="info"; Description="info | success | warning | danger | note" }
            { Name="title";   Type="string"; Default="";    Description="Bold heading line with an auto-selected icon prefix" }
        ]
        CssProps = [
            { Name="--fui-callout-radius";      Description="Border radius" }
            { Name="--fui-callout-title-color"; Description="Title text colour" }
            { Name="--fui-callout-color";       Description="Body text colour" }
            { Name="--fui-callout-font";        Description="Font family" }
        ]
        Events   = []
        HtmlUsage = """<fui-callout variant="info"    title="Tip">Use CSS custom properties to theme any component.</fui-callout>
<fui-callout variant="success" title="Done">All checks passed.</fui-callout>
<fui-callout variant="warning" title="Heads up">This API will be deprecated in v2.</fui-callout>
<fui-callout variant="danger"  title="Error">Failed to connect to the server.</fui-callout>
<fui-callout variant="note"    title="Note">Behaviour changed in v1.4.</fui-callout>"""
    }
    {
        Tag         = "fui-list"
        Name        = "List"
        Slug        = "list"
        Category    = "Data Display"
        Description = "Styled unordered or ordered list rendered from a JSON string array. Four visual variants: default, bordered (divider lines), striped (alternating rows), and bullet (accent ▸ prefix)."
        Attributes  = [
            { Name="items";   Type="string";  Default="[]";      Description="JSON array of string items" }
            { Name="ordered"; Type="boolean"; Default="false";   Description="Renders as an <ol> with CSS counter prefix" }
            { Name="variant"; Type="string";  Default="default"; Description="default | bordered | striped | bullet" }
        ]
        CssProps = [
            { Name="--fui-list-color";         Description="List item text colour" }
            { Name="--fui-list-divider";       Description="Divider line colour (bordered variant)" }
            { Name="--fui-list-counter-color"; Description="Counter number colour (ordered)" }
            { Name="--fui-list-font";          Description="Font family" }
        ]
        Events   = []
        HtmlUsage = """<fui-list items='["Alpha","Beta","Gamma","Delta"]'></fui-list>

<fui-list ordered items='["Install dependencies","Configure","Run the server"]'></fui-list>

<fui-list variant="bordered" items='["Item 1","Item 2","Item 3"]'></fui-list>

<fui-list variant="bullet" items='["Build UI","Add API","Write tests"]'></fui-list>"""
    }
    {
        Tag         = "fui-timeline"
        Name        = "Timeline"
        Slug        = "timeline"
        Category    = "Data Display"
        Description = "Vertical event sequence. Each item has a label, optional date, description, and icon dot. A connector line links consecutive dots."
        Attributes  = [
            { Name="items"; Type="string"; Default="[]"; Description="JSON array of {label, description, date, icon} objects — description, date, and icon are optional (set to \"\" to hide)" }
        ]
        CssProps = [
            { Name="--fui-tl-line";        Description="Connector line colour" }
            { Name="--fui-tl-dot-bg";      Description="Dot background colour" }
            { Name="--fui-tl-dot-border";  Description="Dot border / accent colour" }
            { Name="--fui-tl-dot-color";   Description="Icon colour inside the dot" }
            { Name="--fui-tl-label-color"; Description="Event label text colour" }
            { Name="--fui-tl-date-color";  Description="Date text colour" }
            { Name="--fui-tl-desc-color";  Description="Description text colour" }
            { Name="--fui-tl-font";        Description="Font family" }
        ]
        Events   = []
        HtmlUsage = """<fui-timeline items='[
  {"label":"Project started","description":"Initial planning completed.","date":"Jan 2025","icon":""},
  {"label":"Beta release","description":"First public beta shipped.","date":"Jun 2025","icon":""},
  {"label":"v1.0","description":"General availability.","date":"Sep 2025","icon":""}
]'></fui-timeline>"""
    }
    {
        Tag         = "fui-accordion"
        Name        = "Accordion"
        Slug        = "accordion"
        Category    = "Data Display"
        Description = "Collapsible panel list. In single mode (default) opening one panel closes any other. In multiple mode any number of panels can be open simultaneously."
        Attributes  = [
            { Name="items";    Type="string";  Default="[]";    Description="JSON array of {label, content} objects" }
            { Name="multiple"; Type="boolean"; Default="false"; Description="Allow multiple panels open at once" }
        ]
        CssProps = [
            { Name="--fui-accordion-border";        Description="Container and item border colour" }
            { Name="--fui-accordion-radius";        Description="Container border radius" }
            { Name="--fui-accordion-trigger-color"; Description="Trigger button text colour" }
            { Name="--fui-accordion-icon-color";    Description="Chevron icon colour (closed)" }
            { Name="--fui-accordion-color";         Description="Panel body text colour" }
            { Name="--fui-accordion-font";          Description="Font family" }
        ]
        Events   = []
        HtmlUsage = """<fui-accordion items='[
  {"label":"What is FableUI?","content":"A library of standalone Web Components built with Fable and Lit."},
  {"label":"Do I need a framework?","content":"No — any fui-* tag works in plain HTML."},
  {"label":"Can I theme the components?","content":"Yes — all components expose CSS custom properties."}
]'></fui-accordion>

<!-- Multiple panels open at once -->
<fui-accordion multiple items='[{"label":"Panel A","content":"Content A"},{"label":"Panel B","content":"Content B"}]'></fui-accordion>"""
    }
    {
        Tag         = "fui-carousel"
        Name        = "Carousel"
        Slug        = "carousel"
        Category    = "Data Display"
        Description = "Slide-by-slide content switcher with Prev/Next buttons and dot navigation. Optional loop mode wraps the last slide back to the first."
        Attributes  = [
            { Name="items";     Type="string";  Default="[]";   Description="JSON array of {title, description} objects" }
            { Name="show-dots"; Type="boolean"; Default="true"; Description="Show clickable dot indicators (default true)" }
            { Name="loop";      Type="boolean"; Default="false"; Description="Wrap from last slide to first when navigating" }
        ]
        CssProps = [
            { Name="--fui-carousel-bg";            Description="Slide area background colour" }
            { Name="--fui-carousel-border";        Description="Container and control border colour" }
            { Name="--fui-carousel-radius";        Description="Container border radius" }
            { Name="--fui-carousel-title-color";   Description="Slide title colour" }
            { Name="--fui-carousel-desc-color";    Description="Slide description colour" }
            { Name="--fui-carousel-btn-border";    Description="Nav button border colour" }
            { Name="--fui-carousel-btn-color";     Description="Nav button text colour" }
            { Name="--fui-carousel-dot";           Description="Inactive dot colour" }
            { Name="--fui-carousel-counter-color"; Description="Counter text colour" }
            { Name="--fui-carousel-font";          Description="Font family" }
        ]
        Events = [
            { Name="fui-change"; Description="Fired on slide change — detail: { index: number }" }
        ]
        HtmlUsage = """<fui-carousel items='[
  {"title":"Components","description":"Drop any fui-* tag into HTML and it works immediately."},
  {"title":"Theming","description":"Use CSS custom properties to control every visual detail."},
  {"title":"Accessibility","description":"WCAG 2.1 AA keyboard and ARIA support throughout."}
]'></fui-carousel>

<!-- Loop enabled -->
<fui-carousel loop items='[{"title":"Slide 1","description":"Content 1"},{"title":"Slide 2","description":"Content 2"}]'></fui-carousel>"""
    }
    {
        Tag         = "fui-table"
        Name        = "Table"
        Slug        = "table"
        Category    = "Data Display"
        Description = "Data table with optional client-side column sorting and striped rows. Columns are JSON descriptors; rows are JSON arrays of string arrays (one inner array per row)."
        Attributes  = [
            { Name="columns";  Type="string";  Default="[]";   Description="JSON array of {key, label, sortable} column descriptors" }
            { Name="rows";     Type="string";  Default="[]";   Description="JSON array of string arrays — each inner array is one row" }
            { Name="sortable"; Type="boolean"; Default="false"; Description="Enable client-side sorting for columns where sortable: true" }
            { Name="striped";  Type="boolean"; Default="false"; Description="Alternate odd/even row background colours" }
        ]
        CssProps = [
            { Name="--fui-table-border";     Description="Table and cell border colour" }
            { Name="--fui-table-radius";     Description="Container border radius" }
            { Name="--fui-table-head-bg";    Description="Header row background" }
            { Name="--fui-table-head-color"; Description="Header cell text colour" }
            { Name="--fui-table-color";      Description="Body cell text colour" }
            { Name="--fui-table-font";       Description="Font family" }
        ]
        Events   = []
        HtmlUsage = """<fui-table
  sortable
  columns='[{"key":"name","label":"Name","sortable":true},{"key":"email","label":"Email","sortable":true},{"key":"role","label":"Role","sortable":false}]'
  rows='[["Alice Chen","alice@example.com","Admin"],["Bob Smith","bob@example.com","Developer"],["Carol Davis","carol@example.com","Designer"]]'>
</fui-table>

<!-- Striped, no sorting -->
<fui-table striped columns='[{"key":"n","label":"Name","sortable":false}]' rows='[["Alice"],["Bob"],["Carol"]]'></fui-table>"""
    }
    // ── Navigation ────────────────────────────────────────────────────────────
    {
        Tag         = "fui-tabs"
        Name        = "Tabs"
        Slug        = "tabs"
        Category    = "Navigation"
        Description = "Tabbed panel switcher. Tab labels are supplied via a JSON array; panel content is projected through named slots (`slot=\"tab-{value}\"`). Fires fui-change when the active tab changes."
        Attributes  = [
            { Name="tabs";   Type="string"; Default="[]"; Description="JSON array of {value, label} objects defining the tab bar" }
            { Name="active"; Type="string"; Default="";   Description="Value of the initially-selected tab (defaults to the first tab)" }
        ]
        CssProps = [
            { Name="--fui-tabs-border";         Description="Bottom border colour of the tab bar" }
            { Name="--fui-tabs-color";          Description="Inactive tab label colour" }
            { Name="--fui-tabs-color-hover";    Description="Tab label colour on hover" }
            { Name="--fui-tabs-active-color";   Description="Active tab label colour" }
            { Name="--fui-tabs-accent";         Description="Active tab indicator and focus ring colour" }
            { Name="--fui-tabs-panel-padding";  Description="Padding applied to the content area below the tab bar" }
            { Name="--fui-tabs-font";           Description="Font family" }
        ]
        Events = [
            { Name="fui-change"; Description="Fired when the active tab changes — detail: { value: string }" }
        ]
        HtmlUsage = """<fui-tabs tabs='[{"value":"a","label":"Overview"},{"value":"b","label":"Settings"},{"value":"c","label":"Logs"}]'>
  <div slot="tab-a"><p>Overview content goes here.</p></div>
  <div slot="tab-b"><p>Settings panel.</p></div>
  <div slot="tab-c"><p>Log output.</p></div>
</fui-tabs>"""
    }
    {
        Tag         = "fui-breadcrumb"
        Name        = "Breadcrumb"
        Slug        = "breadcrumb"
        Category    = "Navigation"
        Description = "Horizontal trail of navigation links showing the current page's location. Items are supplied as a JSON array; the last item is rendered as the current page (no link, aria-current)."
        Attributes  = [
            { Name="items";     Type="string"; Default="[]"; Description="JSON array of {label, href} objects — omit href on the last (current) item" }
            { Name="separator"; Type="string"; Default="/";  Description="Text or character rendered between items" }
        ]
        CssProps = [
            { Name="--fui-bc-link-color";    Description="Colour of non-current links" }
            { Name="--fui-bc-link-hover";    Description="Link colour on hover" }
            { Name="--fui-bc-current-color"; Description="Colour of the current page item" }
            { Name="--fui-bc-sep-color";     Description="Separator colour" }
            { Name="--fui-bc-accent";        Description="Focus ring colour" }
            { Name="--fui-bc-font";          Description="Font family" }
        ]
        Events = []
        HtmlUsage = """<fui-breadcrumb
  items='[{"label":"Home","href":"/"},{"label":"Components","href":"/components"},{"label":"Breadcrumb","href":""}]'>
</fui-breadcrumb>

<!-- Custom separator -->
<fui-breadcrumb separator="›"
  items='[{"label":"Docs","href":"/docs"},{"label":"API","href":""}]'>
</fui-breadcrumb>"""
    }
    {
        Tag         = "fui-pagination"
        Name        = "Pagination"
        Slug        = "pagination"
        Category    = "Navigation"
        Description = "Page navigation control showing prev/next arrows and numbered page buttons with ellipsis gaps. Fires fui-change on every page change."
        Attributes  = [
            { Name="total";    Type="number"; Default="1"; Description="Total number of pages" }
            { Name="page";     Type="number"; Default="1"; Description="Initially-selected page number (1-indexed)" }
            { Name="siblings"; Type="number"; Default="1"; Description="Number of page buttons shown on each side of the current page" }
        ]
        CssProps = [
            { Name="--fui-pg-bg";           Description="Button background (idle)" }
            { Name="--fui-pg-border";       Description="Button border colour (idle)" }
            { Name="--fui-pg-color";        Description="Button text colour (idle)" }
            { Name="--fui-pg-bg-hover";     Description="Button background on hover" }
            { Name="--fui-pg-border-hover"; Description="Button border colour on hover" }
            { Name="--fui-pg-color-hover";  Description="Button text colour on hover" }
            { Name="--fui-pg-accent";       Description="Active page background and focus ring colour" }
            { Name="--fui-pg-muted";        Description="Ellipsis colour" }
            { Name="--fui-pg-radius";       Description="Button border radius" }
            { Name="--fui-pg-font";         Description="Font family" }
        ]
        Events = [
            { Name="fui-change"; Description="Fired on page change — detail: { page: number }" }
        ]
        HtmlUsage = """<fui-pagination total="10" page="1"></fui-pagination>
<fui-pagination total="50" page="25" siblings="2"></fui-pagination>"""
    }
    {
        Tag         = "fui-stepper"
        Name        = "Stepper"
        Slug        = "stepper"
        Category    = "Navigation"
        Description = "Step-progress indicator for multi-step workflows. Steps are supplied as a JSON array. Completed steps show a checkmark; the active step is highlighted in accent. Supports horizontal and vertical orientations."
        Attributes  = [
            { Name="steps";       Type="string";  Default="[]";        Description="JSON array of {label, description} objects — description is optional" }
            { Name="active";      Type="number";  Default="0";         Description="0-indexed index of the currently active step" }
            { Name="orientation"; Type="string";  Default="horizontal"; Description="horizontal | vertical" }
            { Name="clickable";   Type="boolean"; Default="false";     Description="When true, clicking a step fires fui-change and updates the active step" }
        ]
        CssProps = [
            { Name="--fui-st-accent";       Description="Accent colour for active and completed steps" }
            { Name="--fui-st-circle-bg";    Description="Circle background for pending steps" }
            { Name="--fui-st-border";       Description="Circle border colour for pending steps" }
            { Name="--fui-st-num-color";    Description="Step number text colour" }
            { Name="--fui-st-connector";    Description="Connector line colour (pending)" }
            { Name="--fui-st-label-color";  Description="Step label colour (pending)" }
            { Name="--fui-st-label-active"; Description="Step label colour (active / done)" }
            { Name="--fui-st-desc-color";   Description="Step description text colour" }
            { Name="--fui-st-font";         Description="Font family" }
        ]
        Events = [
            { Name="fui-change"; Description="Fired when a clickable step is selected — detail: { step: number }" }
        ]
        HtmlUsage = """<fui-stepper
  active="1"
  steps='[{"label":"Account","description":"Create your account"},{"label":"Profile","description":"Fill in your details"},{"label":"Review","description":"Confirm and submit"}]'>
</fui-stepper>

<!-- Vertical, clickable -->
<fui-stepper
  orientation="vertical"
  clickable
  active="0"
  steps='[{"label":"Plan","description":""},{"label":"Build","description":""},{"label":"Deploy","description":""}]'>
</fui-stepper>"""
    }
    {
        Tag         = "fui-menu"
        Name        = "Menu"
        Slug        = "menu"
        Category    = "Navigation"
        Description = "Dropdown menu triggered by a button. Items are supplied as a JSON array supporting labels, disabled items, and separator rules. Closes on outside click. Fires fui-select when an item is chosen."
        Attributes  = [
            { Name="label";     Type="string"; Default="Menu";         Description="Text shown on the trigger button" }
            { Name="items";     Type="string"; Default="[]";           Description="JSON array of {value, label, disabled?, separator?} objects — set separator:true for a divider rule" }
            { Name="placement"; Type="string"; Default="bottom-start"; Description="bottom-start | bottom-end | top-start | top-end" }
        ]
        CssProps = [
            { Name="--fui-menu-trigger-bg";          Description="Trigger button background" }
            { Name="--fui-menu-trigger-border";      Description="Trigger button border colour" }
            { Name="--fui-menu-trigger-color";       Description="Trigger button text colour" }
            { Name="--fui-menu-trigger-bg-hover";    Description="Trigger background on hover" }
            { Name="--fui-menu-trigger-border-hover";Description="Trigger border colour on hover" }
            { Name="--fui-menu-bg";                  Description="Dropdown panel background" }
            { Name="--fui-menu-border";              Description="Dropdown panel border colour" }
            { Name="--fui-menu-item-color";          Description="Menu item text colour" }
            { Name="--fui-menu-item-bg-hover";       Description="Menu item background on hover" }
            { Name="--fui-menu-disabled-color";      Description="Disabled item text colour" }
            { Name="--fui-menu-sep-color";           Description="Separator line colour" }
            { Name="--fui-menu-accent";              Description="Hover text accent and focus ring colour" }
            { Name="--fui-menu-radius";              Description="Border radius for trigger and panel" }
            { Name="--fui-menu-font";                Description="Font family" }
        ]
        Events = [
            { Name="fui-select"; Description="Fired when a menu item is clicked — detail: { value: string }" }
        ]
        HtmlUsage = """<fui-menu label="Actions" items='[
  {"value":"edit",   "label":"Edit"},
  {"value":"dup",    "label":"Duplicate"},
  {"value":"",       "label":"",       "separator":true},
  {"value":"delete", "label":"Delete", "disabled":false}
]'></fui-menu>

<fui-menu label="Options" placement="bottom-end" items='[
  {"value":"profile", "label":"Profile"},
  {"value":"settings","label":"Settings"},
  {"value":"",        "label":"",         "separator":true},
  {"value":"logout",  "label":"Sign out"}
]'></fui-menu>"""
    }
    // ── Layout ────────────────────────────────────────────────────────────────
    {
        Tag         = "fui-divider"
        Name        = "Divider"
        Slug        = "divider"
        Category    = "Layout"
        Description = "Horizontal or vertical separator rule with an optional centred text label. Thickness and colour are fully themeable via CSS custom properties."
        Attributes  = [
            { Name="label";       Type="string"; Default="";           Description="Optional text label displayed in the centre of the rule" }
            { Name="orientation"; Type="string"; Default="horizontal"; Description="horizontal | vertical" }
        ]
        CssProps = [
            { Name="--fui-divider-color";       Description="Rule colour" }
            { Name="--fui-divider-thickness";   Description="Rule thickness (default 1px)" }
            { Name="--fui-divider-label-color"; Description="Label text colour" }
            { Name="--fui-divider-font";        Description="Font family for the label" }
        ]
        Events   = []
        HtmlUsage = """<fui-divider></fui-divider>
<fui-divider label="or"></fui-divider>
<fui-divider label="Section title"></fui-divider>

<!-- Vertical — works inside a fixed-height flex container -->
<div style="display:flex;height:48px;gap:1rem;align-items:center">
  <span>Left</span>
  <fui-divider orientation="vertical"></fui-divider>
  <span>Right</span>
</div>"""
    }
    {
        Tag         = "fui-stack"
        Name        = "Stack"
        Slug        = "stack"
        Category    = "Layout"
        Description = "Thin flexbox wrapper that arranges slotted children in a row or column with configurable gap, alignment, and wrapping. Zero markup overhead — the slot projects children directly."
        Attributes  = [
            { Name="direction"; Type="string";  Default="column";     Description="row | column" }
            { Name="gap";       Type="string";  Default="1rem";       Description="Any CSS gap value — e.g. 0.5rem or 8px" }
            { Name="align";     Type="string";  Default="stretch";    Description="CSS align-items value" }
            { Name="justify";   Type="string";  Default="flex-start"; Description="CSS justify-content value" }
            { Name="wrap";      Type="boolean"; Default="false";      Description="Enables flex-wrap: wrap" }
        ]
        CssProps = []
        Events   = []
        HtmlUsage = """<!-- Row of buttons -->
<fui-stack direction="row" gap="0.75rem">
  <fui-button variant="primary">Save</fui-button>
  <fui-button variant="secondary">Cancel</fui-button>
</fui-stack>

<!-- Centred column -->
<fui-stack direction="column" gap="1rem" align="center">
  <fui-input label="Email"></fui-input>
  <fui-input label="Password" type="password"></fui-input>
  <fui-button variant="primary">Sign in</fui-button>
</fui-stack>"""
    }
    {
        Tag         = "fui-scroll-area"
        Name        = "ScrollArea"
        Slug        = "scroll-area"
        Category    = "Layout"
        Description = "Scrollable container with a slim, accent-coloured custom scrollbar. Set height or max-height via attributes; overflow direction is configurable."
        Attributes  = [
            { Name="height";     Type="string"; Default="";         Description="CSS height applied to the scroll viewport (e.g. 300px)" }
            { Name="max-height"; Type="string"; Default="";         Description="CSS max-height applied to the scroll viewport" }
            { Name="direction";  Type="string"; Default="vertical"; Description="vertical | horizontal | both" }
        ]
        CssProps = [
            { Name="--fui-sa-track";      Description="Scrollbar track background" }
            { Name="--fui-sa-thumb";      Description="Scrollbar thumb colour (idle)" }
            { Name="--fui-sa-thumb-hover";Description="Scrollbar thumb colour on hover" }
        ]
        Events   = []
        HtmlUsage = """<fui-scroll-area height="240px" direction="vertical">
  <ul style="margin:0;padding:0 0 0 1.25rem">
    <li>Item 1</li>
    <li>Item 2</li>
    <!-- ... -->
  </ul>
</fui-scroll-area>"""
    }
    {
        Tag         = "fui-container"
        Name        = "Container"
        Slug        = "container"
        Category    = "Layout"
        Description = "Max-width centred wrapper that constrains content width and applies horizontal padding. The primary page-level layout primitive."
        Attributes  = [
            { Name="max-width"; Type="string"; Default=""; Description="Override max-width (e.g. 800px) — falls back to --fui-container-max-width" }
            { Name="padding";   Type="string"; Default=""; Description="Override horizontal padding (e.g. 1.5rem) — falls back to --fui-container-px" }
        ]
        CssProps = [
            { Name="--fui-container-max-width"; Description="Maximum content width (default 1100px)" }
            { Name="--fui-container-px";        Description="Horizontal padding applied to both sides (default 2.5rem)" }
        ]
        Events   = []
        HtmlUsage = """<fui-container>
  <p>Content centred at 1100px max-width with 2.5rem padding.</p>
</fui-container>

<!-- Narrow reading column -->
<fui-container max-width="680px" padding="1.5rem">
  <p>Article body text — comfortable reading width.</p>
</fui-container>"""
    }
    {
        Tag         = "fui-grid"
        Name        = "Grid"
        Slug        = "grid"
        Category    = "Layout"
        Description = "Thin CSS Grid wrapper. Pass a column count (auto-generates repeat(n, 1fr)) or any grid-template-columns expression. Row and column gap are independently configurable."
        Attributes  = [
            { Name="cols";    Type="string"; Default="2";    Description="Column count (generates repeat(n, 1fr)) or any CSS grid-template-columns value — e.g. 3, 2fr 1fr, repeat(auto-fill, minmax(200px, 1fr))" }
            { Name="gap";     Type="string"; Default="1rem"; Description="Gap applied to both axes when row-gap and col-gap are not set" }
            { Name="row-gap"; Type="string"; Default="";     Description="Row gap — overrides gap for rows only" }
            { Name="col-gap"; Type="string"; Default="";     Description="Column gap — overrides gap for columns only" }
        ]
        CssProps = []
        Events   = []
        HtmlUsage = """<!-- 3-column equal grid -->
<fui-grid cols="3" gap="1rem">
  <div>A</div><div>B</div><div>C</div>
</fui-grid>

<!-- Main + sidebar layout -->
<fui-grid cols="2fr 1fr" gap="1.5rem">
  <div>Main content</div>
  <div>Sidebar</div>
</fui-grid>

<!-- Responsive auto-fill -->
<fui-grid cols="repeat(auto-fill, minmax(200px, 1fr))" gap="1rem">
  <div>Card</div><div>Card</div><div>Card</div>
</fui-grid>"""
    }
    {
        Tag         = "fui-spacer"
        Name        = "Spacer"
        Slug        = "spacer"
        Category    = "Layout"
        Description = "Flexible whitespace element that expands to fill available space in a flex or grid container. Without a size attribute it behaves as flex: 1 1 auto, pushing siblings apart."
        Attributes  = [
            { Name="size"; Type="string"; Default=""; Description="Fixed minimum size (e.g. 1rem or 24px). Omit for a fully flexible spacer." }
        ]
        CssProps = []
        Events   = []
        HtmlUsage = """<!-- Push action buttons to opposite ends of a toolbar -->
<fui-stack direction="row">
  <fui-button variant="ghost">← Back</fui-button>
  <fui-spacer></fui-spacer>
  <fui-button variant="primary">Next →</fui-button>
</fui-stack>

<!-- Fixed gap between two items -->
<fui-stack direction="row">
  <span>Label</span>
  <fui-spacer size="2rem"></fui-spacer>
  <span>Value</span>
</fui-stack>"""
    }
    {
        Tag         = "fui-aspect-ratio"
        Name        = "AspectRatio"
        Slug        = "aspect-ratio"
        Category    = "Layout"
        Description = "Constrains slotted content to a fixed aspect ratio using the CSS aspect-ratio property. Common values: 16/9 (video), 4/3 (traditional), 1/1 (square), 2/1 (wide banner)."
        Attributes  = [
            { Name="ratio"; Type="string"; Default="16/9"; Description="CSS aspect-ratio value — e.g. 16/9, 4/3, 1/1, 2/1, 3/2" }
        ]
        CssProps = [
            { Name="--fui-ar-bg"; Description="Background colour of the aspect-ratio box (default transparent)" }
        ]
        Events   = []
        HtmlUsage = """<!-- 16:9 video thumbnail -->
<fui-aspect-ratio ratio="16/9">
  <img src="thumbnail.jpg" style="width:100%;height:100%;object-fit:cover" alt="Video thumbnail" />
</fui-aspect-ratio>

<!-- Square avatar crop -->
<div style="width:96px">
  <fui-aspect-ratio ratio="1/1">
    <img src="avatar.jpg" style="width:100%;height:100%;object-fit:cover;border-radius:50%" alt="Avatar" />
  </fui-aspect-ratio>
</div>

<!-- Wide banner placeholder -->
<fui-aspect-ratio ratio="3/1" style="--fui-ar-bg:#1E1E21">
  <div style="width:100%;height:100%;display:flex;align-items:center;justify-content:center">
    Banner content
  </div>
</fui-aspect-ratio>"""
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
    WebComponents.FuiFileUpload.register ()
    WebComponents.FuiBadge.register ()
    WebComponents.FuiSpinner.register ()
    WebComponents.FuiProgress.register ()
    WebComponents.FuiAlert.register ()
    WebComponents.FuiSkeleton.register ()
    WebComponents.FuiToast.register ()
    WebComponents.FuiEmptyState.register ()
    WebComponents.FuiModal.register ()
    WebComponents.FuiDrawer.register ()
    WebComponents.FuiTooltip.register ()
    WebComponents.FuiPopover.register ()
    WebComponents.FuiConfirmDialog.register ()
    WebComponents.FuiCard.register ()
    WebComponents.FuiStat.register ()
    WebComponents.FuiAvatar.register ()
    WebComponents.FuiAvatarGroup.register ()
    WebComponents.FuiTag.register ()
    WebComponents.FuiCodeBlock.register ()
    WebComponents.FuiCallout.register ()
    WebComponents.FuiList.register ()
    WebComponents.FuiTimeline.register ()
    WebComponents.FuiAccordion.register ()
    WebComponents.FuiCarousel.register ()
    WebComponents.FuiTable.register ()
    WebComponents.FuiDivider.register ()
    WebComponents.FuiStack.register ()
    WebComponents.FuiScrollArea.register ()
    WebComponents.FuiContainer.register ()
    WebComponents.FuiGrid.register ()
    WebComponents.FuiSpacer.register ()
    WebComponents.FuiAspectRatio.register ()
    WebComponents.FuiTabs.register ()
    WebComponents.FuiBreadcrumb.register ()
    WebComponents.FuiPagination.register ()
    WebComponents.FuiStepper.register ()
    WebComponents.FuiMenu.register ()
