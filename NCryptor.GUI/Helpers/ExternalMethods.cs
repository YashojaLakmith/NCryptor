using System;
using System.Runtime.InteropServices;

namespace NCryptor.GUI.Helpers
{
    /// <summary>
    /// Contains the helper methods that are moderated with P/Invoke.
    /// </summary>
    internal class ExternalMethods
    {
        private ExternalMethods() { }
        
        /// <returns><c>true</c> is matches. Otherwise <c>false</c></returns>
        /// <exception cref="ArgumentNullException"/>
        internal static bool CmpByteArrays(byte[] a, byte[] b)
        {
            if(a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            if(b is null)
            {
                throw new ArgumentNullException(nameof(b));
            }
            return a.Length == b.Length && memcmp(a, b, a.Length) == 0;
        }

        /// <summary>
        /// Sets all the elements of the byte array to 0.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        internal static void ZeroMemset(byte[] a)
        {
            if(a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            memset(a, 0, a.Length);
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] a, byte[] b, long size);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr memset(byte[] a, int ch, long size);
    }
}
