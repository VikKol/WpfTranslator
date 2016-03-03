using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WpfTranslator
{
    class AdmStsClient
    {
        private readonly string azureDatamarketAccessUri;
        private readonly string requestBody;

        public AdmAccessToken AccessToken { get; private set; }

        public AdmStsClient(AdmStsSettings settings)
        {
            this.azureDatamarketAccessUri = settings.AzureDatamarketAccessUri;
            this.requestBody = settings.FormatStsRequestBody();
            RefreshAccessToken();
        }

        public async Task<AdmAccessToken> RequestToken()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(
                    azureDatamarketAccessUri, 
                    new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded"));

                if (!response.IsSuccessStatusCode)
                {
                    throw new WpfTranslatorException(
                        $"Failed to get access token. StatusCode: {response.StatusCode}, Response: {await response.Content.ReadAsStringAsync()}.");
                }

                return JsonConvert.DeserializeObject<AdmAccessToken>(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task RefreshAccessToken()
        {
            this.AccessToken = await RequestToken();
        }
    }
}