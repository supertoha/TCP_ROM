using MultiProcessCommunicator.Server;
using System;

namespace MultiProcessCommunicator.Internal
{
    internal class ServerReqRespMessage
    {
        public ServerReqRespMessage(int requestId, Type responseType)
        {
            RequestId = requestId;
            ReturnType = responseType;
        }

        public int RequestId;
        public Type ReturnType;
        public object ResponseObject;
        public ServerResponseCode ResponseStatus = ServerResponseCode.Unknown;
    }
}
