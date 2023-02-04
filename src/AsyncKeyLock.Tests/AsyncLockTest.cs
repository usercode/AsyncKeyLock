// Copyright (c) usercode
// https://github.com/usercode/ImageWizard
// MIT License

using System.Diagnostics;
using Xunit;

namespace AsyncKeyLock.Tests;

public class AsyncLockTest
{
    [Fact]
    public async Task ThreeParallelReaders()
    {
        AsyncLock lockEntity = new AsyncLock();

        using var r1 = await lockEntity.ReaderLockAsync();
        using var r2 = await lockEntity.ReaderLockAsync();
        using var r3 = await lockEntity.ReaderLockAsync();

        Assert.Equal(3, lockEntity.CountRunningReaders);

        Assert.True(r1.IsAcquiredImmediately);
        Assert.True(r2.IsAcquiredImmediately);
        Assert.True(r3.IsAcquiredImmediately);
    }

    [Fact]
    public async Task ThreeParallelReadersClosed()
    {
        AsyncLock lockEntity = new AsyncLock();

        {
            using var r1 = await lockEntity.ReaderLockAsync();
            using var r2 = await lockEntity.ReaderLockAsync();
            using var r3 = await lockEntity.ReaderLockAsync();
        }

        Assert.Equal(0, lockEntity.CountRunningReaders);
    }

    [Fact]
    public async Task OneOpenWriter()
    {
        AsyncLock lockEntity = new AsyncLock();

        using var r1 = await lockEntity.WriterLockAsync();
        
        Assert.Equal(0, lockEntity.CountRunningReaders);
        Assert.True(lockEntity.IsWriterRunning);
    }

    [Fact]
    public async Task OneClosedWriter()
    {
        AsyncLock lockEntity = new AsyncLock();

        {
            using var r1 = await lockEntity.WriterLockAsync();
        }

        Assert.Equal(0, lockEntity.CountRunningReaders);
        Assert.False(lockEntity.IsWriterRunning);
    }

    [Fact]
    public async Task OneClosedReaderOneClosedWriter()
    {
        AsyncLock lockEntity = new AsyncLock();

        {
            using var r1 = await lockEntity.ReaderLockAsync();
        }

        {
            using var r2 = await lockEntity.WriterLockAsync();
        }

        Assert.Equal(0, lockEntity.CountRunningReaders);
        Assert.False(lockEntity.IsWriterRunning);
    }

    [Fact]
    public async Task OneClosedWriterOneOpenReader()
    {
        AsyncLock lockEntity = new AsyncLock();

        {
            using var r1 = await lockEntity.WriterLockAsync();
        }

        {
            using var r2 = await lockEntity.ReaderLockAsync();
        }

        Assert.Equal(0, lockEntity.CountRunningReaders);
        Assert.False(lockEntity.IsWriterRunning);
    }

    [Fact]
    public async Task BlockedReader()
    {
        AsyncLock lockEntity = new AsyncLock();

        using var r1 = await lockEntity.WriterLockAsync();

        await Assert.ThrowsAsync<TimeoutException>(() => lockEntity.ReaderLockAsync().WaitAsync(TimeSpan.FromSeconds(1)));
    }

    [Fact]
    public async Task BlockedWriter()
    {
        AsyncLock lockEntity = new AsyncLock();

        using var r1 = await lockEntity.ReaderLockAsync();

        await Assert.ThrowsAsync<TimeoutException>(() => lockEntity.WriterLockAsync().WaitAsync(TimeSpan.FromSeconds(1)));
    }

    [Fact]
    public async Task BlockedWriterThreeReaders()
    {
        AsyncLock lockEntity = new AsyncLock();
        
        using var w1 = await lockEntity.WriterLockAsync();

        using var r1 = lockEntity.ReaderLockAsync();
        using var r2 = lockEntity.ReaderLockAsync();
        using var r3 = lockEntity.ReaderLockAsync();

        Assert.Equal(0, lockEntity.CountRunningReaders);

        //release writer lock, starts reader locks
        w1.Dispose();

        Assert.Equal(3, lockEntity.CountRunningReaders);
    }

    [Fact]
    public async Task CancelWriterLock()
    {
        AsyncLock lockEntity = new AsyncLock();

        using CancellationTokenSource source = new CancellationTokenSource(TimeSpan.FromSeconds(1));

        using var w1 = await lockEntity.WriterLockAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            var w2 = await lockEntity.WriterLockAsync(source.Token);
        });
    }

    [Fact]
    public async Task MultipleReadThenWriteAsync()
    {
        AsyncLock asyncLock = new AsyncLock();

        async Task enterReadThenWrite()
        {
            Debug.WriteLine("Enter");

            using (await asyncLock.ReaderLockAsync())
            {
                Debug.WriteLine("Enter Reader");
            }
            using (await asyncLock.WriterLockAsync())
            {
                Debug.WriteLine("Enter Writer");
            }

            Debug.WriteLine("Leave");
        }

        await Task.WhenAll(Enumerable.Range(0, 100).Select(x => Task.Run(enterReadThenWrite)));
    }
}
