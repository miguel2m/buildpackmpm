using AutoDischange.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        //GET Changeset
        //public static TfsModel GetChangeset(string changeset)
        //{
        //    TfsModel tfsReponse = new TfsModel();

        //    string url = BASE_URL + String.Format(URL_GET_CHANGESET, changeset)+ URL_GET_CHANGESET;

        //    return tfsReponse;
        //}
        
        //GET Changeset
        public static async Task<List<TfsItem>> GetChangeset(string changeset)
        {
                List <TfsItem> tfsReponse = new List<TfsItem>();

                string url = BASE_URL + string.Format(URL_GET_CHANGESET, changeset) + URL_VERSION;
                
                using (HttpClient client = new HttpClient())
                {

                    var byteArray = Encoding.ASCII.GetBytes($"SEGWIN\\miguelangel.medina:6qpt4zyxkac6n6vhhql4ha6qyjnmz6c5jmhx3d3bwszmmfzrl4gq");

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
                   
                        tfsReponse.Add(itemLocal.item);
                    }
                    
                }
            
            

            return tfsReponse;
        }

    }
}
