using System;

namespace WpfTranslator
{
    class AdmStsSettings
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string scope;

        public string AzureDatamarketAccessUri { get; private set; }

        public AdmStsSettings(string azureDatamarketAccessUri, string clientId, string clientSecret, string scope)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.scope = scope;
            this.AzureDatamarketAccessUri = azureDatamarketAccessUri;
        }

        public string FormatStsRequestBody()
        {
            return "grant_type=client_credentials" +
                $"&client_id={Uri.EscapeDataString(clientId)}" +
                $"&client_secret={Uri.EscapeDataString(clientSecret)}" +
                $"&scope={scope}";
        }
    }
}
