using System.Net.Http;
using System.Threading.Tasks;

namespace WpfTranslator
{
    public class StsClient
    {
        const string OcpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";

        private readonly StsSettings settings;
        private readonly HttpClient httpClient = new HttpClient();

        public string AccessToken { get; private set; }

        public StsClient(StsSettings settings)
        {
            this.settings = settings;
        }

        public async Task<string> RequestTokenAsync()
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, settings.StsUri);
            msg.Headers.TryAddWithoutValidation(OcpApimSubscriptionKeyHeader, settings.SubscribtionKey);
            HttpResponseMessage response = await httpClient.SendAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                throw new WpfTranslatorException(
                    $"Failed to get access token. StatusCode: {response.StatusCode}, Response: {await response.Content.ReadAsStringAsync()}.");
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> RefreshAccessTokenAsync()
        {
            return this.AccessToken = await RequestTokenAsync();
        }
    }
}
