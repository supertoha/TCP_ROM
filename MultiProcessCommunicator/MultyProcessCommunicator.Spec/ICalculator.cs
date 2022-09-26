using MultiProcessCommunicator;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultyProcessCommunicator.Spec
{
    public interface ICalculator : IMpcService
    {
        int Sum(int a, int b);
    }
}
