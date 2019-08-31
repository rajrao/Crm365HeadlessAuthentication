using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace MsalBasedCrmAuthenticationHeadless201
{
    class Program
    {
        /// <summary>
        /// This sample shows how to use an DelegatingHandler to handle the authentication piece
        /// making for cleaner code.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            var crmBaseUrl = "https://xxxxxxxx.crm.dynamics.com/";
            var tenantId = "00000000-0000-0000-0000-000000000000";
            //see https://blog.aggregatedintelligence.com/2017/02/headless-authentication-against-crm-365.html 
            //on getting applicationId and client secret

            string applicationId = "00000000-0000-0000-0000-000000000000";
            string clientSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx=";

            var confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(applicationId)
                .WithClientSecret(clientSecret)
                .WithTenantId(tenantId)
                .Build();

            string api = "api/data/V9.1/";

            var scopes = new List<string> {$"{crmBaseUrl}.default"};
            
            //.Net recommends to use a single instance of HttpClient for
            using (var httpClient = new HttpClient(new OAuthDelgatingHandler(confidentialClientApplication, scopes)))
            {
                httpClient.Timeout = new TimeSpan(0, 2, 0); // 2 minutes

                var crmApiCallHelper = new CrmWebApiCallHelper(httpClient);

                string url = $"{crmBaseUrl}{api}WhoAmI()";
                var data = await crmApiCallHelper.CallWebApiAndProcessResultASync(url);
                Console.WriteLine(data);

                url = $"{crmBaseUrl}{api}systemusers?$select=firstname,lastname";
                data = await crmApiCallHelper.CallWebApiAndProcessResultASync(url);
                Console.WriteLine(data);
            }

        }
    }
}
