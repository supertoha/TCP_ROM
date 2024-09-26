using MultiProcessCommunicator.Internal;
using MultiProcessCommunicator.Server;
using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection;

namespace MultiProcessCommunicator.Client
{
    internal class ClientRequestMessage
    {
        public ClientRequestMessage(MethodInfo serverMethod, object[] inputParams, ServerInstance serverInstance, int requestId, Socket clientSocket)
        {
            this._serverMethod = serverMethod;
            this._inputParams = inputParams;
            this._serverInstance = serverInstance;
            this._requestId = requestId;
            this._clientSocket = clientSocket;
        }

        private MethodInfo _serverMethod;
        private object[] _inputParams;
        private ServerInstance _serverInstance;
        private int _requestId;
        private Socket _clientSocket;

        public bool Execute()
        {
            object serverSideResponse = null;
            var serverSideMethodExecutionStatus = ServerResponseCode.Ok;
            try
            {
                serverSideResponse = this._serverMethod.Invoke(this._serverInstance.Instance, this._inputParams);
            }
            catch/* (Exception exc)*/
            {
                serverSideMethodExecutionStatus = ServerResponseCode.Exception;
            }

            // pack response and send to Client
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(0);
                    writer.Write(_requestId);
                    writer.Write((int)serverSideMethodExecutionStatus); // execution status

                    var responseType = this._serverMethod.ReturnType;

                    if (serverSideMethodExecutionStatus == ServerResponseCode.Ok && responseType != null && responseType != typeof(void))
                        DataSerializer.Serialize(writer, serverSideResponse, responseType);
                }

                var responseData = ms.ToArray();
                var firstBytes = BitConverter.GetBytes(responseData.Length);
                Buffer.BlockCopy(firstBytes, 0, responseData, 0, 4);

                this._clientSocket.Send(responseData);
            }

            return true;
        }

    }
}
