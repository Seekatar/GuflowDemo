using CCC.CAS.API.Common.Installers;
using Guflow.Decider;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace CCC.CAS.AwsWorkflow
{
    public class WorkflowDeciderInstaller : WorkflowInstaller<AwsWorkflowDeciderService>
    {
        protected override void RegisterWorkflowItems(AwsWorkflowOptions options, IServiceCollection services)
        {
            InstallerExtension.GetTypesInLoadedAssemblies<Workflow>()
                .Where(o => o.GetTypeInfo().GetCustomAttribute<WorkflowDescriptionAttribute>() != null)
                .ToList()
                .ForEach(type => services.AddSingleton(type));
        }
    }
}
