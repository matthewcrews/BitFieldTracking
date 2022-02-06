# Tracking Observation with Bits

This is a test of different ways to track whether an item has been observed or not. The original impetus came from writing a graph algorithm that required monitoring whether a node or arc had been visited or not. The naive approach was to use an `array<bool>`. I wanted to see if there was a way using just raw bits though since a smaller algorithm would likely perform better.

The use cases is limited to tracking less than 64 items which is why the `Int64Tracker` type is viable.

My current results on my machine are the following.

```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT DEBUG
  DefaultJob : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT


|           Method |       Mean |     Error |    StdDev |      Gen 0 |     Allocated |
|----------------- |-----------:|----------:|----------:|-----------:|--------------:|
|       SetTracker | 130.889 ms | 1.7642 ms | 1.4732 ms | 27000.0000 | 226,813,932 B |
|   HashSetTracker |  14.368 ms | 0.0613 ms | 0.0544 ms |          - |       2,800 B |
| BoolArrayTracker |   5.017 ms | 0.0447 ms | 0.0418 ms |          - |          84 B |
|     Int64Tracker |   3.979 ms | 0.0097 ms | 0.0081 ms |          - |           4 B |
```

I welcome all suggestions and Pull requests if you have suggestions for how to improve the performance.
