// Copyright (c) usercode
// https://github.com/usercode/AsyncLock
// MIT License

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

    private readonly IDictionary<TKey, AsyncLock> _locks = new Dictionary<TKey, AsyncLock>();
    private readonly Pool<AsyncLock> _pool;

    /// <summary>
    /// GetAsyncLock
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private AsyncLock GetAsyncLock(TKey key)
    {
        if (_locks.TryGetValue(key, out AsyncLock? asyncLock) == false)
        {
            //create new AsyncLock
            _pool.GetOrCreate(out asyncLock,
                () =>
                {
                    AsyncLock a = new AsyncLock(_locks);
                    a.Released += x =>
                    {
                        _locks.Remove(key);

                        //add idle AsynLock to pool
                        _pool.Add(x);
                    };

                    return a;
                });

            _locks.Add(key, asyncLock);
        }

        return asyncLock;
    }

    /// <summary>
    /// ReaderLockAsync
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Task<AsyncLockReleaser> ReaderLockAsync(TKey key, CancellationToken cancellation = default)
    {
        lock (_locks)
        {
            return GetAsyncLock(key).ReaderLockAsync(cancellation);
        }
    }

    /// <summary>
    /// WriterLockAsync
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Task<AsyncLockReleaser> WriterLockAsync(TKey key, CancellationToken cancellation = default)
    {
        lock (_locks)
        {
            return GetAsyncLock(key).WriterLockAsync(cancellation);
        }
    }
}
