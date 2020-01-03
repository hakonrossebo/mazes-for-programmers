module Common

open System.Diagnostics

let timeOperation<'T> (func: unit -> 'T): 'T =
    let timer = Stopwatch()
    timer.Start()
    let returnValue = func()
    timer.Stop()
    printfn "Time elapsed: %d ms" timer.ElapsedMilliseconds 
    returnValue

let ifNone f =
  function
  | Some _ -> ()
  | None -> f ()