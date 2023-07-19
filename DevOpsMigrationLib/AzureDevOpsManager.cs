using DevOpsMigrationLib.Helpers;
using DevOpsMigrationLib.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DevOpsMigrationLib.Services
{
    public class AzureDevOpsManager : IAzureDevOpsManager
    {
        const string DEVOPS_ORG_URL = "https://dev.azure.com/{Your_DevOps_OrgURL}";
        private readonly IConfiguration _config;

        public AzureDevOpsManager()
        {

        }
        public AzureDevOpsManager(IConfiguration config)
        {
            this._config = config;
        }

        private string GetSourceTokenConfig()
        {
            return "OmJ1aXZ4ZnJyc2RheXBlM3U1bjV3amF5djU2NzZjczc1amxxbmxtb3V3ZHJhY2N0Z2d5eXE=";// "buivxfrrsdaype3u5n5wjayv5676cs75jlqnlmouwdracctggyyq";// _config.GetSection("AzureToken").Value;
        }

        public string GetSourceOrgUrlConfig()
        {
            return "https://dev.azure.com/HitachiSharedProjects";// _config.GetSection("AzureToken").Value;
        }

        public string GetDestOrgUrlConfig()
        {
            return "https://dev.azure.com/MigrateDevOps";// _config.GetSection("AzureToken").Value;
        }
        private string GetDestTokenConfig()
        {
            return "OmViamNjeG9lNzNzdzN1ejVyYWFxY2EyZ3k3ZzJjNGdpcHRibnliaHRsdGo3Y3R2Y3lkdnE=";// "ebjccxoe73sw3uz5raaqca2gy7g2c4giptbnybhtltj7ctvcydvq";// _config.GetSection("AzureToken").Value;
        }

        public string GetProcessesApiUrlConfig()
        {
            return "/_apis/work/processes?api-version=7.0";
        }

        public string GetCreateProcessApiUrlConfig()
        {
            return GetDestOrgUrlConfig() + "/_apis/work/processes/{0}?api-version=7.0";
        }


        public HttpClient GetSourceHttpClient()
        {
            // Set up HttpClient
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetSourceTokenConfig());// Set access token or personal access token (PAT)

            return httpClient;
        }

        public HttpClient GetDestHttpClient()
        {
            // Set up HttpClient
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetDestTokenConfig());// Set access token or personal access token (PAT)

            return httpClient;
        }






        private string GetProjectNameConfig()
        {
            return _config.GetSection("ProjectName").Value;
        }

        private VssConnection Authenticate()
        {
            string token = GetSourceTokenConfig();
            string projectName = GetProjectNameConfig();

            var credentials = new VssBasicCredential(string.Empty, token);
            var connection = new VssConnection(new Uri(DEVOPS_ORG_URL), credentials);

            return connection;
        }
        public List<GitRepository> GetGitRepos()
        {
            var conn = Authenticate();

            using (GitHttpClient gitClient = conn.GetClient<GitHttpClient>())
            {
                var allRepos = gitClient.GetRepositoriesAsync().Result;
                return allRepos;
            }
        }

        public List<PipelineEntity> GetPipelines()
        {
            List<PipelineEntity> pipelines = new List<PipelineEntity>();

            string tokenFormat = string.Format("{0}:{1}", "", GetSourceTokenConfig());
            string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(tokenFormat));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(DEVOPS_ORG_URL + "/" + GetProjectNameConfig() + "/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                HttpResponseMessage response = client.GetAsync("_apis/pipelines").Result;
                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = response.Content.ReadAsStringAsync().Result;
                    dynamic deserialzedJson = JsonConvert.DeserializeObject<ExpandoObject>(jsonResult, new ExpandoObjectConverter());

                    foreach (var item in (IEnumerable<dynamic>)deserialzedJson.value)
                    {
                        PipelineEntity entity = new PipelineEntity()
                        {
                            Name = item.name,
                            URL = item._links.web.href
                        };

                        pipelines.Add(entity);
                    }
                }

                return pipelines;
            }
        }

        public SprintEntity GetSprintData()
        {
            //https://dev.azure.com/{organization}/{project}/_apis/work/teamsettings/iterations?api-version=6.0

            string tokenFormat = string.Format("{0}:{1}", "", GetSourceTokenConfig());
            string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(tokenFormat));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(DEVOPS_ORG_URL + "/" + GetProjectNameConfig() + "/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                HttpResponseMessage response = client.GetAsync("_apis/work/teamsettings/iterations?api-version=6.0").Result;
                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = response.Content.ReadAsStringAsync().Result;
                    SprintEntity sprintEntity = CustomJsonHelper.GetDeserializedJson<SprintEntity>(jsonResult);

                    GetWorkItemsForSprint(sprintEntity);

                    return sprintEntity;
                }
            }

            return null;
        }

        public void GetWorkItemsForSprint(SprintEntity sprintEntity)
        {
            string tokenFormat = string.Format("{0}:{1}", "", GetSourceTokenConfig());
            string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(tokenFormat));

            foreach (Sprint sprintItem in sprintEntity.Sprints)
            {
                using (var client = new HttpClient())
                {
                    var workItemsUrl = sprintItem.Url + "/workitems";

                    client.BaseAddress = new Uri(DEVOPS_ORG_URL + "/" + GetProjectNameConfig() + "/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                    //Get List Of WorkItems Relations
                    HttpResponseMessage response = client.GetAsync(workItemsUrl).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var workItemRelationsJsonResult = response.Content.ReadAsStringAsync().Result;
                        WorkItemRelationsEntity workItemRelationsEntity = CustomJsonHelper.GetDeserializedJson<WorkItemRelationsEntity>(workItemRelationsJsonResult);

                        foreach (var item in workItemRelationsEntity.workItemRelations)
                        {
                            response = client.GetAsync(item.target.url).Result;
                            var workItemJsonResult = response.Content.ReadAsStringAsync().Result;
                            WorkItemEntity workItemEntity = CustomJsonHelper.GetDeserializedJson<WorkItemEntity>(workItemJsonResult);
                            workItemEntity.SprintName = sprintItem.Name;

                            sprintEntity.SprintWorkItems.Add(workItemEntity);
                        }
                    }
                }
            }
        }
    }
}
