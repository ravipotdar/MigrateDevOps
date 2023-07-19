using DevOpsMigrationLib.Models;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;

namespace DevOpsMigrationLib.Services
{
    public interface IAzureDevOpsManager
    {
        HttpClient GetSourceHttpClient();
        HttpClient GetDestHttpClient();

        string GetDestOrgUrlConfig();

        string GetSourceOrgUrlConfig();

        string GetProcessesApiUrlConfig();
        string GetCreateProcessApiUrlConfig();

        List<GitRepository> GetGitRepos();

        List<PipelineEntity> GetPipelines();

        SprintEntity GetSprintData();

        void GetWorkItemsForSprint(SprintEntity sprintEntity);
    }
}