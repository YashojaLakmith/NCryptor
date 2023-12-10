using System;

namespace NCryptor.Core
{
    public interface ILogger
    {
        void Log(string message);
        void Log(Exception exception);
    }
}
