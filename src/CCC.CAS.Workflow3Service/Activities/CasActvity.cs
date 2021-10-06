using CCC.CAS.Workflow3Service.Services;
using Guflow;
using Guflow.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace CCC.CAS.Workflow3Service.Activities
{
    public class CasActvity<T> : Activity where T : class
    {
        private readonly AwsWorkflowOptions _config;
        private readonly ILogger<T> _logger;
        private readonly Domain _domain;

        protected AwsWorkflowOptions Config => _config;
        protected ILogger<T> Logger => _logger;
        protected Domain Domain => _domain;

        public CasActvity(IOptions<AwsWorkflowOptions> config, ILogger<T> logger, Domain domain)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            _config = config.Value;
            _logger = logger;
            _domain = domain;
        }
    }
}