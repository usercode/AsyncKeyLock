// Copyright (c) usercode
// https://github.com/usercode/AsyncKeyLock
// MIT License

namespace AsyncKeyLock;

/// <summary>
/// AsyncLock
/// </summary>
public sealed class AsyncLock
{
    public AsyncLock()
    {
        SyncObj = _waitingWriters;

        _readerReleaserTask = Task.FromResult(new ReaderReleaser(this, true));
        _writerReleaserTask = Task.FromResult(new WriterReleaser(this, true));
    }

    internal AsyncLock(object syncObject)
        : this()
    {
        SyncObj = syncObject;
    }

    internal readonly object SyncObj;

    private readonly Queue<TaskCompletionSource<ReaderReleaser>> _waitingReaders = new();
    private readonly Queue<TaskCompletionSource<WriterReleaser>> _waitingWriters = new();

    private readonly Task<ReaderReleaser> _readerReleaserTask;
    private readonly Task<WriterReleaser> _writerReleaserTask;
    
    private int _readersRunning;

    private bool _isWriterRunning = false;

    public event Action<AsyncLock>? Released;

    /// <summary>
    /// CountRunningReaders
    /// </summary>
    internal int CountRunningReaders => _readersRunning;

    /// <summary>
    /// CountWaitingReaders
    /// </summary>
    internal int CountWaitingReaders => _waitingReaders.Count;

    /// <summary>
    /// IsWriterRunning
    /// </summary>
    internal bool IsWriterRunning => _isWriterRunning;

    /// <summary>
    /// CountWaitingWriters
    /// </summary>
    internal int CountWaitingWriters => _waitingWriters.Count;

    /// <summary>
    /// State
    /// </summary>
    public AsyncLockState State
    {
        get
        {
            if (IsWriterRunning)
            {
                return AsyncLockState.Writer;
            }
            else if (CountRunningReaders > 0)
            {
                return AsyncLockState.Reader;
            }
            else
            {
                return AsyncLockState.Idle;
            }
        }
    }

    public Task<ReaderReleaser> ReaderLockAsync(CancellationToken cancellation = default)
    {
        if (cancellation.IsCancellationRequested)
        {
            return Task.FromCanceled<ReaderReleaser>(cancellation);
        }

        lock (SyncObj)
        {
            //no running or waiting write lock?
            if (_isWriterRunning == false && _waitingWriters.Count == 0)
            {
                _readersRunning++;

                return _readerReleaserTask;
            }
            else
            {
                //create waiting reader
                return _waitingReaders.Enqueue(cancellation);
            }
        }
    }

    public Task<WriterReleaser> WriterLockAsync(CancellationToken cancellation = default)
    {
        if (cancellation.IsCancellationRequested)
        {
            return Task.FromCanceled<WriterReleaser>(cancellation);
        }

        lock (SyncObj)
        {
            if (_isWriterRunning == false && _readersRunning == 0)
            {
                _isWriterRunning = true;

                return _writerReleaserTask;
            }
            else
            {
                //create waiting writer
                return _waitingWriters.Enqueue(cancellation);
            }
        }
    }

    internal void Release(AsyncLockType type, bool sendReleaseEvent = true)
    {
        lock (SyncObj)
        {
            try
            {
                if (type == AsyncLockType.Write)
                {
                    WriterRelease();
                }
                else
                {
                    ReaderRelease();
                }
            }
            finally
            {
                if (sendReleaseEvent && State == AsyncLockState.Idle)
                {
                    Released?.Invoke(this);
                }
            }
        }
    }

    private void ReaderRelease()
    {
        _readersRunning--;

        //start next waiting writer lock
        if (_readersRunning == 0)
        {
            StartNextWaitingWriter();
        }
    }

    private void WriterRelease()
    {
        //start next writer lock?
        StartNextWaitingWriter();

        //no running writer lock?
        if (_isWriterRunning == false)
        {
            while (_waitingReaders.TryDequeue(out var taskSource))
            {
                bool result = taskSource.TrySetResult(new ReaderReleaser(this, false));

                if (result)
                {
                    _readersRunning++;
                }
            }
        }
    }

    private void StartNextWaitingWriter()
    {
        while (_waitingWriters.TryDequeue(out var taskSource))
        {
            bool result = taskSource.TrySetResult(new WriterReleaser(this, false));

            if (result == true)
            {
                _isWriterRunning = true;

                return;
            }
        }

        _isWriterRunning = false;

        return;
    }
}
