using FluentAssertions;
using MultiProcessCommunicator;
using System;
using System.Diagnostics;
using TechTalk.SpecFlow;

namespace MultyProcessCommunicator.Spec.StepDefinitions
{
    [Binding]
    public sealed class ClientServerStepDefinitions
    {
        private readonly FeatureContext _featureContext;

        public ClientServerStepDefinitions(FeatureContext featureContext)
        {
            _featureContext = featureContext;   
        }


        [Given("create Server at port (.*)")]
        public void CreateServerAtPort(int port)
        {
            var server = new ServerSideCalculator();
            MpcManager.CreateServer<ICalculator>(server, port);
        }

        private bool _clientConnectionResult;
        private ICalculator _clientCalculator;

        [When(@"connect to Server at port (.*)")]
        public void WhenConnectToServerAtPort(int port)
        {
            _clientCalculator = MpcManager.CreateClient<ICalculator>();
            _clientConnectionResult = _clientCalculator.Connect("localhost", port);

            _featureContext["client"] = _clientCalculator;
        }

        [Then(@"connection result should be (.*)")]
        public void ThenConnectionResultShouldBe(bool result)
        {
            _clientConnectionResult.Should().Be(result);
        }

        private int _remoteSumResult;
        [Given(@"execute method Sum at Server side with parameters (.*) and (.*)")]
        public void GivenExecuteMethodSumAtServerSideWithParametersAnd(int p0, int p1)
        {
            var clientCalculator = _featureContext["client"] as ICalculator;

            _remoteSumResult = clientCalculator.Sum(p0, p1);
        }

        [Then(@"remote sum result should be (.*)")]
        public void ThenRemoteSumResultShouldBe(int p0)
        {
            _remoteSumResult.Should().Be(p0);
        }

        #region Performance test

        private int _inputBufferSize;
        private double _timesPerSecond;

        [Given(@"Set random input buffer size (.*) bytes")]
        public void GivenSetRandomInputBufferSizeBytes(int p0)
        {
            _inputBufferSize = p0;
        }

        [Given(@"Execute method Concatenate (.*) times")]
        public void GivenExecuteMethodConcatenateTimes(int times)
        {
            var clientCalculator = _featureContext["client"] as ICalculator;

            var stopwatcher = new Stopwatch();
            for(var i=0;i < times; i++)
            {
                var objectA = LargeObject.GenerateRandom(_inputBufferSize);
                var objectB = LargeObject.GenerateRandom(_inputBufferSize);
                stopwatcher.Start();
                var result = clientCalculator.Concatenate(objectA, objectB);
                stopwatcher.Stop();
            }

            _timesPerSecond = times / stopwatcher.Elapsed.TotalSeconds;
            Console.WriteLine($"Actual speed is {_timesPerSecond} executes per second");
        }

        [Then(@"Sped will more then (.*) executes per second")]
        public void ThenSpedWillMoreThenExecutesPerSecond(int p0)
        {
            _timesPerSecond.Should().BeGreaterThan(p0);
        }
        #endregion


    }
}
