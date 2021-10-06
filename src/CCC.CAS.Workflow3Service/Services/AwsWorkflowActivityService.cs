using CCC.CAS.Workflow3Service.Activities;
using Guflow;
using Guflow.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CCC.CAS.Workflow3Service.Services
{
    public class AwsWorkflowActivityService : BackgroundService
    {
        private readonly AwsWorkflowOptions _config;
        private readonly ILogger<AwsWorkflowDeciderService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly Domain _domain;
        private readonly Type[] _types = new[] { typeof(PpoProcessorA), typeof(PpoProcessorB), typeof(PpoProcessorC), typeof(PpoEnd) };

        public AwsWorkflowActivityService(IOptions<AwsWorkflowOptions> config, ILogger<AwsWorkflowDeciderService> logger, IServiceProvider serviceProvider, Domain domain)
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

                using var host = _domain.Host(_types, GetActivity);
                host.OnError(LogError);
                host.OnPollingError(LogError);
                host.OnResponseError(LogError);

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

        private async Task RegisterActivities(Domain domain)
        {
            var method = domain.GetType().GetMethod("RegisterActivityAsync", 0, new[] { typeof(Type) } );
            if (method != null) {
                foreach (var t in _types) {
                    var task = method.Invoke(domain, new[] { t }) as Task;
                    await task!.ConfigureAwait(false);
                }
            }
        }

        private Activity? GetActivity(Type activityType)
        {
            var o = _serviceProvider.GetService(activityType);
            if (o == null)
            {
                _logger.LogError("Didn't create {type} activity. Is it registered with DI?", activityType.Name);
            }
            var ret = o as Activity;
            if (ret == null)
            {
                _logger.LogError("Class {type} isn't derived from Activity", activityType.Name);
            }
            return ret;
        }

        private ErrorAction LogError(Error error)
        {
            _logger.LogError(error.Exception, nameof(AwsWorkflowDeciderService));
            return ErrorAction.Continue;
        }
    }
}

