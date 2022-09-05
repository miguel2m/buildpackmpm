using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoDischange.ViewModel.Helpers
{
    public class DISChangeRequest
    {
        //Root URL
        private static string BASE_URL = "http://tfs:8080/tfs/MPMTFS/";

        //API version
        private static string URL_VERSION = "?api-version=4.0";

        //API GET Changeset
        private static string URL_GET_CHANGESET = "_apis/tfvc/changesets/{0}/changes"; //{0} changeset ID

        //Client credentials provider
        public static async void DischangeGraphClientAsync()
        {
            // The client credentials flow requires that you request the
            // /.default scope, and preconfigure your permissions on the
            // app registration in Azure. An administrator must grant consent
            // to those permissions beforehand.
            //var scopes = new[] { "User.Read", "Files.ReadWrite" };
            var scopes = new[] { "https://graph.microsoft.com/.default" };

            // Multi-tenant apps can use "common",
            // single-tenant apps must use the tenant ID from the Azure portal
            var tenantId = "9e1dccef-5501-4d13-bffd-130f3fe62dfc";

            // Values from app registration
            var clientId = "41dbd95a-644e-4866-b0c8-ecfb366e59b1";
            var clientSecret = "7qv8Q~UqVZq2DuzvNv972UHHiv_t5mMhnnvM3c4C";

            // Multi-tenant apps can use "common",
            // single-tenant apps must use the tenant ID from the Azure portal
            //var tenantId = "479a4cf6-50f0-4b4c-a7e3-24cc0acc9b0c";

            // Values from app registration
            //var clientId = "378c7483-4478-40a4-b2da-d12421968f30";
            //var clientSecret = "DQY8Q~6Xlp.QWfWuRU76RLpyeSGgaYJd6YByeaEZ";

            // using Azure.Identity;
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            //https://docs.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);


            // The client credentials flow requires that you request the
            // /.default scope, and preconfigure your permissions on the
            // app registration in Azure. An administrator must grant consent
            // to those permissions beforehand.
            //var scopes = new[] { "https://graph.microsoft.com/.default" };

            //// Multi-tenant apps can use "common",
            //// single-tenant apps must use the tenant ID from the Azure portal
            //var tenantId = "479a4cf6-50f0-4b4c-a7e3-24cc0acc9b0c";

            //// Values from app registration
            //var clientId = "378c7483-4478-40a4-b2da-d12421968f30";
            //var clientSecret = "DQY8Q~6Xlp.QWfWuRU76RLpyeSGgaYJd6YByeaEZ";

            //// using Azure.Identity;
            //var options = new TokenCredentialOptions
            //{
            //    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            //};

            //// https://docs.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
            //var clientSecretCredential = new ClientSecretCredential(
            //    tenantId, clientId, clientSecret, options);

            //var graphClient = new GraphServiceClient(clientSecretCredential, scopes);



            try
            {
                //var rows = await graphClient
                //.Users["{99cdbfec-35f4-462e-bae2-c4e27a90a3f6}"]
                //.Drive
                //.Items["{ED6F63845B1BF544!452}"]
                //.Workbook
                //.Worksheets["{18789F74-420A-4B82-9558-2C4E90E637B5}"]
                //.Tables["{5C2127C3-6F49-4969-B255-CC2A10F09BDA}"]
                //.Rows
                //.Request()
                //.Skip(5)
                //.Top(5)
                //.GetAsync();

                var rows = await graphClient
                .Users["{0443360f-4ed8-4ba6-86bb-c8d8b4652529}"]
                .Drive
                .Items["{016N7AF7N7376NVRCNKBEIY7OZFFJC35WJ}"]
                .Workbook
                .Worksheets["{2962B4F3-22EE-4CA3-B99D-D11C378DDAF0}"]
                .Tables["{885B09EA-6A5E-4B9C-87C0-AE1C5007DE62}"]
                .Rows
                .Request()
                .Skip(5)
                .Top(5)
                .GetAsync();

                Console.WriteLine(rows.ToString());
            } catch (Exception ex) {
                Console.WriteLine(ex);
            };
            


           //return rows;
        }
    }
}
