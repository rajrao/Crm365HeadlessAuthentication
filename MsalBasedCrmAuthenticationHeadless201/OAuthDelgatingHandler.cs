using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace MsalBasedCrmAuthenticationHeadless201
{
    public class OAuthDelgatingHandler : DelegatingHandler
    {
        protected IConfidentialClientApplication ConfidentialClient { get; private set; }
        protected List<string> Scopes { get; private set; }
        public OAuthDelgatingHandler(IConfidentialClientApplication confidentialClient, List<string> scopes)
            : base(new HttpClientHandler())
        {
            ConfidentialClient = confidentialClient;
            Scopes = scopes;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await ConfidentialClient
                .AcquireTokenForClient(Scopes)
                .ExecuteAsync(cancellationToken);
            string authorizationHeader = token.CreateAuthorizationHeader();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}