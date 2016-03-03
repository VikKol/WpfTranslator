using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WpfTranslator
{
    class Translator
    {
        private readonly string translatorApiUri;
        private readonly AdmStsClient admStsClient;
        private readonly XmlSerializer xml = new XmlSerializer(typeof(TranslationsResponse));

        public Translator(string translatorApiUri, AdmStsClient admStsClient)
        {
            this.admStsClient = admStsClient;
            this.translatorApiUri = translatorApiUri.EndsWith("/") ? translatorApiUri : translatorApiUri + "/";
        }

        public async Task<string> Translate(string text, string from, string to)
        {
            using (var client = new HttpClient())
            {
                var requiestUrl = translatorApiUri + $"GetTranslations?text={text}&from={from}&to={to}&maxTranslations=5";
                var content = await PerformRequest(client, httpClient => httpClient.PostAsync(requiestUrl, new StringContent(string.Empty)));
                var result = (TranslationsResponse)xml.Deserialize(await content.ReadAsStreamAsync());
                return string.Join(Environment.NewLine, result.Translations.Select(it => it.TranslatedText));
            }
        }

        public async Task<Stream> Pronounce(string text, string language)
        {
            using (var client = new HttpClient())
            {
                var requiestUrl = translatorApiUri + $"Speak?text={text}&language={language}";
                var content = await PerformRequest(client, httpClient => httpClient.GetAsync(requiestUrl));
                return await content.ReadAsStreamAsync();
            }
        }

        private async Task<HttpContent> PerformRequest(HttpClient client, Func<HttpClient, Task<HttpResponseMessage>> request)
        {
            var token = admStsClient.AccessToken.AccessToken;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await request(client);
            if (response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await admStsClient.RefreshAccessToken();
                token = admStsClient.AccessToken.AccessToken;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await request(client);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new WpfTranslatorException(
                    $"Request to API failed. StatusCode: {response.StatusCode}, Response: {await response.Content.ReadAsStringAsync()}.");
            }

            return response.Content;
        }
    }
}
