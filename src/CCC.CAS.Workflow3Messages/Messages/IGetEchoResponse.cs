using CCC.CAS.Workflow3Messages.Models;
using MassTransit;
using System;

namespace CCC.CAS.Workflow3Messages.Messages
{
    public interface IGetEchoResponse : CorrelatedBy<Guid>
    {
        EchoResponse? Echo { get; }
    }

    public class GetEchoResponse : IGetEchoResponse
    {
        public Guid CorrelationId { get; } = NewId.NextGuid();
        public EchoResponse? Echo { get; set; }
    }
}
