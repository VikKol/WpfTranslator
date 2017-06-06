using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace WpfTranslator
{
    class Translator
    {
        private readonly string translatorApiUri;
        private readonly StsClient stsClient;
        private readonly HttpClient httpClient = new HttpClient();
        private readonly XmlSerializer xml = new XmlSerializer(typeof(TranslationsResponse));

        public Translator(string translatorApiUri, StsClient stsClient)
        {
            this.stsClient = stsClient;
            this.translatorApiUri = translatorApiUri.EndsWith("/") ? translatorApiUri : translatorApiUri + "/";
        }

        public async Task<string> TranslateAsync(string text, string from, string to)
        {
            string requestUrl = translatorApiUri + $"GetTranslations?text={text}&from={from}&to={to}&maxTranslations=5";
            HttpContent content = await PerformRequestAsync(() => new HttpRequestMessage(HttpMethod.Post, requestUrl));
            var result = (TranslationsResponse)xml.Deserialize(await content.ReadAsStreamAsync());
            return string.Join(Environment.NewLine, result.Translations.Select(it => it.TranslatedText));
        }

        public async Task<Stream> PronounceAsync(string text, string language)
        {
            string requestUrl = translatorApiUri + $"Speak?text={text}&language={language}";
            HttpContent content = await PerformRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, requestUrl));
            return await content.ReadAsStreamAsync();
        }

        public async Task<Dictionary<string, string>> GetLanguagesAsync()
        {
            var dcs = new DataContractSerializer(typeof(string[]));

            HttpContent contentLanCodes = await PerformRequestAsync(() => 
                new HttpRequestMessage(
                    HttpMethod.Get,
                    "http://api.microsofttranslator.com/v2/Http.svc/GetLanguagesForTranslate"));
            var languageCodes = (string[])dcs.ReadObject(await contentLanCodes.ReadAsStreamAsync());

            var stringWriter = new StringWriter();
            var xmlWriter = new XmlTextWriter(stringWriter);
            dcs.WriteObject(xmlWriter, languageCodes);
            
            HttpContent contentLanNames = await PerformRequestAsync(() => 
                new HttpRequestMessage(
                    HttpMethod.Post, 
                    "http://api.microsofttranslator.com/v2/Http.svc/GetLanguageNames?locale=en")
                    {
                        Content = new StringContent(stringWriter.ToString(), Encoding.UTF8, "text/xml")
                    });
            var languageNames = (string[])dcs.ReadObject(await contentLanNames.ReadAsStreamAsync());

            int length = Math.Min(languageCodes.Length, languageNames.Length);
            var languages = new Dictionary<string, string>(length);
            for (int i = 0; i < length; i++)
            {
                languages.Add(languageCodes[i], languageNames[i]);
            }

            return languages;
        }

        private async Task<HttpContent> PerformRequestAsync(Func<HttpRequestMessage> msgFactory)
        {
            string token = stsClient.AccessToken ?? await stsClient.RefreshAccessTokenAsync();
            HttpRequestMessage msg = msgFactory();
            msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await httpClient.SendAsync(msg);
            if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await stsClient.RefreshAccessTokenAsync();
                HttpRequestMessage retryMsg = msgFactory();
                retryMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", stsClient.AccessToken);
                response = await httpClient.SendAsync(retryMsg);
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
