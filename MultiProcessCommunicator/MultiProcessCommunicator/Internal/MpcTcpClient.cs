using MultiProcessCommunicator.Client;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MultiProcessCommunicator.Internal
{
    internal class MpcTcpClient
    {
        public MpcTcpClient(ClientDecorator mpcObject)
        {
            this._mpcObject = mpcObject;
        }

        protected ClientDecorator _mpcObject;
        protected bool _isConnected = false;
        protected Socket _socket = null;
        public DateTime LastRequestTime { get; set; }

        public bool IsConnected
        {
            get { return this._isConnected; }
        }

        public void Disconnect()
        {
            this._isConnected = false;

            //if (IsConnected)
            this._socket.Shutdown(SocketShutdown.Both);

            this._socket.Close();
        }

        public bool Connect(string server, int port)
        {
            try
            {
                this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this._socket.Connect(server, port);
                StartReceive();
            }
            catch (Exception exc)
            {
                this._isConnected = false;
                return false;
            }

            return true;
        }

        protected void StartReceive()
        {
            var thread = new Thread(ReceiveThread);
            thread.Priority = ThreadPriority.AboveNormal;
            thread.Start();
        }

        protected void ReceiveThread(object state)
        {
            CheckConnection();
            var startBuff = new byte[4];
            byte[] packetBuff = null;
            while (this._isConnected)
            {
                try
                {

                    var markerRead = this._socket.Receive(startBuff);

                    if (markerRead < 4)
                        continue;

                    var packetLen = BitConverter.ToInt32(startBuff, 0);

                    if (packetLen < 4)
                        continue;

                    if (packetBuff == null || packetBuff.Length < packetLen)
                        packetBuff = new byte[packetLen];

                    Buffer.BlockCopy(startBuff, 0, packetBuff, 0, 4);
                    var offset = 4;

                    while (offset < packetLen)
                    {
                        var toReadLen = packetLen - offset;
                        var partLen = this._socket.Receive(packetBuff, offset, toReadLen, SocketFlags.None);
                        offset += partLen;
                    }

                    try
                    {
                        this._mpcObject.PushServerResponceToCollection(packetBuff, packetLen);
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine("Exception:" + exc.Message);
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"fatal receive exception {exc}");
                    CheckConnection();
                    return;
                }
                finally
                {

                }
            }
        }

        protected void CheckConnection()
        {
            this._isConnected = this._socket == null ? false : this._socket.Connected;
        }

        public bool SendMessage(byte[] inputMessage)
        {
            try
            {
                var sendData = this._socket.Send(inputMessage, SocketFlags.None);

                if (inputMessage.Length != sendData)
                    Console.WriteLine("not all data sent");

                LastRequestTime = DateTime.Now;

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"sending data to server exception {e}");
                CheckConnection();
                return false;
            }
        }
    }
}
