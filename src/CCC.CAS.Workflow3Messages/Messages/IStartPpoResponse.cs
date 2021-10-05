using CCC.CAS.Workflow3Messages.Models;
using MassTransit;
using System;

namespace CCC.CAS.Workflow3Messages.Messages
{
    public interface IStartPpoResponse : CorrelatedBy<Guid>
    {
        StartPpoResponse? Echo { get; }
    }

    public class StartPpoResponse : IStartPpoResponse
    {
        public Guid CorrelationId { get; } = NewId.NextGuid();
        public StartPpoResponse? Echo { get; set; }
    }
}
