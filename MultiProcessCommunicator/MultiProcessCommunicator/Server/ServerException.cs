using System;

namespace MultiProcessCommunicator.Server
{
    public class ServerException : Exception
    {
        public ServerException(string message) : base(message)
        {

        }
    }
}
