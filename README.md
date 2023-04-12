# Async Key-based Reader Writer Lock with Cancellation Support

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](https://opensource.org/licenses/MIT)
[![NuGet](https://img.shields.io/nuget/v/AsyncKeyLock.svg?style=flat-square)](https://www.nuget.org/packages/AsyncKeyLock)

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

//use writer lock scope to interrupt long running reader lock
using (ReaderReleaser r1 = await asyncLock.ReaderLockAsync())
{
   //use reader lock

   if (..)
   {
      r1.UseWriterLockAsync(async () => { /*use writer lock here*/ });
   }

   //continue with reader lock
}
```
