module Tests

open System
open Xunit
open MazeGrid

[<Fact>]
let ``Init cell`` () =
    let cell = Cell(10,10)
    Assert.True(isNull cell.North)

[<Fact>]
let ``Init grid`` () =
    let grid = Grid(10,10)
    let c = grid.Size
    Assert.Equal(100, c)

[<Fact>]
let ``Init grid check cells`` () =
    let grid = Grid(10,10)
    let c = grid.GetCell(0,0)
    Assert.True(isNull c.North)
    let s = grid.GetCell(0,0)
    Assert.False(isNull s.South)
