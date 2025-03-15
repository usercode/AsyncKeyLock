// Copyright (c) usercode
// https://github.com/usercode/AsyncKeyLock
// MIT License

using System.Runtime.InteropServices;

namespace AsyncKeyLock;

/// <summary>
/// AsyncLock
/// </summary>
public sealed class AsyncLock<TKey>
    where TKey : notnull
{
    public AsyncLock(int maxPoolSize = 64)
    {
        _pool = new Pool<AsyncLock>(maxPoolSize);
    }

    private readonly Dictionary<TKey, AsyncLock> _activeLocks = new();
    private readonly Pool<AsyncLock> _pool;
    private readonly Lock _lock = new Lock();

    /// <summary>
    /// GetAsyncLock
    /// </summary>
    private AsyncLock GetAsyncLock(TKey key)
    {
        ref AsyncLock? dictionaryValue = ref CollectionsMarshal.GetValueRefOrAddDefault(_activeLocks, key, out bool exists);

        if (exists)
        {
            return dictionaryValue!;
        }

        _pool.GetOrCreate(out dictionaryValue,
            () =>
            {
                AsyncLock a = new AsyncLock(_lock);
                a.Released += x =>
                {
                    _activeLocks.Remove(key);

                    //add idle AsynLock to pool
                    _pool.Add(x);
                };

                return a;
            });

        return dictionaryValue;
    }

    /// <summary>
    /// ReaderLockAsync
    /// </summary>
    public Task<ReaderReleaser> ReaderLockAsync(TKey key, CancellationToken cancellation = default)
    {
        lock (_lock)
        {
            return GetAsyncLock(key).ReaderLockAsync(cancellation);
        }
    }

    /// <summary>
    /// WriterLockAsync
    /// </summary>
    public Task<WriterReleaser> WriterLockAsync(TKey key, CancellationToken cancellation = default)
    {
        lock (_lock)
        {
            return GetAsyncLock(key).WriterLockAsync(cancellation);
        }
    }
}
