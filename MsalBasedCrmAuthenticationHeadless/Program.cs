namespace MsalBasedCrmAuthenticationHeadless
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using Microsoft.Identity.Client;

    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var crmBaseUrl = "https://xxxxxxxx.crm.dynamics.com/";
            var tenantId = "00000000-0000-0000-0000-000000000000";
            string applicationId = "00000000-0000-0000-0000-000000000000";
            string clientSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx=";

            var app = ConfidentialClientApplicationBuilder.Create(applicationId)
                .WithClientSecret(clientSecret)
                .WithTenantId(tenantId)
                .Build();

            string api = "api/data/V9.1/";

            var token = await app
                .AcquireTokenForClient(new List<string> { $"{crmBaseUrl}.default" })
                .ExecuteAsync();

            Console.WriteLine(token.AccessToken);
            Console.WriteLine(token.ExpiresOn);
            Console.WriteLine(token.ExpiresOn.ToLocalTime());

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = new TimeSpan(0, 2, 0);  // 2 minutes
                string authorizationHeader = token.CreateAuthorizationHeader();
                httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader);

                string url = $"{crmBaseUrl}{api}WhoAmI()";//$"{crmBaseUrl}{api}systemusers?$select=firstname,lastname
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, url);
                req.Method = HttpMethod.Get;

                // Wait for the web service response.
                HttpResponseMessage response;
                Stopwatch stopwatch = Stopwatch.StartNew();
                response = httpClient.SendAsync(req).Result;
                if (response.IsSuccessStatusCode)
                {
                    stopwatch.Restart();
                    var responseBodyAsText = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(responseBodyAsText);
                }
                else
                {
                    Console.WriteLine(response.StatusCode + Environment.NewLine + response.ReasonPhrase);
                }
            }
        }
    }
}
