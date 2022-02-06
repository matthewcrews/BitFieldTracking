# Tracking Observation with Bits

This is a test of different ways to track whether an item has been observed or not. The original impetus came from writing a graph algorithm that required monitoring whether a node or arc had been visited or not. The naive approach was to use an `array<bool>`. I wanted to see if there was a way using just raw bits though since a smaller algorithm would likely perform better.

My current results on my machine are the following.

```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT DEBUG
  DefaultJob : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT


|           Method |       Mean |     Error |    StdDev |     Median |     Gen 0 |    Allocated |
|----------------- |-----------:|----------:|----------:|-----------:|----------:|-------------:|
|       SetTracker | 8,758.0 us | 170.92 us | 222.24 us | 8,788.5 us | 4781.2500 | 40,000,008 B |
|   HashSetTracker | 5,361.9 us | 105.70 us | 154.93 us | 5,314.5 us |         - |         68 B |
| BoolArrayTracker | 4,979.5 us |  92.54 us |  86.56 us | 4,994.3 us |         - |         84 B |
|     Int64Tracker |   728.8 us |  14.51 us |  17.82 us |   731.7 us |         - |            - |
| SpanInt64Tracker | 3,052.4 us |  60.16 us |  95.42 us | 3,100.9 us |         - |          2 B |
```

This test run was for tracking up to 64 different items. If you need to track more than 64 items, I recommend the `SpanIndex` type since it is able to handle an arbitrary number of items and performs better than the naive `array<bool>` method.

I welcome all suggestions and Pull requests if you have suggestions for how to improve the performance.
