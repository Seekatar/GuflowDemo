using CCC.CAS.API.Common.ServiceBus;
using MassTransit;
using System;

namespace CCC.CAS.Workflow3Messages.Messages
{
    public interface IStartPpo : ICommandMessage
    {
        public static Uri QueueUri => new("queue:StartPpo?durable=true");

        string RequestId { get; }
        string ClientCode { get; }
        int ProfileId { get; }
        bool SendSignal { get; }
        bool PpoBConsume { get; }
    }

    public class StartPpo : IStartPpo
    {
        public Guid CorrelationId { get; set; } = NewId.NextGuid();
        public Uri MessageQueueUri => IStartPpo.QueueUri;

        public string RequestId { get; set; } = "";

        public string ClientCode { get; set; } = "";

        public int ProfileId { get; set; }

        public bool SendSignal { get; set; }

        public bool PpoBConsume { get; set; }
    }
}
