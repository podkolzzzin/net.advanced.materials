using System;

namespace NetUnderTheHood2021._1
{
    struct SampleStruct
    {
        public byte B;
        public ushort S;
    }

    struct SampleAlignment
    {
        public byte B1;
        public byte B2;
        public ulong L;
        public uint I;

    }

    class SampleCast
    {
        public uint A, B, C, D;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var sArr = new uint[] { 0xAAAAAAAA, 0xBBBBBBBB, 0xCCCCCCCC, 0xDDDDDDDD };
            var bArr = ChangeType(sArr, new SampleCast());
            var bPtr = AddressOf(bArr);
        }

        private static unsafe SampleCast ChangeType(uint[] sArr, SampleCast sampleCast)
        {
            var arrPtr = (long*)AddressOf(sArr).ToPointer();
            var dstPtr = (long*)AddressOf(sampleCast).ToPointer();
            *arrPtr = *dstPtr;
            return (SampleCast)(object)sArr;
        }

        private static unsafe byte[] ChangeType(uint[] sArr, byte[] v)
        {
            var arrPtr = (long*)AddressOf(sArr).ToPointer();
            var dstPtr = (long*)AddressOf(v).ToPointer();
            long newLength = sizeof(uint) / sizeof(byte) * sArr.LongLength;
            *arrPtr = *dstPtr;
            *(arrPtr + 1) = newLength;
            return (byte[])(object)sArr;
        }

        private static unsafe IntPtr AddressOf(object str)
        {
            TypedReference tr = __makeref(str);
            return **(IntPtr**)&tr;
        }
    }
}
