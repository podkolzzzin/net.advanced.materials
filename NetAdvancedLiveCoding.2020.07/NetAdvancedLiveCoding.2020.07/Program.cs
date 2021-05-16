using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetAdvancedLiveCoding._2020._07
{
    [MemoryDiagnoser]
    public class BlaBlaRepository
    {
        private readonly ConcurrentDictionary<string, string> cache = new ConcurrentDictionary<string, string>();
        private string requestVal;

        public BlaBlaRepository()
        {
            for (int i = 0; i < 1000000; i++)
            {
                cache[requestVal = Guid.NewGuid().ToString()] = Guid.NewGuid().ToString();
            }
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

    class SemaphoreLock
    {
        private class Lock : IDisposable
        {
            private SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);


            public Lock(SemaphoreSlim semaphore)
            {
                this.semaphore = semaphore;
            }

            public void Dispose()
            {
                semaphore.Release();
            }
        }

        private SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);

        public async Task<IDisposable> GetLock()
        {
            await semaphore.WaitAsync();
            return new Lock(semaphore);
        }
    }

    class Program
    {
        

        static async Task Main(string[] args)
        {
            //BenchmarkRunner.Run<BlaBlaRepository>();
            var semaphore = new SemaphoreSlim(0, 1);

            await semaphore.WaitAsync();
            try 
            {
                await Task.Delay(100);
            }
            finally
            {
                semaphore.Release();
            }
            var sl = new SemaphoreLock();
            using (await sl.GetLock()) 
            {
                await Task.Delay(100);
            }

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
            //    }));
            //}

            //await Task.WhenAll(tasks);
        }
    }
}
