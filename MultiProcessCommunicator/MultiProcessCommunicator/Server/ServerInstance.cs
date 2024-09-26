using MultiProcessCommunicator.Client;
using MultiProcessCommunicator.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace MultiProcessCommunicator.Server
{
    public class ServerInstance
    {
        public ServerInstance(int port)
        {
            this._server = new MpcTcpServer(port, this);
            this._server.StartServer();
        }

        private MpcTcpServer _server;
        private Dictionary<int, MethodInfo> _methodsCache = new Dictionary<int, MethodInfo>();

        protected void StartServerThreadIfNeed()
        {
            if (_isServerThreadStarted != null)
                return;

            var serverThread = new Thread(ServerThread) { Priority = ThreadPriority.AboveNormal };
            serverThread.Start();
        }

        protected void ServerThread()
        {
            this._isServerThreadStarted = new CancellationTokenSource();
            ClientRequestMessage message = null;
            while (this._isServerThreadStarted.IsCancellationRequested == false)
            {
                try
                {                    
                    if (this._clientRequests.TryDequeue(out message))
                    {
                        var returnResult = message.Execute();                        
                    }
                }
                catch (Exception)
                { }
            }
        }


        private CancellationTokenSource _isServerThreadStarted;

        private readonly ConcurrentQueue<ClientRequestMessage> _clientRequests = new ConcurrentQueue<ClientRequestMessage>();

        public IMpcService Instance { get; set; }

        public void Stop()
        {
            this._isServerThreadStarted?.Cancel();
            this._server.Stop();
        }

        public void PushClientRequestToCollection(byte[] clientRequestData, int length, Socket clientSocket)
        {
            if (length < 0)
                return;

            this.StartServerThreadIfNeed();

            using (var ms = new MemoryStream(clientRequestData, 0, length, false))            
            using (var reader = new BinaryReader(ms))
            {
                var packetLen = reader.ReadInt32();
                var clientId = reader.ReadInt32();
                var requestId = reader.ReadInt32();
                var serviceId = reader.ReadInt32();
                var methodId = reader.ReadInt32();

                var serverSide = MpcManager.GetService(serviceId);

                if (serverSide == null)
                    return;

                var serverSideMethod = serverSide.GetMethodWithCache(methodId);

                var inputParametersInfo = serverSideMethod.GetParameters();

                var inputParams = new object[inputParametersInfo.Length];

                for (int i = 0; i < inputParametersInfo.Length; i++)
                {
                    var paramInfo = inputParametersInfo[i];
                    var paramValue = DataSerializer.Deserialize(reader, paramInfo.ParameterType);
                    inputParams[i] = paramValue;
                }

                var clientRequestMessage = new ClientRequestMessage(serverSideMethod, inputParams, serverSide, requestId, clientSocket);
                this._clientRequests.Enqueue(clientRequestMessage);

            }
            
        }


        public MethodInfo GetMethodWithCache(int methodId)
        {
            MethodInfo result = null;
            if (this._methodsCache.TryGetValue(methodId, out result))            
                return result;

            result = GetMethod(methodId);
            if (result != null)
                this._methodsCache.Add(methodId, result);

            return result;
        }


        public MethodInfo GetMethod(int methodId)
        {
            var instanceType = Instance.GetType();
            var methods = instanceType.GetMethods();

            foreach (var methodInfo in methods)
            {
                var methodInfoId = methodInfo.ToString().GetHashCode();
                if (methodId == methodInfoId)
                    return methodInfo;
            }

            return null;
        }

    }
}
