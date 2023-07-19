using DevOpsMigrationLib.Models;
using DevOpsMigrationLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOpsMigrationLib.Logic
{
    public class ProccessLogic
    {
        private readonly IAzureDevOpsManager devOpsManager;
        private ProccessService srcProccessService;
        private ProccessService destProccessService;

        ProcessTypeResponse sourceProcessTemplates;
        ProcessTypeResponse destinationProcessTemplates;
        public ProccessLogic(IAzureDevOpsManager? devOpsManager = null)
        {
            this.devOpsManager = new AzureDevOpsManager();
        }

        public async Task ManageProcesses()
        {
            await this.ExportProcesses();
            await this.ImportProcesses();
        }

        public async Task<ProcessTypeResponse> ExportProcesses()
        {
            srcProccessService = new ProccessService(devOpsManager.GetSourceHttpClient());
            srcProccessService.HttpMethod = HttpMethod.Get;
            srcProccessService.ApiUrl = devOpsManager.GetSourceOrgUrlConfig() + devOpsManager.GetProcessesApiUrlConfig();
            sourceProcessTemplates = await srcProccessService.GetProcesses();

            destProccessService = new ProccessService(devOpsManager.GetDestHttpClient());
            destProccessService.HttpMethod = HttpMethod.Get;
            destProccessService.ApiUrl = devOpsManager.GetDestOrgUrlConfig() + devOpsManager.GetProcessesApiUrlConfig();
            destinationProcessTemplates = await destProccessService.GetProcesses();
            return sourceProcessTemplates;
        }
        private async Task ImportProcesses()
        {
            if (sourceProcessTemplates != null)
            {
                foreach (var processType in sourceProcessTemplates.value.Where(p => !p.CustomizationType.Equals("system")))
                {
                    var destProcessType = destinationProcessTemplates?.value
                        .Where(a => !a.CustomizationType.Equals("system") && a.ReferenceName.Equals(processType.ReferenceName))
                        .FirstOrDefault();

                    if (destProcessType == null)
                    {
                        var processTypeId = destinationProcessTemplates?.value?
                                .Where(p => p.Name == sourceProcessTemplates?.value?
                                                 .Where(p => p.TypeId == processType.ParentProcessTypeId)
                                                 .FirstOrDefault()?
                                                 .Name)
                                .FirstOrDefault()?
                                .TypeId;

                        var apiUrl = devOpsManager.GetDestOrgUrlConfig() + devOpsManager.GetProcessesApiUrlConfig();
                        await destProccessService.ImportProcesse(HttpMethod.Post, apiUrl, PrepareNewProcessType(processType, processTypeId));
                    }
                    else
                    {
                        var apiUrl = string.Format(devOpsManager.GetCreateProcessApiUrlConfig(), destProcessType.TypeId);
                        await destProccessService.ImportProcesse(HttpMethod.Patch, apiUrl, PrepareUpdateProcessType(processType));
                    }
                }
            }
        }

        private static NewProcessType PrepareNewProcessType(ProcessType processType, string? processTypeId) => new ()
        {
            Name = processType.Name,
            Description = processType.Description,
            ParentProcessTypeId = processTypeId,
            ReferenceName = processType.ReferenceName
        };

        private static UpdateProcessType PrepareUpdateProcessType(ProcessType processType) => new ()
        {
            Name = processType.Name,
            Description = processType.Description,
            IsDefault = processType.IsDefault,
            IsEnabled = processType.IsEnabled
        };
    }
}
