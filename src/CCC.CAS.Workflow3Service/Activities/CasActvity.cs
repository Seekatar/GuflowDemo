using CCC.CAS.Workflow3Messages.AwsWorkflow;
using CCC.CAS.Workflow3Service.Services;
using Guflow;
using Guflow.Decider;
using Guflow.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace CCC.CAS.Workflow3Service.Activities
{
    public class CasActvity<T> : Activity where T : Activity
    {
        private readonly AwsWorkflowOptions _config;
        private readonly ILogger<T> _logger;
        private readonly Domain _domain;

        protected AwsWorkflowOptions Config => _config;
        protected ILogger<T> Logger => _logger;
        protected Domain Domain => _domain;

#pragma warning disable CA1000 // Do not declare static members on generic types
        public static Identity Identity
        {
            get
            {
                var desc = ActivityDescription.FindOn<T>();
                return Identity.New(desc.Name, desc.Version);
            }
        }
#pragma warning restore CA1000 // Do not declare static members on generic types

        public CasActvity(IOptions<AwsWorkflowOptions> config, ILogger<T> logger, Domain domain)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            _config = config.Value;
            _logger = logger;
            _domain = domain;
        }
    }
}