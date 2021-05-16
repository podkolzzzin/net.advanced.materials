using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Threading;

namespace NetAdvancedLiveCoding.Benchmarking._2020._05
{
    public class ThreadSleepBenchmark
    {
        [Params(5, 50)]
        public int Delay =5;

        [Benchmark]
        public void ThreadSleep()
        {
            Thread.Sleep(Delay);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ThreadSleepBenchmark>();
        }
    }
}
