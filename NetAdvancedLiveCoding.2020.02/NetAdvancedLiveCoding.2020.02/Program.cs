using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NetAdvancedLiveCodding._2020._02
{
    class A
    {
        public void F()
        {
            Console.WriteLine("Hello World");
        }

        public static void SF()
        {
            Console.WriteLine("Static");
        }

        public virtual void VF()
        {
            Console.WriteLine("Virtual");
        }
    }

    class B
    {
        public static void Overrided(A that)
        {
            Console.WriteLine("The system was corrupted");
        }
    }

    class SCtor<T>
    {
        public static int I { get; set; }

        static SCtor()
        {
            I++;
            Console.WriteLine(I);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new SCtor<int>());
            Console.WriteLine(new SCtor<double>());
            Console.WriteLine(new SCtor<string>());
            Console.WriteLine(new SCtor<object>());

            M<int>();
            M<double>();
            M<string>();
            M<object>();

            //var a = new A();
            //HarmonyLib.Memory.DetourMethod(a.GetType().GetMethod("F"), typeof(B).GetMethod("Overrided"));
            //a.F();
            //A.SF();
            //a.F();
            //a.VF();

            //var sw = Stopwatch.StartNew();
            //var lj = new LongJIT();
            //lj.BigMethod(sw);
            //Console.WriteLine(sw.ElapsedMilliseconds);
        }

        static void M<T>()
        {
            Console.WriteLine(typeof(T));
        }

        static unsafe void IlligallMadOverride(MethodBase source, MethodBase dest)
        {
            var fp1 = source.MethodHandle.GetFunctionPointer();
            var fp2 = dest.MethodHandle.GetFunctionPointer();

            RuntimeHelpers.PrepareMethod(source.MethodHandle);
            RuntimeHelpers.PrepareMethod(dest.MethodHandle);

            var f1Ptr = (byte*)fp1.ToPointer();
            var f2Ptr = (byte*)fp2.ToPointer();
            var sJump = (uint)f1Ptr + 1 + 4;
            *(uint*)(f1Ptr + 1) = (uint)(f2Ptr - sJump);
        }

        static void M(Stopwatch sw)
        {
            sw.Stop();
        }
    }
}