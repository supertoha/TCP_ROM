using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using MultiProcessCommunicator.Client;

namespace MultiProcessCommunicator.Server
{
    public class MpcTcpServer
    {
        public MpcTcpServer(int port, ServerInstance serverContext)
        {
            this._serverPort = port;
            this._serverContext = serverContext;
        }

        private int _serverPort;
        private ServerInstance _serverContext;
        private bool _isStopped = true;
        private Socket _listner = null;

        public void Stop()
        {
            this._isStopped = true;
            this._listner.Close();
            this._listner.Dispose();
        }

        public async void StartServer()
        {
            var endpoint = new IPEndPoint(IPAddress.Any, this._serverPort);
            this._listner = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            this._listner.Bind(endpoint);
            this._listner.Listen(100);
            this._isStopped = false;

            while (!this._isStopped)
            {
                try
                {
                    var clientSocket = await this._listner.AcceptAsync();
                    var client = new ClientConnectionState(clientSocket, this._serverContext);

                    var receiveThread = new Thread(() => { client.Receive(); });
                    receiveThread.Start();
                    //Task.Run(() => client.Receive());
                }
                catch
                {
                }
            }

            this._listner.Close();

        }

    }
}
