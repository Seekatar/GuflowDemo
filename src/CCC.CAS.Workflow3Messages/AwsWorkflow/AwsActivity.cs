using Guflow;
using Guflow.Decider;
using Guflow.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace CCC.CAS.AwsWorkflow
{
    public class AwsActivity : Activity 
    {
        private readonly AwsWorkflowOptions _config;
        private readonly ILogger _logger;
        private readonly Domain _domain;

        protected AwsWorkflowOptions Config => _config;
        protected ILogger Logger => _logger;
        protected Domain Domain => _domain;

        public static Identity ActivityIdentity<T>() where T : Activity
        {
            var desc = ActivityDescription.FindOn<T>();
            return Identity.New(desc.Name, desc.Version);
        }

        public AwsActivity(IOptions<AwsWorkflowOptions> config, ILogger logger, Domain domain)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            _config = config.Value;
            _logger = logger;
            _domain = domain;
        }
    }
}