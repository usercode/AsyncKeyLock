# AsyncLock (Async Key-based ReaderWriterLock with cancellation support)

## Sample

```csharp
AsyncLock<string> asyncLock = new AsyncLock<string>();

//acquire reader lock
var d1 = await asyncLock.ReaderLockAsync("123");

//release reader lock
d1.Dispose();

//set timeout
CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(10));

//acquire writer lock
using var d2 = await asyncLock.WriterLockAsync("123", cancellationToken.Token);

```