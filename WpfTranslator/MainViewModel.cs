using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WpfTranslator
{
    class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel(Translator translator)
        {
            var taskAwaiter = translator.GetLanguagesAsync().ConfigureAwait(true).GetAwaiter();
            taskAwaiter.OnCompleted(() =>
            {
                Dictionary<string, string> languages = taskAwaiter.GetResult();

                foreach (Language lang in MainLanguages.All)
                {
                    languages.Remove(lang.Code);
                    FromLanguages.Add(lang);
                    ToLanguages.Add(lang);
                }

                foreach (KeyValuePair<string, string> kv in languages)
                {
                    var lang = new Language {Code = kv.Key, Name = kv.Value};
                    FromLanguages.Add(lang);
                    ToLanguages.Add(lang);
                }

                SelectedFromLanguage = MainLanguages.DefaultFromLanguage;
                SelectedToLanguage = MainLanguages.DefaultToLanguage;
            });
        }
        
        public ObservableCollection<Language> FromLanguages { get; } = new ObservableCollection<Language>();
        public ObservableCollection<Language> ToLanguages { get; } = new ObservableCollection<Language>();

        private Language _selectedFromLanguage;
        public Language SelectedFromLanguage
        {
            get { return _selectedFromLanguage; }
            set
            {
                _selectedFromLanguage = value;
                OnPropertyChanged(nameof(SelectedFromLanguage));
            }
        }

        private Language _selectedToLanguage;
        public Language SelectedToLanguage
        {
            get { return _selectedToLanguage; }
            set
            {
                _selectedToLanguage = value;
                OnPropertyChanged(nameof(SelectedToLanguage));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };
        private void OnPropertyChanged(string propertyName) => PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}
