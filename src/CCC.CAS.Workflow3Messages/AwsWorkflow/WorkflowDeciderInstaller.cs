using CCC.CAS.API.Common.Installers;
using Guflow.Decider;
using Microsoft.Extensions.DependencyInjection;

namespace CCC.CAS.Workflow3Messages.AwsWorkflow
{
    public class WorkflowDeciderInstaller : WorkflowInstaller<AwsWorkflowDeciderService>
    {
        protected override void RegisterWorkflowItems(AwsWorkflowOptions options, IServiceCollection services)
        {
            InstallerExtension.ForEachClassInLoadedAssemblies<Workflow>((type) => services.AddSingleton(type));
        }
    }
}
