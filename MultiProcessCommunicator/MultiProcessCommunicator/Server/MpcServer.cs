using System;

namespace MultiProcessCommunicator.Server
{
    public class MpcServer
    {
        internal MpcServer(Action onClose)
        {
            _onClose = onClose;
        }

        private Action _onClose;

        public void Stop()
        {
            this._onClose();
        }

    }
}
