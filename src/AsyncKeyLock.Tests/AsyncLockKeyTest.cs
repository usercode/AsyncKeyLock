﻿// Copyright (c) usercode
// https://github.com/usercode/ImageWizard
// MIT License

using Xunit;

namespace AsyncKeyLock.Tests;

public class AsyncLockKeyTest
{
    [Fact]
    public async Task MultipleReaders()
    {
        AsyncLock<string> lockEntity = new AsyncLock<string>();

        using var w1 = await lockEntity.WriterLockAsync("2");

        using var r1 = await lockEntity.ReaderLockAsync("1");
        using var r2 = await lockEntity.ReaderLockAsync("1");
        using var r3 = await lockEntity.ReaderLockAsync("1");

        Assert.True(true);
    }

    [Fact]
    public async Task MultipleWritersWithDifferentKeys()
    {
        AsyncLock<string> lockEntity = new AsyncLock<string>();

        using var r1 = await lockEntity.ReaderLockAsync("4");

        using var w1 = await lockEntity.WriterLockAsync("1");
        using var w2 = await lockEntity.WriterLockAsync("2");
        using var w3 = await lockEntity.WriterLockAsync("3");

        Assert.True(true);
    }

    [Fact]
    public async Task WriterReaderSameKey()
    {
        AsyncLock<string> lockEntity = new AsyncLock<string>();

        CancellationTokenSource s = new CancellationTokenSource();

        using var w1 = await lockEntity.WriterLockAsync("2", s.Token);

        await Assert.ThrowsAsync<TimeoutException>(()=>  lockEntity.ReaderLockAsync("2").WaitAsync(TimeSpan.FromSeconds(1)));
    }

    [Fact]
    public async Task WriterReaderDifferentKey()
    {
        AsyncLock<string> lockEntity = new AsyncLock<string>();

        using var w1 = await lockEntity.WriterLockAsync("2");

        using var r1 = await lockEntity.ReaderLockAsync("1");

        Assert.True(true);
    }
}
