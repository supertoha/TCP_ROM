using System;

namespace MultiProcessCommunicator.Server
{
    internal enum ServerResponseCode
    {
        Unknown = 0,
        Ok = 1,
        Timeout = 2,
        Exception = 3,
    }
}
