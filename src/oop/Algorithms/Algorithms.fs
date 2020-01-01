namespace Algorithms

open MazeGrid

module BinaryTree =
    let createMaze (grid:'T) : 'T when 'T :> Grid = 
        let rnd = System.Random()
        grid.EachCell()
        |> Seq.iter (fun (cell:Cell) ->
            let neighbors =
                [cell.North;cell.East] |> List.choose id
            let index = rnd.Next(List.length neighbors)
            if index < List.length neighbors then 
                let neighbor = neighbors.[index]
                cell.Link(neighbor) |> ignore
            )
        grid

module Sidewinder =
    let createMaze (grid:'T) : 'T when 'T :> Grid = 
        let rnd = System.Random()
        for cellRow in grid.EachRow() do
            let mutable run = new ResizeArray<Cell>()
            for cell in cellRow do  
                run.Add(cell)
                let atEasterBoundrary = Option.isNone cell.East
                let atNorthernBoundrary = Option.isNone cell.North
                let shouldCloseOut = atEasterBoundrary || (not atNorthernBoundrary && rnd.Next(2) = 0)
                if shouldCloseOut then
                    let mmember = run.[rnd.Next(run.Count - 1)]
                    mmember.North |> Option.iter (mmember.Link >> ignore)
                    run.Clear()
                else    
                    cell.East |> Option.iter (cell.Link >> ignore)
        grid

