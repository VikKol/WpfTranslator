using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WpfTranslator
{
    class Translator
    {
        private readonly AdmStsClient admStsClient;
        public Translator(AdmStsClient admStsClient)
        {
            this.admStsClient = admStsClient;
        }

        public async Task<string> Translate(string text, Languages from, Languages to)
        {
            return await Task.FromResult("");
        }

        public Stream Spell(string text, Languages from, Languages to)
        {
            return null;
        }
    }
}
