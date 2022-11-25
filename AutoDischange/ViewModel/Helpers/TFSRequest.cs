using AutoDischange.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AutoDischange.ViewModel.Helpers
{
    public class TFSRequest
    {
        //Root URL
        private static string BASE_URL = "http://tfs:8080/tfs/MPMTFS/";

        //API version
        private static string URL_VERSION = "?api-version=4.0";

        //API GET Changeset
        private static string URL_GET_CHANGESET = "_apis/tfvc/changesets/{0}/changes"; //{0} changeset ID

        //API GET Changeset autor - comments
        private static string URL_GET_DETAIL = "_apis/tfvc/changesets/{0}"; //{0} changeset ID

        //GET Changeset
        public static async Task<List<TfsItem>> GetChangeset(string changeset)
        {
            //ConfigurationManager.AppSettings["rutaJenkins"];

            List<TfsItem> tfsReponse = new List<TfsItem>();

            string ext = string.Empty;

            string url = BASE_URL + string.Format(URL_GET_CHANGESET, changeset) + URL_VERSION;
            string urlDetail = BASE_URL + string.Format(URL_GET_DETAIL, changeset) + URL_VERSION;

            using (HttpClient client = new HttpClient())
            {
                string credentialsTFS = $@"{ConfigurationManager.AppSettings["userTFS"]}:{ConfigurationManager.AppSettings["passTFS"]}";

                var byteArray = Encoding.ASCII.GetBytes(credentialsTFS);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(response.StatusCode.ToString());
                }
                string json = await response.Content.ReadAsStringAsync();
                TfsModel tfsReponseVar = (JsonConvert.DeserializeObject<TfsModel>(json));


                foreach (TfsValue itemLocal in tfsReponseVar.value)
                {
                    //NO SE ADMITE ARCHIVOS SIN EXTENSION O CON EXTENSION .CSPROJ
                    ext = Path.GetExtension(itemLocal.item.path);
                    if (ext != "")
                    {
                        itemLocal.item.changetype = itemLocal.changeType;
                        tfsReponse.Add(itemLocal.item);
                    }
                }
            }
            return tfsReponse;
        }

        //GET Changeset
        public static async Task<TfsModelDetail> GetChangesetAuthor(string changeset)
        {
            //ConfigurationManager.AppSettings["rutaJenkins"];
            TfsModelDetail tfsReponse = new TfsModelDetail();

            string urlDetail = BASE_URL + string.Format(URL_GET_DETAIL, changeset) + URL_VERSION;

            using (HttpClient client = new HttpClient())
            {
                string credentialsTFS = $@"{ConfigurationManager.AppSettings["userTFS"]}:{ConfigurationManager.AppSettings["passTFS"]}";

                var byteArray = Encoding.ASCII.GetBytes(credentialsTFS);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

               
                var responseDetail = await client.GetAsync(urlDetail);
                if (!responseDetail.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(responseDetail.StatusCode.ToString());
                }

                string jsonDetail = await responseDetail.Content.ReadAsStringAsync();
                TfsModelDetail tfsReponseVarDetail = (JsonConvert.DeserializeObject<TfsModelDetail>(jsonDetail));

                //Console.WriteLine(tfsReponseVarDetail);
                tfsReponse = tfsReponseVarDetail;
            }
            return tfsReponse;
        }
    }
}
