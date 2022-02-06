open System
open FSharp.NativeInterop
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open BenchmarkDotNet.Diagnosers

#nowarn "9" // Yes, I'm using pointers

[<Struct>]
type Int64Index =
    private {
        mutable Value : int64
    }
    static member Create () =
        { Value = 0L }

    member this.IsSet (position: int) =
        (this.Value &&& (1L <<< position)) <> 0 

    member this.Set (position: int) =
        this.Value <- (1L <<< position) ||| this.Value

    member this.UnSet (position: int) =
        this.Value <- ~~~ (1 <<< position) &&& this.Value


[<Struct>]
type DoubleIndex =
    private {
        mutable Lower : int64
        mutable Upper : int64
    }
    static member Create () =
        {
            Lower = 0L
            Upper = 0L
        }

    member this.IsSet (position: int) =
        if position < 64 then
            (this.Lower &&& (1 <<< position)) <> 0
        else
            (this.Upper &&& (1 <<< (position - 64))) <> 0

    member this.Set (position: int) =
        if position < 64 then
            this.Lower <- (1L <<< position) ||| this.Lower
        else
            this.Upper <- (1L <<< position) ||| this.Upper

    member this.UnSet (position: int) =
        if position < 64 then
            this.Lower <- ~~~ (1 <<< position) &&& this.Lower
        else
            this.Upper <- ~~~ (1 <<< position) &&& this.Upper


[<MemoryDiagnoser>]
type Benchmarks () =

    let testIndexCount = 1_000_000
    let indexRange = 128
    let rng = Random 123

    let testIndexes =
        [| for _ = 1 to testIndexCount do
            // Note: Next is exclusive on the upper bound
            rng.Next (0, indexRange) 
        |]


    [<Benchmark>]
    member _.BoolArrayIndex () =
        let boolArrayIndex = Array.create indexRange false

        for i = 0 to testIndexes.Length - 1 do
            let testIndex = testIndexes[i]
            if boolArrayIndex[testIndex] = false then
                // Real world we would do work here and then flip the case
                boolArrayIndex[testIndex] <- true
            else
                boolArrayIndex[testIndex] <- false

        boolArrayIndex


    // [<Benchmark>]
    // member _.Int64Index () =
    //     let mutable int64Index = Int64Index.Create ()
        
    //     for i = 0 to testIndexes.Length - 1 do
    //         let testIndex = testIndexes[i]
    //         if int64Index.IsSet testIndex then
    //             // Real world we would do work here and then flip the case
    //             int64Index.Set testIndex
    //         else
    //             int64Index.UnSet testIndex

    //     int64Index


    [<Benchmark>]
    member _.DoubleIndex () =
        let mutable doubleIndex = DoubleIndex.Create ()
        
        for i = 0 to testIndexes.Length - 1 do
            let testIndex = testIndexes[i]
            if doubleIndex.IsSet testIndex then
                // Real world we would do work here and then flip the case
                doubleIndex.Set testIndex
            else
                doubleIndex.UnSet testIndex

        doubleIndex


    [<Benchmark>]
    member _.SpanIndex () =
        let bitsPerInt64 = 64
        let requiredInt64s = (indexRange + bitsPerInt64 - 1) / bitsPerInt64
        let mem = NativePtr.stackalloc<int64> requiredInt64s
        let mem2 = mem |> NativePtr.toVoidPtr
        let spanIndex = Span<int64>(mem2, requiredInt64s)

        for i = 0 to testIndexes.Length - 1 do
            let testIndex = testIndexes[i]
            let subRegion = testIndex / bitsPerInt64
            let subIndex = testIndex % bitsPerInt64

            if (spanIndex[subRegion] &&& (1 <<< subIndex)) <> 0 then
                // Real world we would do work here and then flip the case
                spanIndex[subRegion] <- (1 <<< subIndex) ||| spanIndex[subRegion]
            else
                spanIndex[subRegion] <- ~~~ (1 <<< subIndex) &&& spanIndex[subRegion]

        spanIndex


[<EntryPoint>]
let main argv =
    
    let summary = BenchmarkRunner.Run<Benchmarks>()
    1