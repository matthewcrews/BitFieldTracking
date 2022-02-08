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
    let indexRange = 200
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
    member _.SpanTracker () =
        let tracker = stackalloc<int64> 4
        
        for i = 0 to testIndexes.Length - 1 do
            let testIndex = testIndexes[i]
            let byteIndex = testIndex / 64
            let testValue = 1L <<< (testIndex % 64)
            if (tracker[byteIndex] &&& testValue) <> 0L then
                tracker[byteIndex] <- testValue &&& tracker[byteIndex]
            else
                tracker[byteIndex] <- testValue ||| tracker[byteIndex]

        tracker


    [<Benchmark>]
    member _.BitArrayTracker () =
        let mutable tracker = System.Collections.BitArray(indexRange)

        for i = 0 to testIndexes.Length - 1 do
            let testIndex = testIndexes[i]
            if tracker[testIndex] then
                // Real world we would do work here and then flip the case
                tracker[testIndex] <- false
            else
                tracker[testIndex] <- true

        tracker


type TupleTracker () =

    let testIndexCount = 100
    let indexRange = 50
    let rng = Random 123

    let rec getTuple (rng: Random) =
        let a = rng.Next (0, indexRange)
        let b = rng.Next (0, indexRange)
        if a <> b then
            a, b
        else
            getTuple rng


    let testIndexTuples =
        [| for _ = 1 to testIndexCount do
            getTuple rng
        |]
        |> Array.distinct

    let somePrimes = [ 769; 1543; 3079; 6151; 12289; 24593]

    let hashEntryCount = 1024
    let salt =
        [2 .. 100_000]
        |> List.find (fun saltCandidate ->
            let hashes = Collections.Generic.HashSet ()
            let mutable result = true

            for (a, b) in testIndexTuples do
                let key = (a <<< 16) ||| b
                let hash = (saltCandidate * key) % hashEntryCount
                if hashes.Contains hash then
                    result <- false
                else
                    hashes.Add hash |> ignore

            result
        )


    [<Benchmark>]
    member _.SetTracker () =
        let mutable tracker = Set.empty

        for i = 0 to testIndexTuples.Length - 1 do
            let testIndex = testIndexTuples[i]
            if tracker.Contains testIndex then
                // Real world we would do work here and then flip the case
                tracker <- tracker.Remove testIndex
            else
                tracker <- tracker.Add testIndex

        tracker

    [<Benchmark>]
    member _.HashSetTracker () =
        let mutable tracker = Collections.Generic.HashSet ()

        for i = 0 to testIndexTuples.Length - 1 do
            let testIndex = testIndexTuples[i]
            if tracker.Contains testIndex then
                // Real world we would do work here and then flip the case
                tracker.Remove testIndex |> ignore
            else
                tracker.Add testIndex |> ignore

        tracker

    [<Benchmark>]
    member _.BoolArrayTracker () =
        let tracker = Array.create hashEntryCount false

        for i = 0 to testIndexTuples.Length - 1 do
            let (a, b) = testIndexTuples[i]
            let key = (a <<< 8) &&& b
            let hash = (salt * key) % hashEntryCount
            if tracker[hash] then
                // Real world we would do work here and then flip the case
                tracker[hash] <- false
            else
                tracker[hash] <- true

        tracker


    [<Benchmark>]
    member _.SpanTracker () =
        let tracker = stackalloc<int64> 16
        
        for i = 0 to testIndexTuples.Length - 1 do
            let (a, b) = testIndexTuples[i]
            let key = (a <<< 8) &&& b
            let hash = (salt * key) % hashEntryCount
            let byteIndex = hash / 64
            let testValue = 1L <<< (hash % 64)

            if (tracker[byteIndex] &&& testValue) <> 0L then
                tracker[byteIndex] <- testValue &&& tracker[byteIndex]
            else
                tracker[byteIndex] <- testValue ||| tracker[byteIndex]

        tracker


[<EntryPoint>]
let main argv =
    
    let summary = BenchmarkRunner.Run<Benchmarks>()
    // let summary = BenchmarkRunner.Run<TupleTracker>()
    1