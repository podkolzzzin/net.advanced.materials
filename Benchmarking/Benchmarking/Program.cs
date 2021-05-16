using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Benchmarking
{
    [MemoryDiagnoser]
    public class ThreadSleepBenchmark
    {
        [Params(5, 50)]
        public int Delay;

        [Benchmark]
        public void ThreadSleep()
        {
            Thread.Sleep(Delay);
        }

        [Benchmark]
        public async Task TaskDelay()
        {
            await Task.Delay(Delay);
        }
    }

    [RunOncePerIteration]
    public class BiModalBenchmark
    {
        private readonly Random random = new Random(19);

        [Benchmark]
        public void BiModal()
        {
            if (random.Next() % 2 == 0)
                Thread.Sleep(10);
            else
                Thread.Sleep(2);
        }
    }

    [MemoryDiagnoser]
    public class StringBenchmark
    {
        private readonly string str1, str2, str3, str4, str5;

        public StringBenchmark()
        {
            str1 = new string('1', 10);
            str2 = new string('1', 20);
            str3 = new string('1', 30);
            str4 = new string('1', 40);
            str5 = new string('1', 50);
        }

        [Benchmark(Baseline = true)]
        public string StringPlus()
        {
            return str1 + str2 + str3 + str4 + str5;
        }

        [Benchmark]
        public string StringBuilderAppend()
        {
            var sb = new StringBuilder();
            sb.Append(str1);
            sb.Append(str2);
            sb.Append(str3);
            sb.Append(str4);
            sb.Append(str5);
            return sb.ToString();
        }
    }

    public class ListStructVsListClass
    {
        public class SampleRef
        {
            public Lazy<int> Value { get; set; }
            public string Test { get; set; }
        }

        public class SampleVal
        {
            public int Value { get; set; }
            public string Test { get; set; }
        }

        private readonly List<SampleRef> sampleRefs;
        private readonly List<SampleVal> sampleVals;
        public int Count = 60000;
        private readonly Random random = new Random(43);

        public ListStructVsListClass()
        {
            sampleRefs = Enumerable.Range(0, Count).Select(x => new SampleRef() { Test = Guid.NewGuid().ToString(), Value = new Lazy<int>(random.Next()) }).ToList();
            sampleVals = sampleRefs.Select(x => new SampleVal() { Test = x.Test, Value = x.Value.Value }).ToList();
        }

        [Benchmark]
        public string SampleRefsTest()
        {
            string result = null;
            foreach (var item in sampleRefs)
            {
                if (item.Value.Value % 2 == 0)
                    result = item.Test;
            }
            return result;
        }

        [Benchmark]
        public string SampleValsTest()
        {
            string result = null;
            foreach (var item in sampleVals)
            {
                if (item.Value % 2 == 0)
                    result = item.Test;
            }
            return result;
        }
    }

    [MemoryDiagnoser]
    public class LinkedListVsListBenchmark
    {
        private const int Count = 6000;

        private readonly List<int> list;
        private readonly LinkedList<int> linked;
        private readonly Random random = new Random(3);

        public LinkedListVsListBenchmark()
        {
            list = Enumerable.Range(0, Count).Select(x => random.Next()).ToList();
            linked = new LinkedList<int>();
            list.ForEach(x => linked.AddLast(x));
        }

        [Benchmark]
        public int ListIterate()
        {
            int result = 0;
            foreach (var item in list)
                result = item;
            return result;
        }

        [Benchmark]
        public int LinkedIterate()
        {
            int result = 0;
            foreach (var item in linked)
                result = item;
            return result;
        }

        [Benchmark]
        public List<int> ListAdd()
        {
            List<int> local = new List<int>();
            for (int i = 0; i < Count; i++)
            {
                local.Add(i);
            }
            return local;
        }

        [Benchmark]
        public LinkedList<int> LinkedAddLast()
        {
            LinkedList<int> local = new LinkedList<int>();
            for (int i = 0; i < Count; i++)
            {
                local.AddLast(i);
            }
            return local;
        }

        [Benchmark]
        public List<int> ListInsert()
        {
            List<int> local = new List<int>();
            for (int i = 0; i < Count; i++)
            {
                local.Insert(0, i);
            }
            return local;
        }

        [Benchmark]
        public LinkedList<int> LinkedAddFirst()
        {
            LinkedList<int> local = new LinkedList<int>();
            for (int i = 0; i < Count; i++)
            {
                local.AddLast(i);
            }
            return local;
        }
    }

    class SemaphoreLock
    {
        private class LockObj : IDisposable
        {
            private readonly SemaphoreSlim semaphore;
            public LockObj(SemaphoreSlim semaphore)
            {
                this.semaphore = semaphore;
            }

            public void Dispose()
            {
                semaphore.Release();
            }
        }

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public async Task<IDisposable> Lock()
        {
            await semaphore.WaitAsync();
            return new LockObj(semaphore);
        }
    }

    [MemoryDiagnoser]
    public class CacheBenchmark
    {
        private readonly ConcurrentDictionary<string, string> cache = new ConcurrentDictionary<string, string>();
        private string requestVal;

        public CacheBenchmark()
        {
            for (int i = 0; i < 1000000; i++)
            {
                cache[requestVal = Guid.NewGuid().ToString()] = Guid.NewGuid().ToString();
            }
        }

        public async Task<string> AsyncAwaitCache(string key)
        {
            if (cache.TryGetValue(key, out var val))
                return val;
            return await ReadFromDb(key);
        }
        public Task<string> AsynclessCache(string key)
        {
            if (cache.TryGetValue(key, out var val))
                return Task.FromResult(val);
            return ReadFromDb(key);
        }

        public ValueTask<string> ValueTaskCache(string key)
        {
            if (cache.TryGetValue(key, out var val))
                return new ValueTask<string>(val);
            return new ValueTask<string>(ReadFromDb(key));
        }

        private async Task<string> ReadFromDb(string key)
        {
            await Task.Delay(1);
            var result = cache[key] = Guid.NewGuid().ToString();
            return result;
        }

        [Benchmark]
        public ValueTask<string> ValueTask()
        {
            return ValueTaskCache(requestVal);
        }

        [Benchmark]
        public Task<string> AsyncAwait()
        {
            return AsyncAwaitCache(requestVal);
        }

        [Benchmark]
        public Task<string> Asyncless()
        {
            return AsynclessCache(requestVal);
        }
    }

    [MemoryDiagnoser]
    public class CacheBenchmark2
    {
        private readonly ConcurrentDictionary<string, string> cache = new ConcurrentDictionary<string, string>();
        private readonly List<string> requestVal = new List<string>();

        public CacheBenchmark2()
        {
            var r = new Random(42);
            for (int i = 0; i < 1000000; i++)
            {
                string key;
                cache[key = Guid.NewGuid().ToString()] = Guid.NewGuid().ToString();
                if (r.NextDouble() < .005)
                    requestVal.Add(key);
            }
            Console.WriteLine("Count = " + requestVal.Count);
        }

        public async Task<string> AsyncAwaitCache(string key)
        {
            if (cache.TryGetValue(key, out var val))
                return val;
            return await ReadFromDb(key);
        }
        public Task<string> AsynclessCache(string key)
        {
            if (cache.TryGetValue(key, out var val))
                return Task.FromResult(val);
            return ReadFromDb(key);
        }

        public ValueTask<string> ValueTaskCache(string key)
        {
            if (cache.TryGetValue(key, out var val))
                return new ValueTask<string>(val);
            return new ValueTask<string>(ReadFromDb(key));
        }

        private async Task<string> ReadFromDb(string key)
        {
            await Task.Delay(1);
            var result = cache[key] = Guid.NewGuid().ToString();
            return result;
        }

        [Benchmark(Baseline = true)]
        public async ValueTask<string> ValueTask()
        {
            string result = null;
            foreach (var item in requestVal)
                result = await ValueTaskCache(item);
            return result;
        }

        [Benchmark]
        public async Task<string> AsyncAwait()
        {
            string result = null;
            foreach (var item in requestVal)
                result = await AsyncAwaitCache(item);
            return result;
        }

        [Benchmark]
        public async Task<string> Asyncless()
        {
            string result = null;
            foreach (var item in requestVal)
                result = await AsynclessCache(item);
            return result;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            //lock (args)
            //{
            //    await Task.Delay(1000);
            //}

            var tasks = new List<Task>();
            for (int i = 0; i < Environment.ProcessorCount * 3; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    double result = 0;
                    Random r = new Random();
                    for (int i = 0; i < int.MaxValue / 16; i++)
                        if (r.Next() > 0.5)
                            result = Math.Sin(r.Next());
                    return result;
                }));
            }

            await Task.WhenAll(tasks);

            //var scheduler = new System.Threading.Tasks.Schedulers.LimitedConcurrencyLevelTaskScheduler(Environment.ProcessorCount / 2);
            //var tasks = new List<Task>();
            //for (int i = 0; i < Environment.ProcessorCount * 3; i++)
            //{
            //    tasks.Add(Task.Factory.StartNew(() =>
            //    {
            //        double result = 0;
            //        Random r = new Random();
            //        for (int i = 0; i < int.MaxValue / 16; i++)
            //            if (r.Next() > 0.5)
            //                result = Math.Sin(r.Next());
            //        return result;
            //    }, CancellationToken.None, TaskCreationOptions.None, scheduler));
            //}

            //await Task.WhenAll(tasks);

            //var lockObj = new SemaphoreLock();

            //using (await lockObj.Lock())
            //{

            //}

            //try
            //{
            //    await s.WaitAsync();
            //    await Task.Delay(1000);
            //}
            //finally
            //{
            //    s.Release();
            //}
            Console.WriteLine("X");
            //BenchmarkRunner.Run<NewtonsoftVSSystemTextBenchmark>();
            //BenchmarkRunner.Run<LinkedListVsListBenchmark>();
            //BenchmarkRunner.Run<StringBenchmark>();
            //BenchmarkRunner.Run<BiModalBenchmark>();
            //BenchmarkRunner.Run<ThreadSleepBenchmark>(DefaultConfig.Instance.AddColumn(StatisticColumn.P95));
            //BenchmarkRunner.Run<NewtonsoftVSSystemTextBenchmark>();
        }
    }
}
