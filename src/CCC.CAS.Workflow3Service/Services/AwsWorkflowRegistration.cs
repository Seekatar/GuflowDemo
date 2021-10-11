using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Guflow;
using CCC.CAS.Workflow3Service.Workflows;

namespace CCC.CAS.Workflow3Service.Services
{
    public class AwsWorkflowRegistration
    {
        private readonly AwsWorkflowOptions _config;
        private readonly ILogger<AwsWorkflowRegistration> _logger;
        private readonly Domain _domain;

        public AwsWorkflowRegistration(IOptions<AwsWorkflowOptions> config, ILogger<AwsWorkflowRegistration> logger, Domain domain)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            _config = config.Value;
            _logger = logger;
            _domain = domain;
        }

        public async Task Register()
        {
            await _domain.RegisterWorkflowAsync<PpoWorkflow>().ConfigureAwait(false);
            await _domain.RegisterWorkflowAsync<PpoWorkflowSignaled>().ConfigureAwait(false);
            await _domain.RegisterWorkflowAsync<PpoWorkflowSignaledDynamic>().ConfigureAwait(false);
            await _domain.RegisterWorkflowAsync<MainWorkflow>().ConfigureAwait(false);
        }
    }
}
