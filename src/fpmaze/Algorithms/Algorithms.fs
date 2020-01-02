namespace Algorithms

open MazeGrid

module BinaryTree =
    let createMaze (grid:Grid) =
        let rnd = System.Random()
        grid
        |> Grid.eachCell
        |> Seq.iter (fun (cell:Cell) ->
            let neighbors =
                [cell.north;cell.east] |> List.choose id
            let index = rnd.Next(List.length neighbors)
            if index < List.length neighbors then 
                let neighbor = neighbors.[index] |> (Grid.getCell grid)
                Grid.linkCells grid cell neighbor
            )
        grid

module Sidewinder =
    let createMaze (grid:Grid) =
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
                    // mmember.north |> Option.iter (mmember.Link >> ignore)
                    mmember.north 
                    |> Option.map (Grid.getCell grid) 
                    |> Option.iter (fun mn -> Grid.linkCells grid mmember mn)
                    run.Clear()
                else    
                    // cell.East |> Option.iter (cell.Link >> ignore)
                    cell.east 
                    |> Option.iter (fun c -> Grid.linkCells grid cell (Grid.getCell grid c))
        grid

