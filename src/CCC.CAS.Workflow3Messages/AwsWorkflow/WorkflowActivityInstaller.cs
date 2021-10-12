using CCC.CAS.API.Common.Installers;
using Microsoft.Extensions.DependencyInjection;
using Guflow.Worker;
using System.Reflection;
using System.Linq;

namespace CCC.CAS.AwsWorkflow
{

    public class WorkflowActivityInstaller : WorkflowInstaller<AwsWorkflowActivityService>
    {
        protected override void RegisterWorkflowItems(AwsWorkflowOptions options, IServiceCollection services)
        {
            InstallerExtension.GetTypesInLoadedAssemblies<Activity>()
                .Where(o => o.GetTypeInfo().GetCustomAttribute<ActivityDescriptionAttribute>() != null)
                .ToList()
                .ForEach(type => services.AddTransient(type));
        }
    }
}
