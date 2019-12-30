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


