
open System
open MazeGrid
open Algorithms.BinaryTree


let demoBinaryTreeGridString () =
    let grid = Grid(10,10)
    let newGrid = createMaze grid
    newGrid.ToString()
    


[<EntryPoint>]
let main argv =
    printfn "Maze OOP version in F#"
    demoBinaryTreeGridString() |> printfn "%s"
    printfn "Grid created"
    0 // return an integer exit code




