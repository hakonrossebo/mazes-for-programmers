
open System
open MazeGrid
open Algorithms //.BinaryTree


let demoBinaryTreeGridString () =
    printfn "Maze OOP version in F# - BinaryTree"
    let grid = Grid(4,4)
    let newGrid = BinaryTree.createMaze grid
    newGrid.ToString()

let demoSidewinderGridString () =
    printfn "Maze OOP version in F# - Sidewinder"
    let grid = Grid(4,4)
    let newGrid = Sidewinder.createMaze grid
    newGrid.ToString()
    


[<EntryPoint>]
let main argv =
    // demoBinaryTreeGridString() |> printfn "%s"
    demoSidewinderGridString() |> printfn "%s"
    printfn "Grid created"
    0 // return an integer exit code




