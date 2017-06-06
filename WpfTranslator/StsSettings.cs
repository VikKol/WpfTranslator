using System;

namespace WpfTranslator
{
    class StsSettings
    {
        public string StsUri { get; }
        public string SubscribtionKey { get; }

        public StsSettings(string stsUri, string subscribtionKey)
        {
            if(string.IsNullOrEmpty(stsUri))
                throw new ArgumentException($"{nameof(stsUri)} must not be empty.");
            if (string.IsNullOrEmpty(subscribtionKey))
                throw new ArgumentException($"{nameof(subscribtionKey)} must not be empty.");

            this.StsUri = stsUri;
            this.SubscribtionKey = subscribtionKey;
        }
    }
}
