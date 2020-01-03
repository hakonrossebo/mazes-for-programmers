open MazeGrid
open Algorithms
open System.Diagnostics


let timeOperation<'T> (func: unit -> 'T): 'T =
    let timer = Stopwatch()
    timer.Start()
    let returnValue = func()
    timer.Stop()
    printfn "Time elapsed: %d ms" timer.ElapsedMilliseconds 
    returnValue


let demoBinaryTreeGridString () =
    printfn "Maze OOP version in F# - BinaryTree"
    let grid = Grid(20, 20)
    let newGrid = BinaryTree.createMaze grid
    newGrid.ToString()

let demoBinaryTreeDistanceGridString () =
    printfn "Maze OOP version in F# - BinaryTree Distance"
    let grid = DistanceGrid(10, 10)
    let newGrid = BinaryTree.createMaze grid
    let start = newGrid.GetCell(0, 0)
    let distances = start.Distances()
    newGrid.Distances <- Some(distances)
    newGrid.ToString() |> printfn "%s"

    printfn "Path from northwest corner to southwest"
    newGrid.Distances <- Some(distances.PathTo(newGrid.GetCell(newGrid.Rows - 1, 0)))
    newGrid.ToString()|> printfn "%s"




let demoBinaryTreeGridPng () =
    printfn "Maze OOP version in F# - BinaryTree Png"
    let grid = Grid(20, 20)
    let newGrid = BinaryTree.createMaze grid
    let img = newGrid.ToPng(30)
    img.Save("maze.png")
    let mutable p = new Process()
    p.StartInfo.FileName <- "maze.png"
    p.StartInfo.UseShellExecute <- true
    p.Start() |> ignore
    newGrid.ToString()


let demoSidewinderGridString () =
    printfn "Maze OOP version in F# - Sidewinder"
    let grid = Grid(10, 10)
    let newGrid = Sidewinder.createMaze grid
    newGrid.ToString()
    
let demoSidewinderGridPng () =
    printfn "Maze OOP version in F# - Sidewinder Png"
    let grid = Grid(20, 20)
    let newGrid = Sidewinder.createMaze grid
    let img = newGrid.ToPng(30)
    img.Save("sidewinder-maze.png")
    let mutable p = new Process()
    p.StartInfo.FileName <- "sidewinder-maze.png"
    p.StartInfo.UseShellExecute <- true
    p.Start() |> ignore
    newGrid.ToString()


[<EntryPoint>]
let main argv =

    // demoBinaryTreeGridString() |> printfn "%s"
    // demoBinaryTreeDistanceGridString |> timeOperation |> ignore
    // demoBinaryTreeGridPng() |> printfn "%s"
    demoSidewinderGridString() |> printfn "%s"
    // demoSidewinderGridPng() |> printfn "%s"
    printfn "Grid created"
    0 // return an integer exit code




