using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetAdvancedLiveCoding.Threading._2020._04
{
    class Program
    {
        public static event Action MyEvent;

        static async Task Main(string[] args)
        {
            MyEvent += Program_MyEvent;
            Thread p = new Thread(ThreadProc2);
            p.Start();
            Thread.Sleep(100);
            p.Abort();
            Console.WriteLine("Interrupt called");
            p.Join();

            await foreach(var item in TaskProc())
            {
                Console.WriteLine(item);
            }
        }

        private static void Program_MyEvent()
        {
            throw new NotImplementedException();
        }

        private static void ThreadProc2()
        {
            for (int i = 0; i < int.MaxValue; i++)
                ;
            Console.WriteLine("Loop finished");
            Thread.Sleep(100);
        }

        private static async IAsyncEnumerable<int> TaskProc()
        { //->
            await Task.Delay(100);
            //->
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(10);
                if (i % 2 == 0)
                {
                    await Task.Delay(1);
                    yield return i;
                }
            }
            throw new Exception();
        }

        private static void ThreadProc(object obj)
        {
            Thread.Sleep(500);
            throw new Exception();
        }
    }
}
