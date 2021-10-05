using Amazon;
using Amazon.SimpleWorkflow;
using Amazon.SimpleWorkflow.Model;
using CCC.CAS.Workflow3Service.Activities;
using CCC.CAS.Workflow3Service.Workflows;
using Guflow;
using Guflow.Decider;
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

        public AwsWorkflowActivityService(IOptions<AwsWorkflowConfiguration> config, ILogger<AwsWorkflowDeciderService> logger)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            _config = config.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // ------------------------ init
                using var swfClient = new AmazonSimpleWorkflowClient(_config.AccessKey, _config.SecretKey, RegionEndpoint.GetBySystemName(_config.Region));
                var domain = new Domain(_config.Domain, swfClient);

                await domain.RegisterActivityAsync<PpoProcessorA>().ConfigureAwait(false);

                // -------------------- running it
                using var host = domain.Host(new[] { typeof(PpoProcessorA) });

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
    }
}

