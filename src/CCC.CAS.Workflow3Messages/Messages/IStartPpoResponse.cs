using MassTransit;
using System;

namespace CCC.CAS.Workflow3Messages.Messages
{
    public interface IStartPpoResponse : CorrelatedBy<Guid>
    {
        bool Started { get; }
    }

    public class StartPpoResponse : IStartPpoResponse
    {
        public Guid CorrelationId { get; } = NewId.NextGuid();
        public bool Started { get; set; }
    }
}
