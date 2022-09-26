using System;
using System.Collections.Generic;
using System.Text;

namespace MultiProcessCommunicator.Server
{
    public class ServerException : Exception
    {
        public ServerException(string message) : base(message)
        {

        }
    }
}
