using CCC.CAS.API.Common.Models;
using CCC.CAS.Workflow3Messages.Messages;
using CCC.CAS.Workflow3Messages.Models;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CCC.CAS.Workflow3Service.Consumers;
using CCC.CAS.Workflow3Service.Interfaces;

namespace UnitTest.ServiceBus
{
    public class Tests
    {
        private InMemoryTestHarness? _harness;
        private Mock<IWorkflow3Repository>? _mockRepo;
        private ConsumerTestHarness<SaveWorkflow3Consumer>? _consumer;
        private ConsumerTestHarness<GetWorkflow3Consumer>? _getConsumer;

        [SetUp]
        public async Task Setup()
        {
            _harness = new InMemoryTestHarness();
            _mockRepo = new Mock<IWorkflow3Repository>();
            _mockRepo.Setup(o => o.SaveWorkflow3Async(It.IsAny<CallerIdentity>(), It.IsAny<Workflow3>(), It.IsAny<Guid?>())).Returns(Task.FromResult<Workflow3?>(new Workflow3 { Name = "test", Id = NewId.NextGuid().ToString() }));
            _mockRepo.Setup(o => o.GetWorkflow3Async(It.IsAny<CallerIdentity>(), It.IsAny<string>())).Returns(Task.FromResult<Workflow3?>(new Workflow3 { Name = NewId.NextGuid().ToString() }));

            _consumer = _harness.Consumer(() => new SaveWorkflow3Consumer(new Mock<ILogger<SaveWorkflow3Consumer>>().Object, _mockRepo.Object));
            _getConsumer = _harness.Consumer(() => new GetWorkflow3Consumer(new Mock<ILogger<GetWorkflow3Consumer>>().Object, _mockRepo.Object));
            _harness.TestTimeout = TimeSpan.FromSeconds(2);
            await _harness.Start();
        }

        [TearDown]
        public async Task Teardown()
        {
            await _harness!.Stop();
        }

        [Test]
        public async Task TestSendSaveCommand()
        {
            _harness!.TestTimeout = TimeSpan.FromSeconds(2);
            await _harness.InputQueueSendEndpoint.Send(new SaveWorkflow3 { Workflow3 = new Workflow3 { Name = "test" } });
            var consumed = _harness.Consumed.Select<ISaveWorkflow3>().FirstOrDefault();
            _harness.TestTimeout = TimeSpan.FromSeconds(.1);
            if (await _harness.Published.Any(o => o.MessageType == typeof(Fault)))
            {
                var faults = _harness.Published.Select<Fault>().ToList();
                foreach (var f in faults)
                {
                    Debug.WriteLine(JsonSerializer.Serialize(f.Context.Message, new JsonSerializerOptions { WriteIndented = true }));
                }
            }
            (await _harness.Published.Any(o => o.MessageType == typeof(Fault))).ShouldBeFalse();
            consumed.ShouldNotBeNull();

            // did the endpoint consume the message
            (await _harness.Consumed.Any(o => o.MessageType == typeof(ISaveWorkflow3))).ShouldBeTrue();
            _harness.Consumed.Select<ISaveWorkflow3>().Any().ShouldBeTrue();

            // did the actual consumer consume the message
            _consumer!.Consumed.Select<ISaveWorkflow3>().Any().ShouldBeTrue();

            // did the consumer publish the event
            _harness.Published.Select<IWorkflow3Saved>().Any().ShouldBeTrue();
            (await _harness.Published.Any(o => o.MessageType == typeof(IWorkflow3Saved))).ShouldBeTrue();

            // did we call save?
            _mockRepo!.Verify(o => o.SaveWorkflow3Async(It.IsAny<CallerIdentity>(), It.IsAny<Workflow3>(), It.IsAny<Guid?>()), Times.Once());

        }

        [Test]
        public async Task TestSendSaveCommandFault()
        {
            await _harness!.InputQueueSendEndpoint.Send(new SaveWorkflow3()); // no name will throw

            // did the endpoint consume the message
            _harness.Consumed.Select<ISaveWorkflow3>().Any().ShouldBeTrue();

            // did the actual consumer consume the message
            _consumer!.Consumed.Select<ISaveWorkflow3>().Any().ShouldBeTrue();

            // did the consumer publish the event
            // hangs since didn't publish? _harness.Published.Select<Workflow3Saved>().Any().ShouldBeTrue();
            // but this works.
            (await _harness.Published.Any(o => o.MessageType == typeof(IWorkflow3Saved))).ShouldBeFalse();

            // did the consumer throw
            _harness.Published.Select<Fault<ISaveWorkflow3>>().Any().ShouldBeTrue();
        }

        [Test]
        public async Task TestGetWorkflow3()
        {
            _harness!.TestTimeout = TimeSpan.FromSeconds(2);
            await _harness.InputQueueSendEndpoint.Send<IGetWorkflow3>(new
            {
                Id = "AE000001",
                CorrelationId = NewId.NextGuid()
            }
            );

            _getConsumer!.Consumed.Select<IGetWorkflow3>().Any().ShouldBeTrue();
            var called = _harness.Consumed.Select<IGetWorkflow3>().Any();
            called.ShouldBeTrue();
            _mockRepo!.Verify(o => o.GetWorkflow3Async(It.IsAny<CallerIdentity>(), It.IsAny<string>()), Times.Once());

            (await _harness.Published.Any(o => o.MessageType == typeof(IGetWorkflow3Response))).ShouldBeTrue();
        }
    }
}
