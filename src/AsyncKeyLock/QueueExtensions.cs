﻿// Copyright (c) usercode
// https://github.com/usercode/AsyncKeyLock
// MIT License

namespace AsyncKeyLock;

public static class QueueExtensions
{
    public static Task<T> Enqueue<T>(this Queue<TaskCompletionSource<T>> queue, CancellationToken cancellationToken)
    {
        TaskCompletionSource<T> item = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

        queue.Enqueue(item);

        if (cancellationToken.CanBeCanceled)
        {
            CancellationTokenRegistration registerCallback = cancellationToken.Register(() =>
            {
                item.TrySetCanceled();
            });

            item.Task.ContinueWith(_ =>
            {
                registerCallback.Dispose();
            },
            CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        return item.Task;
    }
}
