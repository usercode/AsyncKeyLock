// Copyright (c) usercode
// https://github.com/usercode/AsyncLock
// MIT License

namespace AsyncLock;

/// <summary>
/// AsyncLockReleaser
/// </summary>
public class AsyncLockReleaser : IDisposable
{
    private readonly AsyncLock _asyncLock;
    private readonly AsyncLockType _type;
    private readonly bool _lockAcquiredImmediately;

    internal AsyncLockReleaser(AsyncLock asyncLock, AsyncLockType type, bool lockAcquiredImmediately)
    {
        _asyncLock = asyncLock;
        _type = type;
        _lockAcquiredImmediately = lockAcquiredImmediately;
    }

    /// <summary>
    /// Type
    /// </summary>
    public AsyncLockType Type => _type;

    /// <summary>
    /// IsAcquiredImmediately
    /// </summary>
    public bool IsAcquiredImmediately => _lockAcquiredImmediately;

    private bool _disposed;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _asyncLock.Release(_type);
        
        _disposed = true;

        GC.SuppressFinalize(this);
    }
}
