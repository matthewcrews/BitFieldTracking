# Tracking Observation with Bits

This is a test of different ways to track whether an item has been observed or not. The original impetus came from writing a graph algorithm that required monitoring whether a node or arc had been visited or not. The naive approach was to use an `array<bool>`. I wanted to see if there was a way using just raw bits though since a smaller algorithm would likely perform better.

My current results on my machine are the following.

```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT DEBUG
  DefaultJob : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT


|         Method |       Mean |    Error |   StdDev | Allocated |
|--------------- |-----------:|---------:|---------:|----------:|
| BoolArrayIndex | 5,048.6 us | 43.28 us | 33.79 us |      92 B |
|     Int64Index |   733.3 us |  9.30 us |  8.70 us |         - |
|    DoubleIndex | 1,218.7 us | 12.45 us | 11.65 us |       1 B |
|      SpanIndex | 3,181.3 us | 31.19 us | 29.18 us |       2 B |
```

This test run was for tracking up to 64 different items. If you need to track more than 64 items, I recommend the `SpanIndex` type since it is able to handle an arbitrary number of items and performs better than the naive `array<bool>` method.

I welcome all suggestions and Pull requests if you have suggestions for how to improve the performance.
