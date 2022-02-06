open System
open System.Runtime.CompilerServices
open FSharp.NativeInterop
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open BenchmarkDotNet.Diagnosers

#nowarn "9" // Yes, I'm using pointers

// From Bartosz Sypytkowski blogpost:
// https://bartoszsypytkowski.com/writing-high-performance-f-code/
let inline stackalloc<'a when 'a: unmanaged> (length: int): Span<'a> =
    let p = NativePtr.stackalloc<'a> length |> NativePtr.toVoidPtr
    Span<'a>(p, length)

[<Struct>]
type Int64Tracker =
    private {
        mutable Value : int64
    }
    static member Init () =
        { Value = 0L }

    member this.IsSet (position: int) =
        (this.Value &&& (1L <<< position)) <> 0L

    member this.Set (position: int) =
        this.Value <- (1L <<< position) ||| this.Value

    member this.UnSet (position: int) =
        this.Value <- ~~~ (1 <<< position) &&& this.Value


[<MemoryDiagnoser>]
type Benchmarks () =

    let testIndexCount = 1_000_000
    let indexRange = 50
    let rng = Random 123

    let testIndexes =
        [| for _ = 1 to testIndexCount do
            // Note: Next is exclusive on the upper bound
            rng.Next (0, indexRange) 
        |]


    [<Benchmark>]
    member _.SetTracker () =
        let mutable tracker = Set.empty

        for i = 0 to testIndexes.Length - 1 do
            let testIndex = testIndexes[i]
            if tracker.Contains testIndex then
                // Real world we would do work here and then flip the case
                tracker <- tracker.Remove testIndex
            else
                tracker <- tracker.Add testIndex

        tracker

    [<Benchmark>]
    member _.HashSetTracker () =
        let mutable tracker = Collections.Generic.HashSet ()

        for i = 0 to testIndexes.Length - 1 do
            let testIndex = testIndexes[i]
            if tracker.Contains testIndex then
                // Real world we would do work here and then flip the case
                tracker.Remove testIndex |> ignore
            else
                tracker.Add testIndex |> ignore

        tracker

    [<Benchmark>]
    member _.BoolArrayTracker () =
        let tracker = Array.create indexRange false

        for i = 0 to testIndexes.Length - 1 do
            let testIndex = testIndexes[i]
            if tracker[testIndex] then
                // Real world we would do work here and then flip the case
                tracker[testIndex] <- false
            else
                tracker[testIndex] <- true

        tracker


    [<Benchmark>]
    member _.Int64Tracker () =
        let mutable tracker = Int64Tracker.Init ()
        
        for i = 0 to testIndexes.Length - 1 do
            let testIndex = testIndexes[i]
            if tracker.IsSet testIndex then
                // Real world we would do work here and then flip the case
                tracker.UnSet testIndex
            else
                tracker.Set testIndex

        tracker


[<EntryPoint>]
let main argv =
    
    let summary = BenchmarkRunner.Run<Benchmarks>()
    1