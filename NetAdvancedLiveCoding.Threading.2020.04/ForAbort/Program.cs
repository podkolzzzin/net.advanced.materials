using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ForAbort
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            TaskJob(tokenSource.Token);
            tokenSource.Cancel();

            Thread p = new Thread(ThreadProc2);
            p.Start();
            Thread.Sleep(100);
            p.Abort();
            Console.WriteLine("Abort called");
            p.Join();
        }

        private static void TaskJob(CancellationToken token)
        {
            while (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
        }

        private static void ThreadProc2()
        {
            try {
                throw new Exception();
            }
            catch
            {
                for (int i = 0; i < int.MaxValue; i++)
                    ;
            }
            Console.WriteLine("Loop finished");
            Thread.Sleep(100);
        }

        private static async Task TaskProc()
        {
            await Task.Delay(100);
            throw new Exception();
        }

        private static void ThreadProc(object obj)
        {
            Thread.Sleep(500);
            throw new Exception();
        }
    }
}
