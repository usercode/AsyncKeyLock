# AsyncKeyLock

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](https://opensource.org/licenses/MIT)
[![NuGet](https://img.shields.io/nuget/v/AsyncKeyLock.Core.svg?style=flat-square)](https://www.nuget.org/packages/AsyncKeyLock/)

## Async Key-based Reader Writer Lock with Cancellation Support

## How to use it

```csharp
//async lock
AsyncLock asyncLock = new AsyncLock();

//async lock with key
AsyncLock<string> asyncLock = new AsyncLock<string>();

//acquire reader lock
var d1 = await asyncLock.ReaderLockAsync("123");

//release reader lock
d1.Dispose();

//set timeout
using CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

//acquire writer lock
using var d2 = await asyncLock.WriterLockAsync("123", cts.Token);

```

### Benchmarks

#### Create und release locks

|       Method | NumberOfLocks |          Mean |       Error |      StdDev |  Ratio | RatioSD |      Gen0 |  Allocated | Alloc Ratio |
|------------- |-------------- |--------------:|------------:|------------:|-------:|--------:|----------:|-----------:|------------:|
| AsyncKeyLock |           100 |      4.703 μs |   0.0379 μs |   0.0355 μs |   1.00 |    0.00 |    2.2049 |   10.16 KB |        1.00 |
|         Nito |           100 |     11.726 μs |   0.2079 μs |   0.2629 μs |   2.52 |    0.06 |    6.7902 |   31.25 KB |        3.08 |
|     NeoSmart |           100 |    881.746 μs |   3.3851 μs |   3.1664 μs | 187.48 |    1.57 |   20.5078 |   95.39 KB |        9.39 |
|              |               |               |             |             |        |         |           |            |             |
| AsyncKeyLock |          1000 |     46.063 μs |   0.4878 μs |   0.4324 μs |   1.00 |    0.00 |   22.0947 |  101.56 KB |        1.00 |
|         Nito |          1000 |    114.609 μs |   0.7213 μs |   0.6747 μs |   2.49 |    0.03 |   67.9932 |   312.5 KB |        3.08 |
|     NeoSmart |          1000 |  8,780.231 μs |  54.3188 μs |  48.1522 μs | 190.63 |    1.80 |  203.1250 |  957.01 KB |        9.42 |
|              |               |               |             |             |        |         |           |            |             |
| AsyncKeyLock |         10000 |    465.074 μs |   2.9952 μs |   2.8017 μs |   1.00 |    0.00 |  220.7031 | 1015.63 KB |        1.00 |
|         Nito |         10000 |  1,212.091 μs |  23.2923 μs |  31.0945 μs |   2.62 |    0.07 |  679.6875 |    3125 KB |        3.08 |
|     NeoSmart |         10000 | 86,948.942 μs | 289.1808 μs | 241.4791 μs | 187.08 |    1.14 | 2000.0000 | 9581.38 KB |        9.43 |

### Create and release key locks

|         Method | NumberOfLocks |        Mean |     Error |    StdDev | Ratio | RatioSD |     Gen0 |  Allocated | Alloc Ratio |
|--------------- |-------------- |------------:|----------:|----------:|------:|--------:|---------:|-----------:|------------:|
|   AsyncKeyLock |           100 |    11.99 μs |  0.087 μs |  0.077 μs |  1.00 |    0.00 |   2.8839 |   13.28 KB |        1.00 |
| AsyncKeyedLock |           100 |    18.72 μs |  0.114 μs |  0.107 μs |  1.56 |    0.01 |   3.9063 |   17.97 KB |        1.35 |
|  ImageSharpWeb |           100 |    29.43 μs |  0.177 μs |  0.166 μs |  2.45 |    0.02 |   3.7231 |   17.19 KB |        1.29 |
|                |               |             |           |           |       |         |          |            |             |
|   AsyncKeyLock |          1000 |   120.84 μs |  0.524 μs |  0.491 μs |  1.00 |    0.00 |  28.8086 |  132.81 KB |        1.00 |
| AsyncKeyedLock |          1000 |   190.34 μs |  1.461 μs |  1.367 μs |  1.58 |    0.01 |  39.0625 |  179.69 KB |        1.35 |
|  ImageSharpWeb |          1000 |   293.56 μs |  1.807 μs |  1.690 μs |  2.43 |    0.02 |  37.1094 |  171.88 KB |        1.29 |
|                |               |             |           |           |       |         |          |            |             |
|   AsyncKeyLock |         10000 | 1,201.14 μs |  9.040 μs |  8.456 μs |  1.00 |    0.00 | 289.0625 | 1328.13 KB |        1.00 |
| AsyncKeyedLock |         10000 | 1,856.96 μs |  9.053 μs |  8.468 μs |  1.55 |    0.01 | 390.6250 | 1796.88 KB |        1.35 |
|  ImageSharpWeb |         10000 | 2,973.14 μs | 17.736 μs | 16.590 μs |  2.48 |    0.02 | 371.0938 | 1718.75 KB |        1.29 |
