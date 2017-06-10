using System.Xml.Serialization;

namespace WpfTranslator
{
    [XmlRoot("GetTranslationsResponse", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2")]
    public class TranslationsResponse
    {
        public TranslationMatch[] Translations { get; set; }
    }

    public class TranslationMatch
    {
        public string TranslatedText { get; set; }
    }
}
