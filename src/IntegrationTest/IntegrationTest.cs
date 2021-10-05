using CCC.CAS.API.Common.Models;
using CCC.CAS.Workflow3Messages.Messages;
using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using CCC.CAS.Workflow3Messages.Models;

namespace IntegrationTest
{
    public class Tests
    {
        const string clientCode = "TestClient";
        const int profileId = 9999;
        private Sender? _sender;
        private string _workflow3Id = "123456";
        private string _name = "Fred";
        private readonly CallerIdentity _identity = new() { ClientCode = clientCode, ClientProfileId = profileId, Username = "testClient" };

        public Tests()
        {
            // not in Setup() since that gets called +1 times
            _sender = Sender.Initialize();

            // user _sender.Configuration to get test values that are environment-specific
        }

        [Test]
        public async Task GetWorkflow3Test()
        {
            var newWorkflow3 = await _sender!.SaveWorkflow3(new SaveWorkflow3 { Workflow3 = new Workflow3 { Name = _name, Description = "From test" } }, _identity).ConfigureAwait(false);
            newWorkflow3.ShouldNotBeNull();
            var workflow3 = (await _sender!.GetWorkflow3(new GetWorkflow3 { Id = newWorkflow3!.Id }, _identity).ConfigureAwait(false));
            workflow3.ShouldNotBeNull();
            workflow3!.Id.ShouldBe(newWorkflow3.Id);
            workflow3.Name.ShouldBe(_name);
        }

        [Test]
        public async Task GetEchoTest()
        {
            var echo = (await _sender!.GetEcho(new GetEcho { Name = _name }, _identity).ConfigureAwait(false))?.Parm;
            echo.ShouldNotBeNull();
            echo!.Message.ShouldNotBeEmpty();
            echo!.Name.ShouldBe(_name);
        }

        [Test]
        public async Task SaveWorkflow3Test()
        {
            var workflow3 = await _sender!.SaveWorkflow3(new SaveWorkflow3 { Workflow3 = new Workflow3 { Name = _name, Description = "From test" } }, _identity).ConfigureAwait(false);
            workflow3.ShouldNotBeNull();
            workflow3!.Id.ShouldNotBeNull();
            workflow3.Name.ShouldBe(_name);

            IntegrationTestWorkflow3SavedConsumer.WaitForMessage(TimeSpan.FromSeconds(3)).ShouldBeTrue();
        }

        [Test]
        public async Task DeleteWorkflow3Test()
        {
            await _sender!.DeleteWorkflow3(new DeleteWorkflow3 { Id = _workflow3Id }, _identity).ConfigureAwait(false);

            IntegrationTestWorkflow3DeletedConsumer.WaitForMessage(TimeSpan.FromSeconds(3)).ShouldBeTrue();
        }
    }
}