// Copyright (c) usercode
// https://github.com/usercode/AsyncKeyLock
// MIT License

namespace AsyncKeyLock;

/// <summary>
/// Pool
/// </summary>
internal sealed class Pool<T>
{
    private readonly List<T> _pool = new();
    private readonly int MaxPoolSize;

    public Pool(int maxPoolSize = 64)
    {
        MaxPoolSize = maxPoolSize;
    }

    public bool Add(T obj)
    {
        if (_pool.Count < MaxPoolSize)
        {
            _pool.Add(obj);

            return true;
        }

        return false;
    }

    public bool GetOrCreate(out T result, Func<T> factory)
    {
        if (_pool.Count > 0)
        {
            int lastPos = _pool.Count - 1;

            result = _pool[lastPos];

            _pool.RemoveAt(lastPos);

            return true;
        }
        else
        {
            result = factory();

            return false;
        }
    }
}
