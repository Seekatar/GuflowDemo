using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SimpleWorkflow;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Amazon;
using System.Text.Json;
using Amazon.SimpleWorkflow.Model;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Guflow;
using CCC.CAS.Workflow3Service.Workflows;
using Guflow.Decider;
using Guflow.Worker;
using CCC.CAS.Workflow3Service.Activities;

namespace CCC.CAS.Workflow3Service.Services
{

    public class AwsWorkflowDeciderService : BackgroundService
    {
        private readonly IOptions<AwsWorkflowConfiguration> _opt;
        private readonly AwsWorkflowConfiguration _config;
        private readonly ILogger<AwsWorkflowDeciderService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public AwsWorkflowDeciderService(IOptions<AwsWorkflowConfiguration> config, ILogger<AwsWorkflowDeciderService> logger, IServiceProvider serviceProvider)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            _opt = config;
            _config = config.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // ------------------------ init
                using var swfClient = new AmazonSimpleWorkflowClient(_config.AccessKey, _config.SecretKey, RegionEndpoint.GetBySystemName(_config.Region));
                var domain = new Domain(_config.Domain, swfClient);

                await domain.RegisterWorkflowAsync<PpoWorkflow>().ConfigureAwait(false);

                // -------------------- running it
                using var host = domain.Host(new[] { new PpoWorkflow() });

                _logger.LogDebug($"{nameof(AwsWorkflowDeciderService)} polling");

                host.StartExecution();

                bool started = false;
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (!started)
                    {
                        Thread.Sleep(1000);
                        // -------------------- start
                        var request = StartWorkflowRequest.For<PpoWorkflow>("workflowid");
                        request.TaskListName = _config.DefaultTaskList;
                        request.Input = new { SessionId = 1 };
                        await domain.StartWorkflowAsync(request).ConfigureAwait(false);
                        started = true;
                    }
                    Thread.Sleep(10000);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "In Decider");
            }
        }

        //{
        //        using var host = domain.Host(new[] { typeof(PpoProcessorA) }, GetActivity );

        //        _logger.LogDebug($"{nameof(AwsWorkflowDeciderService)} polling");

        //        host.StartExecution();

        //        bool started = false;
        //        while (!stoppingToken.IsCancellationRequested)
        //        {
        //            if (!started)
        //            {
        //                Thread.Sleep(1000);
        //                // -------------------- start
        //                var request = StartWorkflowRequest.For<PpoWorkflow>("workflowid");
        //                request.TaskListName = _config.DefaultTaskList;
        //                request.Input = new { SessionId = 1 };
        //                await domain.StartWorkflowAsync(request).ConfigureAwait(false);
        //                started = true;
        //            }
        //            Thread.Sleep(10000);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "In Decider");
        //    }
        //}

    }
}
