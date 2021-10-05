using MassTransit;
using System;

namespace CCC.CAS.Workflow3Messages.Messages
{
    public interface IWorkflow3Deleted : CorrelatedBy<Guid>
    {
        Guid Id { get; }
    }

    public class Workflow3Deleted : IWorkflow3Deleted
    {
        public Guid Id { get; set; } = Guid.Empty;

        public Guid CorrelationId { get; set; } = NewId.NextGuid();
    }
}
