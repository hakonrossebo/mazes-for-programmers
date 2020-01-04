open MazeGrid
open Algorithms
open System.Diagnostics
open Common

let demoBinaryTreeGridString () =
    printfn "Maze FP version in F# - BinaryTree"
    let grid = Grid.createGrid 10 10
    let newGrid = BinaryTree.createMaze grid
    newGrid |> Grid.toString None

let demoBinaryTreeGridPng () =
    printfn "Maze FP version in F# - BinaryTree Png"
    let grid = Grid.createGrid 20 20
    let newGrid = BinaryTree.createMaze grid
    let img = Grid.toPng None newGrid 30
    img.Save("maze.png")
    let mutable p = new Process()
    p.StartInfo.FileName <- "maze.png"
    p.StartInfo.UseShellExecute <- true
    p.Start() |> ignore
    newGrid.ToString()

let demoBinaryDistanceColorTreeGridPng () =
    printfn "Maze FP version in F# - BinaryTree Distance color Png"
    let grid = Grid.createGrid 100 100
    let newGrid = BinaryTree.createMaze grid
    let start = Grid.getCell newGrid (Array2D.length1 newGrid / 2, Array2D.length2 newGrid / 2)
    let distances = Distances.distances newGrid start
    let img = Grid.toPng (Some distances) newGrid 10
    img.Save("colorized.png")
    let mutable p = new Process()
    p.StartInfo.FileName <- "colorized.png"
    p.StartInfo.UseShellExecute <- true
    p.Start() |> ignore

let demoBinaryTreeDistanceGridString () =
    printfn "Maze FP version in F# - BinaryTree Distance"
    let grid = Grid.createGrid 10 10
    let newGrid = BinaryTree.createMaze grid
    let start = Grid.getCell newGrid (0,0)
    let distances = Distances.distances newGrid start
    newGrid |> Grid.toString (Some (distances)) |> printfn "%s"
    printfn "Path from northwest corner to southwest"
    let goal = Grid.getCell newGrid (Array2D.length1 newGrid - 1, 0)
    let pathDistances = Distances.pathTo newGrid distances start goal
    newGrid |> Grid.toString (Some (pathDistances)) |> printfn "%s"

let demoLongestPathBinaryTreeDistanceGridString () =
    printfn "Maze FP version in F# - BinaryTree Longest path Distance"
    let grid = Grid.createGrid 5 5
    let newGrid = BinaryTree.createMaze grid
    let start = Grid.getCell newGrid (0,0)
    let distances = Distances.distances newGrid start
    let newStart, distance = Distances.max distances

    let newDistances = Distances.distances newGrid (Grid.getCell newGrid newStart.pos)
    let goal, distance = Distances.max newDistances

    newGrid |> Grid.toString (Some (distances)) |> printfn "%s"
    printfn "Path from northwest corner to southwest"
    // let goal = Grid.getCell newGrid (Array2D.length1 newGrid - 1, 0)
    let pathDistances = Distances.pathTo newGrid newDistances newStart goal
    newGrid |> Grid.toString (Some (pathDistances)) |> printfn "%s"

let demoSidewinderGridString () =
    printfn "Maze FP version in F# - Sidewinder"
    let grid = Grid.createGrid 10 10
    let newGrid = Sidewinder.createMaze grid
    newGrid |> Grid.toString None
    
let demoSidewinderGridPng () =
    printfn "Maze FP version in F# - Sidewinder Png"
    let grid = Grid.createGrid 20 20 
    let newGrid = Sidewinder.createMaze grid
    let img = Grid.toPng None newGrid 30
    img.Save("sidewinder-maze.png")
    let mutable p = new Process()
    p.StartInfo.FileName <- "sidewinder-maze.png"
    p.StartInfo.UseShellExecute <- true
    p.Start() |> ignore
    newGrid.ToString()

[<EntryPoint>]
let main argv =
    // demoBinaryTreeGridString() |> printfn "%s"
    // demoBinaryTreeGridPng() |> printfn "%s"
    demoBinaryDistanceColorTreeGridPng |> timeOperation |> ignore
    // demoBinaryTreeDistanceGridString |> timeOperation |> ignore
    // demoLongestPathBinaryTreeDistanceGridString |> timeOperation |> ignore
    // demoSidewinderGridString() |> printfn "%s"
    // demoSidewinderGridPng() |> printfn "%s"
    printfn "Grid created"
    0 // return an integer exit code




