using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace AsyncKeyLock.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class BenchmarkSimpleKeyLock
{
    [GlobalSetup]
    public void GlobalSetup()
    {
        _AsyncKeyLock = new AsyncLock<string>();
        _AsyncKeyedLock = new AsyncKeyedLock.AsyncKeyedLocker<string>(o =>
        {
            o.PoolSize = NumberOfLocks;
        });
        _StripedAsyncKeyedLock = new AsyncKeyedLock.StripedAsyncKeyedLocker<string>(NumberOfLocks);
        _ImageSharpWebLock = new SixLabors.ImageSharp.Web.Synchronization.AsyncKeyLock<string>();
    }

    private AsyncLock<string> _AsyncKeyLock;
    private AsyncKeyedLock.AsyncKeyedLocker<string> _AsyncKeyedLock;
    private AsyncKeyedLock.StripedAsyncKeyedLocker<string> _StripedAsyncKeyedLock;
    private SixLabors.ImageSharp.Web.Synchronization.AsyncKeyLock<string> _ImageSharpWebLock;

    [Params(1_00, 1_000, 10_000)]
    public int NumberOfLocks;

    [Benchmark(Baseline = true)]
    public async Task AsyncKeyLock()
    {
        for (int i = 0; i < NumberOfLocks; i++)
        {
            using (await _AsyncKeyLock.WriterLockAsync(string.Empty))
            {

            }
        }
    }

    [Benchmark]
    public async Task AsyncKeyedLock()
    {
        for (int i = 0; i < NumberOfLocks; i++)
        {
            using (await _AsyncKeyedLock.LockAsync(string.Empty))
            {

            }
        }
    }
    
    [Benchmark]
    public async Task StripedAsyncKeyedLock()
    {
        for (int i = 0; i < NumberOfLocks; i++)
        {
            using (await _StripedAsyncKeyedLock.LockAsync(string.Empty))
            {

            }
        }
    }

    [Benchmark]
    public async Task ImageSharpWeb()
    {
        for (int i = 0; i < NumberOfLocks; i++)
        {
            using (await _ImageSharpWebLock.LockAsync(string.Empty))
            {

            }
        }
    }
}
