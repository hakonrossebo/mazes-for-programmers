namespace MazeGrid
open System.Collections.Generic
open System.Drawing

[<AllowNullLiteral>]
type Cell (row:int, column:int) =
    let mutable links = new Dictionary<Cell, bool>()
    member this.Row = row
    member this.Column = column
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

type Grid(rows: int, columns:int) as this =
    let mutable cells = Array2D.create 1 1 (Cell(1, 1))
    let rnd = System.Random()
    do cells <- this.PrepareGrid()
    do this.ConfigureCells()
    member this.Rows = rows
    member this.Columns = columns
    member this.Size = rows * columns
    member this.Cells
        with get() = cells
        and set(value) = cells <- value

    member this.PrepareGrid() =
        Array2D.init rows columns (fun x y ->  Cell(x, y))

    member this.ConfigureCells() =
        this.Cells |> Array2D.iter (fun cell -> 
                let row = cell.Row
                let col = cell.Column
                if row - 1 >= 0 then
                    cell.North <- this.Cells.[row-1, col]
                if row + 1 < this.Rows then
                    cell.South <- this.Cells.[row+1, col]
                if col - 1 >= 0 then
                    cell.West <- this.Cells.[row, col - 1]
                if col + 1  < this.Columns then
                    cell.East <- this.Cells.[row, col + 1]
            )

    member this.GetCell(row, column) =
        this.Cells.[row, column]

    member this.RandomCell() =
        let row = rnd.Next(this.Rows)
        let column = rnd.Next(this.Columns)
        this.Cells.[row, column]

    member this.EachRow() =
        seq {
            for row in [0..this.Rows - 1] do
                    yield this.Cells.[row,*]
        }

    member this.EachCell() =
        seq {
            for row in [0..this.Rows - 1] do
                for col in [0..this.Columns - 1] do
                    yield this.Cells.[row,col]
        }

    override this.ToString() =
        let mutable output = "+"
        for _ in [0 .. this.Columns - 1] do
            output <- output + "---+"
        output <- output + "\n"

        for cellRow in this.EachRow() do
            let mutable top = "|"
            let mutable bottom = "+"
            for cell in cellRow do  
                let body = "   " 
                let eastBoundrary = if cell.IsLinked(cell.East) then " " else "|"
                top <- top + body + eastBoundrary
                let southBoundrary = if cell.IsLinked(cell.South) then "   " else "---"
                let corner = "+"
                bottom <- bottom + southBoundrary + corner
            output <- output + top + "\n"
            output <- output + bottom + "\n"
        output

    member this.ToPng(?cellSize) =
        let cellSize = defaultArg cellSize 10
        let imgWidth = cellSize * this.Columns + 1
        let imgHeight = cellSize * this.Rows + 1

        let background = Brushes.White
        let wall = Pens.Black
        let mazeImage = new Bitmap(imgWidth, imgHeight)
        use graphics = Graphics.FromImage(mazeImage)
        graphics.FillRectangle(background, 0, 0, imgWidth, imgHeight)

        for cell in this.EachCell() do
            let x1 = cell.Column * cellSize
            let y1 = cell.Row * cellSize
            let x2 = (cell.Column + 1) * cellSize
            let y2 = (cell.Row + 1) * cellSize
            if isNull cell.North then
                graphics.DrawLine(wall, x1, y1, x2, y1)
            if isNull cell.West then
                graphics.DrawLine(wall, x1, y1, x1, y2)
            if not (cell.IsLinked(cell.East)) then
                graphics.DrawLine(wall, x2, y1, x2, y2)
            if not (cell.IsLinked(cell.South)) then
                graphics.DrawLine(wall, x1, y2, x2, y2)
        mazeImage














