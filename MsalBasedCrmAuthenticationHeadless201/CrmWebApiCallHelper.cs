using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MsalBasedCrmAuthenticationHeadless201
{
    public class CrmWebApiCallHelper
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClient">HttpClient used to call the protected API</param>
        public CrmWebApiCallHelper(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        protected HttpClient HttpClient { get; private set; }


        /// <summary>
        /// Calls the protected Web API and processes the result
        /// </summary>
        /// <param name="webApiUrl">Url of the Web API to call (supposed to return Json)</param>
        public async Task<JObject> CallWebApiAndProcessResultASync(string webApiUrl)
        {
            var defaultRequetHeaders = HttpClient.DefaultRequestHeaders;
            if (defaultRequetHeaders.Accept == null || defaultRequetHeaders.Accept.All(m => m.MediaType != "application/json"))
            {
                HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            HttpResponseMessage response = await HttpClient.GetAsync(webApiUrl);
            JObject result = null;
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject(json) as JObject;
            }
            else
            {
                string content = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to call {webApiUrl}. StatusCode: {response.StatusCode}. Message: {content}");
            }
            return result;
        }
    }
}