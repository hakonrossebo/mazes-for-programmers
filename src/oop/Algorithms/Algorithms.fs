namespace Algorithms
open MazeGrid

module BinaryTree =
    let createMaze (grid:Grid) : Grid = 
        let rnd = System.Random()
        grid.EachCell()
        |> Seq.iter (fun (cell:Cell) ->
            let neighbors =
                [
                    if not (isNull cell.North) then yield cell.North
                    if not (isNull cell.East) then yield cell.East
                ]
            let index = rnd.Next(List.length neighbors)
            if index < List.length neighbors then 
                let neighbor = neighbors.[index]
                cell.Link(neighbor) |> ignore
            )
        grid

module Sidewinder =
    let createMaze (grid:Grid) : Grid = 
        let rnd = System.Random()
        for cellRow in grid.EachRow() do
            let mutable run = new ResizeArray<Cell>()
            for cell in cellRow do  
                run.Add(cell)
                let atEasterBoundrary = isNull cell.East
                let atNorthernBoundrary = isNull cell.North
                let shouldCloseOut = atEasterBoundrary || (not atNorthernBoundrary && rnd.Next(2) = 0)
                if shouldCloseOut then
                    let mmember = run.[rnd.Next(run.Count - 1)]
                    if not (isNull mmember.North) then
                        mmember.Link(mmember.North) |> ignore
                    run.Clear()
                else    
                    cell.Link(cell.East) |> ignore
        grid

