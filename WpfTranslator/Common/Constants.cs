using System.Collections.Generic;

namespace WpfTranslator
{
    class Constants
    {
        public const int WmHotKey = 0x0312;
        public const int KeyPressDelayMs = 200;

        public const int SpellCheckHotKeyId = 777;
        public const int TranslateHotKeyId = 888;
        public const int PronounceHotKeyId = 999;
    }

    public class MainLanguages
    {
        public static Language DefaultFromLanguage => English;
        public static Language DefaultToLanguage => Ukrainian;

        public static readonly Language English = new Language { Code = "en", Name = "English" };
        public static readonly Language Ukrainian = new Language { Code = "uk", Name = "Ukrainian" };
        public static readonly Language Russian = new Language { Code = "ru", Name = "Russian" };

        public static IEnumerable<Language> All => new[] { English, Ukrainian, Russian };
    }
}
