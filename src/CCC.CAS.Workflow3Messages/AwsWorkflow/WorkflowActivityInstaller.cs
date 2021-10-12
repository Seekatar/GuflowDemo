using CCC.CAS.API.Common.Installers;
using Microsoft.Extensions.DependencyInjection;
using Guflow.Worker;

namespace CCC.CAS.Workflow3Messages.AwsWorkflow
{

    public class WorkflowActivityInstaller : WorkflowInstaller<AwsWorkflowActivityService>
    {
        protected override void RegisterWorkflowItems(AwsWorkflowOptions options, IServiceCollection services)
        {
            InstallerExtension.ForEachClassInLoadedAssemblies<Activity>((type) => services.AddTransient(type));
        }
    }
}
