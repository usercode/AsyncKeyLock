// Copyright (c) usercode
// https://github.com/usercode/AsyncLock
// MIT License

namespace AsyncKeyLock;

/// <summary>
/// AsyncLockReleaser
/// </summary>
public sealed class AsyncLockReleaser : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// AsyncLock
    /// </summary>
    private AsyncLock AsyncLock { get; }

    /// <summary>
    /// Type
    /// </summary>
    public AsyncLockType Type { get; }

    /// <summary>
    /// IsAcquiredImmediately
    /// </summary>
    public bool IsAcquiredImmediately { get; }

    internal AsyncLockReleaser(AsyncLock asyncLock, AsyncLockType type, bool lockAcquiredImmediately)
    {
        AsyncLock = asyncLock;
        Type = type;
        IsAcquiredImmediately = lockAcquiredImmediately;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        AsyncLock.Release(this);
        
        _disposed = true;

        GC.SuppressFinalize(this);
    }
}
