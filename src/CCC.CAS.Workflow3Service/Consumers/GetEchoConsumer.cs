using CCC.CAS.Workflow3Messages.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using CCC.CAS.API.Common.ServiceBus;
using CCC.CAS.Workflow3Service.Interfaces;
using System.Web;
using CCC.CAS.API.Common.Logging;

namespace CCC.CAS.Workflow3Service.Consumers
{
    public class GetEchoConsumer : IConsumer<IGetEcho>
    {
        private readonly ILogger<GetEchoConsumer> _logger;
        private readonly IEchoRepository _workflow3Repository;

        public GetEchoConsumer(ILogger<GetEchoConsumer> logger, IEchoRepository EchoRepository)
        {
            _logger = logger;
            _workflow3Repository = EchoRepository;
        }

        public async Task Consume(ConsumeContext<IGetEcho> context)
        {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }

            _logger.LogInformation(context.CorrelationId, "GetEchoConsumer: got {name}", context.Message.Name);

            // name shouldn't be anything too wacky since we pass it in the url's path
            var Echo = await _workflow3Repository.GetEchoAsync(context.GetIdentity(), HttpUtility.UrlEncode(context.Message.Name)).ConfigureAwait(false);

            await context.RespondAsync<IGetEchoResponse>(new GetEchoResponse { Echo = Echo })
                .ConfigureAwait(false);
        }
    }
}
