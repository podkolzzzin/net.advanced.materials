using System;
using System.Diagnostics;

namespace NetAdvancedLiveCoding
{
    class Sample
    {
        public ulong A;
        public string S;
    }

    class SampleAlignment
    {
        public ushort S;
        public byte B;
        public ulong L;
        public uint I;
        public string Str;

        public void StandardMethod(Stopwatch sw)
        {
            sw.Stop();
        }
    }

    interface I
    {
        double DoSmth();
    }

    class C1 : I
    {
        private static Random r = new Random();

        public double DoSmth()
        {
            return Math.Sin(r.NextDouble());
        }
    }

    class C2 : I
    {
        private static Random r = new Random();

        public double DoSmth()
        {
            return Math.Cos(r.NextDouble());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var c1 = new C1();
            Usage(c1);
            Usage(c1);

            var s = new Sample()
            {
                A = 0xFFFFFFFFFFFFFFFF,
                S = "Hello World"
            };
            var ptr = AddressOf(s);

            s.GetHashCode();

            LongJIT lj = new LongJIT();
            var sw = Stopwatch.StartNew();
            lj.BigMethod(sw);
            Console.WriteLine(sw.ElapsedMilliseconds);

            sw = Stopwatch.StartNew();
            lj.BigMethod(sw);
            Console.WriteLine(sw.ElapsedMilliseconds);

            var ssa = new SampleAlignment();
            sw = Stopwatch.StartNew();
            ssa.StandardMethod(sw);
            Console.WriteLine(sw.ElapsedMilliseconds);

            lock (s)
            {

            }
            var sptr = AddressOf(s.S);

            var sa = new SampleAlignment()
            {
                B = 0xAA,
                S = 0xBBBB,
                I = 0xCCCCCCCC,
                L = 0xDDDDDDDDDDDDDDDD,
                Str = "Ololo"
            };
            var saPtr = AddressOf(sa);
        }

        private static void Usage(C1 c1)
        {
            for (int i = 0; i < 1000000; i++)
            {
                c1.DoSmth();
            }
        }

        public static unsafe IntPtr AddressOf(object o)
        {
            TypedReference ts = __makeref(o);
            return **(IntPtr**)&ts;
        }
    }
}