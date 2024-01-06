using System;
using System.Runtime.InteropServices;

namespace NCryptor.GUI.Helpers
{
    internal class ExternalMethods
    {
        private ExternalMethods() { }

        internal static bool CmpByteArrays(byte[] a, byte[] b)
        {
            return a.Length == b.Length && memcmp(a, b, a.Length) == 0;
        }

        internal static void ZeroMemset(byte[] a)
        {
            memset(a, 0, a.Length);
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] a, byte[] b, long size);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr memset(byte[] a, int ch, long size);
    }
}
