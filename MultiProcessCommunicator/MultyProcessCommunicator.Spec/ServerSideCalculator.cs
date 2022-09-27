using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultyProcessCommunicator.Spec
{
    public class ServerSideCalculator : ICalculator
    {
        public LargeObject Concatenate(LargeObject a, LargeObject b)
        {
            return a.Concatenate(b);
        }

        public bool Connect(string host, int port)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public int Sum(int a, int b)
        {
            return a + b;
        }
    }
}
