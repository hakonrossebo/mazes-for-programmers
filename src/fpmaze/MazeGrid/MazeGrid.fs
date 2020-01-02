namespace MazeGrid

open System.Drawing
open System.Text

type Position = int * int
type Cell = {
    pos: Position
    north: Position option
    south: Position option
    east: Position option
    west: Position option
    links: Position Set
}
type Grid = Cell [,]

module Grid =
    let createGrid rows columns : Grid =
        let getValidNeighbors (cell:Cell) =
            let row, col = cell.pos
            let north = if row - 1 >= 0 then Some(row - 1, col) else None
            let south = if row + 1 < rows then Some(row + 1, col) else None
            let west = if col - 1 >= 0 then Some(row, col - 1) else None
            let east = if col + 1 < columns then Some(row, col + 1) else None
            {cell with north = north; south = south; east = east; west = west}
        Array2D.init rows columns (fun x y -> {pos = (x,y); north = None; south = None; east = None; west = None; links = Set.empty})
        |> Array2D.map getValidNeighbors

    let getCell (grid:Grid) (pos:Position) =
        let row, col = pos
        grid.[row, col]
    
    let isLinked (cell1:Cell) (cell2pos: Position option) =
        cell2pos
        |> Option.map (fun c -> Set.contains c cell1.links)
        |> Option.defaultValue false
        
    let eachRow(grid:Grid) =
        seq {
            for row in 0 .. (Array2D.length1 grid) - 1 do
                yield grid.[row, *]
        }

    let eachCell(grid:Grid) =
        seq {
            for row in 0 .. (Array2D.length1 grid) - 1 do
                for col in 0 .. (Array2D.length2 grid) - 1 do
                    yield grid.[row, col]
        }
    let linkCells(grid:Grid) (cell1:Cell) (cell2:Cell) =
        let cell1new = cell1.links
                        |> Set.add cell2.pos
                        |> fun links -> {cell1 with links = links}
        let cell2new = cell2.links
                        |> Set.add cell1.pos
                        |> fun links -> {cell2 with links = links}
        let cell1Row, cell1Col = cell1.pos
        let cell2Row, cell2Col = cell2.pos
        grid.[cell1Row, cell1Col] <- cell1new
        grid.[cell2Row, cell2Col] <- cell2new

    let toString(grid:Grid) =
        let cols = Array2D.length2 grid
        let corner = "+"
        let sbGrid = StringBuilder()
        sbGrid.AppendLine("+" + ("---+" |> Seq.replicate cols |> String.concat "")) |> ignore
        for cellRow in eachRow grid do
            let sbGridLineTop = StringBuilder("|")
            let sbGridLineBottom = StringBuilder("+")
            for cell in cellRow do
                // let body = sprintf " %s " (this.ContensOf cell)
                let body = sprintf "   "
                let eastBoundrary = if isLinked cell cell.east then " " else "|"
                let southBoundrary = if isLinked cell cell.south then "   " else "---"
                sbGridLineTop.Append(body + eastBoundrary) |> ignore
                sbGridLineBottom.Append(southBoundrary + corner) |> ignore
            sbGrid.AppendLine(sbGridLineTop.ToString()) |> ignore
            sbGrid.AppendLine(sbGridLineBottom.ToString()) |> ignore
        sbGrid.ToString()

    let toPng (grid: Grid) cellSize =
        let imgWidth = cellSize * Array2D.length2 grid + 1
        let imgHeight = cellSize * Array2D.length1 grid + 1

        let background = Brushes.White
        let wall = Pens.Black
        let mazeImage = new Bitmap(imgWidth, imgHeight)
        use graphics = Graphics.FromImage(mazeImage)
        graphics.FillRectangle(background, 0, 0, imgWidth, imgHeight)

        for cell in eachCell grid do
            let row, col = cell.pos
            let x1 = col * cellSize
            let y1 = row * cellSize
            let x2 = (col + 1) * cellSize
            let y2 = (row + 1) * cellSize
            if Option.isNone cell.north then graphics.DrawLine(wall, x1, y1, x2, y1)
            if Option.isNone cell.west then graphics.DrawLine(wall, x1, y1, x1, y2)
            if not (isLinked cell cell.east) then graphics.DrawLine(wall, x2, y1, x2, y2)
            if not (isLinked cell cell.south) then graphics.DrawLine(wall, x1, y2, x2, y2)
        mazeImage
