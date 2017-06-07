using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfTranslator
{
    public class TranslatorViewModel : INotifyPropertyChanged
    {
        private readonly Translator translator;

        public TranslatorViewModel(Translator translator)
        {
            this.translator = translator;
            this.TranslateCmd = new RelayCommand(_ => { TranslateAsync(); });
            this.PronounceCmd = new RelayCommand(_ => { PronounceAsync(); });
            this.SwapLanguagesCmd = new RelayCommand(_ => SwapLanguages());

            Dictionary<string, string> languages = translator.GetLanguagesAsync().Result;

            foreach (Language mainLanguge in MainLanguages.All)
            {
                SourceLanguages.Add(mainLanguge);
                TargetLanguages.Add(mainLanguge);
                languages.Remove(mainLanguge.Code);
            }

            foreach (KeyValuePair<string, string> kv in languages)
            {
                var lang = new Language {Code = kv.Key, Name = kv.Value};
                SourceLanguages.Add(lang);
                TargetLanguages.Add(lang);
            }

            SourceLanguage = MainLanguages.DefaultFromLanguage;
            TargetLanguage = MainLanguages.DefaultToLanguage;
        }

        #region Properties

        public ObservableCollection<Language> SourceLanguages { get; } = new ObservableCollection<Language>();
        public ObservableCollection<Language> TargetLanguages { get; } = new ObservableCollection<Language>();

        private string _inputText;
        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }

        private string _translatedText;
        public string TranslatedText
        {
            get => _translatedText;
            set
            {
                _translatedText = value;
                OnPropertyChanged(nameof(TranslatedText));
            }
        }

        private Language _sourceLanguage;
        public Language SourceLanguage
        {
            get => _sourceLanguage;
            set
            {
                _sourceLanguage = value;
                OnPropertyChanged(nameof(SourceLanguage));
            }
        }

        private Language _targetLanguage;
        public Language TargetLanguage
        {
            get => _targetLanguage;
            set
            {
                _targetLanguage = value;
                OnPropertyChanged(nameof(TargetLanguage));
            }
        }

        #endregion

        #region Commands

        public ICommand TranslateCmd { get; }
        public ICommand PronounceCmd { get; }
        public ICommand SwapLanguagesCmd { get; }

        #endregion

        public async Task TranslateAsync(string text = null)
        {
            InputText = text ?? InputText;
            TranslatedText = await Helpers.SafeExecute(() => translator.TranslateAsync(InputText, SourceLanguage.Code, TargetLanguage.Code));
        }

        public async Task PronounceAsync(string text = null)
        {
            InputText = text ?? InputText;
            var stream = await Helpers.SafeExecute(() => translator.PronounceAsync(InputText, SourceLanguage.Code));
            using (SoundPlayer player = new SoundPlayer(stream))
            {
                player.Play();
            }
        }

        public void SwapLanguages()
        {
            var tmp = SourceLanguage;
            SourceLanguage = TargetLanguage;
            TargetLanguage = tmp;
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };
        private void OnPropertyChanged(string propertyName) => PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}
