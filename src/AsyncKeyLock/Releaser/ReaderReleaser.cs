// Copyright (c) usercode
// https://github.com/usercode/AsyncKeyLock
// MIT License

namespace AsyncKeyLock;

/// <summary>
/// ReaderReleaser
/// </summary>
public readonly struct ReaderReleaser : IDisposable
{
    /// <summary>
    /// AsyncLock
    /// </summary>
    private readonly AsyncLock AsyncLock;

    /// <summary>
    /// IsAcquiredImmediately
    /// </summary>
    public readonly bool IsAcquiredImmediately;

    internal ReaderReleaser(AsyncLock asyncLock, bool lockAcquiredImmediately)
    {
        AsyncLock = asyncLock;
        IsAcquiredImmediately = lockAcquiredImmediately;
    }

    public async Task UseWriterLockAsync(Func<Task> func, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();

        //release reader lock
        AsyncLock.Release(AsyncLockType.Read);

        try
        {
            //create new writer lock
            using (await AsyncLock.WriterLockAsync(cancellation))
            {
                await func();
            }
        }
        finally
        {
            //restore reader lock
            await AsyncLock.ReaderLockAsync();
        }
    }

    public void Dispose()
    {
        AsyncLock.Release(AsyncLockType.Read);
    }
}