namespace WpfTranslator
{
    enum Languages
    {
        En,
        Uk
    }

    static class LanguagesExtensions
    {
        public static string ToLower(this Languages lang) => lang.ToString().ToLower();
    }
}
