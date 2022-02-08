// open System
// open System.Runtime.CompilerServices
// open FSharp.NativeInterop


// // [<Struct>]
// // type Int64Tracker =
// //     private {
// //         mutable Value : int64
// //     }
// //     static member Init () =
// //         { Value = 0L }

// //     member this.IsSet (position: int) =
// //         let test = (1L <<< position)
// //         (this.Value &&& test) = test

// //     member this.Set (position: int) =
// //         this.Value <- (1L <<< position) ||| this.Value

// //     member this.UnSet (position: int) =
// //         this.Value <- ~~~ (1 <<< position) &&& this.Value


// // let testIndexCount = 1_000_000
// // let indexRange = 50
// // let rng = Random 123

// // let testIndexes =
// //     [| for _ = 1 to testIndexCount do
// //         // Note: Next is exclusive on the upper bound
// //         rng.Next (0, indexRange) 
// //     |]


// // let test () =
// //     let mutable tracker = Int64Tracker.Init ()
    
// //     for i = 0 to testIndexes.Length - 1 do
// //         let testIndex = testIndexes[i]
// //         if tracker.IsSet testIndex then
// //             // Real world we would do work here and then flip the case
// //             tracker.UnSet testIndex
// //         else
// //             tracker.Set testIndex

// //     tracker

// // open System

// // let testIndexCount = 100
// // let indexRange = 50
// // let rng = Random 123

// // let rec getTuple (rng: Random) =
// //     let a = rng.Next (0, indexRange)
// //     let b = rng.Next (0, indexRange)
// //     if a <> b then
// //         a, b
// //     else
// //         getTuple rng


// // let testIndexTuples =
// //     [| for _ = 1 to testIndexCount do
// //         getTuple rng
// //     |]
// //     |> Array.distinct

// // let salt1 = 1610612741
// // let salt2 = 805306457
// // let salt3 = 50331653

// // let hashEntryCount = 512
// // let testEntrys = Array.create hashEntryCount false

// // for (a, b) in testIndexTuples do
// //     let key = (a <<< 8) ||| b
// //     let hash1 = Math.Abs ((salt1 * key) % hashEntryCount)
// //     let hash2 = Math.Abs ((salt2 * key) % hashEntryCount)

// //     // if not testEntrys[hash1] then
// //     if (not testEntrys[hash1]) || (not testEntrys[hash2]) then
// //         testEntrys[hash1] <- true
// //         testEntrys[hash2] <- true
// //     else
// //         testEntrys[hash1] <- true
// //         testEntrys[hash2] <- true
// //         printfn "Dup keys"


// // let mutable salt = 2
// // let mutable foundResult = false

// // let result =
// //     {0 .. 1_000_000_000}
// //     |> Seq.find (fun saltCandidate ->
// //         let hashes = Collections.Generic.HashSet ()
// //         let mutable result = true

// //         for (a, b) in testIndexTuples do
// //             let key = (a <<< 16) ||| b
// //             let hash = Math.Abs ((saltCandidate * key) % hashEntryCount)
// //             if hashes.Contains hash then
// //                 result <- false
// //             else
// //                 hashes.Add hash |> ignore

// //         result
// //     )


// // while (not foundResult) || salt < 1_000_000_000 do

// //     let hashEntryCount = 512
// //     let testEntrys = Array.create hashEntryCount false

// //     for (a, b) in testIndexTuples do
// //         let key = (a <<< 8) ||| b
// //         let hash1 = Math.Abs ((salt1 * key) % hashEntryCount)
// //         let hash2 = Math.Abs ((salt2 * key) % hashEntryCount)

// //         // if not testEntrys[hash1] then
// //         if (not testEntrys[hash1]) || (not testEntrys[hash2]) then
// //             testEntrys[hash1] <- true
// //             testEntrys[hash2] <- true
// //         else
// //             testEntrys[hash1] <- true
// //             testEntrys[hash2] <- true
// //             printfn "Dup keys"




