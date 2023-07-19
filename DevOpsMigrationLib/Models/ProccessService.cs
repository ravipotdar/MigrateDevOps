using DevOpsMigrationLib.Helpers;
using DevOpsMigrationLib.Service;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DevOpsMigrationLib.Models
{
    public class ProccessService : RestApiServiceBase
    {
        public ProccessService(HttpClient httpClient) : base(httpClient)
        {

        }

        public async Task<ProcessTypeResponse> GetProcesses()
        {
            ProcessTypeResponse processTemplates = new () { value = new List<ProcessType>() };
            try
            {
                HttpResponseMessage response = await this.GetRequest(null);

                var content = response.Content.ReadAsStringAsync().Result;
                processTemplates = CustomJsonHelper.GetDeserializedJson<ProcessTypeResponse>(content);

                if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(content))
                {
                    return processTemplates;
                }
                else
                {
                    Console.WriteLine("Failed to retrieve process types. Status code: " + response.StatusCode);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return processTemplates;
        }

        public async Task<HttpResponseMessage> ImportProcesse(HttpMethod httpMethod, string apiUrl, dynamic processType)
        {
            HttpMethod = httpMethod;
            ApiUrl = apiUrl;

            try
            {
                if (httpMethod == HttpMethod.Post)
                {
                    httpResponseMessage = await this.PostRequest(processType);
                }
                else
                {
                    httpResponseMessage = await this.PatchRequest(processType);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
           

            return httpResponseMessage;
        }
    }
}
