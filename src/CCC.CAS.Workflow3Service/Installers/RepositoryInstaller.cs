using System;
using CCC.CAS.API.Common.Installers;
using CCC.CAS.API.Common.Logging;
using CCC.CAS.Workflow3Service.Interfaces;
using CCC.CAS.Workflow3Service.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CCC.CAS.Workflow3Service.Installers
{
    public class RepositoryInstaller : IInstaller
    {
        private readonly ILogger<RepositoryInstaller> _debugLogger;

        public RepositoryInstaller()
        {
            _debugLogger = DebuggingLoggerFactory.Create<RepositoryInstaller>();
        }

        public void InstallServices(IConfiguration configuration, IServiceCollection services)
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }

            try
            {
                services.AddTransient<IWorkflow3Repository, Workflow3Repository>();

                services.AddTransient<IEchoRepository, EchoRepository>();

                _debugLogger.LogDebug("DB services added.");
            }
            catch (Exception ex)
            {
                _debugLogger.LogError(ex, "Exception occurred while adding DB services.");
            }
        }
    }
}
