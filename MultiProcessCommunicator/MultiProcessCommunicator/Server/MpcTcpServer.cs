using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using MultiProcessCommunicator.Client;

namespace MultiProcessCommunicator.Server
{
    internal class MpcTcpServer
    {
        public MpcTcpServer(int port, ServerInstance serverContext)
        {
            this._serverPort = port;
            this._serverContext = serverContext;
        }

        private int _serverPort;
        private ServerInstance _serverContext;
        private bool _isStopped = true;
        private Socket _listener = null;

        public void Stop()
        {
            this._isStopped = true;
            this._listener.Close();
            this._listener.Dispose();
        }

        public async void StartServer()
        {
            var endpoint = new IPEndPoint(IPAddress.Any, this._serverPort);
            this._listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            this._listener.Bind(endpoint);
            this._listener.Listen(100);
            this._isStopped = false;

            while (!this._isStopped)
            {
                try
                {
                    var clientSocket = await this._listener.AcceptAsync();
                    var client = new ClientConnectionState(clientSocket, this._serverContext);

                    var receiveThread = new Thread(() => { client.Receive(); });
                    receiveThread.Start();
                }
                catch
                {
                }
            }

            this._listener.Close();

        }

    }
}
