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

|       Method | NumberOfLocks |          Mean |       Error |      StdDev |  Ratio | RatioSD |      Gen0 |  Allocated | Alloc Ratio |
|------------- |-------------- |--------------:|------------:|------------:|-------:|--------:|----------:|-----------:|------------:|
| AsyncKeyLock |           100 |      4.666 μs |   0.0145 μs |   0.0121 μs |   1.00 |    0.00 |    2.2049 |   10.16 KB |        1.00 |
|     NeoSmart |           100 |    891.568 μs |   6.0299 μs |   5.6404 μs | 190.93 |    1.27 |   20.5078 |   95.29 KB |        9.38 |
|         Nito |           100 |     11.715 μs |   0.0700 μs |   0.0654 μs |   2.51 |    0.02 |    6.7902 |   31.25 KB |        3.08 |
|              |               |               |             |             |        |         |           |            |             |
| AsyncKeyLock |          1000 |     46.040 μs |   0.2566 μs |   0.2401 μs |   1.00 |    0.00 |   22.0947 |  101.56 KB |        1.00 |
|     NeoSmart |          1000 |  8,698.138 μs |  32.4090 μs |  30.3154 μs | 188.93 |    1.24 |  203.1250 |  957.37 KB |        9.43 |
|         Nito |          1000 |    116.451 μs |   0.8251 μs |   0.7718 μs |   2.53 |    0.02 |   67.9932 |   312.5 KB |        3.08 |
|              |               |               |             |             |        |         |           |            |             |
| AsyncKeyLock |         10000 |    459.050 μs |   1.0992 μs |   0.9179 μs |   1.00 |    0.00 |  220.7031 | 1015.63 KB |        1.00 |
|     NeoSmart |         10000 | 86,099.198 μs | 311.2871 μs | 291.1781 μs | 187.57 |    0.65 | 2000.0000 | 9583.64 KB |        9.44 |
|         Nito |         10000 |  1,143.306 μs |   8.2156 μs |   7.6848 μs |   2.49 |    0.02 |  679.6875 |    3125 KB |        3.08 |
