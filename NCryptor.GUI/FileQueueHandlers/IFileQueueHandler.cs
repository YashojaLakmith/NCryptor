using System;
using System.Threading.Tasks;

using NCryptor.GUI.Events;

namespace NCryptor.GUI.FileQueueHandlers
{
    internal interface IFileQueueHandler : IFileQueueEvents, IDisposable
    {
        Task EncryptTheFilesAsync();
        Task DecryptTheFilesAsync();
    }
}
