namespace MazeGrid
open System.Collections.Generic
open System.Linq

module Utils =
    let inline isNull (x:^T when ^T : not struct) = obj.ReferenceEquals (x, null)

[<AllowNullLiteral>]
type Cell (row:int, column:int, links: Dictionary<Cell, bool>) =
    let mutable links = links
    member this.Row = row
    member this.Column = column
    // member this.links = links

    member val  North : Cell = null with get, set
    member val  South : Cell = null with get, set
    member val  West : Cell = null with get, set
    member val  East : Cell = null with get, set

    member this.Neighbors 
        with get() = 
            [
                if not (isNull this.North) then yield this.North
                if not (isNull this.South) then yield this.South
                if not (isNull this.West) then yield this.West
                if not (isNull this.East) then yield this.East
            ]

    member this.Link(cell: Cell, ?bidi:bool) : Cell =
        links.[cell] <- true
        let bidi = defaultArg bidi true
        if bidi then
            cell.Link(this, false) |> ignore
        this

    member this.Unlink(cell: Cell, ?bidi:bool) : Cell =
        links.Remove(cell) |> ignore
        let bidi = defaultArg bidi true
        if bidi then
            cell.Unlink(this, false) |> ignore
        this

    member this.Links() : Cell list =
        List.ofSeq links.Keys

    member this.IsLinked(cell: Cell) =
        if isNull cell then
            false
        else
            links.ContainsKey(cell)

type Grid(rows: int, columns:int) =
    member this.Rows = rows
    member this.Columns = columns
    member this.Size = rows * columns

    member this.Cells : Cell [][]

