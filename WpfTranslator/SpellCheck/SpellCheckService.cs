using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WpfTranslator
{
    public class SpellCheckService
    {
        private readonly string key;
        private readonly HttpClient httpClient = new HttpClient();

        public SpellCheckService(string key)
        {
            this.key = key;
        }

        public async Task<SpellCheckResponse> CheckSpellingAsync(string input)
        {
            string url = $"https://montanaflynn-spellcheck.p.mashape.com/check/?text={Uri.EscapeUriString(input)}";
            var msg = new HttpRequestMessage(HttpMethod.Get, url);
            msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            msg.Headers.TryAddWithoutValidation("X-Mashape-Key", key);

            using (msg)
            {
                HttpResponseMessage response = await httpClient.SendAsync(msg);

                if (!response.IsSuccessStatusCode)
                {
                    throw new WpfTranslatorException(
                        $"Request to SpellCheck API failed. StatusCode: {response.StatusCode}, Response: {await response.Content.ReadAsStringAsync()}.");
                }

                return JsonConvert.DeserializeObject<SpellCheckResponse>(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
