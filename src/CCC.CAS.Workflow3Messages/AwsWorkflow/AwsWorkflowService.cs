using Microsoft.Extensions.Options;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Guflow;
using System.Collections.Generic;
using Guflow.Decider;

namespace CCC.CAS.Workflow3Messages.AwsWorkflow
{

    public abstract class AwsWorkflowService<T> : BackgroundService where T : class
    {
        protected readonly IOptions<AwsWorkflowOptions> _options;
        protected readonly AwsWorkflowOptions _config;
        protected readonly ILogger<AwsWorkflowDeciderService> _logger;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly Domain _domain;
        protected readonly List<Workflow> _workflows = new List<Workflow>();

        protected AwsWorkflowService(IOptions<AwsWorkflowOptions> config, ILogger<AwsWorkflowDeciderService> logger, IServiceProvider serviceProvider, Domain domain) 
        {
            _options = config ?? throw new ArgumentNullException(nameof(config));
            _config = config.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _domain = domain;
        }

        protected void SetLogger(Guflow.IHost host)
        {
            host.OnError(LogError);
            host.OnPollingError(LogError);
            host.OnResponseError(LogError);
        }

        protected T? GetService(Type type)
        {
            var o = _serviceProvider.GetService(type);
            if (o == null)
            {
                _logger.LogError("Didn't create {type}. Is it registered with DI?", type.Name);
            }
            var ret = o as T;
            if (ret == null)
            {
                _logger.LogError("Class {type} isn't derived from {parent}", type.Name, typeof(T).Name);
            }
            return ret;
        }

        protected ErrorAction LogError(Error error)
        {
            _logger.LogError(error.Exception, nameof(AwsWorkflowDeciderService));
            return ErrorAction.Continue;
        }
    }
}
