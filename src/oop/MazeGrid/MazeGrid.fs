namespace MazeGrid
open System.Collections.Generic
open System.Drawing
open System.Linq

/// Cell class
type Cell (row:int, column:int) =
    let mutable links = new Dictionary<Cell, bool>()
    member this.Row = row
    member this.Column = column
    member val  North : Cell option = None with get, set
    member val  South : Cell option = None with get, set
    member val  West : Cell option = None with get, set
    member val  East : Cell option = None with get, set
    member this.Neighbors 
        with get() = [this.North;this.South;this.West;this.East] |> List.choose id

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

    member this.IsLinked(cell: Cell option ) =
        cell
        |> Option.map links.ContainsKey
        |> Option.defaultValue false

    member this.Distances() =
        let distances = Distances(this)
        let mutable frontier = ResizeArray<Cell>() 
        frontier.Add(this)
        while frontier.Any() do
            let newFrontier = ResizeArray<Cell>()
            for cell in frontier do
                for linked in cell.Links() do
                    if not (distances.ContainsKey(linked)) then
                        distances.Add(linked, distances.[cell] + 1)
                        newFrontier.Add(linked)
            frontier <- newFrontier
        distances

/// Distances class
and Distances(root:Cell) =
    inherit Dictionary<Cell, int>()
    do base.[root] <- 0
    member this.Root = root
    member this.Cells = base.Keys |> Seq.toList
    member this.SetDistance(cell:Cell, distance:int) = 
        base.[cell] = distance |> ignore

/// Grid class
type Grid(rows: int, columns:int) as this =
    let mutable cells = array2D []
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
                    cell.North <- Some(this.Cells.[row-1, col])
                if row + 1 < this.Rows then
                    cell.South <- Some(this.Cells.[row+1, col])
                if col - 1 >= 0 then
                    cell.West <- Some(this.Cells.[row, col - 1])
                if col + 1  < this.Columns then
                    cell.East <- Some(this.Cells.[row, col + 1])
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
    abstract member ContensOf: Cell -> string
    default this.ContensOf cell = " "

    override this.ToString() =
        let mutable output = "+"
        for _ in [0 .. this.Columns - 1] do
            output <- output + "---+"
        output <- output + "\n"

        for cellRow in this.EachRow() do
            let mutable top = "|"
            let mutable bottom = "+"
            for cell in cellRow do  
                let body = sprintf " %s " (this.ContensOf cell)
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
            if Option.isNone cell.North then
                graphics.DrawLine(wall, x1, y1, x2, y1)
            if Option.isNone cell.West then
                graphics.DrawLine(wall, x1, y1, x1, y2)
            if not (cell.IsLinked(cell.East)) then
                graphics.DrawLine(wall, x2, y1, x2, y2)
            if not (cell.IsLinked(cell.South)) then
                graphics.DrawLine(wall, x1, y2, x2, y2)
        mazeImage

/// DistanceGrid class
type DistanceGrid(rows:int, columns:int) =
    inherit Grid(rows, columns)
    member val Distances:Distances option = None with get, set
    override this.ContensOf(cell:Cell) =
        this.Distances
        |> Option.map (fun distances ->
                    if distances.ContainsKey(cell) then   
                        distances.[cell].ToString().Last().ToString()
                    else
                        " "
                    )
                    |> Option.defaultValue " "

















