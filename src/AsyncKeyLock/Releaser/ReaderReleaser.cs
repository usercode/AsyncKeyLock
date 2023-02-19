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

        Task<WriterReleaser> taskWriter;
        WriterReleaser? writerReleaser = null;

        lock (AsyncLock.SyncObj)
        {
            //release reader lock
            AsyncLock.Release(AsyncLockType.Read, false);

            taskWriter = AsyncLock.WriterLockAsync(cancellation);
        }

        try
        {
            //create new writer lock
            writerReleaser = await taskWriter;
            
            await func();
            
        }
        finally
        {
            Task<ReaderReleaser> taskReader;

            lock (AsyncLock.SyncObj)
            {
                if (writerReleaser != null)
                {
                    AsyncLock.Release(AsyncLockType.Write, false);
                }

                //restore reader lock
                taskReader = AsyncLock.ReaderLockAsync();
            }

            await taskReader;
        }
    }

    public void Dispose()
    {
        AsyncLock.Release(AsyncLockType.Read);
    }
}