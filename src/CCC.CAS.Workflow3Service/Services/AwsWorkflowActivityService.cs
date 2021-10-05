using Amazon;
using Amazon.SimpleWorkflow;
using Amazon.SimpleWorkflow.Model;
using CCC.CAS.Workflow3Service.Activities;
using CCC.CAS.Workflow3Service.Workflows;
using Guflow;
using Guflow.Decider;
using Guflow.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CCC.CAS.Workflow3Service.Services
{
    public class AwsWorkflowActivityService : BackgroundService
    {
        private readonly AwsWorkflowConfiguration _config;
        private readonly ILogger<AwsWorkflowDeciderService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly Domain _domain;

        public AwsWorkflowActivityService(IOptions<AwsWorkflowConfiguration> config, ILogger<AwsWorkflowDeciderService> logger, IServiceProvider serviceProvider, Domain domain)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            _config = config.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _domain = domain;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await RegisterActivities(_domain).ConfigureAwait(false);

                // -------------------- running it
                using var host = _domain.Host(new[] { typeof(PpoProcessorA), typeof(PpoProcessorB), typeof(PpoProcessorC) }, GetActivity);

                _logger.LogDebug($"{nameof(AwsWorkflowActivityService)} polling");

                host.StartExecution(new Guflow.TaskList(_config.DefaultTaskList));

                while (!stoppingToken.IsCancellationRequested)
                {
                    Thread.Sleep(10000);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,"In Activity");
            }
        }

        private static async Task RegisterActivities(Domain domain)
        {
            await domain.RegisterActivityAsync<PpoProcessorA>().ConfigureAwait(false);
            await domain.RegisterActivityAsync<PpoProcessorB>().ConfigureAwait(false);
            await domain.RegisterActivityAsync<PpoProcessorC>().ConfigureAwait(false);
        }

        private Activity? GetActivity(Type activityType)
        {
            var o = _serviceProvider.GetService(activityType);
            return  o as Activity;
        }
    }
}

