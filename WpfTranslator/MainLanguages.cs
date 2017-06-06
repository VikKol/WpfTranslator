using System.Collections.Generic;

namespace WpfTranslator
{
    public class MainLanguages
    {
        public static Language DefaultFromLanguage => English;
        public static Language DefaultToLanguage => Ukrainian;

        public static readonly Language English = new Language {Code = "en", Name = "English"};
        public static readonly Language Ukrainian = new Language { Code = "uk", Name = "Ukrainian" };
        public static readonly Language Russian = new Language { Code = "ru", Name = "Russian" };

        public static IEnumerable<Language> All => new [] { English, Ukrainian, Russian };
    }
}
