using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Guflow;
using Guflow.Decider;
using CCC.CAS.API.Common.Installers;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace CCC.CAS.AwsWorkflow
{

    public class AwsWorkflowDeciderService : AwsWorkflowService<Workflow>
    {
        List<Type> _workflowTypes;

        public AwsWorkflowDeciderService(IOptions<AwsWorkflowOptions> config, ILogger<AwsWorkflowDeciderService> logger, IServiceProvider serviceProvider, Domain domain) : base(config, logger, serviceProvider, domain)
        {
            _workflowTypes = InstallerExtension.GetTypesInLoadedAssemblies<Workflow>().Where(o => o.GetTypeInfo().GetCustomAttribute<WorkflowDescriptionAttribute>() != null).ToList();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await RegisterWorkflows(_domain).ConfigureAwait(false);

                using var host = _domain.Host(_workflows);
                SetLogger(host);

                _logger.LogDebug($"{nameof(AwsWorkflowDeciderService)} polling");

                host.StartExecution();

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(10000, stoppingToken).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "In Decider");
            }
        }

        private async Task RegisterWorkflows(Domain domain)
        {
            foreach (var t in _workflowTypes)
            {
                await domain.RegisterWorkflowAsync(t).ConfigureAwait(false);
                var wf = GetService(t);
                if (wf != null)
                {
                    _workflows.Add(wf);
                }
            }
        }
    }
}
