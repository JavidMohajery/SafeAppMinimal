module WebComponents.FuiTable

open Lit
open Fable.Core

[<Import("unsafeCSS", "lit")>]
let private unsafeCSS: string -> CSSResult = jsNative

[<Emit("JSON.parse($0)")>]
let private parseCols (_: string) : {| key: string; label: string; sortable: bool |} array = jsNative

[<Emit("JSON.parse($0)")>]
let private parseRows (_: string) : string[] array = jsNative

let private styles = unsafeCSS """
    :host {
        display    : block;
        font-family: var(--fui-table-font, 'Sora', sans-serif);
    }
    :host([hidden]) { display: none; }

    .wrap {
        border       : 1px solid var(--fui-table-border, #2A2A2E);
        border-radius: var(--fui-table-radius, 8px);
        overflow     : hidden;
    }

    table {
        border-collapse: collapse;
        width          : 100%;
    }

    thead tr {
        background   : var(--fui-table-head-bg, #1E1E21);
        border-bottom: 1px solid var(--fui-table-border, #2A2A2E);
    }

    th {
        color         : var(--fui-table-head-color, #E8E8ED);
        font-family   : 'JetBrains Mono', monospace;
        font-size     : 0.7rem;
        font-weight   : 600;
        letter-spacing: 0.06em;
        padding       : 0.625rem 0.875rem;
        text-align    : left;
        text-transform: uppercase;
        white-space   : nowrap;
    }

    th.sortable {
        cursor    : pointer;
        user-select: none;
        transition: color 0.15s;
    }
    th.sortable:hover { color: #7C3AED; }
    th.sortable:focus-visible {
        outline       : 2px solid #7C3AED;
        outline-offset: -2px;
    }

    .sort-icon      { color: #3A3A3E;  font-size: 0.6rem; margin-left: 0.25rem; }
    .sort-icon.asc  { color: #7C3AED; }
    .sort-icon.desc { color: #7C3AED; }

    tbody tr {
        border-bottom: 1px solid var(--fui-table-border, #2A2A2E);
        transition   : background 0.1s;
    }
    tbody tr:last-child { border-bottom: none; }
    tbody tr:hover      { background: rgba(255,255,255,0.025); }

    :host([striped]) tbody tr:nth-child(odd) { background: rgba(255,255,255,0.02); }
    :host([striped]) tbody tr:hover          { background: rgba(255,255,255,0.04); }

    td {
        color    : var(--fui-table-color, #A0A0A8);
        font-size: 0.9rem;
        padding  : 0.625rem 0.875rem;
    }

    .empty {
        color    : #6E6E76;
        font-size: 0.875rem;
        padding  : 2rem;
        text-align: center;
    }
"""

[<LitElement("fui-table")>]
let FuiTable () =
    let _host, props = LitElement.init(fun init ->
        init.styles <- [ styles ]
        init.props  <- {|
            columns  = Prop.Of("[]",  attribute = "columns")
            rows     = Prop.Of("[]",  attribute = "rows")
            striped  = Prop.Of(false, attribute = "striped")
            sortable = Prop.Of(false, attribute = "sortable")
        |}
    )

    let cols    = parseCols props.columns.Value
    let rawRows = parseRows props.rows.Value

    let sortCol, setSortCol = Hook.useState -1
    let sortAsc, setSortAsc = Hook.useState true

    let clickSort i _ =
        if sortCol = i then setSortAsc (not sortAsc)
        else
            setSortCol i
            setSortAsc true

    let sortedRows =
        if sortCol < 0 then rawRows
        else
            let sorted = rawRows |> Array.sortBy (fun row ->
                if sortCol < row.Length then row.[sortCol] else "")
            if sortAsc then sorted else Array.rev sorted

    let renderHeader i (col: {| key: string; label: string; sortable: bool |}) =
        let canSort = props.sortable.Value && col.sortable
        if canSort then
            let sortIconCls =
                if sortCol = i then (if sortAsc then "sort-icon asc" else "sort-icon desc")
                else "sort-icon"
            let sortChar = if sortCol = i then (if sortAsc then "▲" else "▼") else "⇅"
            html $"""
                <th class="sortable" tabindex="0" @click={Ev (clickSort i)}>
                    {col.label}<span class={sortIconCls}>{sortChar}</span>
                </th>"""
        else
            html $"""<th>{col.label}</th>"""

    let renderRow (row: string[]) =
        let cells =
            cols |> Array.mapi (fun i _ ->
                let v = if i < row.Length then row.[i] else ""
                html $"""<td>{v}</td>""")
        html $"""<tr>{cells}</tr>"""

    let headers  = cols |> Array.mapi renderHeader
    let bodyRows = sortedRows |> Array.map renderRow

    let tbody =
        if rawRows.Length = 0 then
            html $"""<tr><td colspan={string cols.Length} class="empty">No data</td></tr>"""
        else
            html $"""<tbody>{bodyRows}</tbody>"""

    html $"""
        <div class="wrap" part="wrap">
            <table part="table">
                <thead><tr>{headers}</tr></thead>
                {tbody}
            </table>
        </div>
    """

let register () = ()
