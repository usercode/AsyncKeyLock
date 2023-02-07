# AsyncKeyLock

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](https://opensource.org/licenses/MIT)
[![NuGet](https://img.shields.io/nuget/v/AsyncKeyLock.svg?style=flat-square)](https://www.nuget.org/packages/AsyncKeyLock)

## Async Key-based Reader Writer Lock with Cancellation Support

- Can create reader and writer locks (with key).
- A reader lock allows concurrent access for read-only operations.
- A writer lock allows exclusive access for operations.
- It based internally on TaskCompletionSources which will be queued.

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

![grafik](https://user-images.githubusercontent.com/2958488/217373765-78bd3d41-eb95-4f20-b756-12eaedceaeb5.png)

#### Create and release key locks

![grafik](https://user-images.githubusercontent.com/2958488/217371833-0b576ddb-a8ba-441b-9d65-3399eee1940b.png)
