namespace Algorithms

open MazeGrid

module BinaryTree =
    let createMaze (grid:Grid) =
        let getCell = Grid.getCell grid
        let linkCells = Grid.linkCells grid
        let rnd = System.Random()
        grid
        |> Grid.eachCell
        |> Seq.iter (fun (cell:Cell) ->
            let neighbors =
                [cell.north;cell.east] |> List.choose id
            let index = rnd.Next(List.length neighbors)
            if index < List.length neighbors then 
                let neighbor = neighbors.[index] |> getCell
                linkCells cell neighbor
            )
        grid

module Sidewinder =
    let createMaze (grid:Grid) =
        let getCell = Grid.getCell grid
        let linkCells = Grid.linkCells grid
        let rnd = System.Random()
        for cellRow in Grid.eachRow grid do
            let mutable run = new ResizeArray<Cell>()
            for cell in cellRow do  
                run.Add(cell)
                let atEasterBoundrary = Option.isNone cell.east
                let atNorthernBoundrary = Option.isNone cell.north
                let shouldCloseOut = atEasterBoundrary || (not atNorthernBoundrary && rnd.Next(2) = 0)
                if shouldCloseOut then
                    let mmember = run.[rnd.Next(run.Count - 1)]
                    mmember.north 
                    |> Option.map (Grid.getCell grid) 
                    |> Option.iter (linkCells (getCell mmember.pos))
                    run.Clear()
                else    
                    cell.east 
                    |> Option.map (Grid.getCell grid)
                    |> Option.iter (linkCells (getCell cell.pos))
        grid