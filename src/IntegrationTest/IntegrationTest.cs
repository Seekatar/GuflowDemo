using CCC.CAS.API.Common.Models;
using CCC.CAS.Workflow3Messages.Messages;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace IntegrationTest
{
    public class Tests
    {
        const string clientCode = "TestClient";
        const int profileId = 9999;
        private readonly Sender? _sender;
        //private ActivityArgs argsworkflow2Id = "123456";
        private readonly string _name = "Fred";
        private readonly CallerIdentity _identity = new() { ClientCode = clientCode, ClientProfileId = profileId, Username = "testClient" };

        public Tests()
        {
            // not in Setup() since that gets called +1 times
            _sender = Sender.Initialize();

            // user _sender.Configuration to get test values that are environment-specific
        }

        [Test]
        public async Task StartPpoTest()
        {
            var echo = (await _sender!.StartPpo(new StartPpo { ClientCode = _name }, _identity).ConfigureAwait(false))?.Parm;
            echo.ShouldNotBeNull();
            echo!.Message.ShouldNotBeEmpty();
            echo!.Name.ShouldBe(_name);
        }
    }
}