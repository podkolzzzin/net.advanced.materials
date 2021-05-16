using HarmonyLib;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NetAdvancedLiveCoding._2020._2
{
    class A
    {
        public static void F()
        {
            Console.WriteLine("F");
        }

        public void Inst()
        {
            Console.WriteLine("Inst");
        }

        public virtual void Virt()
        {
            Console.WriteLine("Virt");
        }
    }

    class B
    {
        public static void SmallMethod(LongJIT that, Stopwatch sw)
        {
            sw.Stop();
        }
    }

    struct SampleStruct
    {
        public byte B1, B2;
        public short S;
        public string Str;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var m = typeof(LongJIT);
            var mh = m.GetMethod("BigMethod");
            Memory.DetourMethod(mh, typeof(B).GetMethod("SmallMethod"));
            //IlligalOvverride(mh, typeof(B).GetMethod("SmallMethod"));
            LongJIT lj = new LongJIT();
            var sw = Stopwatch.StartNew();
            lj.BigMethod(sw);
            var mh2 = m.GetMethod("BigMethod").MethodHandle.GetFunctionPointer();
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);


            sw = Stopwatch.StartNew();
            lj.BigMethod(sw);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        public static unsafe void IlligalOvverride(MethodBase original, MethodBase replacement)
        {
            var originalCodeStart = original.MethodHandle.GetFunctionPointer().ToInt64();
            var patchCodeStart = replacement.MethodHandle.GetFunctionPointer().ToInt64();

            RuntimeHelpers.PrepareMethod(original.MethodHandle);
            RuntimeHelpers.PrepareMethod(replacement.MethodHandle);

            *(uint*)(originalCodeStart + 1) = (uint)(int)(patchCodeStart - (originalCodeStart + 1 + sizeof(uint)));
        }
    }
}
