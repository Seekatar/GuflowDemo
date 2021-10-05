using CCC.CAS.Workflow3Messages.Models;
using System.Threading.Tasks;
using CCC.CAS.API.Common.Models;
using Microsoft.Extensions.Logging;
using MassTransit;
using CCC.CAS.Workflow3Service.Interfaces;
using System;
using System.Collections.Generic;
using CCC.CAS.API.Common.Logging;

namespace CCC.CAS.Workflow3Service.Repositories
{

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    /// <summary>
    /// repository for service that hits its domain database
    /// </summary>
    class Workflow3Repository : IWorkflow3Repository
    {
        private readonly ILogger<Workflow3Repository> _logger;
        // TODO: inject you database/storage here
        public Workflow3Repository(ILogger<Workflow3Repository> logger)
        {
            _logger = logger;
        }

        public async Task DeleteWorkflow3Async(CallerIdentity identity, string workflow3Id)
        {
            if (string.IsNullOrEmpty(workflow3Id)) { throw new ArgumentNullException(nameof(workflow3Id)); }
            if (identity == null) { throw new ArgumentNullException(nameof(identity)); }

            // TODO add your code to delete here
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task<List<Workflow3>> GetWorkflow3ByNameAsync(CallerIdentity identity, string name)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }
            if (identity == null) { throw new ArgumentNullException(nameof(identity)); }

            // TODO add your code to get here
            _logger.LogInformation("TODO, create message and consumer to call this");
            return await Task.FromResult(new List<Workflow3>()).ConfigureAwait(false);
        }

        public async Task<Workflow3?> GetWorkflow3Async(CallerIdentity identity, string workflow3Id)
        {
            if (string.IsNullOrEmpty(workflow3Id)) { throw new ArgumentNullException(nameof(workflow3Id)); }
            if (identity == null) { throw new ArgumentNullException(nameof(identity)); }

            // TODO add your code to get here
            return await Task.FromResult<Workflow3?>(new Workflow3() { Name = "Fred", Id = workflow3Id }).ConfigureAwait(false);
        }

        public async Task<Workflow3?> SaveWorkflow3Async(CallerIdentity identity, Workflow3 item, Guid? correlationId)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            if (identity == null) { throw new ArgumentNullException(nameof(identity)); }
            if (string.IsNullOrWhiteSpace(identity.Username)) { throw new ArgumentException("identity.Username must be set"); }

            bool isAdd = string.IsNullOrEmpty(item.Id);
            if (isAdd)
            {
                item.Id = NewId.NextGuid().ToString();
            }

            // TODO add your code to save here
            return await Task.FromResult<Workflow3?>(item).ConfigureAwait(false);
        }
    }
#pragma warning restore CA1812

}
