using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WpfTranslator
{
    class AdmStsClient
    {
        //"https://datamarket.accesscontrol.windows.net/v2/OAuth2-13"
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string azureDatamarketAccessUri;
        private readonly string requestBody;

        public AdmAccessToken AccessToken { get; set; }

        public AdmStsClient(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.requestBody = "";
            //string.Format(
            //    "grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com",
            //    HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret));

            RefreshAccessToken();
        }

        public async Task<AdmAccessToken> RequestToken()
        {
            using (var httpClient = new HttpClient())
            {
                Func<Task<HttpResponseMessage>> request = () => httpClient
                    .PostAsync(azureDatamarketAccessUri, 
                        new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded"));

                var response = await request();
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    response = await request();
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new WpfTranslatorException(
                        $"Failed to get access token. StatusCode: {response.StatusCode}, Response: {await response.Content.ReadAsStringAsync()}.");
                }

                return JsonConvert.DeserializeObject<AdmAccessToken>(await response.Content.ReadAsStringAsync());
            }
        }

        private async void RefreshAccessToken()
        {
            this.AccessToken = await RequestToken();
        }
    }
}