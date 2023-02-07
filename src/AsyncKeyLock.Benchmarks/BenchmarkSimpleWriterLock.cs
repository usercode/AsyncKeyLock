using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace AsyncKeyLock.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class BenchmarkSimpleWriterLock
{
    [GlobalSetup]
    public void GlobalSetup()
    {
        _AsyncKeyLock = new AsyncLock();
        _NeoSmartLock = new NeoSmart.AsyncLock.AsyncLock();
        _NitoLock = new Nito.AsyncEx.AsyncLock();
    }

    private AsyncLock _AsyncKeyLock;
    private NeoSmart.AsyncLock.AsyncLock _NeoSmartLock;
    private Nito.AsyncEx.AsyncLock _NitoLock;

    [Params(1_00, 1_000, 10_000)]
    public int NumberOfLocks;

    [Benchmark(Baseline = true)]
    public async Task AsyncKeyLock()
    {
        for (int i = 0; i < NumberOfLocks; i++)
        {
            using (await _AsyncKeyLock.WriterLockAsync())
            {

            }
        }
    }

    [Benchmark]
    public async Task NeoSmart()
    {
        for (int i = 0; i < NumberOfLocks; i++)
        {
            using (await _NeoSmartLock.LockAsync())
            {

            }
        }
    }

    [Benchmark]
    public async Task Nito()
    {
        for (int i = 0; i < NumberOfLocks; i++)
        {
            using (await _NitoLock.LockAsync())
            {

            }
        }
    }
}