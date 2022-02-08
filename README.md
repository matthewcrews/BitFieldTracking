# Tracking Observation with Bits

This is a test of different ways to track whether an item has been observed or not. The original impetus came from writing a graph algorithm that required monitoring whether a node or arc had been visited or not. The naive approach was to use an `array<bool>`. I wanted to see if there was a way using just raw bits though since a smaller algorithm would likely perform better.

The use cases is limited to tracking less than 64 items which is why the `Int64Tracker` type is viable.

My current results on my machine are the following.

```
// * Summary *

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT DEBUG
  DefaultJob : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT


|           Method |       Mean |     Error |    StdDev |      Gen 0 |     Allocated |
|----------------- |-----------:|----------:|----------:|-----------:|--------------:|
|       SetTracker | 133.889 ms | 2.6145 ms | 2.5678 ms | 27000.0000 | 226,815,978 B |
|   HashSetTracker |  14.107 ms | 0.0586 ms | 0.0520 ms |          - |       2,800 B |
| BoolArrayTracker |   4.938 ms | 0.0294 ms | 0.0260 ms |          - |          84 B |
|  BitArrayTracker |   5.102 ms | 0.0545 ms | 0.0510 ms |          - |          68 B |
|      SpanTracker |   4.140 ms | 0.0408 ms | 0.0382 ms |          - |           4 B |
|     Int64Tracker |   4.073 ms | 0.0777 ms | 0.0895 ms |          - |           4 B |
```

I welcome all suggestions and Pull requests if you have suggestions for how to improve the performance.
