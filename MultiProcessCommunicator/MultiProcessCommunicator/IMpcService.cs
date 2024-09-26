using System;

namespace MultiProcessCommunicator
{
    public interface IMpcService
    {
        bool Connect(string host, int port);

        void Disconnect();

    }
}
