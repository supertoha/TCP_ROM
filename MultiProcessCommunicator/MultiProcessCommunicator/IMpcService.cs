using System;
using System.Collections.Generic;
using System.Text;

namespace MultiProcessCommunicator
{
    public interface IMpcService
    {
        bool Connect(string host, int port);

        void Disconnect();

    }
}
