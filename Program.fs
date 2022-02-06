open System
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
    static member Create () =
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
                tracker <- tracker.Add testIndex
            else
                tracker <- tracker.Remove testIndex

        tracker

    [<Benchmark>]
    member _.HashSetTracker () =
        let mutable tracker = Collections.Generic.HashSet ()

        for i = 0 to testIndexes.Length - 1 do
            let testIndex = testIndexes[i]
            if tracker.Contains testIndex then
                // Real world we would do work here and then flip the case
                tracker.Add testIndex |> ignore
            else
                tracker.Remove testIndex |> ignore

        tracker

    [<Benchmark>]
    member _.BoolArrayTracker () =
        let tracker = Array.create indexRange false

        for i = 0 to testIndexes.Length - 1 do
            let testIndex = testIndexes[i]
            if tracker[testIndex] = false then
                // Real world we would do work here and then flip the case
                tracker[testIndex] <- true
            else
                tracker[testIndex] <- false

        tracker


    [<Benchmark>]
    member _.Int64Tracker () =
        let mutable tracker = Int64Tracker.Create ()
        
        for i = 0 to testIndexes.Length - 1 do
            let testIndex = testIndexes[i]
            if tracker.IsSet testIndex then
                // Real world we would do work here and then flip the case
                tracker.Set testIndex
            else
                tracker.UnSet testIndex

        tracker


    [<Benchmark>]
    member _.SpanInt64Tracker () =
        let bitsPerInt64 = 64
        let requiredInt64s = (indexRange + bitsPerInt64 - 1) / bitsPerInt64
        let spanIndex = stackalloc<int64> requiredInt64s

        for i = 0 to testIndexes.Length - 1 do
            let testIndex = testIndexes[i]
            let int64Index = testIndex / bitsPerInt64
            let bitIndex = testIndex % bitsPerInt64

            if (spanIndex[int64Index] &&& (1 <<< bitIndex)) <> 0 then
                // Real world we would do work here and then flip the case
                spanIndex[int64Index] <- (1 <<< bitIndex) ||| spanIndex[int64Index]
            else
                spanIndex[int64Index] <- ~~~ (1 <<< bitIndex) &&& spanIndex[int64Index]

        spanIndex


[<EntryPoint>]
let main argv =
    
    let summary = BenchmarkRunner.Run<Benchmarks>()
    1