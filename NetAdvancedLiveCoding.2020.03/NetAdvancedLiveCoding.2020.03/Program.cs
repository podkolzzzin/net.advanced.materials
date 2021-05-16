using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace NetAdvancedLiveCoding._2020._03
{
    class Program
    {
        static Action LFUse2()
        {
            long t = 10l;
            return () => Console.WriteLine(t);
        }

        static Action LFUse()
        {
            int x = 10, y = 20;
            int z = 30;
            Action a = () =>
            {
                Console.WriteLine(x + y);
            };

            Action b = () =>
            {
                Console.WriteLine(z);
            };
            //x = 20;
            //a();
            return a + b;

            void F()
            {
                Console.WriteLine(x);
            }
        }

        static void Main(string[] args)
        {
            string str = "Hello \u200c World";
            while (str.IndexOf("  ") > 0)
            {
                str = str.Replace("  ", " ", false, CultureInfo.InvariantCulture);
            }
            Console.WriteLine(str);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 25; i++)
            {
                sb.Insert(0, "Hello");
                sb.Insert(0, " ");
                sb.Insert(0, "World");
            }

            Console.WriteLine(sb.ToString());

            //var enumerable = FirstYeildMethod();
            //var iterator = enumerable.GetEnumerator();
            //iterator.MoveNext();
            //Console.WriteLine(iterator.Current); // 10
            //iterator = enumerable.GetEnumerator(); 
            //iterator.MoveNext(); 
            //Console.WriteLine(iterator.Current); // 10
            //var t = new Thread(() =>
            //{
            //    iterator = enumerable.GetEnumerator();
            //    iterator.MoveNext();
            //    Console.WriteLine(iterator.Current); // 10
            //});
            //t.Start();
            //t.Join();
        }

        //static IEnumerable<int> FirstYeildMethod()
        //{
        //    Console.WriteLine(nameof(FirstYeildMethod));
        //    for (int i = 0; i < 10; i++)
        //    {
        //        yield return Math.Abs(-10);
        //    }
        //    //try
        //    //{
        //    //    yield return Math.Abs(-12);
        //    //}
        //    //catch
        //    //{
        //    //    yield return Math.Abs(-17);
        //    //}
        //    //finally
        //    //{
        //    //    Console.WriteLine(nameof(FirstYeildMethod) + "Hello World");
        //    //    yield return Math.Abs(-198765);
        //    //}
        //    if (-12 % 2 == 0)
        //    {
        //        yield return Math.Abs(-123450);
        //    }
        //    Console.WriteLine("Finally");
        //}
    }
}
