using System;
using System.Threading;
using System.Threading.Tasks;

namespace NCryptor.Core
{
    public interface ICryptoService : IDisposable
    {
        Task EncryptAsync(CancellationToken cancellationToken = default);
        Task DecryptAsync(CancellationToken cancellationToken = default);
    }
}
