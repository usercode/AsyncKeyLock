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
        MaxPoolSize = maxPoolSize;
    }

    private readonly IDictionary<TKey, AsyncLock> _locks = new Dictionary<TKey, AsyncLock>();
    private readonly IList<AsyncLock> _pool = new List<AsyncLock>();
    private readonly int MaxPoolSize;

    /// <summary>
    /// GetAsyncLock
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private AsyncLock GetAsyncLock(TKey key)
    {
        if (_locks.TryGetValue(key, out AsyncLock? asyncLock) == false)
        {
            //is idle AsyncLock available
            if (_pool.Count > 0)
            {
                int lastPos = _pool.Count - 1;

                asyncLock = _pool[lastPos];

                _pool.RemoveAt(lastPos);
            }
            else
            {
                //create new AsyncLock
                asyncLock = new AsyncLock(_locks);
                asyncLock.Released += x =>
                {
                    //is AsyncLock idle
                    if (x.State == AsyncLockState.Idle)
                    {
                        _locks.Remove(key);

                        //add idle AsynLock to pool
                        if (_pool.Count < MaxPoolSize)
                        {
                            _pool.Add(x);
                        }
                    }
                };
            }

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
