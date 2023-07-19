using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DevOpsMigrationLib.Service
{
    public class RestApiServiceBase
    {
        public HttpClient HttpClient { get; set; }
        public HttpMethod HttpMethod { get; set; } = HttpMethod.Get;
        public string ApiUrl { get; set; } = string.Empty;
        public HttpResponseMessage httpResponseMessage { get; set; } = new HttpResponseMessage();

        public RestApiServiceBase(HttpClient httpClient) {
            HttpClient = httpClient;
        }

        public HttpRequestMessage GetHttpRequest (dynamic requestJson)
        {
            HttpRequestMessage request = new()
            {
                Method = HttpMethod,
                RequestUri = new Uri(ApiUrl),
            };
            //request.Headers.Add("Authorization", "Basic OmJ1aXZ4ZnJyc2RheXBlM3U1bjV3amF5djU2NzZjczc1amxxbmxtb3V3ZHJhY2N0Z2d5eXE=");
            string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("buivxfrrsdaype3u5n5wjayv5676cs75jlqnlmouwdracctggyyq"));
            var scred = Convert.ToBase64String(Encoding.ASCII.GetBytes(":buivxfrrsdaype3u5n5wjayv5676cs75jlqnlmouwdracctggyyq"));
            if (requestJson != null)
            {
                string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(requestJson);

                // Create the request content with the updated process details
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                request.Content = content;
            }
            return request;

        } 

        public async Task<HttpResponseMessage> GetRequest(dynamic? requestType = null)
        {
            return await ExecuteRequest(requestType);
        }

        public async Task<HttpResponseMessage> PostRequest(dynamic requestType)
        {
            return await ExecuteRequest(requestType);
        }

        public async Task<HttpResponseMessage> PutRequest(dynamic requestType)
        {
            return await ExecuteRequest(requestType);
        }

        public async Task<HttpResponseMessage> PatchRequest(dynamic requestType)
        {
            return await ExecuteRequest(requestType);
        }

        public async Task<HttpResponseMessage> ExecuteRequest(dynamic requestType)
        {
            HttpResponseMessage response;
            try
            {
                var httpRequest = GetHttpRequest(requestType);
                response = await HttpClient.SendAsync(httpRequest);
            }
            catch (Exception ex)
            {

                throw;
            }


            return response;
        }

    }
}
