using System;

namespace NCryptor.Core
{
    public interface IKeyMaterial : IDisposable
    {
        byte[] Key { get; }
        byte[] IV { get; }
    }
}
