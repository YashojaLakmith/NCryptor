using System;

namespace SymmetricAlgorithms
{
    public interface IKeyMaterial : IDisposable
    {
        byte[] Key { get; }
        byte[] IV { get; }
    }
}
