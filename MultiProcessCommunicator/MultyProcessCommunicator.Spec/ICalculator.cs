using MultiProcessCommunicator;

namespace MultyProcessCommunicator.Spec
{
    /// <summary>
    /// interface definition for Client and Server
    /// </summary>
    public interface ICalculator : IMpcService
    {
        int Sum(int a, int b);

        LargeObject Concatenate(LargeObject a, LargeObject b);
    }

}
