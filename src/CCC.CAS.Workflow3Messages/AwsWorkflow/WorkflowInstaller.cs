using System;
using Amazon;
using Amazon.SimpleWorkflow;
using CCC.CAS.API.Common.Installers;
using CCC.CAS.API.Common.Logging;
using Guflow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace CCC.CAS.AwsWorkflow
{
    public abstract class WorkflowInstaller<T> : IInstaller where T : class, IHostedService
    {
        protected readonly ILogger<WorkflowActivityInstaller> _debugLogger;

        public WorkflowInstaller()
        {
            _debugLogger = DebuggingLoggerFactory.Create<WorkflowActivityInstaller>();
        }

        public void InstallServices(IConfiguration configuration, IServiceCollection services)
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }

            try
            {
                var section = configuration.GetSection(AwsWorkflowOptions.DefaultConfigName);
                var options = section.Get<AwsWorkflowOptions>();
                if (options.InstallerName == GetType().Name)
                {
                    services.AddHostedService<T>();

                    services.AddSingleton((provider) => new AmazonSimpleWorkflowClient(options.AccessKey, options.SecretKey, RegionEndpoint.GetBySystemName(options.Region)));

                    services.AddSingleton((provider) => new Domain(options.Domain, provider.GetRequiredService<AmazonSimpleWorkflowClient>()));

                    services.AddOptions<AwsWorkflowOptions>()
                             .Bind(section)
                             .ValidateDataAnnotations();

                    RegisterWorkflowItems(options, services);

                    Log.Register(type => new GuflowLogger(type.Name, _debugLogger));

                    _debugLogger.LogInformation(options.ToString());
                }
                else
                {
                    _debugLogger.LogInformation($"{GetType().Name} not configured.");
                }
            }
            catch (Exception ex)
            {
                _debugLogger.LogError(ex, "Exception occurred while adding DB services.");
            }
        }

        protected abstract void RegisterWorkflowItems(AwsWorkflowOptions options, IServiceCollection services);
    }
}
