using System;
using System.IO;

namespace NCryptor.Core
{
    public interface IStreamProvider : IDisposable
    {
        Stream InputStream { get; }
        Stream OutputStream { get; }
    }
}
