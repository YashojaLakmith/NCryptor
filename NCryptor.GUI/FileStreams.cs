using System.IO;

using NCryptor.Core;

namespace NCryptor.GUI
{
    internal class FileStreams : IStreamProvider
    {
        private bool disposedValue = false;

        public FileStreams(string pathIn, string pathOut)
        {
            InputStream = File.Open(pathIn, FileMode.Open, FileAccess.Read, FileShare.Read);
            OutputStream = File.Open(pathOut, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
        }

        public Stream InputStream {  get; private set; }

        public Stream OutputStream { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    InputStream.Dispose();
                    OutputStream.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~FileStreams()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}
