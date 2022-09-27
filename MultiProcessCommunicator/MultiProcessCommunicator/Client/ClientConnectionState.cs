using MultiProcessCommunicator.Server;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiProcessCommunicator.Client
{
    public class ClientConnectionState
    {
        public ClientConnectionState(Socket clientSocket, ServerInstance serverContext)
        {
            this._clientSocket = clientSocket;
            this._serverContext = serverContext;
        }

        private readonly Socket _clientSocket;

        private readonly ServerInstance _serverContext;

        public void Receive()
        {
            byte[] packetBuffer = null;

            var markerBuffer = new byte[4];
            while (this._clientSocket.Connected)
            {
                try
                {
                    var sizeMarkerBytesRead = this._clientSocket.Receive(markerBuffer, 4, SocketFlags.None);

                    if (sizeMarkerBytesRead > 0)
                    {
                        var packetIndex = 0;
                        var packetLen = BitConverter.ToInt32(markerBuffer, 0);

                        if (packetLen <= 0)
                            continue;

                        if (packetBuffer == null || packetBuffer.Length < packetLen)
                            packetBuffer = new byte[packetLen];

                        Buffer.BlockCopy(markerBuffer, 0, packetBuffer, 0, 4);
                        packetIndex += 4;

                        while (packetLen > packetIndex)
                        {
                            var maxExpectedDataSize = packetLen - packetIndex;
                            var bytesRead = this._clientSocket.Receive(packetBuffer, packetIndex, maxExpectedDataSize, SocketFlags.None);

                            packetIndex += bytesRead;
                        }

                        this._serverContext.PushClientRequestToCollection(packetBuffer, packetLen, this._clientSocket);
                        //var task = new Task(() => this._serverContext.PushClientRequestToCollection(packetBuffer, packetLen, this._clientSocket));
                        //task.Start();                    
                    }
                    else
                    {
                        //Thread.Sleep(1);
                    }
                }
                catch (SocketException)
                {
                }
            }

        }
    }
}
