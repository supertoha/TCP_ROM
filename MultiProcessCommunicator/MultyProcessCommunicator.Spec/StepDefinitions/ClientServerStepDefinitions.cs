using FluentAssertions;
using MultiProcessCommunicator;
using System;
using TechTalk.SpecFlow;

namespace MultyProcessCommunicator.Spec.StepDefinitions
{
    [Binding]
    public sealed class ClientServerStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;

        public ClientServerStepDefinitions(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _scenarioContext = scenarioContext;
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




    }
}
