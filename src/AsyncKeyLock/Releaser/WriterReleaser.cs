// Copyright (c) usercode
// https://github.com/usercode/AsyncKeyLock
// MIT License

namespace AsyncKeyLock;

/// <summary>
/// WriterReleaser
/// </summary>
public readonly struct WriterReleaser : IDisposable
{
    /// <summary>
    /// AsyncLock
    /// </summary>
    private readonly AsyncLock AsyncLock;

    /// <summary>
    /// IsAcquiredImmediately
    /// </summary>
    public readonly bool IsAcquiredImmediately;

    internal WriterReleaser(AsyncLock asyncLock, bool lockAcquiredImmediately)
    {
        AsyncLock = asyncLock;
        IsAcquiredImmediately = lockAcquiredImmediately;
    }    

    public void Dispose()
    {
        AsyncLock.Release(AsyncLockType.Write);
    }
}