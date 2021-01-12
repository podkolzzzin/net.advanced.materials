using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace NetAdvancedLiveCoding._2020._01
{
    struct SampleStruct
    {
        public byte B1;
        public uint I;
        public ushort S;
    }
    class Program
    {
        static void Main(string[] args)
        {
            var str = "Hello World";
            var ptr = AddressOf(str);

            var obj = new object();
            var oPtr = AddressOf(obj);

            uint i = 0xFFFFFFFF;
            object bi = (object)i;
            var iPtr = AddressOf(bi);

            var sArr = new[] {
                new SampleStruct()
                {
                    B1 = 0xAA,
                    S = 0xBBBB,
                    I = 0xFFFFFFFF
                },
                new SampleStruct()
                {
                    B1 = 0xCC,
                    S = 0xDDDD,
                    I = 0x1111111
                }
            };
            var aPtr = AddressOf(sArr);

            var syncObject = new object();
            var syncObject2 = new object();

            var t = new Thread(o =>
            {
                var soPtr = AddressOf(syncObject);
                var soPtr2 = AddressOf(syncObject2);
                syncObject.GetHashCode();
                lock (syncObject)
                {
                    lock (syncObject2)
                    {
                        syncObject.GetHashCode();
                    }
                }
                syncObject.GetHashCode();
            });
            t.Start();
            t.Join();


            byte[] bArr = new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xAA, 0xBB, 0xCC, 0xDD };
            int[] result = ChangeType(bArr, new int[0]);
        }

        static unsafe int[] ChangeType(byte[] source, int[] typeSample)
        {
            var sPtr = (ulong*)AddressOf(source).ToPointer();
            var dPtr = (ulong*)AddressOf(typeSample).ToPointer();
            *sPtr = *dPtr;
            *(sPtr + 1) = (ulong)(source.Length / sizeof(int));

            var result = (int[])(object)source;
            return result;
        }

        static unsafe IntPtr AddressOf(object o)
        {
            TypedReference mk = __makeref(o);
            return **(IntPtr**)&mk;
        }
    }
}
