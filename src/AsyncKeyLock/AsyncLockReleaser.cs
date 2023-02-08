// Copyright (c) usercode
// https://github.com/usercode/AsyncLock
// MIT License

namespace AsyncKeyLock;

/// <summary>
/// AsyncLockReleaser
/// </summary>
public readonly struct AsyncLockReleaser : IDisposable
{
    /// <summary>
    /// AsyncLock
    /// </summary>
    private readonly AsyncLock AsyncLock;

    /// <summary>
    /// Type
    /// </summary>
    public readonly AsyncLockType Type;

    /// <summary>
    /// IsAcquiredImmediately
    /// </summary>
    public readonly bool IsAcquiredImmediately;

    internal AsyncLockReleaser(AsyncLock asyncLock, AsyncLockType type, bool lockAcquiredImmediately)
    {
        AsyncLock = asyncLock;
        Type = type;
        IsAcquiredImmediately = lockAcquiredImmediately;
    }

    public void Dispose()
    {
        AsyncLock.Release(this);
    }
}