﻿using MultiProcessCommunicator.Internal;
using MultiProcessCommunicator.Server;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MultiProcessCommunicator.Client
{
    public class ClientDecorator : DispatchProxy
    {
        public ClientDecorator() : base()
        {
            this._clientId = Guid.NewGuid().GetHashCode();
            this._client = new MpcTcpClient(this);
        }

        protected static TimeSpan ResponseTimeout = TimeSpan.FromSeconds(60);
        private ConcurrentDictionary<int, ServerReqResponseMessage> _serverResponses = new ConcurrentDictionary<int, ServerReqResponseMessage>();
        private readonly MpcTcpClient _client;
        private readonly int _clientId;

        public void PushServerResponseToCollection(byte[] serverResponse, int lenght)
        {
            using (var ms = new MemoryStream(serverResponse, 0, lenght, false))
            {
                using (var reader = new BinaryReader(ms))
                {
                    var packetLen = reader.ReadInt32();
                    var requestId = reader.ReadInt32();
                    var executionStatusCode = (ServerResponseCode)reader.ReadInt32();

                    ServerReqResponseMessage requestObject = null;
                    if (this._serverResponses.TryGetValue(requestId, out requestObject))
                    {
                        if (requestObject.ReturnType != null && requestObject.ReturnType != typeof(void) && executionStatusCode == ServerResponseCode.Ok)
                            requestObject.ResponseObject = DataSerializer.Deserialize(reader, requestObject.ReturnType);

                        requestObject.ResponseStatus = executionStatusCode;

                        //_log.Info($"[{_instanceId}] got responce id {requestId}; status {executionStatusCode}; len {lenght}");
                    }

                }
            }
        }

        private static int _connectMethodSignatureId = "Boolean Connect(System.String, Int32)".GetHashCode();
        private static int _disconnectMethodSignatureId = "Void Disconnect()".GetHashCode();

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var methodSignature = targetMethod.ToString();
            var methodId = methodSignature.GetHashCode();

            #region internal methods

            if (methodId == _connectMethodSignatureId)
                return this._client.Connect((string)args[0], (Int32)args[1]);

            if (methodId == _disconnectMethodSignatureId)
            {
                this._client.Disconnect();
                return null;
            }

            #endregion

            var serviceId = targetMethod.DeclaringType.Name.GetHashCode();

            var returnType = targetMethod.ReturnType;

            byte[] requestData = null;
            var requestId = Guid.NewGuid().GetHashCode();//this._lastRequestId;
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(0);              // 0. Len
                    writer.Write(this._clientId); // 1. client id
                    writer.Write(requestId);      // 2. request Id
                    writer.Write(serviceId);      // 3. service id
                    writer.Write(methodId);       // 4. method Id

                    foreach (var argVal in args)
                    {
                        var paramType = argVal.GetType();// type will be detected previously?
                        DataSerializer.Serialize(writer, argVal, paramType);
                    }

                    requestData = ms.ToArray();
                    var packetLen = requestData.Length;
                    var packetSizeBytes = BitConverter.GetBytes(packetLen);
                    Buffer.BlockCopy(packetSizeBytes, 0, requestData, 0, 4);
                }
            }

            var reqRespMessage = new ServerReqResponseMessage(requestId, returnType);
            this._serverResponses.TryAdd(requestId, reqRespMessage);

            var sendResult = this._client.SendMessage(requestData);
            //_log.Info($"[{_instanceId}] request id {requestId} sent to server; data size {requestData.Length}; sendResult {sendResult}  \n");

            if (!sendResult)
                throw new ServerException("Server can`t process request");

            //_log.Info($"[{_instanceId}] request id {requestId} Added to dictionary \n");

            var watcher = new Stopwatch();
            watcher.Start();

            while (this._client.IsConnected)
            {
                if (reqRespMessage.ResponseStatus != ServerResponseCode.Unknown)
                    break;

                if (watcher.Elapsed > ResponseTimeout)
                    break;
            }

            var result = reqRespMessage.ResponseObject;

            if (reqRespMessage.ResponseStatus == ServerResponseCode.Exception)
                throw new ServerException($"Unknown exception on the server side {methodSignature}");


            this._serverResponses.TryRemove(requestId, out reqRespMessage);            

            return result;

        }

    }
}
