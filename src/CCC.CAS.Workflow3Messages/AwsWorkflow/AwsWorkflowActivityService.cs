using CCC.CAS.API.Common.Installers;
using Guflow;
using Guflow.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CCC.CAS.Workflow3Messages.AwsWorkflow
{
    public class AwsWorkflowActivityService : AwsWorkflowService<Activity>
    {
        private readonly List<Type> _activityTypes;

        public AwsWorkflowActivityService(IOptions<AwsWorkflowOptions> config, ILogger<AwsWorkflowDeciderService> logger, IServiceProvider serviceProvider, Domain domain) : base(config, logger, serviceProvider, domain)
        {
            _activityTypes = InstallerExtension.GetTypesInLoadedAssemblies<Activity>().Where(o => o.GetTypeInfo().GetCustomAttribute<ActivityDescriptionAttribute>() != null).ToList();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await RegisterActivities(_domain).ConfigureAwait(false);

                using var host = _domain.Host(_activityTypes, GetService);
                SetLogger(host);

                _logger.LogDebug($"{nameof(AwsWorkflowActivityService)} polling");

                host.StartExecution(new TaskList(_config.DefaultTaskList));

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(10000, stoppingToken).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "In Activity");
            }
        }

        private async Task RegisterActivities(Domain domain)
        {
            foreach (var t in _activityTypes)
            {
                await domain.RegisterActivityAsync(t).ConfigureAwait(false);
            }
        }
    }
}

