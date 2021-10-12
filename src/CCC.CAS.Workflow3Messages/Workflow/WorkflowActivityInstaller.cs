using System;
using Amazon;
using Amazon.SimpleWorkflow;
using CCC.CAS.API.Common.Installers;
using CCC.CAS.API.Common.Logging;
using CCC.CAS.Workflow3Service.Services;
using Guflow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Guflow.Worker;

namespace CCC.CAS.Workflow
{
    public class WorkflowActivityInstaller : IInstaller
    {
        private readonly ILogger<WorkflowActivityInstaller> _debugLogger;

        public WorkflowActivityInstaller()
        {
            _debugLogger = DebuggingLoggerFactory.Create<WorkflowActivityInstaller>();
        }

        public void InstallServices(IConfiguration configuration, IServiceCollection services)
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }

            try
            {
                services.AddHostedService<AwsWorkflowActivityService>();

                var section = configuration.GetSection(AwsWorkflowOptions.DefaultConfigName);
                var _config = section.Get<AwsWorkflowOptions>();

                services.AddSingleton((provider) => new AmazonSimpleWorkflowClient(_config.AccessKey, _config.SecretKey, RegionEndpoint.GetBySystemName(_config.Region)));

                services.AddSingleton((provider) => new Domain(_config.Domain, provider.GetRequiredService<AmazonSimpleWorkflowClient>()));

                services.AddOptions<AwsWorkflowOptions>()
                         .Bind(section)
                         .ValidateDataAnnotations();

                services.ForEachClass<Activity>((type, configuration, services) => services.AddTransient(type), configuration, "CCC.*");

                Log.Register(type => new GuflowLogger(type.Name, _debugLogger));

                _debugLogger.LogDebug("Services added.");
            }
            catch (Exception ex)
            {
                _debugLogger.LogError(ex, "Exception occurred while adding DB services.");
            }
        }
    }
}