// // let somePrimes = [ 769; 1543; 3079; 6151; 12289; 24593]

// // let salt =
// //     somePrimes
// //     |> List.find (fun saltCandidate ->
// //         let hashes = Collections.Generic.HashSet ()
// //         let mutable result = true

// //         for (a, b) in testIndexTuples do
// //             let key = (a <<< 8) ||| b
// //             let hash = (saltCandidate * key) % hashEntryCount
// //             if hashes.Contains hash then
// //                 printfn "%A" hash
// //                 result <- false
// //             else
// //                 hashes.Add hash |> ignore

// //         result
// //     )






// open System

// let rng = Random 123
// let entryCount = 100
// let indexCount = 256
// let entries = [ for _ = 1 to entryCount do rng.Next (0, 4095)]


// let simpleHash x =
//     let salt = 50331653
//     Math.Abs (x * salt)

// entries
// |> List.map simpleHash
// |> List.map (fun x -> x % indexCount)
// |> List.groupBy id
// |> List.where (fun (_, grp) -> grp.Length > 2)

// let findHash [a; b; c; d] =
//     let xSalt = 50331653
//     let otherSalt = 0x68E31DA4
//     [0..255]
//     |> List.find (fun salt ->
//         let aHash = ((a * (salt + otherSalt)) ^^^ xSalt) % 4
//         let bHash = ((b * (salt + otherSalt)) ^^^ xSalt) % 4
//         let cHash = ((c * (salt + otherSalt)) ^^^ xSalt) % 4
//         let dHash = ((d * (salt + otherSalt)) ^^^ xSalt) % 4
//         let set = Set [aHash; bHash; cHash; dHash]
//         set.Count = 4 // Check that they are all unique
//     )

// let secondEntry = 
//     [for _ in 1..4 do rng.Next(0, 4095)]
//     |> findHash


// let saltyBaby x =
//     let bitNoise1 = 0xB5297A4D
//     let salt = 50331653
//     let x = (x * salt) ^^^ bitNoise1
//     Math.Abs x

// entries
// |> List.map saltyBaby
// |> List.map (fun x -> x % indexCount)
// |> List.groupBy id
// |> List.where (fun (_, grp) -> grp.Length > 1)

// let squirrel13 x =
//     let bitNoise1 = 0xB5297A4D
//     let bitNoise2 = 0x68E31DA4
//     let bitNoise3 = 0x1B56C4E9
//     let x = x * bitNoise1
//     let x = (x >>> 8) ^^^ x
//     let x = bitNoise2 + x
//     let x = (x <<< 8) ^^^ x
//     let x = x * bitNoise3
//     (x >>> 8) ^^^ x

// entries
// |> List.map squirrel13
// |> List.map (fun x -> x % indexCount)
// |> List.groupBy id
// |> List.where (fun (_, grp) -> grp.Length > 1)


let sampleCount = 1_000_000
let population = 128
// 8 bits/byte, 64 byte/Cache Line
let indexCount = 64
let maxCollisionCount = 8
let rng = System.Random 123

let samples =
    seq {
        for _ in 1 .. sampleCount ->
        let x =
            seq { 
                for _ = 1 to population do 
                rng.Next (0, indexCount) }
            |> Seq.groupBy id
            |> Seq.exists (fun (_, grp) -> Seq.length grp > maxCollisionCount)

        if x then
            1.0
        else
            0.0
    }

Seq.average samples


let sampleCount = 1_000_000
let population = 8
// 8 bits/byte, 64 byte/Cache Line
let indexCount = 128
let maxCollisionCount = 1
let rng = System.Random 123

let samples =
    seq {
        for _ in 1 .. sampleCount ->
        let x =
            seq { 
                for _ = 1 to population do 
                rng.Next (0, indexCount) }
            |> Seq.groupBy id
            |> Seq.exists (fun (_, grp) -> Seq.length grp > maxCollisionCount)

        if x then
            1.0
        else
            0.0
    }

Seq.average samples
