using MultiProcessCommunicator.Server;
using System;
using System.Threading.Tasks;

namespace MultiProcessCommunicator.Internal
{
    internal class ServerReqResponseMessage
    {
        public ServerReqResponseMessage(int requestId, Type responseType)
        {
            RequestId = requestId;
            ReturnType = responseType;
        }

        //public TaskCompletionSource _
        public int RequestId;
        public Type ReturnType;
        public object ResponseObject;
        public ServerResponseCode ResponseStatus = ServerResponseCode.Unknown;
    }
}
