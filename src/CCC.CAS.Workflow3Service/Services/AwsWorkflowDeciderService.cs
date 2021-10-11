using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.Logging;
using Guflow;
using CCC.CAS.Workflow3Service.Workflows;
using Guflow.Decider;

namespace CCC.CAS.Workflow3Service.Services
{

    public class AwsWorkflowDeciderService : BackgroundService
    {
        private readonly IOptions<AwsWorkflowOptions> _opt;
        private readonly AwsWorkflowOptions _config;
        private readonly ILogger<AwsWorkflowDeciderService> _logger;
        private readonly AwsWorkflowRegistration _registration;
        private readonly Domain _domain;

        public AwsWorkflowDeciderService(IOptions<AwsWorkflowOptions> config, ILogger<AwsWorkflowDeciderService> logger, AwsWorkflowRegistration registration, Domain domain)
        {
            _opt = config ?? throw new ArgumentNullException(nameof(config));
            _config = config.Value;
            _logger = logger;
            _registration = registration;
            _domain = domain;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _registration.Register().ConfigureAwait(false);

                using var host = _domain.Host(new Workflow [] { new PpoWorkflow(), new PpoWorkflowSignaled(), new PpoWorkflowSignaledDynamic(), new MainWorkflow() });
                host.OnError(LogError);
                host.OnPollingError(LogError);
                host.OnResponseError(LogError);

                _logger.LogDebug($"{nameof(AwsWorkflowDeciderService)} polling");

                host.StartExecution();

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(10000, stoppingToken).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "In Decider");
            }
        }

        private ErrorAction LogError(Error error)
        {
            _logger.LogError(error.Exception, nameof(AwsWorkflowDeciderService));
            return ErrorAction.Continue;
        }
    }
}
