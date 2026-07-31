using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;

namespace SmartCity.Core.Services
{
    public class PowerBIService
    {
        private readonly string clientId = "***";
        private readonly string clientSecret = "***";
        private readonly string tenantId = "***";
        private readonly Guid workspaceId = Guid.Parse("f289157d-ac08-4707-983f-ef8f2cefc28c");
        private readonly Guid reportId = Guid.Parse("0dd0b44c-db02-4e6d-82b2-e51930bac484");

        public async Task<EmbedToken> GetPowerBIEmbedToken()
        {
            string authorityUri = $"https://login.microsoftonline.com/{tenantId}";
            string resourceUri = "https://graph.microsoft.com/";
            string apiUrl = "https://api.powerbi.com/";

            var authenticationContext = new AuthenticationContext(authorityUri);
            var credential = new ClientCredential(clientId, clientSecret);
            var authResult = await authenticationContext.AcquireTokenAsync(resourceUri, credential);

            using (var client = new PowerBIClient(new Uri(apiUrl), new Microsoft.Rest.TokenCredentials(authResult.AccessToken)))
            {

     

                var reports = await client.Reports.GetReportsInGroupAsync(workspaceId);
                var report = reports.Value.FirstOrDefault(r => r.Id == reportId);





                if (report == null)
                    throw new Exception("Report not found");

                var generateTokenRequestParameters = new GenerateTokenRequest(TokenAccessLevel.View);
                var embedToken = await client.Reports.GenerateTokenInGroupAsync(workspaceId, report.Id, generateTokenRequestParameters);

                return embedToken;
            }
        }
    }
}
