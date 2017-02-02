namespace ConsoleApplication
{
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    class Program
    {
        static void Main(string[] args)
        {
            var crmBaseUrl = "https://xxxxxxxx.crm.dynamics.com/api/data/V8.2/";
            string applicationId = "23b056d1-0351-4346-9727-f12195b1a2c6";  //set to the application ID of your Azure Application
            string clientSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx="; //set to the value of the key you created for your application

            //retrieve authority url and resource
            //and then get the authentication token
            AuthenticationParameters ap = AuthenticationParameters.CreateFromResourceUrlAsync(new Uri(crmBaseUrl)).Result;
            Console.WriteLine($"Authority: {ap.Authority}");
            Console.WriteLine($"Resouce: {ap.Resource}");

            AuthenticationContext authContext = new AuthenticationContext(ap.Authority, false);
            Task<AuthenticationResult> taskResult = authContext.AcquireTokenAsync(ap.Resource, new ClientCredential(applicationId, clientSecret));
            AuthenticationResult result = taskResult.Result;

            string getAccountsWithNameContainingMicrosoft = $"{crmBaseUrl}accounts?$select=name&$filter=contains(name,%20%27Microsof%27)&$orderby=name%20desc&$top=10";
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = new TimeSpan(0, 2, 0);  // 2 minutes
                string authorizationHeader = result.CreateAuthorizationHeader();
                httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader);

                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, getAccountsWithNameContainingMicrosoft);
                req.Method = HttpMethod.Get;

                // Wait for the web service response.
                HttpResponseMessage response;
                response = httpClient.SendAsync(req).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseBodyAsText = response.Content.ReadAsStringAsync().Result;

                    Console.WriteLine(responseBodyAsText);
                }
                else
                {
                    Console.WriteLine(response.StatusCode + Environment.NewLine + response.ReasonPhrase);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
