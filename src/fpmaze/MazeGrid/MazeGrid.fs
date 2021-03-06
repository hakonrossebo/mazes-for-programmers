namespace MazeGrid

open System.Collections.Generic
open System.Drawing
open System.Text
open System.Linq
open Common

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

type Distances = Dictionary<Cell, int>

module Grid =
    let createCell pos =
        {pos = pos; north = None; south = None; east = None; west = None; links = Set.empty}

    let createGrid rows columns : Grid =
        let getValidNeighbors (cell:Cell) =
            let row, col = cell.pos
            let north = if row - 1 >= 0 then Some(row - 1, col) else None
            let south = if row + 1 < rows then Some(row + 1, col) else None
            let west = if col - 1 >= 0 then Some(row, col - 1) else None
            let east = if col + 1 < columns then Some(row, col + 1) else None
            {cell with north = north; south = south; east = east; west = west}
        Array2D.init rows columns (fun x y -> createCell (x, y))
        |> Array2D.map getValidNeighbors
    
    let dimensions (grid:Grid) = (Array2D.length1 grid, Array2D.length2 grid)

    let getCell (grid:Grid) (pos:Position) =
        let row, col = pos
        grid.[row, col]
    
    let isLinked (cell1:Cell) (cell2pos: Position option) =
        cell2pos
        |> Option.map (fun c -> Set.contains c cell1.links)
        |> Option.defaultValue false
        
    let eachRow(grid:Grid) =
        let rows, _ = dimensions grid
        seq {
            for row in 0 .. rows - 1 do
                yield grid.[row, *]
        }

    let eachCell(grid:Grid) =
        let rows, cols = dimensions grid
        seq {
            for row in 0 .. rows - 1 do
                for col in 0 .. cols - 1 do
                    yield grid.[row, col]
        }
    
    let toSeq (grid: Cell [,]) = grid |> Seq.cast<Cell> 

    let linkCells(grid:Grid) (cell1:Cell) (cell2:Cell) =
        let cell1new =  {cell1 with links = Set.add cell2.pos cell1.links}
        let cell2new =  {cell2 with links = Set.add cell1.pos cell2.links}
        let cell1Row, cell1Col = cell1.pos
        let cell2Row, cell2Col = cell2.pos
        grid.[cell1Row, cell1Col] <- cell1new
        grid.[cell2Row, cell2Col] <- cell2new

    let contensOf (distances: Distances) (cell: Cell) =
            if distances.ContainsKey(cell)
            then distances.[cell].ToString().Last().ToString()
            else " "

    let backgroundColorFor (distances: Distances) (maximum:int) (cell: Cell) =
        if distances.ContainsKey(cell) then 
            let distance = distances.[cell]
            let intensity = (float maximum - float distance) / float maximum
            let dark = 255. * intensity |> int
            let brigth = 128 + (127. * intensity |> int)
            Color.FromArgb(dark, brigth, dark)
        else 
            Color.White

    let toString (distances: Distances option) (grid:Grid) =
        let distances = defaultArg distances (Distances())
        let contensOf = contensOf distances
        let _, cols = dimensions grid
        let corner = "+"
        let sbGrid = StringBuilder()
        sbGrid.AppendLine("+" + ("---+" |> Seq.replicate cols |> String.concat "")) |> ignore
        for cellRow in eachRow grid do
            let sbGridLineTop = StringBuilder("|")
            let sbGridLineBottom = StringBuilder("+")
            for cell in cellRow do
                let body = sprintf " %s " (contensOf cell)
                let eastBoundrary = if isLinked cell cell.east then " " else "|"
                let southBoundrary = if isLinked cell cell.south then "   " else "---"
                sbGridLineTop.Append(body + eastBoundrary) |> ignore
                sbGridLineBottom.Append(southBoundrary + corner) |> ignore
            sbGrid.AppendLine(sbGridLineTop.ToString()) |> ignore
            sbGrid.AppendLine(sbGridLineBottom.ToString()) |> ignore
        sbGrid.ToString()
    
    let coordinates (cellSize: int) (cell: Cell) =
        let row, col = cell.pos
        let x1 = col * cellSize
        let y1 = row * cellSize
        let x2 = (col + 1) * cellSize
        let y2 = (row + 1) * cellSize
        (x1, y1, x2, y2)

    let toPng (distances: Distances option) (grid: Grid) cellSize =
        let rows, cols = dimensions grid
        let max (distances:Distances) = distances |> Seq.maxBy (fun x -> x.Value) |> (fun x -> (x.Key, x.Value))
        let imgWidth = cellSize * cols + 1
        let imgHeight = cellSize * rows + 1
        let background = Brushes.White
        let wall = Pens.Black
        let mazeImage = new Bitmap(imgWidth, imgHeight)
        use graphics = Graphics.FromImage(mazeImage)
        graphics.FillRectangle(background, 0, 0, imgWidth, imgHeight)

        distances 
        |> Option.iter (fun distances -> 
            let _, max = max distances
            let backgroundColorFor = backgroundColorFor distances max
            for cell in eachCell grid do
                let (x1, y1, x2, y2) = coordinates cellSize cell
                let color = backgroundColorFor cell
                let brush = new SolidBrush(color)
                graphics.FillRectangle(brush, x1, y1, (x2 - x1), (y2 - y1))
        )

        for cell in eachCell grid do
            let (x1, y1, x2, y2) = coordinates cellSize cell
            cell.north |> ifNone (fun _ -> graphics.DrawLine(wall, x1, y1, x2, y1))
            cell.west |> ifNone (fun _ -> graphics.DrawLine(wall, x1, y1, x1, y2))
            if not (isLinked cell cell.east) then graphics.DrawLine(wall, x2, y1, x2, y2)
            if not (isLinked cell cell.south) then graphics.DrawLine(wall, x1, y2, x2, y2)
        mazeImage

module Distances =
    open Grid
    let initDistances (initCell:Cell) = 
        let distances = Distances()
        distances.Add(initCell, 0)
        distances

    let distances (grid: Grid) (fromCell:Cell) =
        let distances = initDistances fromCell
        let rec nextFrontier frontier =
            match frontier with 
            | [] -> distances
            | f ->
                let newFrontier = [
                    for cell in f do
                        let links = cell.links
                                    |> Set.toSeq
                                    |> Seq.map (getCell grid)
                        for linked in links do
                            if not (distances.ContainsKey(linked)) then
                                distances.Add(linked, distances.[cell] + 1)
                                yield linked
                ]
                nextFrontier newFrontier
        nextFrontier [fromCell]

    let pathTo (grid:Grid) (distances:Distances) (root: Cell) (goal: Cell) =
        let breadcrumbs = initDistances root
        breadcrumbs.[goal] <- distances.[goal]
        let rec findNextBreadcrumb current =
            if current = root then
                breadcrumbs
            else
                let link = current.links
                            |> Set.toSeq
                            |> Seq.map (getCell grid)
                            |> Seq.filter (fun neighbor -> distances.[neighbor] < distances.[current])
                            |> Seq.tryHead
                match link with 
                | None -> breadcrumbs
                | Some neighbor ->
                    breadcrumbs.[neighbor] <- distances.[neighbor]
                    findNextBreadcrumb neighbor
        findNextBreadcrumb goal

    let max (distances:Distances) = 
        distances |> Seq.maxBy (fun x -> x.Value) |> (fun x -> (x.Key, x.Value))


